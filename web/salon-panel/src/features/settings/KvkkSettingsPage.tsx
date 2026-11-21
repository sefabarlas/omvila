import { ApiProblem } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { Badge, Button, DataTable, EmptyState, Skeleton } from '@omvila/ui-web/panel';
import type { BadgeTone } from '@omvila/ui-web/panel';
import { branchToday, daysBetween, toBranchDate } from '@omvila/ui-web/time';
import { useInfiniteQuery } from '@tanstack/react-query';
import { Link } from '@tanstack/react-router';
import { ShieldCheck } from 'lucide-react';
import { useApi } from '@/app/api-context';
import { useClock } from '@/app/clock';
import { defaultTimeZone } from '@/app/config';
import type { DataDeletionRequestView } from '@/features/customers/api';
import { shortDate } from '@/features/customers/labels';
import { deletionRequestsQuery } from '@/features/customers/records-api';
import kvkk from './Kvkk.module.css';
import styles from './Settings.module.css';
import { SettingsLayout } from './SettingsLayout';

const via: Record<DataDeletionRequestView['requestedVia'], string> = {
  CustomerApp: 'Uygulamadan hesap silme',
  Panel: 'Panelden anonimleştirme',
};

/** Durum rozeti: "Bekliyor · 28 gün" (son güne kalan), süresi geçtiyse "Gecikti", "Tamamlandı", "Reddedildi". */
export function deletionStatus(
  request: Pick<DataDeletionRequestView, 'status' | 'dueOn'>,
  today: string,
): { label: string; tone: BadgeTone } {
  switch (request.status) {
    case 'Pending': {
      const left = daysBetween(today, request.dueOn);
      return left < 0
        ? { label: `Gecikti · ${String(-left)} gün`, tone: 'hata' }
        : { label: `Bekliyor · ${String(left)} gün`, tone: 'uyari' };
    }

    case 'Completed':
      return { label: 'Tamamlandı', tone: 'basari' };
    default:
      return { label: 'Reddedildi', tone: 'notr' };
  }
}

/**
 * Ayarlar → KVKK ve veri (salon-paneli/AyarlarKvkk). Dilim 5'te yalnız "Silme talepleri" tablosu
 * (manageListDeletionRequests); aydınlatma metni, veri saklama ve "Tüm verileri dışa aktar" Dilim 6'da (T-86 (7),
 * T-99). Yanıt süresi notu talebin kendi son gününden okunur (DeletionRequestDueDays). customer.kvkk (yalnız sahip).
 */
export function KvkkSettingsPage() {
  const api = useApi();
  const clock = useClock();
  const today = branchToday(clock, defaultTimeZone);
  const query = useInfiniteQuery(deletionRequestsQuery(api));
  const rows = query.data?.pages.flatMap((page) => page.items) ?? [];
  const first = rows[0];
  const dueDays = first
    ? daysBetween(toBranchDate(new Date(first.requestedUtc), defaultTimeZone), first.dueOn)
    : null;

  return (
    <SettingsLayout context="Kişisel verilerin korunması">
      <section className={kvkk.tableCard} aria-labelledby="silme-talepleri">
        <div className={kvkk.tableHead}>
          <h2 id="silme-talepleri" className={styles.cardTitle}>
            Silme talepleri
          </h2>
          {dueDays !== null && dueDays > 0 ? (
            <span className={styles.cardText}>{`${String(dueDays)} gün içinde yanıtlanmalı`}</span>
          ) : null}
        </div>
        {query.isPending ? (
          <Skeleton variant="block" height="160px" />
        ) : query.isError ? (
          <EmptyState
            headingLevel={3}
            title="Silme talepleri yüklenemedi"
            description={errorMessage(
              query.error instanceof ApiProblem ? query.error.code : undefined,
            )}
            actions={<Button onClick={() => void query.refetch()}>Tekrar dene</Button>}
          />
        ) : (
          <DataTable<DataDeletionRequestView>
            caption="Silme talepleri"
            rows={rows}
            rowKey={(row) => row.id}
            empty={
              <EmptyState
                icon={<ShieldCheck size={24} />}
                headingLevel={3}
                title="Silme talebi yok"
                description="Müşteri uygulamadan hesabını silmek isterse veya panelden bir kaydı anonimleştirirseniz burada görünür."
              />
            }
            columns={[
              {
                key: 'customer',
                label: 'Müşteri',
                render: (row) =>
                  row.status === 'Pending' && row.brandCustomerId ? (
                    <Link
                      to="/musteriler/$id"
                      params={{ id: row.brandCustomerId }}
                      className={kvkk.strongLink}
                    >
                      {row.customerDisplayName}
                    </Link>
                  ) : (
                    <strong>{row.customerDisplayName}</strong>
                  ),
              },
              { key: 'via', label: 'Talep', render: (row) => via[row.requestedVia] },
              {
                key: 'date',
                label: 'Tarih',
                render: (row) =>
                  shortDate(toBranchDate(new Date(row.requestedUtc), defaultTimeZone)),
              },
              {
                key: 'status',
                label: 'Durum',
                render: (row) => {
                  const status = deletionStatus(row, today);
                  return <Badge tone={status.tone}>{status.label}</Badge>;
                },
              },
            ]}
            {...(query.hasNextPage
              ? {
                  footer: (
                    <Button
                      size="sm"
                      loading={query.isFetchingNextPage}
                      loadingLabel="Yükleniyor…"
                      onClick={() => void query.fetchNextPage()}
                    >
                      Daha fazla göster
                    </Button>
                  ),
                }
              : {})}
          />
        )}
      </section>
    </SettingsLayout>
  );
}
