import { createFileRoute } from '@tanstack/react-router';
import { useSession } from '@/app/session';
import { PackagesPage } from '@/features/packages/PackagesPage';
import { ForbiddenPage } from '@/shared/ForbiddenPage';

// Paketler (roller-yetkiler.md § Menü: package.manage veya package.sell). Tanım yazma yalnız package.manage.
function PackagesRoute() {
  const { can } = useSession();
  return can('package.manage') || can('package.sell') ? <PackagesPage /> : <ForbiddenPage />;
}

export const Route = createFileRoute('/_panel/paketler')({
  component: PackagesRoute,
});
