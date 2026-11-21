import { ApiProblem } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { formatMoney } from '@omvila/ui-web/format';
import {
  Badge,
  Button,
  DataTable,
  EmptyState,
  PersonCell,
  Skeleton,
  TextField,
} from '@omvila/ui-web/panel';
import { branchToday } from '@omvila/ui-web/time';
import { keepPreviousData, useInfiniteQuery } from '@tanstack/react-query';
import { useNavigate } from '@tanstack/react-router';
import { Phone, Plus, Search, ShieldCheck, UserPlus, Users, X } from 'lucide-react';
import { useEffect, useState } from 'react';
import { useApi } from '@/app/api-context';
import { useSelectedBranch } from '@/app/branch';
import { useClock } from '@/app/clock';
import { defaultTimeZone } from '@/app/config';
import { useSession } from '@/app/session';
import { Page } from '@/shared/Page';
import { AddCustomerModal } from './AddCustomerModal';
import type { CustomerPrefill } from './AddCustomerModal';
import { customersQuery } from './api';
import type { CustomerSummary } from './api';
import styles from './Customers.module.css';
import { FilterChips } from './FilterChips';
import {
  customerPhone,
  dayLabel,
  filterFromSlug,
  filterLabel,
  filterOptions,
  filterSlug,
  instantLabel,
  looksLikePhone,
  tagTone,
} from './labels';

export interface CustomersSearch {
  q?: string;
  filtre?: string;
}

/** Arama kutusu yazmayı bitirince (ms) adres ve sorgu güncellenir. */
const searchDelayMs = 300;

/**
 * Müşteriler (salon-paneli/Musteriler, MusterilerBos, MusterilerAramaYok): filtre çipleri, ad veya telefonla arama
 * ("4418" son 4 hane), imleçli liste. Özet blokları, sıralama, içe/dışa aktarma sözleşmede yok (T-57).
 */
export function CustomersPage({ search }: { search: CustomersSearch }) {
  const api = useApi();
  const clock = useClock();
  const navigate = useNavigate();
  const { session, can } = useSession();
  const { branchId } = useSelectedBranch();
  const branchName =
    session.branches.find((b) => b.id === branchId)?.name ?? session.branches[0]?.name ?? '';
  const q = search.q?.trim() ?? '';
  const filter = filterFromSlug(search.filtre);
  const [text, setText] = useState(q);
  const [adding, setAdding] = useState<CustomerPrefill | null>(null);
  // Yazarken önceki sonuç ekranda kalır (iskelet yanıp sönmez).
  const query = useInfiniteQuery({
    ...customersQuery(api, q, filter),
    placeholderData: keepPreviousData,
  });
  const today = branchToday(clock, defaultTimeZone);
  const canAdd = can('customer.edit');

  const setSearch = (next: { q?: string | undefined; filtre?: string | undefined }) => {
    void navigate({
      to: '/musteriler',
      search: {
        ...(next.q ? { q: next.q } : {}),
        ...(next.filtre ? { filtre: next.filtre } : {}),
      },
    });
  };

  useEffect(() => {
    const trimmed = text.trim();
    if (trimmed === q) {
      return;
    }

    const timer = setTimeout(() => {
      void navigate({
        to: '/musteriler',
        search: (prev: CustomersSearch) => ({
          ...(trimmed !== '' ? { q: trimmed } : {}),
          ...(prev.filtre ? { filtre: prev.filtre } : {}),
        }),
        replace: true,
      });
    }, searchDelayMs);
    return () => {
      clearTimeout(timer);
    };
  }, [text, q, navigate]);

  const rows = query.data?.pages.flatMap((page) => page.items) ?? [];
  const noCustomersAtAll =
    query.isSuccess && rows.length === 0 && q === '' && text.trim() === '' && filter === 'All';
  const clear = () => {
    setText('');
    setSearch({});
  };

  const openCard = (row: CustomerSummary) => {
    void navigate({ to: '/musteriler/$id', params: { id: row.id } });
  };

  return (
    <Page
      title="Müşteriler"
      {...(branchName ? { context: branchName } : {})}
      action={
        canAdd ? (
          <Button
            variant="primary"
            icon={<Plus size={16} aria-hidden="true" />}
            onClick={() => {
              setAdding({});
            }}
          >
            Müşteri ekle
          </Button>
        ) : null
      }
    >
      {noCustomersAtAll ? (
        <CustomersEmpty
          canAdd={canAdd}
          onAdd={() => {
            setAdding({});
          }}
        />
      ) : (
        <>
          <div className={styles.toolbar}>
            <FilterChips
              legend="Müşteri filtresi"
              name="musteri-filtresi"
              options={filterOptions}
              value={filter}
              onChange={(value) => {
                setSearch({ q, filtre: filterSlug(value) });
              }}
            />
            <TextField
              className={styles.search}
              label="Müşteri ara"
              hideLabel
              type="search"
              placeholder="Ad veya telefon"
              icon={<Search size={16} />}
              value={text}
              maxLength={80}
              onChange={(event) => {
                setText(event.target.value);
              }}
            />
          </div>
          {query.isPending ? (
            <Skeleton variant="block" height="320px" />
          ) : query.isError ? (
            <section className={styles.emptyCard} aria-label="Hata">
              <EmptyState
                headingLevel={2}
                title="Müşteriler yüklenemedi"
                description={errorMessage(
                  query.error instanceof ApiProblem ? query.error.code : undefined,
                )}
                actions={<Button onClick={() => void query.refetch()}>Tekrar dene</Button>}
              />
            </section>
          ) : (
            <DataTable<CustomerSummary>
              caption="Müşteriler"
              rows={rows}
              rowKey={(row) => row.id}
              onRowClick={openCard}
              empty={
                <NoResults
                  q={q}
                  filterText={filter === 'All' ? null : filterLabel(filter)}
                  onClear={clear}
                  {...(canAdd && q !== ''
                    ? {
                        onAdd: () => {
                          setAdding(looksLikePhone(q) ? { phone: q } : { fullName: q });
                        },
                      }
                    : {})}
                />
              }
              columns={[
                {
                  key: 'customer',
                  label: 'Müşteri',
                  render: (row) => <PersonCell name={row.fullName} detail={customerPhone(row)} />,
                },
                {
                  key: 'lastVisit',
                  label: 'Son ziyaret',
                  render: (row) => (row.lastVisitOn ? dayLabel(row.lastVisitOn, today) : '—'),
                },
                {
                  key: 'next',
                  label: 'Sonraki randevu',
                  render: (row) =>
                    row.nextAppointmentUtc
                      ? instantLabel(row.nextAppointmentUtc, today, defaultTimeZone)
                      : '—',
                },
                {
                  key: 'visits',
                  label: 'Ziyaret',
                  numeric: true,
                  render: (row) => String(row.visitCount),
                },
                {
                  key: 'total',
                  label: 'Toplam',
                  numeric: true,
                  render: (row) => (
                    <strong>{row.totalSpent ? formatMoney(row.totalSpent) : '—'}</strong>
                  ),
                },
                {
                  key: 'tags',
                  label: 'Etiket',
                  render: (row) => (
                    <span className={styles.tags}>
                      {row.tags.map((tag) => (
                        <Badge key={tag} tone={tagTone(tag)}>
                          {tag}
                        </Badge>
                      ))}
                    </span>
                  ),
                },
              ]}
              footer={
                <div className={styles.footer}>
                  <span>{`${String(rows.length)} müşteri gösteriliyor`}</span>
                  {query.hasNextPage ? (
                    <Button
                      size="sm"
                      loading={query.isFetchingNextPage}
                      loadingLabel="Yükleniyor…"
                      onClick={() => void query.fetchNextPage()}
                    >
                      Daha fazla göster
                    </Button>
                  ) : null}
                </div>
              }
            />
          )}
        </>
      )}
      {adding ? (
        <AddCustomerModal
          prefill={adding}
          onClose={() => {
            setAdding(null);
          }}
          onOpenCard={(id) => {
            setAdding(null);
            void navigate({ to: '/musteriler/$id', params: { id } });
          }}
        />
      ) : null}
    </Page>
  );
}

