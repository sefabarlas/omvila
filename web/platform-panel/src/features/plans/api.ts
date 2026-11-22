import { ifMatch, unwrap } from '@omvila/api-client-ts';
import type { components } from '@omvila/api-client-ts';
import { queryOptions, useMutation, useQueryClient } from '@tanstack/react-query';
import type { Api } from '@/app/api';
import { useApi } from '@/app/api-context';
import { queryKeys } from '@/app/query-keys';

type Schemas = components['schemas'];
export type PlanCatalog = Schemas['AdminPlanCatalog'];
export type Plan = Schemas['AdminPlan'];
export type PlanUpdate = Schemas['AdminPlanUpdate'];
export type PackageCatalog = Schemas['AdminCreditPackageCatalog'];
export type CreditPackage = Schemas['AdminCreditPackage'];

/** Kontör paketleri (adminListCreditPackages). */
export function creditPackagesQuery(api: Api) {
  return queryOptions({
    queryKey: queryKeys.creditPackages(),
    queryFn: () => unwrap(api.GET('/api/v1/admin/credit-packages')),
  });
}

/** "Planı düzenle" (adminUpdatePlans; If-Match). Yanıt güncel katalogdur. */
export function useUpdatePlans() {
  const api = useApi();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ version, plans }: { version: string; plans: PlanUpdate[] }) =>
      unwrap(
        api.PUT('/api/v1/admin/plans', { params: { header: ifMatch(version) }, body: { plans } }),
      ),
    onSuccess: (catalog) => {
      queryClient.setQueryData(queryKeys.plans(), catalog);
    },
  });
}

/** "Paketleri düzenle" (adminUpdateCreditPackages; If-Match). */
export function useUpdatePackages() {
  const api = useApi();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ version, packages }: { version: string; packages: CreditPackage[] }) =>
      unwrap(
        api.PUT('/api/v1/admin/credit-packages', {
          params: { header: ifMatch(version) },
          body: { packages },
        }),
      ),
    onSuccess: (catalog) => {
      queryClient.setQueryData(queryKeys.creditPackages(), catalog);
    },
  });
}
