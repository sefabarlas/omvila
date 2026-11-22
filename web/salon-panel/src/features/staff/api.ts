import { ifMatch, unwrap } from '@omvila/api-client-ts';
import type { components } from '@omvila/api-client-ts';
import { queryOptions, useMutation, useQueryClient } from '@tanstack/react-query';
import type { Api } from '@/app/api';
import { useApi } from '@/app/api-context';
import { queryKeys } from '@/app/query-keys';

export type StaffSummary = components['schemas']['StaffSummary'];
export type StaffDetail = components['schemas']['StaffDetail'];
export type StaffCreate = components['schemas']['StaffCreate'];
export type StaffServiceAssignment = components['schemas']['StaffServiceAssignment'];
export type TimeOffCreate = components['schemas']['TimeOffCreate'];
export type WeeklyHours = components['schemas']['WeeklyHours'];

export function staffDetailQuery(api: Api, staffId: string) {
  return queryOptions({
    queryKey: queryKeys.staffDetail(staffId),
    queryFn: () =>
      unwrap(api.GET('/api/v1/manage/staff/{staffId}', { params: { path: { staffId } } })),
  });
}

function useStaffInvalidation() {
  const queryClient = useQueryClient();
  // Personel listesi, hizmetlerdeki "Kimler veriyor?", kurulumun "ekibiniz" adımı ve Roller'deki kullanıcılar
  // (davet edilen üyelik) birlikte değişir.
  return () =>
    Promise.all([
      queryClient.invalidateQueries({ queryKey: queryKeys.staff() }),
      queryClient.invalidateQueries({ queryKey: queryKeys.users() }),
      queryClient.invalidateQueries({ queryKey: ['services'] }),
      queryClient.invalidateQueries({ queryKey: queryKeys.onboarding() }),
    ]);
}

/** Personel ekle (PersonelEkle); telefon verilirse üyelik Invited olur, davet SMS'i isteğe bağlı. */
export function useCreateStaff() {
  const api = useApi();
  const invalidate = useStaffInvalidation();
  return useMutation({
    mutationFn: ({ key, body }: { key: string; body: StaffCreate }) =>
      unwrap(
        api.POST('/api/v1/manage/staff', { params: { header: { 'Idempotency-Key': key } }, body }),
      ),
    onSuccess: invalidate,
  });
}

/** Personelin bir şubedeki haftalık saatleri; başka şubedeki saatleriyle örtüşürse 400. */
export function useSetStaffHours() {
  const api = useApi();
  const invalidate = useStaffInvalidation();
  return useMutation({
    mutationFn: ({
      staffId,
      branchId,
      version,
      body,
    }: {
      staffId: string;
      branchId: string;
      version: string;
      body: WeeklyHours;
    }) =>
      unwrap(
        api.PUT('/api/v1/manage/staff/{staffId}/branches/{branchId}/working-hours', {
          params: { path: { staffId, branchId }, header: ifMatch(version) },
          body,
        }),
      ),
    onSuccess: invalidate,
  });
}

/** Verdiği hizmetler, özel süre ve ek ücret (kabul kriteri 11). */
export function useSetStaffServices() {
  const api = useApi();
  const invalidate = useStaffInvalidation();
  return useMutation({
    mutationFn: ({
      staffId,
      version,
      items,
    }: {
      staffId: string;
      version: string;
      items: StaffServiceAssignment[];
    }) =>
      unwrap(
        api.PUT('/api/v1/manage/staff/{staffId}/services', {
          params: { path: { staffId }, header: ifMatch(version) },
          body: { items },
        }),
      ),
    onSuccess: invalidate,
  });
}

/** İzin veya tatil ekle (kabul kriteri 12); randevular iptal edilmez, yanıt etkilenenleri listeler. */
export function useCreateTimeOff() {
  const api = useApi();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ staffId, body }: { staffId: string; body: TimeOffCreate }) =>
      unwrap(
        api.POST('/api/v1/manage/staff/{staffId}/time-off', {
          params: { path: { staffId } },
          body,
        }),
      ),
    onSuccess: (_, { staffId }) =>
      queryClient.invalidateQueries({ queryKey: queryKeys.staffDetail(staffId) }),
  });
}

/** "Tek başıma çalışıyorum": kurulumun ekip adımını personel eklemeden tamamlar (dilim-01 § Alan kuralları). */
export function useConfirmSolo() {
  const api = useApi();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: () =>
      unwrap(api.POST('/api/v1/manage/onboarding/events', { body: { event: 'SoloConfirmed' } })),
    onSuccess: () =>
      Promise.all([
        queryClient.invalidateQueries({ queryKey: queryKeys.onboarding() }),
        queryClient.invalidateQueries({ queryKey: queryKeys.session() }),
      ]),
  });
}
