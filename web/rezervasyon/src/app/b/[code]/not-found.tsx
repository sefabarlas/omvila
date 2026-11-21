import type { Metadata } from 'next';
import { LinkMissing } from '@/components/LinkMissing';
import { privateRobots } from '@/lib/metadata';

export const metadata: Metadata = { title: 'Bu teklif bulunamadı', robots: privateRobots };

/** Biçimi bozuk teklif kodu (sunucuda 404). Bilinmeyen kodu istemci aynı bileşenle gösterir (OfferManager). */
export default function OfferNotFound() {
  return <LinkMissing title="Bu teklif bulunamadı" />;
}
