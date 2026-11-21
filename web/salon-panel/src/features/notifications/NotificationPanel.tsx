import { ApiProblem } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { Button, EmptyState, Skeleton } from '@omvila/ui-web/panel';
import { useQuery } from '@tanstack/react-query';
import { useNavigate } from '@tanstack/react-router';
import {
  Banknote,
  Bell,
  BellOff,
  CalendarPlus,
  CalendarX,
  ClipboardList,
  Clock,
  Coins,
  CreditCard,
  Hourglass,
  Star,
  UserCheck,
  Users,
} from 'lucide-react';
import type { LucideIcon } from 'lucide-react';
import { useEffect, useRef, useState } from 'react';
import { useApi } from '@/app/api-context';
import { useClock } from '@/app/clock';
import { defaultTimeZone } from '@/app/config';
import { useSession } from '@/app/session';
import { notificationsQuery, useMarkNotificationsRead } from './api';
import type { InAppNotification } from './api';
import { holdRemaining, notificationRoute, notificationTime } from './notification-view';
import styles from './Notifications.module.css';

type Tone = 'bilgi' | 'uyari' | 'hata' | 'vurgu' | 'notr';

// Olay → ikon ve renk (salon-paneli/BildirimPaneli). Bilinmeyen olay genel bildirim olarak gösterilir.
const looks: Record<string, { icon: LucideIcon; tone: Tone }> = {
  'staff.approval_needed': { icon: Clock, tone: 'bilgi' },
  'staff.appointment_created': { icon: CalendarPlus, tone: 'notr' },
  'staff.appointment_changed': { icon: CalendarX, tone: 'notr' },
  'staff.customer_checked_in': { icon: UserCheck, tone: 'notr' },
  'staff.deposit_reported': { icon: Banknote, tone: 'uyari' },
  'staff.review_received': { icon: Star, tone: 'vurgu' },
  'staff.walkin_queue': { icon: Users, tone: 'notr' },
  'staff.daily_summary': { icon: ClipboardList, tone: 'notr' },
  'owner.credits_low': { icon: Coins, tone: 'uyari' },
  'owner.subscription_payment_failed': { icon: CreditCard, tone: 'hata' },
  'owner.trial_ending': { icon: Hourglass, tone: 'bilgi' },
};
const fallbackLook = { icon: Bell, tone: 'notr' as Tone };

/** Canlı kalan süre için yenileme aralığı (dakika çözünürlüğü yeterli). */
const tickMs = 30 * 1000;

export interface NotificationPanelProps {
  id: string;
  /** Paneli kapatır; `restoreFocus` klavyeyle kapatınca odağı zile döndürür. */
  onClose: (restoreFocus: boolean) => void;
}

/**
 * Bildirim paneli (salon-paneli/BildirimPaneli): zilin altında açılan, modal olmayan iletişim kutusu. Esc veya dışarı
 * tıklama kapatır; satıra dokunmak bildirimi okundu sayar ve hedef sayfayı açar.
 */
