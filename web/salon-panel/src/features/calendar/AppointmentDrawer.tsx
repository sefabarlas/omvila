import { ApiProblem, idempotencyKey } from '@omvila/api-client-ts';
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
import { Banknote, CalendarX2, Check, Clock, UserCheck } from 'lucide-react';
import { useState } from 'react';
import { useApi } from '@/app/api-context';
import { useNow } from '@/app/clock';
import { useSession } from '@/app/session';
import { channelLabels } from '@/features/appointments/labels';
import approvalStyles from '@/features/approvals/Approvals.module.css';
import {
  AppointmentHistory,
  HoldNotice,
  PendingApprovalDetails,
} from '@/features/approvals/PendingApprovalDetails';
import { remainingMinutes } from '@/features/approvals/remaining';
import { useMarkDepositReceived, useRemindDeposit } from '@/features/deposits/api';
import { DepositSummary, PendingDepositSection } from '@/features/deposits/DepositSection';
import { isDepositAwaited } from '@/features/deposits/deposit-view';
import { RefundDepositModal } from '@/features/deposits/RefundDepositModal';
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
 * Kapora bekleyen talepte (dilim-04 KK5, KK7) kapora bölümü, "Hatırlat", "Kaporasız onayla" ve alt buton "Kapora geldi"
 * (appointment.deposit.mark); alınmış kaporada "Salon iade etti" (acik-sorular.md T-84).
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
  const markReceived = useMarkDepositReceived();
  const remind = useRemindDeposit();
  const now = useNow();
  const [conflict, setConflict] = useState(false);
  const [holdExpired, setHoldExpired] = useState(false);
  const [cancelling, setCancelling] = useState(false);
  const [refunding, setRefunding] = useState(false);
  // "Kapora geldi" Payment yazar: yan panel açıkken aynı eylemin tekrarı aynı anahtarla gider.
  const [receiveKey] = useState(() => idempotencyKey()['Idempotency-Key']);
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

    // Durum veya kapora sunucuda değişmiş: güncel ayrıntı doğru butonları gösterir (ör. hatırlatma zaten gitmiş).
    if (
      error instanceof ApiProblem &&
      ['invalid_state_transition', 'deposit_required', 'deposit_reminder_already_sent'].includes(
        error.code,
      )
    ) {
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
  // Kapora işaretleme (V5, V7): Owner ve BranchManager; personel görmez.
  const canMark = can('appointment.deposit.mark');
  const minutesLeft = detail.holdExpiresUtc ? remainingMinutes(detail.holdExpiresUtc, now) : null;
  // Kapora bekleyen randevuda düz "Onayla" yok: "Kapora geldi" (alt buton) veya "Kaporasız onayla" (kapora bölümü).
  const depositAwaited = pending && isDepositAwaited(detail.deposit?.status);
  const canDecide = canApprove || (depositAwaited && canMark);
  const showDecision =
    pending && canDecide && !holdExpired && (minutesLeft === null || minutesLeft > 0);
  const showHold =
    pending &&
    (depositAwaited
      ? holdExpired || minutesLeft === 0 || !canDecide
      : minutesLeft !== null || !canApprove || holdExpired);
  const busy = approve.isPending || decline.isPending || markReceived.isPending || remind.isPending;
  const decisionButtons = [
    canApprove ? 'decline' : null,
    depositAwaited ? (canMark ? 'received' : null) : 'approve',
  ].filter(Boolean);
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
        open={!cancelling && !refunding}
        onClose={onClose}
        title={title}
        {...(showDecision
          ? {
              footer: (
                <div
                  className={
                    decisionButtons.length > 1 ? approvalStyles.decision : approvalStyles.single
                  }
                >
                  {canApprove ? (
                    <Button
                      variant="danger"
                      loading={decline.isPending}
                      loadingLabel="İptal ediliyor…"
                      disabled={busy && !decline.isPending}
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
                  ) : null}
                  {depositAwaited ? (
                    canMark ? (
                      <Button
                        variant="primary"
                        icon={<Banknote size={16} aria-hidden="true" />}
                        loading={markReceived.isPending}
                        loadingLabel="İşaretleniyor…"
                        disabled={busy && !markReceived.isPending}
                        onClick={() => {
                          markReceived.mutate(
                            { detail, input: { key: receiveKey } },
                            {
                              onSuccess: () => {
                                toast.show({
                                  tone: 'basari',
                                  title: 'Kapora geldi olarak işaretlendi',
                                  message: 'Randevu onaylandı, müşteriye SMS gider.',
                                });
                              },
                              onError: onActionError,
                            },
                          );
                        }}
                      >
                        Kapora geldi
                      </Button>
                    ) : null
                  ) : (
                    <Button
                      variant="primary"
                      icon={<Check size={16} aria-hidden="true" />}
                      loading={approve.isPending}
                      loadingLabel="Onaylanıyor…"
                      disabled={busy && !approve.isPending}
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
                  )}
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
          {showHold ? (
            <HoldNotice
              minutesLeft={minutesLeft ?? 0}
              canApprove={canDecide}
              expired={holdExpired}
              deposit={depositAwaited}
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
          {depositAwaited ? (
            <PendingDepositSection
              detail={detail}
              minutesLeft={minutesLeft}
              showActions={showDecision}
              canMark={canMark}
              canApprove={canApprove}
              busy={busy}
              reminding={remind.isPending}
              waiving={approve.isPending}
              onRemind={() => {
                remind.mutate(detail.id, {
                  onSuccess: () => {
                    toast.show({
                      tone: 'basari',
                      title: 'Hatırlatma gönderildi',
                      message: 'Kapora SMS’i müşteriye yeniden gider.',
                    });
                  },
                  onError: onActionError,
                });
              }}
              onWaive={() => {
                approve.mutate(
                  { detail, input: { waiveDeposit: true } },
                  {
                    onSuccess: () => {
                      toast.show({
                        tone: 'basari',
                        title: 'Randevu kaporasız onaylandı',
                        message: 'Müşteriye onay bildirimi gider.',
                      });
                    },
                    onError: onActionError,
                  },
                );
              }}
            />
          ) : null}
          {pending ? (
            <PendingApprovalDetails detail={detail} />
          ) : (
            <ul className={styles.actions}>
              <DepositSummary
                detail={detail}
                canMark={canMark}
                onRefund={() => {
                  setRefunding(true);
                }}
              />
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
          {/* Kaporalı randevuda geçmiş (DepositReceived, DepositWaived, DepositRefunded) onaydan sonra da görünür. */}
          {!pending && detail.deposit ? <AppointmentHistory detail={detail} /> : null}
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
      {/* İade penceresi açıkken yan panel kapanır; kapanınca güncel ayrıntıyla geri açılır. */}
      {refunding ? (
        <RefundDepositModal
          detail={detail}
          onClose={() => {
            setRefunding(false);
          }}
          onConflict={() => {
            setRefunding(false);
            setConflict(true);
          }}
        />
      ) : null}
    </>
  );
}
