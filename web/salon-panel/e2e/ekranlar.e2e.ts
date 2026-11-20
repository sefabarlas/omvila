import { expect, test } from '@playwright/test';

// Görsel karşılaştırma (test-stratejisi.md): Dilim 1 ekranlarının açık ve koyu ekran görüntüleri e2e-results/ekranlar
// altına yazılır; tasarım görüntüleriyle (design/ekranlar/salon-paneli) yan yana insan gözüyle karşılaştırılır.
// Otomatik piksel karşılaştırması yok (Prism örnek verisi tasarımdaki verilerden farklı).
const screens = [
  { path: '/', name: 'GenelBakis', heading: /^(Hoş geldiniz|Günaydın|İyi günler|İyi akşamlar)/ },
  { path: '/hizmetler', name: 'Hizmetler', heading: 'Hizmetler' },
  { path: '/personel', name: 'Personel', heading: 'Personel' },
  { path: '/ayarlar/isletme', name: 'AyarlarIsletme', heading: 'Ayarlar' },
  { path: '/ayarlar/sube', name: 'AyarlarSube', heading: 'Ayarlar' },
  { path: '/ayarlar/randevu-kurallari', name: 'Ayarlar', heading: 'Ayarlar' },
  { path: '/ayarlar/rezervasyon', name: 'AyarlarRezervasyon', heading: 'Ayarlar' },
] as const;

for (const theme of ['light', 'dark'] as const) {
  for (const screen of screens) {
    test(`${screen.name} (${theme === 'light' ? 'açık' : 'koyu'})`, async ({ page }) => {
      const errors: string[] = [];
      page.on('pageerror', (error) => errors.push(error.message));
      await page.emulateMedia({ colorScheme: theme });
      await page.addInitScript((value) => {
        try {
          localStorage.setItem('omvila.tema', value);
        } catch {
          // Depolama kapalıysa sistem teması kullanılır.
        }
      }, theme);
      await page.goto(screen.path);
      await expect(page.getByRole('heading', { level: 1, name: screen.heading })).toBeVisible();
      await page.screenshot({
        path: `e2e-results/ekranlar/${screen.name}${theme === 'dark' ? 'Koyu' : ''}.png`,
        fullPage: true,
      });
      expect(errors).toEqual([]);
    });
  }
}
