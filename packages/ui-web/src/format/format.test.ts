import { describe, expect, it } from 'vitest';
import {
  formatDate,
  formatDuration,
  formatMoney,
  formatPhone,
  formatTime,
  maskPhone,
} from './index';

// Kural: docs/urun/urun-kurallari.md § Biçimler — örnekler birebir.
describe('formatMoney', () => {
  it.each([
    [125000, '1.250 ₺'],
    [8550, '85,50 ₺'],
    [45000, '450 ₺'],
    [0, '0 ₺'],
    [123456789, '1.234.567,89 ₺'],
    [5, '0,05 ₺'],
  ])('%i kuruş → %s', (amountMinor, expected) => {
    expect(formatMoney({ amountMinor, currency: 'TRY' })).toBe(expected);
  });

  it('kuruş tamsayı değilse hata', () => {
    expect(() => formatMoney({ amountMinor: 1.5, currency: 'TRY' })).toThrow(RangeError);
  });
});

describe('formatDate', () => {
  // Belgedeki örnek yılsız; 21 Kasım 2025 Cuma'dır.
  it.each([
    ['long', '21 Kasım Cuma'],
    ['short', '21 Kas Cum'],
    ['numeric', '21.11.2025'],
  ] as const)('%s', (style, expected) => {
    expect(formatDate('2025-11-21', style)).toBe(expected);
  });

  it('panel: "Salı, 25 Kasım"', () => {
    expect(formatDate('2026-11-25', 'panel')).toBe('Çarşamba, 25 Kasım');
    expect(formatDate('2026-11-24', 'panel')).toBe('Salı, 24 Kasım');
  });

  it('geçersiz tarih hata', () => {
    expect(() => formatDate('2026-02-30')).toThrow(RangeError);
  });
});

describe('formatTime', () => {
  it('şubenin saat diliminde 24 saat', () => {
    expect(formatTime('2026-11-25T06:30:00Z', 'Europe/Istanbul')).toBe('09:30');
    expect(formatTime('2026-11-25T11:45:00Z', 'Europe/Istanbul')).toBe('14:45');
    expect(formatTime('2026-11-25T21:05:00Z', 'Europe/Istanbul')).toBe('00:05');
  });
});

describe('formatDuration', () => {
  it.each([
    [45, '45 dk'],
    [90, '1 sa 30 dk'],
    [120, '2 sa'],
  ])('%i → %s', (minutes, expected) => {
    expect(formatDuration(minutes)).toBe(expected);
  });
});

describe('telefon', () => {
  it('gösterim ve maske', () => {
    expect(formatPhone('+905321234567')).toBe('0532 123 45 67');
    expect(maskPhone('+905321234567')).toBe('0532 *** ** 67');
  });
});
