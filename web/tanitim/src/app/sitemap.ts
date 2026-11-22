import type { MetadataRoute } from 'next';
import { siteUrl } from '@/lib/site';

// Statik çıktıda out/sitemap.xml. Yer tutucu yasal sayfalar dahil edilmez. lastModified yazılmaz (derleme saati
// içeriğin değiştiği an değildir).
export const dynamic = 'force-static';

export default function sitemap(): MetadataRoute.Sitemap {
  return [{ url: `${siteUrl()}/` }, { url: `${siteUrl()}/fiyatlar/` }];
}
