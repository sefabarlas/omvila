import logoUrl from '@omvila/design-tokens/assets/logos/omvila-logo-yatay-ters.svg';
import type { ReactNode } from 'react';
import styles from './AuthLayout.module.css';

export interface AuthLayoutProps {
  children: ReactNode;
  /** Marka panelindeki büyük başlık (satır sonu için ReactNode). */
  sloganTitle: ReactNode;
  sloganText: string;
}

/** Giriş ve kayıt düzeni (salon-paneli/Giris): marka paneli dar ekranda gizlenir. */
export function AuthLayout({ children, sloganTitle, sloganText }: AuthLayoutProps) {
  return (
    <div className={styles.layout}>
      <aside className={styles.brand} aria-hidden="true">
        <img className={styles.logo} src={logoUrl} alt="" />
        <div className={styles.shapes}>
          <span className={`${styles.shape ?? ''} ${styles.shapeKum ?? ''}`} />
          <span className={`${styles.shape ?? ''} ${styles.shapeVurgu ?? ''}`} />
          <span className={`${styles.shape ?? ''} ${styles.shapeMarka ?? ''}`} />
        </div>
        <div className={styles.slogan}>
          <p className={styles.sloganTitle}>{sloganTitle}</p>
          <p className={styles.sloganText}>{sloganText}</p>
        </div>
      </aside>
      <main className={styles.panel}>
        <div className={styles.form}>{children}</div>
      </main>
    </div>
  );
}
