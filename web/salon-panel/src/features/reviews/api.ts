import { unwrap } from '@omvila/api-client-ts';
import type { components, operations } from '@omvila/api-client-ts';
import { infiniteQueryOptions, useMutation, useQueryClient } from '@tanstack/react-query';
import type { Api } from '@/app/api';
import { useApi } from '@/app/api-context';
import { queryKeys } from '@/app/query-keys';

export type ReviewView = components['schemas']['ReviewView'];
export type ReviewOverview = components['schemas']['ReviewOverview'];
export type ReviewFilter = NonNullable<
  NonNullable<operations['manageListReviews']['parameters']['query']>['filter']
>;

/**
 * Değerlendirmeler (manageListReviews): filtreye göre sıralı, imleçle sayfalanır; overview filtreden bağımsızdır ve
 * şube seçimine uyar.
 */
export function reviewsQuery(api: Api, filter: ReviewFilter, branchId: string | undefined) {
  return infiniteQueryOptions({
    queryKey: queryKeys.reviews(filter, branchId),
    queryFn: ({ pageParam }) =>
      unwrap(
        api.GET('/api/v1/manage/reviews', {
          params: {
            query: {
              filter,
              ...(branchId ? { branchId } : {}),
              ...(pageParam ? { cursor: pageParam } : {}),
            },
          },
        }),
      ),
    initialPageParam: null as string | null,
    getNextPageParam: (last) => last.nextCursor,
  });
}

/** Müşteri kartı · Değerlendirmeler sekmesi (manageListCustomerReviews; review.view). */
export function customerReviewsQuery(api: Api, brandCustomerId: string) {
  return infiniteQueryOptions({
    queryKey: queryKeys.customerReviews(brandCustomerId),
    queryFn: ({ pageParam }) =>
      unwrap(
        api.GET('/api/v1/manage/customers/{brandCustomerId}/reviews', {
          params: {
            path: { brandCustomerId },
            query: pageParam ? { cursor: pageParam } : {},
          },
        }),
      ),
    initialPageParam: null as string | null,
    getNextPageParam: (last) => last.nextCursor,
  });
}

/** "Yanıtla" (manageReplyReview): yanıt varsa değiştirilir; listeler ve üst kartlar yenilenir. */
export function useReplyReview() {
  const api = useApi();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ reviewId, text }: { reviewId: string; text: string }) =>
      unwrap(
        api.POST('/api/v1/manage/reviews/{reviewId}/reply', {
          params: { path: { reviewId } },
          body: { text },
        }),
      ),
    onSuccess: (review) =>
      Promise.all([
        queryClient.invalidateQueries({ queryKey: ['reviews'] }),
        queryClient.invalidateQueries({
          queryKey: queryKeys.customerReviews(review.customer.brandCustomerId),
        }),
      ]),
  });
}
