import { ApiProblem, unwrap } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { formatDate, formatTime } from '@omvila/ui-web/format';
import {
  Avatar,
  Button,
  DataTable,
  EmptyState,
  FilterChips,
  Select,
  useToast,
} from '@omvila/ui-web/panel';
import { addDays, branchToday, toBranchDate } from '@omvila/ui-web/time';
import { infiniteQueryOptions, keepPreviousData, useInfiniteQuery } from '@tanstack/react-query';
import { useNavigate } from '@tanstack/react-router';
import { FileText, History, Search } from 'lucide-react';
import { useState } from 'react';
import type { Api } from '@/app/api';
import { useApi } from '@/app/api-context';
import { useClock } from '@/app/clock';
import { platformTimeZone } from '@/app/config';
import { queryKeys } from '@/app/query-keys';
import approvalStyles from '@/features/approvals/Approvals.module.css';
import brandStyles from '@/features/brands/Brands.module.css';
import { downloadFile, dispositionFileName } from '@/shared/download';
import { Page } from '@/shared/Page';
import { PermissionButton } from '@/shared/PermissionButton';
import { QueryState } from '@/shared/QueryState';
import styles from './Audit.module.css';
import { auditActionLabel, auditActor, auditDetail } from './labels';
import type { AuditEntry } from './labels';

/** PDenetim kategori çipleri (sözleşme: category). */
export const auditCategories = [
  { value: 'All', label: 'Tümü' },
  { value: 'View', label: 'Görüntüleme' },
  { value: 'ApproveReject', label: 'Onay ve ret' },
  { value: 'CreditsAndPlan', label: 'Kontör ve plan' },
  { value: 'Kvkk', label: 'KVKK' },
] as const;
export type AuditCategory = (typeof auditCategories)[number]['value'];

/** Zaman süzgeci (fromUtc istemcide; dışa aktarma sınırı 92 gün olduğundan en uzun 90 gün). */
export const auditPeriods = [
  { value: '24s', label: 'Son 24 saat', hours: 24 },
  { value: '7g', label: 'Son 7 gün', hours: 7 * 24 },
  { value: '30g', label: 'Son 30 gün', hours: 30 * 24 },
  { value: '90g', label: 'Son 90 gün', hours: 90 * 24 },
] as const;
export type AuditPeriod = (typeof auditPeriods)[number]['value'];

export interface AuditFilters {
  category: AuditCategory;
  q: string;
  brandId: string;
  period: AuditPeriod;
}

function filterQuery({ category, q, brandId }: AuditFilters, fromUtc: string) {
  return {
    fromUtc,
    ...(category === 'All' ? {} : { category }),
    ...(q.length >= 2 ? { q } : {}),
    ...(brandId ? { brandId } : {}),
  };
}

function auditLogQuery(api: Api, filters: AuditFilters, fromUtc: string) {
  return infiniteQueryOptions({
    queryKey: queryKeys.auditLog(filters.category, filters.q, filters.brandId, filters.period),
    queryFn: ({ pageParam }) =>
      unwrap(
        api.GET('/api/v1/admin/audit-log', {
          params: {
            query: {
              ...filterQuery(filters, fromUtc),
              limit: 50,
              ...(pageParam ? { cursor: pageParam } : {}),
            },
          },
        }),
      ),
    initialPageParam: undefined as string | undefined,
    getNextPageParam: (last) => last.nextCursor ?? undefined,
  });
}

/** "01:12" (bugün), "Dün 23:40", "18 Kas 14:05". */
function whenText(utc: string, today: string): string {
  const day = toBranchDate(new Date(utc), platformTimeZone);
  const time = formatTime(utc, platformTimeZone);
  if (day === today) return time;
  if (day === addDays(today, -1)) return `Dün ${time}`;
  return `${formatDate(day, 'dayMonthShort')} ${time}`;
}

/**
 * platform-paneli/PDenetim: kategori, zaman, marka ve arama süzgeçleri; değiştirilemez kayıtlar (yeniden eskiye).
 * "Dışa aktar" yalnız Yönetici (platform.audit.export); diğer rollerde kilitli.
 */
