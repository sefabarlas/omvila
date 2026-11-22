import type { StoryDefault } from '@ladle/react';
import { useState } from 'react';
import { AuthLayout } from './AuthLayout';
import { CodeStep } from './CodeStep';
import { FilterChips } from './FilterChips';

export default { title: 'Panel/Giriş' } satisfies StoryDefault;

const challenge = { otpId: 'otp-1', phoneMasked: '0532 *** ** 05', resendAfterSeconds: 60 };

export const KodAdimi = () => (
  <AuthLayout sloganTitle="Her randevu tek yerde." sloganText="Salonunuzun bütün günü tek ekranda.">
    <CodeStep
      challenge={challenge}
      submitLabel="Giriş yap"
      showRemember
      onBack={() => undefined}
      onVerify={() => Promise.resolve()}
      onResend={() => Promise.resolve(challenge)}
    />
  </AuthLayout>
);

export const FiltreCipleri = () => {
  const [value, setValue] = useState('all');
  return (
    <FilterChips
      legend="Süzgeç"
      name="suzgec"
      options={[
        { value: 'all', label: 'Tümü' },
        { value: 'trial', label: 'Denemede' },
        { value: 'pastDue', label: 'Ödeme gecikti' },
      ]}
      value={value}
      onChange={setValue}
    />
  );
};
