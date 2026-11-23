import Link from 'next/link';
import { signInUrl, signUpUrl } from '@/lib/site';
import { Logo } from './Logo';
import { MobileMenu } from './MobileMenu';
import { ThemeMenu } from './ThemeMenu';
import { navItems } from './nav';
import styles from './SiteHeader.module.css';
import ui from './ui.module.css';
import { cx } from '@/lib/cx';

export function SiteHeader() {
  return (
    <header className={cx(ui.container, styles.header)}>
      <Link href="/" className={styles.brand} aria-label="OMVILA ana sayfa">
        <Logo />
      </Link>
      <nav aria-label="Ana menü" className={styles.nav}>
        {navItems.map((item) => (
          <Link key={item.href} href={item.href} className={styles.navLink}>
            {item.label}
          </Link>
        ))}
      </nav>
      <div className={styles.actions}>
        <ThemeMenu />
        <a href={signInUrl()} className={cx(ui.button, ui.ghost, styles.signIn)}>
          Giriş yap
        </a>
        <a href={signUpUrl()} className={cx(ui.button, ui.primary)}>
          Ücretsiz dene
        </a>
        <MobileMenu signInHref={signInUrl()} />
      </div>
    </header>
  );
}
