import { Topbar } from '@omvila/ui-web/panel';
import type { TopbarProps } from '@omvila/ui-web/panel';
import { useNavigate } from '@tanstack/react-router';
import type { ReactNode } from 'react';
import { useSession } from '@/app/session';
import styles from './Page.module.css';

export interface PageProps extends Omit<TopbarProps, 'search' | 'notifications'> {
  children?: ReactNode;
}

/**
 * Platform sayfası: üst bar (marka araması ⌘K → Markalar listesinde arama; zil rozeti onay kuyruğundaki bekleyenler,
 * session.pendingApprovalCount) ve içerik. Onay kuyruğu ekranı gelene kadar zil Genel bakış'ı açar (T-123).
 */
export function Page({ children, ...topbar }: PageProps) {
  const { session } = useSession();
  const navigate = useNavigate();
  return (
    <>
      <Topbar
        search={{
          placeholder: 'Marka, telefon veya bağlantı ara',
          label: 'Marka ara',
          onSearch: (query) => {
            const q = query.trim();
            void navigate({ to: '/markalar', search: q.length >= 2 ? { q } : {} });
          },
        }}
        notifications={{
          count: session.pendingApprovalCount,
          onOpen: () => void navigate({ to: '/' }),
        }}
        {...topbar}
      />
      <div className={styles.content}>{children}</div>
    </>
  );
}
