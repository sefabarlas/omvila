// Sorgu anahtarları tek yerden (kodlama-typescript.md § API erişimi).
export const queryKeys = {
  session: () => ['session'] as const,
  overview: () => ['overview'] as const,
  /** Marka listesi (süzgeç + arama); önek ['brands'] bütün marka sorgularını geçersizler. */
  brands: (filter: string, q: string) => ['brands', 'list', filter, q] as const,
  brand: (brandId: string) => ['brands', brandId] as const,
  /** Marka detayı sekmeleri (fatura, kontör, ödemeler, denetim). */
  brandTab: (brandId: string, tab: string) => ['brands', brandId, tab] as const,
  plans: () => ['plans'] as const,
};
