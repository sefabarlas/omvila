import { zodResolver } from '@hookform/resolvers/zod';
import { parseMobilePhone } from '@omvila/ui-web/format';
import { Banner, Button, Checkbox, Select, TextField } from '@omvila/ui-web/panel';
import { Link } from '@tanstack/react-router';
import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { useAuthApi } from './api';
import type { OtpChallenge } from './api';
import { industries } from '@/shared/industries';
import type { IndustryCode } from '@/shared/industries';
import styles from './auth.module.css';
import { AuthLayout } from './AuthLayout';
import { CodeStep } from './CodeStep';
import { signupHandoff } from './handoff';
import { describeProblem } from './problem';

const industryCodes = industries.map((i) => i.value) as [IndustryCode, ...IndustryCode[]];

function schema(phoneVerified: boolean) {
  return z.object({
    businessName: z
      .string()
      .trim()
      .min(2, 'İşletme adı en az 2 harf olmalı.')
      .max(160, 'İşletme adı en fazla 160 karakter olabilir.'),
    industryCode: z.enum(industryCodes),
    phone: phoneVerified
      ? z.string()
      : z
          .string()
          .refine(
            (value) => parseMobilePhone(value) !== null,
            'Telefon numarası 10 haneli olmalı.',
          ),
    acceptedTerms: z.boolean().refine((value) => value, 'Devam etmek için koşulları onaylayın.'),
  });
}

type SignupForm = z.infer<ReturnType<typeof schema>>;

/** salon-paneli/Kayit (kabul kriteri 1): 1/2 işletme bilgileri, 2/2 kod; girişten gelen doğrulanmış telefonla tek adım. */
export function SignupScreen() {
  const authApi = useAuthApi();
  const [verified] = useState(() => signupHandoff.peek());
  const [challenge, setChallenge] = useState<OtpChallenge | null>(null);
  const [submitError, setSubmitError] = useState<string>();
  const form = useForm<SignupForm>({
    resolver: zodResolver(schema(verified !== null)),
    defaultValues: {
      businessName: '',
      industryCode: 'hair_salon',
      phone: '',
      acceptedTerms: false,
    },
  });

  const createBusiness = async (signupToken: string) => {
    const { businessName, industryCode } = form.getValues();
    const result = await authApi.signup(signupToken, businessName.trim(), industryCode);
    signupHandoff.clear();
    await authApi.signedIn(result.accessToken, '/');
  };

  const submit = form.handleSubmit(async ({ phone }) => {
    setSubmitError(undefined);
    try {
      if (verified) {
        await createBusiness(verified.signupToken);
        return;
      }

      setChallenge(await authApi.requestOtp(parseMobilePhone(phone) ?? phone, 'Signup'));
    } catch (problem) {
      const message = describeProblem(problem).message;
      if (verified) {
        setSubmitError(message);
      } else {
        form.setError('phone', { message });
      }
    }
  });

  const verify = async (code: string) => {
    if (!challenge) {
      return;
    }

    const result = await authApi.verifyOtp(challenge.otpId, code, false);
    if (result.result === 'SignedIn') {
      // Numara zaten kayıtlı: yeni işletme açılmaz, panele girilir.
      await authApi.signedIn(result.accessToken, '/');
      return;
    }

    if (result.signupToken) {
      await createBusiness(result.signupToken);
    }
  };

  if (challenge) {
    return (
      <AuthLayout>
        <CodeStep
          challenge={challenge}
          eyebrow="2 / 2 · Telefon doğrulama"
          submitLabel="İşletmeyi oluştur"
          showRemember={false}
          onBack={() => {
            setChallenge(null);
          }}
          onVerify={verify}
          onResend={() =>
            authApi.requestOtp(parseMobilePhone(form.getValues('phone')) ?? '', 'Signup')
          }
        />
      </AuthLayout>
    );
  }

  const errors = form.formState.errors;
  return (
    <AuthLayout>
      <form className={styles.fields} onSubmit={(event) => void submit(event)} noValidate>
        <div className={styles.header}>
          <p className={styles.eyebrow}>
            {verified ? 'Ücretsiz deneme, kart gerekmez' : '1 / 2 · Ücretsiz deneme, kart gerekmez'}
          </p>
          <h1 className={styles.title}>İşletmenizi oluşturun</h1>
          {verified ? (
            <p className={styles.description}>{verified.phoneMasked} numarası doğrulandı.</p>
          ) : null}
        </div>
        {submitError ? <Banner tone="hata" title={submitError} /> : null}
        <TextField
          label="İşletme adı"
          placeholder="Kuaför Elif"
          autoComplete="organization"
          error={errors.businessName?.message}
          {...form.register('businessName')}
        />
        <Select
          label="Ne tür bir işletme?"
          options={industries}
          {...form.register('industryCode')}
        />
        {verified ? null : (
          <TextField
            label="Cep telefonunuz"
            type="tel"
            inputMode="tel"
            autoComplete="tel-national"
            placeholder="0532 123 45 67"
            hint="Giriş ve doğrulama için"
            error={errors.phone?.message}
            {...form.register('phone')}
          />
        )}
        <div>
          <Checkbox
            label="Kullanım koşullarını ve aydınlatma metnini okudum"
            {...form.register('acceptedTerms')}
          />
          {errors.acceptedTerms ? (
            <p role="alert" className={styles.footer}>
              {errors.acceptedTerms.message}
            </p>
          ) : null}
        </div>
        <Button
          type="submit"
          variant="primary"
          size="lg"
          className={styles.submit}
          loading={form.formState.isSubmitting}
          loadingLabel="Gönderiliyor…"
        >
          {verified ? 'İşletmeyi oluştur' : 'SMS kodu gönder'}
        </Button>
        <p className={styles.footer}>
          Zaten hesabınız var mı?{' '}
          <Link to="/giris" className={styles.link}>
            Giriş yapın
          </Link>
        </p>
      </form>
    </AuthLayout>
  );
}
