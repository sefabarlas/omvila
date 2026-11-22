import { unwrap } from '@omvila/api-client-ts';
import type { components } from '@omvila/api-client-ts';
import { formatCount, formatDate } from '@omvila/ui-web/format';
import { Avatar, Bento, BentoBlock, Button, EmptyState, Skeleton } from '@omvila/ui-web/panel';
import { branchToday } from '@omvila/ui-web/time';
import { useQuery } from '@tanstack/react-query';
import { Link, useNavigate } from '@tanstack/react-router';
import { ChevronRight, CircleCheck, Store } from 'lucide-react';
import { useApi } from '@/app/api-context';
import { useClock } from '@/app/clock';
import { platformTimeZone } from '@/app/config';
import { queryKeys } from '@/app/query-keys';
import { BrandBadge } from '@/features/brands/BrandBadge';
import { attentionBadge, brandSubtitle } from '@/features/brands/labels';
import { Page } from '@/shared/Page';
import { QueryState } from '@/shared/QueryState';
import { accusativeCount } from '@/shared/turkish';
import cardStyles from '@/shared/Card.module.css';
import styles from './Overview.module.css';

type Overview = components['schemas']['AdminOverview'];

/** platform-paneli/Main: dört özet bloğu, aktivasyon hunisi ve dikkat isteyen markalar (adminGetOverview). */
export function OverviewPage() {
  const api = useApi();
  const clock = useClock();
  const navigate = useNavigate();
  const query = useQuery({
    queryKey: queryKeys.overview(),
    queryFn: () => unwrap(api.GET('/api/v1/admin/overview')),
  });
  return (
    <Page
      title="Platform"
      context={`Bugün · ${formatDate(branchToday(clock, platformTimeZone), 'dayMonth')}`}
      action={
        <Button icon={<Store size={16} />} onClick={() => void navigate({ to: '/markalar' })}>
          Markalar
        </Button>
      }
    >
      <QueryState
        query={query}
        loading={<Skeleton variant="block" height="calc(var(--bosluk-12) * 3)" />}
      >
        {(overview) => <OverviewContent overview={overview} />}
      </QueryState>
    </Page>
  );
}

function OverviewContent({ overview }: { overview: Overview }) {
  const { activeBrands, pendingApprovals, trialing, pastDue } = overview;
  return (
    <>
      <Bento label="Platform özeti">
        <BentoBlock
          tone="marka"
          label="Aktif marka"
          value={formatCount(activeBrands.count)}
          sub={
            activeBrands.newThisMonth > 0
              ? `+${formatCount(activeBrands.newThisMonth)} bu ay`
              : 'Bu ay yeni kayıt yok'
          }
        />
        <BentoBlock
          tone="vurgu"
          label="Onay bekleyen"
          value={formatCount(pendingApprovals.total)}
          sub={`${formatCount(pendingApprovals.paymentNotices)} havale bildirimi · ${formatCount(pendingApprovals.smsSenders)} SMS başlığı`}
        />
        <BentoBlock
          tone="kum"
          label="Denemede"
          value={formatCount(trialing.count)}
          sub={
            trialing.endingThisWeek > 0
              ? `${accusativeCount(trialing.endingThisWeek)} bu hafta bitiyor`
              : 'Bu hafta biten deneme yok'
          }
        />
        <BentoBlock
          label="Ödeme gecikti"
          value={formatCount(pastDue.count)}
          sub={
            pastDue.readOnlySoon > 0
              ? `${accusativeCount(pastDue.readOnlySoon)} salt okunura geçmek üzere`
              : 'Salt okunura geçecek marka yok'
          }
        />
      </Bento>
      <div className={cardStyles.grid}>
        <Funnel funnel={overview.funnel} />
        <AttentionBrands brands={overview.attentionBrands} />
      </div>
    </>
  );
}

function Funnel({ funnel }: { funnel: Overview['funnel'] }) {
  const steps = [
    { label: 'Kaydoldu', value: funnel.signedUp },
    { label: 'Hizmet ekledi', value: funnel.addedServices },
    { label: 'Çalışma saatlerini girdi', value: funnel.setWorkingHours },
    { label: 'İlk online randevu', value: funnel.firstOnlineAppointment },
  ];
  const title = `Aktivasyon hunisi · son ${String(funnel.windowDays)} gün`;
  return (
    <section className={cardStyles.card} aria-label={title}>
      <h2 className={cardStyles.cardTitle}>{title}</h2>
      {funnel.signedUp === 0 ? (
        <p className={cardStyles.muted}>Bu dönemde yeni kayıt yok.</p>
      ) : (
        <ul className={styles.funnel}>
          {steps.map((step) => (
            <li key={step.label} className={styles.step}>
              <div className={styles.stepHead}>
                <span>{step.label}</span>
                <span className={styles.stepValue}>{formatCount(step.value)}</span>
              </div>
              <div className={styles.track} aria-hidden="true">
                <div
                  className={styles.bar}
                  style={{ width: `${String(Math.round((step.value / funnel.signedUp) * 100))}%` }}
                />
              </div>
            </li>
          ))}
        </ul>
      )}
    </section>
  );
}

function AttentionBrands({ brands }: { brands: Overview['attentionBrands'] }) {
  return (
    <section
      className={`${cardStyles.card ?? ''} ${cardStyles.flush ?? ''}`}
      aria-labelledby="dikkat-baslik"
    >
      <div className={cardStyles.cardHead}>
        <h2 id="dikkat-baslik" className={cardStyles.cardTitle}>
          Dikkat isteyen markalar
        </h2>
        <Link to="/markalar" className={cardStyles.link}>
          Tümü
        </Link>
      </div>
      {brands.length === 0 ? (
        <EmptyState
          icon={<CircleCheck size={22} />}
          title="Dikkat isteyen marka yok"
          description="Ödemesi geciken, kontörü biten veya denemesi bitmek üzere olan marka yok."
        />
      ) : (
        <ul className={styles.rows}>
          {brands.map((brand) => {
            const badge = attentionBadge(brand);
            return (
              <li key={brand.brandId}>
                <Link
                  to="/markalar/$brandId"
                  params={{ brandId: brand.brandId }}
                  className={styles.row}
                  aria-label={`${brand.brandName}, ${badge.label}, ${formatCount(brand.branchCount)} şube, bu ay ${formatCount(brand.appointmentsThisMonth)} randevu`}
                >
                  <Avatar name={brand.brandName} size="sm" />
                  <span className={styles.name}>
                    <span className={styles.brandName}>{brand.brandName}</span>
                    <span className={cardStyles.muted}>
                      {brandSubtitle(brand.industryCode, brand.city)}
                    </span>
                  </span>
                  <span>{brand.planName}</span>
                  <span className={styles.badge}>
                    <BrandBadge {...badge} />
                  </span>
                  <span className={styles.number} title="Şube">
                    {formatCount(brand.branchCount)}
                  </span>
                  <span className={styles.number} title="Bu ay randevu">
                    {brand.appointmentsThisMonth > 0
                      ? formatCount(brand.appointmentsThisMonth)
                      : '—'}
                  </span>
                  <ChevronRight size={16} aria-hidden="true" />
                </Link>
              </li>
            );
          })}
        </ul>
      )}
    </section>
  );
}
