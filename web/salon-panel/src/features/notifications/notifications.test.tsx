import { cleanup, screen, waitFor, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { afterEach, describe, expect, it } from 'vitest';
import { ownerPermissions, sessionFixture, staffPermissions } from '../../test/fake-api';
import { notificationIds } from '../../test/fake-notifications';
import { renderApp } from '../../test/render-app';

// Kural: dilim-03 § Kapsam (bildirim paneli) ve kabul kriteri 7 ("Kontör az" şeridi); salon-paneli/BildirimPaneli ve
// Durumlar; sözleşme manageListInAppNotifications (zildeki sayı unreadCount, sahte sayı yok),
// manageMarkNotificationsRead; ManageSession.smsCredits (eşik sunucuda, isLow).
afterEach(() => {
  cleanup();
  localStorage.clear();
});

describe('bildirim paneli', () => {
  it('zildeki sayı sunucudan; panelde satırlar, kalan süre ve zaman', async () => {
    renderApp('/hizmetler');
    const bell = await screen.findByRole('button', { name: 'Bildirimler, 3 okunmamış' });
    expect(bell.getAttribute('aria-expanded')).toBe('false');
    await userEvent.click(bell);
    expect(bell.getAttribute('aria-expanded')).toBe('true');
    const panel = screen.getByRole('dialog', { name: 'Bildirimler' });
    expect(bell.getAttribute('aria-controls')).toBe(panel.id);
    const rows = await within(panel).findAllByRole('button', { name: /·|SMS/ });
    expect(rows.map((row) => row.textContent)).toEqual([
      'Yeni online talep · Pınar Yalçın (okunmadı)Cilt bakımı, bugün 16:00 · 1 sa 35 dk kaldı12:02',
      'Müşteri iptal etti · Hande Er (okunmadı)Saç kesimi 15:00 · Mert · sebep: planım değişti10:15',
      'Yeni değerlendirme · 3 yıldızEce Koç · Zeynep09:40',
      'SMS kontörü azalıyor (okunmadı)1.240 kontör · ~18 günDün',
    ]);
    expect(document.activeElement).toBe(panel);
  });

  it('satıra dokunmak okundu sayar ve hedef sayfayı açar', async () => {
    const calls: Record<string, unknown[]> = {};
    const { router } = renderApp('/hizmetler', { calls });
    await userEvent.click(await screen.findByRole('button', { name: 'Bildirimler, 3 okunmamış' }));
    const panel = screen.getByRole('dialog', { name: 'Bildirimler' });
    await userEvent.click(
      await within(panel).findByRole('button', { name: /Müşteri iptal etti · Hande Er/ }),
    );
    await waitFor(() => {
      expect(router.state.location.pathname).toBe('/takvim');
    });
    expect(calls['/api/v1/manage/notifications/read']).toEqual([
      { notificationIds: [notificationIds.cancelled] },
    ]);
    expect(screen.queryByRole('dialog', { name: 'Bildirimler' })).toBeNull();
    expect(await screen.findByRole('button', { name: 'Bildirimler, 2 okunmamış' })).toBeTruthy();
  });

  it('okunmuş bildirim yeniden okundu sayılmaz', async () => {
    const calls: Record<string, unknown[]> = {};
    const { router } = renderApp('/hizmetler', { calls });
    await userEvent.click(await screen.findByRole('button', { name: 'Bildirimler, 3 okunmamış' }));
    await userEvent.click(await screen.findByRole('button', { name: /Yeni değerlendirme/ }));
    await waitFor(() => {
      expect(router.state.location.pathname).toBe('/');
    });
    expect(calls['/api/v1/manage/notifications/read']).toBeUndefined();
  });

  it('"Tümünü okundu say" rozeti kaldırır', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/hizmetler', { calls });
    await userEvent.click(await screen.findByRole('button', { name: 'Bildirimler, 3 okunmamış' }));
    const panel = screen.getByRole('dialog', { name: 'Bildirimler' });
    await userEvent.click(within(panel).getByRole('button', { name: 'Tümünü okundu say' }));
    expect(await screen.findByRole('button', { name: 'Bildirimler' })).toBeTruthy();
    expect(calls['/api/v1/manage/notifications/read']).toEqual([{ all: true }]);
    expect(within(panel).queryByRole('button', { name: 'Tümünü okundu say' })).toBeNull();
    expect(within(panel).queryByText(/okunmadı/)).toBeNull();
  });

  it('Esc paneli kapatır, odak zile döner', async () => {
    renderApp('/hizmetler');
    const bell = await screen.findByRole('button', { name: 'Bildirimler, 3 okunmamış' });
    bell.focus();
    await userEvent.keyboard('{Enter}');
    expect(screen.getByRole('dialog', { name: 'Bildirimler' })).toBeTruthy();
    await userEvent.keyboard('{Escape}');
    expect(screen.queryByRole('dialog', { name: 'Bildirimler' })).toBeNull();
    expect(document.activeElement).toBe(bell);
  });

  it('dışarı tıklayınca kapanır', async () => {
    renderApp('/hizmetler');
    await userEvent.click(await screen.findByRole('button', { name: 'Bildirimler, 3 okunmamış' }));
    await userEvent.click(screen.getByRole('heading', { level: 1 }));
    expect(screen.queryByRole('dialog', { name: 'Bildirimler' })).toBeNull();
  });

  it('bildirim yoksa rozet yok ve boş hâl', async () => {
    renderApp('/hizmetler', { notifications: { empty: true } });
    await screen.findByRole('heading', { level: 1, name: 'Hizmetler' });
    const bell = await screen.findByRole('button', { name: 'Bildirimler' });
    expect(bell.parentElement?.textContent).toBe('');
    await userEvent.click(bell);
    expect(await screen.findByRole('heading', { name: 'Bildiriminiz yok' })).toBeTruthy();
    expect(screen.queryByRole('button', { name: 'Tümünü okundu say' })).toBeNull();
  });

  it('yüklenemezse rozet yok; panelde hata ve tekrar deneme', async () => {
    renderApp('/hizmetler', { notifications: { error: true } });
    await screen.findByRole('heading', { level: 1, name: 'Hizmetler' });
    const bell = screen.getByRole('button', { name: 'Bildirimler' });
    await userEvent.click(bell);
    expect(await screen.findByRole('heading', { name: 'Bildirimler yüklenemedi' })).toBeTruthy();
    expect(screen.getByRole('button', { name: 'Tekrar dene' })).toBeTruthy();
    expect(bell.parentElement?.querySelector('[aria-hidden="true"]:not(svg)')).toBeNull();
  });
});

