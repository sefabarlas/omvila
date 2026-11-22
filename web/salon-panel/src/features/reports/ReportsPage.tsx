import { ApiProblem } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { formatMoney } from '@omvila/ui-web/format';
import { Bento, BentoBlock, Button, EmptyState, Skeleton } from '@omvila/ui-web/panel';
import { branchToday } from '@omvila/ui-web/time';
import { keepPreviousData, useQuery } from '@tanstack/react-query';
import { useNavigate } from '@tanstack/react-router';
import { ChartColumn, Settings } from 'lucide-react';
import { useApi } from '@/app/api-context';
import { useSelectedBranch } from '@/app/branch';
import { useClock } from '@/app/clock';
import { defaultTimeZone } from '@/app/config';
import { useSession } from '@/app/session';
import { Page } from '@/shared/Page';
import { Segmented } from '@/shared/Segmented';
import { reportOverviewQuery } from './api';
import type { ReportOverview } from './api';
import { BarChart } from './BarChart';
import { CommissionTable } from './CommissionTable';
import {
  closedDaysLabel,
  daysBetweenInclusive,
  formatChange,
  formatPercent,
  periodLabel,
  periodOptions,
  periodRange,
} from './period';
import type { ReportPeriod } from './period';
import styles from './Reports.module.css';

/** Hiç veri yok (RaporlarBos): tutarlar 0, oranlar null döner. */
export function isEmptyReport(report: ReportOverview): boolean {
  return (
    report.earnedRevenue.amountMinor === 0 &&
    report.collections.amountMinor === 0 &&
    report.completedAppointmentCount === 0 &&
    report.noShow.count === 0 &&
    report.online.appointmentCount === 0 &&
    report.dailyCollections.every((day) => day.amountMinor === 0)
  );
}

/**
 * Raporlar ve prim (salon-paneli/Raporlar, RaporlarBos; dilim-06 KK6): Gün / Hafta / Ay ve kenar menüdeki şube; kartlar
 * (manageGetReportOverview), "Günlük tahsilat" grafiği ve "Personel ve prim" tablosu (manageGetCommissionReport,
 * commission.view). Boş hâlde dolu hâlin kart başlıkları "—" ile (T-122). "Dışa aktar" sözleşmede yok, gösterilmez.
 */
