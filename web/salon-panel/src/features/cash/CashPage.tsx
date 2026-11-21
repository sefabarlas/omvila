import { ApiProblem } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { formatDate, formatMoney, formatTime, moneyInputValue } from '@omvila/ui-web/format';
import {
  Badge,
  Bento,
  BentoBlock,
  Button,
  DataTable,
  EmptyState,
  Skeleton,
} from '@omvila/ui-web/panel';
import { branchToday } from '@omvila/ui-web/time';
import { useQuery } from '@tanstack/react-query';
import { useNavigate } from '@tanstack/react-router';
import { CalendarDays, History, Lock, Wallet } from 'lucide-react';
import { useState } from 'react';
import { useApi } from '@/app/api-context';
import { useSelectedBranch } from '@/app/branch';
import { useClock } from '@/app/clock';
import { defaultTimeZone } from '@/app/config';
import { useSession } from '@/app/session';
import type { PaymentView } from '@/features/checkout/api';
import { firstName, methodLabels } from '@/features/checkout/checkout';
import { Page } from '@/shared/Page';
import { cashDayQuery } from './api';
import type { CashDay } from './api';
import styles from './Cash.module.css';
import { cashCountFormId, CashCountCard, ClosedDayCard } from './CashCountCard';
import { methodTone, paymentItemLabel } from './cash-view';
import { PreviousDaysModal } from './PreviousDaysModal';

export interface CashSearch {
  /** "Önceki günler": şubenin yerel günü (yyyy-aa-gg); yoksa bugün. */
  tarih?: string;
}

/**
 * Kasa (salon-paneli/Kasa, KasaBos; dilim-05 KK7): günün tahsilatları, yöntem blokları, tablo ve "Kasa sayımı". "Günü
 * kapat" sayımı gönderir (cash.close); fark varsa açıklama zorunlu. Boş günde sayım kartı "Günü kapat" ile açılır.
 * Kapalı günde sayım özeti görünür. "Önceki günler" tarih seçer (?tarih=…). OMVILA para almaz; "Kart" salonun POS'udur.
 */
export function CashPage({ search }: { search: CashSearch }) {
  const api = useApi();
  const clock = useClock();
  const navigate = useNavigate();
  const { session, can } = useSession();
  const { branchId } = useSelectedBranch();
  const activeBranch = branchId ?? session.branches[0]?.id ?? '';
  const branchName =
    session.branches.find((b) => b.id === activeBranch)?.name ?? session.branches[0]?.name ?? '';
  const today = branchToday(clock, defaultTimeZone);
  const date = search.tarih && search.tarih <= today ? search.tarih : today;
  const isToday = date === today;
  const query = useQuery({
    ...cashDayQuery(api, activeBranch, date),
    enabled: activeBranch !== '',
  });
  const day = query.data;
  const [counting, setCounting] = useState(false);
  const [picking, setPicking] = useState(false);
  const canClose = can('cash.close') && day?.status === 'Open';
  const empty = day?.payments.length === 0;
  const showCount = canClose && (!empty || counting);

  const openDay = (next: string) => {
    setCounting(false);
    void navigate({ to: '/kasa', search: next === today ? {} : { tarih: next } });
  };

  const context = [
    formatDate(date, 'panel'),
    branchName,
    day ? (day.status === 'Open' ? 'gün açık' : 'gün kapatıldı') : null,
  ]
    .filter(Boolean)
    .join(' · ');

  return (
    <Page
      title="Kasa"
      context={context}
      action={
        <div className={styles.actions}>
          {isToday ? (
            <Button
              icon={<History size={16} aria-hidden="true" />}
              onClick={() => {
                setPicking(true);
              }}
            >
              Önceki günler
            </Button>
          ) : (
            <Button
              icon={<CalendarDays size={16} aria-hidden="true" />}
              onClick={() => {
                openDay(today);
              }}
            >
              Bugüne dön
            </Button>
          )}
          {canClose ? (
            showCount ? (
              <Button
                variant="primary"
                type="submit"
                form={cashCountFormId}
                icon={<Lock size={16} aria-hidden="true" />}
              >
                Günü kapat
              </Button>
            ) : (
              <Button
                variant="primary"
                icon={<Lock size={16} aria-hidden="true" />}
                onClick={() => {
                  setCounting(true);
                }}
              >
                Günü kapat
              </Button>
            )
          ) : null}
        </div>
      }
    >
      {query.isPending ? (
        <>
          <Skeleton variant="block" height="120px" />
          <Skeleton variant="block" height="320px" />
        </>
      ) : query.isError ? (
        <section className={styles.card} aria-label="Hata">
          <EmptyState
            headingLevel={2}
            title="Kasa yüklenemedi"
            description={errorMessage(
              query.error instanceof ApiProblem ? query.error.code : undefined,
            )}
            actions={<Button onClick={() => void query.refetch()}>Tekrar dene</Button>}
          />
        </section>
      ) : (
        <>
          <CashTotals day={query.data} isToday={isToday} />
          <div className={showCount || query.data.closing ? styles.grid : undefined}>
            {empty ? (
              <section className={styles.card} aria-label="Tahsilat yok">
                <EmptyState
                  icon={<Wallet size={24} />}
                  headingLevel={2}
                  title={isToday ? 'Bugün henüz tahsilat yok' : 'Bu gün tahsilat yok'}
                  description="Randevuyu tamamlayıp ödeme aldığınızda hareketler burada listelenir. Gün sonunda kasayı kapatırsınız."
                  actions={
                    <Button
                      icon={<CalendarDays size={16} aria-hidden="true" />}
                      onClick={() => {
                        void navigate({
                          to: '/takvim',
                          search: isToday ? {} : { tarih: date },
                        });
                      }}
                    >
                      {isToday ? 'Bugünkü randevular' : 'Günün randevuları'}
                    </Button>
                  }
                />
              </section>
            ) : (
              <PaymentsTable day={query.data} />
            )}
            {showCount ? (
              <CashCountCard day={query.data} focusOnOpen={counting} />
            ) : query.data.closing ? (
              <ClosedDayCard closing={query.data.closing} timeZone={query.data.timeZone} />
            ) : null}
          </div>
        </>
      )}
      {picking ? (
        <PreviousDaysModal
          today={today}
          onClose={() => {
            setPicking(false);
          }}
          onPick={(picked) => {
            setPicking(false);
            openDay(picked);
          }}
        />
      ) : null}
    </Page>
  );
}

