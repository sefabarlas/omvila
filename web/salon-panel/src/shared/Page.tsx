import { Topbar } from '@omvila/ui-web/panel';
import type { TopbarProps } from '@omvila/ui-web/panel';
import type { ReactNode } from 'react';
import { defaultTimeZone } from '@/app/config';
import { useSession } from '@/app/session';
import { SubscriptionBanner } from '@/app/SubscriptionBanner';
import styles from './Page.module.css';

export interface PageProps extends TopbarProps {
  children?: ReactNode;
}

/** Panel sayfası: üst bar, abonelik şeridi (salon-paneli/GenelBakisGecikti: üst barın altında), içerik. */
export function Page({ children, ...topbar }: PageProps) {
  const { session } = useSession();
  return (
    <>
      <Topbar {...topbar} />
      <div className={styles.content}>
        <SubscriptionBanner subscription={session.subscription} timeZone={defaultTimeZone} />
        {children}
      </div>
    </>
  );
}
