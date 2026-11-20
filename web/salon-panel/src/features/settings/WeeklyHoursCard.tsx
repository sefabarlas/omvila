import { Button, IconButton, Switch, TextField } from '@omvila/ui-web/panel';
import { Plus, X } from 'lucide-react';
import { dayError, dayNames, maxIntervalsPerDay, suggestedIntervals } from './hours';
import type { DayDraft, IntervalDraft } from './hours';
import styles from './Settings.module.css';
import { SettingsCard } from './SettingsLayout';

export interface WeeklyHoursCardProps {
  branchName: string;
  days: readonly DayDraft[];
  onChange: (days: DayDraft[]) => void;
  /** Kaydet denendi mi: hatalar ancak o zaman gösterilir. */
  showErrors: boolean;
}

/** "Kadıköy · çalışma saatleri": her gün açık/kapalı ve en fazla dört aralık (mola iki aralık arasıdır). */
export function WeeklyHoursCard({ branchName, days, onChange, showErrors }: WeeklyHoursCardProps) {
  const update = (dayOfWeek: number, change: (day: DayDraft) => DayDraft) => {
    onChange(days.map((day) => (day.dayOfWeek === dayOfWeek ? change(day) : day)));
  };

  const setInterval = (day: DayDraft, index: number, patch: Partial<IntervalDraft>) => {
    update(day.dayOfWeek, (d) => ({
      ...d,
      intervals: d.intervals.map((i, n) => (n === index ? { ...i, ...patch } : i)),
    }));
  };

  return (
    <SettingsCard
      title={`${branchName} · çalışma saatleri`}
      description="Gün içinde birden fazla aralık tanımlayabilirsiniz (ör. 10:00–13:00 ve 14:00–20:00)."
    >
      <ul className={styles.rows}>
        {days.map((day) => {
          const name = dayNames[day.dayOfWeek - 1] ?? '';
          const error = showErrors ? dayError(day) : undefined;
          const errorId = `gun-${String(day.dayOfWeek)}-hata`;
          return (
            <li key={day.dayOfWeek} className={styles.dayRow}>
              <span className={styles.dayName}>{name}</span>
              <Switch
                label={`${name} açık`}
                hideLabel
                checked={day.open}
                onChange={(event) => {
                  const open = event.target.checked;
                  update(day.dayOfWeek, (d) => ({
                    ...d,
                    open,
                    intervals:
                      open && d.intervals.length === 0
                        ? suggestedIntervals(days, d.dayOfWeek)
                        : d.intervals,
                  }));
                }}
              />
              {day.open ? (
                <div className={styles.dayIntervals}>
                  {day.intervals.map((interval, index) => {
                    const label = `${name} ${String(index + 1)}. aralık`;
                    return (
                      <div key={index} className={styles.interval}>
                        <TextField
                          className={styles.timeField}
                          type="time"
                          label={`${label} başlangıç`}
                          hideLabel
                          value={interval.start}
                          aria-invalid={error ? true : undefined}
                          aria-describedby={error ? errorId : undefined}
                          onChange={(event) => {
                            setInterval(day, index, { start: event.target.value });
                          }}
                        />
                        <span aria-hidden="true">–</span>
                        <TextField
                          className={styles.timeField}
                          type="time"
                          label={`${label} bitiş`}
                          hideLabel
                          value={interval.end}
                          aria-invalid={error ? true : undefined}
                          aria-describedby={error ? errorId : undefined}
                          onChange={(event) => {
                            setInterval(day, index, { end: event.target.value });
                          }}
                        />
                        {day.intervals.length > 1 ? (
                          <IconButton
                            label={`${label} kaldır`}
                            icon={<X size={16} />}
                            onClick={() => {
                              update(day.dayOfWeek, (d) => ({
                                ...d,
                                intervals: d.intervals.filter((_, n) => n !== index),
                              }));
                            }}
                          />
                        ) : null}
                      </div>
                    );
                  })}
                  {day.intervals.length < maxIntervalsPerDay ? (
                    <Button
                      variant="ghost"
                      size="sm"
                      icon={<Plus size={16} aria-hidden="true" />}
                      aria-label={`Aralık ekle: ${name}`}
                      onClick={() => {
                        update(day.dayOfWeek, (d) => ({
                          ...d,
                          intervals: [...d.intervals, { start: '', end: '' }],
                        }));
                      }}
                    >
                      Aralık ekle
                    </Button>
                  ) : null}
                  {error ? (
                    <p id={errorId} className={styles.dayError}>
                      {error}
                    </p>
                  ) : null}
                </div>
              ) : (
                <span className={styles.dayClosed}>Kapalı</span>
              )}
            </li>
          );
        })}
      </ul>
    </SettingsCard>
  );
}
