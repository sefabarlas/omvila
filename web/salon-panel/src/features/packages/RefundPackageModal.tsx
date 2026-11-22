import { ApiProblem, idempotencyKey } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { formatMoney, moneyInputValue, parseMoneyInput } from '@omvila/ui-web/format';
import { Banner, Button, Modal, TextField, useToast } from '@omvila/ui-web/panel';
import { useState } from 'react';
import { ConflictBanner } from '@/shared/ConflictBanner';
import { useRefundCustomerPackage } from './api';
import type { CustomerPackage } from './api';
import styles from './Packages.module.css';

/**
 * Müşteri paketini iade et (tasarımı yok; RefundDepositModal deseni). Tutarı salon elle girer (S-10, W9): varsayılan
 * ödenen tutar, kısmi olabilir, ödeneni aşamaz. OMVILA para göndermez, yalnız kaydeder. Kalan seanslar kullanılamaz.
 */
export function RefundPackageModal({
  customerPackage,
  onClose,
}: {
  customerPackage: CustomerPackage;
  onClose: () => void;
}) {
  const toast = useToast();
  const refund = useRefundCustomerPackage();
  const paid = customerPackage.price;
  const [amount, setAmount] = useState(() => moneyInputValue(paid.amountMinor));
  const [note, setNote] = useState('');
  const [error, setError] = useState<string | undefined>();
  const [problem, setProblem] = useState<string | null>(null);
  const [conflict, setConflict] = useState(false);
  const [key] = useState(() => idempotencyKey()['Idempotency-Key']);
  const rangeError = `Tutar 0'dan büyük ve en fazla ${formatMoney(paid)} olmalı.`;

  const submit = () => {
    setProblem(null);
    const minor = parseMoneyInput(amount);
    if (minor === null || minor <= 0 || minor > paid.amountMinor) {
      setError(rangeError);
      return;
    }

    setError(undefined);
    refund.mutate(
      {
        customerPackage,
        key,
        body: {
          amount: { amountMinor: minor, currency: paid.currency },
          ...(note.trim() !== '' ? { note: note.trim() } : {}),
        },
      },
      {
        onSuccess: () => {
          toast.show({
            tone: 'basari',
            title: 'Paket iadesi işaretlendi',
            message: `${customerPackage.customer.fullName} · ${customerPackage.packageName}`,
          });
          onClose();
        },
        onError: (problemError) => {
          if (problemError instanceof ApiProblem && problemError.status === 412) {
            setConflict(true);
            return;
          }

          if (problemError instanceof ApiProblem && problemError.errors.amount) {
            setError(rangeError);
            return;
          }

          setProblem(
            errorMessage(problemError instanceof ApiProblem ? problemError.code : undefined),
          );
        },
      },
    );
  };

  return (
    <Modal
      open
      onClose={onClose}
      title="Paketi iade et"
      description={`${customerPackage.customer.fullName} · ${customerPackage.packageName} · ödenen ${formatMoney(paid)}`}
      footer={
        <>
          <Button onClick={onClose}>Vazgeç</Button>
          <Button
            variant="primary"
            loading={refund.isPending}
            loadingLabel="İşaretleniyor…"
            onClick={submit}
          >
            İadeyi kaydet
          </Button>
        </>
      }
    >
      <div className={styles.form}>
        {conflict ? <ConflictBanner onReload={onClose} /> : null}
        {problem ? <Banner tone="hata" title={problem} /> : null}
        <TextField
          label="İade edilen tutar"
          inputMode="decimal"
          suffix="₺"
          value={amount}
          error={error}
          hint="Tutarı siz belirlersiniz; kısmi iade için değiştirin."
          onChange={(event) => {
            setAmount(event.target.value);
          }}
        />
        <TextField
          label="Not (isteğe bağlı)"
          maxLength={300}
          value={note}
          hint="Yalnız panelde görünür."
          onChange={(event) => {
            setNote(event.target.value);
          }}
        />
        <p className={styles.hint}>
          OMVILA para göndermez. İadeyi kendiniz yaptıktan sonra kaydedin. Kalan{' '}
          {customerPackage.remainingSessions} seans kullanılamaz.
        </p>
      </div>
    </Modal>
  );
}
