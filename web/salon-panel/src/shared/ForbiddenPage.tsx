import { EmptyState } from '@omvila/ui-web/panel';
import { Lock } from 'lucide-react';
import styles from './StatusPage.module.css';

/** 403 (salon-paneli/Durumlar "Yetkiniz yok"). */
export function ForbiddenPage() {
  return (
    <div className={styles.page}>
      <div className={styles.card}>
        <EmptyState
          icon={<Lock size={22} />}
          title="Bu sayfayı görme yetkiniz yok"
          description="Bu bölüm rolünüze açık değil. Yetki için işletme sahibine başvurun."
          headingLevel={1}
        />
      </div>
    </div>
  );
}
