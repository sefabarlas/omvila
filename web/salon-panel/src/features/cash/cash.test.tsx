import { cleanup, screen, waitFor, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { afterEach, describe, expect, it } from 'vitest';
import { ownerPermissions, sessionFixture, staffPermissions } from '../../test/fake-api';
import type { FakeApiOptions } from '../../test/fake-api';
import { renderApp } from '../../test/render-app';
import { paymentItemLabel, serviceList } from './cash-view';

// Kural: dilim-05 KK7 (gün kapatma: beklenen 3.450, sayılan 3.400 → fark −50 ve açıklama), sözleşme manageGetCashDay /
// manageCloseCashDay (Idempotency-Key; fark varsa differenceNote zorunlu; 409 cash_day_already_closed), PaymentView
// ("Kalem" istemcide), roller-yetkiler.md (Kasa: payment.collect; Günü kapat: cash.close; personel görmez).
// Gün: Çarşamba 25 Kasım 2026 (İstanbul); sahte kasada 7 tahsilat, nakit 900 ₺.
afterEach(() => {
  cleanup();
  localStorage.clear();
});

const owner = sessionFixture({ brand: { ...sessionFixture().brand, status: 'Active' } });

function openCash(path = '/kasa', options: FakeApiOptions = {}) {
  const calls: Record<string, unknown[]> = {};
  const rendered = renderApp(path, { calls, session: owner, ...options });
  return { calls, ...rendered };
}

const block = (label: string) => {
  const summary = screen.getByRole('group', { name: 'Kasa özeti' });
  const el = within(summary).getByText(label).parentElement;
  if (!el) {
    throw new Error('Blok yok');
  }

  return el;
};

describe('kasa satır metinleri', () => {
  it('hizmet listesi ve kalem: kapora, kalan, paket satışı', () => {
    expect(serviceList(['Boya', 'Fön'])).toBe('Boya ve fön');
    expect(serviceList(['Saç kesimi', 'Fön', 'Maske'])).toBe('Saç kesimi, fön ve maske');
    const base = {
      id: 'p',
      branchId: 'b',
      method: 'Card' as const,
      amount: { amountMinor: 124000, currency: 'TRY' as const },
      isDeposit: false,
      status: 'Recorded' as const,
      paidUtc: '2026-11-25T09:05:00Z',
      purpose: 'Appointment' as const,
    };
    const tz = 'Europe/Istanbul';
    expect(
      paymentItemLabel({ ...base, serviceNames: ['Boya', 'Fön'], isBalanceAfterDeposit: true }, tz),
    ).toBe('Boya ve fön · kalan');
    expect(
      paymentItemLabel(
        { ...base, isDeposit: true, appointmentStartUtc: '2026-11-25T11:30:00Z' },
        tz,
      ),
    ).toBe('Kapora · 14:30 randevusu');
    expect(
      paymentItemLabel(
        { ...base, purpose: 'CustomerPackage', packageName: 'Fön paketi', packageSessionCount: 5 },
        tz,
      ),
    ).toBe('Fön paketi · 5 seans');
  });
});

describe('Kasa', () => {
  it('menüde; bloklar, tablo ve kasa sayımı (Kasa)', async () => {
    openCash();
    expect(await screen.findByRole('heading', { level: 1, name: 'Kasa' })).toBeTruthy();
    expect(await screen.findByText('Çarşamba, 25 Kasım · Merkez · gün açık')).toBeTruthy();
    const nav = screen.getByRole('navigation', { name: 'Ana menü' });
    expect(within(nav).getByRole('link', { name: 'Kasa' }).getAttribute('aria-current')).toBe(
      'page',
    );
    expect(within(block('Bugün tahsil edilen')).getByText('3.340 ₺')).toBeTruthy();
    expect(within(block('Bugün tahsil edilen')).getByText('7 ödeme')).toBeTruthy();
    expect(within(block('Nakit')).getByText('900 ₺')).toBeTruthy();
    expect(within(block('Kart')).getByText('1.790 ₺')).toBeTruthy();
    expect(within(block('Havale')).getByText('500 ödeme · 150 kapora')).toBeTruthy();

    const table = screen.getByRole('table', { name: 'Günün tahsilatları' });
    const rows = within(table).getAllByRole('row');
    expect(rows).toHaveLength(8);
    expect(rows[1]?.textContent).toBe('12:05Selin ÖztürkBoya ve fön · kalanKartElif1.240 ₺');
    expect(rows[3]?.textContent).toBe('11:35Melis TunçManikür ve pedikürPakettenZeynep0 ₺');
    expect(rows[4]?.textContent).toBe('11:10Gizem ArslanKapora · 14:30 randevusuHavale—150 ₺');
    expect(rows[7]?.textContent).toBe('09:50Hande ErFön paketi · 5 seansHavaleCan500 ₺');

    const count = screen.getByRole('form', { name: 'Kasa sayımı' });
    expect(within(count).getByText('Beklenen').nextElementSibling?.textContent).toBe('900 ₺');
  });

  it('KK7: fark varsa açıklama zorunlu; Günü kapat → kapalı gün özeti', async () => {
    const { calls } = openCash();
    const count = await screen.findByRole('form', { name: 'Kasa sayımı' });
    await userEvent.type(within(count).getByLabelText('Sayılan nakit'), '850');
    expect(within(count).getByText('Fark').nextElementSibling?.textContent).toBe('−50 ₺');
    await userEvent.click(screen.getByRole('button', { name: 'Günü kapat' }));
    expect(await within(count).findByText('Fark var. Nedenini yazın.')).toBeTruthy();
    expect(calls['/api/v1/manage/cash/close']).toBeUndefined();

    await userEvent.type(within(count).getByLabelText('Fark açıklaması'), 'Bozuk para verildi');
    await userEvent.click(screen.getByRole('button', { name: 'Günü kapat' }));
    await waitFor(() => {
      expect(calls['/api/v1/manage/cash/close']).toEqual([
        {
          branchId: owner.branches[0]?.id,
          date: '2026-11-25',
          countedCash: { amountMinor: 85000, currency: 'TRY' },
          differenceNote: 'Bozuk para verildi',
        },
      ]);
    });
    expect(calls['/api/v1/manage/cash/close Idempotency-Key']).toHaveLength(1);
    const closed = await screen.findByRole('region', { name: 'Gün kapatıldı' });
    expect(within(closed).getByText('Fark').nextElementSibling?.textContent).toBe('−50 ₺');
    expect(within(closed).getByText('Fark açıklaması: Bozuk para verildi')).toBeTruthy();
    expect(screen.getByText('Çarşamba, 25 Kasım · Merkez · gün kapatıldı')).toBeTruthy();
    expect(screen.queryByRole('button', { name: 'Günü kapat' })).toBeNull();
  });

  it('fark yoksa açıklamasız kapanır; zaten kapatılmışsa uyarı', async () => {
    const { calls } = openCash('/kasa', { sales: { closeError: 'cash_day_already_closed' } });
    const count = await screen.findByRole('form', { name: 'Kasa sayımı' });
    await userEvent.type(within(count).getByLabelText('Sayılan nakit'), '900');
    expect(within(count).getByText('Fark').nextElementSibling?.textContent).toBe('0 ₺');
    await userEvent.click(screen.getByRole('button', { name: 'Günü kapat' }));
    expect(await screen.findByText('Bu gün zaten kapatıldı. Sayfayı yenileyin.')).toBeTruthy();
    expect(calls['/api/v1/manage/cash/close']).toEqual([
      expect.objectContaining({
        countedCash: { amountMinor: 90000, currency: 'TRY' },
        differenceNote: null,
      }),
    ]);
  });

  it('boş gün (KasaBos): sıfır bloklar, boş durum; Günü kapat sayım kartını açar', async () => {
    openCash('/kasa', { sales: { cashEmpty: true } });
    expect(
      await screen.findByRole('heading', { level: 2, name: 'Bugün henüz tahsilat yok' }),
    ).toBeTruthy();
    expect(within(block('Tahsilat')).getByText('0 ₺')).toBeTruthy();
    expect(within(block('Havale')).getByText('kapora dahil')).toBeTruthy();
    expect(screen.getByRole('button', { name: 'Bugünkü randevular' })).toBeTruthy();
    expect(screen.queryByRole('form', { name: 'Kasa sayımı' })).toBeNull();
    await userEvent.click(screen.getByRole('button', { name: 'Günü kapat' }));
    const count = await screen.findByRole('form', { name: 'Kasa sayımı' });
    expect(document.activeElement).toBe(within(count).getByLabelText('Sayılan nakit'));
  });

  it('Önceki günler: tarih seçilir, adres ?tarih= taşır; Bugüne dön', async () => {
    const { router } = openCash();
    await userEvent.click(await screen.findByRole('button', { name: 'Önceki günler' }));
    const modal = await screen.findByRole('dialog', { name: 'Önceki günler' });
    expect(within(modal).getByLabelText<HTMLInputElement>('Gün').value).toBe('2026-11-24');
    await userEvent.click(within(modal).getByRole('button', { name: 'Günü aç' }));
    expect(await screen.findByText(/^Salı, 24 Kasım · Merkez/)).toBeTruthy();
    expect(router.state.location.search).toEqual({ tarih: '2026-11-24' });
    expect(await screen.findByRole('heading', { name: 'Bu gün tahsilat yok' })).toBeTruthy();
    await userEvent.click(screen.getByRole('button', { name: 'Bugüne dön' }));
    expect(await screen.findByText(/^Çarşamba, 25 Kasım · Merkez/)).toBeTruthy();
  });

  it('kapalı gün: sayım özeti, Günü kapat yok', async () => {
    openCash('/kasa', { sales: { cashClosed: true } });
    const closed = await screen.findByRole('region', { name: 'Gün kapatıldı' });
    expect(within(closed).getByText('Elif Kaya · 19:10')).toBeTruthy();
    expect(screen.queryByRole('button', { name: 'Günü kapat' })).toBeNull();
  });

  it('cash.close yoksa sayım ve Günü kapat yok; personel kasayı görmez', async () => {
    openCash('/kasa', {
      session: sessionFixture({
        brand: owner.brand,
        permissions: ownerPermissions.filter((p) => p !== 'cash.close'),
      }),
    });
    expect(await screen.findByRole('table', { name: 'Günün tahsilatları' })).toBeTruthy();
    expect(screen.queryByRole('form', { name: 'Kasa sayımı' })).toBeNull();
    expect(screen.queryByRole('button', { name: 'Günü kapat' })).toBeNull();
    cleanup();

    renderApp('/kasa', {
      session: sessionFixture({ brand: owner.brand, permissions: staffPermissions }),
    });
    expect(await screen.findByRole('heading', { name: /yetkiniz yok/i })).toBeTruthy();
    expect(screen.queryByRole('link', { name: 'Kasa' })).toBeNull();
  });
});