/** Yöntem blokları: toplam (marka), nakit, kart (POS), havale (kapora ayrımıyla). */
function CashTotals({ day, isToday }: { day: CashDay; isToday: boolean }) {
  const empty = day.payments.length === 0;
  const { transfer } = day;
  const transferSub =
    transfer.depositAmount.amountMinor > 0 && transfer.otherAmount.amountMinor > 0
      ? `${moneyInputValue(transfer.otherAmount.amountMinor)} ödeme · ${moneyInputValue(transfer.depositAmount.amountMinor)} kapora`
      : transfer.depositAmount.amountMinor > 0
        ? 'kapora'
        : empty
          ? 'kapora dahil'
          : `${String(transfer.count)} ödeme`;
  return (
    <Bento label="Kasa özeti">
      <BentoBlock
        tone="marka"
        label={empty ? 'Tahsilat' : isToday ? 'Bugün tahsil edilen' : 'Tahsil edilen'}
        value={formatMoney(day.total.amount)}
        sub={
          empty
            ? isToday
              ? 'bugün'
              : formatDate(day.date, 'dayMonth')
            : `${String(day.total.count)} ödeme`
        }
      />
      <BentoBlock
        {...(empty ? { tone: 'kum' as const } : {})}
        label="Nakit"
        value={formatMoney(day.cash.amount)}
        sub={empty ? 'kasada' : 'kasada olmalı'}
      />
      <BentoBlock label="Kart" value={formatMoney(day.card.amount)} sub="POS" />
      <BentoBlock
        {...(empty ? {} : { tone: 'kum' as const })}
        label="Havale"
        value={formatMoney(transfer.amount)}
        sub={transferSub}
      />
    </Bento>
  );
}

function PaymentsTable({ day }: { day: CashDay }) {
  const tz = day.timeZone;
  return (
    <DataTable<PaymentView>
      caption="Günün tahsilatları"
      rows={day.payments}
      rowKey={(row) => row.id}
      columns={[
        { key: 'time', label: 'Saat', render: (row) => formatTime(row.paidUtc, tz) },
        {
          key: 'customer',
          label: 'Müşteri',
          render: (row) => <strong>{row.customer?.fullName ?? '—'}</strong>,
        },
        {
          key: 'item',
          label: 'Kalem',
          render: (row) => (
            <span className={styles.item}>
              {paymentItemLabel(row, tz)}
              {row.refundedAmount ? (
                <small>{`${formatMoney(row.refundedAmount)} iade edildi`}</small>
              ) : null}
              {row.recordedAfterDayClose ? <small>kapanıştan sonra</small> : null}
            </span>
          ),
        },
        {
          key: 'method',
          label: 'Yöntem',
          render: (row) => <Badge tone={methodTone(row)}>{methodLabels[row.method]}</Badge>,
        },
        { key: 'staff', label: 'Personel', render: (row) => firstName(row.staffName) ?? '—' },
        {
          key: 'amount',
          label: 'Tutar',
          numeric: true,
          render: (row) => <strong>{formatMoney(row.amount)}</strong>,
        },
      ]}
      footer={
        <p className={styles.note}>
          Paket satışı tahsilattır; seans kullanıldıkça gelir sayılır. Ürün satışı ve fiş kesimi bu
          sürümde yok.
        </p>
      }
    />
  );
}
