import type { components } from '@omvila/api-client-ts';
import { formatCount, formatDate, formatMoney, formatTime } from '@omvila/ui-web/format';
import { Badge, Button, DataTable, EmptyState, Skeleton } from '@omvila/ui-web/panel';
import type { BadgeTone } from '@omvila/ui-web/panel';
import { addDays, branchToday, localDateTimeToUtc, toBranchDate } from '@omvila/ui-web/time';
import { useInfiniteQuery } from '@tanstack/react-query';
import { FileText, History, Landmark } from 'lucide-react';
import { useMemo } from 'react';
import type { ReactNode } from 'react';
import type { Api } from '@/app/api';
import { useApi } from '@/app/api-context';
import { useClock } from '@/app/clock';
import { platformTimeZone } from '@/app/config';
import { auditActionLabel, auditActorLabel, auditDetail } from '@/features/audit/labels';
import { QueryState } from '@/shared/QueryState';
import {
  brandAuditQuery,
  creditTransactionsQuery,
  invoicesQuery,
  paymentNoticesQuery,
} from './api';
import { CreditTransactionsTable } from './CreditTransactionsTable';
import type { BrandTab } from './labels';

type Schemas = components['schemas'];
type Invoice = Schemas['SubscriptionInvoice'];
type Notice = Schemas['PaymentNoticeSummary'];

// Marka denetim sekmesi son 90 günü gösterir (fromUtc; sunucunun varsayılanı son 24 saat, T-123).
const auditWindowDays = 90;

const localDate = (utc: string) =>
  formatDate(toBranchDate(new Date(utc), platformTimeZone), 'dayMonthShort');

/**
 * Marka detayının Özet dışındaki sekmeleri. Tasarım yalnız Özet'i gösterir; sekmeler Kontör hareketleri tablosu
 * deseniyle (tarih, ad, tutar, durum) çizildi (T-123). Her sekmenin açılışı sunucuda denetime yazılır.
 */
export function BrandTabPanel({
  brandId,
  tab,
}: {
  brandId: string;
  tab: Exclude<BrandTab, 'ozet'>;
}) {
  switch (tab) {
    case 'fatura':
      return <InvoicesTab brandId={brandId} />;
    case 'kontor':
      return <CreditsTab brandId={brandId} />;
    case 'odemeler':
      return <NoticesTab brandId={brandId} />;
    case 'denetim':
      return <AuditTab brandId={brandId} />;
  }
}

interface Paged<T> {
  data: { pages: { items: T[] }[] } | undefined;
  error: unknown;
  isPending: boolean;
  hasNextPage: boolean;
  isFetchingNextPage: boolean;
  fetchNextPage: () => Promise<unknown>;
  refetch: () => Promise<unknown>;
}

/** İmleçli sekme: yükleniyor, hata, boş ve "Daha fazla göster". */
function PagedTab<T>({
  query,
  children,
}: {
  query: Paged<T>;
  children: (rows: T[], footer: ReactNode) => ReactNode;
}) {
  const rows = query.data?.pages.flatMap((page) => page.items);
  return (
    <QueryState
      query={{ ...query, data: rows }}
      loading={<Skeleton variant="block" height="calc(var(--bosluk-12) * 3)" />}
    >
      {(items) =>
        children(
          items,
          query.hasNextPage ? (
            <Button
              size="sm"
              loading={query.isFetchingNextPage}
              loadingLabel="Yükleniyor…"
              onClick={() => void query.fetchNextPage()}
            >
              Daha fazla göster
            </Button>
          ) : null,
        )
      }
    </QueryState>
  );
}

const invoiceStatus: Record<Invoice['status'], { label: string; tone: BadgeTone }> = {
  Issued: { label: 'Kesildi', tone: 'bilgi' },
  Paid: { label: 'Ödendi', tone: 'basari' },
  Failed: { label: 'Ödenemedi', tone: 'hata' },
  Void: { label: 'İptal', tone: 'notr' },
};

