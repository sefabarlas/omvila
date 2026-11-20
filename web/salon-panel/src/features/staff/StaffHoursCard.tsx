import { ApiProblem } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { Button, useToast } from '@omvila/ui-web/panel';
import { useState } from 'react';
import { WeeklyHoursCard } from '@/features/settings/WeeklyHoursCard';
import { dayError, toDrafts, toWeeklyHours } from '@/features/settings/hours';
import type { DayDraft } from '@/features/settings/hours';
import { useSetStaffHours } from './api';
import type { StaffDetail } from './api';

/**
 * Personelin seçili şubedeki saatleri. Başka şubedeki saatleriyle örtüşemez (dilim-01; sunucu
 * overlaps_other_branch döner). Şube veya personel değişince bileşen yeniden kurulur (key).
 */
export function StaffHoursCard({
  staff,
  branchId,
  branchName,
  multiBranch,
}: {
  staff: StaffDetail;
  branchId: string;
  branchName: string;
  multiBranch: boolean;
}) {
  const toast = useToast();
  const save = useSetStaffHours();
  const current = staff.workingHours.find((w) => w.branchId === branchId)?.hours;
  const [days, setDays] = useState<DayDraft[]>(() => toDrafts(current ?? { days: [] }));
  const [showErrors, setShowErrors] = useState(false);
  const [serverError, setServerError] = useState<string>();
  const name = staff.displayName ?? staff.fullName;

  const onSave = async () => {
    setShowErrors(true);
    setServerError(undefined);
    if (days.some((day) => dayError(day) !== undefined)) {
      return;
    }

    try {
      await save.mutateAsync({
        staffId: staff.id,
        branchId,
        version: current?.version ?? staff.version,
        body: toWeeklyHours(days),
      });
      setShowErrors(false);
      toast.show({ tone: 'basari', title: 'Saatler kaydedildi' });
    } catch (problem) {
      const overlaps =
        problem instanceof ApiProblem &&
        Object.values(problem.errors).some((codes) => codes.includes('overlaps_other_branch'));
      if (overlaps) {
        setServerError('Bu saatler kişinin başka şubedeki saatleriyle çakışıyor.');
        return;
      }

      toast.show({
        tone: 'hata',
        title: errorMessage(problem instanceof ApiProblem ? problem.code : undefined),
      });
    }
  };

  return (
    <WeeklyHoursCard
      branchName={multiBranch ? `${name} · ${branchName}` : name}
      days={days}
      onChange={setDays}
      showErrors={showErrors}
      footer={
        <>
          <Button
            size="sm"
            loading={save.isPending}
            loadingLabel="Kaydediliyor…"
            onClick={() => void onSave()}
          >
            Saatleri kaydet
          </Button>
          {serverError ? <span role="alert">{serverError}</span> : null}
        </>
      }
    />
  );
}
