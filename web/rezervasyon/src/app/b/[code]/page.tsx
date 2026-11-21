import type { Metadata } from 'next';
import { notFound } from 'next/navigation';
import { isAccessCode } from '@/appointment/appointment-api';
import { privateRobots } from '@/lib/metadata';
import { OfferManager } from '@/waitlist/OfferManager';

interface OfferPageProps {
  params: Promise<{ code: string }>;
}

export const metadata: Metadata = { title: 'Bekleme teklifi', robots: privateRobots };

/**
 * Bekleme teklifi bağlantısı (omvila.com/b/{kod}). İstemcide çizilir: teklif, ETag ve geri sayım tarayıcıda. Biçimi
 * bozuk kod API'ye gitmeden 404; bilinmeyen kod istemcide "Bu teklif bulunamadı".
 */
export default async function OfferPage({ params }: OfferPageProps) {
  const { code } = await params;
  if (!isAccessCode(code)) notFound();
  return <OfferManager code={code} />;
}
