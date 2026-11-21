import { unwrap } from '@omvila/api-client-ts';
import type { components, operations } from '@omvila/api-client-ts';
import {
  infiniteQueryOptions,
  queryOptions,
  useMutation,
  useQueryClient,
} from '@tanstack/react-query';
import type { Api } from '@/app/api';
import { useApi } from '@/app/api-context';
import { queryKeys } from '@/app/query-keys';

export type CustomerSummary = components['schemas']['CustomerSummary'];
export type CustomerCreate = components['schemas']['CustomerCreate'];
export type CustomerAppointmentRow = components['schemas']['CustomerAppointmentRow'];
export type CustomerPackage = components['schemas']['CustomerPackage'];
export type PaymentView = components['schemas']['PaymentView'];
export type CustomerImportPreview = components['schemas']['CustomerImportPreview'];
export type CustomerImportRowError = components['schemas']['CustomerImportRowError'];
export type DataDeletionRequestView = components['schemas']['DataDeletionRequestView'];
export type CustomerFilter = NonNullable<
  NonNullable<operations['manageListCustomers']['parameters']['query']>['filter']
>;

/** Sayfa boyu: sözleşmedeki varsayılan (50). */
const pageSize = 50;

/**
 * Müşteri listesi (manageListCustomers): q ad veya telefon rakamıyla ("4418" son 4 hane, T-46 (5)); imleçle sayfalanır.
 * Personel rolünde sunucu telefonu maskeli döner (phone yok, phoneMasked var).
 */
export function customersQuery(api: Api, q: string, filter: CustomerFilter) {
  return infiniteQueryOptions({
    queryKey: queryKeys.customers(q, filter),
    queryFn: ({ pageParam }) =>
      unwrap(
        api.GET('/api/v1/manage/customers', {
          params: {
            query: {
              ...(q !== '' ? { q } : {}),
              ...(filter !== 'All' ? { filter } : {}),
              limit: pageSize,
              ...(pageParam ? { cursor: pageParam } : {}),
            },
          },
        }),
      ),
    initialPageParam: null as string | null,
    getNextPageParam: (last) => last.nextCursor,
  });
}

/** Müşteri kartı (manageGetCustomer). */
export function customerQuery(api: Api, brandCustomerId: string) {
  return queryOptions({
    queryKey: queryKeys.customer(brandCustomerId),
    queryFn: () =>
      unwrap(
        api.GET('/api/v1/manage/customers/{brandCustomerId}', {
          params: { path: { brandCustomerId } },
        }),
      ),
  });
}

/** Müşteri kartı · Randevular (yeniden eskiye; personelde yalnız kendi randevuları). */
export function customerAppointmentsQuery(api: Api, brandCustomerId: string) {
  return infiniteQueryOptions({
    queryKey: queryKeys.customerAppointments(brandCustomerId),
    queryFn: ({ pageParam }) =>
      unwrap(
        api.GET('/api/v1/manage/customers/{brandCustomerId}/appointments', {
          params: {
            path: { brandCustomerId },
            query: { limit: pageSize, ...(pageParam ? { cursor: pageParam } : {}) },
          },
        }),
      ),
    initialPageParam: null as string | null,
    getNextPageParam: (last) => last.nextCursor,
  });
}

/** Müşteri ekle; kayıtlı numarada 409 phone_already_exists + meta.brandCustomerId. */
export function useCreateCustomer() {
  const api = useApi();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ key, body }: { key: string; body: CustomerCreate }) =>
      unwrap(
        api.POST('/api/v1/manage/customers', {
          params: { header: { 'Idempotency-Key': key } },
          body,
        }),
      ),
    onSuccess: (created) => {
      queryClient.setQueryData(queryKeys.customer(created.id), created);
      return queryClient.invalidateQueries({ queryKey: ['customers', 'list'] });
    },
  });
}
