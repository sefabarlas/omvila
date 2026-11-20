import { unwrap } from '@omvila/api-client-ts';
import { useToast } from '@omvila/ui-web/panel';
import { queryOptions, useMutation, useQueryClient } from '@tanstack/react-query';
import type { Api } from '@/app/api';
import { useApi } from '@/app/api-context';
import { queryKeys } from '@/app/query-keys';
import { useSession } from '@/app/session';

export type OnboardingEvent =
  'BookingLinkCopied' | 'QrDownloaded' | 'StoryImageDownloaded' | 'SoloConfirmed';

/** Rezervasyon sayfası ayarları ve bağlantısı (manageGetBookingPageSettings). */
export function bookingPageQuery(api: Api) {
  return queryOptions({
    queryKey: queryKeys.bookingPage(),
    queryFn: () => unwrap(api.GET('/api/v1/manage/brand/booking-page')),
  });
}

/** "https://omvila.com/kuafor-elif" → "omvila.com/kuafor-elif" */
export function displayUrl(url: string | undefined): string {
  return url ? url.replace(/^https?:\/\//, '').replace(/\/$/, '') : '';
}

/** Kurulum olayı (bağlantı kopyalandı…); adımlar ve oturum (marka durumu) yenilenir. */
export function useRecordOnboardingEvent() {
  const api = useApi();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (event: OnboardingEvent) =>
      unwrap(api.POST('/api/v1/manage/onboarding/events', { body: { event } })),
    onSuccess: () =>
      Promise.all([
        queryClient.invalidateQueries({ queryKey: queryKeys.onboarding() }),
        queryClient.invalidateQueries({ queryKey: queryKeys.session() }),
      ]),
  });
}

/**
 * Rezervasyon bağlantısını panoya kopyalar (genel bakış ve Ayarlar → Rezervasyon sayfası). Kopyalamak kurulumun
 * "bağlantıyı paylaşın" adımını tamamlar; olayı yalnız ayar yetkisi olan bildirir.
 */
export function useCopyBookingLink() {
  const toast = useToast();
  const { can } = useSession();
  const record = useRecordOnboardingEvent();
  return async (url: string) => {
    try {
      await navigator.clipboard.writeText(url);
      toast.show({ title: 'Bağlantı kopyalandı' });
      if (can('settings.manage')) {
        record.mutate('BookingLinkCopied');
      }
    } catch {
      toast.show({
        tone: 'hata',
        title: 'Bağlantı kopyalanamadı',
        message: 'Bağlantıyı seçip elle kopyalayın.',
      });
    }
  };
}
