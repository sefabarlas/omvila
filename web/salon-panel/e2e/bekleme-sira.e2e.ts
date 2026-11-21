import { expect, test } from '@playwright/test';
import { newAppointmentDemo } from './new-appointment-demo';
import { queuesDemo } from './queues-demo';
import { captureBody } from './support';

// Dilim 5 KK3 ve KK4, panel tarafı: bekleme listesinde "Saat öner" ve "Teklifi geri al" (If-Match), kapıda sırada
// "Sıraya ekle" (telefonsuz bilet) ve "Randevuya çevir". Liste verisi queues-demo.ts; yazma uçları Prism'e gider
// (yanıt sözleşme örneği); boş saatler new-appointment-demo.ts. Saat: 25 Kasım 2026 12:10 İstanbul.

test('bekleme listesi: teklifi geri al ve saat öner', async ({ page }) => {
  // Hizmetler, takvimin personeli ve boş saatler yeni randevu demosundan (Saat öner aynı ızgarayı kullanır).
  await newAppointmentDemo(page);
  await queuesDemo(page);
  const withdraw = await captureBody(page, '/waitlist/*/offer/withdraw', 'POST');
  await page.goto('/bekleme');
  await expect(
    page.getByText("Nur Ateş'e teklif gönderildi, 11 dakika içinde yanıt bekleniyor."),
  ).toBeVisible();
  const ifMatch = page.waitForRequest(
    (request) => request.url().endsWith('/offer/withdraw') && request.method() === 'POST',
  );
  await page.getByRole('button', { name: 'Teklifi geri al: Nur Ateş' }).click();
  expect((await ifMatch).headers()['if-match']).toBe('W/"1"');
  await withdraw();
  await expect(page.getByText('Teklif geri alındı')).toBeVisible();

  const offer = await captureBody(page, '/waitlist/*/offer', 'POST');
  await page.getByRole('button', { name: 'Saat öner: Tuğba Er' }).click();
  const dialog = page.getByRole('dialog', { name: 'Saat öner' });
  await dialog.getByRole('radio', { name: '14:30' }).check();
  await dialog.getByRole('button', { name: 'Teklifi gönder' }).click();
  const body = (await offer()) as { startUtc: string; staffId: string };
  expect(body.staffId).toBe('0193a8f0-0000-7000-8000-0000000000c1');
  expect(body.startUtc).toMatch(/Z$/);
  await expect(dialog).toBeHidden();
});

test('kapıda sıra: telefonsuz sıraya ekle ve ilk kişiyi randevuya çevir', async ({ page }) => {
  await queuesDemo(page);
  const create = await captureBody(page, '/walk-in-tickets', 'POST');
  await page.goto('/takvim/kapida-sira');
  await expect(page.getByRole('row', { name: /^8 Okan Er/ })).toBeVisible();

  await page.getByRole('button', { name: 'Sıraya ekle' }).click();
  const dialog = page.getByRole('dialog', { name: 'Sıraya ekle' });
  await expect(dialog.getByText('Sıradaki numara: No 11')).toBeVisible();
  await dialog.getByLabel('Ad', { exact: true }).fill('Burak');
  await dialog.getByRole('button', { name: 'Sıraya ekle' }).click();
  expect(await create()).toMatchObject({ fullName: 'Burak', preferredStaffId: null });
  await expect(page.getByText(/^Sıraya eklendi · No \d+$/)).toBeVisible();

  const start = page.waitForRequest(
    (request) => request.url().endsWith('/start') && request.method() === 'POST',
  );
  await page.getByRole('button', { name: 'Randevuya çevir: Okan Er' }).click();
  expect((await start).headers()['if-match']).toBe('W/"1"');
  await expect(page.getByText('Randevuya çevrildi')).toBeVisible();
});
