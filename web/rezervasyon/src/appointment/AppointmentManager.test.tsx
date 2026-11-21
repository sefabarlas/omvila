import { fixedClock } from '@omvila/ui-web/time';
import { render, screen, waitFor, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import { json } from '@/test/api-mock';
import {
  accessCode,
  cancelledFixture,
  etag,
  linkAppointmentFixture,
  pendingFixture,
  requestCancelledFixture,
  rescheduledFixture,
  rescheduleSlotsFixture,
} from '@/test/appointment-fixtures';
import { nowUtc } from '@/test/booking-fixtures';
import type { PublicAppointment } from './appointment-api';
import { AppointmentManager } from './AppointmentManager';
import type { Action } from './action';

const clock = fixedClock(nowUtc);
const base = `/api/v1/public/appointments/${accessCode}`;

interface Call {
  method: string;
  path: string;
  body: unknown;
  ifMatch: string | null;
}

const problemResponse = (status: number, code: string, meta: Record<string, unknown> = {}) =>
  new Response(JSON.stringify({ type: 'about:blank', title: code, status, code, meta }), {
    status,
    headers: { 'Content-Type': 'application/problem+json' },
  });

type Route = (call: Call) => Response | undefined;

/** GET (ETag'li), saat araması, iptal, saat değişikliği. `routes` ilk eşleşen yanıtı verir. */
function mockLink(options: { appointment?: () => PublicAppointment; routes?: Route[] } = {}) {
  const calls: Call[] = [];
  const appointment = options.appointment ?? (() => linkAppointmentFixture());
  vi.stubGlobal(
    'fetch',
    vi.fn(async (request: Request) => {
      const path = new URL(request.url).pathname;
      const text = request.method === 'GET' ? '' : await request.clone().text();
      const call: Call = {
        method: request.method,
        path,
        body: text ? (JSON.parse(text) as unknown) : undefined,
        ifMatch: request.headers.get('If-Match'),
      };
      calls.push(call);
      for (const route of options.routes ?? []) {
        const response = route(call);
        if (response) return response;
      }
      if (path === base && request.method === 'GET') {
        return json(200, appointment(), { ETag: etag });
      }
      if (path === `${base}/availability/slots`) {
        return json(200, rescheduleSlotsFixture((call.body as { date: string }).date));
      }
      if (path === `${base}/cancel`) return json(200, cancelledFixture(), { ETag: 'W/"8813"' });
      if (path === `${base}/reschedule`) {
        return json(200, rescheduledFixture(), { ETag: 'W/"8814"' });
      }
      return problemResponse(404, 'not_found');
    }),
  );
  return calls;
}

function renderPage(action: Action = 'view') {
  window.history.replaceState(null, '', `/r/${accessCode}`);
  const user = userEvent.setup();
  render(<AppointmentManager code={accessCode} initialAction={action} clock={clock} />);
  return user;
}

const gets = (calls: Call[]) => calls.filter((call) => call.method === 'GET');
const planCard = () => {
  const card = screen.getByRole('heading', { name: 'Plan değişti mi?' }).closest('section');
  if (!card) throw new Error('kart yok');
  return within(card);
};

beforeEach(() => {
  window.history.replaceState(null, '', `/r/${accessCode}`);
});

describe('Randevum (/r/[code])', () => {
  it('randevuyu, tutarı ve son tarihleri gösterir', async () => {
    mockLink();
    renderPage();

    expect(
      await screen.findByRole('heading', { level: 1, name: 'Çar 25 Kas · 14:30' }),
    ).toBeTruthy();
    expect(screen.getByText('Randevunuz · Onaylandı')).toBeTruthy();
    expect(screen.getByText('Kuaför Elif')).toBeTruthy();
    expect(screen.getByText('Saç kesimi, Fön · Elif Kaya · 1 sa 15 dk')).toBeTruthy();
    const rows = within(screen.getByRole('region', { name: 'Hizmetler ve tutar' }));
    expect(rows.getByText('500 ₺')).toBeTruthy();
    expect(rows.getByText('250 ₺')).toBeTruthy();
    expect(rows.getByText('Salonda ödenecek')).toBeTruthy();
    expect(rows.getByText('750 ₺')).toBeTruthy();
    expect(
      planCard().getByText(
        'Randevudan 24 saat öncesine kadar iptal edebilirsiniz. Saati değiştirmek için 12 saat öncesine kadar süreniz var.',
      ),
    ).toBeTruthy();
    expect(planCard().getByRole('button', { name: 'Saati değiştir' })).toBeTruthy();
    expect(planCard().getByRole('button', { name: 'Randevuyu iptal et' })).toBeTruthy();
    expect(planCard().queryByRole('link', { name: 'Salonu ara' })).toBeNull();
    expect(screen.getByRole('link', { name: 'Yol tarifi' }).getAttribute('href')).toContain(
      encodeURIComponent('Moda Cad. 42, Kadıköy'),
    );
    // Takvime ekle (dosya indirme), kapora (D4) ve uygulama tanıtımı (D5) yok.
    expect(screen.queryByText('Takvime ekle')).toBeNull();
    expect(screen.queryByText('Kapora')).toBeNull();
    expect(screen.queryByText('Randevularınız bir arada')).toBeNull();
    expect(screen.getByText('Bu sayfa yalnızca bu bağlantıyı açan kişiye görünür.')).toBeTruthy();
  });

  it('bilinmeyen kod "Bu randevu bulunamadı" gösterir', async () => {
    mockLink({
      routes: [(call) => (call.method === 'GET' ? problemResponse(404, 'not_found') : undefined)],
    });
    renderPage();
    expect(
      await screen.findByRole('heading', { level: 1, name: 'Bu randevu bulunamadı' }),
    ).toBeTruthy();
    expect(document.title).toBe('Bu randevu bulunamadı');
  });

  it('süre geçmişse butonların yerine "Salonu ara" (kriter 10)', async () => {
    mockLink({
      appointment: () =>
        linkAppointmentFixture({
          canCancel: false,
          canReschedule: false,
          cancelDeadlineUtc: '2026-11-24T05:30:00Z',
          rescheduleDeadlineUtc: '2026-11-24T05:30:00Z',
        }),
    });
    renderPage('reschedule');

    // Adres çubuğundaki ?islem=saat de açılmaz.
    await screen.findByRole('heading', { name: 'Plan değişti mi?' });
    expect(screen.queryByRole('heading', { name: 'Yeni saat seçin' })).toBeNull();
    expect(planCard().queryByRole('button', { name: 'Saati değiştir' })).toBeNull();
    expect(planCard().queryByRole('button', { name: 'Randevuyu iptal et' })).toBeNull();
    expect(
      planCard().getByText(
        'Online değişiklik süresi geçti. Saati değiştirmek veya iptal etmek için salonu arayın.',
      ),
    ).toBeTruthy();
    const call = planCard().getByRole('link', { name: 'Salonu ara' });
    expect(call.getAttribute('href')).toBe('tel:+905321234567');
  });
});

describe('onay bekleyen talep (Dilim 3)', () => {
  it('"Onay bekliyor" ve kalan yanıt süresi; saat değiştirme ve iptal açık (kriter 1, 6)', async () => {
    mockLink({ appointment: pendingFixture });
    renderPage();

    expect(await screen.findByText('Randevunuz · Onay bekliyor')).toBeTruthy();
    const card = screen.getByRole('heading', { name: 'Salon onayı bekleniyor' }).closest('section');
    if (!card) throw new Error('kart yok');
    expect(
      within(card).getByText(
        'Kuaför Elif en geç 1 saat 35 dakika içinde yanıtlar. Onaylandığında SMS ile bildireceğiz; yanıt gelmezse talep iptal olur.',
      ),
    ).toBeTruthy();
    expect(screen.queryByText('Randevunuz · Onaylandı')).toBeNull();
    expect(planCard().getByRole('button', { name: 'Saati değiştir' })).toBeTruthy();
    expect(planCard().getByRole('button', { name: 'Randevuyu iptal et' })).toBeTruthy();
  });

  it('yanıt süresi dolmuşsa süre yazılmaz', async () => {
    mockLink({
      appointment: () => ({ ...pendingFixture(), holdExpiresUtc: '2026-11-24T06:59:00Z' }),
    });
    renderPage();
    expect(await screen.findByText(/Kuaför Elif talebinizi inceleyip yanıtlar\./)).toBeTruthy();
  });

  it.each([
    ['DeclinedBySalon', 'Kuaför Elif bu talebi onaylayamadı.'],
    ['ApprovalExpired', 'Kuaför Elif talebinizi zamanında yanıtlayamadı.'],
  ] as const)(
    '%s müşteriye "İptal" olarak gösterilir, "reddedildi" yazılmaz (kriter 3, 4)',
    async (reason, text) => {
      mockLink({ appointment: () => requestCancelledFixture(reason) });
      renderPage();

      expect(await screen.findByText('Randevunuz · İptal')).toBeTruthy();
      expect(screen.getByRole('heading', { name: 'Talebiniz iptal edildi' })).toBeTruthy();
      expect(screen.getByText((content) => content.startsWith(text))).toBeTruthy();
      expect(screen.getByRole('link', { name: 'Yeni randevu al' }).getAttribute('href')).toBe(
        '/kuafor-elif',
      );
      expect(document.body.textContent).not.toMatch(/redd/i);
      expect(screen.queryByRole('heading', { name: 'Plan değişti mi?' })).toBeNull();
      expect(screen.queryByRole('heading', { name: 'Salon onayı bekleniyor' })).toBeNull();
    },
  );
});

describe('iptal', () => {
  it('"Planım değişti" ile iptal eder; If-Match gönderilir, sayfa iptal durumuna geçer (kriter 10)', async () => {
    const calls = mockLink();
    const user = renderPage();

    await user.click(await screen.findByRole('button', { name: 'Randevuyu iptal et' }));
    const dialog = within(await screen.findByRole('dialog', { name: 'Randevu iptal edilsin mi?' }));
    expect(dialog.getByText('Kuaför Elif · Çar 25 Kas · 14:30.')).toBeTruthy();
    expect(window.location.search).toBe('?islem=iptal');
    await user.click(dialog.getByRole('radio', { name: 'Planım değişti' }));
    await user.click(dialog.getByRole('button', { name: 'İptal et' }));

    expect(await screen.findByRole('heading', { name: 'Randevunuz iptal edildi' })).toBeTruthy();
    const cancel = calls.find((call) => call.path.endsWith('/cancel'));
    expect(cancel?.ifMatch).toBe(etag);
    expect(cancel?.body).toEqual({ reason: 'PlanChanged' });
    expect(screen.getByText('Randevunuz · İptal')).toBeTruthy();
    expect(screen.queryByRole('heading', { name: 'Plan değişti mi?' })).toBeNull();
    expect(screen.queryByRole('dialog')).toBeNull();
    expect(screen.getByRole('link', { name: 'Yeni randevu al' }).getAttribute('href')).toBe(
      '/kuafor-elif',
    );
  });

  it('neden seçilmeden ve "Diğer"de not yazılmadan gönderilmez', async () => {
    const calls = mockLink();
    const user = renderPage('cancel');

    const dialog = within(await screen.findByRole('dialog'));
    await user.click(dialog.getByRole('button', { name: 'İptal et' }));
    expect(dialog.getByText('İptal nedenini seçin.')).toBeTruthy();

    await user.click(dialog.getByRole('radio', { name: 'Diğer' }));
    await user.click(dialog.getByRole('button', { name: 'İptal et' }));
    expect(dialog.getByText('Kısa bir açıklama yazın.')).toBeTruthy();
    expect(calls.some((call) => call.path.endsWith('/cancel'))).toBe(false);

    await user.type(dialog.getByLabelText('Açıklama'), 'Şehir dışındayım');
    await user.click(dialog.getByRole('button', { name: 'İptal et' }));
    await screen.findByRole('heading', { name: 'Randevunuz iptal edildi' });
    expect(calls.find((call) => call.path.endsWith('/cancel'))?.body).toEqual({
      reason: 'Other',
      note: 'Şehir dışındayım',
    });
  });

  it('"Başka saat istiyorum" saat değişikliğini önerir', async () => {
    mockLink();
    const user = renderPage('cancel');
    const dialog = within(await screen.findByRole('dialog'));
    await user.click(dialog.getByRole('radio', { name: 'Başka saat istiyorum' }));
    await user.click(
      dialog.getByRole('button', { name: 'İptal yerine saati değiştirebilirsiniz' }),
    );
    expect(await screen.findByRole('heading', { name: 'Yeni saat seçin' })).toBeTruthy();
    expect(window.location.search).toBe('?islem=saat');
  });

  it('412: randevu yeniden okunur ve uyarı gösterilir', async () => {
    const calls = mockLink({
      routes: [
        (call) =>
          call.path.endsWith('/cancel') ? problemResponse(412, 'concurrency_conflict') : undefined,
      ],
    });
    const user = renderPage('cancel');
    const dialog = within(await screen.findByRole('dialog'));
    await user.click(dialog.getByRole('radio', { name: 'Hastayım' }));
    await user.click(dialog.getByRole('button', { name: 'İptal et' }));

    expect(await screen.findByText('Randevu bu arada değişti.')).toBeTruthy();
    expect(gets(calls)).toHaveLength(2);
    expect(screen.queryByRole('dialog')).toBeNull();
  });

  it('cutoff_passed: "Salonu ara" deseni', async () => {
    let cutoff = false;
    mockLink({
      appointment: () =>
        cutoff
          ? linkAppointmentFixture({ canCancel: false, canReschedule: false })
          : linkAppointmentFixture(),
      routes: [
        (call) => {
          if (!call.path.endsWith('/cancel')) return undefined;
          cutoff = true;
          return problemResponse(409, 'cutoff_passed');
        },
      ],
    });
    const user = renderPage('cancel');
    const dialog = within(await screen.findByRole('dialog'));
    await user.click(dialog.getByRole('radio', { name: 'Planım değişti' }));
    await user.click(dialog.getByRole('button', { name: 'İptal et' }));

    expect(await screen.findByText('Online değişiklik süresi geçti.')).toBeTruthy();
    await waitFor(() => {
      expect(planCard().getByRole('link', { name: 'Salonu ara' })).toBeTruthy();
    });
    expect(planCard().queryByRole('button', { name: 'Randevuyu iptal et' })).toBeNull();
  });
});

describe('saat değiştir', () => {
  it('yeni saati seçer, If-Match ile gönderir ve Randevum’a döner', async () => {
    const calls = mockLink();
    const user = renderPage();

    await user.click(await screen.findByRole('button', { name: 'Saati değiştir' }));
    expect(await screen.findByRole('heading', { name: 'Yeni saat seçin' })).toBeTruthy();
    expect(window.location.search).toBe('?islem=saat');
    expect(screen.getByText('Çar 25 Kas · 14:30')).toBeTruthy();
    expect(screen.getByRole('heading', { name: 'Çarşamba 25 Kasım' })).toBeTruthy();
    expect(
      screen.getByText('Saat değişikliği randevudan 12 saat öncesine kadar yapılabilir.'),
    ).toBeTruthy();

    // Randevunun şu anki saati seçilemez.
    expect(await screen.findByRole('radio', { name: '14:30, dolu' })).toBeTruthy();
    await user.click(screen.getByRole('radio', { name: '15:15' }));
    expect(screen.getByText('Çar 25 Kas · 15:15')).toBeTruthy();
    expect(screen.getByText("16:30'da biter")).toBeTruthy();
    await user.click(screen.getByRole('button', { name: 'Saati değiştir' }));

    expect(await screen.findByText('Saatiniz değiştirildi.')).toBeTruthy();
    expect(screen.getByText('Yeni randevu saatiniz Çar 25 Kas · 15:15.')).toBeTruthy();
    expect(screen.getByRole('heading', { level: 1, name: 'Çar 25 Kas · 15:15' })).toBeTruthy();
    const reschedule = calls.find((call) => call.path.endsWith('/reschedule'));
    expect(reschedule?.ifMatch).toBe(etag);
    expect(reschedule?.body).toEqual({ startUtc: '2026-11-25T12:15:00Z' });
    expect(calls.find((call) => call.path.endsWith('/availability/slots'))?.body).toEqual({
      date: '2026-11-25',
    });
  });

  it('başka gün seçilince o günün saatleri istenir', async () => {
    const calls = mockLink();
    const user = renderPage('reschedule');
    await user.click(await screen.findByRole('radio', { name: /^27 Kasım Cuma/ }));
    await screen.findByRole('heading', { name: 'Cuma 27 Kasım' });
    await waitFor(() => {
      expect(
        calls.filter((call) => call.path.endsWith('/availability/slots')).at(-1)?.body,
      ).toEqual({ date: '2026-11-27' });
    });
  });

  it('409 slot_taken: alternatif saat çipleri', async () => {
    let first = true;
    const calls = mockLink({
      routes: [
        (call) => {
          if (!call.path.endsWith('/reschedule') || !first) return undefined;
          first = false;
          return problemResponse(409, 'slot_taken', { alternatives: ['2026-11-25T12:45:00Z'] });
        },
      ],
    });
    const user = renderPage('reschedule');
    await user.click(await screen.findByRole('radio', { name: '15:15' }));
    await user.click(screen.getByRole('button', { name: 'Saati değiştir' }));

    const alert = within(await screen.findByRole('alert'));
    expect(alert.getByText('15:15 artık boş değil.')).toBeTruthy();
    expect(
      alert.getByText('Siz seçerken başka biri aldı. En yakın boş saatler 15:45.'),
    ).toBeTruthy();
    await user.click(alert.getByRole('button', { name: '15:45' }));
    expect(await screen.findByRole('radio', { name: '15:45', checked: true })).toBeTruthy();
    await user.click(screen.getByRole('button', { name: 'Saati değiştir' }));
    await screen.findByText('Saatiniz değiştirildi.');
    expect(calls.filter((call) => call.path.endsWith('/reschedule')).at(-1)?.body).toEqual({
      startUtc: '2026-11-25T12:45:00Z',
    });
  });

  it('cutoff_passed: Randevum’a döner, "Salonu ara" (kriter 10)', async () => {
    let cutoff = false;
    mockLink({
      appointment: () =>
        cutoff ? linkAppointmentFixture({ canReschedule: false }) : linkAppointmentFixture(),
      routes: [
        (call) => {
          if (!call.path.endsWith('/reschedule')) return undefined;
          cutoff = true;
          return problemResponse(409, 'cutoff_passed');
        },
      ],
    });
    const user = renderPage('reschedule');
    await user.click(await screen.findByRole('radio', { name: '15:15' }));
    await user.click(screen.getByRole('button', { name: 'Saati değiştir' }));

    expect(await screen.findByText('Online değişiklik süresi geçti.')).toBeTruthy();
    await waitFor(() => {
      expect(planCard().getByRole('link', { name: 'Salonu ara' })).toBeTruthy();
    });
    expect(planCard().queryByRole('button', { name: 'Saati değiştir' })).toBeNull();
    expect(planCard().getByRole('button', { name: 'Randevuyu iptal et' })).toBeTruthy();
    expect(screen.queryByRole('heading', { name: 'Yeni saat seçin' })).toBeNull();
  });

  it('412: yeniden okunur ve uyarı gösterilir', async () => {
    const calls = mockLink({
      routes: [
        (call) =>
          call.path.endsWith('/reschedule')
            ? problemResponse(412, 'concurrency_conflict')
            : undefined,
      ],
    });
    const user = renderPage('reschedule');
    await user.click(await screen.findByRole('radio', { name: '15:15' }));
    await user.click(screen.getByRole('button', { name: 'Saati değiştir' }));

    expect(await screen.findByText('Randevu bu arada değişti.')).toBeTruthy();
    expect(gets(calls)).toHaveLength(2);
  });
});
