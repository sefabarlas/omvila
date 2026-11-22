import { unwrap } from '@omvila/api-client-ts';
import type { components } from '@omvila/api-client-ts';
import { queryOptions, useMutation, useQueryClient } from '@tanstack/react-query';
import type { Api } from '@/app/api';
import { useApi } from '@/app/api-context';
import { queryKeys } from '@/app/query-keys';

export type GoogleCalendarConnection = components['schemas']['GoogleCalendarConnection'];

// Google Takvim tek yönlü aktarım (K7): uçlar oturumdaki kullanıcının kendi personel kaydına bağlıdır (calendar.view).
// v1'de sunucu sahte sağlayıcıyla çalışır; izin sayfası yerine adres doğrudan panelin geri dönüş sayfasını açar.

/** Bağlantı durumu (manageGetGoogleCalendarStatus). */
export function googleCalendarQuery(api: Api) {
  return queryOptions({
    queryKey: queryKeys.googleCalendar(),
    queryFn: () => unwrap(api.GET('/api/v1/manage/integrations/google')),
  });
}

/** "Bağlan": sağlayıcının izin sayfası adresini alır ve tarayıcıyı oraya yönlendirir. */
export function useConnectGoogleCalendar(navigate: (url: string) => void) {
  const api = useApi();
  return useMutation({
    mutationFn: () =>
      unwrap(
        api.GET('/api/v1/manage/integrations/google/connect-url', {
          params: { query: { client: 'web' } },
        }),
      ),
    onSuccess: ({ url }) => {
      navigate(url);
    },
  });
}

/** Geri dönüş sayfası: sağlayıcının code ve state değerleriyle bağlantıyı tamamlar. */
export function useCompleteGoogleCalendar() {
  const api = useApi();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (body: { code: string; state: string }) =>
      unwrap(api.POST('/api/v1/manage/integrations/google/callback', { body })),
    onSuccess: async (connection) => {
      queryClient.setQueryData(queryKeys.googleCalendar(), connection);
      await queryClient.invalidateQueries({ queryKey: queryKeys.staff() });
    },
  });
}

/** "Bağlantıyı kes" (manageDisconnectGoogleCalendar); Google'a aktarılmış etkinlikler silinmez. */
export function useDisconnectGoogleCalendar() {
  const api = useApi();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: () => unwrap(api.DELETE('/api/v1/manage/integrations/google')),
    onSuccess: async () => {
      queryClient.setQueryData<GoogleCalendarConnection>(queryKeys.googleCalendar(), {
        status: 'NotConnected',
        accountEmailMasked: null,
        connectedUtc: null,
        lastSyncedUtc: null,
      });
      await queryClient.invalidateQueries({ queryKey: queryKeys.staff() });
    },
  });
}
