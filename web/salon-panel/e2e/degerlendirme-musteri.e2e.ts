import { expect, test } from '@playwright/test';
import { customersDemo, demoCustomerId } from './customers-demo';
import { reviewsDemo } from './reviews-demo';
import { captureBody } from './support';

// Dilim 5 uçtan uca (KK8, KK9 salon tarafı): değerlendirmeye yanıt; müşteri kartında KVKK silme; CSV içe aktarma.
// Yanıt ve önizleme verisi demo yönlendirmelerinden, silme ve içe aktarma uygulaması Prism'den döner.
test('değerlendirmeye yanıt: kart "Yanıtınız" bloğuyla Tümü filtresinde', async ({ page }) => {
  await reviewsDemo(page);
  await page.goto('/degerlendirmeler');
  const card = page.getByRole('article', { name: 'Ece Koç değerlendirmesi' });
  await card
    .getByPlaceholder('Yanıtınızı yazın, müşteri görür')
    .fill('Geri bildiriminiz için teşekkür ederiz.');
  await card.getByRole('button', { name: 'Yanıtla' }).click();
  await expect(page.getByText('Yanıtınız yayımlandı')).toBeVisible();
  await expect(card).toBeHidden();
  await page.getByText('Tümü', { exact: true }).click();
  await expect(
    page
      .getByRole('article', { name: 'Ece Koç değerlendirmesi' })
      .getByText('Yanıtınız · Elif Kaya'),
  ).toBeVisible();
});

test('KVKK silme: ad yazılınca anonimleştirir, listeye döner', async ({ page }) => {
  await customersDemo(page);
  await page.goto(`/musteriler/${demoCustomerId}`);
  await page.getByRole('button', { name: 'KVKK silme talebi' }).click();
  const dialog = page.getByRole('dialog', { name: 'KVKK silme talebi' });
  const submit = dialog.getByRole('button', { name: 'Kalıcı olarak anonimleştir' });
  await expect(submit).toBeDisabled();
  await dialog.getByLabel('Onaylamak için müşterinin adını yazın').fill('Selin Öztürk');
  const body = await captureBody(page, `/customers/${demoCustomerId}/deletion`, 'POST');
  await submit.click();
  expect(await body()).toEqual({ confirmFullName: 'Selin Öztürk' });
  await expect(page.getByText('Müşteri kaydı anonimleştirildi')).toBeVisible();
  await expect(page).toHaveURL(/\/musteriler$/);
});

test('CSV içe aktarma: önizleme ve uygulama', async ({ page }) => {
  await customersDemo(page);
  await page.goto('/musteriler');
  await page.getByRole('button', { name: 'İçe aktar' }).click();
  const dialog = page.getByRole('dialog', { name: 'Müşterileri içe aktar' });
  await dialog.getByLabel('CSV dosyası').setInputFiles({
    name: 'musteriler-eski-program.csv',
    mimeType: 'text/csv',
    buffer: Buffer.from('Ad;Telefon\nSelin Öztürk;0532 418 33 21\n'),
  });
  await expect(dialog.getByText('1.248 satır · Ad, Telefon, Not sütunları eşlendi')).toBeVisible();
  const download = page.waitForEvent('download');
  await dialog.getByRole('button', { name: 'Hatalı satırları indir' }).click();
  expect((await download).suggestedFilename()).toBe('musteriler-eski-program-hatali-satirlar.csv');
  await dialog.getByRole('button', { name: '1.242 kaydı içe aktar' }).click();
  await expect(page.getByText(/kayıt içe aktarıldı$/)).toBeVisible();
});
