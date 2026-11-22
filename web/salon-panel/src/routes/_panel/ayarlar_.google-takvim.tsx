import { createFileRoute } from '@tanstack/react-router';
import { GoogleCalendarReturn } from '@/features/staff/GoogleCalendarReturn';
import type { GoogleCalendarReturnSearch } from '@/features/staff/GoogleCalendarReturn';
import { RequirePermission } from '@/shared/RequirePermission';

const text = (value: unknown) => (typeof value === 'string' && value !== '' ? value : undefined);

// Google Takvim geri dönüş adresi (sunucu yapılandırması …/ayarlar/google-takvim; K7). Uçlar calendar.view ister.
export const Route = createFileRoute('/_panel/ayarlar_/google-takvim')({
  validateSearch: (search: Record<string, unknown>): GoogleCalendarReturnSearch => {
    const code = text(search.code);
    const state = text(search.state);
    return { ...(code ? { code } : {}), ...(state ? { state } : {}) };
  },
  component: GoogleCalendarRoute,
});

function GoogleCalendarRoute() {
  const search = Route.useSearch();
  return (
    <RequirePermission permission="calendar.view">
      <GoogleCalendarReturn {...search} />
    </RequirePermission>
  );
}
