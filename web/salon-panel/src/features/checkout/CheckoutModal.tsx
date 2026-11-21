import { ApiProblem, idempotencyKey } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { formatMoney, formatTime, moneyInputValue } from '@omvila/ui-web/format';
import { Avatar, Button, Modal, TextField, useToast } from '@omvila/ui-web/panel';
import { displayStatus, statusLabels } from '@omvila/ui-web/status';
import { useQuery, useQueryClient } from '@tanstack/react-query';
import { Plus, X } from 'lucide-react';
import { useState } from 'react';
import type { ReactNode } from 'react';
import { useApi } from '@/app/api-context';
import { queryKeys } from '@/app/query-keys';
import { useSession } from '@/app/session';
import type { AppointmentDetail } from '@/features/calendar/api';
import { notificationSettingsQuery } from '@/features/settings/notification-api';
import { Segmented } from '@/shared/Segmented';
import { genitive } from '@/shared/turkish';
import { customerPackagesQuery, useCompleteAppointment } from './api';
import styles from './Checkout.module.css';
import {
  amountToCollect,
  checkRows,
  depositLine,
  firstName,
  itemAmount,
  maxPaymentRows,
  methodLabels,
  redemptionMatches,
  remainderFor,
} from './checkout';
import type { CollectedMethod, PaymentRow } from './checkout';

interface CheckoutModalProps {
  detail: AppointmentDetail;
  /** Şubede bugün (paketin son kullanım günü ve "bugün 10:30–12:00"). */
  today: string;
  /** Randevu gününün etiketi ("bugün"). */
  dayLabel: string;
  onClose: () => void;
  onCompleted: () => void;
  /** 412: yan panel "Kayıt çakışması" şeridini gösterir. */
  onConflict: () => void;
}

type MethodChoice = CollectedMethod | 'Package';

const collectedOptions: { value: MethodChoice; label: string }[] = (
  ['Cash', 'Card', 'Transfer'] as const
).map((value) => ({ value, label: methodLabels[value] }));
const ordinal = ['İkinci', 'Üçüncü'];

/**
 * "Tamamla ve ödeme al" (salon-paneli/OdemeAl, dilim-05 KK5): kalemler, salonda kalan kapora, alınacak tutar, yöntem
 * ve tutar; "İkinci ödeme ekle" ile en çok üç satır. "Kart" salonun kendi POS'udur: yalnız yöntem kaydı, kart formu yok
 * (OMVILA ödeme almaz). "Paketten" müşterinin kullanılabilir paket kalemi varsa görünür ve eşleşen kalemleri paketten
 * düşer; kalan tutar için ödeme satırı kalır. Pencere başına bir Idempotency-Key; sunucu reddederse yenilenir.
 */
