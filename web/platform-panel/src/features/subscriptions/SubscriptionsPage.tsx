import { unwrap } from '@omvila/api-client-ts';
import type { components } from '@omvila/api-client-ts';
import { formatCount, formatDate } from '@omvila/ui-web/format';
import {
  Bento,
  BentoBlock,
  Button,
  DataTable,
  EmptyState,
  FilterChips,
  PersonCell,
} from '@omvila/ui-web/panel';
import type { BadgeTone } from '@omvila/ui-web/panel';
import { addDays, branchToday } from '@omvila/ui-web/time';
import { infiniteQueryOptions, keepPreviousData, useInfiniteQuery } from '@tanstack/react-query';
import { useNavigate } from '@tanstack/react-router';
import { ChevronRight, CreditCard, Search } from 'lucide-react';
import type { Api } from '@/app/api';
import { useApi } from '@/app/api-context';
import { useClock } from '@/app/clock';
import { platformTimeZone } from '@/app/config';
import { queryKeys } from '@/app/query-keys';
import { BrandBadge } from '@/features/brands/BrandBadge';
import brandStyles from '@/features/brands/Brands.module.css';
import { subscriptionLabels } from '@/features/brands/labels';
import { Page } from '@/shared/Page';
import { QueryState } from '@/shared/QueryState';
import { accusativeCount } from '@/shared/turkish';

type Row = components['schemas']['AdminSubscriptionListItem'];

/** PAbonelikler süzgeç çipleri (sözleşme: filter). */
export const subscriptionFilters = [
  { value: 'All', label: 'Tümü' },
  { value: 'Trialing', label: 'Denemede' },
  { value: 'Active', label: 'Aktif' },
  { value: 'PastDue', label: 'Ödeme gecikti' },
  { value: 'ReadOnly', label: 'Salt okunur' },
  { value: 'Cancelled', label: 'İptal edildi' },
  { value: 'CreditsLow', label: 'Kontörü azalan' },
] as const;
export type SubscriptionFilter = (typeof subscriptionFilters)[number]['value'];

function subscriptionsQuery(api: Api, filter: SubscriptionFilter, q: string) {
  return infiniteQueryOptions({
    queryKey: queryKeys.subscriptions(filter, q),
    queryFn: ({ pageParam }) =>
      unwrap(
        api.GET('/api/v1/admin/subscriptions', {
          params: {
            query: {
              filter,
              limit: 25,
              ...(q.length >= 2 ? { q } : {}),
              ...(pageParam ? { cursor: pageParam } : {}),
            },
          },
        }),
      ),
    initialPageParam: undefined as string | undefined,
    getNextPageParam: (last) => last.nextCursor ?? undefined,
  });
}

const tones: Record<Row['status'], BadgeTone> = {
  Trialing: 'notr',
  Active: 'basari',
  PastDue: 'uyari',
  ReadOnly: 'hata',
  Cancelled: 'notr',
};

/** "Ödeme gecikti · 6. gün", "Aktif · kontör bitti". */
export function subscriptionBadge(row: Row): { label: string; tone: BadgeTone } {
  if (row.status === 'PastDue' && row.pastDueDays !== null) {
    return { label: `Ödeme gecikti · ${String(row.pastDueDays)}. gün`, tone: 'uyari' };
  }

  if (row.status === 'Active' && row.creditBalance === 0) {
    return { label: 'Aktif · kontör bitti', tone: 'uyari' };
  }

  return { label: subscriptionLabels[row.status], tone: tones[row.status] };
}

/** "Sonraki adım": "29 Kas deneme biter", "Yarın salt okunur", "Ödeme gelince açılır". */
export function nextStepText(step: Row['nextStep'], today: string): string {
  const day = step.date
    ? step.date === today
      ? 'Bugün'
      : step.date === addDays(today, 1)
        ? 'Yarın'
        : formatDate(step.date, 'dayMonthShort')
    : '';
  switch (step.type) {
    case 'TrialEnds':
      return `${day} deneme biter`;
    case 'ReadOnlyOn':
      return `${day} salt okunur`;
    case 'Renews':
      return `${day} yenileme`;
    case 'ReactivatesOnPayment':
      return 'Ödeme gelince açılır';
    case 'SmsStopped':
      return 'SMS gönderilmiyor';
    case 'ArchivesOn':
      return `${day} arşivlenir`;
  }
}