export function ReportsPage({ period }: { period: ReportPeriod }) {
  const api = useApi();
  const clock = useClock();
  const navigate = useNavigate();
  const { session, can } = useSession();
  const { branchId } = useSelectedBranch();
  const activeBranch = branchId ?? session.branches[0]?.id ?? '';
  const branchName = session.branches.find((b) => b.id === activeBranch)?.name ?? '';
  const today = branchToday(clock, defaultTimeZone);
  const range = periodRange(period, today);
  const query = useQuery({
    ...reportOverviewQuery(api, activeBranch, range.from, range.to),
    placeholderData: keepPreviousData,
    enabled: activeBranch !== '',
  });
  const report = query.data;
  const empty = report ? isEmptyReport(report) : false;
  const canEditCommission = can('staff.manage');

  return (
    <Page
      title="Raporlar ve prim"
      context={[branchName, periodLabel(range.from, range.to)].filter(Boolean).join(' · ')}
      action={
        <Segmented
          legend="Dönem"
          name="donem"
          options={periodOptions}
          value={period}
          onChange={(value) => {
            void navigate({ to: '/raporlar', search: value === 'hafta' ? {} : { donem: value } });
          }}
        />
      }
    >
      {query.isPending ? (
        <>
          <Skeleton variant="block" height="120px" />
          <Skeleton variant="block" height="320px" />
        </>
      ) : query.isError && !report ? (
        <section className={styles.card} aria-label="Hata">
          <EmptyState
            headingLevel={2}
            title="Rapor yüklenemedi"
            description={errorMessage(
              query.error instanceof ApiProblem ? query.error.code : undefined,
            )}
            actions={<Button onClick={() => void query.refetch()}>Tekrar dene</Button>}
          />
        </section>
      ) : report ? (
        <>
          <ReportCards report={report} empty={empty} />
          {empty ? (
            <section className={styles.card} aria-label="Rapor yok">
              <EmptyState
                icon={<ChartColumn size={24} />}
                headingLevel={2}
                title="Rapor için henüz veri yok"
                description="İlk randevular tamamlandıkça gelir, tahsilat, gelmeyenler ve prim burada oluşur. Prim, randevu tamamlandığında hesaplanır."
                actions={
                  canEditCommission ? (
                    <Button
                      icon={<Settings size={16} aria-hidden="true" />}
                      onClick={() => void navigate({ to: '/personel' })}
                    >
                      Prim kurallarını ayarla
                    </Button>
                  ) : null
                }
              />
            </section>
          ) : (
            <div className={can('commission.view') ? styles.grid : undefined}>
              <section className={styles.card} aria-labelledby="gunluk-tahsilat">
                <div className={styles.cardHead}>
                  <h2 id="gunluk-tahsilat" className={styles.cardTitle}>
                    Günlük tahsilat
                  </h2>
                  <span className={styles.muted}>
                    {closedDaysLabel(
                      report.dailyCollections.filter((d) => d.isClosed).map((d) => d.date),
                    )}
                  </span>
                </div>
                <BarChart
                  caption="Günlük tahsilat"
                  valueHeading="Tahsilat"
                  currency={report.currency}
                  axis
                  bars={daysBetweenInclusive(range.from, range.end).map((day) => {
                    const row = report.dailyCollections.find((d) => d.date === day);
                    return row
                      ? { day, amountMinor: row.amountMinor, closed: row.isClosed }
                      : { day, amountMinor: 0, future: day > range.to };
                  })}
                />
              </section>
              {can('commission.view') ? (
                <CommissionTable
                  branchId={activeBranch}
                  from={range.from}
                  to={range.to}
                  canEditCommission={canEditCommission}
                />
              ) : null}
            </div>
          )}
        </>
      ) : null}
    </Page>
  );
}

function ReportCards({ report, empty }: { report: ReportOverview; empty: boolean }) {
  if (empty) {
    return (
      <Bento label="Özet">
        <BentoBlock label="Hak edilen gelir" value="—" sub="tamamlanan randevu yok" />
        <BentoBlock label="Tahsilat" value="—" sub="kayıt yok" />
        <BentoBlock label="Gelmedi" value="—" />
        <BentoBlock label="Online pay" value="—" />
      </Bento>
    );
  }

  const { noShow, online } = report;
  const change =
    report.earnedRevenueChangePercent === null
      ? ''
      : ` · ${formatChange(report.earnedRevenueChangePercent)}`;
  const noShowSub = [
    noShow.percent === null ? null : formatPercent(noShow.percent),
    noShow.lostRevenue.amountMinor > 0 ? `${formatMoney(noShow.lostRevenue)} kayıp` : null,
  ]
    .filter(Boolean)
    .join(' · ');
  return (
    <Bento label="Özet">
      <BentoBlock
        tone="marka"
        label="Hak edilen gelir"
        value={formatMoney(report.earnedRevenue)}
        sub={`tamamlanan ${String(report.completedAppointmentCount)} randevu${change}`}
      />
      <BentoBlock
        label="Tahsilat"
        value={formatMoney(report.collections)}
        sub="paket satışları ve kaporalar dahil"
      />
      <BentoBlock
        tone="vurgu"
        label="Gelmedi"
        value={String(noShow.count)}
        {...(noShowSub ? { sub: noShowSub } : {})}
      />
      <BentoBlock
        tone="kum"
        label="Online pay"
        value={online.percent === null ? '—' : formatPercent(online.percent)}
        sub={`${String(online.appointmentCount)} randevu online`}
      />
    </Bento>
  );
}