export function AuditPage({ filters }: { filters: AuditFilters }) {
  const api = useApi();
  const clock = useClock();
  const toast = useToast();
  const navigate = useNavigate();
  const today = branchToday(clock, platformTimeZone);
  const hours = auditPeriods.find((p) => p.value === filters.period)?.hours ?? 24;
  // Alt sınır her çizimde "şimdi"den hesaplanır; sorgu anahtarında yok (yalnız süzgeç değişince yeniden çekilir).
  const fromUtc = new Date(clock.now().getTime() - hours * 60 * 60 * 1000).toISOString();
  const query = useInfiniteQuery({
    ...auditLogQuery(api, filters, fromUtc),
    placeholderData: keepPreviousData,
  });
  const rows = query.data?.pages.flatMap((page) => page.items);
  const [draft, setDraft] = useState(filters.q);
  const [exporting, setExporting] = useState(false);
  const brandName = filters.brandId
    ? rows?.find((row) => row.brand?.id === filters.brandId)?.brand?.name
    : undefined;

  const go = (patch: Partial<AuditFilters>) => {
    const next = { ...filters, ...patch };
    void navigate({
      to: '/denetim',
      search: {
        ...(next.category === 'All' ? {} : { kategori: next.category }),
        ...(next.period === '24s' ? {} : { zaman: next.period }),
        ...(next.q.trim().length >= 2 ? { q: next.q.trim() } : {}),
        ...(next.brandId ? { marka: next.brandId } : {}),
      },
    });
  };

  const exportCsv = async () => {
    setExporting(true);
    try {
      const { data, error, response } = await api.GET('/api/v1/admin/audit-log/export', {
        params: { query: filterQuery(filters, fromUtc) },
        parseAs: 'blob',
      });
      if (!response.ok || error !== undefined) {
        throw new ApiProblem(response.status, error);
      }

      downloadFile(data, dispositionFileName(response) ?? `denetim-kaydi-${today}.csv`);
    } catch (problem) {
      toast.show({
        tone: 'hata',
        title: 'Dışa aktarılamadı',
        message: errorMessage(problem instanceof ApiProblem ? problem.code : undefined),
      });
    } finally {
      setExporting(false);
    }
  };

  return (
    <Page
      title="Denetim kaydı"
      context="Tüm platform işlemleri · silinemez"
      action={
        <PermissionButton
          permission="platform.audit.export"
          icon={<FileText size={16} aria-hidden="true" />}
          loading={exporting}
          loadingLabel="Hazırlanıyor…"
          onClick={() => void exportCsv()}
        >
          Dışa aktar
        </PermissionButton>
      }
    >
      <div className={styles.toolbar}>
        <FilterChips
          legend="Denetim kategorisi"
          name="denetim-kategorisi"
          options={auditCategories}
          value={filters.category}
          onChange={(category) => {
            go({ category });
          }}
        />
        <div className={styles.controls}>
          <Select
            label="Zaman"
            hideLabel
            className={styles.period}
            options={auditPeriods.map(({ value, label }) => ({ value, label }))}
            value={filters.period}
            onChange={(event) => {
              go({ period: event.target.value as AuditPeriod });
            }}
          />
          <form
            role="search"
            className={approvalStyles.search}
            onSubmit={(event) => {
              event.preventDefault();
              go({ q: draft });
            }}
          >
            <Search size={16} aria-hidden="true" />
            <input
              type="search"
              value={draft}
              placeholder="Marka veya kişi ara"
              aria-label="Marka veya kişi ara"
              onChange={(event) => {
                setDraft(event.target.value);
              }}
            />
          </form>
        </div>
      </div>
      {filters.brandId ? (
        <p className={brandStyles.searchNote}>
          {brandName ? `${brandName} kayıtları` : 'Tek markanın kayıtları'} ·{' '}
          <button
            type="button"
            className={brandStyles.textButton}
            onClick={() => {
              go({ brandId: '' });
            }}
          >
            Marka süzgecini kaldır
          </button>
        </p>
      ) : null}
      <QueryState query={{ ...query, data: rows }}>
        {(items) => (
          <DataTable
            caption="Denetim kaydı"
            rows={items}
            rowKey={(entry) => String(entry.id)}
            columns={[
              {
                key: 'time',
                label: 'Zaman',
                render: (entry) => whenText(entry.occurredUtc, today),
              },
              { key: 'actor', label: 'Kim', render: (entry) => <Actor entry={entry} /> },
              { key: 'action', label: 'İşlem', render: auditActionLabel },
              {
                key: 'brand',
                label: 'Marka',
                render: (entry) =>
                  entry.brand ? (
                    <button
                      type="button"
                      className={styles.brandButton}
                      aria-label={`Yalnız ${entry.brand.name} kayıtlarını göster`}
                      onClick={() => {
                        go({ brandId: entry.brand?.id ?? '' });
                      }}
                    >
                      {entry.brand.name}
                    </button>
                  ) : (
                    '—'
                  ),
              },
              {
                key: 'detail',
                label: 'Ayrıntı',
                render: (entry) => <span className={styles.detail}>{auditDetail(entry)}</span>,
              },
            ]}
            empty={
              <EmptyState
                icon={filters.q ? <Search size={22} /> : <History size={22} />}
                title="Bu süzgece uyan kayıt yok"
                description="Zaman aralığını genişletin veya başka bir kategori seçin."
              />
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
        Kayıtlar saklama süresi boyunca tutulur ve değiştirilemez. Telefon ve ad bilgisi kayıtlarda
        maskelenir.
      </p>
    </Page>
  );
}

function Actor({ entry }: { entry: AuditEntry }) {
  const { name, role } = auditActor(entry.actor);
  return (
    <span className={styles.actor}>
      <Avatar name={name} size="sm" />
      <span className={styles.actorText}>
        <strong>{name}</strong>
        <span>{role}</span>
      </span>
    </span>
  );
}
