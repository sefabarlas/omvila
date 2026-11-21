import { unwrap } from '@omvila/api-client-ts';
import type { components } from '@omvila/api-client-ts';
import { queryOptions, useMutation, useQueryClient } from '@tanstack/react-query';
import type { Api } from '@/app/api';
import { useApi } from '@/app/api-context';
import { useClock } from '@/app/clock';
import { queryKeys } from '@/app/query-keys';

export type InAppNotification = components['schemas']['InAppNotification'];
export type InAppNotificationPage = components['schemas']['InAppNotificationPage'];

/** Paneldeki satır sayısı (ilk sayfa; T-69). */
const pageSize = 20;
/** Zildeki sayının yenilenme aralığı (teknik; T-69). */
const refreshMs = 60 * 1000;

/**
 * Bildirim paneli (manageListInAppNotifications): ilk sayfa ve zildeki okunmamış sayısı (unreadCount). Sayı yalnız
 * sunucudan gelir; sorgu başarısızsa zilde rozet yoktur.
 */
export function notificationsQuery(api: Api) {
  return queryOptions({
    queryKey: queryKeys.notifications(),
    queryFn: () =>
      unwrap(api.GET('/api/v1/manage/notifications', { params: { query: { limit: pageSize } } })),
    staleTime: refreshMs / 2,
    refetchInterval: refreshMs,
  });
}

/**
 * Okundu say (manageMarkNotificationsRead): tıklanan bildirim (`ids`) ya da "Tümünü okundu say" (`all`). Yanıttaki
 * güncel okunmamış sayısı zile yazılır; okunan satırlar listede okunmuş görünür.
 */
export function useMarkNotificationsRead() {
  const api = useApi();
  const clock = useClock();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (target: { ids: string[] } | { all: true }) =>
      unwrap(
        api.POST('/api/v1/manage/notifications/read', {
          body: 'all' in target ? { all: true } : { notificationIds: target.ids },
        }),
      ),
    onSuccess: ({ unreadCount }, target) => {
      const readUtc = clock.now().toISOString();
      queryClient.setQueryData<InAppNotificationPage>(queryKeys.notifications(), (page) =>
        page
          ? {
              ...page,
              unreadCount,
              items: page.items.map((item) =>
                item.readUtc === null && ('all' in target || target.ids.includes(item.id))
                  ? { ...item, readUtc }
                  : item,
              ),
            }
          : page,
      );
    },
  });
}
