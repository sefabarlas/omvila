import { withPossessive } from '@/lib/suffix';
import type { UsablePackageItem } from './booking-api';

// Onay ekranının paket kutusu (Onay.png, KK6): seçilen her hizmet için müşterinin kullanabileceği bir paket seansı.

export interface PackageOption {
  serviceId: string;
  serviceName: string;
  item: UsablePackageItem;
}

/** Önce süresi en yakın biten paket (son kullanımsız en sonda), eşitse az seansı kalan. */
function compare(a: UsablePackageItem, b: UsablePackageItem): number {
  const left = a.expiresOn ?? '9999-12-31';
  const right = b.expiresOn ?? '9999-12-31';
  return left === right ? a.remainingSessions - b.remainingSessions : left.localeCompare(right);
}

/** Seçilen hizmet sırasıyla, kalan seansı olan paket kalemi bulunan hizmetler. */
export function packageOptions(
  items: readonly UsablePackageItem[],
  services: readonly { id: string; name: string }[],
): PackageOption[] {
  return services.flatMap((service) => {
    const [best] = items
      .filter((item) => item.serviceId === service.id && item.remainingSessions > 0)
      .sort(compare);
    return best ? [{ serviceId: service.id, serviceName: service.name, item: best }] : [];
  });
}

/** "10 seansın 7'si kaldı". */
export function sessionsLeftLabel(
  item: Pick<UsablePackageItem, 'totalSessions' | 'remainingSessions'>,
) {
  return `${String(item.totalSessions)} seansın ${withPossessive(item.remainingSessions)} kaldı`;
}
