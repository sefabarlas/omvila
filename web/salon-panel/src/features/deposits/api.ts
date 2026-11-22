import { ifMatch, unwrap } from '@omvila/api-client-ts';
import type { components } from '@omvila/api-client-ts';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import type { Api } from '@/app/api';
import { useApi } from '@/app/api-context';
import { queryKeys } from '@/app/query-keys';
import { appointmentQuery, useAppointmentAction } from '@/features/calendar/api';
import type { AppointmentDetail } from '@/features/calendar/api';

export type DepositRefundRequest = components['schemas']['DepositRefundRequest'];

// Kapora işlemleri (dilim-04 KK5, KK7; V5–V7). OMVILA para almaz: "Kapora geldi" ve "Salon iade etti" salonun kendi
// bankasında gördüğünü kaydeder. Para kaydeden iki işlem Idempotency-Key ister; anahtar çağıranın bir eylem için
// ürettiği değerdir (aynı eylemin tekrarı aynı anahtarla gider).

function markReceived(api: Api, detail: AppointmentDetail, key: string) {
  return unwrap(
    api.POST('/api/v1/manage/appointments/{appointmentId}/deposit/mark-received', {
      params: {
        path: { appointmentId: detail.id },
        header: { ...ifMatch(detail.version), 'Idempotency-Key': key },
      },
    }),
  );
}

/** "Kapora geldi" (manageMarkDepositReceived, A3): randevu onaylanır, Payment yazılır. */
export function useMarkDepositReceived() {
  return useAppointmentAction<{ key: string }>((api, detail, { key }) =>
    markReceived(api, detail, key),
  );
}

/**
 * Genel bakıştaki "Kapora geldi": satırda sürüm yok; If-Match için ayrıntı önce tazelenir (sürükle-bırak deseni).
 */
export function useMarkDepositReceivedById() {
  const api = useApi();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async ({ appointmentId, key }: { appointmentId: string; key: string }) => {
      const detail = await queryClient.query(appointmentQuery(api, appointmentId));
      return markReceived(api, detail, key);
    },
    onSuccess: (saved) => {
      queryClient.setQueryData(queryKeys.appointment(saved.id), saved);
    },
    onSettled: () =>
      Promise.all([
        queryClient.invalidateQueries({ queryKey: ['calendar'] }),
        queryClient.invalidateQueries({ queryKey: ['approval-queue'] }),
        queryClient.invalidateQueries({ queryKey: ['overview'] }),
      ]),
  });
}

/**
 * "Hatırlat" (manageRemindDeposit): deposit.awaiting SMS'i yeniden gider; randevu başına bir kez. If-Match yok;
 * yanıt 204, remindedUtc ayrıntı tazelenince görünür.
 */
export function useRemindDeposit() {
  const api = useApi();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (appointmentId: string) => {
      await unwrap(
        api.POST('/api/v1/manage/appointments/{appointmentId}/deposit/remind', {
          params: { path: { appointmentId } },
        }),
      );
      return appointmentId;
    },
    onSettled: (_result, _error, appointmentId) =>
      queryClient.invalidateQueries({ queryKey: queryKeys.appointment(appointmentId) }),
  });
}

/** "Salon iade etti" (manageMarkDepositRefunded): yalnız kayıt, tutar ≤ alınan kapora; tek kez. */
export function useMarkDepositRefunded() {
  return useAppointmentAction<{ key: string; body: DepositRefundRequest }>(
    (api, detail, { key, body }) =>
      unwrap(
        api.POST('/api/v1/manage/appointments/{appointmentId}/deposit/refund', {
          params: {
            path: { appointmentId: detail.id },
            header: { ...ifMatch(detail.version), 'Idempotency-Key': key },
          },
          body,
        }),
      ),
  );
}
