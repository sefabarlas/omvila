import { ifMatch, unwrap } from '@omvila/api-client-ts';
import type { components } from '@omvila/api-client-ts';
import {
  infiniteQueryOptions,
  queryOptions,
  useMutation,
  useQueryClient,
} from '@tanstack/react-query';
import type { Api } from '@/app/api';
import { useApi } from '@/app/api-context';
import { queryKeys } from '@/app/query-keys';
import type { BrandDetail, BrandFilter } from './labels';

const pageSize = 25;

/** Markalar (adminListBrands): süzgeç, arama (en az 2 harf), imleçli sayfalama. */
export function brandsQuery(api: Api, filter: BrandFilter, q: string) {
  return infiniteQueryOptions({
    queryKey: queryKeys.brands(filter, q),
    queryFn: ({ pageParam }) =>
      unwrap(
        api.GET('/api/v1/admin/brands', {
          params: {
            query: {
              filter,
              limit: pageSize,
              ...(q.length >= 2 ? { q } : {}),
              ...(pageParam ? { cursor: pageParam } : {}),
            },
          },
        }),
      ),
    initialPageParam: undefined as string | undefined,
    getNextPageParam: (last) => last.nextCursor ?? undefined,
  });
}

/** Marka detayı "Özet" (adminGetBrand; açılış denetime yazılır). */
export function brandQuery(api: Api, brandId: string) {
  return queryOptions({
    queryKey: queryKeys.brand(brandId),
    queryFn: () =>
      unwrap(api.GET('/api/v1/admin/brands/{brandId}', { params: { path: { brandId } } })),
    // Her açılış ayrı denetim kaydıdır; arka planda kendiliğinden yeniden çekilmez.
    staleTime: Infinity,
    refetchOnWindowFocus: false,
  });
}

const tabPage = { limit: pageSize };

export function invoicesQuery(api: Api, brandId: string) {
  return infiniteQueryOptions({
    queryKey: queryKeys.brandTab(brandId, 'fatura'),
    queryFn: ({ pageParam }) =>
      unwrap(
        api.GET('/api/v1/admin/brands/{brandId}/invoices', {
          params: {
            path: { brandId },
            query: { ...tabPage, ...(pageParam ? { cursor: pageParam } : {}) },
          },
        }),
      ),
    initialPageParam: undefined as string | undefined,
    getNextPageParam: (last) => last.nextCursor ?? undefined,
    staleTime: Infinity,
  });
}

export function creditTransactionsQuery(api: Api, brandId: string) {
  return infiniteQueryOptions({
    queryKey: queryKeys.brandTab(brandId, 'kontor'),
    queryFn: ({ pageParam }) =>
      unwrap(
        api.GET('/api/v1/admin/brands/{brandId}/credit-transactions', {
          params: {
            path: { brandId },
            query: { ...tabPage, ...(pageParam ? { cursor: pageParam } : {}) },
          },
        }),
      ),
    initialPageParam: undefined as string | undefined,
    getNextPageParam: (last) => last.nextCursor ?? undefined,
    staleTime: Infinity,
  });
}

export function paymentNoticesQuery(api: Api, brandId: string) {
  return infiniteQueryOptions({
    queryKey: queryKeys.brandTab(brandId, 'odemeler'),
    queryFn: ({ pageParam }) =>
      unwrap(
        api.GET('/api/v1/admin/brands/{brandId}/payment-notices', {
          params: {
            path: { brandId },
            query: { ...tabPage, ...(pageParam ? { cursor: pageParam } : {}) },
          },
        }),
      ),
    initialPageParam: undefined as string | undefined,
    getNextPageParam: (last) => last.nextCursor ?? undefined,
    staleTime: Infinity,
  });
}

/** Marka denetim kaydı (adminListAuditLog?brandId; fromUtc verilir, yoksa sunucu son 24 saati döner). */
export function brandAuditQuery(api: Api, brandId: string, fromUtc: string) {
  return infiniteQueryOptions({
    queryKey: queryKeys.brandTab(brandId, 'denetim'),
    queryFn: ({ pageParam }) =>
      unwrap(
        api.GET('/api/v1/admin/audit-log', {
          params: {
            query: { ...tabPage, brandId, fromUtc, ...(pageParam ? { cursor: pageParam } : {}) },
          },
        }),
      ),
    initialPageParam: undefined as string | undefined,
    getNextPageParam: (last) => last.nextCursor ?? undefined,
  });
}

export function plansQuery(api: Api) {
  return queryOptions({
    queryKey: queryKeys.plans(),
    queryFn: () => unwrap(api.GET('/api/v1/admin/plans')),
  });
}

export type CreditGrant = components['schemas']['AdminCreditGrant'];
export type PlanChange = components['schemas']['AdminPlanChange'];

/** "Kontör ekle" (adminGrantCredits; Idempotency-Key istemcide). Sonra özet ve kontör sekmesi tazelenir. */
export function useGrantCredits(brandId: string) {
  const api = useApi();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (body: CreditGrant) =>
      unwrap(
        api.POST('/api/v1/admin/brands/{brandId}/credits', {
          params: { path: { brandId } },
          body,
        }),
      ),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: queryKeys.brand(brandId) }),
  });
}

/** "Planı değiştir" (adminChangePlan; If-Match). Yanıt güncel detaydır; önbelleğe yazılır. */
export function useChangePlan(brand: BrandDetail) {
  const api = useApi();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (body: PlanChange) =>
      unwrap(
        api.POST('/api/v1/admin/brands/{brandId}/plan', {
          params: { path: { brandId: brand.id }, header: ifMatch(brand.version) },
          body,
        }),
      ),
    onSuccess: (detail) => {
      queryClient.setQueryData(queryKeys.brand(brand.id), detail);
      void queryClient.invalidateQueries({ queryKey: ['brands', 'list'] });
    },
  });
}
