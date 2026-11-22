import { cleanup, screen, waitFor, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { afterEach, describe, expect, it } from 'vitest';
import { branchId, ownerPermissions, sessionFixture, staffPermissions } from '../../test/fake-api';
import type { FakeApiOptions } from '../../test/fake-api';
import { customerIds } from '../../test/fake-customers';
import { renderApp } from '../../test/render-app';
import { addDays, contentsLabel, untilLabel, validityLabel } from './labels';

// Kural: dilim-05 KK6 (paket tanımları, satış, iade), sözleşme manageListPackages / manageCreatePackage /
// manageUpdatePackage (If-Match) / manageListAllCustomerPackages / manageSellPackage (Idempotency-Key) /
// manageRefundCustomerPackage (If-Match + Idempotency-Key; tutar ≤ ödenen, S-10/W9); roller-yetkiler.md (Paketler:
// package.manage veya package.sell; tanım yazma yalnız sahip). Saat: Çarşamba 25 Kasım 2026, 12:00 İstanbul.
afterEach(() => {
  cleanup();
  localStorage.clear();
});

const elif = '0193a8f0-0000-7000-8000-0000000000c1';
const owner = sessionFixture({ brand: { ...sessionFixture().brand, status: 'Active' } });
const manager = sessionFixture({
  brand: { ...sessionFixture().brand, status: 'Active' },
  membership: { role: 'BranchManager', branchIds: [branchId], staffId: elif },
  permissions: ownerPermissions.filter((p) => p !== 'package.manage'),
});

function open(path = '/paketler', options: FakeApiOptions = {}) {
  const calls: Record<string, unknown[]> = {};
  const rendered = renderApp(path, { calls, session: owner, withServices: true, ...options });
  return { calls, ...rendered };
}

const block = (label: string) => {
  const summary = screen.getByRole('group', { name: 'Paket özeti' });
  const el = within(summary).getByText(label).parentElement;
  if (!el) {
    throw new Error('Blok yok');
  }

  return el;
};

describe('paket metinleri', () => {
  it('geçerlilik, son gün eki, içerik', () => {
    expect(validityLabel(90)).toBe('3 ay');
    expect(validityLabel(365)).toBe('12 ay');
    expect(validityLabel(45)).toBe('45 gün');
    expect(addDays('2026-11-25', 90)).toBe('2027-02-23');
    expect(untilLabel('2027-02-20')).toBe("20 Şubat 2027'ye kadar");
    expect(untilLabel('2030-01-01')).toBe("1 Ocak 2030'a kadar");
    expect(untilLabel('2100-03-05')).toBe("5 Mart 2100'e kadar");
    expect(
      contentsLabel([
        { serviceId: 'a', serviceName: 'Manikür', sessionCount: 6 },
        { serviceId: 'b', serviceName: 'Kalıcı oje', sessionCount: 2 },
      ]),
    ).toBe('6 × Manikür, 2 × Kalıcı oje');
  });
});

describe('Paketler', () => {
  it('menüde; üst kartlar, tanımlar, süresi yaklaşanlar (Paketler)', async () => {
    open();
    expect(await screen.findByRole('heading', { level: 1, name: 'Paketler' })).toBeTruthy();
    expect(await screen.findByText('6 paket tanımı · Merkez')).toBeTruthy();
    const nav = screen.getByRole('navigation', { name: 'Ana menü' });
    expect(within(nav).getByRole('link', { name: 'Paketler' }).getAttribute('aria-current')).toBe(
      'page',
    );
    expect(within(block('Aktif müşteri paketi')).getByText('46')).toBeTruthy();
    expect(within(block('Aktif müşteri paketi')).getByText('kalan 182 seans')).toBeTruthy();
    expect(within(block('Bu ay satılan')).getByText('18.600 ₺')).toBeTruthy();
    expect(within(block('Süresi yaklaşan')).getByText('30 gün içinde bitiyor')).toBeTruthy();
    expect(within(block('Kullanım oranı')).getByText('%81')).toBeTruthy();
    const table = screen.getByRole('table', { name: 'Paket tanımları' });
    const fon = within(table).getByText('Fön paketi').closest('tr');
    expect(fon?.textContent).toContain('1.000 ₺');
    expect(fon?.textContent).toContain('3 ay');
    expect(fon?.textContent).toContain('Satışta');
    expect(within(table).getByText('Satışta değil')).toBeTruthy();
    const expiring = screen.getByRole('region', { name: 'Süresi yaklaşanlar' });
    const names = await within(expiring).findAllByRole('listitem');
    expect(names[0]?.textContent).toContain('Melis Tunç');
    expect(names[0]?.textContent).toContain('8 Ara');
    expect(screen.queryByText('Hepsine SMS')).toBeNull();
  });

  it('paket yoksa boş hâl ve oluştur (PaketlerBos)', async () => {
    open('/paketler', { packages: { empty: true } });
    expect(await screen.findByRole('heading', { name: 'Henüz paket tanımı yok' })).toBeTruthy();
    expect(screen.queryByRole('button', { name: 'Müşteriye paket sat' })).toBeNull();
    const buttons = screen.getAllByRole('button', { name: 'Paket oluştur' });
    expect(buttons).toHaveLength(2);
  });

  it('paket oluştur: doğrulama, çok hizmetli içerik, gövde (PaketOlustur)', async () => {
    const { calls } = open();
    await userEvent.click(await screen.findByRole('button', { name: 'Paket oluştur' }));
    const dialog = await screen.findByRole('dialog', { name: 'Paket oluştur' });
    await userEvent.click(within(dialog).getByRole('button', { name: 'Paketi oluştur' }));
    expect(within(dialog).getByText('Paket adı 2–80 karakter olmalı.')).toBeTruthy();
    expect(within(dialog).getByText('Fiyatı yazın: 1.700')).toBeTruthy();
    expect(within(dialog).getByRole('alert').textContent).toContain('Her satırda bir hizmet');

    await userEvent.type(within(dialog).getByLabelText('Paket adı'), 'Kesim ve keratin');
    await userEvent.type(within(dialog).getByLabelText('Fiyat'), '2.400');
    await userEvent.selectOptions(within(dialog).getByLabelText('1. hizmet'), 'Saç kesimi');
    await userEvent.type(within(dialog).getByLabelText('Saç kesimi seans sayısı'), '4');
    await userEvent.click(within(dialog).getByRole('button', { name: 'Hizmet ekle' }));
    const second = within(dialog).getByLabelText('2. hizmet');
    // Seçilmiş hizmet ikinci satırda yok (aynı hizmet iki kez yazılmaz).
    expect(within(second).queryByRole('option', { name: 'Saç kesimi' })).toBeNull();
    await userEvent.selectOptions(second, 'Keratin');
    await userEvent.type(within(dialog).getByLabelText('Keratin seans sayısı'), '2');
    await userEvent.selectOptions(within(dialog).getByLabelText('Geçerlilik'), '6 ay');
    await userEvent.type(within(dialog).getByLabelText('Satış primi'), '10');
    await userEvent.click(within(dialog).getByRole('button', { name: 'Paketi oluştur' }));

    await waitFor(() => {
      expect(calls['/api/v1/manage/packages']).toEqual([
        {
          name: 'Kesim ve keratin',
          price: { amountMinor: 240000, currency: 'TRY' },
          items: [
            { serviceId: '0193a8f0-0000-7000-8000-0000000000f1', sessionCount: 4 },
            { serviceId: '0193a8f0-0000-7000-8000-0000000000f2', sessionCount: 2 },
          ],
          validityDays: 180,
          saleCommissionPercent: 10,
          isAvailableAllBranches: true,
          isActive: true,
        },
      ]);
    });
    expect(calls['/api/v1/manage/packages Idempotency-Key']).toHaveLength(1);
    expect(await screen.findByText('Kesim ve keratin')).toBeTruthy();
    expect(screen.queryByRole('dialog', { name: 'Paket oluştur' })).toBeNull();
  });

  it('satır menüsü: düzenle If-Match ile, satıştan kaldır', async () => {
    const { calls } = open();
    await userEvent.click(await screen.findByRole('button', { name: 'Fön paketi işlemleri' }));
    await userEvent.click(screen.getByRole('menuitem', { name: 'Düzenle' }));
    const dialog = await screen.findByRole('dialog', { name: 'Paketi düzenle' });
    expect(within(dialog).getByLabelText<HTMLInputElement>('Paket adı').value).toBe('Fön paketi');
    const price = within(dialog).getByLabelText('Fiyat');
    await userEvent.clear(price);
    await userEvent.type(price, '1.100');
    await userEvent.click(within(dialog).getByRole('button', { name: 'Değişiklikleri kaydet' }));
    const path = '/api/v1/manage/packages/0193a8f0-0000-7000-8000-0000000e0001';
    await waitFor(() => {
      expect(calls[`PUT ${path} If-Match`]).toEqual(['1']);
    });
    expect(calls[path]?.[0]).toMatchObject({ price: { amountMinor: 110000 }, isActive: true });

    await userEvent.click(await screen.findByRole('button', { name: 'Fön paketi · 10 işlemleri' }));
    await userEvent.click(screen.getByRole('menuitem', { name: 'Satıştan kaldır' }));
    const second = '/api/v1/manage/packages/0193a8f0-0000-7000-8000-0000000e0002';
    await waitFor(() => {
      expect(calls[second]?.[0]).toMatchObject({ name: 'Fön paketi · 10', isActive: false });
    });
    expect(await screen.findByText('Paket satıştan kaldırıldı')).toBeTruthy();
  });

  it('müşteriye paket sat: müşteri ara, paket ve yöntem, özet, gövde (PaketSat)', async () => {
    const { calls } = open();
    await userEvent.click(await screen.findByRole('button', { name: 'Müşteriye paket sat' }));
    const dialog = await screen.findByRole('dialog', { name: 'Müşteriye paket sat' });
    expect(within(dialog).queryByRole('button', { name: 'Yeni müşteri ekle' })).toBeNull();
    await userEvent.type(within(dialog).getByRole('searchbox', { name: 'Müşteri ara' }), 'Selin');
    await userEvent.click(await within(dialog).findByRole('button', { name: /Selin Öztürk/ }));
    await userEvent.selectOptions(await within(dialog).findByLabelText('Paket'), 'Saç kesimi');
    // Satışta olmayan paket listede yok.
    expect(within(dialog).queryByRole('option', { name: 'Keratin bakım' })).toBeNull();
    expect(within(dialog).getByLabelText<HTMLSelectElement>('Satan personel').value).toBe(elif);
    expect(within(dialog).getByText('6 × Saç kesimi')).toBeTruthy();
    expect(within(dialog).getByText("23 Şubat 2027'ye kadar")).toBeTruthy();
    await userEvent.click(within(dialog).getByRole('radio', { name: 'Nakit' }));
    await userEvent.click(
      within(dialog).getByRole('button', { name: '1.700 ₺ al ve paketi tanımla' }),
    );
    const path = `/api/v1/manage/customers/${customerIds.selin}/packages`;
    await waitFor(() => {
      expect(calls[path]).toEqual([
        {
          packageId: '0193a8f0-0000-7000-8000-0000000e0005',
          branchId,
          soldByStaffId: elif,
          paymentMethod: 'Cash',
        },
      ]);
    });
    expect(calls[`${path} Idempotency-Key`]).toHaveLength(1);
    expect(await screen.findByText('Paket tanımlandı')).toBeTruthy();
  });

  it('satan personel şubede değilse alan hatası', async () => {
    open(`/musteriler/${customerIds.selin}`, { packages: { sellError: 'soldByStaffId' } });
    await userEvent.click(await screen.findByRole('button', { name: 'Paket sat' }));
    const dialog = await screen.findByRole('dialog', { name: 'Müşteriye paket sat' });
    await userEvent.click(
      await within(dialog).findByRole('button', { name: '1.000 ₺ al ve paketi tanımla' }),
    );
    expect(
      await within(dialog).findByText('Bu personel bu şubede çalışmıyor. Başka personel seçin.'),
    ).toBeTruthy();
  });

  it('müşteri kartından sat: Paketler sekmesinde ve paket kartında görünür', async () => {
    open(`/musteriler/${customerIds.selin}`);
    await userEvent.click(await screen.findByRole('button', { name: 'Paket sat' }));
    const dialog = await screen.findByRole('dialog', { name: 'Müşteriye paket sat' });
    expect(within(dialog).getByText('Selin Öztürk')).toBeTruthy();
    await userEvent.click(
      await within(dialog).findByRole('button', { name: '1.000 ₺ al ve paketi tanımla' }),
    );
    await waitFor(() => {
      expect(screen.queryByRole('dialog', { name: 'Müşteriye paket sat' })).toBeNull();
    });
    expect(await screen.findByText('seans kaldı · 23 Şub')).toBeTruthy();
    await userEvent.click(screen.getByRole('tab', { name: 'Paketler' }));
    const table = await screen.findByRole('table', { name: 'Paketler' });
    expect(within(table).getByText('5/5 seans')).toBeTruthy();
  });

  it('müşteri paketleri: süzgeç, iade (tutar ≤ ödenen)', async () => {
    const { calls } = open();
    await userEvent.click(await screen.findByRole('tab', { name: /^Müşteri paketleri/ }));
    const table = await screen.findByRole('table', { name: 'Müşteri paketleri' });
    expect(within(table).getByText('Hande Er')).toBeTruthy();
    await userEvent.click(screen.getByRole('radio', { name: 'Süresi dolan' }));
    expect(await screen.findByRole('heading', { name: 'Bu süzgeçte paket yok' })).toBeTruthy();
    await userEvent.click(screen.getByRole('radio', { name: 'Etkin' }));

    await userEvent.click(
      await screen.findByRole('button', { name: 'Hande Er · Cilt bakımı işlemleri' }),
    );
    await userEvent.click(screen.getByRole('menuitem', { name: 'İade et' }));
    const dialog = await screen.findByRole('dialog', { name: 'Paketi iade et' });
    const amount = within(dialog).getByLabelText('İade edilen tutar');
    expect((amount as HTMLInputElement).value).toBe('1.000');
    await userEvent.clear(amount);
    await userEvent.type(amount, '1.200');
    await userEvent.click(within(dialog).getByRole('button', { name: 'İadeyi kaydet' }));
    expect(within(dialog).getByText("Tutar 0'dan büyük ve en fazla 1.000 ₺ olmalı.")).toBeTruthy();
    await userEvent.clear(amount);
    await userEvent.type(amount, '600');
    await userEvent.click(within(dialog).getByRole('button', { name: 'İadeyi kaydet' }));
    const path = '/api/v1/manage/customer-packages/0193a8f0-0000-7000-8000-0000000e0102/refund';
    await waitFor(() => {
      expect(calls[path]).toEqual([{ amount: { amountMinor: 60000, currency: 'TRY' } }]);
    });
    expect(calls[`POST ${path} If-Match`]).toEqual(['1']);
    expect(calls[`${path} Idempotency-Key`]).toHaveLength(1);
    expect(await screen.findByText('Paket iadesi işaretlendi')).toBeTruthy();
  });

  it('şube müdürü satar ama tanım yazmaz', async () => {
    open('/paketler', { session: manager });
    expect(await screen.findByRole('table', { name: 'Paket tanımları' })).toBeTruthy();
    expect(screen.queryByRole('button', { name: 'Paket oluştur' })).toBeNull();
    expect(screen.queryByRole('button', { name: 'Fön paketi işlemleri' })).toBeNull();
    expect(screen.getByRole('button', { name: 'Müşteriye paket sat' })).toBeTruthy();
  });

  it('personel menüde görmez, adresten 403', async () => {
    open('/paketler', {
      session: sessionFixture({
        membership: { role: 'Staff', branchIds: [], staffId: elif },
        permissions: staffPermissions,
      }),
    });
    expect(
      await screen.findByRole('heading', { name: 'Bu sayfayı görme yetkiniz yok' }),
    ).toBeTruthy();
    expect(screen.queryByRole('link', { name: 'Paketler' })).toBeNull();
  });
});
