import { ApiProblem } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { formatDuration, moneyInputValue, parseMoneyInput } from '@omvila/ui-web/format';
import { Banner, Button, Checkbox, Modal, TextField, useToast } from '@omvila/ui-web/panel';
import { useState } from 'react';
import type { SyntheticEvent } from 'react';
import type { Service } from '@/features/services/api';
import { useSetStaffServices } from './api';
import type { StaffDetail, StaffServiceAssignment } from './api';
import styles from './Staff.module.css';

interface Row {
  checked: boolean;
  duration: string;
  surcharge: string;
}

/**
 * Verdiği hizmetler (kabul kriteri 11): her hizmet için seçim, bu kişiye özel süre ("Keratin 75 dk") ve ek ücret
 * ("Balyaj +300 ₺"). Boş alan hizmetin süresi ve ek ücretsiz demektir. Açıkken bağlanır.
 */
export function StaffServicesModal({
  staff,
  services,
  onClose,
}: {
  staff: StaffDetail;
  services: readonly Service[];
  onClose: () => void;
}) {
  const toast = useToast();
  const save = useSetStaffServices();
  const [rows, setRows] = useState<Record<string, Row>>(() =>
    Object.fromEntries(
      services.map((service) => {
        const assigned = staff.services.find((a) => a.serviceId === service.id);
        return [
          service.id,
          {
            checked: assigned !== undefined,
            duration:
              assigned?.durationOverrideMinutes != null
                ? String(assigned.durationOverrideMinutes)
                : '',
            surcharge:
              assigned?.surcharge && assigned.surcharge.amountMinor > 0
                ? moneyInputValue(assigned.surcharge.amountMinor)
                : '',
          },
        ];
      }),
    ),
  );
  const [error, setError] = useState<string>();
  const name = staff.displayName ?? staff.fullName;
  const set = (id: string, patch: Partial<Row>) => {
    setRows({
      ...rows,
      [id]: { ...(rows[id] ?? { checked: false, duration: '', surcharge: '' }), ...patch },
    });
    setError(undefined);
  };

  const onSubmit = async (event: SyntheticEvent) => {
    event.preventDefault();
    const items: StaffServiceAssignment[] = [];
    for (const service of services) {
      const row = rows[service.id];
      if (!row?.checked) {
        continue;
      }

      const duration = row.duration.trim();
      const minutes = Number(duration);
      if (duration !== '' && (!Number.isInteger(minutes) || minutes < 5)) {
        setError(`${service.name}: süre en az 5 dakika olmalı.`);
        return;
      }

      const surcharge = row.surcharge.trim() === '' ? 0 : parseMoneyInput(row.surcharge);
      if (surcharge === null) {
        setError(`${service.name}: ek ücreti kontrol edin (300 gibi).`);
        return;
      }

      items.push({
        serviceId: service.id,
        durationOverrideMinutes: duration === '' ? null : minutes,
        ...(surcharge > 0
          ? { surcharge: { amountMinor: surcharge, currency: 'TRY' as const } }
          : {}),
      });
    }

    try {
      await save.mutateAsync({ staffId: staff.id, version: staff.version, items });
      toast.show({ tone: 'basari', title: 'Hizmetler kaydedildi' });
      onClose();
    } catch (problem) {
      toast.show({
        tone: 'hata',
        title: errorMessage(problem instanceof ApiProblem ? problem.code : undefined),
      });
    }
  };

  return (
    <Modal
      open
      size="lg"
      onClose={onClose}
      title="Verdiği hizmetler"
      description={`${name} · süreyi veya ek ücreti bu kişiye özel değiştirebilirsiniz.`}
      footer={
        <>
          <Button onClick={onClose}>Vazgeç</Button>
          <Button
            variant="primary"
            type="submit"
            form="personel-hizmetleri"
            loading={save.isPending}
            loadingLabel="Kaydediliyor…"
          >
            Hizmetleri kaydet
          </Button>
        </>
      }
    >
      <form id="personel-hizmetleri" noValidate onSubmit={(e) => void onSubmit(e)}>
        {error ? <Banner tone="hata">{error}</Banner> : null}
        {services.length === 0 ? (
          <p className={styles.note}>Önce Hizmetler sayfasından hizmet ekleyin.</p>
        ) : (
          <>
            <div className={styles.serviceRow} aria-hidden="true">
              <span className={styles.columnHead}>Hizmet</span>
              <span className={styles.columnHead}>Özel süre</span>
              <span className={styles.columnHead}>Ek ücret</span>
            </div>
            {services.map((service) => {
              const row = rows[service.id];
              return (
                <div key={service.id} className={styles.serviceRow}>
                  <Checkbox
                    label={service.name}
                    checked={row?.checked ?? false}
                    onChange={(event) => {
                      set(service.id, { checked: event.target.checked });
                    }}
                  />
                  <TextField
                    label={`${service.name} özel süre`}
                    hideLabel
                    inputMode="numeric"
                    suffix="dk"
                    placeholder={String(service.durationMinutes)}
                    hint={formatDuration(service.durationMinutes)}
                    disabled={!row?.checked}
                    value={row?.duration ?? ''}
                    onChange={(event) => {
                      set(service.id, { duration: event.target.value });
                    }}
                  />
                  <TextField
                    label={`${service.name} ek ücret`}
                    hideLabel
                    inputMode="decimal"
                    suffix="₺"
                    disabled={!row?.checked}
                    value={row?.surcharge ?? ''}
                    onChange={(event) => {
                      set(service.id, { surcharge: event.target.value });
                    }}
                  />
                </div>
              );
            })}
          </>
        )}
      </form>
    </Modal>
  );
}
