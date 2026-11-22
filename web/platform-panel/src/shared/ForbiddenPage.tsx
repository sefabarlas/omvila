import { EmptyState } from '@omvila/ui-web/panel';
import { Lock } from 'lucide-react';
import styles from './StatusPage.module.css';

/** 403 (salon-paneli/Durumlar "Yetkiniz yok" deseni): rolün izni olmayan sayfa veya uçtan 403. */
export function ForbiddenPage() {
  return (
    <div className={styles.page}>
      <div className={styles.card}>
        <EmptyState
          icon={<Lock size={22} />}
          title="Bu sayfayı görme yetkiniz yok"
          description="Bu bölüm platform rolünüze açık değil. Yetki için platform yöneticisine başvurun."
          headingLevel={1}
        />
      </div>
    </div>
  );
}
