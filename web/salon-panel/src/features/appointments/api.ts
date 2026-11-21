import { unwrap } from '@omvila/api-client-ts';
import type { components } from '@omvila/api-client-ts';
import { queryOptions, useMutation, useQueryClient } from '@tanstack/react-query';
import type { Api } from '@/app/api';
import { useApi } from '@/app/api-context';
import { queryKeys } from '@/app/query-keys';

export type AppointmentCreate = components['schemas']['ManageAppointmentCreate'];
export type AppointmentCreated = components['schemas']['AppointmentCreated'];
export type Slot = components['schemas']['Slot'];
export type SlotSearchResult = components['schemas']['SlotSearchResult'];

/** Müşteri seçiminde gösterilen sonuç sayısı. */
const pickerLimit = 6;

/** Yeni randevu müşteri araması (manageListCustomers; ad veya telefon, "4418" son 4 hane). */
export function customerSearchQuery(api: Api, q: string) {
  return queryOptions({
    queryKey: queryKeys.customerSearch(q),
    queryFn: () =>
      unwrap(api.GET('/api/v1/manage/customers', { params: { query: { q, limit: pickerLimit } } })),
  });
}

/**
 * "{gün} · {personel} için boş saatler" (manageSearchSlots). Bütün kalemler aynı personelden; MinNotice ve MaxAdvance
 * panelde uygulanmaz (sözleşme).
 */
export function slotsQuery(
  api: Api,
  branchId: string,
  date: string,
  serviceIds: readonly string[],
  staffId: string,
) {
  return queryOptions({
    queryKey: queryKeys.slots(branchId, date, serviceIds, staffId),
    queryFn: () =>
      unwrap(
        api.POST('/api/v1/manage/branches/{branchId}/availability/slots', {
          params: { path: { branchId } },
          body: {
            items: serviceIds.map((serviceId) => ({ serviceId, staffId })),
            sameStaffForAll: true,
            date,
          },
        }),
      ),
  });
}

/** Randevu oluştur (manageCreateAppointment); takvim, müşteri kartı ve boş saatler tazelenir. */
export function useCreateAppointment() {
  const api = useApi();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ key, body }: { key: string; body: AppointmentCreate }) =>
      unwrap(
        api.POST('/api/v1/manage/appointments', {
          params: { header: { 'Idempotency-Key': key } },
          body,
        }),
      ),
    onSuccess: () =>
      Promise.all([
        queryClient.invalidateQueries({ queryKey: ['calendar'] }),
        queryClient.invalidateQueries({ queryKey: ['customers'] }),
        queryClient.invalidateQueries({ queryKey: ['slots'] }),
      ]),
  });
}
