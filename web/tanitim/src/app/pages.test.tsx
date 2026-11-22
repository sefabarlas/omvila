import type { ReactElement } from 'react';
import { render, screen, within } from '@testing-library/react';
import { describe, expect, it, vi } from 'vitest';
import pricingJson from '@content/pricing.json';
import { SiteFooter } from '@/components/SiteFooter';
import { SiteHeader } from '@/components/SiteHeader';
import RootLayout from './layout';

vi.mock('next/navigation', () => ({ notFound: vi.fn() }));

async function renderPage(path: 'home' | 'pricing') {
  vi.stubEnv('NEXT_PUBLIC_PANEL_URL', 'https://salon.example.test');
  const module = path === 'home' ? await import('./page') : await import('./fiyatlar/page');
  const Page = module.default;
  // <html> jsdom'da body'ye çizilemez; düzenin gövdesi (başlık, alt bilgi) ayrıca sınanır.
  return render(<Page />);
}

function byId(id: string): HTMLElement {
  const element = document.getElementById(id);
  if (!element) throw new Error(`#${id} yok`);
  return element;
}

const bracketPlaceholder = /\[[A-ZÇĞİÖŞÜ ]+\]/;
const narrowing = /kuaför|berber|güzellik|salonlar için/i;

describe('ana sayfa', () => {
  it("hero, deneme süresi pricing.json'dan ve kayıt bağlantısı", async () => {
    await renderPage('home');
    expect(
      screen.getByRole('heading', { level: 1, name: 'Randevularınız kendiliğinden dolsun' }),
    ).toBeTruthy();
    expect(
      screen.getByText(
        `${String(pricingJson.trialDays)} gün ücretsiz · kart gerekmez · telefonla giriş`,
      ),
    ).toBeTruthy();
    for (const link of screen.getAllByRole('link', { name: /Ücretsiz dene/ })) {
      expect(link.getAttribute('href')).toBe('https://salon.example.test/kayit');
    }
  });

  it('özellik kartları, kapora cümlesi ve adımlar', async () => {
    await renderPage('home');
    const cards = within(byId('ozellikler')).getAllByRole('heading', { level: 3 });
    expect(cards.map((card) => card.textContent)).toEqual([
      'Kendi randevu bağlantınız',
      'SMS hatırlatma',
      'Kapora sizin hesabınıza',
      'Bekleme listesi',
      'Kapıda sıra',
      'Ekip takvimi',
    ]);
    expect(screen.getByText(/OMVILA ödemeye aracılık etmez\./)).toBeTruthy();
    expect(screen.getByText(/bir gün ve iki saat önce hatırlatma/)).toBeTruthy();
    expect(byId('nasil-calisir')).toBeTruthy();
    expect(screen.getByRole('heading', { name: 'Uygulama zorunlu değil' })).toBeTruthy();
  });

  it('sektöre daraltan, yer tutucu veya ödeme alan ifade yok', async () => {
    const { container } = await renderPage('home');
    const text = container.textContent;
    expect(text).not.toMatch(narrowing);
    expect(text).not.toMatch(bracketPlaceholder);
    expect(text).not.toMatch(/WhatsApp|yakında/i);
  });
});

describe('fiyatlar', () => {
  it('üç plan, önerilen rozeti ve değer yoksa "Fiyat yakında açıklanacak"', async () => {
    await renderPage('pricing');
    const plans = screen.getByRole('region', { name: 'Planlar' });
    const names = within(plans).getAllByRole('heading', { level: 2 });
    expect(names.map((name) => name.textContent)).toEqual(['Tekil', 'Salon', 'Salon Plus']);
    expect(within(plans).getAllByText('Önerilen')).toHaveLength(1);
    expect(within(plans).getAllByText('Fiyat yakında açıklanacak')).toHaveLength(3);
    expect(within(plans).getByText('Personel sınırı yakında açıklanacak')).toBeTruthy();
  });

  it("kontör paketleri ve karakter sınırı pricing.json'dan", async () => {
    await renderPage('pricing');
    for (const label of ['1.000 SMS', '5.000 SMS', '20.000 SMS']) {
      expect(screen.getByText(label)).toBeTruthy();
    }
    expect(screen.getByText(/155 karaktere kadar bir mesaj bir kontördür/)).toBeTruthy();
  });

  it('SSS yalnız cevabı olan ilk soruyu gösterir; kartla ödeme anılmaz', async () => {
    const { container } = await renderPage('pricing');
    const faq = byId('sss');
    expect(within(faq).getAllByRole('heading', { level: 3 })).toHaveLength(1);
    expect(
      within(faq).getByText(/^Plan seçip havaleyle ödersiniz\. Ödeme yapılmazsa 7 gün/),
    ).toBeTruthy();
    expect(container.textContent).not.toMatch(bracketPlaceholder);
    expect(container.textContent).not.toMatch(/salonlar|kuaför/i);
  });
});

describe('düzen', () => {
  it('lang="tr", menü ve yasal bağlantılar', () => {
    vi.stubEnv('NEXT_PUBLIC_PANEL_URL', 'https://salon.example.test');
    expect((RootLayout({ children: null }) as ReactElement<{ lang: string }>).props.lang).toBe(
      'tr',
    );
    render(
      <>
        <SiteHeader />
        <SiteFooter />
      </>,
    );
    const menu = screen.getByRole('navigation', { name: 'Ana menü' });
    expect(
      within(menu)
        .getAllByRole('link')
        .map((link) => link.textContent),
    ).toEqual(['Özellikler', 'Nasıl çalışır', 'Fiyatlar', 'Sık sorulanlar']);
    expect(
      screen.getByRole('banner').querySelector('a[href$="/giris"]')?.getAttribute('href'),
    ).toBe('https://salon.example.test/giris');
    const footer = screen.getByRole('contentinfo');
    // Sondaki eğik çizgiyi Next derlemede ekler (trailingSlash); testte Link onu atar.
    expect(
      within(footer).getByRole('link', { name: 'KVKK aydınlatma metni' }).getAttribute('href'),
    ).toMatch(/^\/yasal\/kvkk\/?$/);
  });
});
