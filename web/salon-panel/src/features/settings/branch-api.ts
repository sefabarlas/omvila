import { ifMatch, unwrap } from '@omvila/api-client-ts';
import type { components } from '@omvila/api-client-ts';
import { queryOptions, useMutation, useQueryClient } from '@tanstack/react-query';
import type { Api } from '@/app/api';
import { useApi } from '@/app/api-context';
import { queryKeys } from '@/app/query-keys';
import { brandSettingsBody } from './api';
import type { BrandSettings } from './api';
import type { WeeklyHours } from './hours';

export type Branch = components['schemas']['Branch'];
export type BranchCreate = components['schemas']['BranchCreate'];
export type WorkingHoursOverrideCreate = components['schemas']['WorkingHoursOverrideCreate'];
export type SlotInterval = NonNullable<Branch['slotIntervalMinutes']>;

const branchPath = (branchId: string) => ({ params: { path: { branchId } } });

/** Şube (manageGetBranch). */
export function branchQuery(api: Api, branchId: string) {
  return queryOptions({
    queryKey: queryKeys.branch(branchId),
    queryFn: () => unwrap(api.GET('/api/v1/manage/branches/{branchId}', branchPath(branchId))),
  });
}

/** Şubenin haftalık saatleri (manageGetBranchWorkingHours). */
export function workingHoursQuery(api: Api, branchId: string) {
  return queryOptions({
    queryKey: queryKeys.workingHours(branchId),
    queryFn: () =>
      unwrap(api.GET('/api/v1/manage/branches/{branchId}/working-hours', branchPath(branchId))),
  });
}

/** Bugünden sonraki özel günler (manageListBranchOverrides). */
export function overridesQuery(api: Api, branchId: string, from: string) {
  return queryOptions({
    queryKey: queryKeys.overrides(branchId, from),
    queryFn: () =>
      unwrap(
        api.GET('/api/v1/manage/branches/{branchId}/overrides', {
          params: { path: { branchId }, query: { from } },
        }),
      ),
  });
}

export interface BranchSaveInput {
  branchId: string;
  hours: { version: string; body: WeeklyHours };
  /** Yalnız değiştiyse: şubenin randevu saat aralığı (null = marka ayarı). */
  slot?: { version: string; value: SlotInterval | null };
  /** Yalnız değiştiyse: şubeler arası geçiş süresi (marka ayarı). */
  transfer?: { settings: BrandSettings; value: number };
}

/**
 * "Değişiklikleri kaydet" (AyarlarSube): haftalık saatler, gerekiyorsa şubenin saat aralığı ve marka geçiş süresi.
 * Her biri kendi sürümüyle (If-Match) sırayla kaydedilir; biri başarısız olursa sonrakiler gönderilmez.
 */
export function useSaveBranchSettings() {
  const api = useApi();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async ({ branchId, hours, slot, transfer }: BranchSaveInput) => {
      const savedHours = await unwrap(
        api.PUT('/api/v1/manage/branches/{branchId}/working-hours', {
          params: { path: { branchId }, header: ifMatch(hours.version) },
          body: hours.body,
        }),
      );
      queryClient.setQueryData(queryKeys.workingHours(branchId), savedHours);

      if (slot) {
        const branch = await unwrap(
          api.PATCH('/api/v1/manage/branches/{branchId}', {
            params: { path: { branchId }, header: ifMatch(slot.version) },
            body: { slotIntervalMinutes: slot.value },
          }),
        );
        queryClient.setQueryData(queryKeys.branch(branchId), branch);
      }

      if (transfer) {
        const settings = await unwrap(
          api.PUT('/api/v1/manage/brand/settings', {
            params: { header: ifMatch(transfer.settings.version ?? '') },
            body: brandSettingsBody(transfer.settings, {
              defaultBranchTransferMinutes: transfer.value,
            }),
          }),
        );
        queryClient.setQueryData(queryKeys.brandSettings(), settings);
      }
    },
    // Kurulumun "çalışma saatleri" adımı saatler kaydedilince tamamlanır.
    onSettled: () => queryClient.invalidateQueries({ queryKey: queryKeys.onboarding() }),
  });
}

export function useCreateOverride(branchId: string) {
  const api = useApi();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (body: WorkingHoursOverrideCreate) =>
      unwrap(
        api.POST('/api/v1/manage/branches/{branchId}/overrides', {
          params: { path: { branchId } },
          body,
        }),
      ),
    onSuccess: () =>
      queryClient.invalidateQueries({ queryKey: [...queryKeys.branch(branchId), 'overrides'] }),
  });
}

/** Çok günlük satır gün gün kaldırılır (sunucu her günü ayrı saklar). */
export function useDeleteOverrides(branchId: string) {
  const api = useApi();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (ids: readonly string[]) => {
      for (const overrideId of ids) {
        await unwrap(
          api.DELETE('/api/v1/manage/branches/{branchId}/overrides/{overrideId}', {
            params: { path: { branchId, overrideId } },
          }),
        );
      }
    },
    onSettled: () =>
      queryClient.invalidateQueries({ queryKey: [...queryKeys.branch(branchId), 'overrides'] }),
  });
}

/** Şube ekle (SubeEkle); yeni şube oturum özetindeki şube listesine ve kenar menüdeki seçiciye girer. */
export function useCreateBranch() {
  const api = useApi();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ key, body }: { key: string; body: BranchCreate }) =>
      unwrap(
        api.POST('/api/v1/manage/branches', {
          params: { header: { 'Idempotency-Key': key } },
          body,
        }),
      ),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: queryKeys.session() }),
  });
}
