import { ApiProblem } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { formatCount } from '@omvila/ui-web/format';
import { Bento, BentoBlock, Button, EmptyState, FilterChips, Skeleton } from '@omvila/ui-web/panel';
import { branchToday } from '@omvila/ui-web/time';
import { keepPreviousData, useInfiniteQuery, useQuery } from '@tanstack/react-query';
import { useNavigate } from '@tanstack/react-router';
import { EyeOff, Link2, MessageSquare, Star } from 'lucide-react';
import { useState } from 'react';
import { useApi } from '@/app/api-context';
import { useSelectedBranch } from '@/app/branch';
import { useClock } from '@/app/clock';
import { defaultTimeZone } from '@/app/config';
import { useSession } from '@/app/session';
import { bookingPageQuery } from '@/shared/booking-link';
import { Page } from '@/shared/Page';
import { accusativeCount } from '@/shared/turkish';
import { reviewsQuery } from './api';
import type { ReviewOverview, ReviewView } from './api';
import { ReviewCard } from './ReviewCard';
import {
  formatRating,
  reviewFilterFromSlug,
  reviewFilterOptions,
  reviewFilterSlug,
  reviewInviteDelayMinutes,
  reviewPublicThreshold,
} from './labels';
import styles from './Reviews.module.css';

export interface ReviewsSearch {
  filtre?: string;
}

/**
 * Değerlendirmeler (salon-paneli/Degerlendirmeler, DegerlendirmelerBos): üst kartlar, filtre çipleri (adreste
 * ?filtre=tumu), kartlar ve yanıt, imleçli sayfalama. "Rezervasyon sayfasında gör" salonun herkese açık sayfasını açar.
 */
export function ReviewsPage({ search }: { search: ReviewsSearch }) {
  const api = useApi();
  const clock = useClock();
  const navigate = useNavigate();
  const { session, can } = useSession();
  const { branchId } = useSelectedBranch();
  const filter = reviewFilterFromSlug(search.filtre);
  const query = useInfiniteQuery({
    ...reviewsQuery(api, filter, branchId),
    placeholderData: keepPreviousData,
  });
  const bookingUrl = useQuery(bookingPageQuery(api)).data?.url;
  const today = branchToday(clock, defaultTimeZone);
  const branchName =
    session.branches.find((b) => b.id === branchId)?.name ?? session.branches[0]?.name ?? '';
  const overview = query.data?.pages[0]?.overview;
  const rows = query.data?.pages.flatMap((page) => page.items) ?? [];
  const canReply = can('review.reply');
  // İlk yanıt bekleyen kartın yanıt alanı açık başlar (tasarım); kullanıcı kapatınca null.
  const [replyingId, setReplyingId] = useState<string | null | undefined>(undefined);
  const openId = replyingId === undefined ? rows.find((r) => r.reply === null)?.id : replyingId;
  const context = [
    branchName,
    overview && overview.totalCount > 0
      ? `${formatCount(overview.totalCount)} değerlendirme`
      : undefined,
  ]
    .filter(Boolean)
    .join(' · ');

  return (
    <Page
      title="Değerlendirmeler"
      {...(context ? { context } : {})}
      action={
        bookingUrl ? (
          <Button
            icon={<Link2 size={16} aria-hidden="true" />}
            onClick={() => window.open(bookingUrl, '_blank', 'noopener')}
          >
            Rezervasyon sayfasında gör
          </Button>
        ) : null
      }
    >
      {query.isPending ? (
        <Skeleton variant="block" height="320px" />
      ) : query.isError ? (
        <section className={styles.emptyCard} aria-label="Hata">
          <EmptyState
            headingLevel={2}
            title="Değerlendirmeler yüklenemedi"
            description={errorMessage(
              query.error instanceof ApiProblem ? query.error.code : undefined,
            )}
            actions={<Button onClick={() => void query.refetch()}>Tekrar dene</Button>}
          />
        </section>
      ) : overview?.totalCount === 0 ? (
        <ReviewsEmpty />
      ) : (
        <>
          {overview ? <OverviewBlocks overview={overview} /> : null}
          <FilterChips
            legend="Değerlendirme filtresi"
            name="degerlendirme-filtresi"
            options={reviewFilterOptions}
            value={filter}
            onChange={(value) => {
              const filtre = reviewFilterSlug(value);
              void navigate({ to: '/degerlendirmeler', search: filtre ? { filtre } : {} });
            }}
          />
          <div className={styles.list}>
            {rows.length === 0 ? (
              <section className={styles.emptyCard} aria-label="Sonuç yok">
                <EmptyState
                  icon={<MessageSquare size={24} />}
                  headingLevel={2}
                  title={
                    filter === 'AwaitingReply'
                      ? 'Yanıt bekleyen değerlendirme yok'
                      : 'Bu filtrede değerlendirme yok'
                  }
                  description="Yeni değerlendirmeler geldikçe burada görünür."
                />
              </section>
            ) : (
              rows.map((review: ReviewView) => (
                <ReviewCard
                  key={review.id}
                  review={review}
                  today={today}
                  canReply={canReply}
                  replying={openId === review.id}
                  onReplyingChange={(open) => {
                    setReplyingId(open ? review.id : null);
                  }}
                />
              ))
            )}
            {query.hasNextPage ? (
              <div className={styles.more}>
                <span>{`${String(rows.length)} değerlendirme gösteriliyor`}</span>
                <Button
                  size="sm"
                  loading={query.isFetchingNextPage}
                  loadingLabel="Yükleniyor…"
                  onClick={() => void query.fetchNextPage()}
                >
                  Daha fazla göster
                </Button>
              </div>
            ) : null}
          </div>
          <p className={styles.footnote}>
            Yalnızca tamamlanan randevular değerlendirilebilir; her randevu bir kez.
          </p>
        </>
      )}
    </Page>
  );
}