describe('kontör az şeridi', () => {
  it('isLow iken bakiye ve "Kontör al": sahipte kontör satın alma penceresi (T-125)', async () => {
    renderApp('/hizmetler', {
      session: sessionFixture({ smsCredits: { balance: 180, isLow: true } }),
    });
    expect(await screen.findByText("SMS kontörünüz 180'e düştü.")).toBeTruthy();
    await userEvent.click(screen.getByRole('button', { name: 'Kontör al' }));
    expect(await screen.findByRole('dialog', { name: 'Kontör al' })).toBeTruthy();
  });

  it('kontör alamayan yöneticide "Kontör al" ayarları açar', async () => {
    const { router } = renderApp('/hizmetler', {
      session: sessionFixture({
        smsCredits: { balance: 180, isLow: true },
        permissions: ownerPermissions.filter((p) => p !== 'subscription.manage'),
      }),
    });
    expect(await screen.findByText("SMS kontörünüz 180'e düştü.")).toBeTruthy();
    expect(
      screen.getByText(
        "Bitince hatırlatma ve onay SMS'leri gitmez; uygulama bildirimleri devam eder.",
      ),
    ).toBeTruthy();
    await userEvent.click(screen.getByRole('button', { name: 'Kontör al' }));
    await waitFor(() => {
      expect(router.state.location.pathname).toBe('/ayarlar/bildirimler');
    });
  });

  it('kontör bitince "bitti" yazar', async () => {
    renderApp('/hizmetler', {
      session: sessionFixture({ smsCredits: { balance: 0, isLow: true } }),
    });
    expect(await screen.findByText('SMS kontörünüz bitti.')).toBeTruthy();
  });

  it('eşiğin üstünde şerit yok (eşik sunucuda; istemci sayıyı karşılaştırmaz)', async () => {
    renderApp('/hizmetler', {
      session: sessionFixture({ smsCredits: { balance: 150, isLow: false } }),
    });
    await screen.findByRole('heading', { level: 1, name: 'Hizmetler' });
    expect(screen.queryByText(/SMS kontörünüz/)).toBeNull();
  });

  it('personel rolünde alan yok, şerit yok', async () => {
    renderApp('/musteriler', {
      session: sessionFixture({
        permissions: staffPermissions,
        membership: {
          role: 'Staff',
          branchIds: [],
          staffId: '0193a8f0-0000-7000-8000-0000000000c2',
        },
      }),
    });
    await screen.findByRole('heading', { level: 1, name: 'Müşteriler' });
    expect(screen.queryByText(/SMS kontörünüz/)).toBeNull();
  });
});
