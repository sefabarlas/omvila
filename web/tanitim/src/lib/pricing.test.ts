import { describe, expect, it } from 'vitest';
import { featureText, monthlyNote, priceText, pricing, trialEndAnswer, trialText } from './pricing';
import type { Pricing } from './pricing';

const base: Pricing = { ...pricing, trialDays: 30, pastDueGraceDays: 5 };

describe('pricing.json', () => {
  it('üç plan, tek önerilen ve üç kontör paketi tanımlar', () => {
    expect(pricing.plans.map((plan) => plan.code)).toEqual(['solo', 'salon', 'salon_plus']);
    expect(pricing.plans.filter((plan) => plan.recommended)).toHaveLength(1);
    expect(pricing.creditPackages.map((pack) => pack.credits)).toEqual([1000, 5000, 20000]);
  });

  it('köşeli parantezli yer tutucu içermez', () => {
    expect(JSON.stringify(pricing)).not.toMatch(/\[[A-ZÇĞİÖŞÜ ]+\]/);
  });
});

describe('metinler', () => {
  it('deneme süresi ve ödeme gecikmesi yapılandırmadan', () => {
    expect(trialText(base)).toBe('30 gün ücretsiz');
    expect(trialEndAnswer(base)).toContain('5 gün hatırlatma');
  });

  it('kartla ödeme kapalıyken yalnız havale anılır (T-06)', () => {
    expect(trialEndAnswer({ ...base, cardPaymentEnabled: false })).toMatch(/^Plan seçip havaleyle/);
    expect(trialEndAnswer({ ...base, cardPaymentEnabled: true })).toMatch(
      /^Plan seçip kartla ya da havaleyle/,
    );
  });

  it('fiyat yoksa null, varsa kuruştan biçimlenir', () => {
    expect(priceText(null)).toBeNull();
    expect(priceText(149900)).toBe('1.499 ₺');
  });

  it('KDV notu yoksa yalnız "aylık"', () => {
    expect(monthlyNote({ ...base, vatNote: null })).toBe('aylık');
    expect(monthlyNote({ ...base, vatNote: 'KDV dahil' })).toBe('aylık · KDV dahil');
  });

  it('plan sınırı yoksa bekleyen metin, varsa sayı', () => {
    const pending = {
      limit: null,
      unit: 'personel',
      pendingText: 'Personel sınırı yakında açıklanacak',
    };
    expect(featureText(pending)).toBe('Personel sınırı yakında açıklanacak');
    expect(featureText({ ...pending, limit: 5 })).toBe('5 personel');
    expect(featureText({ text: 'Öncelikli destek' })).toBe('Öncelikli destek');
  });
});
