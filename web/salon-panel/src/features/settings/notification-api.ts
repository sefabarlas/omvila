import { ifMatch, unwrap } from '@omvila/api-client-ts';
import type { components } from '@omvila/api-client-ts';
import { queryOptions, useMutation, useQueryClient } from '@tanstack/react-query';
import type { Api } from '@/app/api';
import { useApi } from '@/app/api-context';
import { queryKeys } from '@/app/query-keys';

export type NotificationSettings = components['schemas']['NotificationSettings'];
export type NotificationSettingsUpdate = components['schemas']['NotificationSettingsUpdate'];
export type NotificationTemplateSetting = components['schemas']['NotificationTemplateSetting'];
export type SmsOverview = components['schemas']['SmsOverview'];
export type SmsSender = components['schemas']['SmsSender'];

/** Hatırlatmalar, değerlendirme daveti, mesaj şablonları (manageGetNotificationSettings). */
export function notificationSettingsQuery(api: Api) {
  return queryOptions({
    queryKey: queryKeys.notificationSettings(),
    queryFn: () => unwrap(api.GET('/api/v1/manage/brand/notification-settings')),
  });
}

/** Kontör, bu ay gönderilen, gönderen adı (manageGetSmsOverview). */
export function smsOverviewQuery(api: Api) {
  return queryOptions({
    queryKey: queryKeys.smsOverview(),
    queryFn: () => unwrap(api.GET('/api/v1/manage/brand/sms')),
  });
}

/**
 * Bildirim ayarlarını kaydeder ("Değişiklikleri kaydet"). Hatırlatma zamanları ve değerlendirme daveti BrandSettings'te
 * aynalandığı ve iki kaynağın sürümü birlikte değiştiği için randevu kuralları da yeniden okunur (acik-sorular.md T-62 (4)).
 */
export function useUpdateNotificationSettings() {
  const api = useApi();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ version, body }: { version: string; body: NotificationSettingsUpdate }) =>
      unwrap(
        api.PUT('/api/v1/manage/brand/notification-settings', {
          params: { header: ifMatch(version) },
          body,
        }),
      ),
    onSuccess: async (settings) => {
      queryClient.setQueryData(queryKeys.notificationSettings(), settings);
      await queryClient.invalidateQueries({ queryKey: queryKeys.brandSettings() });
    },
  });
}

/** Gönderen adı başvurusu (manageRequestSmsSender); yanıt SMS özetindeki gönderen kartına yazılır. */
export function useRequestSmsSender() {
  const api = useApi();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ key, name }: { key: string; name: string }) =>
      unwrap(
        api.POST('/api/v1/manage/brand/sms-sender', {
          params: { header: { 'Idempotency-Key': key } },
          body: { name },
        }),
      ),
    onSuccess: (sender) => {
      queryClient.setQueryData<SmsOverview>(queryKeys.smsOverview(), (current) =>
        current ? { ...current, sender } : current,
      );
    },
  });
}
