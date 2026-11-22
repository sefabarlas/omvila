import { ApiProblem, unwrap } from '@omvila/api-client-ts';
import { queryOptions, useSuspenseQuery } from '@tanstack/react-query';
import type { AdminPermission, AdminSession, Api } from './api';
import { useApi } from './api-context';
import { queryKeys } from './query-keys';

/** Panel açılışında bir kez (adminGetSession); menü ve butonlar izin listesine göre kurulur. */
export function sessionQuery(api: Api) {
  return queryOptions({
    queryKey: queryKeys.session(),
    queryFn: () => unwrap(api.GET('/api/v1/admin/session')),
    staleTime: 5 * 60 * 1000,
    retry: (count, error) => !(error instanceof ApiProblem && error.status < 500) && count < 2,
  });
}

export interface SessionApi {
  session: AdminSession;
  /** İzin var mı (roller-yetkiler.md § Platform rolleri); gizlemek yetkilendirme değildir, uç ayrıca denetler. */
  can: (permission: AdminPermission) => boolean;
}

export function toSessionApi(session: AdminSession): SessionApi {
  const permissions = new Set<AdminPermission>(session.permissions);
  return { session, can: (permission) => permissions.has(permission) };
}

/** Oturum (panel düzeni içinde; beforeLoad oturumu yüklemiştir). */
export function useSession(): SessionApi {
  const { data } = useSuspenseQuery(sessionQuery(useApi()));
  return toSessionApi(data);
}

export const roleLabels: Record<AdminSession['role'], string> = {
  PlatformAdmin: 'Yönetici',
  PlatformSupport: 'Destek',
  PlatformFinance: 'Finans',
};
