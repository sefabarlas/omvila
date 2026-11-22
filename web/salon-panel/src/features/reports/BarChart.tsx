import { formatDate, formatMoney } from '@omvila/ui-web/format';
import { axisLabel, axisStep } from './period';
import styles from './BarChart.module.css';

const classNames = (...names: (string | false | undefined)[]) => names.filter(Boolean).join(' ');

export interface ChartBar {
  /** Yerel gün (yyyy-aa-gg). */
  day: string;
  amountMinor: number;
  /** Şube o gün kapalı: çubuk yerine taban çizgisi. */
  closed?: boolean;
  /** Dönemin henüz gelmemiş günü: boş sütun, tabloda yok (sahte değer yok). */
  future?: boolean;
  /** Vurgulu çubuk (Primim: en yoğun gün). */
  highlight?: boolean;
}

/**
 * Günlük tutar çubuk grafiği (salon-paneli/Raporlar "Günlük tahsilat", Primim "Bu hafta"). Grafik kütüphanesi yok;
 * çubuklar CSS ile çizilir. Ekran okuyucu ve klavye için "Tablo olarak göster" aynı veriyi tablo olarak verir.
 */
export function BarChart({
  bars,
  caption,
  valueHeading,
  currency,
  axis = false,
  tone = 'marka',
}: {
  bars: readonly ChartBar[];
  /** Tablo adı ("Günlük tahsilat"). */
  caption: string;
  /** Tablonun tutar kolonu ("Tahsilat", "Prim"). */
  valueHeading: string;
  currency: string;
  /** Sağda tutar ekseni ve yatay çizgiler. */
  axis?: boolean;
  /** "yumusak": çubuklar açık marka, vurgulu çubuk koyu (Primim). */
  tone?: 'marka' | 'yumusak';
}) {
  const max = Math.max(0, ...bars.map((b) => b.amountMinor));
  const step = axisStep(max);
  const top = axis && step > 0 ? step * 4 : max;
  const dense = bars.length > 14;
  const height = (amount: number) =>
    top > 0 ? `${String((Math.max(0, amount) / top) * 100)}%` : '0%';
  const label = (day: string) => (dense ? formatDate(day, 'day') : formatDate(day, 'weekdayShort'));
  const showLabel = (day: string, index: number) =>
    !dense || index === 0 || Number(formatDate(day, 'day')) % 5 === 0;

  return (
    <div className={styles.chart}>
      <div
        className={classNames(
          styles.plot,
          axis && styles.withAxis,
          tone === 'yumusak' && styles.short,
        )}
        role="img"
        aria-label={`${caption} grafiği; ayrıntı tabloda`}
      >
        {axis && step > 0 ? (
          <div className={styles.grid} aria-hidden="true">
            {[4, 3, 2, 1].map((n) => (
              <span key={n} className={styles.gridLine} style={{ bottom: `${String(n * 25)}%` }}>
                <span className={styles.gridLabel}>{axisLabel(step * n)}</span>
              </span>
            ))}
          </div>
        ) : null}
        <div className={styles.bars} aria-hidden="true">
          {bars.map((bar, index) => (
            <div key={bar.day} className={styles.column}>
              <div className={styles.barArea}>
                {bar.closed ? (
                  <span className={styles.closed} />
                ) : (
                  <span
                    className={classNames(
                      styles.bar,
                      tone === 'yumusak' && styles.soft,
                      bar.highlight && styles.highlight,
                      bar.future && styles.future,
                    )}
                    style={{ height: bar.future ? undefined : height(bar.amountMinor) }}
                  />
                )}
              </div>
              <span className={styles.label}>
                {showLabel(bar.day, index) ? label(bar.day) : ''}
              </span>
            </div>
          ))}
        </div>
      </div>
      <details className={styles.details}>
        <summary className={styles.summary}>Tablo olarak göster</summary>
        <table className={styles.table}>
          <caption className={styles.caption}>{caption}</caption>
          <thead>
            <tr>
              <th scope="col">Gün</th>
              <th scope="col" className={styles.numeric}>
                {valueHeading}
              </th>
            </tr>
          </thead>
          <tbody>
            {bars
              .filter((bar) => !bar.future)
              .map((bar) => (
                <tr key={bar.day}>
                  <th scope="row">{formatDate(bar.day, 'long')}</th>
                  <td className={styles.numeric}>
                    {bar.closed
                      ? 'Kapalı'
                      : formatMoney({ amountMinor: bar.amountMinor, currency })}
                  </td>
                </tr>
              ))}
          </tbody>
        </table>
      </details>
    </div>
  );
}
