import { ifMatch, unwrap } from '@omvila/api-client-ts';
import type { components } from '@omvila/api-client-ts';
import { queryOptions, useMutation, useQueryClient } from '@tanstack/react-query';
import type { Api } from '@/app/api';
import { useApi } from '@/app/api-context';
import { queryKeys } from '@/app/query-keys';

export type Service = components['schemas']['Service'];
export type ServiceWrite = components['schemas']['ServiceWrite'];
export type ServiceStage = components['schemas']['ServiceStage'];
export type DepositRule = components['schemas']['DepositRule'];
export type ServiceCategory = components['schemas']['ServiceCategory'];
export type ServiceTemplate = components['schemas']['ServiceTemplate'];
export type StaffSummary = components['schemas']['StaffSummary'];
export type IndustryCode = components['schemas']['IndustryCode'];

/** Hizmetler; şube verilirse fiyat ve süre o şubeye göre (manageListServices). Pasifler de listelenir. */
export function servicesQuery(api: Api, branchId: string | undefined) {
  return queryOptions({
    queryKey: queryKeys.services(branchId),
    queryFn: () =>
      unwrap(
        api.GET('/api/v1/manage/services', {
          params: { query: { includeInactive: true, ...(branchId ? { branchId } : {}) } },
        }),
      ),
  });
}

/**
 * Tek hizmet, genel fiyatıyla (manageGetService). Düzenleme ve yazma bunu temel alır: listedeki fiyat şubeye göre
 * çözülmüş olabilir ve genel fiyatın yerine yazılmamalıdır.
 */
export function serviceQuery(api: Api, serviceId: string) {
  return queryOptions({
    queryKey: ['service', serviceId] as const,
    queryFn: () =>
      unwrap(api.GET('/api/v1/manage/services/{serviceId}', { params: { path: { serviceId } } })),
    staleTime: 0,
  });
}

export function serviceCategoriesQuery(api: Api) {
  return queryOptions({
    queryKey: queryKeys.serviceCategories(),
    queryFn: () => unwrap(api.GET('/api/v1/manage/service-categories')),
  });
}

/** Sektör hazır listesi (manageListServiceTemplates). */
export function serviceTemplatesQuery(api: Api, industryCode: IndustryCode) {
  return queryOptions({
    queryKey: queryKeys.serviceTemplates(industryCode),
    queryFn: () =>
      unwrap(
        api.GET('/api/v1/manage/industries/{industryCode}/service-templates', {
          params: { path: { industryCode } },
        }),
      ),
  });
}

/** Personel ("Kimler veriyor?", avatarlar). */
export function staffQuery(api: Api) {
  return queryOptions({
    queryKey: queryKeys.staff(),
    queryFn: () => unwrap(api.GET('/api/v1/manage/staff')),
  });
}

function useCatalogInvalidation() {
  const queryClient = useQueryClient();
  // Hizmet ve kategori sayıları ile kurulumun "hizmetler" adımı birlikte değişir.
  return () =>
    Promise.all([
      queryClient.invalidateQueries({ queryKey: ['services'] }),
      queryClient.invalidateQueries({ queryKey: queryKeys.serviceCategories() }),
      queryClient.invalidateQueries({ queryKey: queryKeys.onboarding() }),
    ]);
}

export function useCreateCategory() {
  const api = useApi();
  const invalidate = useCatalogInvalidation();
  return useMutation({
    mutationFn: (name: string) =>
      unwrap(api.POST('/api/v1/manage/service-categories', { body: { name } })),
    onSuccess: invalidate,
  });
}

export function useCreateService() {
  const api = useApi();
  const invalidate = useCatalogInvalidation();
  return useMutation({
    mutationFn: ({ key, body }: { key: string; body: ServiceWrite }) =>
      unwrap(
        api.POST('/api/v1/manage/services', {
          params: { header: { 'Idempotency-Key': key } },
          body,
        }),
      ),
    onSuccess: invalidate,
  });
}

export function useUpdateService() {
  const api = useApi();
  const invalidate = useCatalogInvalidation();
  return useMutation({
    mutationFn: ({ id, version, body }: { id: string; version: string; body: ServiceWrite }) =>
      unwrap(
        api.PUT('/api/v1/manage/services/{serviceId}', {
          params: { path: { serviceId: id }, header: ifMatch(version) },
          body,
        }),
      ),
    onSuccess: invalidate,
  });
}

/** Online açık/kapalı: güncel genel kayıt okunur, yalnız bu alan değişir. */
export function useToggleOnline() {
  const api = useApi();
  const invalidate = useCatalogInvalidation();
  return useMutation({
    mutationFn: async ({ id, online }: { id: string; online: boolean }) => {
      const current = await unwrap(
        api.GET('/api/v1/manage/services/{serviceId}', { params: { path: { serviceId: id } } }),
      );
      return unwrap(
        api.PUT('/api/v1/manage/services/{serviceId}', {
          params: { path: { serviceId: id }, header: ifMatch(current.version) },
          body: toServiceWrite(current, { isOnlineBookable: online }),
        }),
      );
    },
    onSettled: invalidate,
  });
}

/** Şubeye özel fiyat (kabul kriteri 9) veya genel fiyata dönüş (null). */
export function useSetBranchPrice() {
  const api = useApi();
  const invalidate = useCatalogInvalidation();
  return useMutation({
    mutationFn: async ({
      serviceId,
      branchId,
      amountMinor,
    }: {
      serviceId: string;
      branchId: string;
      amountMinor: number | null;
    }) => {
      const path = { params: { path: { serviceId, branchId } } };
      if (amountMinor === null) {
        await unwrap(
          api.DELETE('/api/v1/manage/services/{serviceId}/branch-prices/{branchId}', path),
        );
        return;
      }

      await unwrap(
        api.PUT('/api/v1/manage/services/{serviceId}/branch-prices/{branchId}', {
          ...path,
          body: { price: { amountMinor, currency: 'TRY' }, isAvailable: true },
        }),
      );
    },
    onSuccess: invalidate,
  });
}

export function useCreateFromTemplates() {
  const api = useApi();
  const invalidate = useCatalogInvalidation();
  return useMutation({
    mutationFn: ({
      key,
      items,
    }: {
      key: string;
      items: { templateId: string; price?: { amountMinor: number; currency: 'TRY' } }[];
    }) =>
      unwrap(
        api.POST('/api/v1/manage/services/from-templates', {
          params: { header: { 'Idempotency-Key': key } },
          body: { items },
        }),
      ),
    onSuccess: invalidate,
  });
}

/** Hizmetin yazma gövdesi (PUT tam gövde ister): okunan hizmetten, değişikliklerle. */
export function toServiceWrite(
  service: Service,
  changes: Partial<ServiceWrite> = {},
): ServiceWrite {
  return {
    categoryId: service.categoryId,
    name: service.name,
    description: service.description ?? null,
    durationMinutes: service.durationMinutes,
    price: service.price,
    priceDisplay: service.priceDisplay,
    bufferBeforeMinutes: service.bufferBeforeMinutes,
    bufferAfterMinutes: service.bufferAfterMinutes,
    deposit: service.deposit,
    requiresApproval: service.requiresApproval ?? null,
    minNoticeMinutes: service.minNoticeMinutes ?? null,
    maxAdvanceDays: service.maxAdvanceDays ?? null,
    resourceTypeId: service.resourceTypeId ?? null,
    isOnlineBookable: service.isOnlineBookable,
    isActive: service.isActive,
    ...(service.sortOrder !== undefined ? { sortOrder: service.sortOrder } : {}),
    stages: service.stages,
    staffIds: service.staffIds,
    ...changes,
  };
}
