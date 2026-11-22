import type { Metadata } from 'next';
import Link from 'next/link';
import ui from '@/components/ui.module.css';
import styles from './yasal/[slug]/page.module.css';
import { cx } from '@/lib/cx';

export const metadata: Metadata = {
  title: 'Sayfa bulunamadı · OMVILA',
  robots: { index: false, follow: false },
};

export default function NotFound() {
  return (
    <section className={cx(ui.container, styles.page)}>
      <h1 className={ui.sectionTitle}>Sayfa bulunamadı</h1>
      <p className={ui.lead}>Adres yanlış yazılmış ya da sayfa kaldırılmış olabilir.</p>
      <p>
        <Link href="/" className={cx(ui.button, ui.primary)}>
          Ana sayfaya dön
        </Link>
      </p>
    </section>
  );
}
