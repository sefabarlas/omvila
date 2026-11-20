import { describe, expect, it, vi } from 'vitest';

import { ApiProblem, createManageClient, idempotencyKey, ifMatch, unwrap } from './client';

const BASE = 'http://api.test';

function problem(status: number, code: string, meta: Record<string, unknown> = {}): Response {
  return new Response(
    JSON.stringify({
      type: `https://docs.omvila.com/errors/${code}`,
      title: code,
      status,
      code,
      meta,
    }),
    { status, headers: { 'Content-Type': 'application/problem+json' } },
  );
}

function json(body: unknown, status = 200): Response {
  return new Response(JSON.stringify(body), {
    status,
    headers: { 'Content-Type': 'application/json' },
  });
}

function recorder(...responses: Response[]) {
  const requests: Request[] = [];
  const fetch = vi.fn((request: Request) => {
    requests.push(request);
    const next = responses.shift();
    if (!next) throw new Error('beklenmeyen istek');
    return Promise.resolve(next);
  });
  return { fetch, requests };
}

describe('createManageClient', () => {
  it('her isteğe güncel erişim jetonunu ve sabit başlıkları ekler', async () => {
    const { fetch, requests } = recorder(json({ ok: true }));
    const client = createManageClient({
      baseUrl: BASE,
      fetch,
      getAccessToken: () => 'jeton-1',
      headers: { 'X-Client-Type': 'web' },
    });

    await client.GET('/api/v1/manage/session');

    expect(requests[0]?.headers.get('Authorization')).toBe('Bearer jeton-1');
    expect(requests[0]?.headers.get('X-Client-Type')).toBe('web');
    expect(requests[0]?.url).toBe(`${BASE}/api/v1/manage/session`);
  });

  it('POST isteğine eksikse Idempotency-Key ekler, verileni korur', async () => {
    const { fetch, requests } = recorder(json({}, 201), json({}, 201));
    const client = createManageClient({
      baseUrl: BASE,
      fetch,
      newIdempotencyKey: () => 'uretilen',
    });

    await client.POST('/api/v1/manage/onboarding/events', { body: { event: 'BookingLinkCopied' } });
    await client.POST('/api/v1/manage/branches', {
      params: { header: { 'Idempotency-Key': 'verilen' } },
      body: {
        name: 'Nişantaşı',
        addressLine: 'Abdi İpekçi Cad. 10',
        district: 'Şişli',
        city: 'İstanbul',
      },
    });

    expect(requests[0]?.headers.get('Idempotency-Key')).toBe('uretilen');
    expect(requests[1]?.headers.get('Idempotency-Key')).toBe('verilen');
  });

  it('GET isteğine Idempotency-Key eklemez', async () => {
    const { fetch, requests } = recorder(json({}));
    const client = createManageClient({ baseUrl: BASE, fetch });

    await client.GET('/api/v1/manage/session');

    expect(requests[0]?.headers.has('Idempotency-Key')).toBe(false);
  });

  it('401 token_expired sonrası bir kez yeniler ve aynı anahtarla, yeni jetonla tekrar dener', async () => {
    const { fetch, requests } = recorder(problem(401, 'token_expired'), json({}, 201));
    let token = 'eski';
    const refreshAccessToken = vi.fn(() => {
      token = 'yeni';
      return Promise.resolve(true);
    });
    const client = createManageClient({
      baseUrl: BASE,
      fetch,
      getAccessToken: () => token,
      refreshAccessToken,
    });

    const result = await client.POST('/api/v1/manage/onboarding/events', {
      body: { event: 'QrDownloaded' },
    });

    expect(result.response.status).toBe(201);
    expect(refreshAccessToken).toHaveBeenCalledTimes(1);
    expect(requests).toHaveLength(2);
    expect(requests[1]?.headers.get('Authorization')).toBe('Bearer yeni');
    expect(requests[1]?.headers.get('Idempotency-Key')).toBe(
      requests[0]?.headers.get('Idempotency-Key'),
    );
    expect(await requests[1]?.json()).toEqual({ event: 'QrDownloaded' });
  });

  it('yenileme başarısızsa veya kod token_expired değilse tekrar denemez', async () => {
    const { fetch, requests } = recorder(
      problem(401, 'token_expired'),
      problem(401, 'unauthorized'),
    );
    const refreshAccessToken = vi.fn(() => Promise.resolve(false));
    const client = createManageClient({ baseUrl: BASE, fetch, refreshAccessToken });

    const first = await client.GET('/api/v1/manage/session');
    const second = await client.GET('/api/v1/manage/session');

    expect(first.response.status).toBe(401);
    expect(second.response.status).toBe(401);
    expect(refreshAccessToken).toHaveBeenCalledTimes(1);
    expect(requests).toHaveLength(2);
  });

  it('eşzamanlı 401 yanıtları tek yenilemeyi paylaşır', async () => {
    const { fetch } = recorder(
      problem(401, 'token_expired'),
      problem(401, 'token_expired'),
      json({}),
      json({}),
    );
    let resolveRefresh: (value: boolean) => void = () => undefined;
    const refreshAccessToken = vi.fn(
      () =>
        new Promise<boolean>((resolve) => {
          resolveRefresh = resolve;
        }),
    );
    const client = createManageClient({ baseUrl: BASE, fetch, refreshAccessToken });

    const calls = Promise.all([
      client.GET('/api/v1/manage/session'),
      client.GET('/api/v1/manage/onboarding'),
    ]);
    await vi.waitFor(() => {
      expect(refreshAccessToken).toHaveBeenCalled();
    });
    resolveRefresh(true);
    const results = await calls;

    expect(refreshAccessToken).toHaveBeenCalledTimes(1);
    expect(results.map((r) => r.response.status)).toEqual([200, 200]);
  });

  it('503 try_again yanıtında bir kez tekrar dener', async () => {
    const { fetch, requests } = recorder(problem(503, 'try_again'), problem(503, 'try_again'));
    const client = createManageClient({ baseUrl: BASE, fetch });

    const result = await client.GET('/api/v1/manage/session');

    expect(result.response.status).toBe(503);
    expect(requests).toHaveLength(2);
  });
});

