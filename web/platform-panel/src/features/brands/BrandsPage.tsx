import { formatCount } from '@omvila/ui-web/format';
import { Button, DataTable, EmptyState, FilterChips, PersonCell } from '@omvila/ui-web/panel';
import { keepPreviousData, useInfiniteQuery } from '@tanstack/react-query';
import { useNavigate } from '@tanstack/react-router';
import { ChevronRight, Search, Store } from 'lucide-react';
import { useApi } from '@/app/api-context';
import { useSession } from '@/app/session';
import { Page } from '@/shared/Page';
import { QueryState } from '@/shared/QueryState';
import { brandsQuery } from './api';
import { BrandBadge } from './BrandBadge';
import styles from './Brands.module.css';
import { brandFilters, brandStatusBadge, brandSubtitle } from './labels';
import type { BrandFilter, BrandListItem } from './labels';

/**
 * platform-paneli/PMarkalar: süzgeç çipleri, arama (üst bardaki ⌘K kutusu; ?q=), tablo ve imleçli sayfalama.
 * "Dışa aktar" sözleşmede yok; gösterilmez (T-123).
 */
export function BrandsPage({ filter, q }: { filter: BrandFilter; q: string }) {
  const api = useApi();
  const navigate = useNavigate();
  const { can } = useSession();
  const query = useInfiniteQuery({
    ...brandsQuery(api, filter, q),
    placeholderData: keepPreviousData,
  });
  const counts = query.data?.pages[0]?.counts;
  const rows = query.data?.pages.flatMap((page) => page.items);
  const open = (brand: BrandListItem) =>
    void navigate({ to: '/markalar/$brandId', params: { brandId: brand.id } });

  return (
    <Page
      title="Markalar"
      {...(counts
        ? {
            context: `${formatCount(counts.active)} aktif · ${formatCount(counts.trialing)} denemede`,
          }
        : {})}
    >
      <div className={styles.toolbar}>
        <FilterChips
          legend="Marka süzgeci"
          name="marka-suzgeci"
          options={brandFilters}
          value={filter}
          onChange={(next) =>
            void navigate({
              to: '/markalar',
              search: { ...(next === 'All' ? {} : { filtre: next }), ...(q ? { q } : {}) },
            })
          }
        />
        {q ? (
          <p className={styles.searchNote}>
            “{q}” için sonuçlar ·{' '}
            <button
              type="button"
              className={styles.textButton}
              onClick={() =>
                void navigate({
                  to: '/markalar',
                  search: filter === 'All' ? {} : { filtre: filter },
                })
              }
            >
              Aramayı temizle
            </button>
          </p>
        ) : null}
      </div>
      <QueryState query={{ ...query, data: rows }}>
        {(items) => (
          <DataTable
            caption="Markalar"
            rows={items}
            rowKey={(brand) => brand.id}
            onRowClick={open}
            columns={[
              {
                key: 'name',
                label: 'Marka',
                render: (brand) => (
                  <PersonCell
                    name={brand.name}
                    detail={brandSubtitle(brand.industryCode, brand.city)}
                  />
                ),
              },
              { key: 'plan', label: 'Plan', render: (brand) => brand.planName },
              {
                key: 'status',
                label: 'Durum',
                render: (brand) => <BrandBadge {...brandStatusBadge(brand)} />,
              },
              {
                key: 'branches',
                label: 'Şube',
                numeric: true,
                render: (brand) => formatCount(brand.branchCount),
              },
              {
                key: 'appointments',
                label: 'Bu ay randevu',
                numeric: true,
                render: (brand) => formatCount(brand.appointmentsThisMonth),
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
                  title="Aramaya uyan marka yok"
                  description="Marka adını, bağlantı adını veya sahip telefonunun son 4 hanesini deneyin."
                />
              ) : (
                <EmptyState
                  icon={<Store size={22} />}
                  title="Bu süzgece uyan marka yok"
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
      <p className={styles.note}>
        {can('platform.brands.manage') ? '' : 'Rolünüz markaları görüntüler, değiştiremez. '}
        Her marka açılışı denetim kaydına yazılır.
      </p>
    </Page>
  );
}
