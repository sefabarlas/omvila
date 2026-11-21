import type { ReactNode } from 'react';
import styles from './LinkButton.module.css';

interface LinkButtonProps {
  href: string;
  /** Metnin önündeki dekoratif ikon. */
  icon?: ReactNode;
  children: ReactNode;
}

/** İkincil buton görünümlü bağlantı ("Salonu ara", "Salon sayfasına dön"). Dokunma alanı 44 px. */
export function LinkButton({ href, icon, children }: LinkButtonProps) {
  return (
    <a className={styles.link} href={href}>
      {icon ? (
        <span className={styles.icon} aria-hidden="true">
          {icon}
        </span>
      ) : null}
      {children}
    </a>
  );
}
