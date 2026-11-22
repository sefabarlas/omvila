import { ApiProblem } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { formatMoney } from '@omvila/ui-web/format';
import {
  Avatar,
  Badge,
  Bento,
  BentoBlock,
  Button,
  DataTable,
  EmptyState,
  Skeleton,
  StatusBadge,
  Tabs,
} from '@omvila/ui-web/panel';
import { displayStatus } from '@omvila/ui-web/status';
import { branchToday } from '@omvila/ui-web/time';
import { useInfiniteQuery, useQuery } from '@tanstack/react-query';
import { CalendarDays, CalendarPlus, GitMerge, ReceiptText, Trash2 } from 'lucide-react';
import { useState } from 'react';
import { useApi } from '@/app/api-context';
import { useSelectedBranch } from '@/app/branch';
import { useClock } from '@/app/clock';
import { defaultTimeZone } from '@/app/config';
import { useSession } from '@/app/session';
import { NewAppointmentButton } from '@/features/appointments/NewAppointmentButton';
import { NewAppointmentModal } from '@/features/appointments/NewAppointmentModal';
import { SellPackageModal } from '@/features/packages/SellPackageModal';
import { NotFoundPage } from '@/shared/NotFoundPage';
import { Page } from '@/shared/Page';
import { customerAppointmentsQuery, customerQuery } from './api';
import type { CustomerAppointmentRow, CustomerSummary } from './api';
import styles from './Customers.module.css';
import { PackagesTab, PaymentsTab, ReviewsTab } from './CustomerTabs';
import { DeleteCustomerModal } from './DeleteCustomerModal';
import {
  activePackage,
  customerPhone,
  inlineDayLabel,
  instantLabel,
  shortDate,
  tagTone,
} from './labels';
import { MergeCustomersModal } from './MergeCustomersModal';
import { customerPackagesQuery } from './records-api';

const breadcrumb = 'Müşteriler / Müşteri kartı';

/**
 * Müşteri kartı (salon-paneli/MusteriKarti): kimlik, etiketler, metrik blokları (etkin paket kartı dahil), sekmeler
 * (Randevular, Paketler, Ödemeler: payment.collect, Değerlendirmeler: review.view), notlar ve "Kayıt işlemleri"
 * (birleştir: customer.merge, KVKK silme: customer.kvkk). "Randevu ver" yeni randevu ekranını müşteri seçili açar
 * (appointment.create, T-58). "Müşteri since", ilk kanal, tercih ettiği personel, KVKK izinleri ve "SMS gönder"
 * sözleşmede yok; gizli (acik-sorular.md T-57, T-99). "Paket sat" (package.sell) tasarımda yok: "SMS gönder"in yerinde
 * ikincil buton, satış penceresini müşteri seçili açar (T-102).
 */
export function CustomerCardPage({ brandCustomerId }: { brandCustomerId: string }) {
  const api = useApi();
  const query = useQuery(customerQuery(api, brandCustomerId));

  if (query.isError) {
    if (query.error instanceof ApiProblem && query.error.status === 404) {
      return <NotFoundPage />;
    }

    return (
      <Page title="Müşteri kartı" context={breadcrumb}>
        <section className={styles.emptyCard} aria-label="Hata">
          <EmptyState
            headingLevel={2}
            title="Müşteri kartı yüklenemedi"
            description={errorMessage(
              query.error instanceof ApiProblem ? query.error.code : undefined,
            )}
            actions={<Button onClick={() => void query.refetch()}>Tekrar dene</Button>}
          />
        </section>
      </Page>
    );
  }

  if (!query.data) {
    return (
      <Page title="Müşteri kartı" context={breadcrumb}>
        <Skeleton variant="block" height="320px" />
      </Page>
    );
  }

  return <CustomerCard customer={query.data} />;
}

