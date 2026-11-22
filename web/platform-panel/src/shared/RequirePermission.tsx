import type { ReactNode } from 'react';
import type { AdminPermission } from '@/app/api';
import { useSession } from '@/app/session';
import { ForbiddenPage } from './ForbiddenPage';

/**
 * Rotanın izni (menü öğesiyle aynı): adres elle yazılsa da rolün göremediği sayfa açılmaz. Gizlemek yetkilendirme
 * değildir; uç ayrıca 403 döner.
 */
export function RequirePermission({
  permission,
  children,
}: {
  permission: AdminPermission;
  children: ReactNode;
}) {
  const { can } = useSession();
  return can(permission) ? children : <ForbiddenPage />;
}
