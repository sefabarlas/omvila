import { formatMoney } from '@omvila/ui-web/format';
import { Button, DataTable, PersonCell, Skeleton } from '@omvila/ui-web/panel';
import type { DataTableColumn } from '@omvila/ui-web/panel';
import { keepPreviousData, useQuery } from '@tanstack/react-query';
import { Link } from '@tanstack/react-router';
import { useApi } from '@/app/api-context';
import { commissionReportQuery } from './api';
import type { CommissionReportRow } from './api';
import { formatPercent } from './period';
import styles from './Reports.module.css';

/**
 * "Personel ve prim" (salon-paneli/Raporlar; manageGetCommissionReport, commission.view): Personel, Randevu, Hizmet
 * cirosu, Oran, Prim ve toplam. Tutarlar CommissionEntry'den gelir, istemcide hesaplanmaz. "Prim kuralları" personelin
 * prim oranının görüldüğü Personel ekranını açar (staff.manage; T-122).
 */
export function CommissionTable({
  branchId,
  from,
  to,
  canEditCommission,
}: {
  branchId: string;
  from: string;
  to: string;
  canEditCommission: boolean;
}) {
  const api = useApi();
  const query = useQuery({
    ...commissionReportQuery(api, branchId, from, to),
    placeholderData: keepPreviousData,
  });
  const report = query.data;
  const money = (amountMinor: number) =>
    formatMoney({ amountMinor, currency: report?.currency ?? 'TRY' });
  const columns: DataTableColumn<CommissionReportRow>[] = [
    { key: 'name', label: 'Personel', render: (row) => <PersonCell name={row.staffName} /> },
    {
      key: 'count',
      label: 'Randevu',
      numeric: true,
      render: (row) => String(row.appointmentCount),
    },
    {
      key: 'revenue',
      label: 'Hizmet cirosu',
      numeric: true,
      render: (row) => money(row.serviceRevenueMinor),
    },
    {
      key: 'rate',
      label: 'Oran',
      numeric: true,
      render: (row) =>
        row.serviceCommissionPercent === null ? '—' : formatPercent(row.serviceCommissionPercent),
    },
    {
      key: 'total',
      label: 'Prim',
      numeric: true,
      render: (row) => <strong>{money(row.totalCommissionMinor)}</strong>,
    },
  ];

  return (
    <section className={styles.tableCard} aria-labelledby="personel-ve-prim">
      <div className={styles.cardHead}>
        <h2 id="personel-ve-prim" className={styles.cardTitle}>
          Personel ve prim
        </h2>
        {canEditCommission ? (
          <Link to="/personel" className={styles.cardLink}>
            Prim kuralları
          </Link>
        ) : null}
      </div>
      {query.isPending ? (
        <Skeleton variant="block" height="200px" />
      ) : query.isError && !report ? (
        <div className={styles.cardError}>
          <p>Prim raporu yüklenemedi.</p>
          <Button size="sm" onClick={() => void query.refetch()}>
            Tekrar dene
          </Button>
        </div>
      ) : report ? (
        <DataTable
          caption="Personel ve prim"
          columns={columns}
          rows={report.items}
          rowKey={(row) => row.staffId}
          empty={<p className={styles.tableEmpty}>Bu dönemde prim kaydı yok.</p>}
          footer={
            <div className={styles.tableFoot}>
              <span>Gelmeyen ve iptal edilen randevular prime dahil değil.</span>
              <strong>{money(report.totalCommissionMinor)}</strong>
            </div>
          }
        />
      ) : null}
    </section>
  );
}
