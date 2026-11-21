import { unwrap } from '@omvila/api-client-ts';
import type { components } from '@omvila/api-client-ts';
import { queryOptions } from '@tanstack/react-query';
import type { Api } from '@/app/api';
import { queryKeys } from '@/app/query-keys';

export type PendingApprovalList = components['schemas']['PendingApprovalList'];
export type PendingApprovalItem = components['schemas']['PendingApprovalItem'];

/**
 * Onay kuyruğu (manageListPendingApprovals): "Dikkat isteyenler" ve "Onay bekliyor" bentosu. Yalnız
 * appointment.approve izniyle çağrılır (personel 403 alır). Kalan süre yanıttaki generatedUtc'ye göre sayıldığı
 * için dakikada bir tazelenir.
 */
export function approvalQueueQuery(api: Api, branchId: string | undefined) {
  return queryOptions({
    queryKey: queryKeys.approvalQueue(branchId),
    queryFn: () =>
      unwrap(
        api.GET('/api/v1/manage/approval-queue', {
          params: { query: branchId ? { branchId } : {} },
        }),
      ),
    refetchInterval: 60_000,
  });
}
