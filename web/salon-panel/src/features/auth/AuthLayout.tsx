import logoUrl from '@omvila/design-tokens/assets/logos/omvila-logo-yatay-ters.svg';
import type { ReactNode } from 'react';
import styles from './AuthLayout.module.css';

/** Giriş ve kayıt düzeni (salon-paneli/Giris): marka paneli dar ekranda gizlenir. */
export function AuthLayout({ children }: { children: ReactNode }) {
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
          <p className={styles.sloganTitle}>
            Her randevu
            <br />
            tek yerde.
          </p>
          <p className={styles.sloganText}>
            Takvim, müşteriler, personel ve kasa. Salonunuzun bütün günü tek ekranda.
          </p>
        </div>
      </aside>
      <main className={styles.panel}>
        <div className={styles.form}>{children}</div>
      </main>
    </div>
  );
}
