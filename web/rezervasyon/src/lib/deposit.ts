import type { components } from '@omvila/api-client-ts';
import type { Clock } from '@omvila/ui-web/time';
import { useEffect, useState } from 'react';

// Kapora (Dilim 4): yanıttaki kapora alanlarının okunması ve havale geri sayımı. Tutar ve son zaman her zaman
// yanıttan gelir (sabit "2 saat" yok); "şimdi" enjekte edilen saatten (Clock).

type PublicAppointment = components['schemas']['PublicAppointment'];
type Money = components['schemas']['Money'];
export type AppointmentDeposit = NonNullable<PublicAppointment['deposit']>;
export type DepositInstructions = components['schemas']['DepositInstructions'];

/** Havale ekranı gösterilecek mi: kapora bekleniyor (DepositPending) ve havale bilgileri yanıtta. */
export function awaitingTransfer(
  appointment: PublicAppointment,
): (AppointmentDeposit & { instructions: DepositInstructions }) | null {
  const { deposit } = appointment;
  if (appointment.displayStatus !== 'DepositPending' || !deposit?.instructions) return null;
  return { ...deposit, instructions: deposit.instructions };
}

/** Müşterinin bankasından gönderdiği ve salonun aldığını işaretlediği kapora ("Salonda ödenecek"ten düşülür). */
export function receivedDeposit(appointment: PublicAppointment): AppointmentDeposit | null {
  const status = appointment.deposit?.status;
  return status === 'Received' || status === 'Refunded' ? (appointment.deposit ?? null) : null;
}

/** Saat değişikliğinde kapora yeni saate aktarılır (A14): istenmiş ve vazgeçilmemiş kapora. */
export function carriesDeposit(appointment: PublicAppointment): boolean {
  const status = appointment.deposit?.status;
  return status === 'Awaiting' || status === 'CustomerReported' || status === 'Received';
}

/** Kalemlerin kapora ipuçlarının toplamı (hizmet listesi "Kapora 300 ₺"); hiçbirinde yoksa null. */
export function depositHintTotal(
  services: { hasDeposit: boolean; depositHint?: Money | null }[],
): Money | null {
  const hints = services.flatMap((service) =>
    service.hasDeposit && service.depositHint ? [service.depositHint] : [],
  );
  const [first] = hints;
  if (!first) return null;
  return {
    amountMinor: hints.reduce((total, hint) => total + hint.amountMinor, 0),
    currency: first.currency,
  };
}

/** Toplamdan kapora düşülür; eksiye inmez. */
export function minus(total: Money, deposit: Money): Money {
  return {
    amountMinor: Math.max(0, total.amountMinor - deposit.amountMinor),
    currency: total.currency,
  };
}

/** Son zamana kalan tam dakika (yukarı yuvarlanır: 30 sn kala "1 dk"); dolduysa 0. */
export function minutesLeft(dueUtc: string, now: Date): number {
  const milliseconds = new Date(dueUtc).getTime() - now.getTime();
  return milliseconds <= 0 ? 0 : Math.ceil(milliseconds / 60_000);
}

/** Saati her `intervalMs`'te yeniden okur (geri sayım). */
export function useNow(clock: Clock, intervalMs = 15_000): Date {
  const [now, setNow] = useState(() => clock.now());
  useEffect(() => {
    const timer = setInterval(() => {
      setNow(clock.now());
    }, intervalMs);
    return () => {
      clearInterval(timer);
    };
  }, [clock, intervalMs]);
  return now;
}
