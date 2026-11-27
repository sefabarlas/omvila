import { expect, test } from '@playwright/test';
import { captureBody, signedOut, unregisteredPhone } from './support';

// Kritik akış 1 (test-stratejisi.md): salon kaydı → kurulum adımları → rezervasyon bağlantısını kopyalama.
test('kayıt → kurulum adımları → bağlantıyı kopyalama', async ({ page, context }) => {
  await context.grantPermissions(['clipboard-read', 'clipboard-write']);
  await signedOut(page);
  await unregisteredPhone(page);
  // Yeni salonda randevu ve onay bekleyen talep yok (Prism örnekleri dolu döner): genel bakış bağlantı kartını
  // gösterir (GenelBakisBos).
  const emptyJson = (body: unknown) => ({
    contentType: 'application/json',
    headers: {
      'Access-Control-Allow-Origin': 'http://localhost:5180',
      'Access-Control-Allow-Credentials': 'true',
    },
    body: JSON.stringify(body),
  });
  await page.route(/\/api\/v1\/manage\/approval-queue/, (route) =>
    route.request().method() === 'OPTIONS'
      ? route.fallback()
      : route.fulfill(
          emptyJson({
            generatedUtc: '2026-11-25T09:00:00Z',
            totalCount: 0,
            earliestHoldExpiresUtc: null,
            items: [],
          }),
        ),
  );
  await page.route(/\/api\/v1\/manage\/calendar\?/, (route) => {
    if (route.request().method() === 'OPTIONS') {
      return route.fallback();
    }

    const url = new URL(route.request().url());
    const from = url.searchParams.get('from') ?? '';
    return route.fulfill(
      emptyJson({
        branchId: url.searchParams.get('branchId'),
        timeZone: 'Europe/Istanbul',
        from,
        to: url.searchParams.get('to') ?? from,
        staff: [],
        days: [],
        appointments: [],
        timeOffs: [],
      }),
    );
  });

  await page.goto('/kayit');
  await expect(
    page.getByRole('heading', { level: 1, name: 'İşletmenizi oluşturun' }),
  ).toBeVisible();
  // T-130: "Adınız soyadınız" ilk alan ve zorunlu; boşken kod istenmez.
  const ownerName = page.getByLabel('Adınız soyadınız');
  await expect(ownerName).toHaveAttribute('autocomplete', 'name');
  await page.getByLabel('İşletme adı').fill('Kuaför Elif');
  await page.getByLabel('Ne tür bir işletme?').selectOption('hair_salon');
  await page.getByLabel('Cep telefonunuz').fill('0555 000 00 01');
  await page
    .getByRole('checkbox', { name: 'Kullanım koşullarını ve aydınlatma metnini okudum' })
    .check();
  await page.getByRole('button', { name: 'SMS kodu gönder' }).click();
  await expect(page.getByText('Adınızı ve soyadınızı yazın.')).toBeVisible();
  await expect(page.getByText('2 / 2 · Telefon doğrulama')).toBeHidden();
  await ownerName.fill('  Elif   Kaya ');
  const otp = await captureBody(page, '/auth/otp/request', 'POST');
  await page.getByRole('button', { name: 'SMS kodu gönder' }).click();
  expect(await otp()).toEqual({ phone: '+905550000001', purpose: 'Signup' });

  // Kod ekranından geri dönünce alanlar dolu kalır; ad kod doğrulanınca kayıt isteğiyle gider.
  await expect(page.getByText('2 / 2 · Telefon doğrulama')).toBeVisible();
  await page.getByRole('button', { name: 'Geri' }).click();
  await expect(page.getByLabel('Adınız soyadınız')).toHaveValue('  Elif   Kaya ');
  await expect(page.getByLabel('İşletme adı')).toHaveValue('Kuaför Elif');
  await page.getByRole('button', { name: 'SMS kodu gönder' }).click();

  await page.getByLabel('Doğrulama kodu, hane 1').click();
  await page.keyboard.type('482193');
  const signup = await captureBody(page, '/auth/signup', 'POST');
  await page.getByRole('button', { name: 'İşletmeyi oluştur' }).click();
  expect(await signup()).toMatchObject({
    signupToken: 'kayit-jetonu',
    ownerFullName: 'Elif Kaya',
    businessName: 'Kuaför Elif',
    industryCode: 'hair_salon',
    acceptedTerms: true,
  });

  // Panel: kurulum adımları (Prism örnek verisiyle; adım sayısı sözleşme örneğine bağlı).
  await expect(page.getByRole('heading', { level: 1, name: /^Hoş geldiniz/ })).toBeVisible();
  await expect(page.getByText(/tamamlandı$/)).toBeVisible();
  await page.getByRole('button', { name: 'Sonra yaparım' }).click();

  const event = await captureBody(page, '/onboarding/events', 'POST');
  await page.getByRole('button', { name: 'Kopyala' }).click();
  await expect(page.getByText('Bağlantı kopyalandı')).toBeVisible();
  expect(await event()).toEqual({ event: 'BookingLinkCopied' });
  expect(await page.evaluate(() => navigator.clipboard.readText())).toMatch(/^https?:\/\//);
});