export function NotificationPanel({ id, onClose }: NotificationPanelProps) {
  const api = useApi();
  const clock = useClock();
  const { can } = useSession();
  const navigate = useNavigate();
  const query = useQuery(notificationsQuery(api));
  const markRead = useMarkNotificationsRead();
  const ref = useRef<HTMLDivElement>(null);
  const [, setTick] = useState(0);

  useEffect(() => {
    ref.current?.focus();
    const timer = setInterval(() => {
      setTick((n) => n + 1);
    }, tickMs);
    return () => {
      clearInterval(timer);
    };
  }, []);

  useEffect(() => {
    // Dışarı tıklama kapatır; zil ve panel birlikte sayılır (zile tıklama paneli kendisi açar/kapatır).
    const onPointer = (event: PointerEvent) => {
      const area = ref.current?.parentElement;
      if (area && event.target instanceof Node && !area.contains(event.target)) {
        onClose(false);
      }
    };
    const panel = ref.current;
    const onKey = (event: KeyboardEvent) => {
      if (event.key === 'Escape') {
        event.stopPropagation();
        onClose(true);
      }
    };
    // Klavyeyle panelden (ve zilden) çıkınca kapanır.
    const onFocusOut = (event: FocusEvent) => {
      const area = panel?.parentElement;
      if (event.relatedTarget instanceof Node && area && !area.contains(event.relatedTarget)) {
        onClose(false);
      }
    };
    document.addEventListener('pointerdown', onPointer);
    panel?.addEventListener('keydown', onKey);
    panel?.addEventListener('focusout', onFocusOut);
    return () => {
      document.removeEventListener('pointerdown', onPointer);
      panel?.removeEventListener('keydown', onKey);
      panel?.removeEventListener('focusout', onFocusOut);
    };
  }, [onClose]);

  const open = (item: InAppNotification) => {
    if (item.readUtc === null) {
      markRead.mutate({ ids: [item.id] });
    }

    onClose(false);
    void navigate({ to: notificationRoute(item.target, can) });
  };

  const unread = query.data?.unreadCount ?? 0;
  return (
    <div
      ref={ref}
      id={id}
      role="dialog"
      aria-labelledby={`${id}-baslik`}
      tabIndex={-1}
      className={styles.panel}
    >
      <div className={styles.head}>
        <h2 id={`${id}-baslik`} className={styles.title}>
          Bildirimler
        </h2>
        {unread > 0 ? (
          <button
            type="button"
            className={styles.markAll}
            disabled={markRead.isPending}
            onClick={() => {
              markRead.mutate({ all: true });
            }}
          >
            Tümünü okundu say
          </button>
        ) : null}
      </div>
      {query.isPending ? (
        <div className={styles.loading} aria-busy="true" aria-label="Bildirimler yükleniyor">
          {[0, 1, 2].map((key) => (
            <Skeleton key={key} variant="block" height="var(--kontrol-lg)" />
          ))}
        </div>
      ) : query.isError ? (
        <EmptyState
          headingLevel={3}
          title="Bildirimler yüklenemedi"
          description={errorMessage(
            query.error instanceof ApiProblem ? query.error.code : undefined,
          )}
          actions={
            <Button size="sm" onClick={() => void query.refetch()}>
              Tekrar dene
            </Button>
          }
        />
      ) : query.data.items.length === 0 ? (
        <EmptyState
          headingLevel={3}
          icon={<BellOff size={20} />}
          title="Bildiriminiz yok"
          description="Yeni talep, iptal ve değerlendirmeler burada görünür."
        />
      ) : (
        <ul className={styles.list}>
          {query.data.items.map((item) => {
            const look = looks[item.event] ?? fallbackLook;
            const Icon = look.icon;
            const remaining =
              item.event === 'staff.approval_needed'
                ? holdRemaining(item.target.holdExpiresUtc, clock)
                : null;
            const unreadItem = item.readUtc === null;
            return (
              <li key={item.id}>
                <button
                  type="button"
                  className={styles.item}
                  data-unread={unreadItem || undefined}
                  onClick={() => {
                    open(item);
                  }}
                >
                  <span className={[styles.icon, styles[look.tone]].join(' ')} aria-hidden="true">
                    <Icon size={16} />
                  </span>
                  <span className={styles.text}>
                    <span className={styles.itemTitle}>
                      {item.title}
                      {unreadItem ? <span className={styles.srOnly}> (okunmadı)</span> : null}
                    </span>
                    <span className={styles.body}>
                      {remaining ? `${item.body} · ${remaining}` : item.body}
                    </span>
                  </span>
                  <span className={styles.time}>
                    {notificationTime(item.createdUtc, clock, defaultTimeZone)}
                  </span>
                </button>
              </li>
            );
          })}
        </ul>
      )}
    </div>
  );
}
