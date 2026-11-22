import { cleanup, screen, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { sessionFixture } from '@/test/fake-api';
import { renderApp } from '@/test/render-app';

// Kural: platform-paneli/PDenetim: işlem adı ve ayrıntı istemcide üretilir; kategori, zaman, arama ve marka süzgeçleri
// sözleşme parametreleriyle gider; "Dışa aktar" yalnız Yönetici (platform.audit.export), diğer rollerde kilitli (KK4).
const downloads: string[] = [];
beforeEach(() => {
  downloads.length = 0;
  URL.createObjectURL = () => 'blob:test';
  URL.revokeObjectURL = () => undefined;
  vi.spyOn(HTMLAnchorElement.prototype, 'click').mockImplementation(function (
    this: HTMLAnchorElement,
  ) {
    downloads.push(this.download);
  });
});
afterEach(() => {
  cleanup();
  vi.restoreAllMocks();
});

describe('denetim kaydı', () => {
  it('satırlar: zaman, kim, işlem, marka, ayrıntı', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/denetim', { calls });
    const table = await screen.findByRole('table', { name: 'Denetim kaydı' });
    expect(screen.getByText('Tüm platform işlemleri · silinemez')).toBeTruthy();
    expect(within(table).getByText('01:12')).toBeTruthy();
    expect(within(table).getByText('Marka görüntülendi')).toBeTruthy();
    expect(within(table).getByText('Özet sekmesi')).toBeTruthy();
    expect(within(table).getByText('Havale eşleştirildi')).toBeTruthy();
    expect(within(table).getByText('5.000 SMS kontörü · KNT-SM-0412')).toBeTruthy();
    expect(within(table).getByText('Salt okunura geçti')).toBeTruthy();
    expect(within(table).getByText('Ödeme gecikti → Salt okunur')).toBeTruthy();
    expect(within(table).getByText('Sistem')).toBeTruthy();
    expect(within(table).getByText('Otomatik')).toBeTruthy();
    // Son 24 saat: fromUtc = şimdi − 24 saat.
    expect(calls['/api/v1/admin/audit-log']?.at(-1)).toEqual({
      fromUtc: '2026-11-19T07:00:00.000Z',
      limit: '50',
    });
  });

  it('kategori, zaman ve arama süzgeçleri', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/denetim', { calls });
    await screen.findByRole('table', { name: 'Denetim kaydı' });
    await userEvent.click(screen.getByRole('radio', { name: 'Onay ve ret' }));
    await userEvent.selectOptions(screen.getByLabelText('Zaman'), '7g');
    await userEvent.type(
      screen.getByRole('searchbox', { name: 'Marka veya kişi ara' }),
      'Ayşe{Enter}',
    );
    const table = await screen.findByRole('table', { name: 'Denetim kaydı' });
    await within(table).findByText('Havale eşleştirildi');
    expect(within(table).queryByText('Marka görüntülendi')).toBeNull();
    expect(calls['/api/v1/admin/audit-log']?.at(-1)).toEqual({
      fromUtc: '2026-11-13T07:00:00.000Z',
      category: 'ApproveReject',
      q: 'Ayşe',
      limit: '50',
    });
  });

  it('marka adına tıklayınca yalnız o markanın kayıtları', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/denetim', { calls });
    await userEvent.click(
      await screen.findByRole('button', { name: 'Yalnız Kuaför Elif kayıtlarını göster' }),
    );
    expect(await screen.findByText(/Kuaför Elif kayıtları/)).toBeTruthy();
    expect(calls['/api/v1/admin/audit-log']?.at(-1)).toMatchObject({
      brandId: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5001',
    });
  });

  it('Destek rolünde "Dışa aktar" kilitli', async () => {
    renderApp('/denetim');
    const button = await screen.findByRole('button', { name: 'Dışa aktar' });
    expect((button as HTMLButtonElement).disabled).toBe(true);
    expect(
      document.getElementById(button.getAttribute('aria-describedby') ?? '')?.textContent,
    ).toBe('Yönetici yetkisi ister');
  });

  it('Yönetici aynı süzgeçlerle CSV indirir', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/denetim?kategori=View', { calls, session: sessionFixture('PlatformAdmin') });
    await screen.findByRole('table', { name: 'Denetim kaydı' });
    await userEvent.click(screen.getByRole('button', { name: 'Dışa aktar' }));
    await vi.waitFor(() => {
      expect(downloads).toEqual(['denetim-kaydi-2026-11-20.csv']);
    });
    expect(calls['/api/v1/admin/audit-log/export']?.[0]).toEqual({
      fromUtc: '2026-11-19T07:00:00.000Z',
      category: 'View',
    });
  });

  it('süzgece uyan kayıt yoksa boş hâl', async () => {
    renderApp('/denetim?kategori=Kvkk');
    expect(await screen.findByRole('heading', { name: 'Bu süzgece uyan kayıt yok' })).toBeTruthy();
  });
});
