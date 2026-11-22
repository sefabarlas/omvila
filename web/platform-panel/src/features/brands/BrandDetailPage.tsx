import { formatCount, formatDate } from '@omvila/ui-web/format';
import { Bento, BentoBlock, Button, Skeleton, Tabs, Tooltip } from '@omvila/ui-web/panel';
import { toBranchDate } from '@omvila/ui-web/time';
import { useQuery } from '@tanstack/react-query';
import { Link, useNavigate } from '@tanstack/react-router';
import { ChevronLeft, Lock, ShieldCheck } from 'lucide-react';
import { useState } from 'react';
import type { ReactNode } from 'react';
import type { AdminPermission } from '@/app/api';
import { useApi } from '@/app/api-context';
import { platformTimeZone } from '@/app/config';
import { useSession } from '@/app/session';
import cardStyles from '@/shared/Card.module.css';
import { Page } from '@/shared/Page';
import { QueryState } from '@/shared/QueryState';
import { brandQuery } from './api';
import { BrandTabPanel } from './BrandTabs';
import styles from './Brands.module.css';
import { ChangePlanModal } from './ChangePlanModal';
import { CreditTransactionsTable } from './CreditTransactionsTable';
import { GrantCreditsModal } from './GrantCreditsModal';
import { brandSubtitle, brandTabs, subscriptionLabels } from './labels';
import type { BrandDetail, BrandTab } from './labels';

const localDate = (utc: string) => toBranchDate(new Date(utc), platformTimeZone);

/**
 * platform-paneli/PMarkaDetay: Özet ve sekmeler. Açılış denetime yazılır (sunucu); telefonlar maskeli gelir. Kontör ve
 * plan butonları izinle (platform.credits.grant, platform.brands.manage); izin yoksa pasif ve nedeni yazar.
 */
export function BrandDetailPage({ brandId, tab }: { brandId: string; tab: BrandTab }) {
  const api = useApi();
  const navigate = useNavigate();
  const query = useQuery(brandQuery(api, brandId));
  const brand = query.data;
  const [dialog, setDialog] = useState<'credits' | 'plan' | null>(null);

  return (
    <Page
      title={brand?.name ?? 'Marka'}
      {...(brand
        ? {
            context: [
              brandSubtitle(brand.industryCode, brand.city),
              `${formatCount(brand.branches.length)} şube`,
              `kayıt ${formatDate(localDate(brand.createdUtc), 'dayMonth')}`,
            ].join(' · '),
            action: (
              <div className={styles.actions}>
                <ManageButton
                  permission="platform.credits.grant"
                  onClick={() => {
                    setDialog('credits');
                  }}
                >
                  Kontör ekle
                </ManageButton>
                <ManageButton
                  permission="platform.brands.manage"
                  onClick={() => {
                    setDialog('plan');
                  }}
                >
                  Planı değiştir
                </ManageButton>
              </div>
            ),
          }
        : {})}
    >
      <Link to="/markalar" className={styles.back}>
        <ChevronLeft size={16} aria-hidden="true" />
        Markalar
      </Link>
      <Tabs
        label="Marka bölümleri"
        idPrefix="marka"
        items={brandTabs}
        value={tab}
        onChange={(next) =>
          void navigate({
            to: '/markalar/$brandId',
            params: { brandId },
            search: next === 'ozet' ? {} : { sekme: next as BrandTab },
          })
        }
      />
      <div role="tabpanel" id={`marka-panel-${tab}`} aria-labelledby={`marka-tab-${tab}`}>
        {tab === 'ozet' ? (
          <QueryState
            query={query}
            loading={<Skeleton variant="block" height="calc(var(--bosluk-12) * 4)" />}
          >
            {(detail) => <Summary brand={detail} />}
          </QueryState>
        ) : (
          <BrandTabPanel brandId={brandId} tab={tab} />
        )}
      </div>
      {brand && dialog === 'credits' ? (
        <GrantCreditsModal
          brand={brand}
          onClose={() => {
            setDialog(null);
          }}
        />
      ) : null}
      {brand && dialog === 'plan' ? (
        <ChangePlanModal
          brand={brand}
          onRefresh={() => void query.refetch()}
          onClose={() => {
            setDialog(null);
          }}
        />
      ) : null}
    </Page>
  );
}

/** İzinli kullanıcıda buton; izni olmayanda kilitli, pasif ve nedeni ipucunda ("Yönetici yetkisi ister"). */
function ManageButton({
  permission,
  onClick,
  children,
}: {
  permission: AdminPermission;
  onClick: () => void;
  children: ReactNode;
}) {
  const { can } = useSession();
  if (can(permission)) {
    return <Button onClick={onClick}>{children}</Button>;
  }

  return (
    <Tooltip content="Yönetici yetkisi ister">
      <Button disabled icon={<Lock size={14} />}>
        {children}
      </Button>
    </Tooltip>
  );
}

