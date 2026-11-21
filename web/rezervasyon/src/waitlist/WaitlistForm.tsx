import { BookingButton, SwitchCard } from '@omvila/ui-web/booking';
import { formatDate } from '@omvila/ui-web/format';
import { Check } from 'lucide-react';
import { useId, useState } from 'react';
import type { TimeWindow } from './waitlist-api';
import { timeWindowLabels, timeWindows } from './windows';
import styles from './waitlist.module.css';

export interface WaitlistChoice {
  timeWindows: TimeWindow[];
  acceptOtherStaff: boolean;
}

interface WaitlistFormProps {
  /** Dolu gün (şubenin takvim günü). */
  date: string;
  /** Tek personel seçildiyse adı ("Başka personel de olur" anahtarı); "Fark etmez"de null, anahtar gösterilmez. */
  staffName: string | null;
  onJoin: (choice: WaitlistChoice) => void;
}

/**
 * DoluGun.png "Bekleme listesine katılın": saat dilimi çipleri (çoklu; hiçbiri seçilmezse her saat uyar) ve
 * "Başka personel de olur". Telefon bir sonraki adımda doğrulanır (mevcut bilgiler + kod adımı).
 */
export function WaitlistForm({ date, staffName, onJoin }: WaitlistFormProps) {
  const [selected, setSelected] = useState<TimeWindow[]>([]);
  const [otherStaff, setOtherStaff] = useState(false);
  const titleId = useId();
  const toggle = (period: TimeWindow) => {
    setSelected((current) =>
      current.includes(period)
        ? current.filter((item) => item !== period)
        : timeWindows.filter((item) => item === period || current.includes(item)),
    );
  };

  return (
    <section className={styles.form} aria-labelledby={titleId}>
      <h3 id={titleId} className={styles.title}>
        Bekleme listesine katılın
      </h3>
      <p className={styles.text}>
        {formatDate(date, 'weekday')} için yer açılırsa size SMS göndeririz. Bağlantıdaki süre
        içinde onaylarsanız randevu sizin olur.
      </p>
      <div className={styles.group}>
        <p className={styles.label} id={`${titleId}-saat`}>
          Hangi saatler size uyar?
        </p>
        <div className={styles.chips} role="group" aria-labelledby={`${titleId}-saat`}>
          {timeWindows.map((period) => {
            const on = selected.includes(period);
            return (
              <button
                key={period}
                type="button"
                className={[styles.chip, on && styles.chipSelected].filter(Boolean).join(' ')}
                aria-pressed={on}
                onClick={() => {
                  toggle(period);
                }}
              >
                {on ? <Check size={16} strokeWidth={2.25} aria-hidden="true" /> : null}
                {timeWindowLabels[period]}
              </button>
            );
          })}
        </div>
      </div>
      {staffName ? (
        <SwitchCard
          label="Başka personel de olur"
          description={`${staffName} dışında yer açılırsa da haber ver`}
          checked={otherStaff}
          onCheckedChange={setOtherStaff}
        />
      ) : null}
      <BookingButton
        block
        onClick={() => {
          onJoin({ timeWindows: selected, acceptOtherStaff: staffName ? otherStaff : true });
        }}
      >
        Bekleme listesine katıl
      </BookingButton>
      <p className={styles.hint}>Telefon numaranızı bir sonraki adımda doğrulayacağız.</p>
    </section>
  );
}
