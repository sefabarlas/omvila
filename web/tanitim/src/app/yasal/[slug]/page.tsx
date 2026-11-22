import { notFound } from 'next/navigation';
import { legalPages } from '@/components/legal';
import ui from '@/components/ui.module.css';
import { pageMetadata } from '@/lib/metadata';
import styles from './page.module.css';
import { cx } from '@/lib/cx';

// Yer tutucu: yasal metinler hukuk onayı bekliyor (teslim-notlari.md); içerik yazılmaz, sayfa arama motoruna girmez.
export const dynamicParams = false;

export function generateStaticParams() {
  return legalPages.map((page) => ({ slug: page.slug }));
}

function find(slug: string) {
  return legalPages.find((page) => page.slug === slug);
}

export async function generateMetadata({ params }: { params: Promise<{ slug: string }> }) {
  const { slug } = await params;
  const page = find(slug);
  return pageMetadata({
    title: `${page?.title ?? 'Sayfa'} · OMVILA`,
    description: 'Bu sayfa hazırlanıyor.',
    path: `/yasal/${slug}/`,
    index: false,
  });
}

export default async function LegalPage({ params }: { params: Promise<{ slug: string }> }) {
  const { slug } = await params;
  const page = find(slug);
  if (!page) notFound();
  return (
    <section className={cx(ui.container, styles.page)}>
      <h1 className={ui.sectionTitle}>{page.title}</h1>
      <p className={ui.lead}>Bu sayfa hazırlanıyor.</p>
    </section>
  );
}