function Summary({ brand }: { brand: BrandDetail }) {
  const { can } = useSession();
  const { subscription, credits, appointmentsThisMonth } = brand;
  const editsLocked = !can('platform.brands.manage') || !can('platform.credits.grant');
  return (
    <div className={styles.side}>
      <Bento label="Marka özeti">
        <BentoBlock
          tone="marka"
          label="Abonelik"
          value={subscriptionLabels[subscription.status]}
          sub={subscriptionSub(subscription)}
        />
        <BentoBlock
          tone="kum"
          label="Kontör"
          value={formatCount(credits.balance)}
          sub={
            credits.estimatedDaysLeft === null
              ? 'SMS'
              : `SMS · tahmini ${formatCount(credits.estimatedDaysLeft)} gün yeter`
          }
        />
        <BentoBlock
          label="Bu ay randevu"
          value={formatCount(appointmentsThisMonth.count)}
          {...(appointmentsThisMonth.onlinePercent === null
            ? {}
            : { sub: `Online oranı %${String(Math.round(appointmentsThisMonth.onlinePercent))}` })}
        />
        <BentoBlock
          tone={subscription.pendingNoticeId ? 'vurgu' : 'default'}
          label="Abonelik ödemesi"
          value={
            subscription.paymentMethod === 'BankTransfer'
              ? 'Havale'
              : subscription.paymentMethod === 'Card'
                ? 'Kart'
                : '—'
          }
          sub={
            subscription.pendingNoticeNotifiedUtc
              ? `${formatDate(localDate(subscription.pendingNoticeNotifiedUtc), 'dayMonth')} bildirildi · eşleştirme bekliyor`
              : `Havale kodu ${subscription.transferCode}`
          }
        />
      </Bento>
      <div className={cardStyles.grid}>
        <section className={cardStyles.card} aria-labelledby="marka-bilgileri">
          <h2 id="marka-bilgileri" className={cardStyles.cardTitle}>
            Marka bilgileri
          </h2>
          <dl className={styles.facts}>
            <Fact label="Sahip">{`${brand.owner.fullName} · ${brand.owner.phoneMasked}`}</Fact>
            <Fact label="Rezervasyon bağlantısı">
              {brand.bookingUrl.replace(/^https?:\/\//, '')}
            </Fact>
            <Fact label="Şubeler">
              {brand.branches.length > 0 ? brand.branches.map((b) => b.name).join(', ') : '—'}
            </Fact>
            <Fact label="Personel">{`${formatCount(brand.activeStaffCount)} aktif`}</Fact>
            <Fact label="Müşteri kaydı">{formatCount(brand.customerCount)}</Fact>
            <Fact label="Google Takvim">
              {brand.googleCalendarConnectedStaffCount > 0
                ? `${formatCount(brand.googleCalendarConnectedStaffCount)} personelde bağlı (tek yönlü)`
                : 'Bağlı değil'}
            </Fact>
            <Fact label="SMS gönderen adı">{smsSenderText(brand.smsSender)}</Fact>
          </dl>
        </section>
        <div className={styles.side}>
          <section
            className={`${cardStyles.card ?? ''} ${cardStyles.flush ?? ''}`}
            aria-labelledby="kontor-hareketleri"
          >
            <div className={cardStyles.cardHead}>
              <h2 id="kontor-hareketleri" className={cardStyles.cardTitle}>
                Kontör hareketleri
              </h2>
              <span className={cardStyles.muted}>son 30 gün</span>
            </div>
            <CreditTransactionsTable rows={brand.recentCreditTransactions} />
          </section>
          <aside className={styles.auditNote}>
            <ShieldCheck size={16} aria-hidden="true" />
            <p>
              <strong>Bu görüntüleme denetim kaydına yazıldı.</strong>
              {editsLocked
                ? 'Rolünüz görüntüler; kontör, plan ve durum değişiklikleri yönetici yetkisi ister. '
                : ''}
              Müşteri telefonları maskelenir.
            </p>
          </aside>
        </div>
      </div>
    </div>
  );
}

function Fact({ label, children }: { label: string; children: ReactNode }) {
  return (
    <div className={styles.fact}>
      <dt>{label}</dt>
      <dd>{children}</dd>
    </div>
  );
}

function subscriptionSub(subscription: BrandDetail['subscription']): string {
  const plan = `${subscription.planName} planı`;
  switch (subscription.status) {
    case 'Trialing':
      return subscription.trialDaysLeft === null
        ? plan
        : `${plan} · ${formatCount(subscription.trialDaysLeft)} gün kaldı`;
    case 'PastDue':
      return subscription.readOnlyOn
        ? `${plan} · salt okunur: ${formatDate(subscription.readOnlyOn, 'dayMonth')}`
        : plan;
    default:
      return `${plan} · ${subscription.billingCycle === 'Yearly' ? 'yıllık' : 'aylık'}`;
  }
}

function smsSenderText(sender: BrandDetail['smsSender']): string {
  const effective =
    sender.effectiveName === 'OMVILA' ? 'OMVILA (varsayılan)' : sender.effectiveName;
  switch (sender.status) {
    case 'None':
      return `${effective} · başvuru yok`;
    case 'Pending':
      return `${effective} · ${sender.name ?? ''} onay bekliyor`;
    case 'Rejected':
      return `${effective} · ${sender.name ?? ''} reddedildi`;
    case 'Approved':
      return effective;
  }
}
