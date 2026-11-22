import { ifMatch, unwrap } from '@omvila/api-client-ts';
import type { components } from '@omvila/api-client-ts';
import { queryOptions, useMutation, useQueryClient } from '@tanstack/react-query';
import type { Api } from '@/app/api';
import { useApi } from '@/app/api-context';
import { queryKeys } from '@/app/query-keys';

export type PrivacySettings = components['schemas']['PrivacySettings'];
export type PrivacySettingsUpdate = components['schemas']['PrivacySettingsUpdate'];
export type DataExportResult = components['schemas']['DataExportRequestResult'];

/** Aydınlatma metni ve saklama süresi (manageGetPrivacySettings; settings.manage). */
export function privacySettingsQuery(api: Api) {
  return queryOptions({
    queryKey: queryKeys.privacySettings(),
    queryFn: () => unwrap(api.GET('/api/v1/manage/brand/privacy')),
  });
}

/** "Değişiklikleri kaydet" (manageUpdatePrivacySettings). */
export function useUpdatePrivacySettings() {
  const api = useApi();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ version, body }: { version: string; body: PrivacySettingsUpdate }) =>
      unwrap(
        api.PUT('/api/v1/manage/brand/privacy', { params: { header: ifMatch(version) }, body }),
      ),
    onSuccess: (settings) => {
      queryClient.setQueryData(queryKeys.privacySettings(), settings);
    },
  });
}

/**
 * "Dışa aktarma iste" (manageRequestDataExport; customer.export). Kuyruğa alınır (202); bağlantı e-postayla gider.
 * Hazırlanmakta olan istek varsa sunucu yeni istek açmaz, onu döner.
 */
export function useRequestDataExport() {
  const api = useApi();
  return useMutation({
    mutationFn: ({ key, email }: { key: string; email: string }) =>
      unwrap(
        api.POST('/api/v1/manage/brand/data-exports', {
          params: { header: { 'Idempotency-Key': key } },
          body: { email },
        }),
      ),
  });
}

/** İsteğin durumu (DataExportRequestResult.status). */
export const dataExportStatusLabels: Record<DataExportResult['status'], string> = {
  Queued: 'Hazırlanıyor',
  Processing: 'Hazırlanıyor',
  Sent: 'Gönderildi',
  Failed: 'Hazırlanamadı',
};
