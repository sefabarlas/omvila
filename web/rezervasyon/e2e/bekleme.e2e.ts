import { expect, test } from '@playwright/test';
import type { Page } from '@playwright/test';
import { depositFixture, ids } from '../src/test/booking-fixtures';
import { offerCode, reviewCode } from '../src/test/d5-fixtures';
import { bookingApi, offerApi, reviewApi } from './support';

// Dilim 5 rezervasyon sayfası (KK3, KK6, KK8): dolu günde bekleme listesine katılma, teklif bağlantısı (/b/: kabul,
// kapora isteyen salonda Havale, ret, süre dolumu, "Listeden çık"), değerlendirme bağlantısı (/d/) ve Onay'da paket
// kutusu. Sunucunun motor tarafı (teklif oluşturma, tutma, sıradakine geçiş) API entegrasyon testlerindedir (T-90);
// burada müşterinin gördüğü yanıtlar verilir. "Şimdi" 24 Kasım 2026 10:00.

const next = (page: Page) => page.getByRole('button', { name: 'Devam et' }).click();

async function verifyPhone(page: Page) {
  await page.getByLabel('Ad soyad').fill('Ayşe Yılmaz');
  await page.getByLabel('Telefon numaranız').fill('532 123 45 67');
  await page.getByText(/okudum; (randevu|bekleme listesi) için/).click();
  await page.getByRole('button', { name: 'Doğrulama kodu gönder' }).click();
  await page.getByRole('textbox', { name: 'Doğrulama kodu, hane 1' }).fill('482193');
}

test('dolu gün: bekleme listesine katıl → telefon → kod → listeye katıldınız', async ({ page }) => {
  const errors: string[] = [];
  page.on('pageerror', (error) => errors.push(error.message));
  const calls = await bookingApi(page);
  await page.goto('/kuafor-elif');
  await page.getByText('Saç kesimi', { exact: true }).click();
  await next(page);
  await page.getByRole('radio', { name: /Elif Kaya/ }).click();
  await next(page);
  await page.getByRole('radio', { name: '26 Kasım Perşembe, boş saat yok' }).click();

  await expect(page.getByRole('heading', { name: 'Bekleme listesine katılın' })).toBeVisible();
  await page.getByRole('button', { name: 'Öğleden sonra' }).click();
  await page.getByRole('button', { name: 'Akşam' }).click();
  await page.getByText('Başka personel de olur', { exact: true }).click();
  await expect(page.getByRole('switch', { name: 'Başka personel de olur' })).toBeChecked();
  await page.getByRole('button', { name: 'Bekleme listesine katıl' }).click();

  await expect(page.getByText('Perşembe, 26 Kasım · Öğleden sonra, Akşam')).toBeVisible();
  await verifyPhone(page);
  await expect(page.getByRole('heading', { name: 'Bekleme listesine katıldınız' })).toBeVisible();
  expect(calls.waitlist).toHaveLength(1);
  expect(calls.waitlist[0]).toMatchObject({
    preferences: {
      serviceIds: [ids.sac],
      preferredStaffId: ids.elif,
      acceptOtherStaff: true,
      dates: ['2026-11-26'],
      timeWindows: ['Afternoon', 'Evening'],
    },
    fullName: 'Ayşe Yılmaz',
    privacyNoticeAcknowledged: true,
  });
  expect(calls.create).toHaveLength(0);
  expect(errors).toEqual([]);
});

test('teklif: geri sayım → "Randevuyu al" (If-Match) → Başarı', async ({ page }) => {
  const calls = await offerApi(page);
  await page.goto(`/b/${offerCode}`);
  await expect(
    page.getByRole('heading', { name: 'Merve, beklediğiniz saat boşaldı' }),
  ).toBeVisible();
  await expect(page.getByRole('timer')).toHaveText('09:05');
  await page.getByRole('button', { name: 'Randevuyu al' }).click();
  await expect(page.getByRole('heading', { name: 'Randevunuz onaylandı' })).toBeVisible();
  expect(calls.accept).toEqual(['W/"9420"']);
});

