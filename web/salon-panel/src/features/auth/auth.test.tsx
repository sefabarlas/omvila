import { cleanup, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { afterEach, describe, expect, it } from 'vitest';
import { renderApp } from '../../test/render-app';
import { signupHandoff } from './handoff';

// Kural: dilim-01 kabul kriterleri 1 (kayıt → panel), 2 (yanlış kod → kalan deneme; sınırda bekleme), 3 (kayıtsız numara
// SignupRequired → kayıt), 4 (30 gün hatırla), ADR-0007 (jeton bellekte), salon-paneli/Giris, GirisKod, Kayit.
afterEach(() => {
  cleanup();
  signupHandoff.clear();
});

async function enterPhoneAndCode(phone = '0532 418 77 05') {
  await userEvent.type(await screen.findByLabelText('Cep telefonu'), phone);
  await userEvent.click(screen.getByRole('button', { name: 'SMS kodu gönder' }));
  expect(await screen.findByText('0532 *** ** 05 numarasına 6 haneli kod gönderdik.')).toBeTruthy();
  await userEvent.click(screen.getByLabelText('Doğrulama kodu, hane 1'));
  await userEvent.keyboard('482193');
}

describe('giriş', () => {
  it('telefon → kod → panel; hatırla seçimi gönderilir', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/giris', { signedIn: false, calls });
    await enterPhoneAndCode();
    await userEvent.click(screen.getByRole('checkbox', { name: 'Bu bilgisayarda 30 gün hatırla' }));
    await userEvent.click(screen.getByRole('button', { name: 'Giriş yap' }));

    expect(
      await screen.findByRole('heading', { level: 1, name: 'Hoş geldiniz, Elif' }),
    ).toBeTruthy();
    expect(calls['/api/v1/manage/auth/otp/request']?.[0]).toEqual({
      phone: '+905324187705',
      purpose: 'Login',
    });
    expect(calls['/api/v1/manage/auth/otp/verify']?.[0]).toEqual({
      otpId: '0193a8f0-0000-7000-8000-0000000000f1',
      code: '482193',
      rememberDevice: false,
    });
  });

  it('geçersiz telefon alan hatası', async () => {
    renderApp('/giris', { signedIn: false });
    await userEvent.type(await screen.findByLabelText('Cep telefonu'), '0212 555 12 34');
    await userEvent.click(screen.getByRole('button', { name: 'SMS kodu gönder' }));
    expect(await screen.findByText('Telefon numarası 10 haneli olmalı.')).toBeTruthy();
  });

  it('yanlış kod: kalan deneme; sınır: bekleme şeridi', async () => {
    renderApp('/giris', { signedIn: false, verify: 'otp_invalid' });
    await enterPhoneAndCode();
    await userEvent.click(screen.getByRole('button', { name: 'Giriş yap' }));
    expect(await screen.findByText('Kod hatalı. 4 deneme hakkınız kaldı.')).toBeTruthy();
  });

  it('deneme sınırı aşılınca bekleme sayacı', async () => {
    renderApp('/giris', { signedIn: false, verify: 'otp_attempts_exceeded' });
    await enterPhoneAndCode();
    await userEvent.click(screen.getByRole('button', { name: 'Giriş yap' }));
    expect((await screen.findByRole('alert')).textContent).toContain('Yeniden deneme: 5:00');
  });

  it('yeniden gönderme sayacı', async () => {
    renderApp('/giris', { signedIn: false });
    await userEvent.type(await screen.findByLabelText('Cep telefonu'), '0532 418 77 05');
    await userEvent.click(screen.getByRole('button', { name: 'SMS kodu gönder' }));
    expect(
      await screen.findByText('Kod gelmedi mi? 1:00 sonra tekrar gönderebilirsiniz.'),
    ).toBeTruthy();
  });

  it('kayıtsız numara: doğrulanmış telefonla kayıt ekranı, tek adımda işletme', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/giris', { signedIn: false, verify: 'SignupRequired', calls });
    await enterPhoneAndCode();
    await userEvent.click(screen.getByRole('button', { name: 'Giriş yap' }));

    expect(await screen.findByText('0532 *** ** 05 numarası doğrulandı.')).toBeTruthy();
    expect(screen.queryByLabelText('Cep telefonunuz')).toBeNull();
    await userEvent.type(screen.getByLabelText('İşletme adı'), 'Kuaför Elif');
    await userEvent.click(
      screen.getByRole('checkbox', { name: 'Kullanım koşullarını ve aydınlatma metnini okudum' }),
    );
    await userEvent.click(screen.getByRole('button', { name: 'İşletmeyi oluştur' }));

    expect(
      await screen.findByRole('heading', { level: 1, name: 'Hoş geldiniz, Elif' }),
    ).toBeTruthy();
    expect(calls['/api/v1/manage/auth/signup']?.[0]).toEqual({
      signupToken: 'kayit-jetonu',
      businessName: 'Kuaför Elif',
      industryCode: 'hair_salon',
      acceptedTerms: true,
      privacyNoticeVersion: '2026-11-01',
    });
  });

  it('oturumu süren kullanıcı girişten panele döner', async () => {
    renderApp('/giris');
    expect(
      await screen.findByRole('heading', { level: 1, name: 'Hoş geldiniz, Elif' }),
    ).toBeTruthy();
  });
});

describe('kayıt', () => {
  it('işletme bilgileri → kod → işletme oluşur', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/kayit', { signedIn: false, verify: 'SignupRequired', calls });
    await userEvent.type(await screen.findByLabelText('İşletme adı'), 'Berber Kaan');
    await userEvent.selectOptions(screen.getByLabelText('Ne tür bir işletme?'), 'barber');
    await userEvent.type(screen.getByLabelText('Cep telefonunuz'), '0532 418 77 05');
    await userEvent.click(
      screen.getByRole('checkbox', { name: 'Kullanım koşullarını ve aydınlatma metnini okudum' }),
    );
    await userEvent.click(screen.getByRole('button', { name: 'SMS kodu gönder' }));

    expect(await screen.findByText('2 / 2 · Telefon doğrulama')).toBeTruthy();
    expect(screen.queryByRole('checkbox', { name: 'Bu bilgisayarda 30 gün hatırla' })).toBeNull();
    await userEvent.click(screen.getByLabelText('Doğrulama kodu, hane 1'));
    await userEvent.keyboard('482193');
    await userEvent.click(screen.getByRole('button', { name: 'İşletmeyi oluştur' }));

    expect(
      await screen.findByRole('heading', { level: 1, name: 'Hoş geldiniz, Elif' }),
    ).toBeTruthy();
    expect(calls['/api/v1/manage/auth/otp/request']?.[0]).toEqual({
      phone: '+905324187705',
      purpose: 'Signup',
    });
    expect(calls['/api/v1/manage/auth/signup']?.[0]).toMatchObject({
      businessName: 'Berber Kaan',
      industryCode: 'barber',
    });
  });

  it('koşullar onaylanmadan ve kısa adla gönderilmez', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/kayit', { signedIn: false, calls });
    await userEvent.type(await screen.findByLabelText('İşletme adı'), 'K');
    await userEvent.type(screen.getByLabelText('Cep telefonunuz'), '0532 418 77 05');
    await userEvent.click(screen.getByRole('button', { name: 'SMS kodu gönder' }));

    expect(await screen.findByText('İşletme adı en az 2 harf olmalı.')).toBeTruthy();
    expect(screen.getByText('Devam etmek için koşulları onaylayın.')).toBeTruthy();
    expect(calls['/api/v1/manage/auth/otp/request']).toBeUndefined();
  });
});
