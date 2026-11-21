import type { PublicAppointment, SlotSearchResult } from '@/appointment/appointment-api';
import { appointmentFixture } from './booking-fixtures';

// SMS bağlantısı (/r/{kod}) testlerinin kurgusal verisi. "Şimdi" 24 Kasım 2026 Salı 10:00 (nowUtc); randevu
// 25 Kasım Çarşamba 14:30 (Europe/Istanbul): iptal 24 saat, saat değişikliği 12 saat öncesine kadar.

export const accessCode = '7Kx2Pq9a';
export const etag = 'W/"8812"';

const tl = (lira: number) => ({ amountMinor: lira * 100, currency: 'TRY' as const });

export function linkAppointmentFixture(
  overrides: Partial<PublicAppointment> = {},
): PublicAppointment {
  const base = appointmentFixture();
  return {
    ...base,
    accessCode,
    startUtc: '2026-11-25T11:30:00Z',
    endUtc: '2026-11-25T12:45:00Z',
    items: [
      {
        serviceName: 'Saç kesimi',
        staffName: 'Elif Kaya',
        startUtc: '2026-11-25T11:30:00Z',
        endUtc: '2026-11-25T12:15:00Z',
        price: tl(500),
        paidFromPackage: false,
      },
      {
        serviceName: 'Fön',
        staffName: 'Elif Kaya',
        startUtc: '2026-11-25T12:15:00Z',
        endUtc: '2026-11-25T12:45:00Z',
        price: tl(250),
        paidFromPackage: false,
      },
    ],
    canCancel: true,
    cancelDeadlineUtc: '2026-11-24T11:30:00Z',
    canReschedule: true,
    rescheduleDeadlineUtc: '2026-11-24T23:30:00Z',
    version: '8812',
    ...overrides,
  };
}

/** Müşteri iptalinden sonraki yanıt. */
export function cancelledFixture(): PublicAppointment {
  return linkAppointmentFixture({
    status: 'Cancelled',
    displayStatus: 'Cancelled',
    canCancel: false,
    canReschedule: false,
    cancelDeadlineUtc: null,
    rescheduleDeadlineUtc: null,
    cancellation: {
      cancelledUtc: '2026-11-24T07:05:00Z',
      cancelledBy: 'Customer',
      reason: 'PlanChanged',
    },
    version: '8813',
  });
}

/** Onay bekleyen talep (Dilim 3, A1): salonun yanıt süresi şimdiden 1 sa 35 dk sonra doluyor. */
export function pendingFixture(): PublicAppointment {
  return linkAppointmentFixture({
    status: 'PendingApproval',
    displayStatus: 'PendingApproval',
    holdExpiresUtc: '2026-11-24T08:35:00Z',
  });
}

/** Salonun onaylayamadığı (A4) veya zamanında yanıtlamadığı (A5) talep. */
export function requestCancelledFixture(
  reason: 'DeclinedBySalon' | 'ApprovalExpired',
): PublicAppointment {
  return linkAppointmentFixture({
    status: 'Cancelled',
    displayStatus: 'Cancelled',
    holdExpiresUtc: null,
    canCancel: false,
    canReschedule: false,
    cancelDeadlineUtc: null,
    rescheduleDeadlineUtc: null,
    cancellation: {
      cancelledUtc: '2026-11-24T06:50:00Z',
      cancelledBy: reason === 'DeclinedBySalon' ? 'Salon' : 'System',
      reason,
    },
  });
}

/** 15:15'e alınmış randevu. */
export function rescheduledFixture(): PublicAppointment {
  return linkAppointmentFixture({
    startUtc: '2026-11-25T12:15:00Z',
    endUtc: '2026-11-25T13:30:00Z',
    version: '8814',
  });
}

const slot = (date: string, localTime: string, utc: string, end: string) => ({
  startUtc: `${date}T${utc}:00Z`,
  localTime,
  endLocalTime: end,
  period: localTime < '12:00' ? ('Morning' as const) : ('Afternoon' as const),
  items: [],
  total: tl(750),
});

/** Ertele.png'deki ızgara; randevunun kendi saati (14:30) sunucuda boş sayılır, ekranda seçilemez. */
export function rescheduleSlotsFixture(date: string): SlotSearchResult {
  return {
    timeZone: 'Europe/Istanbul',
    date,
    unavailableTimes: ['10:45', '16:45'],
    slots: [
      slot(date, '10:00', '07:00', '11:15'),
      slot(date, '11:30', '08:30', '12:45'),
      slot(date, '13:00', '10:00', '14:15'),
      slot(date, '14:30', '11:30', '15:45'),
      slot(date, '15:15', '12:15', '16:30'),
      slot(date, '15:45', '12:45', '17:00'),
      slot(date, '16:00', '13:00', '17:15'),
    ],
  };
}
