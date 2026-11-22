import { createFileRoute } from '@tanstack/react-router';
import type { ReportPeriod } from '@/features/reports/period';
import { ReportsPage } from '@/features/reports/ReportsPage';
import { RequirePermission } from '@/shared/RequirePermission';

interface ReportsSearch {
  /** Gün / Ay; yoksa Hafta (salon-paneli/Raporlar). */
  donem?: Exclude<ReportPeriod, 'hafta'>;
}

// Raporlar ve prim (report.view; roller-yetkiler.md § Menü): dönem adreste (?donem=gun|ay).
function ReportsRoute() {
  const { donem } = Route.useSearch();
  return (
    <RequirePermission permission="report.view">
      <ReportsPage period={donem ?? 'hafta'} />
    </RequirePermission>
  );
}

export const Route = createFileRoute('/_panel/raporlar')({
  validateSearch: (search: Record<string, unknown>): ReportsSearch =>
    search.donem === 'gun' || search.donem === 'ay' ? { donem: search.donem } : {},
  component: ReportsRoute,
});
