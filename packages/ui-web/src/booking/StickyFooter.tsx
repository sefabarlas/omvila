import type { ReactNode } from 'react';
import { cx } from './internal';
import styles from './StickyFooter.module.css';

export interface StickyFooterProps {
  /** Üst satır, kalın: "700 ₺", "Cum 28 Kas · 15:15", "Elif Kaya". */
  summary?: ReactNode;
  /** Alt satır: "2 hizmet · 1 sa 15 dk", "16:30'da biter". */
  detail?: ReactNode;
  /** Ana eylem (BookingButton). Özet yoksa tam genişlik olur. */
  action: ReactNode;
  /** Butonun altındaki kısa not: "Bu bilgiler SMS ile de gönderildi." */
  note?: ReactNode;
  /** sticky: mobilde ekranın altına yapışık; inline: masaüstü adım sonu (çizgi + sağda buton). */
  layout?: 'sticky' | 'inline';
  className?: string;
}

/**
 * Rezervasyon alt çubuğu (components/StickyFooter): toplam tutar ve süre + ana eylem.
 * Özet canlı bölgedir; seçim değiştikçe ekran okuyucu yeni toplamı okur.
 */
export function StickyFooter({
  summary,
  detail,
  action,
  note,
  layout = 'sticky',
  className,
}: StickyFooterProps) {
  const hasSummary = summary != null || detail != null;
  return (
    <div className={cx(styles.footer, styles[layout], className)}>
      <div className={styles.row}>
        {hasSummary ? (
          <div className={styles.summary} aria-live="polite" aria-atomic="true">
            {summary != null ? <strong className={styles.total}>{summary}</strong> : null}
            {detail != null ? <span className={styles.detail}>{detail}</span> : null}
          </div>
        ) : null}
        <div className={cx(styles.action, !hasSummary && styles.actionBlock)}>{action}</div>
      </div>
      {note ? <p className={styles.note}>{note}</p> : null}
    </div>
  );
}
