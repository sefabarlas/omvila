import { formatDuration, formatPrice } from '@omvila/ui-web/format';
import type { ReactNode } from 'react';
import type { Summary } from './summary';
import { totalLabel } from './summary';
import styles from './SummaryCard.module.css';

interface SummaryCardProps {
  summary: Summary;
  /** "Salı, 25 Kasım · 14:30–15:45". */
  when: string;
  /** "Kuaför Elif · Moda Cad. 42, Kadıköy". */
  place: string;
  /** Başlığın sağındaki eylem ("Değiştir"). */
  action?: ReactNode;
}

/** Telefon ve Onay ekranlarının özet kartı: tarih/saat, hizmet · personel · fiyat, toplam. */
export function SummaryCard({ summary, when, place, action }: SummaryCardProps) {
  const total = totalLabel(summary);
  return (
    <section className={styles.card} aria-label="Randevu özeti">
      <div className={styles.head}>
        <div className={styles.headText}>
          <p className={styles.when}>{when}</p>
          <p className={styles.muted}>{place}</p>
        </div>
        {action}
      </div>
      <ul className={styles.lines}>
        {summary.lines.map((line) => (
          <li key={line.serviceId} className={styles.line}>
            <span>{[line.name, line.staffName].filter(Boolean).join(' · ')}</span>
            {line.price ? (
              <span className={styles.amount}>{formatPrice(line.price, line.priceDisplay)}</span>
            ) : null}
          </li>
        ))}
      </ul>
      <p className={styles.total}>
        <span>Toplam · {formatDuration(summary.durationMinutes)}</span>
        {total ? <span className={styles.amount}>{total}</span> : null}
      </p>
    </section>
  );
}

interface AsideSummaryProps {
  summary: Summary;
  /** "Elif Kaya", "Fark etmez"; seçilmediyse null. */
  staff: string | null;
  /** "Sal 25 Kas · 14:30–15:45"; seçilmediyse null. */
  when: string | null;
  /** Personel adımı kapalıysa satır gösterilmez. */
  showStaff: boolean;
}

/** Masaüstü sol kolon "Randevunuz" kartı (Masaustu.png): seçim değiştikçe güncellenir. */
export function AsideSummary({ summary, staff, when, showStaff }: AsideSummaryProps) {
  const total = totalLabel(summary);
  const notSelected = 'Henüz seçilmedi';
  return (
    <section className={styles.aside} aria-labelledby="randevunuz-baslik">
      <h2 id="randevunuz-baslik" className={styles.asideTitle}>
        Randevunuz
      </h2>
      {summary.lines.length > 0 ? (
        <ul className={styles.lines}>
          {summary.lines.map((line) => (
            <li key={line.serviceId} className={styles.line}>
              <span>{line.name}</span>
              {line.price ? (
                <span className={styles.amountRegular}>
                  {formatPrice(line.price, line.priceDisplay)}
                </span>
              ) : null}
            </li>
          ))}
        </ul>
      ) : (
        <p className={styles.muted}>Hizmet seçin; özet burada görünür.</p>
      )}
      <div className={styles.meta}>
        {showStaff ? (
          <p className={styles.metaLine}>
            Personel: <strong>{staff ?? notSelected}</strong>
          </p>
        ) : null}
        <p className={styles.metaLine}>
          Zaman: <strong>{when ?? notSelected}</strong>
        </p>
      </div>
      {summary.lines.length > 0 ? (
        <p className={styles.asideTotal}>
          <span>Toplam · {formatDuration(summary.durationMinutes)}</span>
          {total ? <span>{total}</span> : null}
        </p>
      ) : null}
    </section>
  );
}
