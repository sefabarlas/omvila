import { expect, test } from '@playwright/test';
import { calendarDemo } from './calendar-demo';
import { subscriptionSession } from './subscription-demo';

// Dilim 6 KK1 (salon paneli): ödeme gecikince turuncu (uyarı) şerit çıkar ve yeni randevu açık kalır; salt okunurda
// kırmızı şerit, "Randevu ekle" pasif ve nedeni yazar, takvimdeki boş dilimler tıklanabilir değildir (T-125 (8)), var
// olan randevular açılmaya devam eder. Rezervasyon sayfası tarafı: web/rezervasyon/e2e/kapali.e2e.ts.
test('ödeme gecikti: uyarı şeridi; takvimde yeni randevu açık (KK1)', async ({ page }) => {
  await calendarDemo(page);
  await subscriptionSession(page, {
    status: 'PastDue',
    trialEndsOn: null,
    pastDueSince: '2026-11-23',
    readOnlyOn: '2026-11-30',
  });
  await page.goto('/takvim');
  const calendar = page.getByRole('region', { name: 'Günlük takvim' });
  await expect(calendar).toBeVisible();

  const banner = page.getByRole('status').filter({ hasText: 'Abonelik ödemesi alınamadı' });
  await expect(banner).toContainText('3. gün');
  await expect(banner).toContainText('5 gün içinde havale ile ödemezseniz');
  await expect(banner).toHaveClass(/uyari/);
  await expect(page.getByRole('alert').filter({ hasText: 'Panel salt okunur' })).toHaveCount(0);

  await expect(page.getByRole('button', { name: 'Randevu ekle', exact: true })).toBeEnabled();
  await expect(
    calendar.getByRole('button', { name: 'Randevu ekle: Elif Kaya, 09:00' }),
  ).toBeVisible();
});

test('salt okunur: yeni randevu pasif, boş dilim buton değil, var olan randevu açılır (KK1)', async ({
  page,
}) => {
  await calendarDemo(page);
  await subscriptionSession(page, {
    status: 'ReadOnly',
    trialEndsOn: null,
    pastDueSince: '2026-11-18',
    readOnlyOn: null,
  });
  await page.goto('/takvim');
  const calendar = page.getByRole('region', { name: 'Günlük takvim' });
  await expect(calendar).toBeVisible();

  const banner = page.getByRole('alert').filter({ hasText: 'Panel salt okunur' });
  await expect(banner).toContainText('7 gündür alınamadı');
  await expect(banner).toHaveClass(/hata/);

  const add = page.getByRole('button', { name: 'Randevu ekle', exact: true });
  await expect(add).toBeDisabled();
  await expect(add).toHaveAccessibleDescription('Abonelik salt okunur; yeni randevu eklenemez.');

  // Erişilebilirlik ağacında "Randevu ekle: …" dilim butonu yok; ızgarada yalnız randevu blokları buton.
  await expect(page.getByRole('button', { name: /^Randevu ekle: / })).toHaveCount(0);
  await expect(calendar.locator('[data-slot]')).toHaveCount(0);
  const tree = await calendar.ariaSnapshot();
  expect(tree).not.toContain('Randevu ekle');
  expect(tree).toContain('Deniz Acar');

  // Boş alana tıklama yeni randevu penceresini açmaz.
  await calendar.locator('[data-column] [class*="cell"]').last().click();
  await expect(page.getByRole('dialog')).toHaveCount(0);

  // Var olan randevu açılır (salt okunurda randevular sürer).
  await calendar.getByRole('button', { name: /^Deniz Acar/ }).click();
  await expect(page.getByRole('dialog', { name: /^Randevu · / })).toBeVisible();
});
