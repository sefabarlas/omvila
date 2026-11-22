import { ifMatch, unwrap } from '@omvila/api-client-ts';
import type { components } from '@omvila/api-client-ts';
import { queryOptions, useMutation, useQueryClient } from '@tanstack/react-query';
import type { Api } from '@/app/api';
import { useApi } from '@/app/api-context';
import { queryKeys } from '@/app/query-keys';
import type { AppointmentDetail } from '@/features/calendar/api';

export type CompleteRequest = components['schemas']['ManageCompleteRequest'];
export type CustomerPackage = components['schemas']['CustomerPackage'];
export type PaymentView = components['schemas']['PaymentView'];

interface CompleteInput {
  detail: AppointmentDetail;
  key: string;
  body: CompleteRequest;
}

/**
 * Müşterinin paketleri (manageListCustomerPackages): "Paketten" seçeneği yalnız kullanılabilir paket kalemi varsa
 * görünür. Bir müşterinin paketleri tek sayfaya sığar (en fazla 50; eskiler Expired/Refunded).
 */
export function customerPackagesQuery(api: Api, brandCustomerId: string) {
  return queryOptions({
    queryKey: queryKeys.customerPackages(brandCustomerId),
    queryFn: () =>
      unwrap(
        api.GET('/api/v1/manage/customers/{brandCustomerId}/packages', {
          params: { path: { brandCustomerId }, query: { limit: 50 } },
        }),
      ),
  });
}

/**
 * "Tamamla ve kaydet" (manageCompleteAppointment, A11): CheckedIn → Completed. Para kaydettiği için If-Match ile
 * birlikte Idempotency-Key; anahtar pencerenin bir gönderimi içindir. Kasa ve müşteri kartı da değişir.
 */
export function useCompleteAppointment() {
  const api = useApi();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ detail, key, body }: CompleteInput) =>
      unwrap(
        api.POST('/api/v1/manage/appointments/{appointmentId}/complete', {
          params: {
            path: { appointmentId: detail.id },
            header: { ...ifMatch(detail.version), 'Idempotency-Key': key },
          },
          body,
        }),
      ),
    onSuccess: (saved) => {
      queryClient.setQueryData(queryKeys.appointment(saved.id), saved);
    },
    onSettled: (_saved, _error, { detail }) =>
      Promise.all([
        queryClient.invalidateQueries({ queryKey: ['calendar'] }),
        queryClient.invalidateQueries({ queryKey: ['cash'] }),
        queryClient.invalidateQueries({ queryKey: ['overview'] }),
        queryClient.invalidateQueries({
          queryKey: queryKeys.customer(detail.customer.brandCustomerId),
        }),
      ]),
  });
}