function InvoicesTab({ brandId }: { brandId: string }) {
  const query = useInfiniteQuery(invoicesQuery(useApi(), brandId));
  return (
    <PagedTab query={query}>
      {(rows, footer) => (
        <DataTable
          caption="Faturalar"
          rows={rows}
          rowKey={(invoice) => invoice.id}
          columns={[
            { key: 'no', label: 'No', render: (invoice) => invoice.invoiceNumber },
            {
              key: 'period',
              label: 'Dönem',
              render: (invoice) =>
                `${formatDate(invoice.periodStart, 'dayMonthShort')} – ${formatDate(invoice.periodEnd, 'dayMonthShort')}`,
            },
            {
              key: 'total',
              label: 'Tutar',
              numeric: true,
              render: (invoice) => formatMoney(invoice.total),
            },
            {
              key: 'status',
              label: 'Durum',
              render: (invoice) => (
                <Badge tone={invoiceStatus[invoice.status].tone}>
                  {invoiceStatus[invoice.status].label}
                </Badge>
              ),
            },
            {
              key: 'paid',
              label: 'Ödendiği gün',
              render: (invoice) => (invoice.paidUtc ? localDate(invoice.paidUtc) : '—'),
            },
          ]}
          empty={
            <EmptyState
              icon={<FileText size={22} />}
              title="Henüz fatura yok"
              description="Deneme süresindeki markaya fatura kesilmez."
            />
          }
          footer={footer}
        />
      )}
    </PagedTab>
  );
}

function CreditsTab({ brandId }: { brandId: string }) {
  const query = useInfiniteQuery(creditTransactionsQuery(useApi(), brandId));
  return (
    <PagedTab query={query}>
      {(rows, footer) => <CreditTransactionsTable rows={rows} footer={footer} />}
    </PagedTab>
  );
}

const noticeStatus: Record<Notice['status'], { label: string; tone: BadgeTone }> = {
  Pending: { label: 'Eşleştirme bekliyor', tone: 'uyari' },
  Matched: { label: 'Eşleşti', tone: 'basari' },
  NotFound: { label: 'Bulunamadı', tone: 'hata' },
  Rejected: { label: 'Reddedildi', tone: 'notr' },
};

function NoticesTab({ brandId }: { brandId: string }) {
  const query = useInfiniteQuery(paymentNoticesQuery(useApi(), brandId));
  return (
    <PagedTab query={query}>
      {(rows, footer) => (
        <DataTable
          caption="Havale bildirimleri"
          rows={rows}
          rowKey={(notice) => notice.id}
          columns={[
            { key: 'date', label: 'Tarih', render: (notice) => localDate(notice.notifiedUtc) },
            {
              key: 'purpose',
              label: 'Amaç',
              render: (notice) =>
                notice.purpose === 'Credits'
                  ? `Kontör${notice.credits ? ` · ${formatCount(notice.credits)} SMS` : ''}`
                  : 'Abonelik',
            },
            { key: 'code', label: 'Açıklama kodu', render: (notice) => notice.transferCode },
            {
              key: 'amount',
              label: 'Tutar',
              numeric: true,
              render: (notice) =>
                notice.expectedAmount ? formatMoney(notice.expectedAmount) : '—',
            },
            {
              key: 'status',
              label: 'Durum',
              render: (notice) => (
                <Badge tone={noticeStatus[notice.status].tone}>
                  {noticeStatus[notice.status].label}
                </Badge>
              ),
            },
            {
              key: 'receipt',
              label: 'Dekont',
              render: (notice) => (notice.hasReceipt ? 'Var' : 'Yok'),
            },
          ]}
          empty={
            <EmptyState
              icon={<Landmark size={22} />}
              title="Havale bildirimi yok"
              description="Marka abonelik veya kontör için havale bildirmedi."
            />
          }
          footer={footer}
        />
      )}
    </PagedTab>
  );
}

function auditQuery(api: Api, brandId: string, today: string) {
  return brandAuditQuery(
    api,
    brandId,
    localDateTimeToUtc(addDays(today, -auditWindowDays), '00:00', platformTimeZone),
  );
}

function AuditTab({ brandId }: { brandId: string }) {
  const api = useApi();
  const clock = useClock();
  const today = branchToday(clock, platformTimeZone);
  const options = useMemo(() => auditQuery(api, brandId, today), [api, brandId, today]);
  const query = useInfiniteQuery(options);
  return (
    <PagedTab query={query}>
      {(rows, footer) => (
        <DataTable
          caption="Denetim kaydı"
          rows={rows}
          rowKey={(entry) => String(entry.id)}
          columns={[
            {
              key: 'time',
              label: 'Zaman',
              render: (entry) =>
                `${localDate(entry.occurredUtc)} ${formatTime(entry.occurredUtc, platformTimeZone)}`,
            },
            { key: 'actor', label: 'Kim', render: (entry) => auditActorLabel(entry.actor) },
            { key: 'action', label: 'İşlem', render: auditActionLabel },
            { key: 'detail', label: 'Ayrıntı', render: auditDetail },
          ]}
          empty={
            <EmptyState
              icon={<History size={22} />}
              title="Son 90 günde kayıt yok"
              description="Bu markada platform işlemi veya görüntüleme yapılmadı."
            />
          }
          footer={footer}
        />
      )}
    </PagedTab>
  );
}
