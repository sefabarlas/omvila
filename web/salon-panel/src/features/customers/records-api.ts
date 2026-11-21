import { ApiProblem, unwrap } from '@omvila/api-client-ts';
import {
  infiniteQueryOptions,
  queryOptions,
  useMutation,
  useQueryClient,
} from '@tanstack/react-query';
import type { Api } from '@/app/api';
import { useApi } from '@/app/api-context';
import { queryKeys } from '@/app/query-keys';
import type { CustomerFilter } from './api';

// Müşteri kartı sekmeleri ve kayıt işlemleri (Dilim 5): Paketler, Ödemeler, birleştirme, KVKK silme, CSV içe/dışa
// aktarma, silme talepleri. Yalnız üretilmiş istemciyle (api-client-ts).

/** Müşteri kartı · Paketler (Active önce, sonra yeniden eskiye); üstteki paket kartı ilk etkin paketten. */
export function customerPackagesQuery(api: Api, brandCustomerId: string) {
  return infiniteQueryOptions({
    queryKey: queryKeys.customerPackageList(brandCustomerId),
    queryFn: ({ pageParam }) =>
      unwrap(
        api.GET('/api/v1/manage/customers/{brandCustomerId}/packages', {
          params: { path: { brandCustomerId }, query: pageParam ? { cursor: pageParam } : {} },
        }),
      ),
    initialPageParam: null as string | null,
    getNextPageParam: (last) => last.nextCursor,
  });
}

/** Müşteri kartı · Ödemeler (payment.collect; personel 403). */
export function customerPaymentsQuery(api: Api, brandCustomerId: string) {
  return infiniteQueryOptions({
    queryKey: queryKeys.customerPayments(brandCustomerId),
    queryFn: ({ pageParam }) =>
      unwrap(
        api.GET('/api/v1/manage/customers/{brandCustomerId}/payments', {
          params: { path: { brandCustomerId }, query: pageParam ? { cursor: pageParam } : {} },
        }),
      ),
    initialPageParam: null as string | null,
    getNextPageParam: (last) => last.nextCursor,
  });
}

/** "Kayıtları birleştir": yoldaki müşteri kalır, kaynak arşive kalkar; yanıt hedefin güncel kartı. */
export function useMergeCustomers(targetId: string) {
  const api = useApi();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (body: { sourceBrandCustomerId: string; mergeNotes: boolean }) =>
      unwrap(
        api.POST('/api/v1/manage/customers/{brandCustomerId}/merge', {
          params: { path: { brandCustomerId: targetId } },
          body,
        }),
      ),
    onSuccess: (merged) => {
      queryClient.setQueryData(queryKeys.customer(merged.id), merged);
      // Taşınan randevu, paket, ödeme ve değerlendirmeler sekmelerde yenilenir.
      return queryClient.invalidateQueries({ queryKey: ['customers'] });
    },
  });
}

/** KVKK silme önizlemesi (customer.kvkk). */
export function deletionPreviewQuery(api: Api, brandCustomerId: string) {
  return queryOptions({
    queryKey: queryKeys.customerDeletionPreview(brandCustomerId),
    queryFn: () =>
      unwrap(
        api.GET('/api/v1/manage/customers/{brandCustomerId}/deletion', {
          params: { path: { brandCustomerId } },
        }),
      ),
    staleTime: 0,
  });
}

/** "Kalıcı olarak anonimleştir": geri alınamaz; müşteri listeleri ve silme talepleri yenilenir. */
export function useRequestDeletion(brandCustomerId: string) {
  const api = useApi();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (confirmFullName: string) =>
      unwrap(
        api.POST('/api/v1/manage/customers/{brandCustomerId}/deletion', {
          params: { path: { brandCustomerId } },
          body: { confirmFullName },
        }),
      ),
    onSuccess: () => {
      queryClient.removeQueries({ queryKey: queryKeys.customer(brandCustomerId) });
      return Promise.all([
        queryClient.invalidateQueries({ queryKey: ['customers', 'list'] }),
        queryClient.invalidateQueries({ queryKey: queryKeys.deletionRequests() }),
      ]);
    },
  });
}

/** CSV önizlemesi (multipart; kayıt değişmez). Dosya sözleşmede ikili alan; gövde FormData olarak gönderilir. */
export function usePreviewImport() {
  const api = useApi();
  return useMutation({
    mutationFn: ({ file, key }: { file: File; key: string }) =>
      unwrap(
        api.POST('/api/v1/manage/customers/imports', {
          params: { header: { 'Idempotency-Key': key } },
          // Üretilmiş tip ikili alanı string sayar; gerçek gövde FormData'dır.
          body: { file: file as unknown as string },
          bodySerializer: (body) => {
            const form = new FormData();
            form.append('file', body.file as unknown as Blob);
            return form;
          },
        }),
      ),
  });
}

/** Önizlemedeki geçerli satırları uygular (kendiliğinden idempotent). */
export function useCommitImport() {
  const api = useApi();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ importId, sendInfoSms }: { importId: string; sendInfoSms: boolean }) =>
      unwrap(
        api.POST('/api/v1/manage/customers/imports/{importId}/commit', {
          params: { path: { importId } },
          body: { sendInfoSms },
        }),
      ),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['customers'] }),
  });
}

/** "Dışa aktar": CSV dosyası ve sunucunun verdiği dosya adı. */
export async function exportCustomers(
  api: Api,
  filter: CustomerFilter,
): Promise<{ blob: Blob; fileName: string | null }> {
  const { data, error, response } = await api.GET('/api/v1/manage/customers/export', {
    params: { query: filter !== 'All' ? { filter } : {} },
    parseAs: 'blob',
  });
  if (!response.ok || error !== undefined) {
    throw new ApiProblem(response.status, error);
  }

  const disposition = response.headers.get('Content-Disposition') ?? '';
  const fileName = /filename="?([^";]+)"?/.exec(disposition)?.[1] ?? null;
  return { blob: data, fileName };
}

/** Ayarlar → KVKK ve veri · Silme talepleri (yeniden eskiye). */
export function deletionRequestsQuery(api: Api) {
  return infiniteQueryOptions({
    queryKey: queryKeys.deletionRequests(),
    queryFn: ({ pageParam }) =>
      unwrap(
        api.GET('/api/v1/manage/customer-deletion-requests', {
          params: { query: pageParam ? { cursor: pageParam } : {} },
        }),
      ),
    initialPageParam: null as string | null,
    getNextPageParam: (last) => last.nextCursor,
  });
}

/** Tarayıcıda dosya indirir (dışa aktarma, hatalı satırlar). */
export function downloadFile(blob: Blob, fileName: string) {
  const url = URL.createObjectURL(blob);
  const link = document.createElement('a');
  link.href = url;
  link.download = fileName;
  document.body.append(link);
  link.click();
  link.remove();
  URL.revokeObjectURL(url);
}
