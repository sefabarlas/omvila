import { ApiProblem } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { formatCount, formatMoney } from '@omvila/ui-web/format';
import {
  Avatar,
  Badge,
  Bento,
  BentoBlock,
  Button,
  DataTable,
  EmptyState,
  Skeleton,
  Tabs,
  TextField,
  useToast,
} from '@omvila/ui-web/panel';
import { keepPreviousData, useInfiniteQuery, useQuery } from '@tanstack/react-query';
import { Package, Plus, ReceiptText, Search } from 'lucide-react';
import { useEffect, useState } from 'react';
import { useApi } from '@/app/api-context';
import { useSelectedBranch } from '@/app/branch';
import { useSession } from '@/app/session';
import { FilterChips } from '@/features/customers/FilterChips';
import { packageStatusBadge, shortDate } from '@/features/customers/labels';
import { Page } from '@/shared/Page';
import { allCustomerPackagesQuery, packagesQuery, toPackageWrite, useUpdatePackage } from './api';
import type { CustomerPackage, CustomerPackageFilter, PackageDefinition, PackageList } from './api';
import { customerPackageFilters, expiringSoonDays, validityLabel } from './labels';
import { PackageFormModal } from './PackageFormModal';
import styles from './Packages.module.css';
import { RefundPackageModal } from './RefundPackageModal';
import { RowMenu } from './RowMenu';
import { SellPackageModal } from './SellPackageModal';

const searchDelayMs = 300;
const expiringShown = 5;

type Dialog =
  | { kind: 'create' }
  | { kind: 'edit'; pkg: PackageDefinition }
  | { kind: 'sell' }
  | { kind: 'refund'; customerPackage: CustomerPackage };

/**
 * Paketler (salon-paneli/Paketler, PaketlerBos; dilim-05 KK6): üst kartlar, "Paket tanımları" ve "Müşteri paketleri"
 * sekmeleri, "Süresi yaklaşanlar". Görmek için package.manage veya package.sell; tanım yazma (oluştur, düzenle,
 * satıştan kaldır) yalnız package.manage, satış ve iade package.sell. "Hepsine SMS" kapsam dışı (T-102).
 */
export function PackagesPage() {
  const api = useApi();
  const { session, can } = useSession();
  const { branchId: selected } = useSelectedBranch();
  const branchId = selected ?? session.branches[0]?.id ?? '';
  const branchName = session.branches.find((b) => b.id === branchId)?.name ?? '';
  const query = useQuery({ ...packagesQuery(api, branchId), enabled: branchId !== '' });
  const [dialog, setDialog] = useState<Dialog | null>(null);
  const canManage = can('package.manage');
  const canSell = can('package.sell');
  const count = query.data?.items.length ?? 0;
  const hasOnSale = query.data?.items.some((p) => p.isActive) ?? false;
  const close = () => {
    setDialog(null);
  };

  const sellButton =
    canSell && hasOnSale ? (
      <Button
        variant={canManage ? 'secondary' : 'primary'}
        icon={<ReceiptText size={16} aria-hidden="true" />}
        onClick={() => {
          setDialog({ kind: 'sell' });
        }}
      >
        Müşteriye paket sat
      </Button>
    ) : null;
  const createButton = canManage ? (
    <Button
      variant="primary"
      icon={<Plus size={16} aria-hidden="true" />}
      onClick={() => {
        setDialog({ kind: 'create' });
      }}
    >
      Paket oluştur
    </Button>
  ) : null;

  return (
    <Page
      title="Paketler"
      context={count > 0 ? `${String(count)} paket tanımı · ${branchName}` : branchName}
      action={
        <div className={styles.actions}>
          {sellButton}
          {createButton}
        </div>
      }
    >
      {dialog?.kind === 'create' ? <PackageFormModal onClose={close} /> : null}
      {dialog?.kind === 'edit' ? <PackageFormModal pkg={dialog.pkg} onClose={close} /> : null}
      {dialog?.kind === 'sell' ? <SellPackageModal branchId={branchId} onClose={close} /> : null}
      {dialog?.kind === 'refund' ? (
        <RefundPackageModal customerPackage={dialog.customerPackage} onClose={close} />
      ) : null}
      {query.isPending ? (
        <>
          <Skeleton variant="block" height="120px" />
          <Skeleton variant="block" height="320px" />
        </>
      ) : query.isError ? (
        <section className={styles.card} aria-label="Hata">
          <EmptyState
            headingLevel={2}
            title="Paketler yüklenemedi"
            description={errorMessage(
              query.error instanceof ApiProblem ? query.error.code : undefined,
            )}
            actions={<Button onClick={() => void query.refetch()}>Tekrar dene</Button>}
          />
        </section>
      ) : count === 0 ? (
        <section className={styles.card} aria-label="Paket yok">
          <EmptyState
            icon={<Package size={24} />}
            headingLevel={2}
            title="Henüz paket tanımı yok"
            description="Seans paketleri (ör. 5 fön, 8 seans lazer) müşterinin sık gelmesini sağlar. Satılan paketler müşterinin uygulamasında kalan seanslarla görünür."
            {...(createButton ? { actions: createButton } : {})}
          />
        </section>
      ) : (
        <PackagesContent
          data={query.data}
          branchId={branchId}
          canManage={canManage}
          canSell={canSell}
          onEdit={(pkg) => {
            setDialog({ kind: 'edit', pkg });
          }}
          onRefund={(customerPackage) => {
            setDialog({ kind: 'refund', customerPackage });
          }}
        />
      )}
    </Page>
  );
}

