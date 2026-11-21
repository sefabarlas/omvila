import { expect, test } from '@playwright/test';
import { calendarDemo } from './calendar-demo';
import { captureBody } from './support';

// Dilim 3: bildirim paneli (zil) ve "Kontör az" şeridi, Prism örnekleriyle (manageListInAppNotifications: 3 okunmamış;
// ManageSession.smsCredits: 180, isLow). Saat takvim demosundaki gibi 25 Kasım 2026 12:10 İstanbul. Görüntüler
// (BildirimPaneli, DurumKontorAz; açık ve koyu) e2e-results/ekranlar altına yazılır ve
// design/ekranlar/salon-paneli/BildirimPaneli ve Durumlar ile yan yana karşılaştırılır.
for (const theme of ['light', 'dark'] as const) {
  test(`bildirim paneli ve kontör az (${theme === 'light' ? 'açık' : 'koyu'})`, async ({
    page,
  }) => {
    const suffix = theme === 'dark' ? 'Koyu' : '';
    await page.emulateMedia({ colorScheme: theme });
    await page.addInitScript((value) => {
      try {
        localStorage.setItem('omvila.tema', value);
      } catch {
        // Depolama kapalıysa sistem teması kullanılır.
      }
    }, theme);
    await calendarDemo(page);
    await page.goto('/takvim');
    await expect(page.getByRole('heading', { level: 1, name: 'Takvim' })).toBeVisible();

    const credits = page.getByRole('status').filter({ hasText: 'SMS kontörünüz' });
    await expect(credits).toContainText("SMS kontörünüz 180'e düştü.");
    await credits.screenshot({ path: `e2e-results/ekranlar/DurumKontorAz${suffix}.png` });

    const bell = page.getByRole('button', { name: 'Bildirimler, 3 okunmamış' });
    await bell.click();
    const panel = page.getByRole('dialog', { name: 'Bildirimler' });
    await expect(panel.getByRole('listitem')).toHaveCount(3);
    await expect(panel).toContainText('1 sa 50 dk kaldı');
    await expect(panel).toContainText('Dün');
    await page.screenshot({ path: `e2e-results/ekranlar/BildirimPaneli${suffix}.png` });

    await page.keyboard.press('Escape');
    await expect(panel).toBeHidden();
    await expect(bell).toBeFocused();
  });
}

test('"Tümünü okundu say" ve "Kontör al"', async ({ page }) => {
  await calendarDemo(page);
  const markRead = await captureBody(page, '/notifications/read', 'POST');
  await page.goto('/takvim');
  await page.getByRole('button', { name: 'Bildirimler, 3 okunmamış' }).click();
  await page
    .getByRole('dialog', { name: 'Bildirimler' })
    .getByRole('button', { name: 'Tümünü okundu say' })
    .click();
  expect(await markRead()).toEqual({ all: true });
  await expect(page.getByRole('button', { name: 'Bildirimler', exact: true })).toBeVisible();

  await page.getByRole('button', { name: 'Kontör al' }).click();
  await expect(page).toHaveURL(/\/ayarlar\//);
});
