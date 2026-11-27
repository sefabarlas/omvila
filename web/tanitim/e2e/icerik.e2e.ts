import { readFileSync } from 'node:fs';
import path from 'node:path';
import { expect, test } from '@playwright/test';

// Dilim 6 KK7: sitede köşeli parantezli yer tutucu, uydurma sayı, yorum veya marka logosu yok; fiyatlar ve plan
// sınırları content/pricing.json'dan (değer yoksa "yakında açıklanacak"); "Ücretsiz dene" salon paneli kaydına gider.
interface Feature {
  text?: string;
  limit?: number | null;
  unit?: string;
  pendingText?: string;
}
interface Pricing {
  trialDays: number;
  currency: string;
  plans: { name: string; monthlyPriceMinor: number | null; features: Feature[] }[];
  creditPackages: { credits: number; priceMinor: number | null }[];
}
const pricing = JSON.parse(
  readFileSync(path.join(process.cwd(), 'content', 'pricing.json'), 'utf8'),
) as Pricing;

const count = (value: number) => new Intl.NumberFormat('tr-TR').format(value);
/** ui-web formatMoney ile aynı gösterim: "1.250 ₺", kuruş varsa "85,50 ₺". */
const money = (minor: number) =>
  `${new Intl.NumberFormat('tr-TR', {
    minimumFractionDigits: minor % 100 === 0 ? 0 : 2,
    maximumFractionDigits: 2,
  }).format(minor / 100)} ₺`;
const pending = 'Fiyat yakında açıklanacak';
const signUp = 'https://salon.example.test/kayit';

for (const route of ['/', '/fiyatlar/']) {
  test(`${route}: yer tutucu, uydurma sayı, yorum veya logo yok (KK7)`, async ({ page }) => {
    await page.goto(route);
    const text = await page.locator('body').innerText();
    // [AYLIK FİYAT], [PLAN SINIRI], [OMVILA IBAN]…
    expect(text).not.toMatch(/\[[^\]\n]{2,40}\]/);
    // "10.000 salon", "5.000+ işletme", "%98 memnuniyet" gibi kanıtsız sayılar.
    expect(text).not.toMatch(/\d[\d.]*\s*\+?\s*(salon|işletme|müşteri|kullanıcı)\b/i);
    expect(text).not.toMatch(/memnuniyet|müşterilerimiz ne diyor|bize güvenen/i);
    await expect(page.locator('blockquote')).toHaveCount(0);
    // Görseller yalnız OMVILA logosu ve ürünün kendi ekran görüntüleri.
    for (const img of await page.locator('img').all()) {
      const src = (await img.getAttribute('src')) ?? '';
      expect(src).toMatch(/ekranlar|logo|omvila/i);
    }
  });

  test(`${route}: her "Ücretsiz dene" salon paneli kaydına gider (KK7)`, async ({ page }) => {
    await page.goto(route);
    const links = page.getByRole('link', { name: /ücretsiz dene/i });
    expect(await links.count()).toBeGreaterThan(1);
    for (const link of await links.all()) {
      await expect(link).toHaveAttribute('href', signUp);
    }
  });
}

test("fiyatlar: plan fiyatı, sınırlar ve kontör paketleri pricing.json'dan (KK7)", async ({
  page,
}) => {
  await page.goto('/fiyatlar/');
  const plans = page.getByRole('region', { name: 'Planlar' });
  await expect(plans.getByRole('heading', { level: 2 })).toHaveText(
    pricing.plans.map((plan) => plan.name),
  );
  for (const plan of pricing.plans) {
    const card = plans.getByRole('listitem').filter({
      has: page.getByRole('heading', { level: 2, name: plan.name, exact: true }),
    });
    await expect(card).toContainText(
      plan.monthlyPriceMinor === null ? pending : money(plan.monthlyPriceMinor),
    );
    for (const feature of plan.features) {
      const expected =
        feature.text ??
        (feature.limit == null
          ? (feature.pendingText ?? '')
          : `${count(feature.limit)} ${feature.unit ?? ''}`);
      await expect(card).toContainText(expected);
    }
    await expect(card.getByRole('link')).toHaveAttribute('href', signUp);
  }

  for (const pack of pricing.creditPackages) {
    const row = page.getByText(`${count(pack.credits)} SMS`, { exact: true }).locator('..');
    await expect(row).toContainText(pack.priceMinor === null ? pending : money(pack.priceMinor));
  }
});

test("ana sayfa: deneme süresi pricing.json'dan", async ({ page }) => {
  await page.goto('/');
  await expect(
    page.getByText(new RegExp(`${String(pricing.trialDays)} gün`)).first(),
  ).toBeVisible();
});
