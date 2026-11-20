import { Banner, Button, useToast } from '@omvila/ui-web/panel';
import { errorMessage } from '@omvila/ui-web/errors';
import { moneyInputValue } from '@omvila/ui-web/format';
import { ApiProblem } from '@omvila/api-client-ts';
import { useQuery, useSuspenseQueries, useSuspenseQuery } from '@tanstack/react-query';
import { Plus } from 'lucide-react';
import { Suspense, useState } from 'react';
import { useApi } from '@/app/api-context';
import { useSelectedBranch } from '@/app/branch';
import { useSession } from '@/app/session';
import { brandQuery, brandSettingsQuery } from '@/features/settings/api';
import { industries } from '@/shared/industries';
import { Page } from '@/shared/Page';
import {
  serviceCategoriesQuery,
  serviceQuery,
  servicesQuery,
  serviceTemplatesQuery,
  staffQuery,
  useToggleOnline,
} from './api';
import type { Service, ServiceCategory, StaffSummary } from './api';
import { CategoryModal } from './CategoryModal';
import { emptyServiceForm, serviceToForm } from './service-form';
import { ServiceModal } from './ServiceModal';
import type { BranchPriceContext } from './ServiceModal';
import styles from './Services.module.css';
import { ServicesEmpty } from './ServicesEmpty';
import { ServiceTable } from './ServiceTable';
import { TemplatesModal } from './TemplatesModal';

type Dialog =
  | { kind: 'none' }
  | { kind: 'create' }
  | { kind: 'edit'; row: Service }
  | { kind: 'category' }
  | { kind: 'templates' };

/**
 * Hizmetler (salon-paneli/Hizmetler, HizmetlerBos): kategoriler, seçili şubenin fiyatlarıyla hizmet tablosu, hazır
 * liste, hizmet ve kategori ekleme.
 */
export function ServicesPage() {
  const api = useApi();
  const toast = useToast();
  const { session } = useSession();
  const { branchId: selected } = useSelectedBranch();
  const branchId = selected ?? session.branches[0]?.id;
  const branchName = session.branches.find((b) => b.id === branchId)?.name ?? '';
  const multiBranch = session.branches.length > 1;
  const [servicesResult, categoriesResult, staffResult, settingsResult, brandResult] =
    useSuspenseQueries({
      queries: [
        servicesQuery(api, branchId),
        serviceCategoriesQuery(api),
        staffQuery(api),
        brandSettingsQuery(api),
        brandQuery(api),
      ],
    });
  const services = servicesResult.data.items;
  const categories = [...categoriesResult.data.items].sort((a, b) => a.sortOrder - b.sortOrder);
  const staff = staffResult.data.items.filter((s) => s.isActive);
  const depositAvailable = settingsResult.data.depositAvailable === true;
  const industryCode = brandResult.data.industryCode;
  const industryLabel = industries.find((i) => i.value === industryCode)?.label ?? 'Sektör';
  const templates =
    useQuery({ ...serviceTemplatesQuery(api, industryCode), enabled: services.length === 0 }).data
      ?.items ?? [];
  const toggle = useToggleOnline();
  const [dialog, setDialog] = useState<Dialog>({ kind: 'none' });
  const [categoryId, setCategoryId] = useState<string | undefined>(categories[0]?.id);
  const activeCategory = categories.find((c) => c.id === categoryId) ?? categories[0];
  const close = () => {
    setDialog({ kind: 'none' });
  };

  const onToggle = async (service: Service, online: boolean) => {
    try {
      await toggle.mutateAsync({ id: service.id, online });
    } catch (problem) {
      toast.show({
        tone: 'hata',
        title: errorMessage(problem instanceof ApiProblem ? problem.code : undefined),
      });
    }
  };

  const context =
    services.length === 0
      ? `${branchName} fiyatları`
      : `${String(services.length)} hizmet · ${String(categories.length)} kategori · ${branchName} fiyatları`;
  const visible = services.filter((s) => s.categoryId === activeCategory?.id);
  const defaults = emptyServiceForm({
    categoryId: activeCategory?.id ?? '',
    staffIds: staff.filter((s) => s.isBookableOnline).map((s) => s.id),
  });

  return (
    <Page
      title="Hizmetler"
      context={context}
      action={
        <>
          <Button
            onClick={() => {
              setDialog({ kind: 'category' });
            }}
          >
            Kategori ekle
          </Button>
          <Button
            variant="primary"
            icon={<Plus size={16} aria-hidden="true" />}
            onClick={() => {
              setDialog({ kind: 'create' });
            }}
          >
            Hizmet ekle
          </Button>
        </>
      }
    >
      {services.length === 0 ? (
        <ServicesEmpty
          industryLabel={industryLabel}
          templateCount={templates.length}
          onTemplates={() => {
            setDialog({ kind: 'templates' });
          }}
          onBlank={() => {
            setDialog({ kind: 'create' });
          }}
        />
      ) : (
        <div className={styles.layout}>
          <CategoryNav
            categories={categories}
            services={services}
            active={activeCategory?.id}
            onSelect={setCategoryId}
            onAdd={() => {
              setDialog({ kind: 'category' });
            }}
          />
          <div className={styles.main}>
            {multiBranch ? (
              <Banner title={`Fiyatlar ${branchName} şubesi için.`}>
                Başka şubede farklı fiyat tanımlıysa o şubeyi seçince görünür. Personel ek ücretleri
                personel sayfasında.
              </Banner>
            ) : null}
            <ServiceTable
              services={visible}
              staff={staff}
              depositAvailable={depositAvailable}
              canEdit
              onEdit={(row) => {
                setDialog({ kind: 'edit', row });
              }}
              onToggleOnline={(row, online) => void onToggle(row, online)}
            />
            {visible.length > 0 ? (
              <p className={styles.note}>
                Online kapalı hizmetler rezervasyon sayfasında görünmez, yalnızca panelden eklenir.
              </p>
            ) : null}
          </div>
        </div>
      )}
      {dialog.kind === 'create' ? (
        <ServiceModal
          defaults={defaults}
          categories={categories}
          staff={staff}
          depositAvailable={depositAvailable}
          onClose={close}
        />
      ) : null}
      {dialog.kind === 'edit' ? (
        <Suspense fallback={null}>
          <EditService
            row={dialog.row}
            categories={categories}
            staff={staff}
            depositAvailable={depositAvailable}
            branchPrice={
              multiBranch && branchId
                ? {
                    branchId,
                    branchName,
                    current:
                      dialog.row.isBranchPrice === true ? dialog.row.price.amountMinor : null,
                  }
                : undefined
            }
            onClose={close}
          />
        </Suspense>
      ) : null}
      {dialog.kind === 'category' ? (
        <CategoryModal
          onClose={close}
          onCreated={(category) => {
            setCategoryId(category.id);
          }}
        />
      ) : null}
      {dialog.kind === 'templates' ? (
        <Suspense fallback={null}>
          <TemplatesModalLoader
            industryLabel={industryLabel}
            industryCode={industryCode}
            onClose={close}
          />
        </Suspense>
      ) : null}
    </Page>
  );
}

