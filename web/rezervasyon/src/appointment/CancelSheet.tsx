import { BookingButton } from '@omvila/ui-web/booking';
import { CalendarClock, CalendarX2, Check, ChevronRight } from 'lucide-react';
import { useEffect, useId, useRef, useState } from 'react';
import { messageFor, problemCode } from '@/booking/problem';
import fieldStyles from '@/booking/fields/fields.module.css';
import type { Outcome } from './AppointmentManager';
import { cancelAppointment } from './appointment-api';
import type { CancelReason, PublicAppointment } from './appointment-api';
import { whenLabel } from './details';
import styles from './appointment.module.css';

/** Müşterinin seçebileceği nedenler (CustomerCancelRequest.reason; salon nedenleri burada yok). */
const reasons: { value: CancelReason; label: string }[] = [
  { value: 'PlanChanged', label: 'Planım değişti' },
  { value: 'Sick', label: 'Hastayım' },
  { value: 'WantsDifferentTime', label: 'Başka saat istiyorum' },
  { value: 'Other', label: 'Diğer' },
];

/** Sözleşme: not en çok 300 karakter. */
const noteMaxLength = 300;

/** Sunucunun ele aldığı hatalar: sayfa kapanır, randevu yeniden okunur. */
const terminal = new Set([
  'cutoff_passed',
  'concurrency_conflict',
  'invalid_state_transition',
  'not_found',
]);

interface CancelSheetProps {
  code: string;
  appointment: PublicAppointment;
  etag: string;
  onClose: () => void;
  /** "İptal yerine saati değiştirebilirsiniz". */
  onReschedule: () => void;
  outcome: Outcome;
}

/**
 * İptal (Iptal.png): alt sayfa, neden zorunlu, "Diğer"de not zorunlu (T-52 (5)). Kapora uyarısı Dilim 4'te.
 * Sağlık bilgisi istenmez (ürün kuralı 4): "Hastayım" yalnız neden kodudur, açıklama alanı açılmaz.
 */
export function CancelSheet({
  code,
  appointment,
  etag,
  onClose,
  onReschedule,
  outcome,
}: CancelSheetProps) {
  const [reason, setReason] = useState<CancelReason | null>(null);
  const [note, setNote] = useState('');
  const [errors, setErrors] = useState<{ reason?: string; note?: string }>({});
  const [failure, setFailure] = useState<unknown>(null);
  const [busy, setBusy] = useState(false);
  const titleRef = useRef<HTMLHeadingElement>(null);
  const id = useId();

  useEffect(() => {
    titleRef.current?.focus();
  }, []);
  useEffect(() => {
    const onKey = (event: KeyboardEvent) => {
      if (event.key === 'Escape') onClose();
    };
    document.addEventListener('keydown', onKey);
    return () => {
      document.removeEventListener('keydown', onKey);
    };
  }, [onClose]);

  const submit = async () => {
    const nextErrors: { reason?: string; note?: string } = {};
    if (!reason) nextErrors.reason = 'İptal nedenini seçin.';
    if (reason === 'Other' && !note.trim()) nextErrors.note = 'Kısa bir açıklama yazın.';
    setErrors(nextErrors);
    if (!reason || nextErrors.note) return;
    setBusy(true);
    setFailure(null);
    try {
      outcome.done(
        await cancelAppointment(code, etag, {
          reason,
          ...(reason === 'Other' ? { note: note.trim() } : {}),
        }),
      );
    } catch (error) {
      const errorCode = problemCode(error);
      if (errorCode && terminal.has(errorCode)) outcome.failed(error);
      else setFailure(error);
    } finally {
      setBusy(false);
    }
  };

  return (
    <div className={styles.overlay}>
      <section
        className={styles.sheet}
        role="dialog"
        aria-modal="true"
        aria-labelledby={`${id}-baslik`}
      >
        <span className={styles.handle} aria-hidden="true" />
        <span className={styles.sheetIcon} aria-hidden="true">
          <CalendarX2 size={24} />
        </span>
        <div className={styles.titleGroup}>
          <h2 id={`${id}-baslik`} ref={titleRef} className={styles.title} tabIndex={-1}>
            Randevu iptal edilsin mi?
          </h2>
          <p className={styles.muted}>
            {appointment.brand.name} · {whenLabel(appointment)}.
          </p>
        </div>
        <fieldset
          className={styles.reasons}
          aria-describedby={errors.reason ? `${id}-neden-hata` : undefined}
        >
          <legend className={fieldStyles.label}>
            İptal nedeni{' '}
            <span className={styles.required} aria-hidden="true">
              *
            </span>
          </legend>
          <div className={styles.chips}>
            {reasons.map((option) => (
              <label key={option.value} className={styles.chip}>
                <input
                  type="radio"
                  name={`${id}-neden`}
                  value={option.value}
                  checked={reason === option.value}
                  required
                  onChange={() => {
                    setReason(option.value);
                    setErrors({});
                    setFailure(null);
                  }}
                />
                {reason === option.value ? (
                  <Check size={16} strokeWidth={2.25} aria-hidden="true" />
                ) : null}
                {option.label}
              </label>
            ))}
          </div>
          {errors.reason ? (
            <p id={`${id}-neden-hata`} className={fieldStyles.error}>
              {errors.reason}
            </p>
          ) : null}
        </fieldset>
        {reason === 'Other' ? (
          <div className={fieldStyles.field}>
            <label className={fieldStyles.label} htmlFor={`${id}-not`}>
              Açıklama
            </label>
            <div
              className={[fieldStyles.control, styles.textareaControl].join(' ')}
              data-invalid={errors.note ? 'true' : undefined}
            >
              <textarea
                id={`${id}-not`}
                className={[fieldStyles.input, styles.textarea].join(' ')}
                value={note}
                maxLength={noteMaxLength}
                rows={3}
                required
                aria-invalid={errors.note ? true : undefined}
                aria-describedby={errors.note ? `${id}-not-hata` : undefined}
                onChange={(event) => {
                  setNote(event.target.value);
                  setErrors({});
                }}
              />
            </div>
            {errors.note ? (
              <p id={`${id}-not-hata`} className={fieldStyles.error}>
                {errors.note}
              </p>
            ) : null}
          </div>
        ) : null}
        {reason === 'WantsDifferentTime' && appointment.canReschedule ? (
          <button type="button" className={styles.suggestion} onClick={onReschedule}>
            <CalendarClock size={18} aria-hidden="true" />
            <span>İptal yerine saati değiştirebilirsiniz</span>
            <ChevronRight size={18} aria-hidden="true" />
          </button>
        ) : null}
        {failure ? (
          <p className={styles.error} role="alert">
            {messageFor(failure)}
          </p>
        ) : null}
        <div className={styles.sheetActions}>
          <BookingButton
            variant="danger"
            block
            className={styles.dangerSolid ?? ''}
            loading={busy}
            loadingLabel="İptal ediliyor…"
            onClick={() => {
              void submit();
            }}
          >
            İptal et
          </BookingButton>
          <BookingButton variant="ghost" block onClick={onClose}>
            Vazgeç
          </BookingButton>
        </div>
      </section>
    </div>
  );
}
