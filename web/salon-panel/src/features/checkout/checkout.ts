import type { components } from '@omvila/api-client-ts';
import { formatDate, formatMoney, moneyInputValue, parseMoneyInput } from '@omvila/ui-web/format';
import { toBranchDate } from '@omvila/ui-web/time';
import type { AppointmentDetail } from '@/features/calendar/api';
import type { CustomerPackage } from './api';

// "Tamamla ve ödeme al" (salon-paneli/OdemeAl) hesapları. Para kuruş cinsinden tamsayıdır; alınacak tutarın kaynağı
// sunucunun amountDue'sudur (kalemler − salonda kalan kapora − randevu oluşurken paketten düşülenler). İstemci yalnız
// şimdi "Paketten" düşülen kalemlerin fiyatını çıkarır; uyuşmazlıkta sunucu 409 payment_total_mismatch döner.

export type CollectedMethod = components['schemas']['CollectedPaymentMethod'];
export type PackageRedemption = components['schemas']['PackageRedemptionRequest'];

/** Bir ödeme satırına en çok bu kadar satır (sözleşme: payments maxItems 3). */
export const maxPaymentRows = 3;

export const methodLabels: Record<components['schemas']['PaymentMethod'], string> = {
  Cash: 'Nakit',
  Card: 'Kart',
  Transfer: 'Havale',
  PackageRedemption: 'Paketten',
};

/** Kalemin tahsil edilen tutarı: fiyat + personel farkı − indirim. */
export function itemAmount(item: AppointmentDetail['items'][number]): number {
  return (
    item.price.amountMinor +
    (item.staffSurcharge?.amountMinor ?? 0) -
    (item.discount?.amountMinor ?? 0)
  );
}

/** Personelin ilk adı ("Boya · Elif"). */
export function firstName(fullName: string | null | undefined): string | null {
  return fullName?.trim().split(/\s+/)[0] ?? null;
}

export interface DepositLine {
  amountMinor: number;
  /** "Kapora (havale, 11 Kas)". */
  label: string;
}

/**
 * Salonda kalan kapora (alınan − iade edilen). Ödemeler görünüyorsa (payment.collect) kapora kaydından, yoksa randevunun
 * kapora özetinden okunur. Kapora OMVILA'dan geçmez; salonun IBAN'ına gelen havaledir.
 */
export function depositLine(detail: AppointmentDetail): DepositLine | null {
  const tz = detail.timeZone;
  const label = (utc: string | null | undefined) =>
    utc
      ? `Kapora (havale, ${formatDate(toBranchDate(new Date(utc), tz), 'dayMonthShort')})`
      : 'Kapora (havale)';
  const depositPayments = (detail.payments ?? []).filter((p) => p.isDeposit);
  if (depositPayments.length > 0) {
    const amountMinor = depositPayments.reduce(
      (sum, p) => sum + p.amount.amountMinor - (p.refundedAmount?.amountMinor ?? 0),
      0,
    );
    return amountMinor > 0 ? { amountMinor, label: label(depositPayments[0]?.paidUtc) } : null;
  }

  const deposit = detail.deposit;
  if (!deposit || (deposit.status !== 'Received' && deposit.status !== 'Refunded')) {
    return null;
  }

  const amountMinor = deposit.required.amountMinor - (deposit.refundedAmount?.amountMinor ?? 0);
  return amountMinor > 0 ? { amountMinor, label: label(deposit.receivedUtc) } : null;
}

export interface RedemptionMatch extends PackageRedemption {
  serviceName: string;
  packageName: string;
  amountMinor: number;
  /** Düşümden sonra kalan seans. */
  remainingAfter: number;
}

/**
 * Şimdi paketten düşülebilecek kalemler: kalem paketten ödenmemiş ve etkin; paket bu müşterinin, Active, süresi
 * geçmemiş, şubede kullanılabilir ve aynı hizmetin kalan seansı var. Aynı paket kalemi birden çok kaleme yetiyorsa
 * kalan seans kadar kullanılır. Sunucu aynı kuralı denetler (409 package_session_unavailable).
 */
