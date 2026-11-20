import { formatDuration, formatMoney } from '@omvila/ui-web/format';
import { Avatar, Button, DataTable, EmptyState, useToast } from '@omvila/ui-web/panel';
import { branchToday } from '@omvila/ui-web/time';
import { useSuspenseQueries, useSuspenseQuery } from '@tanstack/react-query';
import { Calendar, Plus, UserPlus } from 'lucide-react';
import { Suspense, useState } from 'react';
import { useApi } from '@/app/api-context';
import { useSelectedBranch } from '@/app/branch';
import { useClock } from '@/app/clock';
import { defaultTimeZone } from '@/app/config';
import { useSession } from '@/app/session';
import { servicesQuery, staffQuery } from '@/features/services/api';
import type { Service } from '@/features/services/api';
import { onboardingQuery } from '@/features/overview/api';
import { Page } from '@/shared/Page';
import { AddStaffDrawer } from './AddStaffDrawer';
import { staffDetailQuery, useConfirmSolo } from './api';
import type { StaffDetail, StaffSummary } from './api';
import { roleLabel, workingSummary } from './labels';
import styles from './Staff.module.css';
import { StaffCard } from './StaffCard';
import { StaffHoursCard } from './StaffHoursCard';
import { StaffServicesModal } from './StaffServicesModal';
import { TimeOffModal } from './TimeOffModal';

type Dialog = 'none' | 'add' | 'timeOff' | 'services';

/**
 * Personel (salon-paneli/Personel, PersonelBos): kartlar, seçili kişinin saatleri, verdiği hizmetler ve prim;
 * personel ekleme ve izin. Yalnız sahip varken ekip davet ekranı.
 */
export function StaffPage() {
  const api = useApi();
  const clock = useClock();
  const { session } = useSession();
  const { branchId: selectedBranch } = useSelectedBranch();
  const branchId = selectedBranch ?? session.branches[0]?.id ?? '';
  const branchName = session.branches.find((b) => b.id === branchId)?.name ?? '';
  const multiBranch = session.branches.length > 1;
  const [staffResult, servicesResult] = useSuspenseQueries({
    queries: [staffQuery(api), servicesQuery(api, branchId)],
  });
  const people = staffResult.data.items
    .filter((s) => s.isActive && (s.branchIds.length === 0 || s.branchIds.includes(branchId)))
    .sort((a, b) =>
      a.role === 'Owner' ? -1 : b.role === 'Owner' ? 1 : a.fullName.localeCompare(b.fullName, 'tr'),
    );
  const services = servicesResult.data.items.filter((s) => s.isActive);
  const [selectedId, setSelectedId] = useState<string | undefined>(people[0]?.id);
  const selected = people.find((p) => p.id === selectedId) ?? people[0];
  const [dialog, setDialog] = useState<Dialog>('none');
  const close = () => {
    setDialog('none');
  };
  const today = branchToday(clock, defaultTimeZone);
  const onlyOwner = people.length <= 1;

  return (
    <Page
      title="Personel"
      context={`${String(people.length)} kişi · ${branchName}`}
      action={
        <>
          <Button
            icon={<Calendar size={16} aria-hidden="true" />}
            disabled={people.length === 0}
            onClick={() => {
              setDialog('timeOff');
            }}
          >
            İzin ve tatil
          </Button>
          <Button
            variant="primary"
            icon={<Plus size={16} aria-hidden="true" />}
            onClick={() => {
              setDialog('add');
            }}
          >
            Personel ekle
          </Button>
        </>
      }
    >
      {onlyOwner ? (
        <TeamEmpty
          people={people}
          onAdd={() => {
            setDialog('add');
          }}
        />
      ) : (
        <ul className={styles.cards} aria-label="Personel">
          {people.map((person) => (
            <li key={person.id}>
              <StaffCard
                person={person}
                selected={person.id === selected?.id}
                onSelect={() => {
                  setSelectedId(person.id);
                }}
              />
            </li>
          ))}
        </ul>
      )}
      {selected && !onlyOwner ? (
        <Suspense fallback={null}>
          <StaffDetailView
            key={`${selected.id}-${branchId}`}
            staffId={selected.id}
            branchId={branchId}
            branchName={branchName}
            multiBranch={multiBranch}
            services={services}
            onEditServices={() => {
              setDialog('services');
            }}
          />
        </Suspense>
      ) : null}
      {dialog === 'add' ? (
        <AddStaffDrawer
          services={services}
          branchId={branchId}
          usedColors={people.map((p) => p.calendarColor)}
          onClose={close}
          onCreated={(staff) => {
            setSelectedId(staff.id);
          }}
        />
      ) : null}
      {dialog === 'timeOff' && selected ? (
        <TimeOffModal
          staff={people}
          initialStaffId={selected.id}
          timeZone={defaultTimeZone}
          today={today}
          onClose={close}
        />
      ) : null}
      {dialog === 'services' && selected ? (
        <Suspense fallback={null}>
          <ServicesDialog staffId={selected.id} services={services} onClose={close} />
        </Suspense>
      ) : null}
    </Page>
  );
}

