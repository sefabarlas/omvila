import { Button, Select, useToast } from '@omvila/ui-web/panel';
import { branchToday } from '@omvila/ui-web/time';
import { useSuspenseQueries } from '@tanstack/react-query';
import { Plus } from 'lucide-react';
import { useState } from 'react';
import type { SyntheticEvent } from 'react';
import { useApi } from '@/app/api-context';
import { useSelectedBranch } from '@/app/branch';
import { useClock } from '@/app/clock';
import { useSession } from '@/app/session';
import { Segmented } from '@/shared/Segmented';
import { AddBranchModal } from './AddBranchModal';
import { brandSettingsQuery } from './api';
import { branchQuery, useSaveBranchSettings, workingHoursQuery } from './branch-api';
import type { Branch, SlotInterval } from './branch-api';
import { ConflictBanner } from '@/shared/ConflictBanner';
import { dayError, toDrafts, toWeeklyHours } from './hours';
import type { DayDraft } from './hours';
import { describeSaveError } from './save-error';
import styles from './Settings.module.css';
import { SettingRow, SettingRows, SettingsCard, SettingsLayout } from './SettingsLayout';
import { SpecialDaysCard } from './SpecialDaysCard';
import { WeeklyHoursCard } from './WeeklyHoursCard';

const formId = 'sube-saatleri';
const slotOptions: readonly SlotInterval[] = [5, 10, 15];
const transferMinutes = [0, 15, 30, 45, 60, 90, 120];

/** "İstanbul (UTC+3)": şehir adı saat diliminden, fark o anki (yaz saati dahil) Intl değerinden. */
export function timeZoneLabel(timeZone: string, now: Date): string {
  const names: Record<string, string> = { 'Europe/Istanbul': 'İstanbul' };
  const city = names[timeZone] ?? (timeZone.split('/').at(-1) ?? timeZone).replaceAll('_', ' ');
  const offset =
    new Intl.DateTimeFormat('en-US', { timeZone, timeZoneName: 'shortOffset' })
      .formatToParts(now)
      .find((part) => part.type === 'timeZoneName')
      ?.value.replace('GMT', 'UTC') ?? '';
  return offset === '' || offset === 'UTC' ? `${city} (UTC)` : `${city} (${offset})`;
}

/**
 * Ayarlar → Şubeler ve saatler (salon-paneli/AyarlarSube): kenar menüde seçili şubenin haftalık saatleri, özel
 * günleri ve takvim düzeni. Şube ekleme üst barda (SubeEkle).
 */
export function BranchSettingsPage() {
  const { session } = useSession();
  const { branchId: selected } = useSelectedBranch();
  const branchId = selected ?? session.branches[0]?.id ?? '';
  // Şube değişince form o şubenin verisiyle baştan kurulur.
  return <BranchSettings key={branchId} branchId={branchId} />;
}