function PackagesContent({
  data,
  branchId,
  canManage,
  canSell,
  onEdit,
  onRefund,
}: {
  data: PackageList;
  branchId: string;
  canManage: boolean;
  canSell: boolean;
  onEdit: (pkg: PackageDefinition) => void;
  onRefund: (customerPackage: CustomerPackage) => void;
}) {
  const [tab, setTab] = useState('tanimlar');
  const { overview } = data;
  return (
    <>
      <Bento label="Paket özeti">
        <BentoBlock
          tone="marka"
          label="Aktif müşteri paketi"
          value={formatCount(overview.activeCustomerPackages)}
          sub={`kalan ${formatCount(overview.remainingSessions)} seans`}
        />
        <BentoBlock
          label="Bu ay satılan"
          value={formatCount(overview.soldThisMonth)}
          sub={formatMoney(overview.soldThisMonthAmount)}
        />
        <BentoBlock
          tone="kum"
          label="Süresi yaklaşan"
          value={formatCount(overview.expiringSoon)}
          sub={`${String(expiringSoonDays)} gün içinde bitiyor`}
        />
        <BentoBlock
          label="Kullanım oranı"
          value={
            overview.usageRatePercent != null
              ? `%${String(Math.round(overview.usageRatePercent))}`
              : '—'
          }
          sub="süresi biten paketlerde"
        />
      </Bento>
      <div className={styles.grid}>
        <div className={styles.main}>
          <Tabs
            label="Paketler"
            idPrefix="paketler"
            items={[
              { value: 'tanimlar', label: 'Paket tanımları', count: data.items.length },
              {
                value: 'musteri',
                label: 'Müşteri paketleri',
                count: overview.activeCustomerPackages,
              },
            ]}
            value={tab}
            onChange={setTab}
          />
          <div
            role="tabpanel"
            id={`paketler-panel-${tab}`}
            aria-labelledby={`paketler-tab-${tab}`}
            className={styles.main}
          >
            {tab === 'musteri' ? (
              <CustomerPackagesTab branchId={branchId} canSell={canSell} onRefund={onRefund} />
            ) : (
              <DefinitionsTable items={data.items} canManage={canManage} onEdit={onEdit} />
            )}
          </div>
          <p className={styles.note}>
            Online randevuda paketli hizmet seçilince seans ayrılır; salonda ödeme alırken
            &quot;Paketten&quot; seçilir. Müşteri kalan seansını rezervasyon sayfasında görür.
          </p>
        </div>
        <ExpiringCard branchId={branchId} />
      </div>
    </>
  );
}

