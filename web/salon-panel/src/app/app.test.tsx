import { cleanup, screen, within } from '@testing-library/react';
import { afterEach, describe, expect, it } from 'vitest';
import { sessionFixture, staffPermissions } from '../test/fake-api';
import { renderApp } from '../test/render-app';
import { activeMenuKey, visibleMenu } from './menu';

// Kural: salon-panel/AGENTS.md (menü izinle; şerit desenleri; 404/403), ADR-0007 (jeton bellekte, yenileme çereziyle),
// dilim-01 kabul kriterleri 1 (deneme 14 gün) ve 10 (personel menüsü kısıtlı), Dilim 1 varsayımı 1 (yapılmamış bölümler gizli).
afterEach(cleanup);

describe('oturum', () => {
  it('yenileme çerezi yoksa girişe yönlenir ve dönülecek adresi taşır', async () => {
    const { router } = renderApp('/hizmetler', { signedIn: false });
    expect(await screen.findByRole('heading', { name: 'Panele giriş' })).toBeTruthy();
    expect(router.state.location.search).toEqual({ yonlendir: '/hizmetler' });
  });

  it('sayfa yenilenince çerezle oturum açılır; sahibin menüsü Dilim 1 bölümleri', async () => {
    renderApp('/');
    const nav = await screen.findByRole('navigation', { name: 'Ana menü' });
    const labels = within(nav)
      .getAllByRole('link')
      .map((a) => a.textContent);
    expect(labels).toEqual(['Genel bakış', 'Hizmetler', 'Personel', 'Ayarlar']);
    expect(
      within(nav).getByRole('link', { name: 'Genel bakış' }).getAttribute('aria-current'),
    ).toBe('page');
    expect(screen.getByRole('heading', { level: 1, name: 'Genel bakış' })).toBeTruthy();
  });

  it('personelde yönetim menüsü yok', async () => {
    renderApp('/', {
      session: sessionFixture({
        permissions: staffPermissions,
        membership: { role: 'Staff', branchIds: [], staffId: null },
      }),
    });
    const nav = await screen.findByRole('navigation', { name: 'Ana menü' });
    expect(
      within(nav)
        .getAllByRole('link')
        .map((a) => a.textContent),
    ).toEqual(['Genel bakış']);
    expect(within(nav).getByText('Personel')).toBeTruthy();
  });
});

describe('abonelik şeridi', () => {
  it('deneme: kalan gün şubenin gününe göre', async () => {
    renderApp('/');
    expect((await screen.findByRole('status')).textContent).toContain(
      'Deneme sürenizin bitmesine 14 gün var',
    );
  });

  it('salt okunur: hata şeridi', async () => {
    renderApp('/', { session: sessionFixture({ subscription: { status: 'ReadOnly' } }) });
    expect((await screen.findByRole('alert')).textContent).toContain('Online randevu kapalı.');
  });

  it('aktif abonelikte şerit yok', async () => {
    renderApp('/', { session: sessionFixture({ subscription: { status: 'Active' } }) });
    await screen.findByRole('heading', { level: 1, name: 'Genel bakış' });
    expect(screen.queryByRole('status')).toBeNull();
  });
});

it('panel içinde bilinmeyen adres: 404 kenar menüyle', async () => {
  renderApp('/olmayan-sayfa');
  expect(
    await screen.findByRole('heading', { level: 1, name: 'Bu kayıt bulunamadı' }),
  ).toBeTruthy();
  expect(screen.getByRole('navigation', { name: 'Ana menü' })).toBeTruthy();
});

describe('menü', () => {
  it('izin ve dilim süzgeci', () => {
    expect(visibleMenu(() => true).map((e) => e.key)).toEqual([
      'genel',
      'hizmetler',
      'personel',
      'ayarlar',
    ]);
    expect(visibleMenu(() => false).map((e) => e.key)).toEqual(['genel']);
  });

  it('etkin öğe yoldan', () => {
    expect(activeMenuKey('/')).toBe('genel');
    expect(activeMenuKey('/ayarlar/isletme')).toBe('ayarlar');
    expect(activeMenuKey('/hizmetlerim')).toBe('');
  });
});
