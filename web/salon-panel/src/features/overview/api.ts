import { unwrap } from '@omvila/api-client-ts';
import type { components } from '@omvila/api-client-ts';
import { queryOptions } from '@tanstack/react-query';
import type { Api } from '@/app/api';
import { queryKeys } from '@/app/query-keys';

export type OnboardingStepKey =
  components['schemas']['OnboardingChecklist']['steps'][number]['key'];

/** Kurulum adımları (manageGetOnboarding). Okumak markayı tamamlanınca Active yapabilir (T-27). */
export function onboardingQuery(api: Api) {
  return queryOptions({
    queryKey: queryKeys.onboarding(),
    queryFn: () => unwrap(api.GET('/api/v1/manage/onboarding')),
  });
}

export type ManageOverview = components['schemas']['ManageOverview'];

/**
 * Genel bakış bento ve "Bu hafta doluluk" (manageGetOverview; report.view). Gün ve hafta şubenin saat dilimindedir;
 * kalan süreler generatedUtc'ye göre sayılır.
 */
export function overviewQuery(api: Api, branchId: string) {
  return queryOptions({
    queryKey: queryKeys.overview(branchId),
    queryFn: () => unwrap(api.GET('/api/v1/manage/overview', { params: { query: { branchId } } })),
  });
}
