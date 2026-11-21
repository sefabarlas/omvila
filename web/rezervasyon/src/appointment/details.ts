import { appointmentWhen } from '@omvila/ui-web/booking';
import { formatDuration } from '@omvila/ui-web/format';
import type { PublicAppointment } from './appointment-api';

// Randevum, Ertele ve İptal ekranlarının ortak metinleri; süreler yanıttaki son tarihlerden hesaplanır (sabit yok).

/** Online değişiklik yapılabilen durumlar (A6, A14). */
export function isActive(appointment: PublicAppointment): boolean {
  return appointment.status === 'PendingApproval' || appointment.status === 'Confirmed';
}

export function durationMinutes(appointment: Pick<PublicAppointment, 'startUtc' | 'endUtc'>) {
  return Math.round(
    (new Date(appointment.endUtc).getTime() - new Date(appointment.startUtc).getTime()) / 60_000,
  );
}

/** Kalemlerdeki personel adları, tekrarsız: "Elif Kaya". */
export function staffNames(appointment: PublicAppointment): string | null {
  const names = [
    ...new Set(appointment.items.map((item) => item.staffName).filter((name) => name != null)),
  ];
  return names.length > 0 ? names.join(', ') : null;
}

/** "Saç kesimi, Fön · Elif Kaya · 1 sa 15 dk". */
export function detailsLine(appointment: PublicAppointment): string {
  return [
    appointment.items.map((item) => item.serviceName).join(', '),
    staffNames(appointment),
    formatDuration(durationMinutes(appointment)),
  ]
    .filter(Boolean)
    .join(' · ');
}

/** "Sal 25 Kas · 14:30" (şubenin saat diliminde). */
export function whenLabel(appointment: Pick<PublicAppointment, 'startUtc' | 'timeZone'>): string {
  return appointmentWhen(appointment.startUtc, appointment.timeZone);
}

/** Son tarihin randevudan kaç saat önce olduğu (CancellationCutoffHours / RescheduleCutoffHours). */
function hoursBefore(startUtc: string, deadlineUtc: string): number {
  return Math.round((new Date(startUtc).getTime() - new Date(deadlineUtc).getTime()) / 3_600_000);
}

/**
 * "Plan değişti mi?" kartının metni (Randevum.png). Kapora (Dilim 4) yokken "ücretsiz" yazılmaz: süre geçince
 * online iptal kapanır, ücret doğmaz.
 */
export function planText(appointment: PublicAppointment): string {
  const { canCancel, canReschedule, cancelDeadlineUtc, rescheduleDeadlineUtc, startUtc } =
    appointment;
  if (!canCancel && !canReschedule) {
    return 'Online değişiklik süresi geçti. Saati değiştirmek veya iptal etmek için salonu arayın.';
  }
  const parts: string[] = [];
  if (canCancel && cancelDeadlineUtc) {
    parts.push(
      `Randevudan ${String(hoursBefore(startUtc, cancelDeadlineUtc))} saat öncesine kadar iptal edebilirsiniz.`,
    );
  } else if (!canCancel) {
    parts.push('Online iptal süresi geçti; iptal için salonu arayın.');
  }
  if (canReschedule && rescheduleDeadlineUtc) {
    parts.push(
      `Saati değiştirmek için ${String(hoursBefore(startUtc, rescheduleDeadlineUtc))} saat öncesine kadar süreniz var.`,
    );
  } else if (!canReschedule) {
    parts.push('Saat değişikliği süresi geçti; yeni saat için salonu arayın.');
  }
  return parts.length > 0 ? parts.join(' ') : 'Randevunuzu buradan değiştirebilirsiniz.';
}

/** Ertele ekranının alt notu: "Saat değişikliği randevudan 12 saat öncesine kadar yapılabilir." */
export function rescheduleRule(appointment: PublicAppointment): string | null {
  return appointment.rescheduleDeadlineUtc
    ? `Saat değişikliği randevudan ${String(hoursBefore(appointment.startUtc, appointment.rescheduleDeadlineUtc))} saat öncesine kadar yapılabilir.`
    : null;
}
