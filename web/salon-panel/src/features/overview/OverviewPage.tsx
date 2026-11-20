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
import { bookingPageQuery } from '@/shared/booking-link';
import { Page } from '@/shared/Page';
import { onboardingQuery } from './api';
import { dismissOnboarding, isOnboardingDismissed } from './dismissal';
import { firstName, greeting } from './greeting';
import { OnboardingChecklist } from './OnboardingChecklist';
import styles from './Overview.module.css';
import { ShareLinkCard } from './ShareLinkCard';

/**
 * Genel bakış (Dilim 1): marka kurulumdayken adımlar (KayitKurulum), sonra ilk hafta görünümü (GenelBakisBos).
 * Randevu verisi Dilim 2'de; bloklar bugün gerçekten 0/— (sahte sayı değil).
 */
export function OverviewPage() {
  const api = useApi();
  const clock = useClock();
  const { session } = useSession();
  const { branchId } = useSelectedBranch();
  const onboarding = useSuspenseQuery(onboardingQuery(api)).data;
  const bookingUrl = useQuery(bookingPageQuery(api)).data?.url;
  const [dismissed, setDismissed] = useState(() => isOnboardingDismissed(session.brand.id));

  const name = firstName(session.user.fullName);
  const branchName =
    session.branches.find((b) => b.id === branchId)?.name ?? session.branches[0]?.name ?? '';
  const today = branchToday(clock, defaultTimeZone);
  const setupComplete = onboarding.completedCount === onboarding.totalCount;

  if (session.brand.status === 'Onboarding' && !setupComplete && !dismissed) {
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
  return (
    <Page
      title={name ? `${hello}, ${name}` : hello}
      context={`${formatDate(today, 'panel')} · ${branchName}`}
    >
      <Bento label="Özet">
        <BentoBlock tone="marka" label="Bugün" value="0" sub="randevu" />
        <BentoBlock tone="kum" label="Bu hafta" value="0" sub="online randevu" />
        <BentoBlock label="Onay bekleyen" value="0" sub="talep yok" />
        <BentoBlock label="Doluluk" value="—" sub="ilk hafta sonra" />
      </Bento>
      {bookingUrl ? <ShareLinkCard bookingUrl={bookingUrl} setupComplete={setupComplete} /> : null}
    </Page>
  );
}
