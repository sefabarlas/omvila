import { fixedClock } from '@omvila/ui-web/time';
import { render, screen, waitFor, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { beforeEach, describe, expect, it } from 'vitest';
import { brandFixture, json, problem } from '@/test/api-mock';
import { depositExpiredFixture } from '@/test/appointment-fixtures';
import {
  appointmentFixture,
  catalogFixture,
  depositCatalogFixture,
  depositFixture,
  ids,
  mockBooking,
  nowUtc,
  tokenFixture,
} from '@/test/booking-mock';
import type { Recorded } from '@/test/booking-mock';
import type { PublicBrand } from '@/lib/brand';
import { BookingFlow } from './BookingFlow';

const clock = fixedClock(nowUtc);

function renderFlow(search = '', brand: PublicBrand = brandFixture()) {
  window.history.replaceState(null, '', `/kuafor-elif${search}`);
  const user = userEvent.setup();
  render(<BookingFlow brand={brand} initialSearch={search} clock={clock} />);
  return user;
}

const lastCall = (calls: Recorded[], suffix: string) =>
  calls.filter((call) => call.path.endsWith(suffix)).at(-1);

const footerButton = () => screen.getByRole('button', { name: 'Devam et' });
/** Alt çubuk (masaüstü özet kartında aynı tutarlar da yazar). */
function footer() {
  const element = footerButton().closest<HTMLElement>('.footer');
  if (!element) throw new Error('alt çubuk yok');
  return within(element);
}

/** Saç kesimi + Fön, Elif, 24 Kas 14:30: doğrudan bilgiler adımı (adres çubuğundan). */
const detailsSearch = (staff: string = ids.elif) =>
  `?step=details&services=${ids.sac},${ids.fon}&staff=${staff}&date=2026-11-24&time=14:30`;

async function fillDetails(user: ReturnType<typeof userEvent.setup>) {
  await user.type(await screen.findByLabelText('Ad soyad'), 'Ayşe Yılmaz');
  await user.type(screen.getByLabelText('Telefon numaranız'), '532 123 45 67');
  await user.click(screen.getByRole('checkbox', { name: /Aydınlatma metnini/ }));
  await user.click(screen.getByRole('button', { name: 'Doğrulama kodu gönder' }));
}

beforeEach(() => {
  window.history.replaceState(null, '', '/kuafor-elif');
});

describe('online randevu akışı', () => {
  it('hizmet → personel → saat → telefon → kod → onay → başarı (kriter 1)', async () => {
    const calls = mockBooking();
    const user = renderFlow();

    await user.click(await screen.findByRole('checkbox', { name: 'Saç kesimi' }));
    await user.click(screen.getByRole('checkbox', { name: 'Fön' }));
    expect(footer().getByText('700 ₺')).toBeTruthy();
    expect(footer().getByText('2 hizmet · 1 sa 15 dk')).toBeTruthy();
    expect(window.location.search).toBe(`?services=${ids.sac}%2C${ids.fon}`);
    await user.click(footerButton());

    expect(await screen.findByRole('heading', { name: 'Personel seçin' })).toBeTruthy();
    expect(await screen.findByRole('switch', { name: 'İki hizmet de aynı kişiden' })).toBeTruthy();
    expect(footerButton()).toHaveProperty('disabled', true);
    await user.click(screen.getByRole('radio', { name: /Elif Kaya/ }));
    // Elif'in Saç kesimi ek ücreti toplamda.
    expect(footer().getByText('750 ₺')).toBeTruthy();
    await user.click(footerButton());

    expect(await screen.findByRole('heading', { name: 'Gün ve saat' })).toBeTruthy();
    expect(await screen.findByText('Elif Kaya için uygun saatler')).toBeTruthy();
    const days = lastCall(calls, '/availability/days');
    expect(days?.body).toEqual({
      items: [
        { serviceId: ids.sac, staffId: ids.elif },
        { serviceId: ids.fon, staffId: ids.elif },
      ],
      sameStaffForAll: true,
      from: '2026-11-24',
      to: '2026-12-24',
    });
    expect(screen.getByText('60 gün ileriye kadar')).toBeTruthy();
    await user.click(await screen.findByRole('radio', { name: '14:30' }));
    expect(footer().getByText('Sal 24 Kas · 14:30–15:45')).toBeTruthy();
    expect(screen.getByRole('radio', { name: '13:00, dolu' })).toHaveProperty('disabled', true);
    await user.click(footerButton());

    expect(await screen.findByRole('heading', { name: 'Bilgileriniz' })).toBeTruthy();
    expect(await screen.findByText('Salı, 24 Kasım · 14:30–15:45')).toBeTruthy();
    await fillDetails(user);
    const otp = lastCall(calls, '/otp/request');
    expect(otp?.body).toEqual({ phone: '+905321234567', branchId: brandFixture().branches[0]?.id });
    expect(otp?.request.credentials).toBe('include');
    expect(otp?.request.headers.get('X-Client')).toBe('booking-page');

    expect(
      await screen.findByRole('heading', { name: 'Telefonunuza gelen kodu girin' }),
    ).toBeTruthy();
    expect(screen.getByText(/Kod gelmedi mi\? 1:00 sonra/)).toBeTruthy();
    await user.type(screen.getByRole('textbox', { name: 'Doğrulama kodu, hane 1' }), '482193');
    const verify = await waitFor(() => {
      const call = lastCall(calls, '/otp/verify');
      if (!call) throw new Error('doğrulama yok');
      return call;
    });
    expect(verify.body).toEqual({
      otpId: '0190f000-0000-7000-8000-000000000001',
      code: '482193',
      rememberDevice: true,
    });
    expect(verify.request.credentials).toBe('include');

    expect(await screen.findByRole('heading', { name: 'Son kontrol' })).toBeTruthy();
    const summary = await screen.findByRole('region', { name: 'Randevu özeti' });
    expect(within(summary).getByText('Saç kesimi · Elif Kaya')).toBeTruthy();
    expect(within(summary).getByText('500 ₺')).toBeTruthy();
    expect(within(summary).getByText('750 ₺')).toBeTruthy();
    await user.click(screen.getByRole('button', { name: 'Randevuyu oluştur' }));

    expect(await screen.findByRole('heading', { name: 'Randevunuz onaylandı' })).toBeTruthy();
    expect(screen.getByText('Onay mesajını 0532 *** ** 67 numarasına gönderdik.')).toBeTruthy();
    expect(screen.getByRole('link', { name: 'randevu sayfanızdan' }).getAttribute('href')).toBe(
      '/r/7Kx2Pq9a',
    );
    expect(window.location.search).toBe('');

    const create = lastCall(calls, '/appointments');
    expect(create?.body).toEqual({
      items: [
        { serviceId: ids.sac, staffId: ids.elif },
        { serviceId: ids.fon, staffId: ids.elif },
      ],
      sameStaffForAll: true,
      startUtc: '2026-11-24T11:30:00Z',
      fullName: 'Ayşe Yılmaz',
      customerNote: null,
      privacyNoticeAcknowledged: true,
      privacyNoticeVersion: 'musteri-v1',
      marketingConsent: false,
    });
    expect(create?.request.headers.get('Authorization')).toBe('Bearer rezervasyon-jetonu');
    expect(create?.request.headers.get('Idempotency-Key')).toMatch(/^[0-9a-f-]{36}$/);
    expect(create?.request.credentials).not.toBe('include');
  });

  it('onay gerektiren salonda talep ekranı: "Onay bekliyor", kalan yanıt süresi (Dilim 3 kriter 1)', async () => {
    mockBooking({
      routes: [
        (request) =>
          new URL(request.url).pathname.endsWith('/appointments')
            ? json(
                201,
                appointmentFixture({
                  status: 'PendingApproval',
                  displayStatus: 'PendingApproval',
                  holdExpiresUtc: '2026-11-24T09:00:00Z',
                }),
              )
            : undefined,
      ],
    });
    const user = renderFlow(detailsSearch());
    await fillDetails(user);
    await user.type(
      await screen.findByRole('textbox', { name: 'Doğrulama kodu, hane 1' }),
      '482193',
    );
    await user.click(await screen.findByRole('button', { name: 'Randevuyu oluştur' }));

    expect(await screen.findByRole('heading', { name: 'Talebiniz alındı' })).toBeTruthy();
    expect(
      screen.getByText(
        'Kuaför Elif en geç 2 saat içinde yanıtlar. Onaylandığında 0532 *** ** 67 numarasına SMS göndereceğiz.',
      ),
    ).toBeTruthy();
    const card = within(screen.getByRole('region', { name: 'Talep özeti' }));
    expect(card.getByText('Onay bekliyor')).toBeTruthy();
    expect(card.getByText('Saç kesimi, Fön · Elif Kaya')).toBeTruthy();
    expect(card.getByText('24 Kas Sal')).toBeTruthy();
    expect(card.getByText('14:30')).toBeTruthy();
    expect(card.getByText('1 sa 15 dk')).toBeTruthy();
    expect(card.getByText('750 ₺')).toBeTruthy();
    expect(screen.getByText(/Saat sizin için ayrıldı\./)).toBeTruthy();
    expect(screen.getByRole('link', { name: 'Salon sayfasına dön' }).getAttribute('href')).toBe(
      '/kuafor-elif',
    );
    expect(screen.queryByRole('heading', { name: 'Randevunuz onaylandı' })).toBeNull();
    expect(document.body.textContent).not.toMatch(/redd/i);
  });

  it('“Fark etmez”de atanan personelin adı onayda görünür ve o kişi gönderilir (kriter 6)', async () => {
    const calls = mockBooking({
      routes: [
        (request) =>
          request.url.endsWith('/otp/request')
            ? json(200, { status: 'AlreadyVerified', booking: tokenFixture })
            : undefined,
      ],
    });
    const user = renderFlow(`?step=staff&services=${ids.sac}`);

    await user.click(await screen.findByRole('radio', { name: /Fark etmez/ }));
    await user.click(footerButton());
    await user.click(await screen.findByRole('radio', { name: '14:30' }));
    expect(lastCall(calls, '/availability/slots')?.body).toMatchObject({
      items: [{ serviceId: ids.sac, staffId: null }],
    });
    await user.click(footerButton());
    await fillDetails(user);

    const summary = await screen.findByRole('region', { name: 'Randevu özeti' });
    expect(within(summary).getByText('Saç kesimi · Zeynep Aksoy')).toBeTruthy();
    await user.click(screen.getByRole('button', { name: 'Randevuyu oluştur' }));
    await screen.findByRole('heading', { name: 'Randevunuz onaylandı' });
    expect(lastCall(calls, '/appointments')?.body).toMatchObject({
      items: [{ serviceId: ids.sac, staffId: ids.zeynep }],
    });
  });

  it('dolu gün en yakın uygun saate geçer (kriter 7)', async () => {
    mockBooking();
    const user = renderFlow(`?step=time&services=${ids.sac}&staff=${ids.elif}`);

    const thursday = await screen.findByRole('radio', {
      name: '26 Kasım Perşembe, boş saat yok',
    });
    await user.click(thursday);
    expect(screen.getByRole('heading', { name: 'Perşembe dolu' })).toBeTruthy();
    expect(
      screen.getByText('26 Kasım için boş saat yok. En yakın uygun saat Cuma 10:30.'),
    ).toBeTruthy();
    await user.click(screen.getByRole('button', { name: "Cuma 10:30'u seç" }));

    expect(screen.getByRole('radio', { name: '27 Kasım Cuma' }).getAttribute('aria-checked')).toBe(
      'true',
    );
    await waitFor(() => {
      expect(screen.getByRole('radio', { name: '10:30' }).getAttribute('aria-checked')).toBe(
        'true',
      );
    });
    expect(window.location.search).toContain('date=2026-11-27&time=10%3A30');
  });

  it('kapalı gün seçilemez, en ileri tarihten sonrası şeritte yok (kriter 8, 16)', async () => {
    mockBooking();
    renderFlow(`?step=time&services=${ids.sac}&staff=${ids.elif}`);

    const sunday = await screen.findByRole('radio', { name: '29 Kasım Pazar, kapalı' });
    expect(sunday).toHaveProperty('disabled', true);
    expect(screen.getByRole('radio', { name: '24 Kasım Salı, bugün' })).toBeTruthy();
    expect(screen.queryByRole('radio', { name: /25 Ocak/ })).toBeNull();
  });

  it('güvenilir cihazda kod adımı atlanır ve müşteri adıyla karşılanır (kriter 9)', async () => {
    const calls = mockBooking({
      routes: [
        (request) =>
          request.url.endsWith('/otp/request')
            ? json(200, {
                status: 'AlreadyVerified',
                booking: { ...tokenFixture, knownCustomerFirstName: 'Ayşe' },
              })
            : undefined,
      ],
    });
    const user = renderFlow(detailsSearch());

    await fillDetails(user);

    expect(await screen.findByRole('heading', { name: 'Son kontrol' })).toBeTruthy();
    expect(screen.getByText(/Tekrar hoş geldiniz,/).textContent).toBe(
      'Tekrar hoş geldiniz, Ayşe. Randevunuzu onaylamadan önce son bir kez bakın.',
    );
    expect(screen.queryByRole('heading', { name: 'Telefonunuza gelen kodu girin' })).toBeNull();
    expect(lastCall(calls, '/otp/verify')).toBeUndefined();
  });

  it('saat az önce dolduysa alternatif saatle yeniden dener', async () => {
    let attempts = 0;
    const calls = mockBooking({
      routes: [
        (request) =>
          request.url.endsWith('/otp/request')
            ? json(200, { status: 'AlreadyVerified', booking: tokenFixture })
            : undefined,
        (request) => {
          if (!request.url.endsWith('/appointments') || attempts++ > 0) return undefined;
          return json(409, {
            type: 'about:blank',
            title: 'slot_taken',
            status: 409,
            code: 'slot_taken',
            meta: { alternatives: ['2026-11-24T11:45:00Z', '2026-11-24T12:15:00Z'] },
          });
        },
      ],
    });
    const user = renderFlow(detailsSearch());
    await fillDetails(user);
    await user.click(await screen.findByRole('button', { name: 'Randevuyu oluştur' }));

    const alert = await screen.findByRole('alert');
    expect(alert.textContent).toContain('14:30 artık boş değil.');
    expect(alert.textContent).toContain('En yakın boş saatler 14:45 ve 15:15.');
    const slotCalls = calls.filter((call) => call.path.endsWith('/availability/slots')).length;
    await user.click(within(alert).getByRole('button', { name: '14:45' }));

    await waitFor(() => {
      expect(screen.getByText(/Salı, 24 Kasım · 14:45/)).toBeTruthy();
    });
    expect(calls.filter((call) => call.path.endsWith('/availability/slots')).length).toBe(
      slotCalls + 1,
    );
    const keys = calls
      .filter((call) => call.path.endsWith('/appointments'))
      .map((call) => call.request.headers.get('Idempotency-Key'));
    await user.click(screen.getByRole('button', { name: 'Randevuyu oluştur' }));
    await screen.findByRole('heading', { name: 'Randevunuz onaylandı' });
    const creates = calls.filter((call) => call.path.endsWith('/appointments'));
    expect(creates.at(-1)?.body).toMatchObject({ startUtc: '2026-11-24T11:45:00Z' });
    expect(creates.at(-1)?.request.headers.get('Idempotency-Key')).not.toBe(keys[0]);
  });

  it('aydınlatma metni onayı zorunlu, telefon biçimi denetlenir', async () => {
    const calls = mockBooking();
    const user = renderFlow(detailsSearch());

    await user.click(await screen.findByRole('button', { name: 'Doğrulama kodu gönder' }));
    expect(screen.getByText('Adınızı ve soyadınızı yazın.')).toBeTruthy();
    expect(
      screen.getByText('Cep telefonu numaranızı 0532 123 45 67 biçiminde yazın.'),
    ).toBeTruthy();
    expect(screen.getByText('Devam etmek için aydınlatma metnini onaylayın.')).toBeTruthy();

    await user.type(screen.getByLabelText('Ad soyad'), 'Ayşe Yılmaz');
    await user.type(screen.getByLabelText('Telefon numaranız'), '212 123 45 67');
    await user.click(screen.getByRole('button', { name: 'Doğrulama kodu gönder' }));
    expect(screen.getByLabelText('Telefon numaranız').getAttribute('aria-invalid')).toBe('true');
    expect(screen.getByRole('checkbox', { name: /Aydınlatma metnini/ })).toHaveProperty(
      'checked',
      false,
    );
    expect(lastCall(calls, '/otp/request')).toBeUndefined();
  });

  it('kod hatalıysa kalan deneme hakkını söyler', async () => {
    mockBooking({
      routes: [
        (request) =>
          request.url.endsWith('/otp/verify')
            ? json(422, {
                type: 'about:blank',
                title: 'otp_invalid',
                status: 422,
                code: 'otp_invalid',
                meta: { attemptsLeft: 2 },
              })
            : undefined,
      ],
    });
    const user = renderFlow(detailsSearch());
    await fillDetails(user);
    await user.type(
      await screen.findByRole('textbox', { name: 'Doğrulama kodu, hane 1' }),
      '111111',
    );
    expect((await screen.findByRole('alert')).textContent).toBe(
      'Kod hatalı. 2 deneme hakkınız kaldı.',
    );
  });

  it('fiyat gösterimi kapalıysa tutar yazılmaz', async () => {
    const catalog = catalogFixture();
    mockBooking({
      catalog: {
        ...catalog,
        services: catalog.services.map((service) => ({ ...service, price: null })),
        staff: catalog.staff.map((member) => ({ ...member, surcharges: [] })),
      },
    });
    const user = renderFlow('', brandFixture({ showPrices: false }));

    await user.click(await screen.findByRole('checkbox', { name: 'Saç kesimi' }));
    expect(footer().getByText('1 hizmet · 45 dk')).toBeTruthy();
    expect(document.body.textContent).not.toContain('₺');
  });

  it('personel seçimi kapalıysa personel adımı atlanır', async () => {
    const calls = mockBooking({ catalog: catalogFixture({ allowStaffSelection: false }) });
    const user = renderFlow();

    await user.click(await screen.findByRole('checkbox', { name: 'Saç kesimi' }));
    await user.click(footerButton());
    expect(await screen.findByRole('heading', { name: 'Gün ve saat' })).toBeTruthy();
    expect(screen.getByRole('navigation', { name: /adım 2 \/ 3/ })).toBeTruthy();
    await waitFor(() => {
      expect(lastCall(calls, '/availability/days')?.body).toMatchObject({
        items: [{ serviceId: ids.sac, staffId: null }],
      });
    });
  });

  it('birden fazla şubede önce şube seçilir', async () => {
    const [branch] = brandFixture().branches;
    if (!branch) throw new Error('şube yok');
    const second = { ...branch, id: '0190a000-0000-7000-8000-000000000009', name: 'Nişantaşı' };
    const calls = mockBooking();
    const user = renderFlow('', brandFixture({ branches: [branch, second] }));

    expect(screen.getByRole('heading', { name: 'Hangi şubeye gelmek istersiniz?' })).toBeTruthy();
    await user.click(screen.getByRole('button', { name: /Nişantaşı/ }));
    expect(await screen.findByRole('checkbox', { name: 'Saç kesimi' })).toBeTruthy();
    expect(calls[0]?.path).toBe(`/api/v1/public/branches/${second.id}/catalog`);
    expect(window.location.search).toBe(`?branch=${second.id}`);
  });

  it('online randevu kapandıysa akış durur (kriter 17)', async () => {
    mockBooking({
      routes: [
        (request) =>
          request.url.endsWith('/catalog') ? problem(403, 'booking_disabled') : undefined,
      ],
    });
    renderFlow();
    expect((await screen.findByRole('status')).textContent).toContain(
      'Kuaför Elif şu an online randevu almıyor.',
    );
  });

  it('kaporasız hizmette kapora kutusu ve satırı yok', async () => {
    mockBooking();
    const user = renderFlow(detailsSearch());
    await fillDetails(user);
    await user.type(
      await screen.findByRole('textbox', { name: 'Doğrulama kodu, hane 1' }),
      '482193',
    );
    expect(await screen.findByRole('button', { name: 'Randevuyu oluştur' })).toBeTruthy();
    expect(screen.queryByText('Kapora (havale ile)')).toBeNull();
    expect(screen.queryByRole('region', { name: 'Kapora' })).toBeNull();
    expect(screen.getByText('Ödeme sayfadan alınmaz; ücreti salonda ödersiniz.')).toBeTruthy();
  });

  it('adres çubuğundaki seçimle kaldığı adımdan açılır, geçersiz seçim geri çekilir', async () => {
    mockBooking();
    renderFlow(`?step=time&services=${ids.bakim}&staff=${ids.elif}`);
    // Elif Saç bakımı vermiyor: personel adımına döner.
    expect(await screen.findByRole('heading', { name: 'Personel seçin' })).toBeTruthy();
  });
});

describe('kapora havalesi (Dilim 4)', () => {
  const isPath = (request: Request, suffix: string) =>
    new URL(request.url).pathname.endsWith(suffix);

  async function reachConfirm(user: ReturnType<typeof userEvent.setup>) {
    await fillDetails(user);
    await user.type(
      await screen.findByRole('textbox', { name: 'Doğrulama kodu, hane 1' }),
      '482193',
    );
  }

  it('hizmet listesinde kapora ipucu', async () => {
    mockBooking({ catalog: depositCatalogFixture() });
    renderFlow();
    expect(await screen.findByRole('checkbox', { name: 'Saç kesimi' })).toBeTruthy();
    expect(screen.getByText('45 dk · Kapora 150 ₺')).toBeTruthy();
  });

  it('onayda kapora kutusu ve satırı; oluşturunca Havale, bildirince "Salon onayı bekleniyor" (KK3, KK4, S-03)', async () => {
    const calls = mockBooking({
      catalog: depositCatalogFixture(),
      routes: [
        (request) =>
          isPath(request, '/appointments') ? json(201, depositFixture('Awaiting')) : undefined,
        (request) =>
          isPath(request, '/deposit/report')
            ? json(200, depositFixture('CustomerReported'), { ETag: 'W/"2"' })
            : undefined,
      ],
    });
    const user = renderFlow(detailsSearch());
    await reachConfirm(user);

    const summary = within(await screen.findByRole('region', { name: 'Randevu özeti' }));
    expect(summary.getByText('Toplam · 1 sa 15 dk')).toBeTruthy();
    expect(summary.getByText('750 ₺')).toBeTruthy();
    expect(summary.getByText('Kapora (havale ile)')).toBeTruthy();
    expect(summary.getByText('−150 ₺')).toBeTruthy();
    expect(summary.getByText('Salonda ödenecek')).toBeTruthy();
    expect(summary.getByText('600 ₺')).toBeTruthy();
    const box = within(screen.getByRole('region', { name: 'Kapora' }));
    expect(box.getByText('150 ₺')).toBeTruthy();
    expect(box.getByText(/^Bu randevu 150 ₺ kapora ile kesinleşir\./)).toBeTruthy();
    expect(
      screen.getByText('Ödeme sayfadan alınmaz; havaleyi kendi bankanızdan yaparsınız.'),
    ).toBeTruthy();
    await user.click(screen.getByRole('button', { name: 'Randevuyu oluştur' }));

    expect(
      await screen.findByRole('heading', { name: 'Kaporayı havale ile gönderin' }),
    ).toBeTruthy();
    expect(screen.queryByRole('heading', { name: 'Randevunuz onaylandı' })).toBeNull();
    const hero = within(screen.getByRole('region', { name: 'Kapora bekleniyor' }));
    expect(hero.getByText('1 sa 58 dk kaldı')).toBeTruthy();
    expect(hero.getByText('150 ₺')).toBeTruthy();
    expect(hero.getByText('Sal 24 Kas · 14:30 · Kuaför Elif')).toBeTruthy();
    expect(screen.getByText('Elif Kaya')).toBeTruthy();
    expect(screen.getByText('TR12 0006 4000 0011 2345 6789 01')).toBeTruthy();
    expect(screen.getByText('OMV-7K2X')).toBeTruthy();
    expect(
      screen.getByText(/Kapora 11:58'e kadar gelmezse randevu otomatik iptal olur/),
    ).toBeTruthy();
    expect(screen.getByText('Bu bilgiler SMS ile de gönderildi.')).toBeTruthy();

    // IBAN boşluksuz, tutar sembolsüz kopyalanır.
    await user.click(screen.getByRole('button', { name: 'Kopyala IBAN' }));
    expect(await navigator.clipboard.readText()).toBe('TR120006400000112345678901');
    expect(screen.getByRole('button', { name: 'Kopyalandı IBAN' })).toBeTruthy();
    await user.click(screen.getByRole('button', { name: 'Kopyala Tutar' }));
    expect(await navigator.clipboard.readText()).toBe('150');

    await user.click(screen.getByRole('button', { name: 'Havaleyi yaptım, salona bildir' }));
    expect(await screen.findByRole('heading', { name: 'Salon onayı bekleniyor' })).toBeTruthy();
    expect(
      calls.filter((call) => call.path === '/api/v1/public/appointments/7Kx2Pq9a/deposit/report'),
    ).toHaveLength(1);
    expect(screen.queryByRole('button', { name: 'Havaleyi yaptım, salona bildir' })).toBeNull();
    expect(document.body.textContent).not.toContain('onaylandı');
  });

  it('bildirimde süre dolmuşsa (hold_expired) randevu yeniden okunur: "Kapora gelmedi"', async () => {
    mockBooking({
      catalog: depositCatalogFixture(),
      routes: [
        (request) =>
          isPath(request, '/appointments') ? json(201, depositFixture('Awaiting')) : undefined,
        (request) =>
          isPath(request, '/deposit/report') ? problem(409, 'hold_expired') : undefined,
        (request) =>
          isPath(request, '/appointments/7Kx2Pq9a')
            ? json(200, depositExpiredFixture(), { ETag: 'W/"3"' })
            : undefined,
      ],
    });
    const user = renderFlow(detailsSearch());
    await reachConfirm(user);
    await user.click(await screen.findByRole('button', { name: 'Randevuyu oluştur' }));
    await user.click(await screen.findByRole('button', { name: 'Havaleyi yaptım, salona bildir' }));

    expect(await screen.findByText('Kapora süresinde gelmedi, randevu iptal edildi.')).toBeTruthy();
    expect(screen.getByRole('link', { name: 'Salonu ara' }).getAttribute('href')).toBe(
      'tel:+905321234567',
    );
  });
});
