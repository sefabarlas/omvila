import { formatDuration } from '@omvila/ui-web/format';
import { Avatar, Badge, DataTable, EmptyState, IconButton, Switch } from '@omvila/ui-web/panel';
import { MoreHorizontal } from 'lucide-react';
import type { Service, StaffSummary } from './api';
import { depositLabel, formatServicePrice } from './labels';
import styles from './Services.module.css';

export interface ServiceTableProps {
  services: readonly Service[];
  staff: readonly StaffSummary[];
  depositAvailable: boolean;
  canEdit: boolean;
  onEdit: (service: Service) => void;
  onToggleOnline: (service: Service, online: boolean) => void;
}

const maxAvatars = 3;

/** Hizmet tablosu: ad ve açıklama, süre, fiyat (sağda), personel, online, kapora, düzenle. */
export function ServiceTable({
  services,
  staff,
  depositAvailable,
  canEdit,
  onEdit,
  onToggleOnline,
}: ServiceTableProps) {
  const names = new Map(staff.map((s) => [s.id, s.displayName ?? s.fullName]));
  return (
    <DataTable<Service>
      caption="Hizmetler"
      rows={services}
      rowKey={(s) => s.id}
      empty={
        <EmptyState
          headingLevel={2}
          title="Bu kategoride hizmet yok"
          description="Hizmet eklerken bu kategoriyi seçin."
        />
      }
      columns={[
        {
          key: 'name',
          label: 'Hizmet',
          render: (s) => (
            <button
              type="button"
              className={styles.nameButton}
              disabled={!canEdit}
              onClick={() => {
                onEdit(s);
              }}
            >
              <span className={styles.serviceName}>{s.name}</span>
              {s.description ? <span className={styles.serviceDetail}>{s.description}</span> : null}
            </button>
          ),
        },
        { key: 'duration', label: 'Süre', render: (s) => formatDuration(s.durationMinutes) },
        {
          key: 'price',
          label: 'Fiyat',
          numeric: true,
          render: (s) => <span className={styles.price}>{formatServicePrice(s)}</span>,
        },
        {
          key: 'staff',
          label: 'Personel',
          render: (s) => {
            const people = s.staffIds.flatMap((id) => {
              const name = names.get(id);
              return name ? [name] : [];
            });
            if (people.length === 0) {
              return <span className={styles.muted}>Atanmadı</span>;
            }

            return (
              <span className={styles.avatars} aria-label={people.join(', ')}>
                {people.slice(0, maxAvatars).map((name) => (
                  <Avatar key={name} name={name} size="sm" />
                ))}
                {people.length > maxAvatars ? (
                  <span className={styles.more}>+{people.length - maxAvatars}</span>
                ) : null}
              </span>
            );
          },
        },
        {
          key: 'online',
          label: 'Online',
          render: (s) => (
            <Switch
              label={`${s.name} online randevuya açık`}
              hideLabel
              checked={s.isOnlineBookable}
              disabled={!canEdit}
              onChange={(event) => {
                onToggleOnline(s, event.target.checked);
              }}
            />
          ),
        },
        {
          key: 'deposit',
          label: 'Kapora',
          render: (s) => {
            const label = depositLabel(s.deposit, depositAvailable);
            return s.deposit.mode === 'Always' && depositAvailable ? (
              <Badge tone="uyari">{label}</Badge>
            ) : (
              <span className={styles.muted}>{label}</span>
            );
          },
        },
        {
          key: 'actions',
          label: 'İşlemler',
          align: 'right',
          render: (s) =>
            canEdit ? (
              <IconButton
                label={`${s.name} düzenle`}
                icon={<MoreHorizontal size={18} />}
                onClick={() => {
                  onEdit(s);
                }}
              />
            ) : null,
        },
      ]}
    />
  );
}
