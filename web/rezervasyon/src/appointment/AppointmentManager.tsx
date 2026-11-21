'use client';

import { BookingButton } from '@omvila/ui-web/booking';
import { systemClock } from '@omvila/ui-web/time';
import type { Clock } from '@omvila/ui-web/time';
import { WifiOff } from 'lucide-react';
import { Suspense, lazy, useEffect, useRef, useState } from 'react';
import { StateBanner } from '@/components/StateBanner';
import { problemCode } from '@/booking/problem';
import { actionParam, actionSearch, parseAction } from './action';
import type { Action } from './action';
import { fetchAppointment } from './appointment-api';
import type { Loaded } from './appointment-api';
import { AppointmentMissing } from './AppointmentMissing';
import { AppointmentView } from './AppointmentView';
import type { Notice } from './AppointmentView';
import { isActive } from './details';
import styles from './appointment.module.css';

// Randevum ilk boyamada gerekir; saat değiştir ve iptal ayrı parçalar, randevu gelince arka planda indirilir.
const RescheduleView = lazy(() =>
  import('./RescheduleView').then((m) => ({ default: m.RescheduleView })),
);
const CancelSheet = lazy(() => import('./CancelSheet').then((m) => ({ default: m.CancelSheet })));

function preload() {
  void import('./RescheduleView');
  void import('./CancelSheet');
}

type State =
  | { kind: 'loading' }
  | { kind: 'ready'; loaded: Loaded }
  | { kind: 'missing' }
  | { kind: 'failed' };

export interface AppointmentManagerProps {
  code: string;
  /** Sunucunun gördüğü `?islem=` (ilk çizim adresle aynı). */
  initialAction: Action;
  /** "Şimdi" (testte sabit saat). */
  clock?: Clock;
}

/** Mutasyon sonuçlarını ekranlara bildiren geri çağrılar (Ertele ve İptal aynı deseni kullanır). */
export interface Outcome {
  /** Başarılı: güncel randevu + yeni ETag. */
  done: (loaded: Loaded) => void;
  /** 409 cutoff_passed, 412 concurrency_conflict, 409 invalid_state_transition, 404: yeniden okunur. */
  failed: (error: unknown) => void;
}

/**
 * SMS bağlantısı (omvila.com/r/{kod}, T-59): Randevum, saat değiştir (Ertele) ve iptal. Randevu istemcide okunur;
 * ETag bellekte tutulur ve iptal / saat değişikliği `If-Match` ile gider (T-52 (4)).
 */
