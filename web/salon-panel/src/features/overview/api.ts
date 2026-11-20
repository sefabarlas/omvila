import { unwrap } from '@omvila/api-client-ts';
import type { components } from '@omvila/api-client-ts';
import { queryOptions, useMutation, useQueryClient } from '@tanstack/react-query';
import type { Api } from '@/app/api';
import { useApi } from '@/app/api-context';
import { queryKeys } from '@/app/query-keys';

export type OnboardingStepKey =
  components['schemas']['OnboardingChecklist']['steps'][number]['key'];
export type OnboardingEvent =
  'BookingLinkCopied' | 'QrDownloaded' | 'StoryImageDownloaded' | 'SoloConfirmed';

/** Kurulum adımları (manageGetOnboarding). Okumak markayı tamamlanınca Active yapabilir (T-27). */
export function onboardingQuery(api: Api) {
  return queryOptions({
    queryKey: queryKeys.onboarding(),
    queryFn: () => unwrap(api.GET('/api/v1/manage/onboarding')),
  });
}

/** Rezervasyon bağlantısı (manageGetBookingPageSettings). */
export function bookingPageQuery(api: Api) {
  return queryOptions({
    queryKey: queryKeys.bookingPage(),
    queryFn: () => unwrap(api.GET('/api/v1/manage/brand/booking-page')),
  });
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
