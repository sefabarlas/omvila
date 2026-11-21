import { fixedClock } from '@omvila/ui-web/time';
import { render, screen, waitFor, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { beforeEach, describe, expect, it } from 'vitest';
import { brandFixture, json, problem } from '@/test/api-mock';
import { depositCatalogFixture, ids, mockBooking, nowUtc } from '@/test/booking-mock';
import type { Recorded } from '@/test/booking-mock';
import { entryFixture, packageFixture } from '@/test/d5-fixtures';
import type { PublicBrand } from '@/lib/brand';
import { BookingFlow } from './BookingFlow';

// Dilim 5 rezervasyon akışı: dolu günde bekleme listesi (KK3), onayda paket kutusu (KK6, KK8), salon puanı.

const clock = fixedClock(nowUtc);

function renderFlow(search = '', brand: PublicBrand = brandFixture()) {
  window.history.replaceState(null, '', `/kuafor-elif${search}`);
  const user = userEvent.setup();
  render(<BookingFlow brand={brand} initialSearch={search} clock={clock} />);
  return user;
}

const isPath = (request: Request, suffix: string) => new URL(request.url).pathname.endsWith(suffix);
const calledWith = (calls: Recorded[], suffix: string) =>
  calls.filter((call) => call.path.endsWith(suffix));

async function verifyPhone(user: ReturnType<typeof userEvent.setup>) {
  await user.type(await screen.findByLabelText('Ad soyad'), 'Ayşe Yılmaz');
  await user.type(screen.getByLabelText('Telefon numaranız'), '532 123 45 67');
  await user.click(screen.getByRole('checkbox', { name: /Aydınlatma metnini/ }));
  await user.click(screen.getByRole('button', { name: 'Doğrulama kodu gönder' }));
  await user.type(await screen.findByRole('textbox', { name: 'Doğrulama kodu, hane 1' }), '482193');
}

const confirmSearch = `?step=details&services=${ids.sac},${ids.fon}&staff=${ids.elif}&date=2026-11-24&time=14:30`;

beforeEach(() => {
  window.history.replaceState(null, '', '/kuafor-elif');
});

describe('bekleme listesi (KK3)', () => {
  it('dolu gün → saat dilimleri + başka personel → telefon → kod → listeye katılım', async () => {
    const calls = mockBooking({
      routes: [(request) => (isPath(request, '/waitlist') ? json(201, entryFixture()) : undefined)],
    });
    const user = renderFlow(`?step=time&services=${ids.sac},${ids.fon}&staff=${ids.elif}`);

    await user.click(await screen.findByRole('radio', { name: '26 Kasım Perşembe, boş saat yok' }));
    expect(screen.getByRole('heading', { name: 'Bekleme listesine katılın' })).toBeTruthy();
    expect(screen.getByText('Telefon numaranızı bir sonraki adımda doğrulayacağız.')).toBeTruthy();
    const afternoon = screen.getByRole('button', { name: 'Öğleden sonra' });
    await user.click(screen.getByRole('button', { name: 'Akşam' }));
    await user.click(afternoon);
    expect(afternoon.getAttribute('aria-pressed')).toBe('true');
    const other = screen.getByRole('switch', { name: 'Başka personel de olur' });
    expect(screen.getByText('Elif Kaya dışında yer açılırsa da haber ver')).toBeTruthy();
    await user.click(other);
    await user.click(screen.getByRole('button', { name: 'Bekleme listesine katıl' }));

    expect(await screen.findByRole('heading', { name: 'Bilgileriniz' })).toBeTruthy();
    expect(await screen.findByText('Perşembe, 26 Kasım · Öğleden sonra, Akşam')).toBeTruthy();
    // Katılma gövdesinde not ve kampanya izni yok; sorulmaz.
    expect(screen.queryByLabelText('Salona notunuz (isteğe bağlı)')).toBeNull();
    expect(screen.queryByRole('checkbox', { name: /kampanyaları/ })).toBeNull();
    await verifyPhone(user);

    expect(
      await screen.findByRole('heading', { name: 'Bekleme listesine katıldınız' }),
    ).toBeTruthy();
    const [join] = calledWith(calls, '/waitlist');
    expect(join?.request.headers.get('Authorization')).toBe('Bearer rezervasyon-jetonu');
    expect(join?.request.headers.get('Idempotency-Key')).toBeTruthy();
    expect(join?.body).toEqual({
      preferences: {
        serviceIds: [ids.sac, ids.fon],
        preferredStaffId: ids.elif,
        acceptOtherStaff: true,
        dates: ['2026-11-26'],
        timeWindows: ['Afternoon', 'Evening'],
      },
      fullName: 'Ayşe Yılmaz',
      privacyNoticeAcknowledged: true,
      privacyNoticeVersion: expect.any(String) as string,
    });
    expect(calledWith(calls, '/waitlist')).toHaveLength(1);
    expect(calledWith(calls, '/appointments')).toHaveLength(0);
    const facts = within(screen.getByLabelText('Bekleme tercihiniz'));
    expect(facts.getByText('Saç kesimi, Fön')).toBeTruthy();
    expect(facts.getByText('Perşembe, 26 Kasım')).toBeTruthy();
    expect(window.location.search).toBe('');
  });

  it('"Fark etmez"de personel anahtarı yok, tercih gönderilmez; hata olursa tekrar denenir', async () => {
    let attempts = 0;
    const calls = mockBooking({
      routes: [
        (request) => {
          if (!isPath(request, '/waitlist')) return undefined;
          attempts += 1;
          return attempts === 1 ? problem(422, 'validation_failed') : json(200, entryFixture());
        },
      ],
    });
    const user = renderFlow(`?step=time&services=${ids.sac}&staff=any`);
    await user.click(await screen.findByRole('radio', { name: '26 Kasım Perşembe, boş saat yok' }));
    expect(screen.queryByRole('switch', { name: 'Başka personel de olur' })).toBeNull();
    await user.click(screen.getByRole('button', { name: 'Bekleme listesine katıl' }));
    expect(await screen.findByText('Perşembe, 26 Kasım · Her saat')).toBeTruthy();
    await verifyPhone(user);

    expect(await screen.findByText('Bekleme listesine eklenemediniz.')).toBeTruthy();
    await user.click(screen.getByRole('button', { name: 'Tekrar dene' }));
    expect(
      await screen.findByRole('heading', { name: 'Bekleme listesine katıldınız' }),
    ).toBeTruthy();
    const bodies = calledWith(calls, '/waitlist').map(
      (call) => call.body as { preferences: unknown },
    );
    expect(bodies).toHaveLength(2);
    expect(bodies[1]?.preferences).toEqual({
      serviceIds: [ids.sac],
      preferredStaffId: null,
      acceptOtherStaff: true,
      dates: ['2026-11-26'],
      timeWindows: [],
    });
  });
});

describe('paket kutusu (KK6, KK8)', () => {
  it('Fön paketinden düşülür: "Paketten", "Salonda ödenecek" paket ve kapora düşülmüş; usePackageItems gider', async () => {
    const calls = mockBooking({
      catalog: depositCatalogFixture(),
      routes: [
        (request) =>
          isPath(request, '/my-packages') ? json(200, { items: [packageFixture()] }) : undefined,
      ],
    });
    const user = renderFlow(confirmSearch);
    await verifyPhone(user);

    const toggle = await screen.findByRole('switch', { name: 'Fön paketinizden düşülsün' });
    expect(toggle).toHaveProperty('checked', true);
    expect(screen.getByText("10 seansın 7'si kaldı · son kullanım 30 Mart")).toBeTruthy();
    const packagesCall = calledWith(calls, '/my-packages')[0];
    expect(packagesCall?.request.headers.get('Authorization')).toBe('Bearer rezervasyon-jetonu');
    const summary = within(screen.getByRole('region', { name: 'Randevu özeti' }));
    expect(summary.getByText('Paketten')).toBeTruthy();
    // 750 − Fön 250 (paket) − kapora 150 (Saç kesimi) = 350 (Onay.png).
    expect(summary.getByText('350 ₺')).toBeTruthy();
    expect(summary.queryByText(/^Toplam/)).toBeNull();
    expect(within(screen.getByRole('region', { name: 'Kapora' })).getByText('150 ₺')).toBeTruthy();

    await user.click(screen.getByRole('button', { name: 'Randevuyu oluştur' }));
    await waitFor(() => {
      expect(calledWith(calls, '/appointments')).toHaveLength(1);
    });
    expect(calledWith(calls, '/appointments')[0]?.body).toMatchObject({
      usePackageItems: [
        { serviceId: ids.fon, customerPackageItemId: '0191c000-0000-7000-8000-000000000001' },
      ],
    });
  });

  it('anahtar kapatılınca paket kullanılmaz; paketten düşen kalemde kapora yok (KK8)', async () => {
    const calls = mockBooking({
      catalog: depositCatalogFixture(),
      routes: [
        (request) =>
          isPath(request, '/my-packages')
            ? json(200, {
                items: [packageFixture({ serviceId: ids.sac, packageName: 'Kesim paketi' })],
              })
            : undefined,
      ],
    });
    const user = renderFlow(confirmSearch);
    await verifyPhone(user);

    const toggle = await screen.findByRole('switch', { name: 'Saç kesimi paketinizden düşülsün' });
    // Kaporalı hizmet paketten düşülünce kapora istenmez.
    expect(screen.queryByRole('region', { name: 'Kapora' })).toBeNull();
    expect(screen.getByText('Ödeme sayfadan alınmaz; ücreti salonda ödersiniz.')).toBeTruthy();
    await user.click(toggle);
    expect(screen.getByRole('region', { name: 'Kapora' })).toBeTruthy();
    const summary = within(screen.getByRole('region', { name: 'Randevu özeti' }));
    expect(summary.getByText('Toplam · 1 sa 15 dk')).toBeTruthy();
    expect(summary.queryByText('Paketten')).toBeNull();

    await user.click(screen.getByRole('button', { name: 'Randevuyu oluştur' }));
    await waitFor(() => {
      expect(calledWith(calls, '/appointments')).toHaveLength(1);
    });
    expect(calledWith(calls, '/appointments')[0]?.body).not.toHaveProperty('usePackageItems');
  });

  it('paketi olmayan müşteride ve paket listesi hata verince kutu yok', async () => {
    mockBooking({
      routes: [
        (request) => (isPath(request, '/my-packages') ? problem(401, 'unauthorized') : undefined),
      ],
    });
    const user = renderFlow(confirmSearch);
    await verifyPhone(user);
    expect(await screen.findByRole('button', { name: 'Randevuyu oluştur' })).toBeTruthy();
    expect(screen.queryByRole('switch')).toBeNull();
  });
});

describe('salon puanı', () => {
  it('PublicBrand.rating varsa salon kartında puan ve değerlendirme sayısı', async () => {
    mockBooking();
    renderFlow('', brandFixture({ rating: { average: 4.9, count: 312 } }));
    expect(await screen.findByText('Puan 5 üzerinden 4,9, 312 değerlendirme')).toBeTruthy();
  });

  it('eşik altında (rating null) puan gösterilmez', async () => {
    mockBooking();
    renderFlow();
    await screen.findByRole('checkbox', { name: 'Saç kesimi' });
    expect(screen.queryByText(/değerlendirme\)/)).toBeNull();
  });
});
