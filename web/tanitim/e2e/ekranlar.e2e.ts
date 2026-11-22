import { expect, test } from '@playwright/test';

// Görsel karşılaştırma (test-stratejisi.md): sayfaların açık/koyu, masaüstü (1440 px)/mobil (390 px) görüntüleri
// e2e-results/ekranlar altına yazılır; design/ekranlar/tanitim-sitesi ile yan yana insan gözüyle karşılaştırılır.
// Otomatik piksel karşılaştırması yok. Dosya adları tasarımla aynı: Main, MainKoyu, MainMobil, MainMobilKoyu…
const pages = [
  { name: 'Main', path: '/', heading: 'Randevularınız kendiliğinden dolsun' },
  { name: 'Fiyatlar', path: '/fiyatlar/', heading: 'İşletmenize göre plan' },
] as const;

for (const mobile of [false, true]) {
  for (const theme of ['light', 'dark'] as const) {
    for (const page of pages) {
      const name = `${page.name}${mobile ? 'Mobil' : ''}${theme === 'dark' ? 'Koyu' : ''}`;
      test(name, async ({ page: tab }) => {
        const errors: string[] = [];
        tab.on('pageerror', (error) => errors.push(error.message));
        if (mobile) await tab.setViewportSize({ width: 390, height: 844 });
        await tab.emulateMedia({ colorScheme: theme });
        await tab.goto(page.path);
        await expect(tab.getByRole('heading', { level: 1, name: page.heading })).toBeVisible();
        await expect(tab.locator('html')).toHaveAttribute('data-theme', theme);
        // Tembel yüklenen görseller tam sayfa görüntüsüne girsin.
        for (const img of await tab.locator('img:visible').all()) {
          await img.scrollIntoViewIfNeeded();
          await expect(img).toHaveJSProperty('complete', true);
        }
        await tab.screenshot({
          path: `e2e-results/ekranlar/${name}.png`,
          fullPage: true,
          animations: 'disabled',
        });
        expect(errors).toEqual([]);
      });
    }
  }
}
