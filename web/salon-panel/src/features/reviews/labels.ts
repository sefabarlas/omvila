import { formatDate } from '@omvila/ui-web/format';
import { daysBetween, toBranchDate } from '@omvila/ui-web/time';
import type { ReviewFilter } from './api';

/**
 * Platform ayarları (sureler-ve-ayarlar.md): sözleşme bunları panele vermez, yalnız boş durum metninde geçer
 * (acik-sorular.md T-99). Ayar uçtan gelirse buradan kaldırılır.
 */
export const reviewInviteDelayMinutes = 120;
export const reviewPublicThreshold = 5;

/** Filtre çipleri (salon-paneli/Degerlendirmeler). Adreste Türkçe değer: ?filtre=tumu. */
export const reviewFilterOptions: readonly { value: ReviewFilter; slug: string; label: string }[] =
  [
    { value: 'AwaitingReply', slug: 'yanit-bekleyen', label: 'Yanıt bekleyen' },
    { value: 'All', slug: 'tumu', label: 'Tümü' },
    { value: 'LowRating', slug: 'dusuk-puan', label: '3 yıldız ve altı' },
  ];

export function reviewFilterFromSlug(slug: string | undefined): ReviewFilter {
  return reviewFilterOptions.find((option) => option.slug === slug)?.value ?? 'AwaitingReply';
}

export function reviewFilterSlug(filter: ReviewFilter): string | undefined {
  return filter === 'AwaitingReply'
    ? undefined
    : reviewFilterOptions.find((option) => option.value === filter)?.slug;
}

/** Puan: "4,8" (tek ondalık); yoksa "—". */
export function formatRating(rating: number | null | undefined): string {
  return rating === null || rating === undefined
    ? '—'
    : (Math.round(rating * 10) / 10).toFixed(1).replace('.', ',');
}

/** Değerlendirme yaşı: "bugün", "dün", "3 gün önce", "1 hafta önce"; 4 haftadan eskiyse "21 Ekim". */
export function reviewAge(createdUtc: string, today: string, timeZone: string): string {
  const day = toBranchDate(new Date(createdUtc), timeZone);
  const days = daysBetween(day, today);
  if (days <= 0) {
    return 'bugün';
  }

  if (days === 1) {
    return 'dün';
  }

  if (days < 7) {
    return `${String(days)} gün önce`;
  }

  return days < 28 ? `${String(Math.floor(days / 7))} hafta önce` : formatDate(day, 'dayMonth');
}

/** Kart üst satırı: "Kaş ve kirpik · Zeynep · 3 gün önce" (personelin yalnız adı). */
export function reviewMeta(
  review: { serviceNames: readonly string[]; staffName?: string | null; createdUtc: string },
  today: string,
  timeZone: string,
): string {
  const staff = review.staffName?.trim().split(/\s+/)[0];
  return [review.serviceNames.join(', '), staff, reviewAge(review.createdUtc, today, timeZone)]
    .filter((part) => part !== undefined && part !== '')
    .join(' · ');
}
