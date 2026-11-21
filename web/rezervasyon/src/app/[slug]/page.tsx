import type { Metadata } from 'next';
import { notFound, permanentRedirect } from 'next/navigation';
import { BookingDisabled } from '@/components/BookingDisabled';
import { SalonHeader } from '@/components/SalonHeader';
import { loadBrand } from '@/lib/brand';
import { brandMetadata, notFoundTitle, privateRobots } from '@/lib/metadata';
import styles from './page.module.css';

interface SalonPageProps {
  params: Promise<{ slug: string }>;
}

export async function generateMetadata({ params }: SalonPageProps): Promise<Metadata> {
  const result = await loadBrand((await params).slug);
  if (result.kind === 'found') return brandMetadata(result.brand);
  // 404 belgesi istemcide çizilir ve sayfanın metadatasını kullanır; başlık not-found.tsx ile aynı olmalı.
  return { title: notFoundTitle, robots: privateRobots };
}

/**
 * Salon sayfası (omvila.com/{slug}). Sunucuda salon kartı okunur; randevu adımları (Hizmet → Personel →
 * Saat → Onay) istemci bileşeni olarak sonraki PR'da bu sayfanın altına gelir.
 */
export default async function SalonPage({ params }: SalonPageProps) {
  const result = await loadBrand((await params).slug);
  if (result.kind === 'moved') permanentRedirect(`/${result.slug}`);
  if (result.kind === 'notFound') notFound();
  const { brand } = result;

  return (
    <main className={styles.page}>
      <SalonHeader brand={brand} />
      {/* Randevu akışı (sonraki PR) bookingEnabled iken buraya gelir: ?branch=…&services=…&staff=…&date=…&time=… */}
      {brand.bookingEnabled ? null : (
        <div className={styles.content}>
          <BookingDisabled brand={brand} />
        </div>
      )}
    </main>
  );
}