function CustomerCard({ customer }: { customer: CustomerSummary }) {
  const clock = useClock();
  const { session, can } = useSession();
  const { branchId: selectedBranch } = useSelectedBranch();
  const branchId = selectedBranch ?? session.branches[0]?.id ?? '';
  const api = useApi();
  const [booking, setBooking] = useState(false);
  const [tab, setTab] = useState('randevular');
  const [dialog, setDialog] = useState<'merge' | 'delete' | 'sell' | null>(null);
  const packages = useInfiniteQuery(customerPackagesQuery(api, customer.id));
  const active = activePackage(packages.data?.pages.flatMap((page) => page.items) ?? []);
  const tabs = [
    { value: 'randevular', label: 'Randevular' },
    { value: 'paketler', label: 'Paketler' },
    ...(can('payment.collect') ? [{ value: 'odemeler', label: 'Ödemeler' }] : []),
    ...(can('review.view') ? [{ value: 'degerlendirmeler', label: 'Değerlendirmeler' }] : []),
  ];
  const canMerge = can('customer.merge');
  const canDelete = can('customer.kvkk');
  const sellButton =
    can('package.sell') && branchId ? (
      <Button
        icon={<ReceiptText size={16} aria-hidden="true" />}
        onClick={() => {
          setDialog('sell');
        }}
      >
        Paket sat
      </Button>
    ) : null;
  const bookButton =
    can('appointment.create') && branchId ? (
      <NewAppointmentButton
        label="Randevu ver"
        icon={<CalendarPlus size={16} aria-hidden="true" />}
        readOnly={session.subscription.status === 'ReadOnly'}
        onClick={() => {
          setBooking(true);
        }}
      />
    ) : null;
  const today = branchToday(clock, defaultTimeZone);
  const phone = customerPhone(customer);
  const visits = customer.visitCount;
  const total = customer.totalSpent;
  // Ortalama yalnız gösterim içindir: tam liraya yuvarlanır (para hesabı değil).
  const average =
    total && visits > 0
      ? formatMoney({
          amountMinor: Math.round(total.amountMinor / visits / 100) * 100,
          currency: total.currency,
        })
      : null;
  const rows: { label: string; value: string }[] = [
    ...(customer.nextAppointmentUtc
      ? [
          {
            label: 'Sonraki randevu',
            value: instantLabel(customer.nextAppointmentUtc, today, defaultTimeZone),
          },
        ]
      : []),
    ...(customer.birthYear ? [{ label: 'Doğum yılı', value: String(customer.birthYear) }] : []),
  ];

  return (
    <Page
      title={customer.fullName}
      context={breadcrumb}
      {...(sellButton || bookButton
        ? {
            action: (
              <div className={styles.topActions}>
                {sellButton}
                {bookButton}
              </div>
            ),
          }
        : {})}
    >
      {booking ? (
        <NewAppointmentModal
          branchId={branchId}
          prefill={{ customer }}
          onClose={() => {
            setBooking(false);
          }}
        />
      ) : null}
      {dialog === 'merge' ? (
        <MergeCustomersModal
          customer={customer}
          onClose={() => {
            setDialog(null);
          }}
        />
      ) : null}
      {dialog === 'sell' ? (
        <SellPackageModal
          branchId={branchId}
          customer={customer}
          onClose={() => {
            setDialog(null);
          }}
        />
      ) : null}
      {dialog === 'delete' ? (
        <DeleteCustomerModal
          customer={customer}
          onClose={() => {
            setDialog(null);
          }}
        />
      ) : null}
      <div className={styles.cardLayout}>
        <aside className={styles.side}>
          <section className={styles.panel} aria-label="Müşteri">
            <div className={styles.identity}>
              <Avatar name={customer.fullName} size="lg" />
              <div className={styles.identityText}>
                <strong className={styles.identityName}>{customer.fullName}</strong>
                {phone ? <span className={styles.muted}>{phone}</span> : null}
              </div>
            </div>
            {customer.tags.length > 0 ? (
              <span className={styles.tags}>
                {customer.tags.map((tag) => (
                  <Badge key={tag} tone={tagTone(tag)}>
                    {tag}
                  </Badge>
                ))}
              </span>
            ) : null}
            {rows.length > 0 ? (
              <dl className={styles.rows}>
                {rows.map((row) => (
                  <div key={row.label} className={styles.row}>
                    <dt>{row.label}</dt>
                    <dd>{row.value}</dd>
                  </div>
                ))}
              </dl>
            ) : null}
          </section>
          {canMerge || canDelete ? (
            <section className={styles.panel} aria-labelledby="musteri-kayit-islemleri">
              <h2 id="musteri-kayit-islemleri" className={styles.recordTitle}>
                Kayıt işlemleri
              </h2>
              <ul className={styles.recordActions}>
                {canMerge ? (
                  <li>
                    <Button
                      size="sm"
                      variant="ghost"
                      icon={<GitMerge size={16} aria-hidden="true" />}
                      onClick={() => {
                        setDialog('merge');
                      }}
                    >
                      Başka kayıtla birleştir
                    </Button>
                  </li>
                ) : null}
                {canDelete ? (
                  <li>
                    <Button
                      size="sm"
                      variant="ghost"
                      className={styles.dangerAction}
                      icon={<Trash2 size={16} aria-hidden="true" />}
                      onClick={() => {
                        setDialog('delete');
                      }}
                    >
                      KVKK silme talebi
                    </Button>
                  </li>
                ) : null}
              </ul>
            </section>
          ) : null}
        </aside>
        <div className={styles.main}>
          <Bento label="Müşteri özeti">
            <BentoBlock
              tone="marka"
              label="Ziyaret"
              value={String(visits)}
              sub={
                customer.lastVisitOn
                  ? `son: ${inlineDayLabel(customer.lastVisitOn, today)}`
                  : 'henüz ziyaret yok'
              }
            />
            <BentoBlock
              label="Toplam"
              value={total ? formatMoney(total) : '—'}
              {...(average ? { sub: `ort. ${average}` } : {})}
            />
            <BentoBlock
              label="Gelmedi"
              value={String(customer.noShowCount)}
              sub={
                customer.noShowCount === 0
                  ? 'hiç'
                  : customer.isNoShowRisk
                    ? 'gelmeme riski'
                    : 'randevuya gelmedi'
              }
            />
            {active ? (
              <BentoBlock
                tone="kum"
                label={active.packageName}
                value={`${String(active.remainingSessions)}/${String(active.totalSessions)}`}
                sub={`seans kaldı · ${shortDate(active.expiresOn)}`}
              />
            ) : null}
          </Bento>
          <Tabs
            label="Müşteri kartı"
            idPrefix="musteri"
            items={tabs}
            value={tab}
            onChange={setTab}
          />
          <div role="tabpanel" id={`musteri-panel-${tab}`} aria-labelledby={`musteri-tab-${tab}`}>
            {tab === 'paketler' ? (
              <PackagesTab brandCustomerId={customer.id} today={today} />
            ) : tab === 'odemeler' ? (
              <PaymentsTab brandCustomerId={customer.id} today={today} />
            ) : tab === 'degerlendirmeler' ? (
              <ReviewsTab brandCustomerId={customer.id} today={today} />
            ) : (
              <AppointmentsTab brandCustomerId={customer.id} today={today} />
            )}
          </div>
          {customer.notes ? (
            <section className={styles.panel} aria-labelledby="musteri-notlar">
              <h2 id="musteri-notlar" className={styles.panelTitle}>
                Notlar
              </h2>
              <p className={styles.notes}>{customer.notes}</p>
            </section>
          ) : null}
        </div>
      </div>
    </Page>
  );
}

