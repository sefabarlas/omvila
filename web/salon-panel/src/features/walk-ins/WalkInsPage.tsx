import { ApiProblem } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { formatDuration } from '@omvila/ui-web/format';
import {
  BentoBlock,
  Button,
  DataTable,
  EmptyState,
  Skeleton,
  useToast,
} from '@omvila/ui-web/panel';
import { useQuery, useQueryClient } from '@tanstack/react-query';
import { Plus, Users } from 'lucide-react';
import { useState } from 'react';
import { useApi } from '@/app/api-context';
import { useSelectedBranch } from '@/app/branch';
import { useSession } from '@/app/session';
import { staffFirstName } from '@/features/waitlist/labels';
import { ConflictBanner } from '@/shared/ConflictBanner';
import styles from '@/shared/ListPage.module.css';
import { Page } from '@/shared/Page';
import { minutesBetween, useServerNow } from '@/shared/server-now';
import { AddWalkInModal } from './AddWalkInModal';
import { useLeaveWalkIn, useStartWalkIn, walkInsQuery } from './api';
import type { WalkInStartRequest, WalkInTicket } from './api';
import { StartWalkInModal } from './StartWalkInModal';

/**
 * Kapıda sıra (salon-paneli/KapidaSira, KapidaSiraBos; KK4): Takvim altında (/takvim/kapida-sira). Özet (sırada,
 * yeni gelen için tahmini bekleme, bugün sıradan), numara sırasıyla tablo; ilk satır "Randevuya çevir" (Kapıdan
 * kanallı Geldi randevusu), diğerleri "Sıradan çıkar"; "Sıraya ekle". Bekleme dakikaları sunucunun "şimdi"sinden.
 * İzin walkin.manage (personel dahil).
 */
