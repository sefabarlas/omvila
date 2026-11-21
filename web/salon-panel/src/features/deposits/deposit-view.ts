// Kapora gösterim kuralları (dilim-04; sözleşme DepositStatus). "Kapora bekleniyor" türetilmiş gösterimdir
// (displayStatus DepositPending); müşteri havaleyi bildirdiyse panel "Havale bildirildi · kontrol edin" yazar.
import type { components } from '@omvila/api-client-ts';

export type DepositStatus = components['schemas']['DepositStatus'];

/** Kapora istenmiş ve henüz işaretlenmemiş (Awaiting veya CustomerReported): "Kapora geldi" / "Hatırlat" sunulur. */
export function isDepositAwaited(status: DepositStatus | null | undefined): boolean {
  return status === 'Awaiting' || status === 'CustomerReported';
}

/** Takvim bloğundaki küçük işaret. */
export const transferReportedNote = 'Havale bildirildi';

/** Yan panel şeridi ve Genel bakış satırı. */
export const transferReportedTitle = 'Havale bildirildi · kontrol edin';

/** KaporaOnay'daki yönlendirme metni. */
export const depositCheckHint =
  'Banka uygulamanızda açıklama kodunu arayın. İşaretleyince randevu kesinleşir ve müşteriye SMS gider.';
