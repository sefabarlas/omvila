import { formatCount, formatMoney } from '@omvila/ui-web/format';
import { Badge, DataTable, EmptyState, Skeleton } from '@omvila/ui-web/panel';
import { useQuery } from '@tanstack/react-query';
import { Check, Package, Pencil } from 'lucide-react';
import { useState } from 'react';
import { useApi } from '@/app/api-context';
import { plansQuery } from '@/features/brands/api';
import brandStyles from '@/features/brands/Brands.module.css';
import cardStyles from '@/shared/Card.module.css';
import { Page } from '@/shared/Page';
import { adminOrFinanceHint, PermissionButton } from '@/shared/PermissionButton';
import { QueryState } from '@/shared/QueryState';
import { creditPackagesQuery } from './api';
import type { PackageCatalog, Plan, PlanCatalog } from './api';
import { EditPackagesModal } from './EditPackagesModal';
import { EditPlansModal } from './EditPlansModal';
import { planLines, planPriceText } from './labels';
import styles from './Plans.module.css';

const cx = (...names: (string | false | undefined)[]) => names.filter(Boolean).join(' ');

/** En çok markanın olduğu plan vurgulanır (tasarımda koyu kart); hiç marka yoksa vurgu yok. */
function featuredCode(plans: Plan[]): string | null {
  const top = plans.reduce<Plan | null>(
    (best, plan) => (plan.brandCount > (best?.brandCount ?? 0) ? plan : best),
    null,
  );
  return top?.code ?? null;
}

/**
 * platform-paneli/PPlanlar: plan kartları, ortak kurallar ve kontör paketleri. Fiyat ve sınırlar veriden (S-14);
 * yoksa "belirlenmedi". Düzenleme Yönetici ve Finans'ta (platform.plans.manage); Destek görür, buton kilitli.
 */
export function PlansPage() {
  const api = useApi();
  const plans = useQuery(plansQuery(api));
  const packages = useQuery(creditPackagesQuery(api));
  const [dialog, setDialog] = useState<'plans' | 'packages' | null>(null);

  return (
    <Page
      title="Planlar ve fiyat"
      context="Yeni kayıtlar için geçerli · mevcut aboneler korunur"
      action={
        <PermissionButton
          permission="platform.plans.manage"
          lockedHint={adminOrFinanceHint}
          icon={<Pencil size={16} aria-hidden="true" />}
          disabled={!plans.data}
          onClick={() => {
            setDialog('plans');
          }}
        >
          Planı düzenle
        </PermissionButton>
      }
    >
      <QueryState
        query={plans}
        loading={<Skeleton variant="block" height="calc(var(--bosluk-12) * 4)" />}
      >
        {(catalog) => <PlanCards catalog={catalog} />}
      </QueryState>
      <div className={styles.tables}>
        {plans.data ? <Rules catalog={plans.data} /> : <span />}
        <section className={cx(cardStyles.card, cardStyles.flush)} aria-labelledby="paket-baslik">
          <div className={cardStyles.cardHead}>
            <h2 id="paket-baslik" className={cardStyles.cardTitle}>
              Kontör paketleri
            </h2>
            <PermissionButton
              size="sm"
              permission="platform.plans.manage"
              lockedHint={adminOrFinanceHint}
              disabled={!packages.data}
              onClick={() => {
                setDialog('packages');
              }}
            >
              Paketleri düzenle
            </PermissionButton>
          </div>
          <QueryState query={packages}>
            {(catalog) => <PackagesTable catalog={catalog} />}
          </QueryState>
        </section>
      </div>
      {dialog === 'plans' && plans.data ? (
        <EditPlansModal
          catalog={plans.data}
          onRefresh={() => void plans.refetch()}
          onClose={() => {
            setDialog(null);
          }}
        />
      ) : null}
      {dialog === 'packages' && packages.data ? (
        <EditPackagesModal
          catalog={packages.data}
          onRefresh={() => void packages.refetch()}
          onClose={() => {
            setDialog(null);
          }}
        />
      ) : null}
    </Page>
  );
}

function PlanCards({ catalog }: { catalog: PlanCatalog }) {
  const featured = featuredCode(catalog.plans);
  return (
    <ul className={styles.plans} aria-label="Planlar">
      {catalog.plans.map((plan) => (
        <li
          key={plan.code}
          className={cx(styles.plan, plan.code === featured && styles.featured)}
          aria-labelledby={`plan-${plan.code}`}
        >
          <div className={styles.planHead}>
            <h2 id={`plan-${plan.code}`} className={styles.planName}>
              {plan.name}
            </h2>
            <span className={styles.count}>{`${formatCount(plan.brandCount)} marka`}</span>
          </div>
          {plan.description ? <p className={styles.description}>{plan.description}</p> : null}
          <p className={cx(styles.price, !plan.monthlyPrice && styles.unpriced)}>
            {planPriceText(plan)}
          </p>
          <ul className={styles.lines}>
            {planLines(plan).map((line) => (
              <li key={line}>
                <Check size={16} aria-hidden="true" />
                {line}
              </li>
            ))}
          </ul>
          {plan.isActive ? null : (
            <Badge tone="notr" className={cx(styles.inactive)}>
              Yeni kayıtlara kapalı
            </Badge>
          )}
        </li>
      ))}
    </ul>
  );
}

function Rules({ catalog }: { catalog: PlanCatalog }) {
  const { trialDays, pastDueGraceDays, trialCredits } = catalog.rules;
  const rows: [string, string][] = [
    ['Deneme süresi', `${formatCount(trialDays)} gün`],
    ['Ödeme gecikmesinde', `${formatCount(pastDueGraceDays)} gün hatırlatma, sonra salt okunur`],
    [
      'Deneme hediyesi',
      trialCredits === null ? 'Belirlenmedi' : `${formatCount(trialCredits)} SMS`,
    ],
    ['Kanallar', 'Push, SMS, e-posta'],
  ];
  return (
    <section className={cardStyles.card} aria-labelledby="kural-baslik">
      <div className={cardStyles.cardHead}>
        <h2 id="kural-baslik" className={cardStyles.cardTitle}>
          Ortak kurallar
        </h2>
      </div>
      <dl className={brandStyles.facts}>
        {rows.map(([label, value]) => (
          <div key={label} className={brandStyles.fact}>
            <dt>{label}</dt>
            <dd>{value}</dd>
          </div>
        ))}
      </dl>
    </section>
  );
}

function PackagesTable({ catalog }: { catalog: PackageCatalog }) {
  return (
    <DataTable
      caption="Kontör paketleri"
      rows={catalog.packages}
      rowKey={(p) => p.code}
      columns={[
        {
          key: 'name',
          label: 'Paket',
          render: (p) => (p.isActive ? p.name : `${p.name} · satışta değil`),
        },
        { key: 'sms', label: 'SMS', numeric: true, render: (p) => formatCount(p.credits) },
        {
          key: 'price',
          label: 'Fiyat',
          numeric: true,
          render: (p) => (p.price ? formatMoney(p.price) : 'Belirlenmedi'),
        },
      ]}
      empty={
        <EmptyState
          icon={<Package size={22} />}
          title="Kontör paketi yok"
          description="Paketler yapılandırmayla gelir."
        />
      }
    />
  );
}
