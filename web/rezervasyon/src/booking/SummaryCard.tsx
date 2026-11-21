import { formatDuration, formatMoney, formatPrice } from '@omvila/ui-web/format';
import type { ReactNode } from 'react';
import { minus } from '@/lib/deposit';
import type { Money } from './booking-api';
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
  /** Onay ekranında kapora (YROnay.png): "Kapora (havale ile) −150 ₺" ve "Salonda ödenecek" = toplam − kapora. */
  deposit?: Money | null;
  /** Paketten düşülen kalemlerin hizmetleri (Onay.png): fiyat yerine "Paketten", "Salonda ödenecek"e girmez. */
  packaged?: readonly string[];
}

/** Telefon ve Onay ekranlarının özet kartı: tarih/saat, hizmet · personel · fiyat, toplam. */
export function SummaryCard({
  summary,
  when,
  place,
  action,
  deposit = null,
  packaged = [],
}: SummaryCardProps) {
  const total = totalLabel(summary);
  const fromPackage = summary.lines.filter((line) => packaged.includes(line.serviceId));
  const payable = summary.total
    ? minus(summary.total, {
        amountMinor: fromPackage.reduce((sum, line) => sum + (line.price?.amountMinor ?? 0), 0),
        currency: summary.total.currency,
      })
    : null;
  const due =
    (deposit || fromPackage.length > 0) && payable
      ? formatMoney(deposit ? minus(payable, deposit) : payable)
      : null;
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
            {packaged.includes(line.serviceId) ? (
              <span className={styles.amountRegular}>Paketten</span>
            ) : line.price ? (
              <span className={styles.amount}>{formatPrice(line.price, line.priceDisplay)}</span>
            ) : null}
          </li>
        ))}
      </ul>
      {fromPackage.length === 0 ? (
        <p className={deposit ? styles.line : styles.total}>
          <span>Toplam · {formatDuration(summary.durationMinutes)}</span>
          {total ? <span className={styles.amount}>{total}</span> : null}
        </p>
      ) : null}
      {deposit ? (
        <p className={styles.line}>
          <span>Kapora (havale ile)</span>
          <span className={styles.amount}>−{formatMoney(deposit)}</span>
        </p>
      ) : null}
      {due ? (
        <p className={styles.total}>
          <span>Salonda ödenecek</span>
          <span className={styles.amount}>{summary.startingFrom ? `${due}'den` : due}</span>
        </p>
      ) : null}
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