export function CheckoutModal({
  detail,
  today,
  dayLabel,
  onClose,
  onCompleted,
  onConflict,
}: CheckoutModalProps) {
  const api = useApi();
  const toast = useToast();
  const queryClient = useQueryClient();
  const { can } = useSession();
  const complete = useCompleteAppointment();
  const customerId = detail.customer.brandCustomerId;
  const packages = useQuery({
    ...customerPackagesQuery(api, customerId),
    enabled: can('customer.view'),
  });
  // Değerlendirme daveti ayarı yalnız ayarları yöneten kullanıcıya okunur; bilinmiyorsa metinde anılmaz.
  const invite = useQuery({
    ...notificationSettingsQuery(api),
    enabled: can('settings.manage'),
  }).data?.reviewInviteEnabled;
  const matches = redemptionMatches(detail, packages.data?.items ?? [], today);
  const [usePackage, setUsePackage] = useState(false);
  const redemptions = usePackage ? matches : [];
  const due = amountToCollect(detail, redemptions);
  const [rows, setRows] = useState<PaymentRow[]>(() => [
    { method: 'Cash', amount: moneyInputValue(detail.amountDue.amountMinor) },
  ]);
  const [rowErrors, setRowErrors] = useState<(string | undefined)[]>([]);
  const [totalError, setTotalError] = useState<string | undefined>();
  const [key, setKey] = useState(() => idempotencyKey()['Idempotency-Key']);
  const deposit = depositLine(detail);
  const tz = detail.timeZone;
  const status = displayStatus(detail.displayStatus);
  const staffNames = [
    ...new Set(detail.items.map((item) => item.staffName).filter((n): n is string => !!n)),
  ];
  const showRows = due > 0;

  const resetErrors = () => {
    setRowErrors([]);
    setTotalError(undefined);
  };

  const updateRow = (index: number, patch: Partial<PaymentRow>) => {
    resetErrors();
    setRows((current) => current.map((row, i) => (i === index ? { ...row, ...patch } : row)));
  };

  const chooseFirst = (choice: MethodChoice) => {
    resetErrors();
    if (choice === 'Package') {
      const rest = amountToCollect(detail, matches);
      setUsePackage(true);
      setRows([{ method: 'Cash', amount: rest > 0 ? moneyInputValue(rest) : '' }]);
      return;
    }

    if (usePackage) {
      setUsePackage(false);
      setRows([{ method: choice, amount: moneyInputValue(detail.amountDue.amountMinor) }]);
      return;
    }

    updateRow(0, { method: choice });
  };

  const onProblem = (problem: unknown) => {
    // Sunucu isteği reddetti: düzeltilmiş gövde yeni anahtarla gider (aynı anahtar farklı gövdeyle reddedilir).
    if (problem instanceof ApiProblem) {
      setKey(idempotencyKey()['Idempotency-Key']);
    }

    if (problem instanceof ApiProblem && problem.status === 412) {
      onConflict();
      return;
    }

    if (problem instanceof ApiProblem && problem.code === 'payment_total_mismatch') {
      const expected = problem.meta.expectedAmountMinor;
      setTotalError(
        typeof expected === 'number'
          ? `Alınacak tutar ${formatMoney({ amountMinor: expected, currency: detail.total.currency })} olmalı. Tutarları kontrol edin.`
          : errorMessage(problem.code),
      );
      void queryClient.invalidateQueries({ queryKey: queryKeys.appointment(detail.id) });
      return;
    }

    if (problem instanceof ApiProblem && problem.code === 'package_session_unavailable') {
      setTotalError(errorMessage(problem.code));
      setUsePackage(false);
      void packages.refetch();
      return;
    }

    toast.show({
      tone: 'hata',
      title: errorMessage(problem instanceof ApiProblem ? problem.code : undefined),
    });
    if (problem instanceof ApiProblem && problem.code === 'invalid_state_transition') {
      void queryClient.invalidateQueries({ queryKey: queryKeys.appointment(detail.id) });
      onClose();
    }
  };

  const submit = () => {
    const check = checkRows(rows, due);
    if (!check.ok) {
      setRowErrors(check.rowErrors);
      setTotalError(check.totalError);
      return;
    }

    resetErrors();
    complete.mutate(
      {
        detail,
        key,
        body: {
          payments: check.amounts.map((amountMinor, i) => ({
            method: rows[i]?.method ?? 'Cash',
            amount: { amountMinor, currency: detail.total.currency },
          })),
          ...(redemptions.length > 0
            ? {
                packageRedemptions: redemptions.map(
                  ({ appointmentItemId, customerPackageItemId }) => ({
                    appointmentItemId,
                    customerPackageItemId,
                  }),
                ),
              }
            : {}),
        },
      },
      {
        onSuccess: () => {
          toast.show({
            tone: 'basari',
            title: 'Randevu tamamlandı',
            ...(due > 0 ? { message: 'Ödeme kasaya işlendi.' } : {}),
          });
          onCompleted();
        },
        onError: onProblem,
      },
    );
  };

  const firstOptions =
    matches.length > 0
      ? [...collectedOptions, { value: 'Package' as MethodChoice, label: 'Paketten' }]
      : collectedOptions;
  const commission =
    staffNames.length > 0
      ? `Tamamlandığında ${staffNames.map((n) => genitive(n)).join(' ve ')} primi hak edilir`
      : 'Tamamlandığında randevu kapanır';
  const info = `${commission}${invite ? ' ve müşteriye değerlendirme daveti gider' : ''}.`;

  return (
    <Modal
      open
      onClose={onClose}
      title="Tamamla ve ödeme al"
      footer={
        <>
          <Button onClick={onClose}>Vazgeç</Button>
          <Button
            variant="primary"
            loading={complete.isPending}
            loadingLabel="Kaydediliyor…"
            onClick={submit}
          >
            Tamamla ve kaydet
          </Button>
        </>
      }
    >
      <div className={styles.body}>
        <div className={styles.customer}>
          <Avatar name={detail.customer.fullName} size="sm" />
          <span>
            <strong>{detail.customer.fullName}</strong>
            {[
              '',
              `${dayLabel} ${formatTime(detail.startUtc, tz)}–${formatTime(detail.endUtc, tz)}`,
              ...(status ? [statusLabels[status]] : []),
            ].join(' · ')}
          </span>
        </div>
        <dl className={styles.lines} aria-label="Kalemler">
          {detail.items
            .filter((item) => item.status !== 'Cancelled')
            .map((item) => (
              <div key={item.id} className={styles.line}>
                <dt>{[item.serviceName, firstName(item.staffName)].filter(Boolean).join(' · ')}</dt>
                <dd>
                  {item.paidFromPackage
                    ? 'Paketten'
                    : formatMoney({ amountMinor: itemAmount(item), currency: item.price.currency })}
                </dd>
              </div>
            ))}
          {deposit ? (
            <div className={styles.line}>
              <dt>{deposit.label}</dt>
              <dd className={styles.minus}>
                {`−${formatMoney({ amountMinor: deposit.amountMinor, currency: detail.total.currency })}`}
              </dd>
            </div>
          ) : null}
          {redemptions.map((r) => (
            <div key={r.appointmentItemId} className={styles.line}>
              <dt>{`Paketten (${r.packageName} · ${r.serviceName})`}</dt>
              <dd className={styles.minus}>
                {`−${formatMoney({ amountMinor: r.amountMinor, currency: detail.total.currency })}`}
              </dd>
            </div>
          ))}
          <div className={styles.totalLine}>
            <dt>Alınacak</dt>
            <dd>{formatMoney({ amountMinor: due, currency: detail.total.currency })}</dd>
          </div>
        </dl>
        {showRows || matches.length > 0 ? (
          <div className={styles.payments}>
            {usePackage ? (
              <PaymentBlock
                index={0}
                title="Ödeme yöntemi"
                options={firstOptions}
                value="Package"
                onChoose={chooseFirst}
              >
                <p className={styles.packageNote}>
                  {redemptions
                    .map(
                      (r) =>
                        `${r.serviceName}: ${r.packageName}, düşümden sonra ${String(r.remainingAfter)} seans kalır`,
                    )
                    .join(' · ')}
                </p>
              </PaymentBlock>
            ) : null}
            {showRows
              ? rows.map((row, index) => {
                  const position = usePackage ? index + 1 : index;
                  const last = index === rows.length - 1;
                  return (
                    <PaymentBlock
                      key={position}
                      index={position}
                      title={
                        position === 0
                          ? 'Ödeme yöntemi'
                          : usePackage && index === 0
                            ? 'Kalan tutarın yöntemi'
                            : `${ordinal[index - 1] ?? ''} ödeme`
                      }
                      options={position === 0 ? firstOptions : collectedOptions}
                      value={row.method}
                      onChoose={(choice) => {
                        if (position === 0) {
                          chooseFirst(choice);
                        } else if (choice !== 'Package') {
                          updateRow(index, { method: choice });
                        }
                      }}
                    >
                      <div className={styles.amountRow}>
                        <TextField
                          className={styles.amount}
                          label="Tutar"
                          inputMode="decimal"
                          suffix="₺"
                          value={row.amount}
                          error={rowErrors[index]}
                          onChange={(event) => {
                            updateRow(index, { amount: event.target.value });
                          }}
                        />
                        {index > 0 ? (
                          <Button
                            variant="ghost"
                            size="sm"
                            icon={<X size={16} aria-hidden="true" />}
                            onClick={() => {
                              resetErrors();
                              setRows((current) => current.filter((_, i) => i !== index));
                            }}
                          >
                            Kaldır
                          </Button>
                        ) : null}
                        {last && rows.length < maxPaymentRows ? (
                          <Button
                            variant="ghost"
                            size="sm"
                            icon={<Plus size={16} aria-hidden="true" />}
                            onClick={() => {
                              resetErrors();
                              setRows((current) => [
                                ...current,
                                { method: 'Cash', amount: remainderFor(current, due) },
                              ]);
                            }}
                          >
                            {`${ordinal[rows.length - 1] ?? ''} ödeme ekle`}
                          </Button>
                        ) : null}
                      </div>
                    </PaymentBlock>
                  );
                })
              : null}
          </div>
        ) : null}
        {totalError ? (
          <p className={styles.error} role="alert">
            {totalError}
          </p>
        ) : null}
        <p className={styles.info}>{info}</p>
      </div>
    </Modal>
  );
}

interface PaymentBlockProps {
  index: number;
  title: string;
  options: readonly { value: MethodChoice; label: string }[];
  value: MethodChoice;
  onChoose: (choice: MethodChoice) => void;
  children: ReactNode;
}

function PaymentBlock({ index, title, options, value, onChoose, children }: PaymentBlockProps) {
  return (
    <section className={styles.block} aria-label={title}>
      <h3 className={styles.blockTitle}>{title}</h3>
      <Segmented<MethodChoice>
        legend={title}
        name={`odeme-yontemi-${String(index)}`}
        options={options}
        value={value}
        onChange={onChoose}
      />
      {children}
    </section>
  );
}
