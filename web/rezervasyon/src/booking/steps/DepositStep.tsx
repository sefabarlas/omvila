import type { Clock } from '@omvila/ui-web/time';
import { useCallback } from 'react';
import { fetchAppointment } from '@/appointment/appointment-api';
import { DepositNotReceived } from '@/deposit/DepositNotReceived';
import { DepositTransfer } from '@/deposit/DepositTransfer';
import type { PublicAppointment } from '../booking-api';
import styles from './steps.module.css';

interface DepositStepProps {
  appointment: PublicAppointment;
  clock: Clock;
  /** Bildirim veya yeniden okuma sonrası güncel randevu. */
  onChange: (appointment: PublicAppointment) => void;
}

/**
 * Oluşturma sonrası kapora bekleyen randevu (displayStatus DepositPending): Havale ekranı. Süre sayfa açıkken
 * dolarsa randevu erişim koduyla yeniden okunur (salon işaretlediyse Başarı, iş çalıştıysa "Kapora gelmedi").
 */
export function DepositStep({ appointment, clock, onChange }: DepositStepProps) {
  const { accessCode } = appointment;
  const refresh = useCallback(() => {
    fetchAppointment(accessCode).then(
      (loaded) => {
        if (loaded) onChange(loaded.appointment);
      },
      () => undefined,
    );
  }, [accessCode, onChange]);

  if (appointment.status === 'Cancelled') {
    return (
      <div className={styles.stack}>
        <h2 className={styles.titleMedium}>Randevunuz iptal edildi</h2>
        <DepositNotReceived phone={appointment.branch.phone} />
        <a className={styles.linkButton} href={`/${encodeURIComponent(appointment.brand.slug)}`}>
          Yeni randevu al
        </a>
      </div>
    );
  }
  return (
    <DepositTransfer
      appointment={appointment}
      clock={clock}
      headingLevel={2}
      onReported={(loaded) => {
        onChange(loaded.appointment);
      }}
      onStale={refresh}
    />
  );
}
