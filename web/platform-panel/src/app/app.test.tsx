import { cleanup, screen, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { afterEach, describe, expect, it } from 'vitest';
import { renderApp } from '@/test/render-app';
import { auth } from './auth';
import { menuEntries, visibleMenu } from './menu';
import { toSessionApi } from './session';
import { sessionFixture } from '@/test/fake-api';

// Kural: K2 (telefon + SMS kodu, kayıt yok), T-116 (1) (yenileme jetonu gövdede, sekme ömrü: sessionStorage), roller
// (roller-yetkiler.md § Platform rolleri), kenar menü (platform-paneli/Main), 403/404.
afterEach(() => {
  cleanup();
  auth.clear();
});

async function signIn() {
  await userEvent.type(await screen.findByLabelText('Cep telefonu'), '0532 700 10 20');
  await userEvent.click(screen.getByRole('button', { name: 'SMS kodu gönder' }));
  expect(await screen.findByText('0532 *** ** 20 numarasına 6 haneli kod gönderdik.')).toBeTruthy();
  expect(screen.queryByRole('checkbox', { name: /hatırla/ })).toBeNull();
  await userEvent.click(screen.getByLabelText('Doğrulama kodu, hane 1'));
  await userEvent.keyboard('482193');
  await userEvent.click(screen.getByRole('button', { name: 'Giriş yap' }));
}

describe('giriş ve oturum', () => {
  it('oturum yoksa girişe yönlenir; kodla girince jetonlar saklanır ve panel açılır', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/markalar', { signedIn: false, calls });
    expect(
      await screen.findByRole('heading', { level: 1, name: 'Platform paneline giriş' }),
    ).toBeTruthy();
    // Yenileme jetonu yoksa yenileme ucu çağrılmaz.
    expect(calls['/api/v1/admin/auth/refresh']).toBeUndefined();
    await signIn();

    expect(await screen.findByRole('heading', { level: 1, name: 'Markalar' })).toBeTruthy();
    expect(calls['/api/v1/admin/auth/otp/request']?.[0]).toEqual({ phone: '+905327001020' });
    expect(calls['/api/v1/admin/auth/otp/verify']?.[0]).toEqual({
      otpId: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fa001',
      code: '482193',
    });
    expect(auth.getToken()).toBe('erisim-1');
    expect(sessionStorage.getItem('omvila.platform.yenileme')).toBe('yenileme-1');
    expect(localStorage.getItem('omvila.platform.yenileme')).toBeNull();
  });

  it('"Çıkış yap" yenileme jetonunu sunucuda iptal eder, sekmeden siler ve girişe döner', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/', { calls });
    const nav = await screen.findByRole('navigation', { name: 'Ana menü' });
    await userEvent.click(within(nav).getByRole('button', { name: 'Çıkış yap' }));
    expect(
      await screen.findByRole('heading', { level: 1, name: 'Platform paneline giriş' }),
    ).toBeTruthy();
    expect(calls['/api/v1/admin/auth/logout']?.[0]).toEqual({ refreshToken: 'yenileme-2' });
    expect(auth.getToken()).toBeNull();
    expect(sessionStorage.getItem('omvila.platform.yenileme')).toBeNull();
  });

  it('hatalı kod kalan denemeyi yazar', async () => {
    renderApp('/giris', { signedIn: false, verify: 'otp_invalid' });
    await signIn();
    expect(await screen.findByText('Kod hatalı. 4 deneme hakkınız kaldı.')).toBeTruthy();
  });

  it('sayfa yenilenince sekmedeki yenileme jetonuyla oturum sürer (rotation)', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/', { calls });
    expect(await screen.findByRole('heading', { level: 1, name: 'Platform' })).toBeTruthy();
    expect(calls['/api/v1/admin/auth/refresh']?.[0]).toEqual({ refreshToken: 'yenileme-0' });
    expect(sessionStorage.getItem('omvila.platform.yenileme')).toBe('yenileme-2');
  });
});

describe('düzen ve roller', () => {
  it('kenar menü: bölümler, onay rozeti, rol ve ad', async () => {
    renderApp('/');
    const nav = await screen.findByRole('navigation', { name: 'Ana menü' });
    expect(
      within(nav).getByRole('link', { name: 'Genel bakış' }).getAttribute('aria-current'),
    ).toBe('page');
    expect(within(nav).getByRole('link', { name: 'Markalar' })).toBeTruthy();
    for (const name of [/Onay kuyrukları/, 'Abonelikler', 'Denetim kaydı', 'Planlar ve fiyat']) {
      expect(within(nav).getByRole('link', { name })).toBeTruthy();
    }
    expect(within(nav).getByText('Destek · OMVILA')).toBeTruthy();
    // Zil rozeti oturumdaki onay sayısı.
    expect(screen.getByRole('button', { name: 'Bildirimler, 7 okunmamış' })).toBeTruthy();
  });

  it('menü izinle süzülür (KK4: menüde izin denetimi)', () => {
    const support = toSessionApi(sessionFixture());
    expect(visibleMenu(support.can).map((e) => e.key)).toEqual(menuEntries.map((e) => e.key));
    const none = toSessionApi({ ...sessionFixture(), permissions: ['platform.brands.view'] });
    expect(visibleMenu(none.can).map((e) => e.key)).toEqual(['genel', 'markalar', 'abonelikler']);
  });

  it('uçtan 403 gelirse yetki sayfası', async () => {
    renderApp('/', { failures: { '/api/v1/admin/overview': 403 } });
    expect(
      await screen.findByRole('heading', { level: 1, name: 'Bu sayfayı görme yetkiniz yok' }),
    ).toBeTruthy();
  });

  it('izni olmayan rol adresi elle yazsa da sayfayı göremez', async () => {
    renderApp('/denetim', {
      session: { ...sessionFixture(), permissions: ['platform.brands.view'] },
    });
    expect(
      await screen.findByRole('heading', { level: 1, name: 'Bu sayfayı görme yetkiniz yok' }),
    ).toBeTruthy();
  });

  it('bilinmeyen adres 404', async () => {
    renderApp('/olmayan-sayfa');
    expect(
      await screen.findByRole('heading', { level: 1, name: 'Bu kayıt bulunamadı' }),
    ).toBeTruthy();
  });
});
