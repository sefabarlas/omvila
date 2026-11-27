import { ApiProblem } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { Avatar, Badge, Button, DataTable, EmptyState, Skeleton } from '@omvila/ui-web/panel';
import type { BadgeTone } from '@omvila/ui-web/panel';
import { useQuery, useSuspenseQueries } from '@tanstack/react-query';
import { Check, Users } from 'lucide-react';
import { Suspense, useState } from 'react';
import type { ReactNode } from 'react';
import { useApi } from '@/app/api-context';
import { useSelectedBranch } from '@/app/branch';
import { useSession } from '@/app/session';
import { servicesQuery, staffQuery } from '@/features/services/api';
import { AddStaffDrawer } from '@/features/staff/AddStaffDrawer';
import { EditUserModal } from './EditUserModal';
import kvkk from './Kvkk.module.css';
import {
  membershipRoleLabels,
  roleGroupLabels,
  roleMatrixQuery,
  userStatusLabels,
  usersQuery,
} from './roles-api';
import type { ManageUser, RoleAccessLevel, RoleMatrixRow } from './roles-api';
import roles from './Roles.module.css';
import styles from './Settings.module.css';
import { SettingsLayout } from './SettingsLayout';

const statusTone: Record<ManageUser['status'], BadgeTone> = {
  Active: 'basari',
  Invited: 'notr',
  Disabled: 'notr',
};

function loadError(error: unknown, title: string, retry: () => void) {
  return (
    <EmptyState
      headingLevel={3}
      title={title}
      description={errorMessage(error instanceof ApiProblem ? error.code : undefined)}
      actions={<Button onClick={retry}>Tekrar dene</Button>}
    />
  );
}

/** Matris hücresi: tam ve şube düzeyi onay işareti, "Kendi", yoksa "—". */
function AccessCell({ level }: { level: RoleAccessLevel }) {
  switch (level) {
    case 'All':
    case 'Branch':
      return (
        <span className={[roles.level, roles.allowed].join(' ')}>
          <Check size={16} aria-label={level === 'All' ? 'Tam' : 'Kendi şubesinde'} />
        </span>
      );
    case 'Own':
      return <span className={roles.level}>Kendi</span>;
    default:
      return (
        <span className={roles.level} aria-label="Yok">
          —
        </span>
      );
  }
}

/**
 * Ayarlar → Roller ve yetkiler (salon-paneli/AyarlarRoller; K6). Kullanıcılar tablosu ve salt okunur rol matrisi;
 * "Düzenle" rolü ve şubeleri değiştirir. "Kullanıcı ekle" ayrı uç değil, personel davet akışıdır (manageCreateStaff).
 * Markaya özel rol yok; matris düzenlenmez. settings.roles (yalnız işletme sahibi).
 */
