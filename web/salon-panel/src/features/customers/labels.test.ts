import { describe, expect, it } from 'vitest';
import { inlineDayLabel } from './labels';

// Cümle içindeki gün ("15 ziyaret · son bugün", "son 21 Ekim"): göreli gün küçük, ay adı büyük kalır.
describe('inlineDayLabel', () => {
  it('göreli günü küçük harfle yazar', () => {
    expect(inlineDayLabel('2026-11-25', '2026-11-25')).toBe('bugün');
    expect(inlineDayLabel('2026-11-24', '2026-11-25')).toBe('dün');
  });

  it('tarihte ay adını büyük bırakır', () => {
    expect(inlineDayLabel('2026-10-21', '2026-11-25')).toBe('21 Ekim');
  });
});
