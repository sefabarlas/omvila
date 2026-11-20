import { describe, expect, it } from 'vitest';
import { roleLabel, workingSummary } from './labels';

// Kural: salon-paneli/PersonelBos ("Pzt–Cmt · 10:00–20:00"), roller-yetkiler.md (rol adları).
describe('personel etiketleri', () => {
  const day = (dayOfWeek: number, start = '10:00', end = '20:00') => ({
    dayOfWeek,
    intervals: [{ start, end }],
  });

  it('çalışma özeti', () => {
    expect(workingSummary({ days: [1, 2, 3, 4, 5, 6].map((d) => day(d)) })).toBe(
      'Pzt–Cmt · 10:00–20:00',
    );
    expect(workingSummary({ days: [day(2), day(6, '09:00', '18:00')] })).toBe(
      'Sal, Cmt · farklı saatler',
    );
    expect(workingSummary({ days: [] })).toBe('Saat girilmedi');
  });

  it('rol', () => {
    expect(roleLabel('Owner')).toBe('İşletme sahibi');
    expect(roleLabel(null)).toBe('Hesabı yok');
  });
});
