import { formatDate, formatDuration, formatTime } from '@omvila/ui-web/format';
import { addDays, toBranchDate } from '@omvila/ui-web/time';
import type { Clock } from '@omvila/ui-web/time';
import type { InAppNotification } from './api';

/**
 * Ayarlar → Bildirimler ve SMS (kontör).
 * "Kontör az" şeridi ve kontör bildirimi buradan yönlenir; rota gelince yalnız burası değişir.
 */
export const smsCreditsRoute = '/ayarlar/bildirimler';

export type NotificationRoute =
  | { to: '/' | '/ayarlar' | '/ayarlar/bildirimler' }
  | { to: '/takvim'; search?: { randevu: string } };

/**
 * Bildirime dokununca açılacak sayfa (NotificationTarget). Randevu ve takvim hedefleri takvimi açar. "Havale
 * bildirildi" (Deposit, staff.deposit_reported) takvimde o randevunun yan panelini kapora bölümüyle açar
 * (`?randevu=`; T-84). Henüz yapılmamış sayfalar (değerlendirmeler, kapıda sıra) ana sayfayı açar (T-69). İzni
 * olmayan sayfaya yönlenilmez.
 */
export function notificationRoute(
  target: InAppNotification['target'],
  can: (permission: string) => boolean,
): NotificationRoute {
  switch (target.type) {
    case 'Appointment':
    case 'Calendar':
      return { to: can('calendar.view') ? '/takvim' : '/' };
    case 'Deposit':
      if (!can('calendar.view')) {
        return { to: '/' };
      }

      return target.id ? { to: '/takvim', search: { randevu: target.id } } : { to: '/takvim' };
    case 'SmsCredits':
    case 'Subscription':
      return { to: can('settings.manage') ? smsCreditsRoute : '/' };
    default:
      return { to: '/' };
  }
}

/** Satırdaki zaman: bugün "12:02", dün "Dün", daha eski "24 Kasım" (şubenin saat diliminde). */
export function notificationTime(createdUtc: string, clock: Clock, timeZone: string): string {
  const today = toBranchDate(clock.now(), timeZone);
  const day = toBranchDate(new Date(createdUtc), timeZone);
  if (day === today) {
    return formatTime(createdUtc, timeZone);
  }

  return day === addDays(today, -1) ? 'Dün' : formatDate(day, 'dayMonth');
}

/** Onay bekleyen talebin kalan süresi ("1 sa 35 dk kaldı"); süre dolduysa veya talep yanıtlandıysa null. */
export function holdRemaining(
  holdExpiresUtc: string | null | undefined,
  clock: Clock,
): string | null {
  if (!holdExpiresUtc) {
    return null;
  }

  const minutes = Math.ceil((Date.parse(holdExpiresUtc) - clock.now().getTime()) / 60_000);
  return minutes > 0 ? `${formatDuration(minutes)} kaldı` : null;
}