/** Üst kartlar: Ortalama (son 90 gün), Yanıt bekleyen, Bu ay (davet oranı), En çok övülen (varsa). */
function OverviewBlocks({ overview }: { overview: ReviewOverview }) {
  const percent = overview.thisMonthInviteResponsePercent;
  const top = overview.topStaff;
  return (
    <Bento label="Değerlendirme özeti">
      <BentoBlock
        tone="marka"
        label="Ortalama"
        value={formatRating(overview.averageRating)}
        sub={`son 90 gün · ${formatCount(overview.ratingCount)} değerlendirme`}
      />
      <BentoBlock
        tone="vurgu"
        label="Yanıt bekleyen"
        value={formatCount(overview.awaitingReplyCount)}
        {...(overview.awaitingReplyCount > 0 ? { sub: 'düşük puanlılar önce' } : {})}
      />
      <BentoBlock
        label="Bu ay"
        value={formatCount(overview.thisMonthCount)}
        {...(percent !== null && percent !== undefined
          ? { sub: `davet edilenlerin %${accusativeCount(Math.round(percent))}` }
          : {})}
      />
      {top ? (
        <BentoBlock
          tone="kum"
          label="En çok övülen"
          value={top.fullName}
          sub={`${formatRating(top.averageRating)} · ${formatCount(top.count)} değerlendirme`}
        />
      ) : null}
    </Bento>
  );
}

/** DegerlendirmelerBos: henüz değerlendirme yok; iki bilgi kutusu. */
function ReviewsEmpty() {
  const hours = reviewInviteDelayMinutes / 60;
  return (
    <section className={styles.emptyCard} aria-label="Değerlendirme yok">
      <EmptyState
        icon={<Star size={24} />}
        headingLevel={2}
        title="Henüz değerlendirme yok"
        description={`Tamamlanan her randevudan ${String(hours)} saat sonra müşteriye kısa bir değerlendirme mesajı gider. Gelen puanlar ve yorumlar burada, personele göre görünür.`}
      />
      <div className={styles.tips}>
        <div className={styles.tip}>
          <MessageSquare size={18} className={styles.tipIcon} aria-hidden="true" />
          <div>
            <p className={styles.tipTitle}>Yanıt verebilirsiniz</p>
            <p className={styles.tipText}>
              Yanıtınız rezervasyon sayfasında yorumun altında görünür.
            </p>
          </div>
        </div>
        <div className={styles.tip}>
          <EyeOff size={18} className={styles.tipIcon} aria-hidden="true" />
          <div>
            <p className={styles.tipTitle}>Sayfada ne zaman çıkar?</p>
            <p className={styles.tipText}>
              {`En az ${String(reviewPublicThreshold)} değerlendirme olunca ortalama puan gösterilir.`}
            </p>
          </div>
        </div>
      </div>
    </section>
  );
}
