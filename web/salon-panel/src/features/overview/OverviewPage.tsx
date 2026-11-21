import { formatDate } from '@omvila/ui-web/format';
import { Bento, BentoBlock, Button } from '@omvila/ui-web/panel';
import { branchHour, branchToday, daysBetween } from '@omvila/ui-web/time';
import { useQuery, useSuspenseQuery } from '@tanstack/react-query';
import { useState } from 'react';
import { useApi } from '@/app/api-context';
import { useSelectedBranch } from '@/app/branch';
import { useClock } from '@/app/clock';
import { defaultTimeZone } from '@/app/config';
import { useSession } from '@/app/session';
import { approvalQueueQuery, splitQueue } from '@/features/approvals/api';
import { firstExpiresLabel, remainingMinutes } from '@/features/approvals/remaining';
import { calendarQuery } from '@/features/calendar/api';
import { AppointmentDrawer } from '@/features/calendar/AppointmentDrawer';
import { WalkInBlock } from '@/features/calendar/CalendarSummaryBlocks';
import { walkInsQuery } from '@/features/walk-ins/api';
import { bookingPageQuery } from '@/shared/booking-link';
import { Page } from '@/shared/Page';
import { onboardingQuery } from './api';
import { AttentionCard } from './AttentionCard';
import { DayFlowCard, dayFlowRows } from './DayFlowCard';
import { dismissOnboarding, isOnboardingDismissed } from './dismissal';
import { firstName, greeting } from './greeting';
import { OnboardingChecklist } from './OnboardingChecklist';
import styles from './Overview.module.css';
import { ShareLinkCard } from './ShareLinkCard';

/**
 * Genel bakış: marka kurulumdayken adımlar (KayitKurulum). Sonra bugün randevu veya onay bekleyen talep varsa
 * GenelBakis (bento, "Gün akışı", "Dikkat isteyenler"); hiçbiri yoksa ilk hafta görünümü (GenelBakisBos, bağlantıyı
 * paylaş). Kapıda sıra manageListWalkIns'ten (T-98); ciro kasa ile, doluluk Dilim 6'da; o zamana kadar "—" (sahte sayı yok; T-67).
 */
