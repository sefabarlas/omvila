import type { Metadata } from 'next';
import { notFound } from 'next/navigation';
import { isAccessCode } from '@/appointment/appointment-api';
import { privateRobots } from '@/lib/metadata';
import { ReviewForm } from '@/review/ReviewForm';

interface ReviewPageProps {
  params: Promise<{ code: string }>;
}

export const metadata: Metadata = { title: 'Değerlendirme', robots: privateRobots };

/**
 * Değerlendirme bağlantısı (omvila.com/d/{kod}, review.invite). İstemcide çizilir. Biçimi bozuk kod API'ye gitmeden
 * 404; bilinmeyen veya süresi dolmuş kod istemcide "Bu bağlantı geçersiz".
 */
export default async function ReviewPage({ params }: ReviewPageProps) {
  const { code } = await params;
  if (!isAccessCode(code)) notFound();
  return <ReviewForm code={code} />;
}
