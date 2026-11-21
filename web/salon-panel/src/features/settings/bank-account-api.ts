import { ifMatch, unwrap } from '@omvila/api-client-ts';
import type { components } from '@omvila/api-client-ts';
import { queryOptions, useMutation, useQueryClient } from '@tanstack/react-query';
import type { Api } from '@/app/api';
import { useApi } from '@/app/api-context';
import { queryKeys } from '@/app/query-keys';

export type BankAccount = components['schemas']['BankAccount'];
export type BankAccountWrite = components['schemas']['BankAccountWrite'];
export type TransferWindowMinutes = BankAccount['transferWindowMinutes'];

/** Kaporanın gönderileceği hesap (manageGetBankAccount; izin settings.payment). Tanımsızsa configured = false, ETag W/"0". */
export function bankAccountQuery(api: Api) {
  return queryOptions({
    queryKey: queryKeys.bankAccount(),
    queryFn: () => unwrap(api.GET('/api/v1/manage/brand/bank-account')),
  });
}

/** "Kaydet": hesap yoksa oluşturur (If-Match W/"0"), varsa günceller. Kayıttan sonra depositAvailable değişir. */
export function useUpdateBankAccount() {
  const api = useApi();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ version, body }: { version: string; body: BankAccountWrite }) =>
      unwrap(
        api.PUT('/api/v1/manage/brand/bank-account', {
          params: { header: ifMatch(version) },
          body,
        }),
      ),
    onSuccess: async (account) => {
      queryClient.setQueryData(queryKeys.bankAccount(), account);
      await queryClient.invalidateQueries({ queryKey: queryKeys.brandSettings() });
    },
  });
}

/** Havale süresi seçenekleri (sözleşme: 60, 120, 240 dk). */
export const transferWindowOptions: { value: TransferWindowMinutes; label: string }[] = [
  { value: 60, label: '1 saat' },
  { value: 120, label: '2 saat' },
  { value: 240, label: '4 saat' },
];

export function transferWindowLabel(minutes: number): string {
  return (
    transferWindowOptions.find((option) => option.value === minutes)?.label ??
    `${String(minutes / 60)} saat`
  );
}
