import { ApiProblem } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { formatDate, formatMoney } from '@omvila/ui-web/format';
import {
  Badge,
  BentoBlock,
  Button,
  DataTable,
  EmptyState,
  Skeleton,
  useToast,
} from '@omvila/ui-web/panel';
import { toBranchDate } from '@omvila/ui-web/time';
import { useInfiniteQuery, useQuery } from '@tanstack/react-query';
import { FileText, ReceiptText, Upload } from 'lucide-react';
import { useState } from 'react';
import { useApi } from '@/app/api-context';
import { defaultTimeZone } from '@/app/config';
import { SubscriptionBanner } from '@/app/SubscriptionBanner';
import { downloadFile } from '@/features/customers/records-api';
import settingsStyles from '@/features/settings/Settings.module.css';
import { SettingsLayout } from '@/features/settings/SettingsLayout';
import { fetchInvoicePdf, invoicesQuery, subscriptionQuery } from './api';
import type { SubscriptionDetail, SubscriptionInvoice } from './api';
import { branchDetail, invoicePeriod, invoiceStatus, planDetail, staffDetail } from './labels';
import { NotifyTransferModal } from './NotifyTransferForm';
import styles from './Subscription.module.css';
import { TransferDetails } from './TransferDetails';

/**
 * Ayarlar → Abonelik ve fatura (salon-paneli/AyarlarAbonelik; subscription.manage, yalnız Owner). Kartla ödeme (T-06)
 * olmadığı için "Kartla öde" bölümü ve üst bardaki "Ödeme yöntemini kaydet" yoktur; ödeme havaleyle (T-125).
 */
export function SubscriptionPage() {
  const api = useApi();
  const query = useQuery(subscriptionQuery(api));

  return (
    <SettingsLayout context="OMVILA aboneliği" hideSubscriptionBanner>
      {query.isPending ? (
        <Skeleton variant="block" height="320px" />
      ) : query.isError ? (
        <EmptyState
          title="Abonelik bilgisi yüklenemedi"
          description={errorMessage(
            query.error instanceof ApiProblem ? query.error.code : undefined,
          )}
          actions={<Button onClick={() => void query.refetch()}>Tekrar dene</Button>}
        />
      ) : (
        <SubscriptionContent detail={query.data} />
      )}
    </SettingsLayout>
  );
}

function SubscriptionContent({ detail }: { detail: SubscriptionDetail }) {
  const [notifying, setNotifying] = useState(false);
  const staffSub = staffDetail(detail.staff);
  const pending = detail.pendingNotice;

  return (
    <>
      <SubscriptionBanner
        subscription={detail}
        timeZone={defaultTimeZone}
        manages
        canPay
        onSubscriptionPage
      />
      <div className={styles.cards}>
        <BentoBlock label="Plan" value={detail.plan.name} sub={planDetail(detail)} tone="marka" />
        <BentoBlock
          label="Şube"
          value={String(detail.branches.count)}
          sub={branchDetail(detail.branches)}
        />
        <BentoBlock
          label="Personel"
          value={String(detail.staff.count)}
          {...(staffSub ? { sub: staffSub } : {})}
        />
      </div>
      <InvoicesCard />
      <section className={settingsStyles.card} aria-labelledby="havale-ile-ode">
        <div className={settingsStyles.cardHead}>
          <h2 id="havale-ile-ode" className={settingsStyles.cardTitle}>
            Havale / EFT ile öde
          </h2>
        </div>
        <TransferDetails
          account={detail.omvilaBankAccount}
          transferCode={detail.transferCode}
          amount={detail.amountDue}
        />
        {pending ? (
          <p className={styles.pending} role="status">
            <Badge tone="bilgi">Eşleştirme bekliyor</Badge>
            {`Havale ${formatDate(toBranchDate(new Date(pending.notifiedUtc), defaultTimeZone), 'dayMonth')} tarihinde bildirildi; ekibimiz banka hareketiyle eşleştirince aboneliğiniz etkinleşir.`}
          </p>
        ) : detail.omvilaBankAccount ? (
          <>
            <p className={settingsStyles.cardText}>
              Açıklamaya kodu yazmayı unutmayın. Gönderdiyseniz bildirin; ekibimiz banka hareketiyle
              eşleştirince aboneliğiniz etkinleşir.
            </p>
            <div>
              <Button
                icon={<Upload size={16} aria-hidden="true" />}
                onClick={() => {
                  setNotifying(true);
                }}
              >
                Havaleyi bildir (dekont isteğe bağlı)
              </Button>
            </div>
          </>
        ) : null}
      </section>
      <NotifyTransferModal
        open={notifying}
        onClose={() => {
          setNotifying(false);
        }}
      />
    </>
  );
}

function InvoicesCard() {
  const api = useApi();
  const toast = useToast();
  const query = useInfiniteQuery(invoicesQuery(api));
  const [downloading, setDownloading] = useState<string | null>(null);
  const rows = query.data?.pages.flatMap((page) => page.items) ?? [];

  const download = async (invoice: SubscriptionInvoice) => {
    setDownloading(invoice.id);
    try {
      const { blob, fileName } = await fetchInvoicePdf(api, invoice.id);
      downloadFile(blob, fileName ?? `${invoice.invoiceNumber}.pdf`);
    } catch (error) {
      toast.show({
        tone: 'hata',
        title: errorMessage(error instanceof ApiProblem ? error.code : undefined),
      });
    } finally {
      setDownloading(null);
    }
  };

  return (
    <section className={styles.tableCard} aria-labelledby="faturalar">
      <div className={styles.tableHead}>
        <h2 id="faturalar" className={settingsStyles.cardTitle}>
          Faturalar
        </h2>
        <span className={settingsStyles.cardText}>e-Arşiv değil, bilgi amaçlı</span>
      </div>
      {query.isPending ? (
        <Skeleton variant="block" height="120px" />
      ) : query.isError ? (
        <EmptyState
          headingLevel={3}
          title="Faturalar yüklenemedi"
          description={errorMessage(
            query.error instanceof ApiProblem ? query.error.code : undefined,
          )}
          actions={<Button onClick={() => void query.refetch()}>Tekrar dene</Button>}
        />
      ) : (
        <DataTable<SubscriptionInvoice>
          caption="Faturalar"
          rows={rows}
          rowKey={(row) => row.id}
          empty={
            <EmptyState
              icon={<ReceiptText size={24} />}
              headingLevel={3}
              title="Henüz fatura yok"
              description="Fatura kesildiğinde burada görünür ve PDF olarak indirilebilir."
            />
          }
          columns={[
            { key: 'no', label: 'No', render: (row) => row.invoiceNumber },
            { key: 'period', label: 'Dönem', render: (row) => invoicePeriod(row) },
            {
              key: 'total',
              label: 'Tutar',
              align: 'right',
              numeric: true,
              render: (row) => <strong>{formatMoney(row.total)}</strong>,
            },
            {
              key: 'status',
              label: 'Durum',
              render: (row) => (
                <Badge tone={invoiceStatus[row.status].tone}>
                  {invoiceStatus[row.status].label}
                </Badge>
              ),
            },
            {
              key: 'pdf',
              label: 'PDF',
              align: 'right',
              render: (row) => (
                <Button
                  size="sm"
                  variant="ghost"
                  icon={<FileText size={14} aria-hidden="true" />}
                  aria-label={`${row.invoiceNumber} PDF indir`}
                  loading={downloading === row.id}
                  loadingLabel="İndiriliyor…"
                  onClick={() => void download(row)}
                >
                  PDF
                </Button>
              ),
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
  );
}