export function RolesPage() {
  const api = useApi();
  const { session } = useSession();
  const users = useQuery(usersQuery(api));
  const matrix = useQuery(roleMatrixQuery(api));
  const [editing, setEditing] = useState<ManageUser | null>(null);
  const [adding, setAdding] = useState(false);
  const branchNames = new Map(session.branches.map((b) => [b.id, b.name]));
  const branchText = (user: ManageUser) =>
    user.role === 'Owner' || user.branchIds.length === 0
      ? 'Tümü'
      : user.branchIds.map((id) => branchNames.get(id) ?? '—').join(', ');

  return (
    <SettingsLayout context="Kim neyi görebilir">
      <section className={kvkk.tableCard} aria-labelledby="kullanicilar">
        <div className={kvkk.tableHead}>
          <h2 id="kullanicilar" className={styles.cardTitle}>
            Kullanıcılar
          </h2>
          <Button
            size="sm"
            variant="ghost"
            className={roles.addUser}
            onClick={() => {
              setAdding(true);
            }}
          >
            Kullanıcı ekle
          </Button>
        </div>
        {users.isPending ? (
          <Skeleton variant="block" height="200px" />
        ) : users.isError ? (
          loadError(users.error, 'Kullanıcılar yüklenemedi', () => void users.refetch())
        ) : (
          <DataTable<ManageUser>
            caption="Kullanıcılar"
            rows={users.data.items}
            rowKey={(user) => user.userId}
            empty={
              <EmptyState
                icon={<Users size={24} />}
                headingLevel={3}
                title="Kullanıcı yok"
                description="Personel eklediğinizde telefonuyla giriş yapar ve burada rolüyle görünür."
              />
            }
            columns={[
              {
                key: 'name',
                label: 'Ad',
                render: (user) => (
                  <span className={roles.person}>
                    <Avatar name={user.fullName} size="sm" />
                    <span>
                      {user.fullName}
                      {user.isCurrentUser ? <span className={roles.you}> (siz)</span> : null}
                    </span>
                  </span>
                ),
              },
              { key: 'role', label: 'Rol', render: (user) => membershipRoleLabels[user.role] },
              { key: 'branch', label: 'Şube', render: branchText },
              {
                key: 'status',
                label: 'Durum',
                render: (user) => (
                  <Badge tone={statusTone[user.status]}>{userStatusLabels[user.status]}</Badge>
                ),
              },
              {
                key: 'actions',
                label: 'Eylem',
                align: 'right',
                render: (user) => (
                  <Button
                    size="sm"
                    variant="ghost"
                    aria-label={`${user.fullName}: düzenle`}
                    onClick={() => {
                      setEditing(user);
                    }}
                  >
                    Düzenle
                  </Button>
                ),
              },
            ]}
          />
        )}
      </section>
      <section className={kvkk.tableCard} aria-labelledby="rol-izinleri">
        <div className={kvkk.tableHead}>
          <h2 id="rol-izinleri" className={styles.cardTitle}>
            Rol izinleri
          </h2>
          <span className={styles.cardText}>İşletme sahibi her şeyi görür</span>
        </div>
        {matrix.isPending ? (
          <Skeleton variant="block" height="320px" />
        ) : matrix.isError ? (
          loadError(matrix.error, 'Rol izinleri yüklenemedi', () => void matrix.refetch())
        ) : (
          <>
            <DataTable<RoleMatrixRow>
              caption="Rol izinleri"
              rows={matrix.data.rows}
              rowKey={(row) => row.group}
              columns={[
                {
                  key: 'group',
                  label: 'İzin',
                  render: (row) => <strong>{roleGroupLabels[row.group]}</strong>,
                },
                {
                  key: 'manager',
                  label: membershipRoleLabels.BranchManager,
                  align: 'center',
                  render: (row) => <AccessCell level={row.branchManager} />,
                },
                {
                  key: 'staff',
                  label: membershipRoleLabels.Staff,
                  align: 'center',
                  render: (row) => <AccessCell level={row.staff} />,
                },
              ]}
            />
            <p className={roles.footnote}>
              “Kendi”: yalnızca kendi randevuları ve kendi primi. Şube yöneticisi yalnızca kendi
              şubesinde işlem yapar.
            </p>
          </>
        )}
      </section>
      {editing ? (
        <EditUserModal
          user={editing}
          onClose={() => {
            setEditing(null);
          }}
        />
      ) : null}
      {adding ? (
        <Suspense fallback={null}>
          <InviteStaff
            onClose={() => {
              setAdding(false);
            }}
          />
        </Suspense>
      ) : null}
    </SettingsLayout>
  );
}

/** "Kullanıcı ekle": personel davet akışı (PersonelEkle); eklenen kişi kullanıcılar tablosuna düşer. */
function InviteStaff({ onClose }: { onClose: () => void }): ReactNode {
  const api = useApi();
  const { session } = useSession();
  const { branchId: selected } = useSelectedBranch();
  const branchId = selected ?? session.branches[0]?.id ?? '';
  const [staff, services] = useSuspenseQueries({
    queries: [staffQuery(api), servicesQuery(api, branchId)],
  });
  return (
    <AddStaffDrawer
      services={services.data.items.filter((s) => s.isActive)}
      branchId={branchId}
      usedColors={staff.data.items.map((p) => p.calendarColor)}
      onClose={onClose}
      onCreated={() => undefined}
    />
  );
}
