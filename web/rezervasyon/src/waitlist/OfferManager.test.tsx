import { fixedClock } from '@omvila/ui-web/time';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, expect, it } from 'vitest';
import { json, mockApi, problem } from '@/test/api-mock';
import { appointmentFixture, depositFixture, nowUtc } from '@/test/booking-fixtures';
import { entryFixture, offerCode, offerFixture, waitlistCode } from '@/test/d5-fixtures';
import type { WaitlistOffer } from './waitlist-api';
import { OfferManager } from './OfferManager';

// Bekleme teklifi bağlantısı (/b/{kod}; BeklemeTeklif, BeklemeDoldu; KK3).

const clock = fixedClock(nowUtc);
const base = `/api/v1/public/waitlist-offers/${offerCode}`;

function setup(
  offer: WaitlistOffer | null,
  routes: Record<string, (request: Request) => Response> = {},
) {
  const calls = mockApi((request) => {
    const path = new URL(request.url).pathname;
    const route = routes[path];
    if (route) return route(request);
    if (path === base && request.method === 'GET') {
      return offer
        ? json(200, offer, { ETag: `W/"${offer.version.replace(/\D/g, '')}"` })
        : problem(404, 'access_code_invalid');
    }
    return problem(404, 'not_found');
  });
  const user = userEvent.setup();
  render(<OfferManager code={offerCode} clock={clock} />);
  return { calls, user };
}

