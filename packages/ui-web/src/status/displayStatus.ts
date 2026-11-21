// Randevu durumunun gösterimi (StatusBadge README, urun-kurallari.md: durum adları sabit). "Kapora bekleniyor"
// türetilmiş gösterimdir (API displayStatus = AwaitingDeposit). Draft hiçbir ekranda gösterilmez.

export const statusKeys = [
  'onay',
  'kapora',
  'onaylandi',
  'geldi',
  'tamamlandi',
  'gelmedi',
  'iptal',
] as const;
export type StatusKey = (typeof statusKeys)[number];

export const statusLabels: Record<StatusKey, string> = {
  onay: 'Onay bekliyor',
  kapora: 'Kapora bekleniyor',
  onaylandi: 'Onaylandı',
  geldi: 'Geldi',
  tamamlandi: 'Tamamlandı',
  gelmedi: 'Gelmedi',
  iptal: 'İptal',
};

const byApiStatus: Record<string, StatusKey> = {
  PendingApproval: 'onay',
  // Sözleşmedeki DisplayStatus adı DepositPending; AwaitingDeposit eski ad olarak kalır.
  DepositPending: 'kapora',
  AwaitingDeposit: 'kapora',
  Confirmed: 'onaylandi',
  CheckedIn: 'geldi',
  Completed: 'tamamlandi',
  NoShow: 'gelmedi',
  Cancelled: 'iptal',
};

/** API durumu (veya displayStatus) → gösterim anahtarı; Draft ve bilinmeyen değer null. */
export function displayStatus(apiStatus: string): StatusKey | null {
  return byApiStatus[apiStatus] ?? null;
}
