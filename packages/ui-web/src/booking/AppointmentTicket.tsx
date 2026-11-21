import type { ReactNode } from 'react';
import { formatDate, formatDuration, formatTime } from '../format/date';
import { initials } from '../format/name';
import { statusLabels } from '../status';
import type { StatusKey } from '../status';
import { toBranchDate } from '../time';
import { cx } from './internal';
import styles from './AppointmentTicket.module.css';

export interface AppointmentTicketProps {
  /** "Kuaför Elif". */
  salonName: string;
  logoUrl?: string | null;
  /** "Moda Cad. 42, Kadıköy". */
  branchAddress?: string | null;
  /** Gösterim durumu (displayStatus): "Randevunuz · Onaylandı". */
  status: StatusKey;
  /** Durum satırının ön eki. */
  statusPrefix?: string;
  startUtc: string;
  /** Şubenin saat dilimi; tarih ve saat bununla gösterilir. */
  timeZone: string;
  /** ["Saç kesimi", "Fön"]. */
  serviceNames: readonly string[];
  staffName?: string | null;
  durationMinutes: number;
  /** Bloğun altındaki eylemler: "Yol tarifi", "Takvime ekle" (BookingButton). */
  actions?: ReactNode;
  /** Başlık düzeyi; sayfada tek h1. */
  headingLevel?: 1 | 2;
  className?: string;
}

/** "Sal 25 Kas · 14:30" (şubenin saat diliminde). */
export function appointmentWhen(startUtc: string, timeZone: string): string {
  const date = toBranchDate(new Date(startUtc), timeZone);
  return `${formatDate(date, 'compact')} · ${formatTime(startUtc, timeZone)}`;
}

/**
 * Randevum başlık bloğu (rezervasyon-sayfasi/Randevum; components/AppointmentTicket): salon, durum satırı,
 * büyük tarih/saat, hizmetler · personel · süre; altında eylem butonları.
 */
export function AppointmentTicket({
  salonName,
  logoUrl,
  branchAddress,
  status,
  statusPrefix = 'Randevunuz',
  startUtc,
  timeZone,
  serviceNames,
  staffName,
  durationMinutes,
  actions,
  headingLevel = 1,
  className,
}: AppointmentTicketProps) {
  const Heading = headingLevel === 1 ? 'h1' : 'h2';
  const details = [serviceNames.join(', '), staffName, formatDuration(durationMinutes)]
    .filter(Boolean)
    .join(' · ');
  return (
    <section className={cx(styles.root, className)}>
      <div className={cx(styles.block, status === 'iptal' && styles.cancelled)}>
        <div className={styles.salon}>
          <span className={styles.logo} aria-hidden="true">
            {logoUrl ? <img src={logoUrl} alt="" /> : initials(salonName)}
          </span>
          <span className={styles.salonText}>
            <strong className={styles.salonName}>{salonName}</strong>
            {branchAddress ? <span className={styles.address}>{branchAddress}</span> : null}
          </span>
        </div>
        <p className={styles.status}>
          {statusPrefix} · {statusLabels[status]}
        </p>
        <Heading className={styles.when}>{appointmentWhen(startUtc, timeZone)}</Heading>
        <p className={styles.details}>{details}</p>
      </div>
      {actions ? <div className={styles.actions}>{actions}</div> : null}
    </section>
  );
}