export function redemptionMatches(
  detail: AppointmentDetail,
  packages: readonly CustomerPackage[],
  today: string,
): RedemptionMatch[] {
  const usable = packages.filter(
    (p) =>
      p.status === 'Active' &&
      p.expiresOn >= today &&
      (p.isAvailableAllBranches || p.branchId === detail.branchId),
  );
  const remaining = new Map<string, number>();
  for (const p of usable) {
    for (const item of p.items) {
      remaining.set(item.customerPackageItemId, item.remainingSessions);
    }
  }

  const matches: RedemptionMatch[] = [];
  for (const item of detail.items) {
    if (item.paidFromPackage || item.status !== 'Active') {
      continue;
    }

    for (const p of usable) {
      const packageItem = p.items.find(
        (candidate) =>
          candidate.serviceId === item.serviceId &&
          (remaining.get(candidate.customerPackageItemId) ?? 0) > 0,
      );
      if (packageItem) {
        const left = (remaining.get(packageItem.customerPackageItemId) ?? 0) - 1;
        remaining.set(packageItem.customerPackageItemId, left);
        matches.push({
          appointmentItemId: item.id,
          customerPackageItemId: packageItem.customerPackageItemId,
          serviceName: item.serviceName,
          packageName: p.packageName,
          amountMinor: itemAmount(item),
          remainingAfter: left,
        });
        break;
      }
    }
  }

  return matches;
}

/** Alınacak: amountDue − şimdi paketten düşülenler (eksiye inmez). */
export function amountToCollect(
  detail: AppointmentDetail,
  redemptions: readonly RedemptionMatch[],
): number {
  const redeemed = redemptions.reduce((sum, r) => sum + r.amountMinor, 0);
  return Math.max(0, detail.amountDue.amountMinor - redeemed);
}

export interface PaymentRow {
  method: CollectedMethod;
  /** Kullanıcının yazdığı tutar ("1.240"). */
  amount: string;
}

export type RowsCheck =
  | { ok: true; amounts: number[] }
  | { ok: false; rowErrors: (string | undefined)[]; totalError?: string };

/**
 * İstemci denetimi: her satırın tutarı 0'dan büyük olmalı ve satırların toplamı alınacak tutara eşit olmalı. Alınacak
 * 0 ise ödeme satırı gönderilmez.
 */
export function checkRows(rows: readonly PaymentRow[], dueMinor: number): RowsCheck {
  if (dueMinor === 0) {
    return { ok: true, amounts: [] };
  }

  const amounts = rows.map((row) => parseMoneyInput(row.amount));
  const rowErrors = amounts.map((minor) =>
    minor === null || minor <= 0 ? 'Tutarı girin, ör. 450.' : undefined,
  );
  if (rowErrors.some(Boolean)) {
    return { ok: false, rowErrors };
  }

  const valid = amounts.filter((minor): minor is number => minor !== null);
  const total = valid.reduce((sum, minor) => sum + minor, 0);
  if (total !== dueMinor) {
    const money = (amountMinor: number) => formatMoney({ amountMinor, currency: 'TRY' });
    return {
      ok: false,
      rowErrors: [],
      totalError: `Ödemelerin toplamı (${money(total)}) alınacak tutarla (${money(dueMinor)}) aynı olmalı.`,
    };
  }

  return { ok: true, amounts: valid };
}

/** Yeni satırın önerilen tutarı: alınacaktan diğer satırların toplamı düşülür (kalmadıysa boş). */
export function remainderFor(rows: readonly PaymentRow[], dueMinor: number): string {
  const entered = rows.reduce((sum, row) => sum + (parseMoneyInput(row.amount) ?? 0), 0);
  const rest = dueMinor - entered;
  return rest > 0 ? moneyInputValue(rest) : '';
}
