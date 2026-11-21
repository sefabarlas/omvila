import { ApiProblem } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { formatMoney } from '@omvila/ui-web/format';
import { Badge, Button, DataTable, EmptyState, Skeleton } from '@omvila/ui-web/panel';
import { toBranchDate } from '@omvila/ui-web/time';
import { useInfiniteQuery } from '@tanstack/react-query';
import { Package, Star, Wallet } from 'lucide-react';
import { useState } from 'react';
import { useApi } from '@/app/api-context';
import { defaultTimeZone } from '@/app/config';
import { useSession } from '@/app/session';
import { customerReviewsQuery } from '@/features/reviews/api';
import { ReviewCard } from '@/features/reviews/ReviewCard';
import reviewStyles from '@/features/reviews/Reviews.module.css';
import type { CustomerPackage, PaymentView } from './api';
import styles from './Customers.module.css';
import {
  dayLabel,
  instantLabel,
  packageStatusBadge,
  paymentItemLabel,
  paymentMethodLabel,
  paymentStatusBadge,
  shortDate,
} from './labels';
import { customerPackagesQuery, customerPaymentsQuery } from './records-api';

// Müşteri kartı sekmeleri (salon-paneli/MusteriKarti): Paketler, Ödemeler, Değerlendirmeler. Randevular sekmesi
// CustomerCardPage'de. Her sekmede yükleniyor, hata ve boş hâl.

interface Pager {
  hasNextPage: boolean;
  isFetchingNextPage: boolean;
  fetchNextPage: () => Promise<unknown>;
}

function TabError({ title, error, retry }: { title: string; error: unknown; retry: () => void }) {
  return (
    <section className={styles.emptyCard} aria-label="Hata">
      <EmptyState
        headingLevel={3}
        title={title}
        description={errorMessage(error instanceof ApiProblem ? error.code : undefined)}
        actions={<Button onClick={retry}>Tekrar dene</Button>}
      />
    </section>
  );
}

function MoreFooter({ query, shown, noun }: { query: Pager; shown: number; noun: string }) {
  return (
    <div className={styles.footer}>
      <span>{`${String(shown)} ${noun} gösteriliyor`}</span>
      <Button
        size="sm"
        loading={query.isFetchingNextPage}
        loadingLabel="Yükleniyor…"
        onClick={() => void query.fetchNextPage()}
      >
        Daha fazla göster
      </Button>
    </div>
  );
}

/** Paketler: ad ve içerik, satış günü, son gün, kalan seans, durum. Satış ve iade ekranları paket PR'ında. */
export function PackagesTab({
  brandCustomerId,
  today,
}: {
  brandCustomerId: string;
  today: string;
}) {
  const api = useApi();
  const query = useInfiniteQuery(customerPackagesQuery(api, brandCustomerId));
  if (query.isPending) {
    return <Skeleton variant="block" height="200px" />;
  }

  if (query.isError) {
    return (
      <TabError
        title="Paketler yüklenemedi"
        error={query.error}
        retry={() => void query.refetch()}
      />
    );
  }

  const rows = query.data.pages.flatMap((page) => page.items);
  return (
    <DataTable<CustomerPackage>
      caption="Paketler"
      rows={rows}
      rowKey={(row) => row.id}
      empty={
        <EmptyState
          icon={<Package size={24} />}
          headingLevel={3}
          title="Paketi yok"
          description="Satılan paketler ve kalan seanslar burada görünür."
        />
      }
      columns={[
        {
          key: 'name',
          label: 'Paket',
          render: (row) => (
            <span className={styles.cellStack}>
              <strong>{row.packageName}</strong>
              {row.items.length > 0 ? (
                <span className={styles.muted}>
                  {row.items
                    .map(
                      (item) =>
                        `${item.serviceName} ${String(item.remainingSessions)}/${String(item.totalSessions)}`,
                    )
                    .join(', ')}
                </span>
              ) : null}
            </span>
          ),
        },
        {
          key: 'purchased',
          label: 'Satış',
          render: (row) =>
            dayLabel(toBranchDate(new Date(row.purchasedUtc), defaultTimeZone), today),
        },
        { key: 'expires', label: 'Son gün', render: (row) => shortDate(row.expiresOn) },
        {
          key: 'remaining',
          label: 'Kalan',
          numeric: true,
          render: (row) => `${String(row.remainingSessions)}/${String(row.totalSessions)} seans`,
        },
        {
          key: 'status',
          label: 'Durum',
          render: (row) => {
            const badge = packageStatusBadge(row.status);
            return <Badge tone={badge.tone}>{badge.label}</Badge>;
          },
        },
        {
          key: 'price',
          label: 'Tutar',
          numeric: true,
          render: (row) => <strong>{formatMoney(row.price)}</strong>,
        },
      ]}
      {...(query.hasNextPage
        ? { footer: <MoreFooter query={query} shown={rows.length} noun="paket" /> }
        : {})}
    />
  );
}