function DefinitionsTable({
  items,
  canManage,
  onEdit,
}: {
  items: readonly PackageDefinition[];
  canManage: boolean;
  onEdit: (pkg: PackageDefinition) => void;
}) {
  const update = useUpdatePackage();
  const toast = useToast();
  const toggle = (pkg: PackageDefinition) => {
    update.mutate(
      { pkg, body: { ...toPackageWrite(pkg), isActive: !pkg.isActive } },
      {
        onSuccess: (saved) => {
          toast.show({
            tone: 'basari',
            title: saved.isActive ? 'Paket satışa alındı' : 'Paket satıştan kaldırıldı',
            message: saved.name,
          });
        },
        onError: (error) => {
          toast.show({
            tone: 'hata',
            title:
              error instanceof ApiProblem && error.status === 412
                ? 'Kayıt çakışması. Liste yenilendi, tekrar deneyin.'
                : errorMessage(error instanceof ApiProblem ? error.code : undefined),
          });
        },
      },
    );
  };

  return (
    <DataTable<PackageDefinition>
      caption="Paket tanımları"
      rows={items}
      rowKey={(p) => p.id}
      columns={[
        { key: 'name', label: 'Paket', render: (p) => <strong>{p.name}</strong> },
        { key: 'sessions', label: 'Seans', numeric: true, render: (p) => p.totalSessions },
        {
          key: 'price',
          label: 'Fiyat',
          numeric: true,
          render: (p) => <strong>{formatMoney(p.price)}</strong>,
        },
        { key: 'validity', label: 'Geçerlilik', render: (p) => validityLabel(p.validityDays) },
        { key: 'sold', label: 'Satılan', numeric: true, render: (p) => p.soldCount },
        { key: 'active', label: 'Aktif', numeric: true, render: (p) => p.activeCount },
        {
          key: 'status',
          label: 'Durum',
          render: (p) =>
            p.isActive ? <Badge tone="marka">Satışta</Badge> : <Badge>Satışta değil</Badge>,
        },
        {
          key: 'actions',
          label: 'İşlemler',
          align: 'right',
          render: (p) =>
            canManage ? (
              <RowMenu
                label={`${p.name} işlemleri`}
                items={[
                  {
                    label: 'Düzenle',
                    onSelect: () => {
                      onEdit(p);
                    },
                  },
                  {
                    label: p.isActive ? 'Satıştan kaldır' : 'Satışa al',
                    onSelect: () => {
                      toggle(p);
                    },
                  },
                ]}
              />
            ) : null,
        },
      ]}
    />
  );
}