function AppointmentsTab({ brandCustomerId, today }: { brandCustomerId: string; today: string }) {
  const api = useApi();
  const query = useInfiniteQuery(customerAppointmentsQuery(api, brandCustomerId));
  if (query.isPending) {
    return <Skeleton variant="block" height="200px" />;
  }

  if (query.isError) {
    return (
      <section className={styles.emptyCard} aria-label="Hata">
        <EmptyState
          headingLevel={3}
          title="Randevular yüklenemedi"
          description={errorMessage(
            query.error instanceof ApiProblem ? query.error.code : undefined,
          )}
          actions={<Button onClick={() => void query.refetch()}>Tekrar dene</Button>}
        />
      </section>
    );
  }

  const rows = query.data.pages.flatMap((page) => page.items);
  return (
    <DataTable<CustomerAppointmentRow>
      caption="Randevular"
      rows={rows}
      rowKey={(row) => row.appointmentId}
      empty={
        <EmptyState
          icon={<CalendarDays size={24} />}
          headingLevel={3}
          title="Henüz randevu yok"
          description="Bu müşterinin randevuları burada yeniden eskiye listelenir."
        />
      }
      columns={[
        {
          key: 'date',
          label: 'Tarih',
          render: (row) => instantLabel(row.startUtc, today, defaultTimeZone),
        },
        {
          key: 'service',
          label: 'Hizmet',
          render: (row) => <strong>{row.serviceNames.join(', ')}</strong>,
        },
        { key: 'staff', label: 'Personel', render: (row) => row.staffNames.join(', ') || '—' },
        {
          key: 'status',
          label: 'Durum',
          render: (row) => {
            const status = displayStatus(row.displayStatus);
            return status ? <StatusBadge status={status} /> : null;
          },
        },
        {
          key: 'total',
          label: 'Tutar',
          numeric: true,
          render: (row) => (
            <strong>{row.status === 'Cancelled' ? '—' : formatMoney(row.total)}</strong>
          ),
        },
      ]}
      {...(query.hasNextPage
        ? {
            footer: (
              <div className={styles.footer}>
                <span>{`${String(rows.length)} randevu gösteriliyor`}</span>
                <Button
                  size="sm"
                  loading={query.isFetchingNextPage}
                  loadingLabel="Yükleniyor…"
                  onClick={() => void query.fetchNextPage()}
                >
                  Daha fazla göster
                </Button>
              </div>
            ),
          }
        : {})}
    />
  );
}
