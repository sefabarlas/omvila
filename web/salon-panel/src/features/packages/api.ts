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

// Paketler (Dilim 5, KK6): tanımlar, müşteri paketleri, satış ve iade. Yalnız üretilmiş istemciyle (api-client-ts).
export type PackageDefinition = components['schemas']['PackageDefinition'];
export type PackageList = components['schemas']['PackageList'];
export type PackageWrite = components['schemas']['PackageWrite'];
export type CustomerPackage = components['schemas']['CustomerPackage'];
export type PackageSaleRequest = components['schemas']['PackageSaleRequest'];
export type PackageRefundRequest = components['schemas']['PackageRefundRequest'];
export type CustomerPackageFilter = 'Active' | 'ExpiringSoon' | 'Expired' | 'All';

/** Paket tanımları (satışta olanlar önce) ve üst kartlar; sayılar seçili şubeye göre. */
export function packagesQuery(api: Api, branchId: string) {
  return queryOptions({
    queryKey: queryKeys.packages(branchId),
    queryFn: () =>
      unwrap(
        api.GET('/api/v1/manage/packages', {
          params: { query: branchId ? { branchId } : {} },
        }),
      ),
  });
}

/** Müşteri paketleri (imleçli). `q` yalnız paket adında arar (T-95). */
export function allCustomerPackagesQuery(
  api: Api,
  branchId: string,
  filter: CustomerPackageFilter,
  q: string,
  limit?: number,
) {
  return infiniteQueryOptions({
    queryKey: [...queryKeys.allCustomerPackages(branchId, filter, q), limit ?? 'all'],
    queryFn: ({ pageParam }) =>
      unwrap(
        api.GET('/api/v1/manage/customer-packages', {
          params: {
            query: {
              filter,
              ...(branchId ? { branchId } : {}),
              ...(q ? { q } : {}),
              ...(limit ? { limit } : {}),
              ...(pageParam ? { cursor: pageParam } : {}),
            },
          },
        }),
      ),
    initialPageParam: null as string | null,
    getNextPageParam: (last) => last.nextCursor,
  });
}

function usePackagesInvalidation() {
  const queryClient = useQueryClient();
  return () =>
    Promise.all([
      queryClient.invalidateQueries({ queryKey: ['packages'] }),
      queryClient.invalidateQueries({ queryKey: ['customer-packages'] }),
    ]);
}

/** "Paketi oluştur" (package.manage). Pencere başına bir Idempotency-Key. */
export function useCreatePackage() {
  const api = useApi();
  const invalidate = usePackagesInvalidation();
  return useMutation({
    mutationFn: ({ key, body }: { key: string; body: PackageWrite }) =>
      unwrap(
        api.POST('/api/v1/manage/packages', {
          params: { header: { 'Idempotency-Key': key } },
          body,
        }),
      ),
    onSuccess: invalidate,
  });
}

/** Düzenle, satıştan kaldır / satışa al (If-Match). Satılmış paketler değişmez. */
export function useUpdatePackage() {
  const api = useApi();
  const invalidate = usePackagesInvalidation();
  return useMutation({
    mutationFn: ({ pkg, body }: { pkg: PackageDefinition; body: PackageWrite }) =>
      unwrap(
        api.PUT('/api/v1/manage/packages/{packageId}', {
          params: { path: { packageId: pkg.id }, header: ifMatch(pkg.version) },
          body,
        }),
      ),
    onSuccess: invalidate,
  });
}

/** Tanımı yazma gövdesine çevirir (satış durumunu değiştirmek için). */
export function toPackageWrite(pkg: PackageDefinition): PackageWrite {
  return {
    name: pkg.name,
    price: pkg.price,
    items: pkg.items.map(({ serviceId, sessionCount }) => ({ serviceId, sessionCount })),
    validityDays: pkg.validityDays,
    saleCommissionPercent: pkg.saleCommissionPercent ?? null,
    isAvailableAllBranches: pkg.isAvailableAllBranches,
    isActive: pkg.isActive,
  };
}

/**
 * "{tutar} al ve paketi tanımla" (manageSellPackage): para kaydettiği için Idempotency-Key. Müşteri kartı (Paketler,
 * Ödemeler, paket kartı), ödeme al "Paketten" seçeneği, kasa ve paket sayıları yenilenir.
 */
export function useSellPackage() {
  const api = useApi();
  const queryClient = useQueryClient();
  const invalidate = usePackagesInvalidation();
  return useMutation({
    mutationFn: ({
      brandCustomerId,
      key,
      body,
    }: {
      brandCustomerId: string;
      key: string;
      body: PackageSaleRequest;
    }) =>
      unwrap(
        api.POST('/api/v1/manage/customers/{brandCustomerId}/packages', {
          params: { path: { brandCustomerId }, header: { 'Idempotency-Key': key } },
          body,
        }),
      ),
    onSuccess: (_sold, { brandCustomerId }) =>
      Promise.all([
        invalidate(),
        queryClient.invalidateQueries({ queryKey: queryKeys.customer(brandCustomerId) }),
        queryClient.invalidateQueries({ queryKey: ['cash'] }),
      ]),
  });
}

/** "İade et" (W9: tutarı salon girer, ödenen tutarı aşamaz). If-Match + Idempotency-Key. */
export function useRefundCustomerPackage() {
  const api = useApi();
  const queryClient = useQueryClient();
  const invalidate = usePackagesInvalidation();
  return useMutation({
    mutationFn: ({
      customerPackage,
      key,
      body,
    }: {
      customerPackage: CustomerPackage;
      key: string;
      body: PackageRefundRequest;
    }) =>
      unwrap(
        api.POST('/api/v1/manage/customer-packages/{customerPackageId}/refund', {
          params: {
            path: { customerPackageId: customerPackage.id },
            header: { ...ifMatch(customerPackage.version), 'Idempotency-Key': key },
          },
          body,
        }),
      ),
    onSuccess: (_saved, { customerPackage }) =>
      Promise.all([
        invalidate(),
        queryClient.invalidateQueries({
          queryKey: queryKeys.customer(customerPackage.customer.brandCustomerId),
        }),
        queryClient.invalidateQueries({ queryKey: ['cash'] }),
      ]),
  });
}
