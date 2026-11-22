import type { MetadataRoute } from 'next';
import { noIndex, siteUrl } from '@/lib/site';

// Statik çıktıda out/robots.txt. Test ortamında (NEXT_PUBLIC_NOINDEX=true) bütün site kapalı.
export const dynamic = 'force-static';

export default function robots(): MetadataRoute.Robots {
  if (noIndex()) {
    return { rules: { userAgent: '*', disallow: '/' } };
  }
  return {
    rules: { userAgent: '*', allow: '/', disallow: '/yasal/' },
    sitemap: `${siteUrl()}/sitemap.xml`,
  };
}
