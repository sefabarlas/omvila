import { ifMatch, unwrap } from '@omvila/api-client-ts';
import type { components } from '@omvila/api-client-ts';
import { queryOptions, useMutation, useQueryClient } from '@tanstack/react-query';
import type { Api } from '@/app/api';
import { useApi } from '@/app/api-context';
import { queryKeys } from '@/app/query-keys';

export type WalkInQueue = components['schemas']['WalkInQueue'];
export type WalkInTicket = components['schemas']['WalkInTicketView'];
export type WalkInCreate = components['schemas']['WalkInCreate'];
export type WalkInStartRequest = components['schemas']['WalkInStartRequest'];

/** Sıra başka cihazdan (işletme uygulaması) da değişir; açık sayfa bu aralıkla tazelenir. */
const queueRefreshMs = 60_000;

/**
 * Kapıda sıra (manageListWalkIns): şubenin bugünkü Waiting ve Called biletleri numara sırasıyla, özet sayılar.
 * Takvim ve Genel bakıştaki "Kapıda sırada" bloğu da waitingCount'u buradan okur (sözleşmede başka alan yok, T-91 (6)).
 */
export function walkInsQuery(api: Api, branchId: string) {
  return queryOptions({
    queryKey: queryKeys.walkIns(branchId),
    queryFn: () =>
      unwrap(api.GET('/api/v1/manage/walk-in-tickets', { params: { query: { branchId } } })),
    refetchInterval: queueRefreshMs,
  });
}

function useQueueInvalidation() {
  const queryClient = useQueryClient();
  return () => queryClient.invalidateQueries({ queryKey: ['walk-ins'] });
}

/** "Sıraya ekle" (manageCreateWalkIn): numara sunucuda atomik verilir; dönen bilet numarayı taşır. */
export function useCreateWalkIn() {
  const api = useApi();
  const invalidate = useQueueInvalidation();
  return useMutation({
    mutationFn: ({ key, body }: { key: string; body: WalkInCreate }) =>
      unwrap(
        api.POST('/api/v1/manage/walk-in-tickets', {
          params: { header: { 'Idempotency-Key': key } },
          body,
        }),
      ),
    onSuccess: invalidate,
  });
}

/**
 * "Randevuya çevir" (manageStartWalkIn): şimdi başlayan, Kapıdan kanallı, Geldi durumunda randevu; takvim tazelenir.
 * Bilette personel "Fark etmez" veya hizmet yoksa gövdede verilir.
 */
export function useStartWalkIn() {
  const api = useApi();
  const queryClient = useQueryClient();
  const invalidate = useQueueInvalidation();
  return useMutation({
    mutationFn: ({ ticket, body }: { ticket: WalkInTicket; body?: WalkInStartRequest }) =>
      unwrap(
        api.POST('/api/v1/manage/walk-in-tickets/{ticketId}/start', {
          params: { path: { ticketId: ticket.id }, header: ifMatch(ticket.version) },
          ...(body ? { body } : {}),
        }),
      ),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['calendar'] }),
    onSettled: invalidate,
  });
}

/** "Sıradan çıkar" (manageLeaveWalkIn): arkadakilerin tahmini yeniden hesaplanır. */
export function useLeaveWalkIn() {
  const api = useApi();
  const invalidate = useQueueInvalidation();
  return useMutation({
    mutationFn: (ticket: WalkInTicket) =>
      unwrap(
        api.POST('/api/v1/manage/walk-in-tickets/{ticketId}/leave', {
          params: { path: { ticketId: ticket.id }, header: ifMatch(ticket.version) },
        }),
      ),
    onSettled: invalidate,
  });
}
