import { ApiProblem } from '@omvila/api-client-ts';
import { cleanup, render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { useState } from 'react';
import { afterEach, expect, it, vi } from 'vitest';
import { describeAuthProblem, formatCountdown } from './authProblem';
import { CodeStep } from './CodeStep';
import { FilterChips } from './FilterChips';

// Kural: salon-paneli/GirisKod (kalan deneme, bekleme sayacı, yeniden gönderme) ortak kod adımı; platform paneli
// girişi aynı bileşeni kullanır. Filtre çipleri tek seçimli radyo grubudur.
afterEach(cleanup);

const challenge = { otpId: 'otp-1', phoneMasked: '0532 *** ** 05', resendAfterSeconds: 60 };

const problem = (code: string, meta: Record<string, unknown>) =>
  new ApiProblem(422, { type: 'about:blank', title: code, status: 422, code, meta });

it('hatalı kodda kalan denemeyi yazar', async () => {
  const onVerify = vi.fn(() => Promise.reject(problem('otp_invalid', { attemptsLeft: 4 })));
  render(
    <CodeStep
      challenge={challenge}
      submitLabel="Giriş yap"
      showRemember={false}
      onBack={() => undefined}
      onVerify={onVerify}
      onResend={() => Promise.resolve(challenge)}
    />,
  );
  expect(screen.getByText('0532 *** ** 05 numarasına 6 haneli kod gönderdik.')).toBeTruthy();
  expect(screen.getByText('Kod gelmedi mi? 1:00 sonra tekrar gönderebilirsiniz.')).toBeTruthy();
  expect(screen.queryByRole('checkbox')).toBeNull();
  await userEvent.click(screen.getByLabelText('Doğrulama kodu, hane 1'));
  await userEvent.keyboard('482193');
  await userEvent.click(screen.getByRole('button', { name: 'Giriş yap' }));
  expect(onVerify).toHaveBeenCalledWith('482193', true);
  expect(await screen.findByText(/4 deneme hakkınız kaldı\./)).toBeTruthy();
});

it('deneme sınırında bekleme süresini verir', () => {
  expect(describeAuthProblem(problem('otp_attempts_exceeded', { retryAfterSeconds: 300 }))).toEqual(
    expect.objectContaining({ retryAfterSeconds: 300 }),
  );
  expect(formatCountdown(272)).toBe('4:32');
});

it('filtre çipleri tek seçimdir', async () => {
  function Harness() {
    const [value, setValue] = useState<'all' | 'trial'>('all');
    return (
      <FilterChips
        legend="Süzgeç"
        name="suzgec"
        options={[
          { value: 'all', label: 'Tümü' },
          { value: 'trial', label: 'Denemede' },
        ]}
        value={value}
        onChange={setValue}
      />
    );
  }

  render(<Harness />);
  expect(screen.getByRole('radio', { name: 'Tümü' })).toHaveProperty('checked', true);
  await userEvent.click(screen.getByRole('radio', { name: 'Denemede' }));
  expect(screen.getByRole('radio', { name: 'Denemede' })).toHaveProperty('checked', true);
  expect(screen.getByRole('radio', { name: 'Tümü' })).toHaveProperty('checked', false);
});
