import { expect, test } from '@playwright/test';
import { cashDemo, checkoutDemo, selinAppointmentId } from './cash-demo';
import { captureBody } from './support';

// Dilim 5 akışları, panel tarafı (dilim-05 KK5, KK7): Geldi randevuda "Tamamla ve ödeme al" → Kart 1.240 ₺ →
// manageCompleteAppointment gövdesi; Kasa'da sayım (3.400 ₺, beklenen 3.450 ₺ → fark −50 ₺) ve açıklamayla "Günü
// kapat" → manageCloseCashDay gövdesi. Yanıtlar Prism örneğinden (Completed, Closed). Saat 25 Kasım 2026 12:10 İstanbul.
test('tamamla ve ödeme al: kapora düşülür, Kart 1.240 ₺ kaydedilir (KK5)', async ({ page }) => {
  await checkoutDemo(page);
  const body = await captureBody(page, `/appointments/${selinAppointmentId}/complete`, 'POST');
  await page.goto('/takvim');
  await page.getByRole('button', { name: /^Selin Öztürk/ }).click();
  await page.getByRole('button', { name: 'Tamamla ve ödeme al' }).click();
  const dialog = page.getByRole('dialog', { name: 'Tamamla ve ödeme al' });
  await expect(dialog.getByText('Kapora (havale, 11 Kas)')).toBeVisible();
  await expect(dialog.getByText('−200 ₺')).toBeVisible();
  await expect(dialog.getByText('1.240 ₺')).toBeVisible();
  await dialog.getByText('Kart', { exact: true }).click();
  await expect(dialog.getByLabel('Tutar')).toHaveValue('1.240');
  await dialog.getByRole('button', { name: 'Tamamla ve kaydet' }).click();
  expect(await body()).toEqual({
    payments: [{ method: 'Card', amount: { amountMinor: 124000, currency: 'TRY' } }],
  });
  await expect(page.getByText('Randevu tamamlandı')).toBeVisible();
});

test('kasa sayımı: fark −50 ₺ açıklamayla, Günü kapat (KK7)', async ({ page }) => {
  await cashDemo(page);
  const body = await captureBody(page, '/cash/close', 'POST');
  await page.goto('/kasa');
  const count = page.getByRole('form', { name: 'Kasa sayımı' });
  await count.getByLabel('Sayılan nakit').fill('3.400');
  await expect(count.getByText('−50 ₺')).toBeVisible();
  await page.getByRole('button', { name: 'Günü kapat' }).click();
  await expect(count.getByText('Fark var. Nedenini yazın.')).toBeVisible();
  await count.getByLabel('Fark açıklaması').fill('Bozuk para verildi');
  await page.getByRole('button', { name: 'Günü kapat' }).click();
  expect(await body()).toMatchObject({
    date: '2026-11-25',
    countedCash: { amountMinor: 340000, currency: 'TRY' },
    differenceNote: 'Bozuk para verildi',
  });
  await expect(page.getByRole('region', { name: 'Gün kapatıldı' })).toBeVisible();
});
