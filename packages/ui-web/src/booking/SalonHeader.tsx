import { MapPin, Star } from 'lucide-react';
import type { ReactNode } from 'react';
import { initials } from '../format/name';
import { cx } from './internal';
import type { OpenStatus } from './openStatus';
import shared from './shared.module.css';
import styles from './SalonHeader.module.css';

/** BookingPageSettings.coverColor / PublicBrand.coverColor. */
export type CoverColor = 'petrol' | 'mercan' | 'kum';

export interface SalonRating {
  average: number;
  count: number;
}

export interface SalonHeaderProps {
  /** Marka adı: "Kuaför Elif". */
  name: string;
  logoUrl?: string | null;
  coverColor?: CoverColor;
  /** Kategori satırı: "Kuaför · Saç, bakım, makyaj". */
  category?: string | null;
  /** Yalnız salon gösteriyorsa ve eşik geçildiyse; yoksa satır gizlenir. */
  rating?: SalonRating | null;
  /** branchOpenStatus() sonucu: "Şu an açık · 20:00'ye kadar". */
  openStatus?: OpenStatus | null;
  /** "Moda Cad. 42, Kadıköy · İstanbul". */
  address?: string | null;
  /** page: mobil sayfa başı (köşesiz); card: masaüstü sol kolon kartı. */
  variant?: 'page' | 'card';
  /** Başlık düzeyi; sayfada tek h1 vardır. */
  headingLevel?: 1 | 2;
  /** Adresin yanına eklenecek ek (ör. "Yol tarifi" bağlantısı). */
  children?: ReactNode;
}

/** "4,9" (bir ondalık, virgül). */
export function formatRating(average: number): string {
  return average.toFixed(1).replace('.', ',');
}

/** Rezervasyon sayfası başlığı (components/SalonHeader): kapak, logo/monogram, ad, kategori, puan, açık durumu, adres. */
export function SalonHeader({
  name,
  logoUrl,
  coverColor = 'petrol',
  category,
  rating,
  openStatus,
  address,
  variant = 'page',
  headingLevel = 1,
  children,
}: SalonHeaderProps) {
  const Heading = headingLevel === 1 ? 'h1' : 'h2';
  return (
    <header className={cx(styles.root, variant === 'card' && styles.card)}>
      <div className={cx(styles.cover, styles[coverColor])} aria-hidden="true">
        <span className={styles.shape} />
      </div>
      <span className={styles.logo}>
        {logoUrl ? <img src={logoUrl} alt="" /> : <span aria-hidden="true">{initials(name)}</span>}
      </span>
      <div className={styles.body}>
        <Heading className={styles.name}>{name}</Heading>
        {category ? <p className={styles.category}>{category}</p> : null}
        {rating || openStatus ? (
          <div className={styles.meta}>
            {rating ? (
              <span className={styles.rating}>
                <span className={shared.visuallyHidden}>
                  {`Puan 5 üzerinden ${formatRating(rating.average)}, ${String(rating.count)} değerlendirme`}
                </span>
                <Star className={styles.star} size={16} fill="currentColor" aria-hidden="true" />
                <span className={shared.num} aria-hidden="true">
                  {formatRating(rating.average)}
                </span>
                <span className={styles.reviews} aria-hidden="true">
                  ({rating.count} değerlendirme)
                </span>
              </span>
            ) : null}
            {openStatus ? (
              <span className={cx(styles.pill, openStatus.open ? styles.open : styles.closed)}>
                {openStatus.label}
              </span>
            ) : null}
          </div>
        ) : null}
        {address ? (
          <p className={styles.address}>
            <MapPin size={16} aria-hidden="true" className={styles.pin} />
            <span>{address}</span>
          </p>
        ) : null}
        {children}
      </div>
    </header>
  );
}
