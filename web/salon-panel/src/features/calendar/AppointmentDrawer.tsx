import { ApiProblem } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import {
  formatDate,
  formatDuration,
  formatMoney,
  formatPhone,
  formatTime,
} from '@omvila/ui-web/format';
import { Avatar, Button, Drawer, Skeleton, StatusBadge, useToast } from '@omvila/ui-web/panel';
import { displayStatus } from '@omvila/ui-web/status';
import { addDays, toBranchDate } from '@omvila/ui-web/time';
import { useQuery, useQueryClient } from '@tanstack/react-query';
import { useNavigate } from '@tanstack/react-router';
import { CalendarX2, Check, Clock, UserCheck } from 'lucide-react';
import { useState } from 'react';
import { useApi } from '@/app/api-context';
import { useNow } from '@/app/clock';
import { useSession } from '@/app/session';
import { channelLabels } from '@/features/appointments/labels';
import approvalStyles from '@/features/approvals/Approvals.module.css';
import { HoldNotice, PendingApprovalDetails } from '@/features/approvals/PendingApprovalDetails';
import { remainingMinutes } from '@/features/approvals/remaining';
import { ConflictBanner } from '@/shared/ConflictBanner';
import { locativeTime } from '@/shared/turkish';
import {
  appointmentQuery,
  useApproveAppointment,
  useCheckIn,
  useDeclineAppointment,
  useUndoNoShow,
} from './api';
import styles from './Calendar.module.css';
import { CancelAppointmentModal } from './CancelAppointmentModal';

interface AppointmentDrawerProps {
  appointmentId: string;
  /** Şubede bugün (Geldiyse geri al yalnız aynı gün). */
  today: string;
  onClose: () => void;
}

/** "Bugün", "Yarın" ya da "Salı, 25 Kasım". */
function dayLabel(date: string, today: string): string {
  if (date === today) {
    return 'Bugün';
  }

  return date === addDays(today, 1) ? 'Yarın' : formatDate(date, 'panel');
}

/**
 * Randevu yan paneli (salon-paneli/RandevuOnayli): durum kartı, müşteri, eylemler; altta "Geldi" veya gelmeyen için
 * "Geldiyse geri al". Durum eylemleri If-Match ile; 412'de "Kayıt çakışması" şeridi. "Tamamla ve ödeme al" (D5),
 * "Hizmet ekle" ve "Müşteriye SMS gönder" bu dilimde yok (acik-sorular.md T-54); "Müşteri kartı" kartı açar (T-57).
 * Onay bekleyen talepte (salon-paneli/RandevuPanel) kalan süre şeridi, ücretler, not, geçmiş ve altta Reddet / Onayla
 * (appointment.approve); 409 hold_expired → "Yanıt süresi doldu" şeridi, butonlar kalkar (acik-sorular.md T-67).
 */