/** MusterilerBos: henüz hiç müşteri yok. CSV içe aktarma ayrı dilimde (T-57). */
function CustomersEmpty({ canAdd, onAdd }: { canAdd: boolean; onAdd: () => void }) {
  return (
    <section className={styles.emptyCard} aria-label="Müşteri yok">
      <EmptyState
        icon={<Users size={24} />}
        headingLevel={2}
        title="Henüz müşteriniz yok"
        description="Online randevu alan herkes buraya kendiliğinden eklenir. Telefonla gelen müşterilerinizi tek tek ekleyebilirsiniz."
        actions={
          canAdd ? (
            <Button icon={<Plus size={16} aria-hidden="true" />} onClick={onAdd}>
              Tek tek ekle
            </Button>
          ) : undefined
        }
      />
      <div className={styles.tips}>
        <div className={styles.tip}>
          <Phone size={18} className={styles.tipIcon} aria-hidden="true" />
          <div>
            <p className={styles.tipTitle}>Telefon numarası anahtardır</p>
            <p className={styles.tipText}>Aynı numara iki kez eklenmez.</p>
          </div>
        </div>
        <div className={styles.tip}>
          <ShieldCheck size={18} className={styles.tipIcon} aria-hidden="true" />
          <div>
            <p className={styles.tipTitle}>İzinler müşteride</p>
            <p className={styles.tipText}>Kampanya mesajı yalnızca izin verenlere gider.</p>
          </div>
        </div>
      </div>
    </section>
  );
}

/** MusterilerAramaYok: arama veya filtre sonuçsuz. */
function NoResults({
  q,
  filterText,
  onClear,
  onAdd,
}: {
  q: string;
  filterText: string | null;
  onClear: () => void;
  onAdd?: () => void;
}) {
  const hint = 'Ad, soyad ya da telefonun son 4 hanesiyle arayabilirsiniz.';
  return (
    <EmptyState
      icon={<Search size={24} />}
      headingLevel={2}
      title={q !== '' ? `“${q}” için sonuç yok` : 'Bu filtrede müşteri yok'}
      description={filterText ? `${hint} Seçili filtre: ${filterText}.` : hint}
      actions={
        <>
          <Button icon={<X size={16} aria-hidden="true" />} onClick={onClear}>
            {filterText ? 'Filtreyi temizle' : 'Aramayı temizle'}
          </Button>
          {onAdd ? (
            <Button
              variant="primary"
              icon={<UserPlus size={16} aria-hidden="true" />}
              onClick={onAdd}
            >
              Yeni müşteri olarak ekle
            </Button>
          ) : null}
        </>
      }
    />
  );
}
