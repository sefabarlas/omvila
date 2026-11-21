import { ApiProblem } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { formatDate, formatDuration, formatTime } from '@omvila/ui-web/format';
import {
  Badge,
  Banner,
  BentoBlock,
  Button,
  DataTable,
  EmptyState,
  PersonCell,
  Skeleton,
  useToast,
} from '@omvila/ui-web/panel';
import { toBranchDate } from '@omvila/ui-web/time';
import { useQuery, useQueryClient } from '@tanstack/react-query';
import { ListChecks, Plus } from 'lucide-react';
import { useState } from 'react';
import { useApi } from '@/app/api-context';
import { useSelectedBranch } from '@/app/branch';
import { useSession } from '@/app/session';
import { remainingMinutes } from '@/features/approvals/remaining';
import { ConflictBanner } from '@/shared/ConflictBanner';
import styles from '@/shared/ListPage.module.css';
import { Page } from '@/shared/Page';
import { useServerNow } from '@/shared/server-now';
import { dative } from '@/shared/turkish';
import { AddToWaitlistModal } from './AddToWaitlistModal';
import { useWithdrawOffer, waitlistQuery } from './api';
import type { WaitlistEntry } from './api';
import { oldestLabel, preferenceLabel, staffFirstName, waitingSinceLabel } from './labels';
import { OfferSlotModal } from './OfferSlotModal';

/**
 * Bekleme listesi (salon-paneli/BeklemeListesi, BeklemeListesiBos; KK3): özet, bekleyen teklif şeridi (geri sayım
 * sunucunun "şimdi"sinden), teklifin gideceği sırayla tablo, "Teklifi geri al", "Saat öner", "Listeye ekle".
 * İzin waitlist.manage (personel 403; menüde görünmez).
 */
