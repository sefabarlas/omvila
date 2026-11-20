import { ApiProblem, unwrap } from '@omvila/api-client-ts';
import { queryOptions, useSuspenseQuery } from '@tanstack/react-query';
import type { Api, ManageSession } from './api';
import { useApi } from './api-context';
import { queryKeys } from './query-keys';

/** Panel açılışında bir kez (manageGetSession); menü ve butonlar izin listesine göre kurulur. */
export function sessionQuery(api: Api) {
  return queryOptions({
    queryKey: queryKeys.session(),
    queryFn: () => unwrap(api.GET('/api/v1/manage/session')),
    staleTime: 5 * 60 * 1000,
    retry: (count, error) => !(error instanceof ApiProblem && error.status < 500) && count < 2,
  });
}

export interface SessionApi {
  session: ManageSession;
  /** İzin var mı (roller-yetkiler.md; kapsam sunucuda uygulanır). */
  can: (permission: string) => boolean;
}

export function toSessionApi(session: ManageSession): SessionApi {
  const permissions = new Set<string>(session.permissions);
  return { session, can: (permission) => permissions.has(permission) };
}

/** Oturum (panel düzeni içinde; beforeLoad oturumu yüklemiştir). */
export function useSession(): SessionApi {
  const { data } = useSuspenseQuery(sessionQuery(useApi()));
  return toSessionApi(data);
}
