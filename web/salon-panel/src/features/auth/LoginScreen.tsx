import { zodResolver } from '@hookform/resolvers/zod';
import { parseMobilePhone } from '@omvila/ui-web/format';
import { Button, TextField } from '@omvila/ui-web/panel';
import { Link, useNavigate } from '@tanstack/react-router';
import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { useAuthApi } from './api';
import type { OtpChallenge } from './api';
import styles from './auth.module.css';
import { AuthLayout } from './AuthLayout';
import { CodeStep } from './CodeStep';
import { signupHandoff } from './handoff';
import { describeProblem } from './problem';

const schema = z.object({
  phone: z
    .string()
    .refine((value) => parseMobilePhone(value) !== null, 'Telefon numarası 10 haneli olmalı.'),
});

type LoginForm = z.infer<typeof schema>;

/** salon-paneli/Giris ve GirisKod (kabul kriterleri 2, 3, 4). */
export function LoginScreen({ redirectTo }: { redirectTo: string | undefined }) {
  const authApi = useAuthApi();
  const navigate = useNavigate();
  const [challenge, setChallenge] = useState<OtpChallenge | null>(null);
  const [phone, setPhone] = useState('');
  const form = useForm<LoginForm>({ resolver: zodResolver(schema), defaultValues: { phone: '' } });

  const sendCode = form.handleSubmit(async ({ phone: input }) => {
    try {
      const e164 = parseMobilePhone(input) ?? input;
      setPhone(e164);
      setChallenge(await authApi.requestOtp(e164, 'Login'));
    } catch (problem) {
      form.setError('phone', { message: describeProblem(problem).message });
    }
  });

  const verify = async (code: string, remember: boolean) => {
    if (!challenge) {
      return;
    }

    const result = await authApi.verifyOtp(challenge.otpId, code, remember);
    if (result.result === 'SignupRequired' && result.signupToken) {
      // Numara kayıtlı değil: doğrulanmış telefonla kayıt ekranına (kabul kriteri 3).
      signupHandoff.set({ signupToken: result.signupToken, phoneMasked: challenge.phoneMasked });
      await navigate({ to: '/kayit' });
      return;
    }

    await authApi.signedIn(result.accessToken, safeRedirect(redirectTo));
  };

  return (
    <AuthLayout>
      {challenge ? (
        <CodeStep
          challenge={challenge}
          submitLabel="Giriş yap"
          showRemember
          onBack={() => {
            setChallenge(null);
          }}
          onVerify={verify}
          onResend={() => authApi.requestOtp(phone, 'Login')}
        />
      ) : (
        <form className={styles.fields} onSubmit={(event) => void sendCode(event)} noValidate>
          <div className={styles.header}>
            <h1 className={styles.title}>Panele giriş</h1>
            <p className={styles.description}>İşletmenize kayıtlı telefon numaranızla girin.</p>
          </div>
          <TextField
            label="Cep telefonu"
            type="tel"
            inputMode="tel"
            autoComplete="tel-national"
            placeholder="0532 123 45 67"
            error={form.formState.errors.phone?.message}
            {...form.register('phone')}
          />
          <Button
            type="submit"
            variant="primary"
            size="lg"
            className={styles.submit}
            loading={form.formState.isSubmitting}
            loadingLabel="Gönderiliyor…"
          >
            SMS kodu gönder
          </Button>
          <p className={styles.footer}>
            İşletmeniz OMVILA&apos;da değil mi?{' '}
            <Link to="/kayit" className={styles.link}>
              Hemen başlayın
            </Link>
          </p>
        </form>
      )}
    </AuthLayout>
  );
}

/** Yalnız uygulama içi yol (açık yönlendirme yok). */
function safeRedirect(target: string | undefined): string {
  return target?.startsWith('/') && !target.startsWith('//') ? target : '/';
}
