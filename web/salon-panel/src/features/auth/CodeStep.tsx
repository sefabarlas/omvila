import { Banner, Button, Checkbox, CodeInput } from '@omvila/ui-web/panel';
import { ChevronLeft } from 'lucide-react';
import { useState } from 'react';
import type { SyntheticEvent } from 'react';
import type { OtpChallenge } from './api';
import styles from './auth.module.css';
import { describeProblem, formatCountdown } from './problem';
import { useCountdown } from './use-countdown';

interface CodeStepProps {
  challenge: OtpChallenge;
  /** Giriş "Giriş yap", kayıt "İşletmeyi oluştur". */
  submitLabel: string;
  /** "Bu bilgisayarda 30 gün hatırla" (yalnız giriş). */
  showRemember: boolean;
  eyebrow?: string;
  onBack: () => void;
  onVerify: (code: string, remember: boolean) => Promise<void>;
  onResend: () => Promise<OtpChallenge>;
}

/** salon-paneli/GirisKod: altı haneli kod; hatalı kodda kalan deneme, sınırda bekleme, yeniden gönderme sayacı. */
export function CodeStep({
  challenge,
  submitLabel,
  showRemember,
  eyebrow,
  onBack,
  onVerify,
  onResend,
}: CodeStepProps) {
  const [code, setCode] = useState('');
  const [remember, setRemember] = useState(true);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState<string>();
  const [phoneMasked, setPhoneMasked] = useState(challenge.phoneMasked);
  const [resendIn, setResendIn] = useCountdown(challenge.resendAfterSeconds);
  const [lockedFor, setLockedFor] = useCountdown(0);

  const submit = async (event: SyntheticEvent<HTMLFormElement>) => {
    event.preventDefault();
    if (code.length !== 6) {
      setError('Kodun 6 hanesini girin.');
      return;
    }

    setBusy(true);
    setError(undefined);
    try {
      await onVerify(code, remember);
    } catch (problem) {
      const described = describeProblem(problem);
      if (described.retryAfterSeconds !== undefined) {
        setLockedFor(described.retryAfterSeconds);
      } else {
        setError(described.message);
      }

      setCode('');
    } finally {
      setBusy(false);
    }
  };

  const resend = async () => {
    setError(undefined);
    try {
      const next = await onResend();
      setPhoneMasked(next.phoneMasked);
      setResendIn(next.resendAfterSeconds);
    } catch (problem) {
      const described = describeProblem(problem);
      if (described.retryAfterSeconds !== undefined) {
        setResendIn(described.retryAfterSeconds);
      } else {
        setError(described.message);
      }
    }
  };

  return (
    <form className={styles.fields} onSubmit={(event) => void submit(event)} noValidate>
      <Button
        variant="ghost"
        size="sm"
        className={styles.back}
        icon={<ChevronLeft size={16} />}
        onClick={onBack}
      >
        Geri
      </Button>
      <div className={styles.header}>
        {eyebrow ? <p className={styles.eyebrow}>{eyebrow}</p> : null}
        <h1 className={styles.title}>Kodu girin</h1>
        <p className={styles.description}>{phoneMasked} numarasına 6 haneli kod gönderdik.</p>
      </div>
      {lockedFor > 0 ? (
        <Banner tone="hata" title="Çok fazla deneme yaptınız.">
          Güvenliğiniz için biraz bekleyin. Yeniden deneme: {formatCountdown(lockedFor)}
        </Banner>
      ) : null}
      <CodeInput
        label="Doğrulama kodu"
        value={code}
        onChange={setCode}
        error={error}
        disabled={busy || lockedFor > 0}
        focusOnMount
      />
      {showRemember ? (
        <Checkbox
          label="Bu bilgisayarda 30 gün hatırla"
          checked={remember}
          onChange={(event) => {
            setRemember(event.target.checked);
          }}
        />
      ) : null}
      <Button
        type="submit"
        variant="primary"
        size="lg"
        className={styles.submit}
        loading={busy}
        loadingLabel="Doğrulanıyor…"
        disabled={lockedFor > 0}
      >
        {submitLabel}
      </Button>
      <p className={styles.footer} aria-live="polite">
        {resendIn > 0 ? (
          `Kod gelmedi mi? ${formatCountdown(resendIn)} sonra tekrar gönderebilirsiniz.`
        ) : (
          <>
            Kod gelmedi mi?{' '}
            <button type="button" className={styles.textButton} onClick={() => void resend()}>
              Kodu tekrar gönder
            </button>
          </>
        )}
      </p>
    </form>
  );
}
