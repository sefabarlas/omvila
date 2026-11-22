import { ApiProblem, ifMatch, unwrap } from '@omvila/api-client-ts';
import type { components } from '@omvila/api-client-ts';
import { queryOptions, useMutation, useQueryClient } from '@tanstack/react-query';
import type { Api } from '@/app/api';
import { useApi } from '@/app/api-context';
import { queryKeys } from '@/app/query-keys';
import { dispositionFileName } from '@/shared/download';

type Schemas = components['schemas'];
export type ApprovalQueue = Schemas['AdminApprovalQueue'];
export type ApprovalItem = Schemas['AdminApprovalQueueItem'];
export type QueueNotice = NonNullable<ApprovalItem['paymentNotice']>;
export type QueueSender = NonNullable<ApprovalItem['smsSender']>;
export type NoticeDetail = Schemas['AdminPaymentNoticeDetail'];
export type Candidate = Schemas['BankTransactionCandidate'];

/** Onay kuyrukları (adminListApprovalQueue); iki tür birden, en eskiden yeniye. */
export function approvalQueueQuery(api: Api) {
  return queryOptions({
    queryKey: queryKeys.approvalQueue(),
    queryFn: () => unwrap(api.GET('/api/v1/admin/approval-queue')),
  });
}

/** Havale bildirimi ve adaylar (adminGetPaymentNotice; açılış denetime yazılır, kendiliğinden yeniden çekilmez). */
export function paymentNoticeQuery(api: Api, noticeId: string) {
  return queryOptions({
    queryKey: queryKeys.paymentNotice(noticeId),
    queryFn: () =>
      unwrap(
        api.GET('/api/v1/admin/payment-notices/{noticeId}', { params: { path: { noticeId } } }),
      ),
    staleTime: Infinity,
    refetchOnWindowFocus: false,
  });
}

/** Karardan sonra: kuyruk, menü rozeti (oturum), banka hareketleri ve marka listeleri tazelenir. */
function useRefreshAfterDecision() {
  const queryClient = useQueryClient();
  return () =>
    Promise.all([
      queryClient.invalidateQueries({ queryKey: queryKeys.approvalQueue() }),
      queryClient.invalidateQueries({ queryKey: queryKeys.session() }),
      queryClient.invalidateQueries({ queryKey: ['bank-transactions'] }),
      queryClient.invalidateQueries({ queryKey: ['subscriptions'] }),
      queryClient.invalidateQueries({ queryKey: ['brands'] }),
    ]);
}

interface NoticeTarget {
  noticeId: string;
  version: string;
}

/** "Seçili hareketle eşleştir" / "Eşleştir" (adminMatchPaymentNotice; If-Match). */
export function useMatchNotice() {
  const api = useApi();
  const queryClient = useQueryClient();
  const refresh = useRefreshAfterDecision();
  return useMutation({
    mutationFn: ({
      noticeId,
      version,
      bankTransactionId,
    }: NoticeTarget & { bankTransactionId: string }) =>
      unwrap(
        api.POST('/api/v1/admin/payment-notices/{noticeId}/match', {
          params: { path: { noticeId }, header: ifMatch(version) },
          body: { bankTransactionId },
        }),
      ),
    onSuccess: (detail) => {
      queryClient.setQueryData(queryKeys.paymentNotice(detail.id), detail);
      return refresh();
    },
  });
}

/** "Bulunamadı, markaya yaz" (not isteğe bağlı) veya "Reddet" (not zorunlu); If-Match. */
export function useResolveNotice(kind: 'not-found' | 'reject') {
  const api = useApi();
  const queryClient = useQueryClient();
  const refresh = useRefreshAfterDecision();
  return useMutation({
    mutationFn: ({ noticeId, version, message }: NoticeTarget & { message: string }) => {
      const params = { path: { noticeId }, header: ifMatch(version) };
      return unwrap(
        kind === 'reject'
          ? api.POST('/api/v1/admin/payment-notices/{noticeId}/reject', {
              params,
              body: { message },
            })
          : api.POST('/api/v1/admin/payment-notices/{noticeId}/not-found', {
              params,
              body: message ? { message } : {},
            }),
      );
    },
    onSuccess: (detail) => {
      queryClient.setQueryData(queryKeys.paymentNotice(detail.id), detail);
      return refresh();
    },
  });
}

interface SenderTarget {
  senderId: string;
  version: string;
}

/** SMS gönderen adı "Onayla" (adminApproveSmsSender; If-Match). */
export function useApproveSender() {
  const api = useApi();
  const refresh = useRefreshAfterDecision();
  return useMutation({
    mutationFn: ({ senderId, version }: SenderTarget) =>
      unwrap(
        api.POST('/api/v1/admin/sms-senders/{senderId}/approve', {
          params: { path: { senderId }, header: ifMatch(version) },
        }),
      ),
    onSuccess: refresh,
  });
}

/** SMS gönderen adı "Reddet" (neden salonun ayarlarında görünür; If-Match). */
export function useRejectSender() {
  const api = useApi();
  const refresh = useRefreshAfterDecision();
  return useMutation({
    mutationFn: ({ senderId, version, reason }: SenderTarget & { reason: string }) =>
      unwrap(
        api.POST('/api/v1/admin/sms-senders/{senderId}/reject', {
          params: { path: { senderId }, header: ifMatch(version) },
          body: { reason },
        }),
      ),
    onSuccess: refresh,
  });
}

/** Dekont dosyası ("Aç"; indirme denetime yazılır). */
export async function downloadReceipt(
  api: Api,
  noticeId: string,
): Promise<{ blob: Blob; fileName: string | null }> {
  const { data, error, response } = await api.GET(
    '/api/v1/admin/payment-notices/{noticeId}/receipt',
    { params: { path: { noticeId } }, parseAs: 'blob' },
  );
  if (!response.ok || error !== undefined) {
    throw new ApiProblem(response.status, error);
  }

  return { blob: data, fileName: dispositionFileName(response) };
}
