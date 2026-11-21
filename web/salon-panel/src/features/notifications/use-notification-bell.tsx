import type { TopbarProps } from '@omvila/ui-web/panel';
import { useQuery } from '@tanstack/react-query';
import { useCallback, useId, useRef, useState } from 'react';
import { useApi } from '@/app/api-context';
import { notificationsQuery } from './api';
import { NotificationPanel } from './NotificationPanel';

/**
 * Üst bardaki zil (Topbar `notifications`): okunmamış sayısı yalnız sunucudan (unreadCount); yüklenirken veya hata
 * olduğunda rozet yoktur. Zile basmak paneli açar/kapatır; klavyeyle kapatınca odak zile döner.
 */
export function useNotificationBell(): NonNullable<TopbarProps['notifications']> {
  const query = useQuery(notificationsQuery(useApi()));
  const [open, setOpen] = useState(false);
  const opener = useRef<HTMLElement | null>(null);
  const panelId = `bildirimler-${useId().replace(/:/g, '')}`;

  const close = useCallback((restoreFocus: boolean) => {
    setOpen(false);
    if (restoreFocus) {
      opener.current?.focus();
    }
  }, []);

  return {
    ...(query.data ? { count: query.data.unreadCount } : {}),
    onOpen: () => {
      opener.current =
        document.activeElement instanceof HTMLElement ? document.activeElement : null;
      setOpen((value) => !value);
    },
    expanded: open,
    ...(open
      ? { controls: panelId, panel: <NotificationPanel id={panelId} onClose={close} /> }
      : {}),
  };
}
