import { describe, expect, it } from 'vitest';
import { accusativeCount, dative, dativeCount, genitive, locativeTime } from './turkish';

// Kural: dilim-02 kabul kriteri 12 ("Zeynep Aksoy'a taşı"), RandevuOnayli ("Gelmezse 13:15'te otomatik").
describe('yönelme eki', () => {
  it.each([
    ['Zeynep Aksoy', "Zeynep Aksoy'a"],
    ['Mert Demir', "Mert Demir'e"],
    ['Can Yücel', "Can Yücel'e"],
    ['Elif Kaya', "Elif Kaya'ya"],
    ['Deniz Acar', "Deniz Acar'a"],
    ['Tuna Oral', "Tuna Oral'a"],
    ['Buse Kaya', "Buse Kaya'ya"],
    ['Hande Er', "Hande Er'e"],
    ['Gül', "Gül'e"],
    ['Mehmet', "Mehmet'e"],
    ['Ece', "Ece'ye"],
    ['Okan', "Okan'a"],
  ])('%s → %s', (name, expected) => {
    expect(dative(name)).toBe(expected);
  });

  it('cins adda kesme işareti yok: Salıya, Perşembeye, Pazara', () => {
    expect(dative('Salı', false)).toBe('Salıya');
    expect(dative('Perşembe', false)).toBe('Perşembeye');
    expect(dative('Pazar', false)).toBe('Pazara');
    expect(dative('Cuma', false)).toBe('Cumaya');
  });
});

describe('saatin bulunma eki', () => {
  it.each([
    ['13:15', "13:15'te"],
    ['13:00', "13:00'te"],
    ['12:30', "12:30'da"],
    ['12:45', "12:45'te"],
    ['09:40', "09:40'ta"],
    ['10:00', "10:00'da"],
    ['14:20', "14:20'de"],
    ['16:50', "16:50'de"],
    ['11:06', "11:06'da"],
    ['12:00', "12:00'de"],
    ['19:00', "19:00'da"],
  ])('%s → %s', (time, expected) => {
    expect(locativeTime(time)).toBe(expected);
  });
});

// Kural: Durumlar "Kontör az" ("SMS kontörünüz 180'e düştü."), binlik nokta (urun-kurallari.md § Biçimler).
describe('sayının yönelme eki', () => {
  it.each([
    [180, "180'e"],
    [1240, "1.240'a"],
    [6, "6'ya"],
    [200, "200'e"],
    [199, "199'a"],
    [50, "50'ye"],
    [3, "3'e"],
    [1000, "1.000'e"],
    [0, "0'a"],
    [2_000_000, "2.000.000'a"],
  ])('%s → %s', (n, expected) => {
    expect(dativeCount(n)).toBe(expected);
  });
});

// Kural: salon-paneli/MusteriSil ("Selin Öztürk'ün adı…"), Degerlendirmeler ("davet edilenlerin %38'i").
describe('tamlayan ve belirtme ekleri', () => {
  it.each([
    ['Selin Öztürk', "Selin Öztürk'ün"],
    ['Elif Kaya', "Elif Kaya'nın"],
    ['Tuna Oral', "Tuna Oral'ın"],
    ['Mert Demir', "Mert Demir'in"],
    ['Gül Ufuk', "Gül Ufuk'un"],
  ])('%s → %s', (name, expected) => {
    expect(genitive(name)).toBe(expected);
  });

  it.each([
    [38, "38'i"],
    [40, "40'ı"],
    [6, "6'yı"],
    [100, "100'ü"],
    [3, "3'ü"],
    [10, "10'u"],
    [0, "0'ı"],
  ])('%s → %s', (n, expected) => {
    expect(accusativeCount(n)).toBe(expected);
  });
});
