import { unwrap } from '@omvila/api-client-ts';
import type { components } from '@omvila/api-client-ts';
import { queryOptions, useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import type { Api } from '@/app/api';
import { useApi } from '@/app/api-context';
import { queryKeys } from '@/app/query-keys';
import { useSession } from '@/app/session';

export type CashDay = components['schemas']['CashDay'];
export type CashDayCloseRequest = components['schemas']['CashDayCloseRequest'];

/**
 * Kasa (manageGetCashDay): şubenin bir yerel günündeki tahsilatlar, yöntem toplamları, beklenen nakit ve gün kapatma
 * kaydı. "Önceki günler" aynı uçla tarih değiştirilerek açılır. İzin: payment.collect.
 */
export function cashDayQuery(api: Api, branchId: string, date: string) {
  return queryOptions({
    queryKey: queryKeys.cashDay(branchId, date),
    queryFn: () =>
      unwrap(api.GET('/api/v1/manage/cash', { params: { query: { branchId, date } } })),
  });
}

/**
 * Takvim ve Genel bakıştaki günün tahsilatı ("Bugünkü ciro"): yalnız kasayı görebilen kullanıcıda (payment.collect)
 * istenir; diğerlerinde veri yoktur ve bloklar randevu sayısını gösterir.
 */
export function useDayRevenue(branchId: string, date: string, enabled = true) {
  const api = useApi();
  const { can } = useSession();
  return useQuery({
    ...cashDayQuery(api, branchId, date),
    enabled: enabled && branchId !== '' && can('payment.collect'),
  }).data?.total;
}

/** "Günü kapat" (manageCloseCashDay): para kaydı oluşturduğu için Idempotency-Key; yanıt güncel kasadır. */
export function useCloseCashDay() {
  const api = useApi();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ key, body }: { key: string; body: CashDayCloseRequest }) =>
      unwrap(
        api.POST('/api/v1/manage/cash/close', {
          params: { header: { 'Idempotency-Key': key } },
          body,
        }),
      ),
    onSuccess: (day, { body }) => {
      queryClient.setQueryData(queryKeys.cashDay(body.branchId, body.date), day);
    },
  });
}