export function AppointmentManager({
  code,
  initialAction,
  clock = systemClock,
}: AppointmentManagerProps) {
  const [state, setState] = useState<State>({ kind: 'loading' });
  const [action, setAction] = useState<Action>(initialAction);
  const [notice, setNotice] = useState<Notice | null>(null);
  // Her okuma bir istek; `after` okuma bitince gösterilecek şerit (412, cutoff_passed sonrası).
  const [request, setRequest] = useState<{ attempt: number; after?: Notice }>({ attempt: 0 });
  const mainRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    let live = true;
    fetchAppointment(code).then(
      (loaded) => {
        if (!live) return;
        setState(loaded ? { kind: 'ready', loaded } : { kind: 'missing' });
        if (request.after) setNotice(request.after);
      },
      () => {
        if (live) setState({ kind: 'failed' });
      },
    );
    return () => {
      live = false;
    };
  }, [code, request]);

  const reload = (after?: Notice) => {
    setRequest((previous) => ({ attempt: previous.attempt + 1, ...(after ? { after } : {}) }));
  };

  useEffect(() => {
    const onPop = () => {
      setAction(parseAction(new URLSearchParams(window.location.search).get(actionParam)));
    };
    window.addEventListener('popstate', onPop);
    return () => {
      window.removeEventListener('popstate', onPop);
    };
  }, []);

  useEffect(() => {
    if (state.kind === 'missing') document.title = 'Bu randevu bulunamadı';
    if (state.kind === 'ready' && isActive(state.loaded.appointment)) preload();
  }, [state]);

  // Ekran değişince odak yeni ekrana (ekran okuyucu okur); ilk açılışta değil.
  const previous = useRef(action);
  useEffect(() => {
    if (previous.current === action) return;
    previous.current = action;
    if (action !== 'cancel') {
      mainRef.current?.focus();
    }
  }, [action]);

  const open = (next: Action) => {
    setNotice(null);
    setAction(next);
    window.history.pushState(
      { omvilaAction: next },
      '',
      `${window.location.pathname}${actionSearch(next)}`,
    );
  };
  const close = () => {
    setAction('view');
    const historyState: unknown = window.history.state;
    if (typeof historyState === 'object' && historyState !== null && 'omvilaAction' in historyState)
      window.history.back();
    else window.history.replaceState(null, '', window.location.pathname);
  };

  const outcome = (success: Notice): Outcome => ({
    done: (loaded) => {
      setState({ kind: 'ready', loaded });
      setNotice(success);
      close();
    },
    failed: (error) => {
      close();
      const code = problemCode(error);
      reload(
        code === 'cutoff_passed'
          ? { kind: 'cutoff' }
          : code === 'concurrency_conflict'
            ? { kind: 'conflict' }
            : { kind: 'error', error },
      );
    },
  });

  if (state.kind === 'missing') return <AppointmentMissing />;
  if (state.kind === 'failed') {
    return (
      <main className={styles.page}>
        <div className={styles.body}>
          <h1 className={styles.srOnly}>Randevunuz</h1>
          <StateBanner
            tone="hata"
            icon={<WifiOff size={20} />}
            title="Randevu yüklenemedi."
            description="Bağlantınızı kontrol edip tekrar deneyin."
          >
            <BookingButton
              variant="secondary"
              size="sm"
              onClick={() => {
                setState({ kind: 'loading' });
                reload();
              }}
            >
              Tekrar dene
            </BookingButton>
          </StateBanner>
        </div>
      </main>
    );
  }
  if (state.kind === 'loading') {
    return (
      <main className={styles.page} aria-busy="true">
        <h1 className={styles.srOnly}>Randevunuz yükleniyor</h1>
        <div className={styles.skeletonHeader} />
        <div className={styles.body}>
          <span className={styles.skeleton} />
          <span className={styles.skeleton} />
        </div>
      </main>
    );
  }

  const { appointment, etag } = state.loaded;
  const active = isActive(appointment);
  // Süre geçmişse veya randevu etkin değilse alt ekran açılmaz; Randevum "Salonu ara" gösterir (kriter 10).
  const shown: Action =
    action === 'reschedule' && active && appointment.canReschedule
      ? 'reschedule'
      : action === 'cancel' && active && appointment.canCancel
        ? 'cancel'
        : 'view';

  const skeleton = (
    <div className={styles.body} aria-busy="true">
      <span className={styles.skeleton} />
    </div>
  );

  return (
    <div ref={mainRef} className={styles.root} tabIndex={-1}>
      {shown === 'reschedule' ? (
        <Suspense fallback={skeleton}>
          <RescheduleView
            code={code}
            appointment={appointment}
            etag={etag}
            clock={clock}
            onBack={close}
            outcome={outcome({ kind: 'rescheduled' })}
          />
        </Suspense>
      ) : (
        <AppointmentView
          appointment={appointment}
          notice={notice}
          inert={shown === 'cancel'}
          onReschedule={() => {
            open('reschedule');
          }}
          onCancel={() => {
            open('cancel');
          }}
        />
      )}
      {shown === 'cancel' ? (
        <Suspense fallback={null}>
          <CancelSheet
            code={code}
            appointment={appointment}
            etag={etag}
            onClose={close}
            onReschedule={() => {
              setAction('reschedule');
              window.history.replaceState(
                { omvilaAction: 'reschedule' },
                '',
                `${window.location.pathname}${actionSearch('reschedule')}`,
              );
            }}
            outcome={outcome({ kind: 'cancelled' })}
          />
        </Suspense>
      ) : null}
    </div>
  );
}
