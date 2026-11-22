import type { Metadata } from 'next';
import { noIndex, siteUrl } from './site';

const closedRobots: Metadata['robots'] = { index: false, follow: false };

/** Sayfa başlığı, açıklaması, canonical ve Open Graph (web/tanitim/AGENTS.md § SEO). */
export function pageMetadata(input: {
  title: string;
  description: string;
  path: string;
  /** Yer tutucu sayfalar arama motoruna girmez. */
  index?: boolean;
}): Metadata {
  const url = `${siteUrl()}${input.path}`;
  const closed = noIndex() || input.index === false;
  return {
    title: input.title,
    description: input.description,
    alternates: { canonical: url },
    openGraph: {
      type: 'website',
      locale: 'tr_TR',
      siteName: 'OMVILA',
      title: input.title,
      description: input.description,
      url,
    },
    twitter: { card: 'summary', title: input.title, description: input.description },
    ...(closed ? { robots: closedRobots } : {}),
  };
}
