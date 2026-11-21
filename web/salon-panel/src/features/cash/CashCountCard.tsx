import { ApiProblem, idempotencyKey } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { formatMoney, formatTime, parseMoneyInput } from '@omvila/ui-web/format';
import { TextField, useToast } from '@omvila/ui-web/panel';
import { useQueryClient } from '@tanstack/react-query';
import { useEffect, useState } from 'react';
import type { SyntheticEvent } from 'react';
import { queryKeys } from '@/app/query-keys';
import { useCloseCashDay } from './api';
import type { CashDay } from './api';
import styles from './Cash.module.css';

/** Üst bardaki "Günü kapat" bu formu gönderir. */
export const cashCountFormId = 'kasa-sayimi';
const countedInputId = 'kasa-sayilan-nakit';

/** Fark: "−50 ₺", "+20 ₺", "0 ₺" (Money eksi taşımaz; fark kuruş tamsayısıdır). */
export function differenceLabel(differenceMinor: number, currency: string): string {
  const amount = formatMoney({ amountMinor: Math.abs(differenceMinor), currency });
  return differenceMinor < 0 ? `−${amount}` : differenceMinor > 0 ? `+${amount}` : amount;
}

function differenceClass(differenceMinor: number): string | undefined {
  return differenceMinor < 0 ? styles.shortage : differenceMinor > 0 ? styles.surplus : undefined;
}

/**
 * "Kasa sayımı" (salon-paneli/Kasa): sayılan nakit, beklenen (sunucu: nakit tahsilat − nakit iade), fark ve fark
 * açıklaması. Fark varsa açıklama zorunludur (istemci denetler; sunucu 400 errors.differenceNote). Gün zaten
 * kapatıldıysa 409 cash_day_already_closed → kasa tazelenir. Kart başına bir Idempotency-Key; sunucu reddederse yenilenir.
 */
export function CashCountCard({ day, focusOnOpen }: { day: CashDay; focusOnOpen: boolean }) {
  const toast = useToast();
  const queryClient = useQueryClient();
  const close = useCloseCashDay();
  const [counted, setCounted] = useState('');
  const [note, setNote] = useState('');
  const [countError, setCountError] = useState<string | undefined>();
  const [noteError, setNoteError] = useState<string | undefined>();
  const [key, setKey] = useState(() => idempotencyKey()['Idempotency-Key']);
  const currency = day.expectedCash.currency;
  const countedMinor = parseMoneyInput(counted);
  const differenceMinor =
    countedMinor === null ? null : countedMinor - day.expectedCash.amountMinor;
  // Boş günde "Günü kapat" kartı açınca sayım alanına odaklanılır.
  useEffect(() => {
    if (focusOnOpen) {
      document.getElementById(countedInputId)?.focus();
    }
  }, [focusOnOpen]);

  const submit = (event: SyntheticEvent<HTMLFormElement>) => {
    event.preventDefault();
    if (countedMinor === null) {
      setCountError('Kasadaki nakdi girin, ör. 3.400.');
      return;
    }

    if (differenceMinor !== 0 && note.trim() === '') {
      setCountError(undefined);
      setNoteError('Fark var. Nedenini yazın.');
      return;
    }

    setCountError(undefined);
    setNoteError(undefined);
    close.mutate(
      {
        key,
        body: {
          branchId: day.branchId,
          date: day.date,
          countedCash: { amountMinor: countedMinor, currency },
          differenceNote: note.trim() === '' ? null : note.trim(),
        },
      },
      {
        onSuccess: () => {
          toast.show({ tone: 'basari', title: 'Gün kapatıldı' });
        },
        onError: (problem) => {
          if (problem instanceof ApiProblem) {
            setKey(idempotencyKey()['Idempotency-Key']);
          }

          if (problem instanceof ApiProblem && problem.errors.differenceNote) {
            setNoteError('Fark var. Nedenini yazın.');
            return;
          }

          if (problem instanceof ApiProblem && problem.code === 'cash_day_already_closed') {
            void queryClient.invalidateQueries({
              queryKey: queryKeys.cashDay(day.branchId, day.date),
            });
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
    <form
      id={cashCountFormId}
      className={styles.count}
      aria-labelledby="kasa-sayimi-baslik"
      noValidate
      onSubmit={submit}
    >
      <div>
        <h2 id="kasa-sayimi-baslik" className={styles.countTitle}>
          Kasa sayımı
        </h2>
        <p className={styles.countHint}>Günü kapatmadan önce kasadaki nakdi sayın.</p>
      </div>
      <TextField
        label="Sayılan nakit"
        inputMode="decimal"
        suffix="₺"
        id={countedInputId}
        value={counted}
        error={countError}
        disabled={close.isPending}
        onChange={(event) => {
          setCounted(event.target.value);
          setCountError(undefined);
        }}
      />
      <dl className={styles.countLines}>
        <div className={styles.countLine}>
          <dt>Beklenen</dt>
          <dd>{formatMoney(day.expectedCash)}</dd>
        </div>
        <div className={styles.countLine}>
          <dt>Fark</dt>
          <dd className={differenceMinor === null ? undefined : differenceClass(differenceMinor)}>
            {differenceMinor === null ? '—' : differenceLabel(differenceMinor, currency)}
          </dd>
        </div>
      </dl>
      <TextField
        label="Fark açıklaması"
        placeholder="Örn. bozuk para verildi"
        maxLength={300}
        value={note}
        error={noteError}
        disabled={close.isPending}
        onChange={(event) => {
          setNote(event.target.value);
          setNoteError(undefined);
        }}
      />
      {close.isPending ? <p className={styles.countHint}>Kaydediliyor…</p> : null}
    </form>
  );
}

/** Kapatılmış gün: sayım özeti (CashDayClose). */
export function ClosedDayCard({
  closing,
  timeZone,
}: {
  closing: NonNullable<CashDay['closing']>;
  timeZone: string;
}) {
  const currency = closing.expectedCash.currency;
  return (
    <section className={styles.count} aria-labelledby="gun-kapatildi-baslik">
      <div>
        <h2 id="gun-kapatildi-baslik" className={styles.countTitle}>
          Gün kapatıldı
        </h2>
        <p className={styles.countHint}>
          {[closing.closedByName, formatTime(closing.closedUtc, timeZone)]
            .filter(Boolean)
            .join(' · ')}
        </p>
      </div>
      <dl className={styles.countLines}>
        <div className={styles.countLine}>
          <dt>Sayılan nakit</dt>
          <dd>{formatMoney(closing.countedCash)}</dd>
        </div>
        <div className={styles.countLine}>
          <dt>Beklenen</dt>
          <dd>{formatMoney(closing.expectedCash)}</dd>
        </div>
        <div className={styles.countLine}>
          <dt>Fark</dt>
          <dd className={differenceClass(closing.differenceMinor)}>
            {differenceLabel(closing.differenceMinor, currency)}
          </dd>
        </div>
      </dl>
      {closing.differenceNote ? (
        <p className={styles.countHint}>{`Fark açıklaması: ${closing.differenceNote}`}</p>
      ) : null}
    </section>
  );
}