test('teklif: kapora isteyen salonda kabul Havale ekranına geçer (W4)', async ({ page }) => {
  await offerApi(
    page,
    { requiresApproval: true, depositAmount: { amountMinor: 15000, currency: 'TRY' } },
    { accepted: depositFixture('Awaiting') },
  );
  await page.goto(`/b/${offerCode}`);
  await page.getByRole('button', { name: 'Randevuyu al' }).click();
  await expect(page.getByRole('heading', { name: 'Kaporayı havale ile gönderin' })).toBeVisible();
});

test('teklif: "Bu saat olmaz" → listede kalınır; "Listeden çık"', async ({ page }) => {
  const calls = await offerApi(page);
  await page.goto(`/b/${offerCode}`);
  await page.getByRole('button', { name: 'Bu saat olmaz' }).click();
  await expect(page.getByRole('heading', { name: 'Bu saati geri çevirdiniz' })).toBeVisible();
  await expect(page.getByText(/Bekleme listesinde kalmaya devam ediyorsunuz/)).toBeVisible();
  expect(calls.decline).toEqual(['W/"9420"']);
  await page.getByRole('button', { name: 'Listeden çık' }).click();
  await expect(page.getByRole('heading', { name: 'Bekleme listesinden çıktınız' })).toBeVisible();
  expect(calls.leave).toBe(1);
});

test('teklif: sayfa açıkken süre dolar → BeklemeDoldu', async ({ page }) => {
  await offerApi(page, {}, { install: true });
  await page.goto(`/b/${offerCode}`);
  await expect(page.getByRole('button', { name: 'Randevuyu al' })).toBeVisible();
  await page.clock.fastForward('09:06');
  await expect(page.getByRole('heading', { name: 'Bu saatin süresi doldu' })).toBeVisible();
  await expect(page.getByRole('button', { name: 'Randevuyu al' })).toHaveCount(0);
  await expect(page.getByRole('link', { name: 'Başka saat seç' })).toHaveAttribute(
    'href',
    '/spa-moda',
  );
});

test('değerlendirme: 4 yıldız + yorum → teşekkür; yeniden açınca "zaten değerlendirdiniz"', async ({
  page,
}) => {
  const calls = await reviewApi(page);
  await page.goto(`/d/${reviewCode}`);
  await expect(page.getByRole('heading', { name: "Kuaför Elif'te nasıldı?" })).toBeVisible();
  await page.getByRole('radio', { name: '4 yıldız, Çok iyi' }).check({ force: true });
  await page.getByLabel('Yorum (isteğe bağlı)').fill('Boya tam istediğim gibi oldu.');
  await page.getByRole('button', { name: 'Gönder' }).click();
  await expect(page.getByRole('heading', { name: 'Teşekkür ederiz' })).toBeVisible();
  expect(calls.posts).toHaveLength(1);
  expect(calls.posts[0]?.body).toEqual({ rating: 4, comment: 'Boya tam istediğim gibi oldu.' });
  expect(calls.posts[0]?.key).toBeTruthy();

  await page.reload();
  await expect(
    page.getByRole('heading', { name: 'Bu randevuyu zaten değerlendirdiniz' }),
  ).toBeVisible();
});

test('paket kutusu: Fön paketinden düşülür, kapora yalnız Saç kesiminden (KK6, KK8)', async ({
  page,
}) => {
  const calls = await bookingApi(page, { deposit: true, packages: true });
  await page.goto('/kuafor-elif');
  await page.getByText('Saç kesimi', { exact: true }).click();
  await page.getByText('Fön', { exact: true }).click();
  await next(page);
  await page.getByRole('radio', { name: /Elif Kaya/ }).click();
  await next(page);
  await page.getByRole('radio', { name: '14:30' }).click();
  await next(page);
  await verifyPhone(page);

  await expect(page.getByRole('switch', { name: 'Fön paketinizden düşülsün' })).toBeChecked();
  await expect(page.getByText("10 seansın 7'si kaldı · son kullanım 30 Mart")).toBeVisible();
  const summary = page.getByRole('region', { name: 'Randevu özeti' });
  await expect(summary.getByText('Paketten')).toBeVisible();
  await expect(summary.getByText('350 ₺')).toBeVisible();
  await page.getByRole('button', { name: 'Randevuyu oluştur' }).click();
  await expect(page.getByRole('heading', { name: 'Kaporayı havale ile gönderin' })).toBeVisible();
  expect(calls.create[0]).toMatchObject({
    usePackageItems: [{ serviceId: ids.fon }],
  });
});
