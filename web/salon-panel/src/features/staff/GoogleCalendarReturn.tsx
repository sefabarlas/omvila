import { ApiProblem } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { Button, EmptyState, useToast } from '@omvila/ui-web/panel';
import { useNavigate } from '@tanstack/react-router';
import { CalendarClock, CalendarX } from 'lucide-react';
import { useEffect, useRef, useState } from 'react';
import { useSession } from '@/app/session';
import styles from '@/shared/StatusPage.module.css';
import { useCompleteGoogleCalendar } from './google-calendar-api';

/** Sağlayıcının geri dönüşündeki sorgu değerleri; izin reddedilirse code yerine error gelir. */
export interface GoogleCalendarReturnSearch {
  code?: string;
  state?: string;
}

/**
 * Google Takvim geri dönüş sayfası (/ayarlar/google-takvim; sunucunun Integrations:Calendar:RedirectUris:Web adresi).
 * Kendi ekranı yok (T-127): code ve state ile bağlantı tamamlanır, kullanıcı Personel sayfasına (personel yönetemeyen
 * rolde takvime) döner. Değer eksik veya geçersizse "Google Takvim bağlanamadı" boş durumu.
 */
export function GoogleCalendarReturn({ code, state }: GoogleCalendarReturnSearch) {
  const navigate = useNavigate();
  const toast = useToast();
  const { can } = useSession();
  const complete = useCompleteGoogleCalendar();
  const [failure, setFailure] = useState<string | null>(
    code && state
      ? null
      : 'Google izin vermedi veya bağlantı adresi eksik. Yeniden bağlanmayı deneyin.',
  );
  const started = useRef(false);
  const back = can('staff.manage')
    ? { to: '/personel' as const, label: 'Personele dön' }
    : { to: '/takvim' as const, label: 'Takvime dön' };

  useEffect(() => {
    if (started.current || !code || !state) {
      return;
    }

    started.current = true;
    complete.mutate(
      { code, state },
      {
        onSuccess: () => {
          toast.show({ tone: 'basari', title: 'Google Takvim bağlandı' });
          void navigate({ to: back.to, replace: true });
        },
        onError: (error) => {
          const invalid = error instanceof ApiProblem && error.code === 'validation_failed';
          setFailure(
            invalid
              ? 'Bağlantı adresinin süresi dolmuş veya geçersiz. Yeniden bağlanmayı deneyin.'
              : errorMessage(error instanceof ApiProblem ? error.code : undefined),
          );
        },
      },
    );
  }, [back.to, code, complete, navigate, state, toast]);

  return (
    <div className={styles.page}>
      <div className={styles.card}>
        {failure ? (
          <EmptyState
            icon={<CalendarX size={22} />}
            headingLevel={1}
            title="Google Takvim bağlanamadı"
            description={failure}
            actions={
              <Button onClick={() => void navigate({ to: back.to, replace: true })}>
                {back.label}
              </Button>
            }
          />
        ) : (
          <EmptyState
            icon={<CalendarClock size={22} />}
            headingLevel={1}
            title="Google Takvim bağlanıyor"
            description="Bağlantı tamamlanınca Personel sayfasına dönersiniz."
          />
        )}
      </div>
    </div>
  );
}