export function WalkInsPage() {
  const api = useApi();
  const toast = useToast();
  const queryClient = useQueryClient();
  const { session, can } = useSession();
  const { branchId: selected } = useSelectedBranch();
  const branchId = selected ?? session.branches[0]?.id ?? '';
  const branchName = session.branches.find((b) => b.id === branchId)?.name ?? '';
  const query = useQuery({ ...walkInsQuery(api, branchId), enabled: branchId !== '' });
  const now = useServerNow(query.data?.generatedUtc);
  const start = useStartWalkIn();
  const leave = useLeaveWalkIn();
  const [adding, setAdding] = useState(false);
  const [starting, setStarting] = useState<WalkInTicket | null>(null);
  const [conflict, setConflict] = useState(false);
  const data = query.data;
  const items = data?.items ?? [];
  const canStart = can('appointment.create');

  const onError = (error: Error) => {
    if (error instanceof ApiProblem && error.status === 412) {
      setConflict(true);
      return;
    }

    toast.show({
      tone: 'hata',
      title: errorMessage(error instanceof ApiProblem ? error.code : undefined),
    });
  };

  const runStart = (ticket: WalkInTicket, body?: WalkInStartRequest) => {
    start.mutate(
      { ticket, ...(body ? { body } : {}) },
      {
        onSuccess: () => {
          setStarting(null);
          toast.show({
            tone: 'basari',
            title: 'Randevuya çevrildi',
            message: `${ticket.customerName} takvime “Kapıdan” kanalıyla eklendi.`,
          });
        },
        onError,
      },
    );
  };

  const onStart = (ticket: WalkInTicket) => {
    // Bilette personel "Fark etmez" veya hizmet yoksa önce seçilir (sözleşme: gövdede zorunlu).
    if (!ticket.preferredStaffId || !ticket.serviceId) {
      setStarting(ticket);
      return;
    }

    runStart(ticket);
  };

  const onLeave = (ticket: WalkInTicket) => {
    leave.mutate(ticket, {
      onSuccess: () => {
        toast.show({ tone: 'basari', title: `${ticket.customerName} sıradan çıkarıldı` });
      },
      onError,
    });
  };

  const add = () => {
    setAdding(true);
  };

  return (
    <Page
      title="Kapıda sıra"
      context={[
        branchName,
        'bugün',
        ...(items.length > 0 ? ['numaralar her gün 1’den başlar'] : []),
      ]
        .filter(Boolean)
        .join(' · ')}
      action={
        <Button variant="primary" icon={<Plus size={16} aria-hidden="true" />} onClick={add}>
          Sıraya ekle
        </Button>
      }
    >
      {conflict ? (
        <ConflictBanner
          onReload={() => {
            setConflict(false);
            void queryClient.invalidateQueries({ queryKey: ['walk-ins'] });
          }}
        />
      ) : null}
      {query.isPending ? (
        <Skeleton variant="block" height="320px" />
      ) : query.isError ? (
        <section className={styles.emptyCard} aria-label="Hata">
          <EmptyState
            headingLevel={2}
            title="Sıra yüklenemedi"
            description={errorMessage(
              query.error instanceof ApiProblem ? query.error.code : undefined,
            )}
            actions={<Button onClick={() => void query.refetch()}>Tekrar dene</Button>}
          />
        </section>
      ) : items.length === 0 ? (
        <section className={styles.emptyCard} aria-label="Sıra boş">
          <EmptyState
            icon={<Users size={24} />}
            headingLevel={2}
            title="Sırada kimse yok"
            description="Randevusuz gelen müşteriyi sıraya alın. Numara her gün 1’den başlar; sırası yaklaşınca müşteriye SMS gider."
            actions={
              <Button variant="primary" icon={<Plus size={16} aria-hidden="true" />} onClick={add}>
                Sıraya ekle
              </Button>
            }
          />
        </section>
      ) : (
        <>
          <div className={styles.kpis} role="group" aria-label="Sıra özeti">
            <BentoBlock
              tone="kum"
              label="Sırada"
              value={String(query.data.waitingCount)}
              {...(query.data.oldestJoinedUtc && now
                ? {
                    sub: `en eskisi ${formatDuration(minutesBetween(query.data.oldestJoinedUtc, now))}`,
                  }
                : {})}
            />
            <BentoBlock
              label="Yeni gelen için"
              value={
                query.data.estimatedWaitMinutesForNew !== null &&
                query.data.estimatedWaitMinutesForNew !== undefined
                  ? `~${formatDuration(query.data.estimatedWaitMinutesForNew)}`
                  : '—'
              }
              sub={
                query.data.estimatedWaitMinutesForNew !== null &&
                query.data.estimatedWaitMinutesForNew !== undefined
                  ? 'tahmini bekleme'
                  : 'şu an müsait personel yok'
              }
            />
            <BentoBlock
              label="Bugün sıradan"
              value={String(query.data.servedCount)}
              sub="randevuya çevrildi"
            />
          </div>
          <DataTable<WalkInTicket>
            caption="Sıradakiler"
            rows={items}
            rowKey={(ticket) => ticket.id}
            columns={[
              {
                key: 'number',
                label: 'No',
                render: (ticket) => (
                  <span
                    className={[styles.number, ticket.id === items[0]?.id ? styles.numberFirst : '']
                      .filter(Boolean)
                      .join(' ')}
                  >
                    {ticket.queueNumber}
                  </span>
                ),
              },
              {
                key: 'customer',
                label: 'Müşteri',
                render: (ticket) => <span className={styles.strong}>{ticket.customerName}</span>,
              },
              { key: 'service', label: 'Hizmet', render: (ticket) => ticket.serviceName ?? '—' },
              {
                key: 'staff',
                label: 'Personel',
                render: (ticket) => staffFirstName(ticket.preferredStaffName),
              },
              {
                key: 'waiting',
                label: 'Bekliyor',
                render: (ticket) =>
                  now ? formatDuration(minutesBetween(ticket.joinedUtc, now)) : '—',
              },
              {
                key: 'action',
                label: 'Eylem',
                align: 'right',
                render: (ticket) => (
                  <div className={styles.rowAction}>
                    {ticket.id === items[0]?.id && canStart ? (
                      <Button
                        size="sm"
                        variant="primary"
                        loading={start.isPending && start.variables.ticket.id === ticket.id}
                        loadingLabel="Çevriliyor…"
                        aria-label={`Randevuya çevir: ${ticket.customerName}`}
                        onClick={() => {
                          onStart(ticket);
                        }}
                      >
                        Randevuya çevir
                      </Button>
                    ) : (
                      <Button
                        size="sm"
                        variant="ghost"
                        loading={leave.isPending && leave.variables.id === ticket.id}
                        loadingLabel="Çıkarılıyor…"
                        aria-label={`Sıradan çıkar: ${ticket.customerName}`}
                        onClick={() => {
                          onLeave(ticket);
                        }}
                      >
                        Sıradan çıkar
                      </Button>
                    )}
                  </div>
                ),
              },
            ]}
          />
          <p className={styles.note}>
            Randevuya çevrilen kişi takvime “Kapıdan” kanalıyla eklenir.
          </p>
        </>
      )}
      {adding ? (
        <AddWalkInModal
          branchId={branchId}
          queue={data}
          onClose={() => {
            setAdding(false);
          }}
        />
      ) : null}
      {starting ? (
        <StartWalkInModal
          branchId={branchId}
          ticket={starting}
          pending={start.isPending}
          onClose={() => {
            setStarting(null);
          }}
          onStart={(body) => {
            runStart(starting, body);
          }}
        />
      ) : null}
    </Page>
  );
}
