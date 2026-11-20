import { Button, Checkbox, IconButton, TextField } from '@omvila/ui-web/panel';
import { Plus, X } from 'lucide-react';
import { useState } from 'react';
import type { ServiceStage } from './api';
import styles from './Services.module.css';

/** Sözleşme: en fazla 6 aşama, her biri 5–600 dk. */
const maxStages = 6;

/**
 * Aşamalar (salon-paneli/HizmetEkle): "Uygulama · 45 dk", "Bekleme · 40 dk" (personel başka müşteriye müsait),
 * "Yıkama ve fön · 35 dk". Bekleme aşaması taralı/çerçeveli gösterilir.
 */
export function StagesEditor({
  stages,
  onChange,
}: {
  stages: readonly ServiceStage[];
  onChange: (stages: ServiceStage[]) => void;
}) {
  const [adding, setAdding] = useState(false);
  const [name, setName] = useState('');
  const [duration, setDuration] = useState('');
  const [waiting, setWaiting] = useState(false);
  const [error, setError] = useState<string>();

  const add = () => {
    const minutes = Number(duration);
    if (name.trim() === '') {
      setError('Aşama adını girin.');
      return;
    }

    if (!Number.isInteger(minutes) || minutes < 5 || minutes > 600) {
      setError('Süre 5 ile 600 dakika arasında olmalı.');
      return;
    }

    onChange([...stages, { name: name.trim(), durationMinutes: minutes, staffBusy: !waiting }]);
    setName('');
    setDuration('');
    setWaiting(false);
    setError(undefined);
    setAdding(false);
  };

  return (
    <>
      <ul className={styles.stages} aria-label="Aşamalar">
        {stages.map((stage, index) => (
          <li
            key={`${stage.name}-${String(index)}`}
            className={
              stage.staffBusy ? styles.stage : `${styles.stage ?? ''} ${styles.waiting ?? ''}`
            }
          >
            {stage.name} · {stage.durationMinutes} dk
            {stage.staffBusy ? null : <span className={styles.muted}> (bekleme)</span>}
            <IconButton
              size="sm"
              label={`${stage.name} aşamasını kaldır`}
              icon={<X size={14} />}
              onClick={() => {
                onChange(stages.filter((_, n) => n !== index));
              }}
            />
          </li>
        ))}
        {stages.length < maxStages && !adding ? (
          <li>
            <Button
              variant="ghost"
              size="sm"
              icon={<Plus size={16} aria-hidden="true" />}
              onClick={() => {
                setAdding(true);
              }}
            >
              Aşama
            </Button>
          </li>
        ) : null}
      </ul>
      {adding ? (
        <div className={styles.stageForm}>
          <TextField
            label="Aşama adı"
            placeholder="Bekleme"
            maxLength={60}
            value={name}
            error={error}
            onChange={(event) => {
              setName(event.target.value);
            }}
          />
          <TextField
            label="Aşama süresi"
            inputMode="numeric"
            suffix="dk"
            value={duration}
            onChange={(event) => {
              setDuration(event.target.value);
            }}
          />
          <Checkbox
            label="Bekleme (personel müsait)"
            checked={waiting}
            onChange={(event) => {
              setWaiting(event.target.checked);
            }}
          />
          <Button size="sm" onClick={add}>
            Aşamayı ekle
          </Button>
        </div>
      ) : null}
    </>
  );
}