/** Ödemeler: tarih, kalem, yöntem, iade işareti, tutar (yeniden eskiye). */
export function PaymentsTab({
  brandCustomerId,
  today,
}: {
  brandCustomerId: string;
  today: string;
}) {
  const api = useApi();
  const query = useInfiniteQuery(customerPaymentsQuery(api, brandCustomerId));
  if (query.isPending) {
    return <Skeleton variant="block" height="200px" />;
  }

  if (query.isError) {
    return (
      <TabError
        title="Ödemeler yüklenemedi"
        error={query.error}
        retry={() => void query.refetch()}
      />
    );
  }

  const rows = query.data.pages.flatMap((page) => page.items);
  return (
    <DataTable<PaymentView>
      caption="Ödemeler"
      rows={rows}
      rowKey={(row) => row.id}
      empty={
        <EmptyState
          icon={<Wallet size={24} />}
          headingLevel={3}
          title="Henüz ödeme yok"
          description="Randevu tahsilatları, kaporalar ve paket satışları burada görünür."
        />
      }
      columns={[
        {
          key: 'date',
          label: 'Tarih',
          render: (row) => instantLabel(row.paidUtc, today, defaultTimeZone),
        },
        {
          key: 'item',
          label: 'Kalem',
          render: (row) => <strong>{paymentItemLabel(row, defaultTimeZone)}</strong>,
        },
        { key: 'method', label: 'Yöntem', render: (row) => paymentMethodLabel(row.method) },
        {
          key: 'status',
          label: 'Durum',
          render: (row) => {
            const badge = paymentStatusBadge(row.status);
            return badge ? <Badge tone={badge.tone}>{badge.label}</Badge> : null;
          },
        },
        {
          key: 'amount',
          label: 'Tutar',
          numeric: true,
          render: (row) => <strong>{formatMoney(row.amount)}</strong>,
        },
      ]}
      {...(query.hasNextPage
        ? { footer: <MoreFooter query={query} shown={rows.length} noun="ödeme" /> }
        : {})}
    />
  );
}

/** Değerlendirmeler: kartlar ve yanıt (review.reply). */
export function ReviewsTab({ brandCustomerId, today }: { brandCustomerId: string; today: string }) {
  const api = useApi();
  const { can } = useSession();
  const query = useInfiniteQuery(customerReviewsQuery(api, brandCustomerId));
  const [replyingId, setReplyingId] = useState<string | null>(null);
  if (query.isPending) {
    return <Skeleton variant="block" height="200px" />;
  }

  if (query.isError) {
    return (
      <TabError
        title="Değerlendirmeler yüklenemedi"
        error={query.error}
        retry={() => void query.refetch()}
      />
    );
  }

  const rows = query.data.pages.flatMap((page) => page.items);
  if (rows.length === 0) {
    return (
      <section className={styles.emptyCard} aria-label="Değerlendirme yok">
        <EmptyState
          icon={<Star size={24} />}
          headingLevel={3}
          title="Değerlendirme yok"
          description="Müşteri tamamlanan randevusunu değerlendirince burada görünür."
        />
      </section>
    );
  }

  return (
    <div className={reviewStyles.list}>
      {rows.map((review) => (
        <ReviewCard
          key={review.id}
          review={review}
          today={today}
          canReply={can('review.reply')}
          replying={replyingId === review.id}
          onReplyingChange={(open) => {
            setReplyingId(open ? review.id : null);
          }}
        />
      ))}
      {query.hasNextPage ? (
        <MoreFooter query={query} shown={rows.length} noun="değerlendirme" />
      ) : null}
    </div>
  );
}
