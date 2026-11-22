'use client';

import { Menu, X } from 'lucide-react';
import Link from 'next/link';
import { useId, useState } from 'react';
import { navItems } from './nav';
import styles from './SiteHeader.module.css';

/** 900 px altında üst menünün yerine geçen menü butonu ve açılır liste. */
export function MobileMenu({ signInHref }: { signInHref: string }) {
  const [open, setOpen] = useState(false);
  const panelId = useId();
  const close = () => {
    setOpen(false);
  };

  return (
    <div className={styles.mobile}>
      <button
        type="button"
        className={styles.menuButton}
        aria-expanded={open}
        aria-controls={panelId}
        aria-label={open ? 'Menüyü kapat' : 'Menüyü aç'}
        onClick={() => {
          setOpen((value) => !value);
        }}
      >
        {open ? <X size={20} aria-hidden /> : <Menu size={20} aria-hidden />}
      </button>
      {open && (
        <nav id={panelId} aria-label="Menü" className={styles.panel}>
          {navItems.map((item) => (
            <Link key={item.href} href={item.href} className={styles.panelLink} onClick={close}>
              {item.label}
            </Link>
          ))}
          <a href={signInHref} className={styles.panelLink}>
            Giriş yap
          </a>
        </nav>
      )}
    </div>
  );
}
