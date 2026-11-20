import type { ReactNode } from 'react';
import { useSession } from '@/app/session';
import { ForbiddenPage } from './ForbiddenPage';

/** İzni olmayan kullanıcıya sayfa yerine 403 (menüde zaten görünmez; doğrudan adres girilirse). */
export function RequirePermission({
  permission,
  children,
}: {
  permission: string;
  children: ReactNode;
}) {
  const { can } = useSession();
  return can(permission) ? children : <ForbiddenPage />;
}
