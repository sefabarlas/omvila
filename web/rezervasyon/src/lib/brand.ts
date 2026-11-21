import type { components } from '@omvila/api-client-ts';
import { cache } from 'react';
import { createApi } from './api';

export type PublicBrand = components['schemas']['PublicBrand'];

export type BrandResult =
  { kind: 'found'; brand: PublicBrand } | { kind: 'moved'; slug: string } | { kind: 'notFound' };

// Sözleşmedeki brandSlug deseni; uymayan adres API'ye gitmeden geçersiz sayılır.
const slugPattern = /^[a-z0-9-]{3,60}$/;

/** `Location: /api/v1/public/brands/{yeni}` başlığından yeni bağlantı adı. */
function slugFromLocation(location: string | null): string | undefined {
  const last = location?.split('?')[0]?.split('/').filter(Boolean).pop();
  return last && slugPattern.test(last) ? last : undefined;
}

/**
 * Salon kartını okur. Eski bağlantı adı (301) yeni adrese, bilinmeyen ad "bulunamadı"ya çıkar.
 * fetch yönlendirmeyi kendisi izlerse 200 gövdesindeki `slug` istenenden farklı olur; o da taşınmış sayılır.
 * İstek başına bir kez çalışır (generateMetadata ve sayfa aynı sonucu paylaşır).
 */
export const loadBrand = cache(async (slug: string): Promise<BrandResult> => {
  if (!slugPattern.test(slug)) return { kind: 'notFound' };

  const { data, response } = await createApi().GET('/api/v1/public/brands/{brandSlug}', {
    params: { path: { brandSlug: slug } },
    redirect: 'manual',
    cache: 'no-store',
  });

  if (response.status === 301 || response.status === 308) {
    const moved = slugFromLocation(response.headers.get('Location'));
    return moved ? { kind: 'moved', slug: moved } : { kind: 'notFound' };
  }
  if (response.status === 404) return { kind: 'notFound' };
  if (!response.ok || !data) {
    // 5xx ve ağ hataları error.tsx'e düşer; kişisel veri yok, yalnız durum kodu.
    throw new Error(`publicGetBrand ${String(response.status)}`);
  }
  if (data.slug !== slug) return { kind: 'moved', slug: data.slug };
  return { kind: 'found', brand: data };
});