/** PersonelBos: sahibin satırı ve ekip daveti; "tek başıma çalışıyorum" kurulum adımını tamamlar. */
function TeamEmpty({ people, onAdd }: { people: readonly StaffSummary[]; onAdd: () => void }) {
  const api = useApi();
  const toast = useToast();
  const { session } = useSession();
  const solo = useConfirmSolo();
  const staffStepDone =
    useSuspenseQuery(onboardingQuery(api)).data.steps.find((s) => s.key === 'Staff')?.done ?? false;
  return (
    <>
      <DataTable<StaffSummary>
        caption="Personel"
        rows={people}
        rowKey={(p) => p.id}
        columns={[
          {
            key: 'person',
            label: 'Kişi',
            render: (p) => (
              <span className={styles.cardHead}>
                <Avatar name={p.fullName} size="sm" />
                <span className={styles.personText}>
                  <span className={styles.lineName}>
                    {p.fullName}
                    {p.id === session.membership.staffId ? (
                      <span className={styles.muted}> (siz)</span>
                    ) : null}
                  </span>
                  <span className={styles.muted}>{roleLabel(p.role)}</span>
                </span>
              </span>
            ),
          },
          {
            key: 'online',
            label: 'Online',
            render: (p) => (p.isBookableOnline ? 'Açık' : 'Yalnızca panelden'),
          },
        ]}
      />
      <section className={styles.emptyCard} aria-label="Ekip">
        <EmptyState
          icon={<UserPlus size={24} />}
          headingLevel={2}
          title="Ekibinizi ekleyin"
          description="Her çalışan kendi telefonuyla giriş yapar, kendi takvimini ve primini görür. Müşteri telefonları personel rolünde maskelenir."
          actions={
            <>
              <Button
                variant="primary"
                icon={<Plus size={16} aria-hidden="true" />}
                onClick={onAdd}
              >
                Personel ekle
              </Button>
              {staffStepDone ? null : (
                <Button
                  loading={solo.isPending}
                  onClick={() => {
                    solo.mutate(undefined, {
                      onSuccess: () => {
                        toast.show({ title: 'Kurulumda ekip adımı tamamlandı' });
                      },
                    });
                  }}
                >
                  Tek başıma çalışıyorum
                </Button>
              )}
            </>
          }
        />
      </section>
    </>
  );
}

function StaffDetailView({
  staffId,
  branchId,
  branchName,
  multiBranch,
  services,
  onEditServices,
}: {
  staffId: string;
  branchId: string;
  branchName: string;
  multiBranch: boolean;
  services: readonly Service[];
  onEditServices: () => void;
}) {
  const api = useApi();
  const staff = useSuspenseQuery(staffDetailQuery(api, staffId)).data;
  return (
    <div className={styles.detail}>
      <StaffHoursCard
        staff={staff}
        branchId={branchId}
        branchName={branchName}
        multiBranch={multiBranch}
      />
      <div className={styles.side}>
        <StaffServicesPanel staff={staff} services={services} onEdit={onEditServices} />
        <section className={styles.panel} aria-label="Prim">
          <h2 className={styles.panelTitle}>Prim</h2>
          <ul className={styles.list}>
            <li className={styles.line}>
              <span>Hizmet primi</span>
              <span />
              <span className={styles.number}>%{staff.serviceCommissionPercent}</span>
            </li>
            <li className={styles.line}>
              <span>Paket satışı primi</span>
              <span />
              <span className={styles.number}>%{staff.saleCommissionPercent}</span>
            </li>
          </ul>
          <p className={styles.note}>
            Prim tamamlanan randevulardan hesaplanır, gelmeyen ve iptal edilenlerden hesaplanmaz.
          </p>
        </section>
        <p className={styles.note}>
          Çalışma: {workingSummary(staff.workingHours.find((w) => w.branchId === branchId)?.hours)}
        </p>
      </div>
    </div>
  );
}

/** "Verdiği hizmetler": hizmet, süre (özel), ek ücret. */
function StaffServicesPanel({
  staff,
  services,
  onEdit,
}: {
  staff: StaffDetail;
  services: readonly Service[];
  onEdit: () => void;
}) {
  const byId = new Map(services.map((s) => [s.id, s]));
  return (
    <section className={styles.panel} aria-label="Verdiği hizmetler">
      <div className={styles.panelHead}>
        <h2 className={styles.panelTitle}>Verdiği hizmetler</h2>
        <Button size="sm" variant="ghost" onClick={onEdit}>
          Düzenle
        </Button>
      </div>
      {staff.services.length === 0 ? (
        <p className={styles.note}>Henüz hizmet atanmadı.</p>
      ) : (
        <ul className={styles.list}>
          {staff.services.flatMap((assignment) => {
            const service = byId.get(assignment.serviceId);
            if (!service) {
              return [];
            }

            const custom = assignment.durationOverrideMinutes;
            return [
              <li key={assignment.serviceId} className={styles.line}>
                <span className={styles.lineName}>{service.name}</span>
                <span className={styles.muted}>
                  {custom != null
                    ? `${formatDuration(custom)} (özel)`
                    : formatDuration(service.durationMinutes)}
                </span>
                <span className={styles.number}>
                  {assignment.surcharge && assignment.surcharge.amountMinor > 0
                    ? `+${formatMoney(assignment.surcharge)}`
                    : '—'}
                </span>
              </li>,
            ];
          })}
        </ul>
      )}
    </section>
  );
}

function ServicesDialog({
  staffId,
  services,
  onClose,
}: {
  staffId: string;
  services: readonly Service[];
  onClose: () => void;
}) {
  const api = useApi();
  const staff = useSuspenseQuery(staffDetailQuery(api, staffId)).data;
  return <StaffServicesModal staff={staff} services={services} onClose={onClose} />;
}
