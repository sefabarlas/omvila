import type { Metadata } from 'next';
import { notFound, permanentRedirect } from 'next/navigation';
import { BookingFlow } from '@/booking/BookingFlow';
import { loadBrand } from '@/lib/brand';
import { brandMetadata, notFoundTitle, privateRobots } from '@/lib/metadata';

interface SalonPageProps {
  params: Promise<{ slug: string }>;
  searchParams?: Promise<Record<string, string | string[] | undefined>>;
}

export async function generateMetadata({ params }: SalonPageProps): Promise<Metadata> {
  const result = await loadBrand((await params).slug);
  if (result.kind === 'found') return brandMetadata(result.brand);
  // 404 belgesi istemcide çizilir ve sayfanın metadatasını kullanır; başlık not-found.tsx ile aynı olmalı.
  return { title: notFoundTitle, robots: privateRobots };
}

/** Sunucunun gördüğü arama dizesi; istemci akışı ilk çizimi bununla yapar (hidrasyon adresle aynı). */
function searchString(params: Record<string, string | string[] | undefined>): string {
  const search = new URLSearchParams();
  for (const [key, value] of Object.entries(params)) {
    if (typeof value === 'string') search.set(key, value);
  }
  const query = search.toString();
  return query ? `?${query}` : '';
}

/**
 * Salon sayfası (omvila.com/{slug}). Sunucuda salon kartı okunur (Open Graph, 301, 404); randevu adımları
 * istemci bileşenidir (T-56). Online randevu kapalıysa akış "Online randevu kapalı · Salonu ara" gösterir.
 */
export default async function SalonPage({ params, searchParams }: SalonPageProps) {
  const result = await loadBrand((await params).slug);
  if (result.kind === 'moved') permanentRedirect(`/${result.slug}`);
  if (result.kind === 'notFound') notFound();

  return (
    <BookingFlow brand={result.brand} initialSearch={searchString((await searchParams) ?? {})} />
  );
}
