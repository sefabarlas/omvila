import { ifMatch, unwrap } from '@omvila/api-client-ts';
import type { components } from '@omvila/api-client-ts';
import { queryOptions, useMutation, useQueryClient } from '@tanstack/react-query';
import type { Api } from '@/app/api';
import { useApi } from '@/app/api-context';
import { queryKeys } from '@/app/query-keys';

export type ManageUser = components['schemas']['ManageUser'];
export type ManageUserUpdate = components['schemas']['ManageUserUpdate'];
export type MembershipRole = ManageUser['role'];
export type RoleMatrixRow = components['schemas']['RoleMatrix']['rows'][number];
export type RoleAccessLevel = components['schemas']['RoleAccessLevel'];

/** Kullanıcılar (manageListUsers; settings.roles). */
export function usersQuery(api: Api) {
  return queryOptions({
    queryKey: queryKeys.users(),
    queryFn: () => unwrap(api.GET('/api/v1/manage/users')),
  });
}

/** Rol izinleri, salt okunur (manageGetRoleMatrix; settings.roles). */
export function roleMatrixQuery(api: Api) {
  return queryOptions({
    queryKey: queryKeys.roleMatrix(),
    queryFn: () => unwrap(api.GET('/api/v1/manage/roles')),
    staleTime: Infinity,
  });
}

/** "Düzenle": rol ve şubeler (manageUpdateUser). Son Owner başka role alınamaz (409 last_owner_required). */
export function useUpdateUser() {
  const api = useApi();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({
      userId,
      version,
      body,
    }: {
      userId: string;
      version: string;
      body: ManageUserUpdate;
    }) =>
      unwrap(
        api.PATCH('/api/v1/manage/users/{userId}', {
          params: { path: { userId }, header: ifMatch(version) },
          body,
        }),
      ),
    // Personel kartlarındaki rol de değişir; kendi rolünü değiştiren kullanıcının izinleri oturumdan yeniden okunur.
    onSuccess: () =>
      Promise.all([
        queryClient.invalidateQueries({ queryKey: queryKeys.users() }),
        queryClient.invalidateQueries({ queryKey: queryKeys.staff() }),
        queryClient.invalidateQueries({ queryKey: queryKeys.session() }),
      ]),
  });
}

/** Rol adları (salon-paneli/AyarlarRoller; roller-yetkiler.md). */
export const membershipRoleLabels: Record<MembershipRole, string> = {
  Owner: 'İşletme sahibi',
  BranchManager: 'Şube yöneticisi',
  Staff: 'Personel',
};

export const userStatusLabels: Record<ManageUser['status'], string> = {
  Active: 'Aktif',
  Invited: 'Davet gönderildi',
  Disabled: 'Pasif',
};

/** Matris satır adları group kodundan (sözleşme: satır adları istemcide üretilir). */
export const roleGroupLabels: Record<RoleMatrixRow['group'], string> = {
  CalendarAndAppointments: 'Takvim ve randevu',
  CustomerPhone: 'Müşteri telefonu',
  CustomerCardAndNotes: 'Müşteri kartı ve notlar',
  CashAndPayments: 'Kasa ve ödeme alma',
  ReportsAndCommission: 'Raporlar ve prim',
  ServicesAndPrices: 'Hizmet ve fiyat düzenleme',
  Settings: 'Ayarlar',
};