/** platform-paneli/PAbonelikler: üst sayılar, süzgeçler ve aciliyete göre sıralı tablo. "Dışa aktar" sözleşmede yok. */
export function SubscriptionsPage({ filter, q }: { filter: SubscriptionFilter; q: string }) {
  const api = useApi();
  const navigate = useNavigate();
  const today = branchToday(useClock(), platformTimeZone);
  const query = useInfiniteQuery({
    ...subscriptionsQuery(api, filter, q),
    placeholderData: keepPreviousData,
  });
  const counts = query.data?.pages[0]?.counts;
  const rows = query.data?.pages.flatMap((page) => page.items);

  return (
    <Page title="Abonelikler" context={formatDate(today, 'monthYear')}>
      {counts ? (
        <Bento label="Abonelik özeti">
          <BentoBlock
            tone="marka"
            label="Aktif"
            value={formatCount(counts.active)}
            sub="ödeme düzenli"
          />
          <BentoBlock
            tone="kum"
            label="Denemede"
            value={formatCount(counts.trialing)}
            sub={
              counts.trialEndingThisWeek > 0
                ? `${accusativeCount(counts.trialEndingThisWeek)} bu hafta bitiyor`
                : 'Bu hafta biten deneme yok'
            }
          />
          <BentoBlock
            tone="vurgu"
            label="Ödeme gecikti"
            value={formatCount(counts.pastDue)}
            sub="hatırlatma süresinde"
          />
          <BentoBlock
            label="Salt okunur"
            value={formatCount(counts.readOnly)}
            sub="online randevu kapalı"
          />
        </Bento>
      ) : null}
      <div className={brandStyles.toolbar}>
        <FilterChips
          legend="Abonelik süzgeci"
          name="abonelik-suzgeci"
          options={subscriptionFilters}
          value={filter}
          onChange={(next) =>
            void navigate({
              to: '/abonelikler',
              search: { ...(next === 'All' ? {} : { filtre: next }), ...(q ? { q } : {}) },
            })
          }
        />
      </div>
      <QueryState query={{ ...query, data: rows }}>
        {(items) => (
          <DataTable
            caption="Abonelikler"
            rows={items}
            rowKey={(row) => row.brandId}
            onRowClick={(row) =>
              void navigate({ to: '/markalar/$brandId', params: { brandId: row.brandId } })
            }
            columns={[
              {
                key: 'brand',
                label: 'Marka',
                render: (row) => <PersonCell name={row.brandName} />,
              },
              { key: 'plan', label: 'Plan', render: (row) => row.planName },
              {
                key: 'status',
                label: 'Durum',
                render: (row) => <BrandBadge {...subscriptionBadge(row)} />,
              },
              {
                key: 'next',
                label: 'Sonraki adım',
                render: (row) => nextStepText(row.nextStep, today),
              },
              {
                key: 'credits',
                label: 'Kontör',
                numeric: true,
                render: (row) => formatCount(row.creditBalance),
              },
              {
                key: 'open',
                label: 'Aç',
                align: 'right',
                render: () => <ChevronRight size={16} aria-hidden="true" />,
              },
            ]}
            empty={
              q ? (
                <EmptyState
                  icon={<Search size={22} />}
                  title="Aramaya uyan abonelik yok"
                  description="Marka adının bir kısmını deneyin."
                />
              ) : (
                <EmptyState
                  icon={<CreditCard size={22} />}
                  title="Bu süzgece uyan abonelik yok"
                  description="Başka bir süzgeç seçin."
                />
              )
            }
            footer={
              query.hasNextPage ? (
                <Button
                  size="sm"
                  loading={query.isFetchingNextPage}
                  loadingLabel="Yükleniyor…"
                  onClick={() => void query.fetchNextPage()}
                >
                  Daha fazla göster
                </Button>
              ) : undefined
            }
          />
        )}
      </QueryState>
      <p className={brandStyles.note}>
        Durum sırası: Deneme → Aktif → Ödeme gecikti → Salt okunur. Salt okunurda veriler korunur,
        online randevu ve SMS durur.
      </p>
    </Page>
  );
}
