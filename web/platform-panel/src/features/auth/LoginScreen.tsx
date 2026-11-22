import { zodResolver } from '@hookform/resolvers/zod';
import { unwrap } from '@omvila/api-client-ts';
import { parseMobilePhone } from '@omvila/ui-web/format';
import {
  AuthLayout,
  authStyles as styles,
  Button,
  CodeStep,
  describeAuthProblem,
  TextField,
} from '@omvila/ui-web/panel';
import type { OtpChallenge } from '@omvila/ui-web/panel';
import { useQueryClient } from '@tanstack/react-query';
import { useNavigate } from '@tanstack/react-router';
import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { useApi } from '@/app/api-context';
import { auth } from '@/app/auth';
import { queryKeys } from '@/app/query-keys';

const schema = z.object({
  phone: z
    .string()
    .refine((value) => parseMobilePhone(value) !== null, 'Telefon numarası 10 haneli olmalı.'),
});

type LoginForm = z.infer<typeof schema>;

/**
 * Platform paneli girişi (K2): telefon + SMS kodu, salon paneliyle aynı bileşenler. Numaranın platform kullanıcısı olup
 * olmadığı açık edilmez (sunucu her durumda 202 döner; doğrulama otp_invalid). Kayıt yok; "hatırla" yok (sekme ömrü).
 */
export function LoginScreen({ redirectTo }: { redirectTo: string | undefined }) {
  const api = useApi();
  const queryClient = useQueryClient();
  const navigate = useNavigate();
  const [challenge, setChallenge] = useState<OtpChallenge | null>(null);
  const [phone, setPhone] = useState('');
  const form = useForm<LoginForm>({ resolver: zodResolver(schema), defaultValues: { phone: '' } });

  const requestOtp = async (e164: string): Promise<OtpChallenge> => {
    const data = await unwrap(
      api.POST('/api/v1/admin/auth/otp/request', { body: { phone: e164 } }),
    );
    return {
      otpId: data.otpId,
      phoneMasked: data.phoneMasked,
      resendAfterSeconds: data.resendAfterSeconds,
    };
  };

  const sendCode = form.handleSubmit(async ({ phone: input }) => {
    try {
      const e164 = parseMobilePhone(input) ?? input;
      setPhone(e164);
      setChallenge(await requestOtp(e164));
    } catch (problem) {
      form.setError('phone', { message: describeAuthProblem(problem).message });
    }
  });

  const verify = async (code: string) => {
    if (!challenge) {
      return;
    }

    const result = await unwrap(
      api.POST('/api/v1/admin/auth/otp/verify', { body: { otpId: challenge.otpId, code } }),
    );
    auth.setTokens(result.accessToken, result.refreshToken);
    await queryClient.invalidateQueries({ queryKey: queryKeys.session() });
    await navigate({ to: safeRedirect(redirectTo) });
  };

  return (
    <AuthLayout
      sloganTitle={
        <>
          Platform
          <br />
          paneli.
        </>
      }
      sloganText="Markalar, onaylar ve abonelikler. OMVILA ekibinin çalışma alanı."
    >
      {challenge ? (
        <CodeStep
          challenge={challenge}
          submitLabel="Giriş yap"
          showRemember={false}
          onBack={() => {
            setChallenge(null);
          }}
          onVerify={verify}
          onResend={() => requestOtp(phone)}
        />
      ) : (
        <form className={styles.fields} onSubmit={(event) => void sendCode(event)} noValidate>
          <div className={styles.header}>
            <p className={styles.eyebrow}>OMVILA ekibi</p>
            <h1 className={styles.title}>Platform paneline giriş</h1>
            <p className={styles.description}>
              Platform kullanıcısı olarak kayıtlı numaranızla girin.
            </p>
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
        </form>
      )}
    </AuthLayout>
  );
}

/** Yalnız uygulama içi yol (açık yönlendirme yok). */
function safeRedirect(target: string | undefined): string {
  return target?.startsWith('/') && !target.startsWith('//') ? target : '/';
}