export function AppointmentDrawer({ appointmentId, today, onClose }: AppointmentDrawerProps) {
  const api = useApi();
  const queryClient = useQueryClient();
  const toast = useToast();
  const { can } = useSession();
  const navigate = useNavigate();
  const query = useQuery(appointmentQuery(api, appointmentId));
  const checkIn = useCheckIn();
  const undo = useUndoNoShow();
  const approve = useApproveAppointment();
  const decline = useDeclineAppointment();
  const now = useNow();
  const [conflict, setConflict] = useState(false);
  const [holdExpired, setHoldExpired] = useState(false);
  const [cancelling, setCancelling] = useState(false);
  const detail = query.data;

  const onActionError = (error: unknown) => {
    if (error instanceof ApiProblem && error.status === 412) {
      setConflict(true);
      return;
    }

    if (error instanceof ApiProblem && error.code === 'hold_expired') {
      setHoldExpired(true);
      void queryClient.invalidateQueries({ queryKey: ['approval-queue'] });
      return;
    }

    if (error instanceof ApiProblem && error.code === 'invalid_state_transition') {
      void query.refetch();
    }

    toast.show({
      tone: 'hata',
      title: errorMessage(error instanceof ApiProblem ? error.code : undefined),
    });
  };

  const reload = () => {
    setConflict(false);
    void query.refetch();
    void queryClient.invalidateQueries({ queryKey: ['calendar'] });
  };

  if (!detail) {
    return (
      <Drawer open onClose={onClose} title="Randevu">
        {query.isError ? (
          <p className={styles.dialogText}>
            {errorMessage(query.error instanceof ApiProblem ? query.error.code : undefined)}
          </p>
        ) : (
          <Skeleton variant="block" height="160px" />
        )}
      </Drawer>
    );
  }

  const tz = detail.timeZone;
  const status = displayStatus(detail.displayStatus);
  const date = toBranchDate(new Date(detail.startUtc), tz);
  const active = detail.status === 'Confirmed' || detail.status === 'PendingApproval';
  const services = detail.items.map((item) => item.serviceName).join(' + ');
  const staffNames = [
    ...new Set(detail.items.map((item) => item.staffName).filter((n): n is string => !!n)),
  ];
  const customer = detail.customer;
  const phone = customer.phone ? formatPhone(customer.phone) : customer.phoneMasked;
  const visits = customer.isNew ? 'Yeni müşteri' : `${String(customer.visitCount)} ziyaret`;
  const showCheckIn = detail.status === 'Confirmed' && can('appointment.checkin');
  const showUndo = detail.status === 'NoShow' && date === today && can('appointment.checkin');
  const pending = detail.status === 'PendingApproval';
  const canApprove = can('appointment.approve');
  const minutesLeft = detail.holdExpiresUtc ? remainingMinutes(detail.holdExpiresUtc, now) : null;
  const showDecision =
    pending && canApprove && !holdExpired && (minutesLeft === null || minutesLeft > 0);
  const durationMinutes = Math.round(
    (Date.parse(detail.endUtc) - Date.parse(detail.startUtc)) / 60_000,
  );
  const title = [
    'Randevu',
    detail.reference ? `#${detail.reference}` : null,
    channelLabels[detail.channel],
  ]
    .filter(Boolean)
    .join(' · ');

  return (
    <>
      <Drawer
        open={!cancelling}
        onClose={onClose}
        title={title}
        {...(showDecision
          ? {
              footer: (
                <div className={approvalStyles.decision}>
                  <Button
                    variant="danger"
                    loading={decline.isPending}
                    loadingLabel="İptal ediliyor…"
                    disabled={approve.isPending}
                    onClick={() => {
                      decline.mutate(
                        { detail, input: undefined },
                        {
                          onSuccess: () => {
                            toast.show({
                              tone: 'basari',
                              title: 'Talep iptal edildi',
                              message: 'Müşteriye iptal bildirimi gider.',
                            });
                          },
                          onError: onActionError,
                        },
                      );
                    }}
                  >
                    Reddet
                  </Button>
                  <Button
                    variant="primary"
                    icon={<Check size={16} aria-hidden="true" />}
                    loading={approve.isPending}
                    loadingLabel="Onaylanıyor…"
                    disabled={decline.isPending}
                    onClick={() => {
                      approve.mutate(
                        { detail, input: undefined },
                        {
                          onSuccess: () => {
                            toast.show({
                              tone: 'basari',
                              title: 'Randevu onaylandı',
                              message: 'Müşteriye onay bildirimi gider.',
                            });
                          },
                          onError: onActionError,
                        },
                      );
                    }}
                  >
                    Onayla
                  </Button>
                </div>
              ),
            }
          : showCheckIn || showUndo
            ? {
                footer: showCheckIn ? (
                  <Button
                    variant="primary"
                    icon={<UserCheck size={16} aria-hidden="true" />}
                    loading={checkIn.isPending}
                    loadingLabel="İşaretleniyor…"
                    onClick={() => {
                      checkIn.mutate(
                        { detail, input: undefined },
                        {
                          onSuccess: () => {
                            toast.show({ tone: 'basari', title: `${customer.fullName} geldi` });
                          },
                          onError: onActionError,
                        },
                      );
                    }}
                  >
                    Geldi
                  </Button>
                ) : (
                  <Button
                    variant="primary"
                    loading={undo.isPending}
                    loadingLabel="Geri alınıyor…"
                    onClick={() => {
                      undo.mutate(
                        { detail, input: undefined },
                        {
                          onSuccess: () => {
                            toast.show({ tone: 'basari', title: 'Gelmedi geri alındı' });
                          },
                          onError: onActionError,
                        },
                      );
                    }}
                  >
                    Geldiyse geri al
                  </Button>
                ),
              }
            : {})}
      >
        <div className={styles.drawerBody}>
          {conflict ? <ConflictBanner onReload={reload} /> : null}
          <section className={styles.statusCard} aria-label="Randevu durumu">
            <div className={styles.statusTop}>
              <span className={styles.statusEyebrow}>
                {[
                  dayLabel(date, today),
                  pending && detail.channel === 'Online' ? 'Online randevu' : null,
                  detail.isRecurring ? 'Tekrarlayan' : null,
                ]
                  .filter(Boolean)
                  .join(' · ')}
              </span>
              {status ? <StatusBadge status={status} /> : null}
            </div>
            <strong className={styles.statusTime}>
              {formatTime(detail.startUtc, tz)}–{formatTime(detail.endUtc, tz)}
            </strong>
            <span className={styles.statusLine}>
              {[
                services,
                ...staffNames,
                pending ? formatDuration(durationMinutes) : formatMoney(detail.total),
              ].join(' · ')}
            </span>
          </section>
          {pending && (minutesLeft !== null || !canApprove || holdExpired) ? (
            <HoldNotice
              minutesLeft={minutesLeft ?? 0}
              canApprove={canApprove}
              expired={holdExpired}
            />
          ) : null}
          <section className={styles.customerCard} aria-label="Müşteri">
            <Avatar name={customer.fullName} />
            <span className={styles.customerText}>
              <strong>{customer.fullName}</strong>
              <small>{[phone, visits].filter(Boolean).join(' · ')}</small>
            </span>
            {can('customer.view') ? (
              <Button
                size="sm"
                className={styles.customerLink}
                onClick={() => {
                  onClose();
                  void navigate({
                    to: '/musteriler/$id',
                    params: { id: customer.brandCustomerId },
                  });
                }}
              >
                Müşteri kartı
              </Button>
            ) : null}
          </section>
          {pending ? (
            <PendingApprovalDetails detail={detail} />
          ) : (
            <ul className={styles.actions}>
              {active && can('appointment.update') ? (
                <li className={styles.action}>
                  <span className={styles.actionIcon} aria-hidden="true">
                    <Clock size={16} />
                  </span>
                  <span className={styles.actionText}>
                    <strong>Saati veya personeli değiştir</strong>
                    <small>Takvimde sürükleyerek taşıyabilirsiniz</small>
                  </span>
                </li>
              ) : null}
              {active && can('appointment.cancel') ? (
                <li>
                  <button
                    type="button"
                    className={styles.action}
                    onClick={() => {
                      setCancelling(true);
                    }}
                  >
                    <span className={styles.actionIconDanger} aria-hidden="true">
                      <CalendarX2 size={16} />
                    </span>
                    <span className={styles.actionText}>
                      <strong className={styles.danger}>Randevuyu iptal et</strong>
                      <small>
                        {detail.isRecurring
                          ? 'Tekrarlayan: yalnızca bu veya sonrakiler'
                          : 'Müşteriye bildirmek isteğe bağlı'}
                      </small>
                    </span>
                  </button>
                </li>
              ) : null}
            </ul>
          )}
          {detail.status === 'Confirmed' && detail.noShowAtUtc ? (
            <p className={styles.note}>
              {`Gelmezse ${locativeTime(formatTime(detail.noShowAtUtc, tz))} otomatik "Gelmedi" olur.`}
            </p>
          ) : null}
          {detail.cancellation ? (
            <p className={styles.note}>
              {`${formatDate(toBranchDate(new Date(detail.cancellation.cancelledUtc), tz), 'dayMonth')} tarihinde iptal edildi.`}
            </p>
          ) : null}
        </div>
      </Drawer>
      {/* İptal penceresi açıkken yan panel kapanır (TekrarIptal); vazgeçilince geri açılır. */}
      {cancelling ? (
        <CancelAppointmentModal
          detail={detail}
          onClose={() => {
            setCancelling(false);
          }}
          onCancelled={() => {
            setCancelling(false);
            onClose();
          }}
          onConflict={() => {
            setCancelling(false);
            setConflict(true);
          }}
        />
      ) : null}
    </>
  );
}
