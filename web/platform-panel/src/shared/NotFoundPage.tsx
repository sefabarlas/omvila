import { Button, EmptyState } from '@omvila/ui-web/panel';
import { useNavigate } from '@tanstack/react-router';
import { Search } from 'lucide-react';
import styles from './StatusPage.module.css';

/** 404: yok veya kapsam dışı kayıt aynı görünür. */
export function NotFoundPage() {
  const navigate = useNavigate();
  return (
    <div className={styles.page}>
      <div className={styles.card}>
        <EmptyState
          icon={<Search size={22} />}
          title="Bu kayıt bulunamadı"
          description="Silinmiş ya da adresi değişmiş olabilir."
          headingLevel={1}
          actions={<Button onClick={() => void navigate({ to: '/' })}>Genel bakışa dön</Button>}
        />
      </div>
    </div>
  );
}
