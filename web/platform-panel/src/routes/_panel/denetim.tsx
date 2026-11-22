import { createFileRoute } from '@tanstack/react-router';
import { AuditPage, auditCategories, auditPeriods } from '@/features/audit/AuditPage';
import type { AuditCategory, AuditPeriod } from '@/features/audit/AuditPage';
import { RequirePermission } from '@/shared/RequirePermission';

interface AuditSearch {
  kategori?: AuditCategory;
  zaman?: AuditPeriod;
  q?: string;
  marka?: string;
}

const uuid = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;

function Audit() {
  const { kategori = 'All', zaman = '24s', q = '', marka = '' } = Route.useSearch();
  return (
    <RequirePermission permission="platform.audit.view">
      <AuditPage filters={{ category: kategori, period: zaman, q, brandId: marka }} />
    </RequirePermission>
  );
}

export const Route = createFileRoute('/_panel/denetim')({
  validateSearch: (search: Record<string, unknown>): AuditSearch => ({
    ...(auditCategories.some((c) => c.value === search.kategori)
      ? { kategori: search.kategori as AuditCategory }
      : {}),
    ...(auditPeriods.some((p) => p.value === search.zaman)
      ? { zaman: search.zaman as AuditPeriod }
      : {}),
    ...(typeof search.q === 'string' && search.q.trim() !== '' ? { q: search.q.trim() } : {}),
    ...(typeof search.marka === 'string' && uuid.test(search.marka) ? { marka: search.marka } : {}),
  }),
  component: Audit,
});
