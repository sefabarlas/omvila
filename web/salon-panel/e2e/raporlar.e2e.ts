import { expect, test } from '@playwright/test';
import { reportsDemo } from './reports-demo';

// Dilim 6 KK6 (salon paneli): Raporlar ve prim'de hak edilen gelir ile tahsilat ayrı kartlardır (peşin satılan paket
// ve kapora tahsilatta, gelir seans kullanıldıkça); dönem adreste; veri yokken boş hâl sahte sayı göstermez. Tutarların
// hesabı API testlerindedir (ReportTests); burada ekranın iki rakamı ayrı ve doğru etiketle gösterdiği denetlenir.
test('raporlar: hak edilen gelir ve tahsilat ayrı kartlarda; prim tablosu (KK6)', async ({
  page,
}) => {
  await reportsDemo(page);
  const queries: URL[] = [];
  page.on('request', (request) => {
    const url = new URL(request.url());
    if (request.method() === 'GET' && url.pathname.endsWith('/reports/overview')) {
      queries.push(url);
    }
  });
  await page.goto('/raporlar');
  await expect(page.getByRole('heading', { level: 1, name: 'Raporlar ve prim' })).toBeVisible();

  // Hak edilen 61.200 ₺ (tamamlanan randevular); tahsilat 67.950 ₺ (paket satışı ve kapora dahil): iki ayrı rakam.
  const earned = page.getByText('Hak edilen gelir', { exact: true }).locator('..');
  await expect(earned).toContainText('61.200 ₺');
  await expect(earned).toContainText('tamamlanan 118 randevu');
  const collected = page.getByText('Tahsilat', { exact: true }).first().locator('..');
  await expect(collected).toContainText('67.950 ₺');
  await expect(collected).toContainText('paket satışları ve kaporalar dahil');
  await expect(earned).not.toContainText('67.950 ₺');

  // Günlük tahsilat tablosu (grafiğin erişilebilir karşılığı) ve prim tablosu.
  await page.getByText('Tablo olarak göster').click();
  const daily = page.getByRole('table', { name: 'Günlük tahsilat' });
  await expect(daily.getByRole('row').filter({ hasText: 'Cumartesi' })).toContainText('18.450 ₺');
  const staff = page.getByRole('region', { name: 'Personel ve prim' });
  await expect(staff.getByRole('row').filter({ hasText: 'Elif Kaya' })).toContainText('5.832 ₺');
  await expect(staff.getByText('15.665 ₺')).toBeVisible();

  // Hafta: Pazartesi – bugün (29 Kasım Pazar). Ay: ayın 1'i – bugün; dönem adreste.
  expect(queries[0]?.searchParams.get('from')).toBe('2026-11-23');
  expect(queries[0]?.searchParams.get('to')).toBe('2026-11-29');
  await page.getByRole('radio', { name: 'Ay' }).check();
  await expect(page).toHaveURL(/\/raporlar\?donem=ay$/);
  await expect.poll(() => queries.at(-1)?.searchParams.get('from')).toBe('2026-11-01');
  expect(queries.at(-1)?.searchParams.get('to')).toBe('2026-11-29');
});

test('raporlar boş: sahte sayı yok, kartlar çizgiyle (KK6)', async ({ page }) => {
  await reportsDemo(page, true);
  await page.goto('/raporlar');
  await expect(page.getByRole('heading', { name: 'Rapor için henüz veri yok' })).toBeVisible();
  const earned = page.getByText('Hak edilen gelir', { exact: true }).locator('..');
  await expect(earned).toContainText('—');
  await expect(earned).toContainText('tamamlanan randevu yok');
  const collected = page.getByText('Tahsilat', { exact: true }).locator('..');
  await expect(collected).toContainText('—');
  await expect(page.getByText(/\d ₺/)).toHaveCount(0);
  await expect(page.getByRole('region', { name: 'Personel ve prim' })).toHaveCount(0);
});