export function OverviewPage() {
  const api = useApi();
  const clock = useClock();
  const { session, can } = useSession();
  const { branchId } = useSelectedBranch();
  const activeBranch = branchId ?? session.branches[0]?.id ?? '';
  const onboarding = useSuspenseQuery(onboardingQuery(api)).data;
  const bookingUrl = useQuery(bookingPageQuery(api)).data?.url;
  const [dismissed, setDismissed] = useState(() => isOnboardingDismissed(session.brand.id));
  const [openAppointment, setOpenAppointment] = useState<string | null>(null);

  const name = firstName(session.user.fullName);
  const branchName =
    session.branches.find((b) => b.id === branchId)?.name ?? session.branches[0]?.name ?? '';
  const today = branchToday(clock, defaultTimeZone);
  const setupComplete = onboarding.completedCount === onboarding.totalCount;
  const onboardingView = session.brand.status === 'Onboarding' && !setupComplete && !dismissed;
  const canApprove = can('appointment.approve');
  const dayQuery = useQuery({
    ...calendarQuery(api, { branchId: activeBranch, from: today, to: today }),
    enabled: !onboardingView && can('calendar.view') && activeBranch !== '',
  });
  const managesWalkIns = can('walkin.manage');
  const walkIns = useQuery({
    ...walkInsQuery(api, activeBranch),
    enabled: !onboardingView && managesWalkIns && activeBranch !== '',
  });
  const queueQuery = useQuery({
    ...approvalQueueQuery(api, branchId ?? undefined),
    enabled: !onboardingView && canApprove,
  });

  if (onboardingView) {
    const trialDays =
      session.subscription.status === 'Trialing' && session.subscription.trialEndsOn
        ? Math.max(0, daysBetween(today, session.subscription.trialEndsOn))
        : null;
    return (
      <Page
        title={name ? `Hoş geldiniz, ${name}` : 'Hoş geldiniz'}
        context={
          trialDays !== null
            ? `${session.brand.name} · deneme ${String(trialDays)} gün`
            : session.brand.name
        }
        action={
          <Button
            onClick={() => {
              dismissOnboarding(session.brand.id);
              setDismissed(true);
            }}
          >
            Sonra yaparım
          </Button>
        }
      >
        <div className={styles.onboarding}>
          <OnboardingChecklist
            steps={onboarding.steps}
            completedCount={onboarding.completedCount}
            totalCount={onboarding.totalCount}
            branchName={branchName}
            bookingUrl={bookingUrl}
          />
          <aside className={styles.why} aria-labelledby="neden-saatler">
            <p id="neden-saatler" className={styles.whyLabel}>
              Neden önce saatler?
            </p>
            <p className={styles.whyText}>
              Müşterileriniz yalnızca açık olduğunuz saatlerde randevu alabilir. Saatleri ve
              hizmetleri girdiğiniz anda rezervasyon sayfanız yayına hazır olur.
            </p>
            <p className={styles.whyNote}>
              Kapora almak isterseniz IBAN&apos;ınızı daha sonra Ayarlar&apos;dan eklersiniz.
            </p>
          </aside>
        </div>
      </Page>
    );
  }

  const hello = greeting(branchHour(clock, defaultTimeZone));
  const rows = dayQuery.data ? dayFlowRows(dayQuery.data) : [];
  const queue = queueQuery.data;
  // Bento salonun onayını bekleyenleri sayar; kapora bekleyenler "Dikkat isteyenler"de ayrı satırdır (GenelBakis).
  const { approvals } = splitQueue(queue?.items ?? []);
  const pendingCount = approvals.length;
  const loading = (dayQuery.isLoading || queueQuery.isLoading) && !dayQuery.data && !queue;
  const active =
    rows.length > 0 || (queue?.totalCount ?? 0) > 0 || dayQuery.isError || queueQuery.isError;
  // Kuyruk holdExpiresUtc'ye göre artan sıralı: ilk onay talebi en acilidir.
  const firstExpiry =
    queue && approvals[0] ? remainingMinutes(approvals[0].holdExpiresUtc, queue.generatedUtc) : 0;

  return (
    <Page
      title={name ? `${hello}, ${name}` : hello}
      context={`${formatDate(today, 'panel')} · ${branchName}`}
    >
      {active || loading ? (
        <Bento label="Özet">
          <BentoBlock
            tone="marka"
            label="Bugün"
            value={dayQuery.data ? String(rows.length) : '—'}
            sub={`randevu · ${String(rows.filter((r) => r.status === 'tamamlandi').length)} tamamlandı`}
          />
          {canApprove ? (
            <BentoBlock
              tone="vurgu"
              label="Onay bekliyor"
              value={queue ? String(pendingCount) : '—'}
              sub={firstExpiry > 0 ? firstExpiresLabel(firstExpiry) : 'Bekleyen talep yok'}
            />
          ) : null}
          {managesWalkIns ? (
            <WalkInBlock
              summary={{
                waitingCount: walkIns.data?.waitingCount ?? null,
                estimatedWaitMinutesForNew: walkIns.data?.estimatedWaitMinutesForNew ?? null,
              }}
            />
          ) : null}
          <BentoBlock label="Bu hafta doluluk" value="—" sub="henüz veri yok" />
        </Bento>
      ) : (
        <Bento label="Özet">
          <BentoBlock tone="marka" label="Bugün" value="0" sub="randevu" />
          <BentoBlock tone="kum" label="Bu hafta" value="0" sub="online randevu" />
          <BentoBlock label="Onay bekleyen" value="0" sub="talep yok" />
          <BentoBlock label="Doluluk" value="—" sub="ilk hafta sonra" />
        </Bento>
      )}
      {active || loading ? (
        <div className={styles.overviewGrid}>
          <DayFlowCard
            view={dayQuery.data}
            isError={dayQuery.isError}
            onRetry={() => void dayQuery.refetch()}
            onOpen={setOpenAppointment}
          />
          <AttentionCard
            queue={queue}
            loading={queueQuery.isLoading}
            isError={queueQuery.isError}
            canMarkDeposit={can('appointment.deposit.mark')}
            onRetry={() => void queueQuery.refetch()}
            onOpen={setOpenAppointment}
          />
        </div>
      ) : bookingUrl ? (
        <ShareLinkCard bookingUrl={bookingUrl} setupComplete={setupComplete} />
      ) : null}
      {openAppointment ? (
        <AppointmentDrawer
          appointmentId={openAppointment}
          today={today}
          onClose={() => {
            setOpenAppointment(null);
          }}
        />
      ) : null}
    </Page>
  );
}
