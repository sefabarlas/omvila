import type { Metadata } from 'next';
import { LinkMissing } from '@/components/LinkMissing';
import { privateRobots } from '@/lib/metadata';

export const metadata: Metadata = { title: 'Bu bağlantı geçersiz', robots: privateRobots };

/** Biçimi bozuk değerlendirme kodu (sunucuda 404). Bilinmeyen kodu istemci aynı bileşenle gösterir (ReviewForm). */
export default function ReviewNotFound() {
  return <LinkMissing title="Bu bağlantı geçersiz" />;
}
