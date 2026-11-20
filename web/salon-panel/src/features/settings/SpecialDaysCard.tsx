import { formatDate } from '@omvila/ui-web/format';
import { Button, IconButton, useToast } from '@omvila/ui-web/panel';
import { useSuspenseQuery } from '@tanstack/react-query';
import { Plus, Trash2 } from 'lucide-react';
import { useState } from 'react';
import { useApi } from '@/app/api-context';
import { AddSpecialDayModal } from './AddSpecialDayModal';
import { overridesQuery, useDeleteOverrides } from './branch-api';
import { formatIntervals, groupOverrides, overrideKindLabel } from './hours';
import type { OverrideGroup } from './hours';
import { describeSaveError } from './save-error';
import styles from './Settings.module.css';
import { SettingsCard } from './SettingsLayout';

/** "31 Aralık" veya "20–22 Mart" / "30 Mart – 1 Nisan". */
export function formatDateRange(from: string, to: string): string {
  if (from === to) {
    return formatDate(from, 'dayMonth');
  }

  const [fromDay, fromMonth] = formatDate(from, 'dayMonth').split(' ');
  const toText = formatDate(to, 'dayMonth');
  return fromMonth === toText.split(' ')[1]
    ? `${fromDay ?? ''}–${toText}`
    : `${formatDate(from, 'dayMonth')} – ${toText}`;
}

function describe(group: OverrideGroup): string {
  const { override } = group;
  const hours = override.isClosed ? '' : formatIntervals(override.intervals ?? []);
  return [override.note, hours].filter(Boolean).join(' ');
}

export interface SpecialDaysCardProps {
  branchId: string;
  branchName: string;
  /** Şubenin bugünü (yyyy-aa-gg); geçmiş özel günler listelenmez. */
  today: string;
}

/** "Özel günler ve tatiller": yaklaşan kapanışlar ve özel saatler; tek gün veya çok günlü (kabul kriteri 6). */
export function SpecialDaysCard({ branchId, branchName, today }: SpecialDaysCardProps) {
  const api = useApi();
  const toast = useToast();
  const items = useSuspenseQuery(overridesQuery(api, branchId, today)).data.items;
  const remove = useDeleteOverrides(branchId);
  const [adding, setAdding] = useState(false);
  const groups = groupOverrides(items);

  const onRemove = async (group: OverrideGroup) => {
    try {
      await remove.mutateAsync(group.ids);
      toast.show({ title: 'Özel gün kaldırıldı' });
    } catch (error) {
      toast.show({ tone: 'hata', title: describeSaveError(error).message });
    }
  };

  return (
    <SettingsCard
      title="Özel günler ve tatiller"
      description="Bayram, tatil veya kısa gün için tarih ve saat girin; o gün haftalık saatlerin yerine geçer."
    >
      {groups.length > 0 ? (
        <ul className={styles.rows}>
          {groups.map((group) => {
            const date = formatDateRange(group.fromDate, group.toDate);
            return (
              <li key={group.ids.join()} className={styles.specialRow}>
                <span className={styles.specialDate}>{date}</span>
                <span>{describe(group) || overrideKindLabel(group.override)}</span>
                <span className={styles.specialKind}>
                  {overrideKindLabel(group.override)} ·{' '}
                  {group.dayCount === 1 ? 'Tek gün' : `${String(group.dayCount)} gün`}
                </span>
                <IconButton
                  label={`${date} özel gününü kaldır`}
                  icon={<Trash2 size={16} />}
                  disabled={remove.isPending}
                  onClick={() => void onRemove(group)}
                />
              </li>
            );
          })}
        </ul>
      ) : (
        <p className={styles.cardText}>Yaklaşan özel gün yok. Haftalık saatler geçerli.</p>
      )}
      <div className={styles.cardFoot}>
        <Button
          size="sm"
          icon={<Plus size={16} aria-hidden="true" />}
          onClick={() => {
            setAdding(true);
          }}
        >
          Özel gün ekle
        </Button>
      </div>
      {adding ? (
        <AddSpecialDayModal
          branchId={branchId}
          branchName={branchName}
          today={today}
          onClose={() => {
            setAdding(false);
          }}
        />
      ) : null}
    </SettingsCard>
  );
}
