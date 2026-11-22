import { formatCount, formatDate, formatMoney, formatTime } from '@omvila/ui-web/format';
import { daysBetween, toBranchDate } from '@omvila/ui-web/time';
import { platformTimeZone } from '@/app/config';
import type { Candidate, NoticeDetail, QueueNotice } from './api';

/** Bildirimden bu yana geçen takvim günü (İstanbul takvimi; dün gelen 1 gün). */
export function waitingDays(submittedUtc: string, now: Date): number {
  return Math.max(
    0,
    daysBetween(
      toBranchDate(new Date(submittedUtc), platformTimeZone),
      toBranchDate(now, platformTimeZone),
    ),
  );
}

/** "2 gündür bekliyor"; aynı gün "bugün geldi". */
export function waitingText(submittedUtc: string, now: Date): string {
  const days = waitingDays(submittedUtc, now);
  return days === 0 ? 'bugün geldi' : `${formatCount(days)} gündür bekliyor`;
}

/** "Kasım" (dönemin ayı; ay adı yıl olmadan). */
export function periodMonth(periodStart: string): string {
  return formatDate(periodStart, 'monthYear').split(' ')[0] ?? '';
}

/** Kuyruk satırı başlığının devamı: "Abonelik · Kasım · havale bildirildi", "SMS kontörü · 5.000 · havale bildirildi". */
export function noticeHeadline(notice: QueueNotice): string {
  const what =
    notice.purpose === 'Credits'
      ? ['SMS kontörü', notice.credits === null ? null : formatCount(notice.credits)]
      : ['Abonelik', notice.periodStart ? periodMonth(notice.periodStart) : null];
  return [...what, 'havale bildirildi'].filter(Boolean).join(' · ');
}

/** "Ne için": "Salon planı · Kasım 2026", "SMS kontörü · 5.000". */
export function noticePurposeText(notice: NoticeDetail): string {
  if (notice.purpose === 'Credits') {
    return notice.credits === null ? 'SMS kontörü' : `SMS kontörü · ${formatCount(notice.credits)}`;
  }

  return [
    notice.planName ? `${notice.planName} planı` : 'Abonelik',
    notice.periodStart ? formatDate(notice.periodStart, 'monthYear') : null,
  ]
    .filter(Boolean)
    .join(' · ');
}

/** Beklenen tutar; tanımlı değilse "belirlenmedi" (S-14: fiyat yapılandırmada yoksa). */
export function expectedAmountText(amount: QueueNotice['expectedAmount']): string {
  return amount ? formatMoney(amount) : 'Tutar belirlenmedi';
}

/** "Kontrol edin" uyarısının nedeni (kod ve tutar denetiminden). */
export function checkReason({
  codeFound,
  amountMatches,
}: {
  codeFound: boolean;
  amountMatches: boolean;
}): string {
  if (!codeFound && amountMatches) {
    return 'açıklama kodu yok; tutar ve gönderen adıyla eşleştirin.';
  }

  if (codeFound && !amountMatches) {
    return 'tutar beklenenden farklı; dekontu ve gönderen adını karşılaştırın.';
  }

  return 'açıklama kodu yok ve tutar farklı; havalenin bu bildirime ait olduğundan emin olun.';
}

/** "18 Kas" (İstanbul takvimi). */
export const shortDate = (utc: string) =>
  formatDate(toBranchDate(new Date(utc), platformTimeZone), 'dayMonthShort');

/** "18 Kas 14:20". */
export function dateTimeShort(utc: string): string {
  return `${shortDate(utc)} ${formatTime(utc, platformTimeZone)}`;
}

/** Adayın anı: dökümde saat varsa "18 Kas 14:05", yoksa değer tarihi "18 Kas". */
export function candidateWhen(candidate: Pick<Candidate, 'receivedUtc' | 'valueDate'>): string {
  return candidate.receivedUtc
    ? dateTimeShort(candidate.receivedUtc)
    : formatDate(candidate.valueDate, 'dayMonthShort');
}

/** Aday aralığı: "11–21 Kasım"; aylar farklıysa "28 Eki – 3 Kas". */
export function windowText({ from, to }: { from: string; to: string }): string {
  if (from.slice(0, 7) === to.slice(0, 7)) {
    return `${formatDate(from, 'day')}–${formatDate(to, 'dayMonth')}`;
  }

  return `${formatDate(from, 'dayMonthShort')} – ${formatDate(to, 'dayMonthShort')}`;
}

/** Eşleştirmenin sonucu (Karar kartı notu). */
export function matchConsequence(notice: NoticeDetail): string {
  const what =
    notice.purpose === 'Credits'
      ? `Eşleştirince ${notice.credits === null ? 'paketin' : formatCount(notice.credits)} kontörü markaya eklenir`
      : `Eşleştirince ${notice.periodStart ? `${periodMonth(notice.periodStart)} dönemi` : 'dönem'} ödendi sayılır, fatura kesilir`;
  return `${what} ve markaya bildirim gider. Eşleştirme denetim kaydına yazılır.`;
}
