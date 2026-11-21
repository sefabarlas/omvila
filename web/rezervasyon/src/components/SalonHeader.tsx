import { MapPin, Star } from 'lucide-react';
import type { PublicBrand } from '@/lib/brand';
import styles from './SalonHeader.module.css';

const coverClass: Record<PublicBrand['coverColor'], string | undefined> = {
  petrol: styles.coverPetrol,
  mercan: styles.coverMercan,
  kum: styles.coverKum,
};

const ratingFormat = new Intl.NumberFormat('tr-TR', {
  minimumFractionDigits: 1,
  maximumFractionDigits: 1,
});
const countFormat = new Intl.NumberFormat('tr-TR');

/** Logo yoksa adın ilk iki kelimesinin baş harfleri ("Kuaför Elif" → "KE"). */
export function monogram(name: string): string {
  return name
    .split(/\s+/)
    .filter(Boolean)
    .slice(0, 2)
    .map((word) => word.charAt(0))
    .join('')
    .toLocaleUpperCase('tr-TR');
}

function addressLine(brand: PublicBrand): string | undefined {
  const [only, ...rest] = brand.branches;
  if (!only) return undefined;
  if (rest.length > 0) return `${String(brand.branches.length)} şube`;
  return `${only.addressLine}, ${only.district} · ${only.city}`;
}

/**
 * Salon kartı (Main.png üst bölümü): kapak, logo veya monogram, ad, sektör, puan, adres.
 * Geçici: @omvila/ui-web/booking SalonHeader'ı gelince onunla değiştirilir.
 */
export function SalonHeader({ brand }: { brand: PublicBrand }) {
  const subtitle = [brand.industryLabel, brand.shortDescription].filter(Boolean).join(' · ');
  const address = addressLine(brand);

  return (
    <header className={styles.header}>
      <div
        className={[styles.cover, coverClass[brand.coverColor]].filter(Boolean).join(' ')}
        aria-hidden="true"
      />
      <div className={styles.body}>
        <div className={styles.logo}>
          {brand.logoUrl ? (
            <img className={styles.logoImage} src={brand.logoUrl} alt="" width={72} height={72} />
          ) : (
            <span aria-hidden="true">{monogram(brand.name)}</span>
          )}
        </div>
        <h1 className={styles.name}>{brand.name}</h1>
        {subtitle ? <p className={styles.meta}>{subtitle}</p> : null}
        {brand.rating ? (
          <p className={styles.rating}>
            <Star size={16} aria-hidden="true" className={styles.star} />
            <span>{ratingFormat.format(brand.rating.average)}</span>
            <span className={styles.count}>
              ({countFormat.format(brand.rating.count)} değerlendirme)
            </span>
          </p>
        ) : null}
        {address ? (
          <p className={styles.meta}>
            <MapPin size={16} aria-hidden="true" />
            <span>{address}</span>
          </p>
        ) : null}
      </div>
    </header>
  );
}
