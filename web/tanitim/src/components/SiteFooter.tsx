import Link from 'next/link';
import { signInUrl, site } from '@/lib/site';
import { legalPages } from './legal';
import { Logo } from './Logo';
import styles from './SiteFooter.module.css';
import ui from './ui.module.css';
import { cx } from '@/lib/cx';

export function SiteFooter() {
  return (
    <footer className={styles.footer}>
      <div className={cx(ui.container, styles.grid)}>
        <div className={styles.about}>
          <Logo inverse />
          <p className={styles.muted}>
            Randevuyla çalışan her işletme için online randevu ve işletme yönetimi.
          </p>
        </div>
        <nav aria-labelledby="footer-urun" className={styles.column}>
          <h2 id="footer-urun" className={styles.heading}>
            Ürün
          </h2>
          <Link href="/#ozellikler">Özellikler</Link>
          <Link href="/fiyatlar/">Fiyatlar</Link>
          <a href={signInUrl()}>Giriş yap</a>
        </nav>
        <nav aria-labelledby="footer-yasal" className={styles.column}>
          <h2 id="footer-yasal" className={styles.heading}>
            Yasal
          </h2>
          {legalPages.map((page) => (
            <Link key={page.slug} href={`/yasal/${page.slug}/`}>
              {page.title}
            </Link>
          ))}
        </nav>
      </div>
      <div className={ui.container}>
        <p className={styles.copyright}>© {site.copyrightYear} OMVILA</p>
      </div>
    </footer>
  );
}
