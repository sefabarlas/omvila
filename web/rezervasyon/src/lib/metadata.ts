import type { Metadata } from 'next';
import type { PublicBrand } from './brand';

/** Sayfanın kendi kökü (Open Graph bağlantıları): NEXT_PUBLIC_SITE_URL, yoksa yerel geliştirme. */
export const siteUrl: string = process.env.NEXT_PUBLIC_SITE_URL ?? 'http://localhost:3000';

/** Bağlantı önizlemesi (design/ekranlar/bildirim-dagitim/PaylasimOnizleme.png): salon adı, açıklama, logo. */
export function brandMetadata(brand: PublicBrand): Metadata {
  const title = `${brand.name} · Online randevu`;
  const short = brand.shortDescription?.trim();
  const description =
    short !== undefined && short.length > 0
      ? short
      : `${brand.name} için hizmetinizi ve saatinizi seçin, üye olmadan randevu alın.`;
  const url = `${siteUrl}/${brand.slug}`;
  const images = brand.logoUrl ? [{ url: brand.logoUrl, alt: brand.name }] : undefined;

  return {
    title,
    description,
    alternates: { canonical: url },
    openGraph: {
      type: 'website',
      locale: 'tr_TR',
      siteName: 'OMVILA',
      title,
      description,
      url,
      ...(images ? { images } : {}),
    },
    twitter: {
      card: 'summary',
      title,
      description,
      ...(images ? { images } : {}),
    },
  };
}

/** Geçersiz salon bağlantısının başlığı (not-found.tsx ve salon sayfasının 404 metadatası). */
export const notFoundTitle = 'Bu bağlantı geçersiz';

/** Kişiye özel bağlantılar (/r/, durum sayfaları) arama motoruna girmez. */
export const privateRobots: Metadata['robots'] = { index: false, follow: false };
