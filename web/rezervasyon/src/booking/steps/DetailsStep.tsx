import { BookingButton } from '@omvila/ui-web/booking';
import { parseMobilePhone } from '@omvila/ui-web/format';
import { ShieldAlert } from 'lucide-react';
import { useState } from 'react';
import type { ReactNode, SyntheticEvent } from 'react';
import { StateBanner } from '@/components/StateBanner';
import { privacyNoticeUrl } from '@/lib/legal';
import { requestOtp } from '../booking-api';
import type { BookingToken, OtpChallenge } from '../booking-api';
import { CheckboxField } from '../fields/CheckboxField';
import { TextField } from '../fields/TextField';
import { fieldErrors, messageFor, metaNumber, problemCode, waitLabel } from '../problem';
import styles from './steps.module.css';

export interface DetailsForm {
  fullName: string;
  phone: string;
  note: string;
  privacy: boolean;
  marketing: boolean;
}

export const emptyDetails: DetailsForm = {
  fullName: '',
  phone: '',
  note: '',
  privacy: false,
  marketing: false,
};

type Errors = Partial<Record<'fullName' | 'phone' | 'privacy', string>>;

interface DetailsStepProps {
  brandName: string;
  branchId: string;
  form: DetailsForm;
  onFormChange: (form: DetailsForm) => void;
  /** Bu oturumda aynı telefonla alınmış jeton varsa kod yeniden istenmez. */
  existingToken: (BookingToken & { phone: string }) | null;
  onCodeSent: (challenge: OtpChallenge, phone: string) => void;
  onVerified: (token: BookingToken, phone: string) => void;
  /** Özet kartı (Telefon.png üstü). */
  summary: ReactNode;
}

function validate(form: DetailsForm): Errors {
  const errors: Errors = {};
  if (form.fullName.trim().length < 2) errors.fullName = 'Adınızı ve soyadınızı yazın.';
  if (!parseMobilePhone(form.phone)) {
    errors.phone = 'Cep telefonu numaranızı 0532 123 45 67 biçiminde yazın.';
  }
  if (!form.privacy) errors.privacy = 'Devam etmek için aydınlatma metnini onaylayın.';
  return errors;
}

/**
 * Bilgileriniz (Telefon.png): ad, telefon, not, aydınlatma metni (zorunlu) ve kampanya izni (isteğe bağlı,
 * varsayılan kapalı). Kod isteğinde güvenilir cihaz çerezi gider; aynı telefonsa kod adımı atlanır (kriter 9).
 */
export function DetailsStep({
  brandName,
  branchId,
  form,
  onFormChange,
  existingToken,
  onCodeSent,
  onVerified,
  summary,
}: DetailsStepProps) {
  const [errors, setErrors] = useState<Errors>({});
  const [failure, setFailure] = useState<unknown>(null);
  const [sending, setSending] = useState(false);

  const set = <K extends keyof DetailsForm>(key: K, value: DetailsForm[K]) => {
    onFormChange({ ...form, [key]: value });
    if (key in errors) setErrors({ ...errors, [key]: undefined });
  };

  const submit = async (event: SyntheticEvent) => {
    event.preventDefault();
    const found = validate(form);
    setErrors(found);
    setFailure(null);
    const phone = parseMobilePhone(form.phone);
    if (Object.values(found).some(Boolean) || !phone) return;
    if (existingToken?.phone === phone) {
      onVerified(existingToken, phone);
      return;
    }
    setSending(true);
    try {
      const result = await requestOtp(branchId, phone);
      if (result.status === 'AlreadyVerified' && result.booking) {
        onVerified(result.booking, phone);
      } else if (result.challenge) {
        onCodeSent(result.challenge, phone);
      } else {
        setFailure(new Error('otp'));
      }
    } catch (error) {
      if (problemCode(error) === 'validation_failed' && fieldErrors(error).phone) {
        setErrors({
          phone: 'Bu numaraya kod gönderilemiyor. Cep telefonu numaranızı kontrol edin.',
        });
      } else {
        setFailure(error);
      }
    } finally {
      setSending(false);
    }
  };

  const limited = ['otp_attempts_exceeded', 'otp_resend_too_soon', 'rate_limited'].includes(
    problemCode(failure) ?? '',
  );
  const retry = metaNumber(failure, 'retryAfterSeconds');

  return (
    <form
      className={styles.stack}
      noValidate
      onSubmit={(event) => {
        void submit(event);
      }}
    >
      {summary}
      <TextField
        label="Ad soyad"
        name="name"
        autoComplete="name"
        placeholder="Ayşe Yılmaz"
        maxLength={120}
        value={form.fullName}
        error={errors.fullName}
        onChange={(event) => {
          set('fullName', event.target.value);
        }}
      />
      <TextField
        label="Telefon numaranız"
        name="tel"
        type="tel"
        inputMode="tel"
        autoComplete="tel-national"
        placeholder="532 123 45 67"
        prefix="+90"
        maxLength={20}
        value={form.phone}
        error={errors.phone}
        hint="Doğrulama kodu bu numaraya SMS ile gelir. Şifre veya üyelik gerekmez."
        onChange={(event) => {
          set('phone', event.target.value);
        }}
      />
      <TextField
        label="Salona notunuz (isteğe bağlı)"
        name="note"
        placeholder="Saçım omuz hizasında"
        maxLength={500}
        value={form.note}
        onChange={(event) => {
          set('note', event.target.value);
        }}
      />
      <CheckboxField
        checked={form.privacy}
        required
        error={errors.privacy}
        onCheckedChange={(checked) => {
          set('privacy', checked);
        }}
      >
        {privacyNoticeUrl ? (
          <a href={privacyNoticeUrl} target="_blank" rel="noreferrer">
            Aydınlatma metnini
          </a>
        ) : (
          'Aydınlatma metnini'
        )}{' '}
        okudum; randevu için bilgilerimin {brandName} ile paylaşılmasını onaylıyorum.
      </CheckboxField>
      <CheckboxField
        checked={form.marketing}
        onCheckedChange={(checked) => {
          set('marketing', checked);
        }}
      >
        {brandName} kampanyaları için SMS ve e-posta almak istiyorum. (İsteğe bağlı)
      </CheckboxField>
      {failure ? (
        limited ? (
          <StateBanner
            tone="hata"
            icon={<ShieldAlert size={20} />}
            title="Çok fazla deneme yaptınız."
            description={`Güvenliğiniz için ${retry ? waitLabel(retry) : 'biraz'} sonra yeni kod isteyebilirsiniz.`}
          />
        ) : (
          <p className={styles.note} role="alert">
            {messageFor(failure)}
          </p>
        )
      ) : null}
      <BookingButton type="submit" block loading={sending} loadingLabel="Gönderiliyor…">
        Doğrulama kodu gönder
      </BookingButton>
    </form>
  );
}
