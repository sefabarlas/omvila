import { describe, expect, it } from 'vitest';
import { depositLabel, formatServicePrice } from './labels';

// Kural: salon-paneli/Hizmetler (fiyat "450 ₺", "1.800 ₺'den"; kapora "Yok", "Her zaman · 300 ₺", "Her zaman · %30"),
// dilim-01 (IBAN yoksa kapora istenemez).
describe('hizmet etiketleri', () => {
  it('fiyat sabit veya en az', () => {
    const price = { amountMinor: 180000, currency: 'TRY' as const };
    expect(
      formatServicePrice({ price: { amountMinor: 45000, currency: 'TRY' }, priceDisplay: 'Fixed' }),
    ).toBe('450\u00a0₺');
    expect(formatServicePrice({ price, priceDisplay: 'StartingFrom' })).toBe("1.800\u00a0₺'den");
  });

  it('kapora', () => {
    expect(depositLabel({ mode: 'Never' }, true)).toBe('Yok');
    expect(depositLabel({ mode: 'Inherit' }, true)).toBe('Genel ayar');
    expect(
      depositLabel({ mode: 'Always', amount: { amountMinor: 30000, currency: 'TRY' } }, true),
    ).toBe('Her zaman · 300\u00a0₺');
    expect(depositLabel({ mode: 'Always', percent: 30 }, true)).toBe('Her zaman · %30');
    expect(depositLabel({ mode: 'Always', percent: 30 }, false)).toBe('Yok');
  });
});