describe('unwrap', () => {
  it('başarılı yanıtta veriyi döner', async () => {
    const { fetch } = recorder(json({ completedCount: 1, totalCount: 5, steps: [] }));
    const client = createManageClient({ baseUrl: BASE, fetch });

    const data = await unwrap(client.GET('/api/v1/manage/onboarding'));

    expect(data.completedCount).toBe(1);
  });

  it('hata yanıtında code, status ve meta taşıyan ApiProblem fırlatır', async () => {
    const { fetch } = recorder(problem(422, 'otp_invalid', { attemptsLeft: 3 }));
    const client = createManageClient({ baseUrl: BASE, fetch });

    const call = unwrap(
      client.POST('/api/v1/manage/auth/otp/verify', {
        body: { otpId: '0193a8f0-0000-7000-8000-000000000001', code: '482193' },
      }),
    );

    await expect(call).rejects.toBeInstanceOf(ApiProblem);
    await call.catch((error: unknown) => {
      const problemError = error as ApiProblem;
      expect(problemError.code).toBe('otp_invalid');
      expect(problemError.status).toBe(422);
      expect(problemError.meta).toEqual({ attemptsLeft: 3 });
    });
  });

  it('gövdesi problem olmayan hatada genel kod verir', async () => {
    const { fetch } = recorder(new Response('Bad Gateway', { status: 502 }));
    const client = createManageClient({ baseUrl: BASE, fetch });

    const error = await unwrap(client.GET('/api/v1/manage/session')).catch((e: unknown) => e);

    expect(error).toBeInstanceOf(ApiProblem);
    expect((error as ApiProblem).code).toBe('try_again');
  });
});

describe('tipler', () => {
  it('sarmalayıcının doldurduğu başlıklar isteğe bağlı, If-Match zorunlu', () => {
    const client = createManageClient({ baseUrl: BASE, fetch: recorder().fetch });
    // Çağrılmaz; yalnız tip denetimi (tsc) için.
    const checks = () => [
      // X-Client-Type ve Idempotency-Key verilmeden çağrılabilir
      client.POST('/api/v1/manage/auth/otp/verify', {
        body: { otpId: '0193a8f0-0000-7000-8000-000000000001', code: '482193' },
      }),
      client.POST('/api/v1/manage/branches', {
        body: {
          name: 'Nişantaşı',
          addressLine: 'Abdi İpekçi Cad. 10',
          district: 'Şişli',
          city: 'İstanbul',
        },
      }),
      // @ts-expect-error If-Match verilmeden düzenleme yapılamaz
      client.PATCH('/api/v1/manage/brand', { body: { name: 'Kuaför Elif' } }),
      client.PATCH('/api/v1/manage/brand', {
        params: { header: ifMatch('W/"8812"') },
        body: { name: 'Kuaför Elif' },
      }),
    ];
    expect(typeof checks).toBe('function');
  });
});

describe('yardımcılar', () => {
  it('ifMatch sürümü If-Match başlığına koyar', () => {
    expect(ifMatch('W/"8812"')).toEqual({ 'If-Match': 'W/"8812"' });
  });

  it('idempotencyKey her çağrıda farklı UUID üretir', () => {
    const a = idempotencyKey()['Idempotency-Key'];
    const b = idempotencyKey()['Idempotency-Key'];
    expect(a).toMatch(/^[0-9a-f-]{36}$/);
    expect(a).not.toBe(b);
  });
});
