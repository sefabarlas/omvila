import { formatDate, formatDuration, formatMoney } from '@omvila/ui-web/format';
import type { PriceDisplay } from '@omvila/ui-web/format';
import { ANY_STAFF } from '@omvila/ui-web/booking';
import { parseClock } from '@omvila/ui-web/time';
import type { Catalog, Money, Slot } from './booking-api';
import { effectiveSameStaff } from './selection';
import type { Selection } from './selection';

// Seçimin özeti: alt çubuk, masaüstü "Randevunuz" kartı ve Telefon/Onay özet kartı aynı hesaptan beslenir.

export interface SummaryLine {
  serviceId: string;
  name: string;
  durationMinutes: number;
  /** showPrices kapalıysa null. */
  price: Money | null;
  priceDisplay: PriceDisplay;
  /** Saat seçildiyse atanan personel ("fark etmez" dahil, kriter 6); değilse seçilen personel. */
  staffName: string | null;
}

export interface Summary {
  lines: SummaryLine[];
  durationMinutes: number;
  /** showPrices kapalıysa veya hiç hizmet yoksa null. */
  total: Money | null;
  /** Kalemlerden biri "'den" fiyatlıysa toplam da öyle gösterilir. */
  startingFrom: boolean;
}

function staffName(catalog: Catalog, id: string | null | undefined): string | null {
  if (!id || id === ANY_STAFF) return null;
  return catalog.staff.find((member) => member.id === id)?.displayName ?? null;
}

function surcharge(catalog: Catalog, staffId: string | undefined, serviceId: string): number {
  if (!staffId || staffId === ANY_STAFF) return 0;
  const member = catalog.staff.find((candidate) => candidate.id === staffId);
  return member?.surcharges?.find((item) => item.serviceId === serviceId)?.amount.amountMinor ?? 0;
}

/** Saat seçilmeden önce katalogdan, seçildikten sonra saat yanıtından (kalem fiyatı, atanan personel). */
export function summarize(
  catalog: Catalog,
  selection: Selection,
  showPrices: boolean,
  slot: Slot | null,
): Summary {
  const same = effectiveSameStaff(selection, catalog);
  const lines = selection.serviceIds.flatMap((serviceId, index): SummaryLine[] => {
    const service = catalog.services.find((candidate) => candidate.id === serviceId);
    if (!service) return [];
    const chosen = catalog.allowStaffSelection
      ? same
        ? selection.staff[0]
        : selection.staff[index]
      : undefined;
    const slotItem = slot?.items.find((item) => item.serviceId === serviceId);
    const basePrice = service.price
      ? {
          amountMinor: service.price.amountMinor + surcharge(catalog, chosen, serviceId),
          currency: service.price.currency,
        }
      : null;
    return [
      {
        serviceId,
        name: service.name,
        durationMinutes: service.durationMinutes,
        price: showPrices ? (slotItem?.price ?? basePrice) : null,
        priceDisplay: service.priceDisplay,
        staffName: slotItem
          ? (slotItem.staffName ?? staffName(catalog, slotItem.staffId))
          : staffName(catalog, chosen),
      },
    ];
  });

  const slotDuration =
    slot?.endLocalTime !== undefined
      ? parseClock(slot.endLocalTime) - parseClock(slot.localTime)
      : null;
  const priced = lines.filter((line) => line.price !== null);
  const sum =
    showPrices && lines.length > 0 && priced.length === lines.length
      ? {
          amountMinor: priced.reduce((total, line) => total + (line.price?.amountMinor ?? 0), 0),
          currency: priced[0]?.price?.currency ?? 'TRY',
        }
      : null;

  return {
    lines,
    durationMinutes:
      slotDuration !== null && slotDuration > 0
        ? slotDuration
        : lines.reduce((total, line) => total + line.durationMinutes, 0),
    total: showPrices && slot ? slot.total : sum,
    startingFrom: lines.some((line) => line.priceDisplay === 'StartingFrom'),
  };
}

/** "700 ₺" veya "2.800 ₺'den". */
export function totalLabel(summary: Summary): string | null {
  if (!summary.total) return null;
  const amount = formatMoney(summary.total);
  return summary.startingFrom ? `${amount}'den` : amount;
}

/** "2 hizmet · 1 sa 15 dk". */
export function countLabel(summary: Summary): string {
  return `${String(summary.lines.length)} hizmet · ${formatDuration(summary.durationMinutes)}`;
}

/** "14:30–15:45" (bitiş yoksa yalnız başlangıç). */
export function timeRange(slot: Pick<Slot, 'localTime' | 'endLocalTime'>): string {
  return slot.endLocalTime ? `${slot.localTime}–${slot.endLocalTime}` : slot.localTime;
}

/** Alt çubuk: "Sal 25 Kas · 14:30–15:45". */
export function compactWhen(date: string, slot: Slot): string {
  return `${formatDate(date, 'compact')} · ${timeRange(slot)}`;
}

/** Özet kartı: "Salı, 25 Kasım · 14:30–15:45". */
export function longWhen(date: string, slot: Slot): string {
  return `${formatDate(date, 'panel')} · ${timeRange(slot)}`;
}
