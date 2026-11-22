import { cleanup, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { afterEach, describe, expect, it, vi } from 'vitest';
import { goToExternal } from '@/shared/external-navigation';
import { sessionFixture, staffPermissions } from '../../test/fake-api';
import { renderApp } from '../../test/render-app';

vi.mock('@/shared/external-navigation', () => ({ goToExternal: vi.fn() }));

// Kural: dilim-06 kapsam (Google Takvim tek yönlü aktarım), K7 (sahte sağlayıcı; bağlan / bağlantıyı kes / durum);
// sözleşme manageGetGoogleCalendarStatus, manageGetGoogleCalendarConnectUrl (client=web), manageCompleteGoogleCalendarConnect
// (400 state invalid_format), manageDisconnectGoogleCalendar (204); StaffDetail.googleCalendarStatus; salon-paneli/Personel
// ("Google Takvim · bağlı", "Bağlantıyı kes"), Durumlar ("Google Takvim bağlantısı koptu").
afterEach(() => {
  cleanup();
  vi.mocked(goToExternal).mockClear();
});

const card = () => screen.findByRole('region', { name: 'Google Takvim' });

describe('Google Takvim', () => {
  it('bağlı değil: "Bağlan" sağlayıcının adresine yönlendirir', async () => {
    renderApp('/personel', { withServices: true });
    expect((await card()).textContent).toContain('Google Takvim · bağlı değil');
    await userEvent.click(screen.getByRole('button', { name: 'Bağlan' }));
    await vi.waitFor(() => {
      expect(goToExternal).toHaveBeenCalledWith(
        'http://localhost:5173/ayarlar/google-takvim?code=fake-kod&state=ornek-state',
      );
    });
  });

  it('bağlı: maskeli hesap, tek yönlü aktarım notu; "Bağlantıyı kes"', async () => {
    renderApp('/personel', { withServices: true, access: { google: 'Active' } });
    expect((await screen.findByText('Google Takvim · bağlı')).textContent).toBeTruthy();
    expect(screen.getByText(/^el\*\*\*@gmail\.com · randevular tek yönlü aktarılır/)).toBeTruthy();
    await userEvent.click(screen.getByRole('button', { name: 'Bağlantıyı kes' }));
    expect(await screen.findByText('Google Takvim bağlantısı kesildi')).toBeTruthy();
    expect(await screen.findByText('Google Takvim · bağlı değil')).toBeTruthy();
  });

  it('bağlantı koptu: uyarı ve "Yeniden bağlan"', async () => {
    renderApp('/personel', { withServices: true, access: { google: 'TokenExpired' } });
    expect(await screen.findByText('Google Takvim bağlantınızın süresi doldu.')).toBeTruthy();
    expect(screen.getByRole('button', { name: 'Yeniden bağlan' })).toBeTruthy();
  });

  it('personel kaydı yoksa kart yok; başka personelde bağlantı yoksa kart yok', async () => {
    renderApp('/personel', { withServices: true, access: { google: 'missing' } });
    expect(await screen.findByRole('region', { name: 'Verdiği hizmetler' })).toBeTruthy();
    expect(screen.queryByRole('region', { name: 'Google Takvim' })).toBeNull();
    await userEvent.click(screen.getByRole('button', { name: /Mert Demir/ }));
    expect(await screen.findByRole('region', { name: 'Verdiği hizmetler' })).toBeTruthy();
    expect(screen.queryByRole('region', { name: 'Google Takvim' })).toBeNull();
  });

  it('geri dönüş: bağlantı tamamlanır, Personel sayfasına dönülür', async () => {
    const calls: Record<string, unknown[]> = {};
    const { router } = renderApp('/ayarlar/google-takvim?code=fake-kod&state=ornek-state', {
      calls,
      withServices: true,
    });
    expect(await screen.findByText('Google Takvim bağlandı')).toBeTruthy();
    await vi.waitFor(() => {
      expect(router.state.location.pathname).toBe('/personel');
    });
    expect(calls['/api/v1/manage/integrations/google/callback']).toEqual([
      { code: 'fake-kod', state: 'ornek-state' },
    ]);
    expect(await screen.findByText('Google Takvim · bağlı')).toBeTruthy();
  });

  it('geri dönüş hatası: geçersiz state ve eksik değer', async () => {
    renderApp('/ayarlar/google-takvim?code=fake-kod&state=eski', {
      access: { googleCallbackInvalid: true },
    });
    expect(await screen.findByRole('heading', { name: 'Google Takvim bağlanamadı' })).toBeTruthy();
    expect(
      screen.getByText(
        'Bağlantı adresinin süresi dolmuş veya geçersiz. Yeniden bağlanmayı deneyin.',
      ),
    ).toBeTruthy();
    cleanup();
    const calls: Record<string, unknown[]> = {};
    renderApp('/ayarlar/google-takvim?error=access_denied', { calls });
    expect(await screen.findByRole('heading', { name: 'Google Takvim bağlanamadı' })).toBeTruthy();
    expect(calls['/api/v1/manage/integrations/google/callback']).toBeUndefined();
  });

  it('personel rolü (ayarlar izni yok) geri dönüşte takvime döner', async () => {
    const { router } = renderApp('/ayarlar/google-takvim?code=fake-kod&state=ornek-state', {
      session: sessionFixture({
        membership: {
          role: 'Staff',
          branchIds: ['0193a8f0-0000-7000-8000-0000000000b1'],
          staffId: '0193a8f0-0000-7000-8000-0000000000c2',
        },
        permissions: staffPermissions,
      }),
    });
    expect(await screen.findByText('Google Takvim bağlandı')).toBeTruthy();
    await vi.waitFor(() => {
      expect(router.state.location.pathname).toBe('/takvim');
    });
  });
});
