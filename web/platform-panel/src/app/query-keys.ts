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
  creditPackages: () => ['credit-packages'] as const,
  /** Onay kuyruğu; önek ['approvals'] kuyruğu ve bildirim ayrıntılarını geçersizler. */
  approvalQueue: () => ['approvals', 'queue'] as const,
  paymentNotice: (noticeId: string) => ['approvals', 'notice', noticeId] as const,
  bankTransactions: (q: string, unmatchedOnly: boolean) =>
    ['bank-transactions', q, unmatchedOnly] as const,
  subscriptions: (filter: string, q: string) => ['subscriptions', filter, q] as const,
  auditLog: (category: string, q: string, brandId: string, period: string) =>
    ['audit-log', category, q, brandId, period] as const,
};
