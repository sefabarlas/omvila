import { createFileRoute } from '@tanstack/react-router';
import { WalkInsPage } from '@/features/walk-ins/WalkInsPage';
import { RequirePermission } from '@/shared/RequirePermission';

// Kapıda sıra (walkin.manage, personel dahil): Takvim altında alt rota, takvim düzenine gömülmez (menüde Takvim seçili).
function WalkInsRoute() {
  return (
    <RequirePermission permission="walkin.manage">
      <WalkInsPage />
    </RequirePermission>
  );
}

export const Route = createFileRoute('/_panel/takvim_/kapida-sira')({ component: WalkInsRoute });
