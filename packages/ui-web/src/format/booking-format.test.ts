import { describe, expect, it } from 'vitest';
import { formatDate, formatIban, formatPrice, normalizeIban, withTimeSuffix } from './index';
import { initials } from './name';

// Kural: rezervasyon sayfası ekranlarındaki metinler birebir (design/ekranlar/rezervasyon-sayfasi).
describe('formatPrice', () => {
  it('sabit ve "den başlayan" fiyat', () => {
    expect(formatPrice({ amountMinor: 45000, currency: 'TRY' })).toBe('450 ₺');
    expect(formatPrice({ amountMinor: 280000, currency: 'TRY' }, 'StartingFrom')).toBe(
      "2.800 ₺'den",
    );
    expect(formatPrice({ amountMinor: 110000, currency: 'TRY' }, 'StartingFrom')).toBe(
      "1.100 ₺'den",
    );
  });
});

describe('formatDate rezervasyon biçimleri', () => {
  // 26 Kasım 2026 Perşembe, 27 Kasım 2026 Cuma, 28 Kasım 2026 Cumartesi.
  it.each([
    ['compact', '2026-11-27', 'Cum 27 Kas'],
    ['weekdayDayMonth', '2026-11-27', 'Cuma 27 Kasım'],
    ['monthYear', '2026-11-27', 'Kasım 2026'],
    ['weekday', '2026-11-26', 'Perşembe'],
    ['weekdayShort', '2026-11-28', 'Cmt'],
    ['day', '2026-11-05', '5'],
  ] as const)('%s', (style, date, expected) => {
    expect(formatDate(date, style)).toBe(expected);
  });
});

describe('formatIban', () => {
  it('dörtlü gruplar, kopyalanan değer boşluksuz', () => {
    expect(formatIban('TR120006400000112345678901')).toBe('TR12 0006 4000 0011 2345 6789 01');
    expect(normalizeIban('tr12 0006 4000 0011 2345 6789 01')).toBe('TR120006400000112345678901');
  });
});

describe('withTimeSuffix', () => {
  it.each([
    ['20:00', 'dative', "20:00'ye"],
    ['19:00', 'dative', "19:00'a"],
    ['10:00', 'locative', "10:00'da"],
    ['16:30', 'locative', "16:30'da"],
    ['13:00', 'locative', "13:00'te"],
    ['14:00', 'locative', "14:00'te"],
    ['18:40', 'locative', "18:40'ta"],
    ['12:00', 'locative', "12:00'de"],
    ['10:30', 'accusative', "10:30'u"],
    ['20:00', 'accusative', "20:00'yi"],
    ['14:45', 'accusative', "14:45'i"],
    ['09:15', 'accusative', "09:15'i"],
    ['00:00', 'accusative', "00:00'ı"],
    ['15:00', 'ablative', "15:00'ten"],
    ['11:30', 'ablative', "11:30'dan"],
  ] as const)('%s %s → %s', (time, kind, expected) => {
    expect(withTimeSuffix(time, kind)).toBe(expected);
  });

  it('biçimsiz saat hata', () => {
    expect(() => withTimeSuffix('9:30', 'dative')).toThrow(RangeError);
  });
});

describe('initials', () => {
  it('iki baş harf, Türkçe büyük harf', () => {
    expect(initials('Kuaför Elif')).toBe('KE');
    expect(initials('ilker şahin')).toBe('İŞ');
  });
});