function CategoryNav({
  categories,
  services,
  active,
  onSelect,
  onAdd,
}: {
  categories: readonly ServiceCategory[];
  services: readonly Service[];
  active: string | undefined;
  onSelect: (id: string) => void;
  onAdd: () => void;
}) {
  return (
    <nav className={styles.categories} aria-label="Kategoriler">
      <ul className={styles.categoryList}>
        {categories.map((category) => (
          <li key={category.id}>
            <button
              type="button"
              className={styles.category}
              aria-pressed={category.id === active}
              onClick={() => {
                onSelect(category.id);
              }}
            >
              {category.name}
              <span className={styles.count}>
                {services.filter((s) => s.categoryId === category.id).length}
              </span>
            </button>
          </li>
        ))}
      </ul>
      <div className={styles.categoryFoot}>
        <Button
          variant="ghost"
          size="sm"
          icon={<Plus size={16} aria-hidden="true" />}
          onClick={onAdd}
        >
          Kategori
        </Button>
      </div>
    </nav>
  );
}

/** Düzenleme genel kaydı okur (listedeki fiyat şubeye göre çözülmüş olabilir). */
function EditService({
  row,
  branchPrice,
  ...props
}: {
  row: Service;
  categories: readonly ServiceCategory[];
  staff: readonly StaffSummary[];
  depositAvailable: boolean;
  branchPrice: BranchPriceContext | undefined;
  onClose: () => void;
}) {
  const api = useApi();
  const query = useSuspenseQuery(serviceQuery(api, row.id));
  const service = query.data;
  const branchText = branchPrice?.current != null ? moneyInputValue(branchPrice.current) : '';
  // Sürüm değişince (Yenile) pencere güncel kayıtla yeniden kurulur (kabul kriteri 15).
  return (
    <ServiceModal
      key={service.version}
      {...props}
      onReload={() => void query.refetch()}
      service={service}
      defaults={serviceToForm(service, branchText)}
      {...(branchPrice ? { branchPrice } : {})}
    />
  );
}

function TemplatesModalLoader({
  industryLabel,
  industryCode,
  onClose,
}: {
  industryLabel: string;
  industryCode: Parameters<typeof serviceTemplatesQuery>[1];
  onClose: () => void;
}) {
  const api = useApi();
  const templates = useSuspenseQuery(serviceTemplatesQuery(api, industryCode)).data.items;
  return <TemplatesModal industryLabel={industryLabel} templates={templates} onClose={onClose} />;
}
