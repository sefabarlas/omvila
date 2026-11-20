import { expect, test } from '@playwright/test';
import { captureBody, signedOut, unregisteredPhone } from './support';

// Kritik akış 1 (test-stratejisi.md): salon kaydı → kurulum adımları → rezervasyon bağlantısını kopyalama.
test('kayıt → kurulum adımları → bağlantıyı kopyalama', async ({ page, context }) => {
  await context.grantPermissions(['clipboard-read', 'clipboard-write']);
  await signedOut(page);
  await unregisteredPhone(page);

  await page.goto('/kayit');
  await expect(
    page.getByRole('heading', { level: 1, name: 'İşletmenizi oluşturun' }),
  ).toBeVisible();
  await page.getByLabel('İşletme adı').fill('Kuaför Elif');
  await page.getByLabel('Ne tür bir işletme?').selectOption('hair_salon');
  await page.getByLabel('Cep telefonunuz').fill('0555 000 00 01');
  await page
    .getByRole('checkbox', { name: 'Kullanım koşullarını ve aydınlatma metnini okudum' })
    .check();
  const otp = await captureBody(page, '/auth/otp/request', 'POST');
  await page.getByRole('button', { name: 'SMS kodu gönder' }).click();
  expect(await otp()).toEqual({ phone: '+905550000001', purpose: 'Signup' });

  await page.getByLabel('Doğrulama kodu, hane 1').click();
  await page.keyboard.type('482193');
  const signup = await captureBody(page, '/auth/signup', 'POST');
  await page.getByRole('button', { name: 'İşletmeyi oluştur' }).click();
  expect(await signup()).toMatchObject({
    signupToken: 'kayit-jetonu',
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
