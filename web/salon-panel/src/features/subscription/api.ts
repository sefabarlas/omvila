import { ApiProblem, unwrap } from '@omvila/api-client-ts';
import type { components } from '@omvila/api-client-ts';
import {
  infiniteQueryOptions,
  queryOptions,
  useMutation,
  useQueryClient,
} from '@tanstack/react-query';
import type { Api } from '@/app/api';
import { useApi } from '@/app/api-context';
import { queryKeys } from '@/app/query-keys';

// Ayarlar → Abonelik ve fatura, havale bildirimi ve kontör satın alma (Dilim 6; subscription.manage, yalnız Owner).
// Yalnız üretilmiş istemciyle (api-client-ts).
export type SubscriptionDetail = components['schemas']['SubscriptionDetail'];
export type SubscriptionInvoice = components['schemas']['SubscriptionInvoice'];
export type CreditPackage = components['schemas']['CreditPackage'];
export type CreditPurchaseInstructions = components['schemas']['CreditPurchaseInstructions'];
export type OmvilaBankAccount = components['schemas']['OmvilaBankAccount'];
export type PaymentNoticePurpose = components['schemas']['PaymentNoticePurpose'];
export type Money = components['schemas']['Money'];

export function subscriptionQuery(api: Api) {
  return queryOptions({
    queryKey: queryKeys.subscription(),
    queryFn: () => unwrap(api.GET('/api/v1/manage/subscription')),
  });
}

/** Faturalar (yeniden eskiye, imleçli). */
export function invoicesQuery(api: Api) {
  return infiniteQueryOptions({
    queryKey: queryKeys.invoices(),
    queryFn: ({ pageParam }) =>
      unwrap(
        api.GET('/api/v1/manage/subscription/invoices', {
          params: { query: pageParam ? { cursor: pageParam } : {} },
        }),
      ),
    initialPageParam: null as string | null,
    getNextPageParam: (last) => last.nextCursor,
  });
}

/** Fatura PDF'i (bilgi amaçlı) ve sunucunun verdiği dosya adı. */
export async function fetchInvoicePdf(
  api: Api,
  invoiceId: string,
): Promise<{ blob: Blob; fileName: string | null }> {
  const { data, error, response } = await api.GET(
    '/api/v1/manage/subscription/invoices/{invoiceId}/pdf',
    { params: { path: { invoiceId } }, parseAs: 'blob' },
  );
  if (!response.ok || error !== undefined) {
    throw new ApiProblem(response.status, error);
  }

  const disposition = response.headers.get('Content-Disposition') ?? '';
  const fileName = /filename="?([^";]+)"?/.exec(disposition)?.[1] ?? null;
  return { blob: data, fileName };
}

export interface NotifyTransferInput {
  key: string;
  purpose: PaymentNoticePurpose;
  creditPackageId?: string;
  senderName?: string;
  receipt?: File;
}

/** "Havaleyi bildir" (multipart; dekont isteğe bağlı). Aynı bildirimin tekrarı aynı anahtarla gider. */
export function useNotifyTransfer() {
  const api = useApi();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ key, purpose, creditPackageId, senderName, receipt }: NotifyTransferInput) =>
      unwrap(
        api.POST('/api/v1/manage/subscription/payment-notices', {
          params: { header: { 'Idempotency-Key': key } },
          body: {
            purpose,
            ...(creditPackageId ? { creditPackageId } : {}),
            ...(senderName ? { senderName } : {}),
            // Üretilmiş tip ikili alanı string sayar; gerçek gövde FormData'dır.
            ...(receipt ? { receipt: receipt as unknown as string } : {}),
          },
          bodySerializer: (body) => {
            const form = new FormData();
            // Gövdede yalnız verilen alanlar vardır (isteğe bağlı alanlar yukarıda eklenmez).
            for (const [name, value] of Object.entries(body)) {
              form.append(name, value as unknown as Blob | string);
            }

            return form;
          },
        }),
      ),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: queryKeys.subscription() }),
  });
}

export function creditPackagesQuery(api: Api) {
  return queryOptions({
    queryKey: queryKeys.creditPackages(),
    queryFn: () => unwrap(api.GET('/api/v1/manage/credit-packages')),
  });
}

/** Paket seçimi → havale bilgisi (KNT- kodu, tutar, alıcı). Kayıt oluşturmaz. */
export function usePurchaseCredits() {
  const api = useApi();
  return useMutation({
    mutationFn: (creditPackageId: string) =>
      unwrap(api.POST('/api/v1/manage/credits/purchase', { body: { creditPackageId } })),
  });
}
