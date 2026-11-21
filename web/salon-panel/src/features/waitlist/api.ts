import { ifMatch, unwrap } from '@omvila/api-client-ts';
import type { components } from '@omvila/api-client-ts';
import { queryOptions, useMutation, useQueryClient } from '@tanstack/react-query';
import type { Api } from '@/app/api';
import { useApi } from '@/app/api-context';
import { queryKeys } from '@/app/query-keys';

export type WaitlistList = components['schemas']['WaitlistList'];
export type WaitlistEntry = components['schemas']['WaitlistEntryView'];
export type WaitlistCreate = components['schemas']['ManageWaitlistCreate'];
export type WaitlistTimeWindow = components['schemas']['WaitlistTimeWindow'];

/** Bekleyen teklif varken liste bu aralıkla tazelenir (teklif kabul, ret veya süre dolumuyla sonuçlanır). */
const offerRefreshMs = 60_000;

/**
 * Bekleme listesi (manageListWaitlist): Waiting kayıtları teklifin gideceği sırayla ve özet. Kalan teklif süresi
 * generatedUtc'ye göre sayılır.
 */
export function waitlistQuery(api: Api, branchId: string) {
  return queryOptions({
    queryKey: queryKeys.waitlist(branchId),
    queryFn: () => unwrap(api.GET('/api/v1/manage/waitlist', { params: { query: { branchId } } })),
    refetchInterval: (query) =>
      query.state.data?.items.some((entry) => entry.pendingOffer) ? offerRefreshMs : false,
  });
}

function useWaitlistInvalidation() {
  const queryClient = useQueryClient();
  return () => queryClient.invalidateQueries({ queryKey: ['waitlist'] });
}

/** "Listeye ekle" (manageAddToWaitlist): kayıtlı numarada 409 phone_already_exists + meta.brandCustomerId. */
export function useAddToWaitlist() {
  const api = useApi();
  const invalidate = useWaitlistInvalidation();
  return useMutation({
    mutationFn: ({ key, body }: { key: string; body: WaitlistCreate }) =>
      unwrap(
        api.POST('/api/v1/manage/waitlist', {
          params: { header: { 'Idempotency-Key': key } },
          body,
        }),
      ),
    onSuccess: invalidate,
  });
}

/** "Saat öner" (manageOfferSlot): 409 slot_taken, invalid_state_transition, insufficient_credits; 412. */
export function useOfferSlot() {
  const api = useApi();
  const invalidate = useWaitlistInvalidation();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({
      entry,
      startUtc,
      staffId,
    }: {
      entry: WaitlistEntry;
      startUtc: string;
      staffId: string;
    }) =>
      unwrap(
        api.POST('/api/v1/manage/waitlist/{entryId}/offer', {
          params: { path: { entryId: entry.id }, header: ifMatch(entry.version) },
          body: { startUtc, staffId },
        }),
      ),
    // Tutulan saat online ve panelde dolu görünür.
    onSuccess: () =>
      Promise.all([invalidate(), queryClient.invalidateQueries({ queryKey: ['slots'] })]),
  });
}

/** "Teklifi geri al" (manageWithdrawWaitlistOffer): saat açılır, sıradakine teklif edilmez; kişi listede kalır. */
export function useWithdrawOffer() {
  const api = useApi();
  const invalidate = useWaitlistInvalidation();
  return useMutation({
    mutationFn: (entry: WaitlistEntry) =>
      unwrap(
        api.POST('/api/v1/manage/waitlist/{entryId}/offer/withdraw', {
          params: { path: { entryId: entry.id }, header: ifMatch(entry.version) },
        }),
      ),
    onSettled: invalidate,
  });
}
