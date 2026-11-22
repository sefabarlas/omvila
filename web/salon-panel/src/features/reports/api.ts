import { unwrap } from '@omvila/api-client-ts';
import type { components } from '@omvila/api-client-ts';
import { queryOptions } from '@tanstack/react-query';
import type { Api } from '@/app/api';
import { queryKeys } from '@/app/query-keys';

export type ReportOverview = components['schemas']['ReportOverview'];
export type CommissionReport = components['schemas']['CommissionReport'];
export type CommissionReportRow = components['schemas']['CommissionReportRow'];
export type MyCommissionSummary = components['schemas']['MyCommissionSummary'];
export type MyCommissionEntry = components['schemas']['MyCommissionEntry'];

/** Raporlar üst kartları ve "Günlük tahsilat" (manageGetReportOverview; report.view). Aralık iki uç dahil, ≤ 31 gün. */
export function reportOverviewQuery(api: Api, branchId: string, from: string, to: string) {
  return queryOptions({
    queryKey: queryKeys.reportOverview(branchId, from, to),
    queryFn: () =>
      unwrap(
        api.GET('/api/v1/manage/reports/overview', { params: { query: { branchId, from, to } } }),
      ),
  });
}

/** "Personel ve prim" tablosu (manageGetCommissionReport; commission.view). */
export function commissionReportQuery(api: Api, branchId: string, from: string, to: string) {
  return queryOptions({
    queryKey: queryKeys.commissionReport(branchId, from, to),
    queryFn: () =>
      unwrap(
        api.GET('/api/v1/manage/reports/commissions', {
          params: { query: { branchId, from, to } },
        }),
      ),
  });
}

/** Primim (manageGetMyCommissionSummary): oturumdaki kullanıcının kendi primi; personel kaydı yoksa 404. */
export function myCommissionQuery(api: Api, month: string) {
  return queryOptions({
    queryKey: queryKeys.myCommission(month),
    queryFn: () =>
      unwrap(api.GET('/api/v1/manage/me/commission-summary', { params: { query: { month } } })),
  });
}
