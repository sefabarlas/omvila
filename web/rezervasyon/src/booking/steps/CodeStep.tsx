import { BookingButton } from '@omvila/ui-web/booking';
import { ShieldAlert, ShieldCheck } from 'lucide-react';
import { useEffect, useRef, useState } from 'react';
import { StateBanner } from '@/components/StateBanner';
import { requestOtp, verifyOtp } from '../booking-api';
import type { BookingToken, OtpChallenge } from '../booking-api';
import { CodeInput } from '../fields/CodeInput';
import { messageFor, metaNumber, problemCode, waitLabel } from '../problem';
import styles from './steps.module.css';

const codeLength = 6;

interface CodeStepProps {
  branchId: string;
  phone: string;
  challenge: OtpChallenge;
  onVerified: (token: BookingToken) => void;
  onNewChallenge: (challenge: OtpChallenge) => void;
  onChangeNumber: () => void;
}

/** Geri sayım (saniye); değer değişince yeniden başlar. */
function useCountdown(seconds: number, restartKey: string) {
  const [state, setState] = useState({ key: restartKey, left: seconds });
  const left = state.key === restartKey ? state.left : seconds;
  useEffect(() => {
    if (left <= 0) return;
    const timer = setTimeout(() => {
      setState({ key: restartKey, left: left - 1 });
    }, 1000);
    return () => {
      clearTimeout(timer);
    };
  }, [left, restartKey]);
  return left;
}

const clock = (seconds: number) =>
  `${String(Math.floor(seconds / 60))}:${String(seconds % 60).padStart(2, '0')}`;

/**
 * SMS kodu (Kod.png). Altı hane tamamlanınca kendiliğinden doğrulanır. Hatalar: kod hatalı (kalan deneme),
 * süresi doldu (yeni kod), deneme sınırı (Durumlar.png deseni, bekleme süresiyle).
 */
export function CodeStep({
  branchId,
  phone,
  challenge,
  onVerified,
  onNewChallenge,
  onChangeNumber,
}: CodeStepProps) {
  const [code, setCode] = useState('');
  const [failure, setFailure] = useState<unknown>(null);
  const [busy, setBusy] = useState(false);
  const [expired, setExpired] = useState(false);
  const submitted = useRef('');
  const left = useCountdown(challenge.resendAfterSeconds, challenge.otpId);

  const verify = async (value: string) => {
    if (value.length !== codeLength || busy) return;
    submitted.current = value;
    setBusy(true);
    setFailure(null);
    try {
      onVerified(await verifyOtp(challenge.otpId, value));
    } catch (error) {
      setFailure(error);
      setCode('');
      if (problemCode(error) === 'otp_expired') setExpired(true);
    } finally {
      setBusy(false);
    }
  };

  const resend = async () => {
    setBusy(true);
    setFailure(null);
    try {
      const result = await requestOtp(branchId, phone);
      if (result.status === 'AlreadyVerified' && result.booking) {
        onVerified(result.booking);
      } else if (result.challenge) {
        setExpired(false);
        setCode('');
        onNewChallenge(result.challenge);
      }
    } catch (error) {
      setFailure(error);
    } finally {
      setBusy(false);
    }
  };

  const failureCode = problemCode(failure);
  const locked = failureCode === 'otp_attempts_exceeded' || failureCode === 'rate_limited';
  const attemptsLeft = metaNumber(failure, 'attemptsLeft');
  const retry = metaNumber(failure, 'retryAfterSeconds');
  const message =
    failureCode === 'otp_invalid' && attemptsLeft !== undefined
      ? `Kod hatalı. ${String(attemptsLeft)} deneme hakkınız kaldı.`
      : messageFor(failure);

  return (
    <div className={styles.stack}>
      <h2 className={styles.title}>Telefonunuza gelen kodu girin</h2>
      <p className={styles.text}>
        {challenge.phoneMasked} numarasına {codeLength} haneli bir kod gönderdik.{' '}
        <button
          type="button"
          className={[styles.linkButton, styles.inlineLink].join(' ')}
          onClick={onChangeNumber}
        >
          Numarayı değiştir
        </button>
      </p>
      {locked ? (
        <StateBanner
          tone="hata"
          icon={<ShieldAlert size={20} />}
          title="Çok fazla deneme yaptınız."
          description={`Güvenliğiniz için ${retry ? waitLabel(retry) : 'biraz'} sonra yeni kod isteyebilirsiniz.`}
        />
      ) : null}
      <CodeInput
        label="Doğrulama kodu"
        value={code}
        invalid={Boolean(failure) && !locked}
        disabled={locked || expired}
        describedBy={failure && !locked ? 'kod-hata' : undefined}
        onChange={(value) => {
          setCode(value);
          if (value.length === codeLength && value !== submitted.current) void verify(value);
        }}
      />
      {failure && !locked ? (
        <p id="kod-hata" className={styles.note} role="alert">
          {message}
        </p>
      ) : null}
      {left > 0 && !expired ? (
        <p className={styles.text} aria-live="off">
          Kod gelmedi mi? {clock(left)} sonra yeniden gönderebilirsiniz.
        </p>
      ) : (
        <div>
          <BookingButton
            variant="ghost"
            size="sm"
            disabled={busy || (locked && !expired)}
            onClick={() => {
              void resend();
            }}
          >
            Kodu yeniden gönder
          </BookingButton>
        </div>
      )}
      <BookingButton
        block
        disabled={code.length !== codeLength || locked || expired}
        loading={busy}
        loadingLabel="Doğrulanıyor…"
        onClick={() => {
          submitted.current = '';
          void verify(code);
        }}
      >
        Doğrula
      </BookingButton>
      <div className={styles.infoBox}>
        <ShieldCheck size={20} aria-hidden="true" />
        <p>
          Kod tamamlanınca kendiliğinden doğrulanır. Numaranız yalnızca randevu bildirimleri için
          kullanılır.
        </p>
      </div>
    </div>
  );
}
