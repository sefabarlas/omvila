import type { ReactNode } from 'react';
import styles from './LinkButton.module.css';

interface LinkButtonProps {
  href: string;
  /** Metnin önündeki dekoratif ikon. */
  icon?: ReactNode;
  children: ReactNode;
  /** Tam genişlik (Randevum "Yol tarifi", "Salonu ara"). */
  block?: boolean;
}

/** İkincil buton görünümlü bağlantı ("Salonu ara", "Salon sayfasına dön"). Dokunma alanı 44 px. */
export function LinkButton({ href, icon, children, block = false }: LinkButtonProps) {
  return (
    <a className={[styles.link, block && styles.block].filter(Boolean).join(' ')} href={href}>
      {icon ? (
        <span className={styles.icon} aria-hidden="true">
          {icon}
        </span>
      ) : null}
      {children}
    </a>
  );
}
