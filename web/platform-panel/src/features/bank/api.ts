import { unwrap } from '@omvila/api-client-ts';
import type { components } from '@omvila/api-client-ts';
import { infiniteQueryOptions, useMutation, useQueryClient } from '@tanstack/react-query';
import type { Api } from '@/app/api';
import { useApi } from '@/app/api-context';
import { queryKeys } from '@/app/query-keys';

type Schemas = components['schemas'];
export type BankTransaction = Schemas['BankTransaction'];
export type BankTransactionCreate = Schemas['BankTransactionCreate'];
export type ImportResult = Schemas['BankTransactionImportResult'];

/** OMVILA hesabına gelen havaleler (adminListBankTransactions): arama (en az 2 harf), eşleşmemiş süzgeci, imleç. */
export function bankTransactionsQuery(api: Api, q: string, unmatchedOnly: boolean) {
  return infiniteQueryOptions({
    queryKey: queryKeys.bankTransactions(q, unmatchedOnly),
    queryFn: ({ pageParam }) =>
      unwrap(
        api.GET('/api/v1/admin/bank-transactions', {
          params: {
            query: {
              limit: 25,
              ...(q.length >= 2 ? { q } : {}),
              ...(unmatchedOnly ? { unmatchedOnly } : {}),
              ...(pageParam ? { cursor: pageParam } : {}),
            },
          },
        }),
      ),
    initialPageParam: undefined as string | undefined,
    getNextPageParam: (last) => last.nextCursor ?? undefined,
  });
}

/** Yeni hareket bekleyen bildirimlerin adaylarını değiştirir: kuyruk ve bildirimler de tazelenir. */
function useRefreshBank() {
  const queryClient = useQueryClient();
  return () =>
    Promise.all([
      queryClient.invalidateQueries({ queryKey: ['bank-transactions'] }),
      queryClient.invalidateQueries({ queryKey: ['approvals'] }),
    ]);
}

/** "Hareket ekle" (adminCreateBankTransaction; Idempotency-Key istemcide). */
export function useCreateBankTransaction() {
  const api = useApi();
  const refresh = useRefreshBank();
  return useMutation({
    mutationFn: (body: BankTransactionCreate) =>
      unwrap(api.POST('/api/v1/admin/bank-transactions', { body })),
    onSuccess: refresh,
  });
}

/** CSV içe aktarma: dryRun = true önizleme (kayıt yazılmaz), false uygular (kendiliğinden idempotent). */
export function useImportBankTransactions() {
  const api = useApi();
  const refresh = useRefreshBank();
  return useMutation({
    mutationFn: ({ file, dryRun }: { file: File; dryRun: boolean }) =>
      unwrap(
        api.POST('/api/v1/admin/bank-transactions/import', {
          // Üretilmiş tip ikili alanı string sayar; gerçek gövde FormData'dır.
          body: { file: file as unknown as string, dryRun },
          bodySerializer: (body) => {
            const form = new FormData();
            form.append('file', body.file as unknown as Blob);
            form.append('dryRun', String(body.dryRun));
            return form;
          },
        }),
      ),
    onSuccess: (result) => (result.dryRun ? undefined : refresh()),
  });
}