function BranchSettings({ branchId }: { branchId: string }) {
  const api = useApi();
  const clock = useClock();
  const toast = useToast();
  const { session } = useSession();
  const [branchResult, hoursResult, settingsResult] = useSuspenseQueries({
    queries: [
      branchQuery(api, branchId),
      workingHoursQuery(api, branchId),
      brandSettingsQuery(api),
    ],
  });
  const branch: Branch = branchResult.data;
  const settings = settingsResult.data;
  const save = useSaveBranchSettings();
  const [days, setDays] = useState<DayDraft[]>(() => toDrafts(hoursResult.data));
  const [slot, setSlot] = useState<SlotInterval | null>(branch.slotIntervalMinutes ?? null);
  const [transfer, setTransfer] = useState(settings.defaultBranchTransferMinutes);
  const [showErrors, setShowErrors] = useState(false);
  const [conflict, setConflict] = useState(false);
  const [addingBranch, setAddingBranch] = useState(false);
  const today = branchToday(clock, branch.timeZone);
  const multiBranch = session.branches.length > 1;

  const onSubmit = async (event: SyntheticEvent) => {
    event.preventDefault();
    setShowErrors(true);
    if (days.some((day) => dayError(day) !== undefined)) {
      return;
    }

    try {
      await save.mutateAsync({
        branchId,
        hours: { version: hoursResult.data.version ?? '', body: toWeeklyHours(days) },
        ...(slot !== (branch.slotIntervalMinutes ?? null)
          ? { slot: { version: branch.version, value: slot } }
          : {}),
        ...(transfer !== settings.defaultBranchTransferMinutes
          ? { transfer: { settings, value: transfer } }
          : {}),
      });
      setConflict(false);
      setShowErrors(false);
      toast.show({ tone: 'basari', title: 'Değişiklikler kaydedildi' });
    } catch (error) {
      const view = describeSaveError(error);
      setConflict(view.conflict);
      if (!view.conflict) {
        toast.show({ tone: 'hata', title: view.message });
      }
    }
  };

  const reload = async () => {
    const [hours, fresh, freshSettings] = await Promise.all([
      hoursResult.refetch(),
      branchResult.refetch(),
      settingsResult.refetch(),
    ]);
    if (hours.data) {
      setDays(toDrafts(hours.data));
    }

    setSlot(fresh.data?.slotIntervalMinutes ?? null);
    if (freshSettings.data) {
      setTransfer(freshSettings.data.defaultBranchTransferMinutes);
    }

    setConflict(false);
  };

  return (
    <SettingsLayout
      context={`${branch.name} · ${branch.addressLine}`}
      formId={formId}
      saving={save.isPending}
      extraAction={
        <Button
          icon={<Plus size={16} aria-hidden="true" />}
          onClick={() => {
            setAddingBranch(true);
          }}
        >
          Şube ekle
        </Button>
      }
    >
      {conflict ? <ConflictBanner onReload={() => void reload()} /> : null}
      <form id={formId} className={styles.sections} noValidate onSubmit={(e) => void onSubmit(e)}>
        <WeeklyHoursCard
          branchName={branch.name}
          days={days}
          onChange={setDays}
          showErrors={showErrors}
        />
      </form>
      <SpecialDaysCard branchId={branchId} branchName={branch.name} today={today} />
      <SettingsCard title="Takvim düzeni">
        <SettingRows>
          <SettingRow
            id="aralik"
            narrow
            title="Randevu saat aralığı"
            description={`Müşteriye gösterilen saatlerin adımı. Seçilmezse marka ayarı (${String(settings.slotIntervalMinutes)} dk) kullanılır.`}
          >
            <Segmented<SlotInterval | 'marka'>
              legend="Randevu saat aralığı"
              name="slot"
              form={formId}
              describedBy="aralik-aciklama"
              options={[
                { value: 'marka', label: 'Marka ayarı' },
                ...slotOptions.map((v) => ({ value: v, label: `${String(v)} dk` })),
              ]}
              value={slot ?? 'marka'}
              onChange={(v) => {
                setSlot(v === 'marka' ? null : v);
              }}
            />
          </SettingRow>
          {multiBranch ? (
            <SettingRow
              id="gecis"
              title="Şubeler arası geçiş süresi"
              description="İki şubede çalışan personelin randevuları arasına eklenir. Bütün şubeler için ortak."
            >
              <Select
                label="Şubeler arası geçiş süresi"
                hideLabel
                form={formId}
                aria-describedby="gecis-aciklama"
                value={String(transfer)}
                options={[...new Set([...transferMinutes, transfer])]
                  .sort((a, b) => a - b)
                  .map((n) => ({
                    value: String(n),
                    label: n === 0 ? 'Yok' : `${String(n)} dakika`,
                  }))}
                onChange={(event) => {
                  setTransfer(Number(event.target.value));
                }}
              />
            </SettingRow>
          ) : null}
          <SettingRow
            id="saat-dilimi"
            narrow
            title="Saat dilimi"
            description="Tüm saatler bu dilimde gösterilir."
          >
            <span className={styles.staticValue}>
              {timeZoneLabel(branch.timeZone, clock.now())}
            </span>
          </SettingRow>
        </SettingRows>
      </SettingsCard>
      {addingBranch ? (
        <AddBranchModal
          onClose={() => {
            setAddingBranch(false);
          }}
        />
      ) : null}
    </SettingsLayout>
  );
}