export function WaitlistPage() {
  const api = useApi();
  const toast = useToast();
  const queryClient = useQueryClient();
  const { session } = useSession();
  const { branchId: selected } = useSelectedBranch();
  const branchId = selected ?? session.branches[0]?.id ?? '';
  const branchName = session.branches.find((b) => b.id === branchId)?.name ?? '';
  const query = useQuery({ ...waitlistQuery(api, branchId), enabled: branchId !== '' });
  const now = useServerNow(query.data?.generatedUtc);
  const withdraw = useWithdrawOffer();
  const [adding, setAdding] = useState(false);
  const [offering, setOffering] = useState<WaitlistEntry | null>(null);
  const [conflict, setConflict] = useState(false);

  const data = query.data;
  const timeZone = data?.timeZone ?? 'Europe/Istanbul';
  const today = now ? toBranchDate(now, timeZone) : '';
  const items = data?.items ?? [];
  const waiting = data?.overview.waitingCount ?? 0;
  const holdMinutes = data?.overview.offerHoldMinutes;
  const joinedDate = (utc: string) => toBranchDate(new Date(utc), timeZone);
  const add = () => {
    setAdding(true);
  };

  const onWithdraw = (entry: WaitlistEntry) => {
    withdraw.mutate(entry, {
      onSuccess: () => {
        toast.show({
          tone: 'basari',
          title: 'Teklif geri alındı',
          message: `${entry.customer.fullName} listede kalıyor; saat yeniden açık.`,
        });
      },
      onError: (error) => {
        if (error instanceof ApiProblem && error.status === 412) {
          setConflict(true);
          return;
        }

        toast.show({
          tone: 'hata',
          title: errorMessage(error instanceof ApiProblem ? error.code : undefined),
        });
      },
    });
  };

  return (
    <Page
      title="Bekleme listesi"
      context={waiting > 0 ? `${branchName} · ${String(waiting)} kişi bekliyor` : branchName}
      action={
        <Button variant="primary" icon={<Plus size={16} aria-hidden="true" />} onClick={add}>
          Listeye ekle
        </Button>
      }
    >
      {conflict ? (
        <ConflictBanner
          onReload={() => {
            setConflict(false);
            void queryClient.invalidateQueries({ queryKey: ['waitlist'] });
          }}
        />
      ) : null}
      {query.isPending ? (
        <Skeleton variant="block" height="320px" />
      ) : query.isError ? (
        <section className={styles.emptyCard} aria-label="Hata">
          <EmptyState
            headingLevel={2}
            title="Bekleme listesi yüklenemedi"
            description={errorMessage(
              query.error instanceof ApiProblem ? query.error.code : undefined,
            )}
            actions={<Button onClick={() => void query.refetch()}>Tekrar dene</Button>}
          />
        </section>
      ) : items.length === 0 ? (
        <section className={styles.emptyCard} aria-label="Bekleme listesi boş">
          <EmptyState
            icon={<ListChecks size={24} />}
            headingLevel={2}
            title="Bekleme listesi boş"
            description={`Dolu bir güne randevu isteyen müşteriler buraya düşer. Bir saat boşalınca sıradaki kişiye otomatik teklif gider ve saat ${String(query.data.overview.offerHoldMinutes)} dakika onun için tutulur.`}
            actions={
              <Button variant="primary" icon={<Plus size={16} aria-hidden="true" />} onClick={add}>
                Listeye ekle
              </Button>
            }
          />
        </section>
      ) : (
        <>
          <div className={styles.kpis} role="group" aria-label="Bekleme özeti">
            <BentoBlock
              tone="kum"
              label="Bekleyen"
              value={String(waiting)}
              {...(query.data.overview.oldestJoinedUtc && today
                ? { sub: oldestLabel(joinedDate(query.data.overview.oldestJoinedUtc), today) }
                : {})}
            />
            <BentoBlock
              tone="marka"
              label="Bu ay yerleşen"
              value={String(query.data.overview.convertedThisMonth)}
              sub="boşalan saatlerden"
            />
            <BentoBlock
              label="Teklif süresi"
              value={holdMinutes !== undefined ? formatDuration(holdMinutes) : '—'}
              sub="sonra sıradakine geçer"
            />
          </div>
          {now
            ? items
                .filter((entry) => entry.pendingOffer)
                .map((entry) => {
                  const offer = entry.pendingOffer;
                  if (!offer) {
                    return null;
                  }

                  const left = remainingMinutes(offer.holdExpiresUtc, now);
                  const day = formatDate(
                    toBranchDate(new Date(offer.startUtc), timeZone),
                    'weekday',
                  );
                  return (
                    <Banner
                      key={offer.id}
                      tone="basari"
                      title={`${day} ${formatTime(offer.startUtc, timeZone)} boşaldı.`}
                    >
                      {left > 0
                        ? `${dative(entry.customer.fullName)} teklif gönderildi, ${String(left)} dakika içinde yanıt bekleniyor.`
                        : `${dative(entry.customer.fullName)} teklif gönderildi; yanıt süresi doldu.`}
                    </Banner>
                  );
                })
            : null}
          <DataTable<WaitlistEntry>
            caption="Bekleyenler"
            rows={items}
            rowKey={(entry) => entry.id}
            columns={[
              {
                key: 'customer',
                label: 'Müşteri',
                render: (entry) => (
                  <PersonCell
                    name={entry.customer.fullName}
                    {...(today
                      ? { detail: waitingSinceLabel(joinedDate(entry.joinedUtc), today) }
                      : {})}
                  />
                ),
              },
              {
                key: 'services',
                label: 'İstediği',
                render: (entry) => entry.serviceNames.join(', '),
              },
              {
                key: 'preference',
                label: 'Tercih',
                render: (entry) => preferenceLabel(entry.dates, entry.timeWindows),
              },
              {
                key: 'staff',
                label: 'Personel',
                render: (entry) => staffFirstName(entry.preferredStaffName),
              },
              {
                key: 'status',
                label: 'Durum',
                render: (entry) =>
                  entry.pendingOffer ? (
                    <Badge tone="bilgi">Teklif gönderildi</Badge>
                  ) : (
                    <Badge>Bekliyor</Badge>
                  ),
              },
              {
                key: 'action',
                label: 'Eylem',
                align: 'right',
                render: (entry) => (
                  <div className={styles.rowAction}>
                    {entry.pendingOffer ? (
                      <Button
                        size="sm"
                        loading={withdraw.isPending && withdraw.variables.id === entry.id}
                        loadingLabel="Geri alınıyor…"
                        aria-label={`Teklifi geri al: ${entry.customer.fullName}`}
                        onClick={() => {
                          onWithdraw(entry);
                        }}
                      >
                        Teklifi geri al
                      </Button>
                    ) : (
                      <Button
                        size="sm"
                        variant="primary"
                        aria-label={`Saat öner: ${entry.customer.fullName}`}
                        onClick={() => {
                          setOffering(entry);
                        }}
                      >
                        Saat öner
                      </Button>
                    )}
                  </div>
                ),
              },
            ]}
          />
          <p className={styles.note}>
            Bir randevu iptal edildiğinde uygun bekleyenlere sırayla teklif gider. İlk onaylayan
            saati alır.
          </p>
        </>
      )}
      {adding ? (
        <AddToWaitlistModal
          branchId={branchId}
          onClose={() => {
            setAdding(false);
          }}
        />
      ) : null}
      {offering ? (
        <OfferSlotModal
          branchId={branchId}
          entry={offering}
          today={today}
          holdMinutes={holdMinutes}
          onClose={() => {
            setOffering(null);
          }}
          onConflict={() => {
            setOffering(null);
            setConflict(true);
          }}
        />
      ) : null}
    </Page>
  );
}
