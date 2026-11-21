import { ApiProblem, idempotencyKey } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { formatMoney, moneyInputValue, parseMoneyInput } from '@omvila/ui-web/format';
import { Button, Modal, TextField, useToast } from '@omvila/ui-web/panel';
import { useState } from 'react';
import type { AppointmentDetail } from '@/features/calendar/api';
import { useMarkDepositRefunded } from './api';
import styles from './Deposits.module.css';

interface RefundDepositModalProps {
  detail: AppointmentDetail;
  onClose: () => void;
  /** 412: yan panel "Kayıt çakışması" şeridini gösterir. */
  onConflict: () => void;
}

/**
 * "Salon iade etti" onay penceresi (V7): iade salonun kendi bankasından yapılır, OMVILA yalnız kaydeder. Tutar elle
 * girilir (varsayılan alınan kaporanın tamamı, kısmi olabilir, alınanı aşamaz), not isteğe bağlı ve yalnız panelde
 * görünür. Pencere başına bir Idempotency-Key.
 */
export function RefundDepositModal({ detail, onClose, onConflict }: RefundDepositModalProps) {
  const toast = useToast();
  const refund = useMarkDepositRefunded();
  const received = detail.deposit?.required ?? { amountMinor: 0, currency: 'TRY' };
  const [amount, setAmount] = useState(() => moneyInputValue(received.amountMinor));
  const [note, setNote] = useState('');
  const [error, setError] = useState<string | undefined>();
  const [key] = useState(() => idempotencyKey()['Idempotency-Key']);
  const rangeError = `Tutar 0'dan büyük ve en fazla ${formatMoney(received)} olmalı.`;

  const submit = () => {
    const minor = parseMoneyInput(amount);
    if (minor === null || minor <= 0 || minor > received.amountMinor) {
      setError(rangeError);
      return;
    }

    setError(undefined);
    refund.mutate(
      {
        detail,
        input: {
          key,
          body: {
            amount: { amountMinor: minor, currency: received.currency },
            ...(note.trim() !== '' ? { note: note.trim() } : {}),
          },
        },
      },
      {
        onSuccess: () => {
          toast.show({
            tone: 'basari',
            title: 'Kapora iadesi işaretlendi',
            message: 'Müşteriye bildirim gitmez.',
          });
          onClose();
        },
        onError: (problem) => {
          if (problem instanceof ApiProblem && problem.status === 412) {
            onConflict();
            return;
          }

          if (problem instanceof ApiProblem && problem.errors.amount) {
            setError(rangeError);
            return;
          }

          toast.show({
            tone: 'hata',
            title: errorMessage(problem instanceof ApiProblem ? problem.code : undefined),
          });
        },
      },
    );
  };

  return (
    <Modal
      open
      onClose={onClose}
      title="Kapora iadesini işaretle"
      description={`${detail.customer.fullName} · alınan kapora ${formatMoney(received)}`}
      footer={
        <>
          <Button onClick={onClose}>Vazgeç</Button>
          <Button
            variant="primary"
            loading={refund.isPending}
            loadingLabel="İşaretleniyor…"
            onClick={submit}
          >
            Salon iade etti
          </Button>
        </>
      }
    >
      <div className={styles.form}>
        <TextField
          label="İade edilen tutar"
          inputMode="decimal"
          suffix="₺"
          value={amount}
          error={error}
          hint="Kısmi iade için tutarı değiştirin."
          onChange={(event) => {
            setAmount(event.target.value);
          }}
        />
        <TextField
          label="Not (isteğe bağlı)"
          maxLength={300}
          value={note}
          hint="Geçmişte görünür, müşteriye gösterilmez."
          onChange={(event) => {
            setNote(event.target.value);
          }}
        />
        <p className={styles.hint}>
          OMVILA para göndermez. İadeyi kendi bankanızdan yaptıktan sonra işaretleyin.
        </p>
      </div>
    </Modal>
  );
}
