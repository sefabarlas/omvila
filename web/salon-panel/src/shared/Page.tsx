import { Topbar } from '@omvila/ui-web/panel';
import type { TopbarProps } from '@omvila/ui-web/panel';
import type { ReactNode } from 'react';
import { defaultTimeZone } from '@/app/config';
import { useSession } from '@/app/session';
import { SubscriptionBanner } from '@/app/SubscriptionBanner';
import { SmsCreditsBanner } from '@/features/notifications/SmsCreditsBanner';
import { useNotificationBell } from '@/features/notifications/use-notification-bell';
import styles from './Page.module.css';

export interface PageProps extends TopbarProps {
  children?: ReactNode;
}

/**
 * Panel sayfası: üst bar (zil: salon-paneli/BildirimPaneli), abonelik ve kontör şeritleri
 * (salon-paneli/GenelBakisGecikti, Durumlar: üst barın altında), içerik.
 */
export function Page({ children, ...topbar }: PageProps) {
  const { session, can } = useSession();
  const bell = useNotificationBell();
  return (
    <>
      <Topbar notifications={bell} {...topbar} />
      <div className={styles.content}>
        <SubscriptionBanner
          subscription={session.subscription}
          timeZone={defaultTimeZone}
          manages={can('settings.manage')}
        />
        <SmsCreditsBanner smsCredits={session.smsCredits} />
        {children}
      </div>
    </>
  );
}
