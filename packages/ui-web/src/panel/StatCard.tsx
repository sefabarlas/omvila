import { TrendingDown, TrendingUp } from 'lucide-react';
import type { ReactNode } from 'react';
import { classNames } from './Button';
import styles from './Stat.module.css';

export interface StatCardProps {
  label: string;
  /** Biçimlenmiş değer ("12", "8.450 ₺"); veri yoksa "—" (sahte sayı yok). */
  value: string;
  /** Değişim: yön ve anlam ayrı (gelmeyen artarsa kırmızı). */
  delta?: { text: string; direction: 'up' | 'down'; meaning: 'good' | 'bad' };
  /** Neye göre: "geçen salıya göre". */
  context?: string;
  icon?: ReactNode;
}

/** Özet ekranındaki tek metrik; bir satırda en fazla dört. */
export function StatCard({ label, value, delta, context, icon }: StatCardProps) {
  const Trend = delta?.direction === 'down' ? TrendingDown : TrendingUp;
  return (
    <section className={styles.card} aria-label={label}>
      <div className={styles.head}>
        <span>{label}</span>
        {icon ? <span aria-hidden="true">{icon}</span> : null}
      </div>
      <div className={styles.value}>{value}</div>
      {delta || context ? (
        <div className={styles.foot}>
          {delta ? (
            <span
              className={classNames(
                styles.delta,
                delta.meaning === 'good' ? styles.good : styles.bad,
              )}
            >
              <Trend size={14} aria-hidden="true" />
              {delta.text}
            </span>
          ) : null}
          {context ? <span>{context}</span> : null}
        </div>
      ) : null}
    </section>
  );
}

export interface BentoProps {
  children: ReactNode;
  label?: string;
}

/** Özet blok ızgarası (4 sütun; dar ekranda 2). */
export function Bento({ children, label }: BentoProps) {
  return (
    <div className={styles.bento} role={label ? 'group' : undefined} aria-label={label}>
      {children}
    </div>
  );
}

export interface BentoBlockProps {
  label: string;
  value: string;
  sub?: string;
  tone?: 'default' | 'marka' | 'vurgu' | 'kum';
  /** Bloğa tıklamak ilgili sayfayı açar. */
  href?: string;
}

/** Özet bloğu: etiket, büyük sayı, alt satır. Sayılar sayarak artmaz. */
export function BentoBlock({ label, value, sub, tone = 'default', href }: BentoBlockProps) {
  const content = (
    <>
      <span className={styles.label}>{label}</span>
      <span className={styles.value}>{value}</span>
      {sub ? <span className={styles.sub}>{sub}</span> : null}
    </>
  );
  const className = classNames(styles.block, tone !== 'default' && styles[tone]);
  return href ? (
    <a href={href} className={className}>
      {content}
    </a>
  ) : (
    <div className={className}>{content}</div>
  );
}