function CustomerPackagesTab({
  branchId,
  canSell,
  onRefund,
}: {
  branchId: string;
  canSell: boolean;
  onRefund: (customerPackage: CustomerPackage) => void;
}) {
  const api = useApi();
  const [filter, setFilter] = useState<CustomerPackageFilter>('Active');
  const [text, setText] = useState('');
  const [q, setQ] = useState('');
  useEffect(() => {
    const timer = setTimeout(() => {
      setQ(text.trim());
    }, searchDelayMs);
    return () => {
      clearTimeout(timer);
    };
  }, [text]);
  const query = useInfiniteQuery({
    ...allCustomerPackagesQuery(api, branchId, filter, q),
    placeholderData: keepPreviousData,
  });
  const rows = query.data?.pages.flatMap((page) => page.items) ?? [];

  return (
    <>
      <div className={styles.toolbar}>
        <FilterChips<CustomerPackageFilter>
          legend="Müşteri paketleri süzgeci"
          name="musteri-paketleri-suzgec"
          options={customerPackageFilters}
          value={filter}
          onChange={setFilter}
        />
        <TextField
          label="Paket adında ara"
          hideLabel
          type="search"
          placeholder="Paket adında ara: Fön"
          autoComplete="off"
          icon={<Search size={16} />}
          className={styles.search}
          value={text}
          onChange={(event) => {
            setText(event.target.value);
          }}
        />
      </div>
      {query.isPending ? (
        <Skeleton variant="block" height="240px" />
      ) : query.isError ? (
        <section className={styles.card} aria-label="Hata">
          <EmptyState
            headingLevel={3}
            title="Müşteri paketleri yüklenemedi"
            description={errorMessage(
              query.error instanceof ApiProblem ? query.error.code : undefined,
            )}
            actions={<Button onClick={() => void query.refetch()}>Tekrar dene</Button>}
          />
        </section>
      ) : (
        <DataTable<CustomerPackage>
          caption="Müşteri paketleri"
          rows={rows}
          rowKey={(p) => p.id}
          empty={
            <EmptyState
              icon={<Package size={24} />}
              headingLevel={3}
              title={q ? `"${q}" adında paket yok` : 'Bu süzgeçte paket yok'}
              description="Satılan paketler ve kalan seanslar burada görünür."
            />
          }
          columns={[
            {
              key: 'customer',
              label: 'Müşteri',
              render: (p) => <strong>{p.customer.fullName}</strong>,
            },
            { key: 'package', label: 'Paket', render: (p) => p.packageName },
            {
              key: 'remaining',
              label: 'Kalan',
              numeric: true,
              render: (p) => `${String(p.remainingSessions)}/${String(p.totalSessions)} seans`,
            },
            { key: 'expires', label: 'Son gün', render: (p) => shortDate(p.expiresOn) },
            {
              key: 'status',
              label: 'Durum',
              render: (p) => {
                const badge = packageStatusBadge(p.status);
                return <Badge tone={badge.tone}>{badge.label}</Badge>;
              },
            },
            {
              key: 'price',
              label: 'Tutar',
              numeric: true,
              render: (p) => <strong>{formatMoney(p.price)}</strong>,
            },
            {
              key: 'actions',
              label: 'İşlemler',
              align: 'right',
              render: (p) =>
                canSell && (p.status === 'Active' || p.status === 'Expired') ? (
                  <RowMenu
                    label={`${p.customer.fullName} · ${p.packageName} işlemleri`}
                    items={[
                      {
                        label: 'İade et',
                        danger: true,
                        onSelect: () => {
                          onRefund(p);
                        },
                      },
                    ]}
                  />
                ) : null,
            },
          ]}
          {...(query.hasNextPage
            ? {
                footer: (
                  <div className={styles.footer}>
                    <span>{`${String(rows.length)} paket gösteriliyor`}</span>
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
      )}
    </>
  );
}

/** "Süresi yaklaşanlar": en yakın biten beş etkin paket. "Hepsine SMS" bildirim kataloğunda yok (T-86, T-102). */
function ExpiringCard({ branchId }: { branchId: string }) {
  const api = useApi();
  const query = useInfiniteQuery(
    allCustomerPackagesQuery(api, branchId, 'ExpiringSoon', '', expiringShown),
  );
  const rows = (query.data?.pages[0]?.items ?? []).slice(0, expiringShown);
  return (
    <section className={styles.side} aria-labelledby="paketler-suresi-yaklasan">
      <h2 id="paketler-suresi-yaklasan" className={styles.sideTitle}>
        Süresi yaklaşanlar
      </h2>
      {query.isPending ? (
        <Skeleton variant="block" height="160px" />
      ) : query.isError ? (
        <p className={styles.hint}>Liste yüklenemedi.</p>
      ) : rows.length === 0 ? (
        <p className={styles.hint}>
          {`Önümüzdeki ${String(expiringSoonDays)} günde süresi biten paket yok.`}
        </p>
      ) : (
        <ul className={styles.expiring}>
          {rows.map((p) => (
            <li key={p.id} className={styles.expiringRow}>
              <Avatar name={p.customer.fullName} size="sm" />
              <span className={styles.expiringText}>
                <strong>{p.customer.fullName}</strong>
                <span className={styles.muted}>
                  {`${p.packageName} · ${String(p.remainingSessions)} seans kaldı`}
                </span>
              </span>
              <span className={styles.expiringDate}>{shortDate(p.expiresOn)}</span>
            </li>
          ))}
        </ul>
      )}
    </section>
  );
}
