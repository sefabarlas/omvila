import { ifMatch, unwrap } from '@omvila/api-client-ts';
import type { components } from '@omvila/api-client-ts';
import { queryOptions, useMutation, useQueryClient } from '@tanstack/react-query';
import type { Api } from '@/app/api';
import { useApi } from '@/app/api-context';
import { queryKeys } from '@/app/query-keys';

export type Brand = components['schemas']['Brand'];
export type BrandUpdate = components['schemas']['BrandUpdate'];
export type BrandSettings = components['schemas']['BrandSettings'];
export type BookingPageSettings = components['schemas']['BookingPageSettings'];
export type BookingPageSettingsUpdate = components['schemas']['BookingPageSettingsUpdate'];
export type CoverColor = BookingPageSettings['coverColor'];

/** İşletme bilgileri (manageGetBrand). */
export function brandQuery(api: Api) {
  return queryOptions({
    queryKey: queryKeys.brand(),
    queryFn: () => unwrap(api.GET('/api/v1/manage/brand')),
  });
}

/** Randevu kuralları (manageGetBrandSettings). */
export function brandSettingsQuery(api: Api) {
  return queryOptions({
    queryKey: queryKeys.brandSettings(),
    queryFn: () => unwrap(api.GET('/api/v1/manage/brand/settings')),
  });
}

/** İşletme bilgilerini kaydeder; ad oturum özetinde de görünür (kenar menü, üst bar). */
export function useUpdateBrand() {
  const api = useApi();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ version, body }: { version: string; body: BrandUpdate }) =>
      unwrap(api.PATCH('/api/v1/manage/brand', { params: { header: ifMatch(version) }, body })),
    onSuccess: async (brand) => {
      queryClient.setQueryData(queryKeys.brand(), brand);
      await queryClient.invalidateQueries({ queryKey: queryKeys.session() });
    },
  });
}

/** Randevu kurallarını kaydeder ("Değişiklikleri kaydet"). */
export function useUpdateBrandSettings() {
  const api = useApi();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ version, body }: { version: string; body: BrandSettings }) =>
      unwrap(
        api.PUT('/api/v1/manage/brand/settings', {
          params: { header: ifMatch(version) },
          body,
        }),
      ),
    onSuccess: (settings) => {
      queryClient.setQueryData(queryKeys.brandSettings(), settings);
    },
  });
}

/** Rezervasyon sayfası ayarlarını kaydeder; bağlantı adı değişirse paylaşılan adres de değişir. */
export function useUpdateBookingPage() {
  const api = useApi();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ version, body }: { version: string; body: BookingPageSettingsUpdate }) =>
      unwrap(
        api.PUT('/api/v1/manage/brand/booking-page', {
          params: { header: ifMatch(version) },
          body,
        }),
      ),
    onSuccess: async (settings) => {
      queryClient.setQueryData(queryKeys.bookingPage(), settings);
      await queryClient.invalidateQueries({ queryKey: queryKeys.session() });
    },
  });
}