describe('bekleme teklifi', () => {
  it('geri sayım, ad, tarih/hizmet/kişi/salonda ödenecek; kod istenmez', async () => {
    setup(offerFixture());
    expect(
      await screen.findByRole('heading', { name: 'Merve, beklediğiniz saat boşaldı' }),
    ).toBeTruthy();
    expect(screen.getByRole('timer').textContent).toBe('09:05');
    expect(screen.getByText('Sizin için tutuluyor')).toBeTruthy();
    expect(screen.getByText('Cuma 27 Kasım · 16:00')).toBeTruthy();
    expect(screen.getByText('Masaj · 1 sa')).toBeTruthy();
    expect(screen.getByText('Seda Kurt')).toBeTruthy();
    expect(screen.getByText('900 ₺')).toBeTruthy();
    expect(screen.getByText(/kod girmeniz gerekmez/)).toBeTruthy();
  });

  it('"Randevuyu al" If-Match ile; onaylı randevu Başarı ekranı', async () => {
    const { calls, user } = setup(offerFixture(), {
      [`${base}/accept`]: () => json(200, appointmentFixture()),
    });
    await user.click(await screen.findByRole('button', { name: 'Randevuyu al' }));
    expect(await screen.findByRole('heading', { name: 'Randevunuz onaylandı' })).toBeTruthy();
    expect(screen.getByText('Onay mesajını telefonunuza gönderdik.')).toBeTruthy();
    const accept = calls.find((request) => request.url.endsWith('/accept'));
    expect(accept?.headers.get('If-Match')).toBe('W/"9420"');
  });

  it('kapora isteyen salonda kabul Havale ekranına geçer (W4)', async () => {
    const { user } = setup(
      offerFixture({
        requiresApproval: true,
        depositAmount: { amountMinor: 15000, currency: 'TRY' },
      }),
      {
        [`${base}/accept`]: () => json(200, depositFixture('Awaiting')),
      },
    );
    expect(
      await screen.findByText('Randevu kapora havalesinden sonra kesinleşir.', { exact: false }),
    ).toBeTruthy();
    await user.click(screen.getByRole('button', { name: 'Randevuyu al' }));
    expect(
      await screen.findByRole('heading', { name: 'Kaporayı havale ile gönderin' }),
    ).toBeTruthy();
  });

  it('onay isteyen salonda kabul Talep ekranı', async () => {
    const { user } = setup(offerFixture({ requiresApproval: true }), {
      [`${base}/accept`]: () =>
        json(
          200,
          appointmentFixture({
            status: 'PendingApproval',
            displayStatus: 'PendingApproval',
            holdExpiresUtc: '2026-11-24T09:00:00Z',
          }),
        ),
    });
    await user.click(await screen.findByRole('button', { name: 'Randevuyu al' }));
    expect(await screen.findByRole('heading', { name: 'Talebiniz alındı' })).toBeTruthy();
  });

  it('"Bu saat olmaz": reddedilir, listede kalındığı söylenir', async () => {
    const { calls, user } = setup(offerFixture(), {
      [`${base}/decline`]: () =>
        json(200, offerFixture({ status: 'Declined', version: 'W/"9421"' })),
    });
    await user.click(await screen.findByRole('button', { name: 'Bu saat olmaz' }));
    expect(await screen.findByRole('heading', { name: 'Bu saati geri çevirdiniz' })).toBeTruthy();
    expect(screen.getByText(/Bekleme listesinde kalmaya devam ediyorsunuz/)).toBeTruthy();
    expect(calls.find((r) => r.url.endsWith('/decline'))?.headers.get('If-Match')).toBe('W/"9420"');
  });

  it('süresi dolmuş teklif BeklemeDoldu; "Listeden çık" kayıt koduyla', async () => {
    const { calls, user } = setup(offerFixture({ status: 'Expired' }), {
      [`/api/v1/public/waitlist/${waitlistCode}/leave`]: () =>
        json(200, entryFixture({ status: 'Cancelled' })),
    });
    expect(await screen.findByRole('heading', { name: 'Bu saatin süresi doldu' })).toBeTruthy();
    expect(screen.getByText(/Cuma 16:00 için ayrılan süre geçti/)).toBeTruthy();
    expect(screen.getByRole('link', { name: 'Başka saat seç' }).getAttribute('href')).toBe(
      '/spa-moda',
    );
    await user.click(screen.getByRole('button', { name: 'Listeden çık' }));
    expect(
      await screen.findByRole('heading', { name: 'Bekleme listesinden çıktınız' }),
    ).toBeTruthy();
    expect(calls.some((r) => r.url.endsWith(`/waitlist/${waitlistCode}/leave`))).toBe(true);
  });

  it('geri sayım biterse (tutma süresi geçti) BeklemeDoldu', async () => {
    setup(offerFixture({ holdExpiresUtc: nowUtc }));
    expect(await screen.findByRole('heading', { name: 'Bu saatin süresi doldu' })).toBeTruthy();
    expect(screen.queryByRole('button', { name: 'Randevuyu al' })).toBeNull();
  });

  it('kabulde 409 hold_expired: BeklemeDoldu', async () => {
    const { user } = setup(offerFixture(), {
      [`${base}/accept`]: () => problem(409, 'hold_expired'),
    });
    await user.click(await screen.findByRole('button', { name: 'Randevuyu al' }));
    expect(await screen.findByRole('heading', { name: 'Bu saatin süresi doldu' })).toBeTruthy();
  });

  it('kabulde 412: teklif yeniden okunur', async () => {
    let reads = 0;
    const { user } = setup(null, {
      [base]: () => {
        reads += 1;
        return json(200, offerFixture(), { ETag: 'W/"9420"' });
      },
      [`${base}/accept`]: () => problem(412, 'concurrency_conflict'),
    });
    await user.click(await screen.findByRole('button', { name: 'Randevuyu al' }));
    expect(await screen.findByText(/Teklif bu arada değişti/)).toBeTruthy();
    await waitFor(() => {
      expect(reads).toBe(2);
    });
  });

  it('kabul edilmiş teklif randevu sayfasına bağlanır', async () => {
    setup(offerFixture({ status: 'Accepted', appointmentAccessCode: '7Kx2Pq9a' }));
    expect(await screen.findByRole('heading', { name: 'Bu saati aldınız' })).toBeTruthy();
    expect(screen.getByRole('link', { name: 'Randevunuzu görün' }).getAttribute('href')).toBe(
      '/r/7Kx2Pq9a',
    );
  });

  it('bilinmeyen kod "Bu teklif bulunamadı"', async () => {
    setup(null);
    expect(await screen.findByRole('heading', { name: 'Bu teklif bulunamadı' })).toBeTruthy();
  });
});
