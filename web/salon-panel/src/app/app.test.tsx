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

  it('sayfa yenilenince çerezle oturum açılır; sahibin menüsü açılmış bölümler', async () => {
    renderApp('/');
    const nav = await screen.findByRole('navigation', { name: 'Ana menü' });
    const labels = within(nav)
      .getAllByRole('link')
      .map((a) => a.textContent);
    expect(labels).toEqual(['Genel bakış', 'Takvim', 'Hizmetler', 'Personel', 'Ayarlar']);
    expect(
      within(nav).getByRole('link', { name: 'Genel bakış' }).getAttribute('aria-current'),
    ).toBe('page');
    expect(
      await screen.findByRole('heading', { level: 1, name: 'Hoş geldiniz, Elif' }),
    ).toBeTruthy();
  });

  it('personelde yönetim menüsü yok; kişisel menü "Benim" (kabul kriteri 10)', async () => {
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
    ).toEqual(['Takvimim']);
    expect(within(nav).getByText('Benim')).toBeTruthy();
    expect(within(nav).queryByText('Yönetim')).toBeNull();
    expect(within(nav).getByText('Personel')).toBeTruthy();
  });
});

describe('abonelik şeridi', () => {
  it('deneme: son haftada şerit (T-40)', async () => {
    renderApp('/', {
      session: sessionFixture({ subscription: { status: 'Trialing', trialEndsOn: '2026-11-30' } }),
    });
    expect((await screen.findByRole('status')).textContent).toContain(
      'Deneme sürenizin bitmesine 5 gün var',
    );
  });

  it('deneme 14 gün: şerit yok, kurulumda bağlamda yazar', async () => {
    renderApp('/');
    expect(await screen.findByText('Kuaför Elif · deneme 14 gün')).toBeTruthy();
    expect(screen.queryByRole('status')).toBeNull();
  });

  it('salt okunur: hata şeridi', async () => {
    renderApp('/', { session: sessionFixture({ subscription: { status: 'ReadOnly' } }) });
    expect((await screen.findByRole('alert')).textContent).toContain('Online randevu kapalı.');
  });

  it('aktif abonelikte şerit yok', async () => {
    renderApp('/', { session: sessionFixture({ subscription: { status: 'Active' } }) });
    await screen.findByRole('heading', { level: 1, name: 'Hoş geldiniz, Elif' });
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
      'takvim',
      'hizmetler',
      'personel',
      'ayarlar',
    ]);
    // Raporları görmeyen kullanıcı (personel) kişisel menüyü görür; bugün yalnız Takvimim açık (calendar.view).
    expect(visibleMenu((p) => p === 'calendar.view').map((e) => e.key)).toEqual(['takvimim']);
    expect(visibleMenu(() => false)).toEqual([]);
  });

  it('personel menüsü bölümler gelince Takvimim, Müşteriler, Kapıda sıra, Primim (kabul kriteri 10)', () => {
    const staff = new Set(['calendar.view', 'customer.view', 'walkin.manage', 'commission.view']);
    expect(visibleMenu((p) => staff.has(p), true).map((e) => e.label)).toEqual([
      'Takvimim',
      'Müşteriler',
      'Kapıda sıra',
      'Primim',
    ]);
  });

  it('etkin öğe yoldan', () => {
    expect(activeMenuKey('/')).toBe('genel');
    expect(activeMenuKey('/', true)).toBe('takvimim');
    expect(activeMenuKey('/takvim', true)).toBe('takvimim');
    expect(activeMenuKey('/takvim')).toBe('takvim');
    expect(activeMenuKey('/ayarlar/isletme')).toBe('ayarlar');
    expect(activeMenuKey('/hizmetlerim')).toBe('');
  });
});
