import { ApiProblem } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { Button, Skeleton, useToast } from '@omvila/ui-web/panel';
import { useQuery } from '@tanstack/react-query';
import { CalendarDays } from 'lucide-react';
import type { ReactNode } from 'react';
import { useApi } from '@/app/api-context';
import { useSession } from '@/app/session';
import { goToExternal } from '@/shared/external-navigation';
import { genitive } from '@/shared/turkish';
import type { StaffDetail } from './api';
import {
  googleCalendarQuery,
  useConnectGoogleCalendar,
  useDisconnectGoogleCalendar,
} from './google-calendar-api';
import styles from './Staff.module.css';

const oneWay = "randevular tek yönlü aktarılır, Google'daki değişiklikler buraya gelmez";

function Card({
  title,
  text,
  warning = false,
  action,
}: {
  title: string;
  text: string;
  warning?: boolean;
  action?: ReactNode;
}) {
  return (
    <section
      className={warning ? [styles.google, styles.googleWarning].join(' ') : styles.google}
      aria-label="Google Takvim"
    >
      <span className={styles.googleIcon} aria-hidden="true">
        <CalendarDays size={20} />
      </span>
      <div className={styles.googleText}>
        <strong>{title}</strong>
        <span className={styles.note}>{text}</span>
      </div>
      {action}
    </section>
  );
}

/**
 * Personel → "Google Takvim · bağlı" kartı (salon-paneli/Personel; Durumlar "Google Takvim bağlantısı koptu"; K7).
 * Bağlan ve bağlantıyı kes yalnız oturumdaki kullanıcının kendi personel kaydında (uçlar kendi kaydına bağlı); diğer
 * personelde durum StaffDetail.googleCalendarStatus'tan salt okunur, bağlantı yoksa kart görünmez (T-127).
 */
export function GoogleCalendarCard({ staff }: { staff: StaffDetail }) {
  const { session } = useSession();
  return staff.id === session.membership.staffId ? (
    <OwnGoogleCalendar />
  ) : (
    <OtherGoogleCalendar staff={staff} />
  );
}

function OtherGoogleCalendar({ staff }: { staff: StaffDetail }) {
  const first = staff.fullName.split(' ')[0] ?? staff.fullName;
  switch (staff.googleCalendarStatus) {
    case 'Active':
      return <Card title="Google Takvim · bağlı" text={`Bu kişinin ${oneWay}.`} />;
    case 'TokenExpired':
      return (
        <Card
          warning
          title={`${genitive(staff.fullName)} Google Takvim bağlantısının süresi doldu.`}
          text={`Yeni randevular Google'a aktarılmıyor. ${first} yeniden bağlamalı.`}
        />
      );
    default:
      return null;
  }
}

function OwnGoogleCalendar() {
  const api = useApi();
  const toast = useToast();
  const query = useQuery(googleCalendarQuery(api));
  const connect = useConnectGoogleCalendar(goToExternal);
  const disconnect = useDisconnectGoogleCalendar();
  const fail = (error: unknown) => {
    toast.show({
      tone: 'hata',
      title: errorMessage(error instanceof ApiProblem ? error.code : undefined),
    });
  };
  const connectButton = (label: string) => (
    <Button
      size="sm"
      loading={connect.isPending}
      onClick={() => {
        connect.mutate(undefined, { onError: fail });
      }}
    >
      {label}
    </Button>
  );

  if (query.isPending) {
    return <Skeleton variant="block" height="96px" />;
  }

  if (query.isError) {
    // Personel kaydı yoksa (404) kart yok; diğer hatalarda yeniden deneme.
    return query.error instanceof ApiProblem && query.error.status === 404 ? null : (
      <Card
        title="Google Takvim durumu yüklenemedi"
        text={errorMessage(query.error instanceof ApiProblem ? query.error.code : undefined)}
        action={
          <Button size="sm" onClick={() => void query.refetch()}>
            Tekrar dene
          </Button>
        }
      />
    );
  }

  const connection = query.data;
  switch (connection.status) {
    case 'Active':
      return (
        <Card
          title="Google Takvim · bağlı"
          text={`${connection.accountEmailMasked ? `${connection.accountEmailMasked} · ` : ''}${oneWay}`}
          action={
            <Button
              size="sm"
              loading={disconnect.isPending}
              onClick={() => {
                disconnect.mutate(undefined, {
                  onSuccess: () => {
                    toast.show({ title: 'Google Takvim bağlantısı kesildi' });
                  },
                  onError: fail,
                });
              }}
            >
              Bağlantıyı kes
            </Button>
          }
        />
      );
    case 'TokenExpired':
      return (
        <Card
          warning
          title="Google Takvim bağlantınızın süresi doldu."
          text="Yeni randevular Google'a aktarılmıyor. Yeniden bağlayın."
          action={connectButton('Yeniden bağlan')}
        />
      );
    default:
      return (
        <Card
          title="Google Takvim · bağlı değil"
          text={`Bağlarsanız ${oneWay}.`}
          action={connectButton('Bağlan')}
        />
      );
  }
}
