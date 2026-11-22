import { cleanup, fireEvent, screen, waitFor, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { ownerPermissions, sessionFixture, staffPermissions } from '../../test/fake-api';
import type { FakeApiOptions } from '../../test/fake-api';
import { creditPackagesFixture } from '../../test/fake-subscription';
import type { SubscriptionFakeOptions } from '../../test/fake-subscription';
import { renderApp } from '../../test/render-app';
import { branchDetail, invoicePeriod, planDetail, staffDetail } from './labels';

// Kural: dilim-06 KK1 ve KK2 (salon tarafı): Ayarlar → Abonelik ve fatura, havale bildirimi, kontör satın alma (K1:
// yalnız havale), abonelik şeritleri; roller-yetkiler.md (subscription.manage yalnız Owner); S-14 (fiyat ve OMVILA
// hesabı yoksa uydurma değer veya köşeli parantez yok); T-06 (kart yok); T-125. Saat: 25 Kasım 2026, 12:00 İstanbul.
afterEach(() => {
  cleanup();
  vi.restoreAllMocks();
});

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

const managerPermissions = ownerPermissions.filter((p) => p !== 'subscription.manage');

function open(
  path: string,
  subscription: SubscriptionFakeOptions = {},
  options: FakeApiOptions = {},
) {
  const notices: Record<string, string>[] = [];
  const result = renderApp(path, { subscription: { notices, ...subscription }, ...options });
  return { ...result, notices };
}

async function openPage(subscription: SubscriptionFakeOptions = {}, options: FakeApiOptions = {}) {
  const result = open('/ayarlar/abonelik', subscription, options);
  await screen.findByRole('heading', { level: 1, name: 'Ayarlar' });
  await screen.findByText('ABN-KE-1125');
  return result;
}

describe('etiketler', () => {
  it('plan, şube, personel ve fatura dönemi', () => {
    expect(planDetail({ billingCycle: 'Monthly', status: 'Trialing' })).toBe('aylık · deneme');
    expect(branchDetail({ count: 2, included: 1, extra: 1 })).toBe('1 dahil + 1 ek şube');
    expect(branchDetail({ count: 1, included: 1, extra: 0 })).toBe('1 dahil');
    expect(branchDetail({ count: 3, included: null, extra: 0 })).toBe('sınırsız şube');
    expect(staffDetail({ count: 5, limit: null })).toBeUndefined();
    expect(staffDetail({ count: 5, limit: 8 })).toBe('8 kişiye kadar dahil');
    expect(invoicePeriod({ periodStart: '2026-10-01', periodEnd: '2026-10-31' })).toBe('Ekim 2026');
    expect(invoicePeriod({ periodStart: '2026-12-04', periodEnd: '2027-01-03' })).toBe(
      '4 Ara – 3 Oca 2027',
    );
  });
});

describe('Abonelik ve fatura', () => {
  it('kartlar, deneme şeridi, faturalar; tanımsız hesap ve fiyat uydurulmaz', async () => {
    await openPage();
    expect(screen.getByRole('link', { name: 'Abonelik ve fatura' })).toBeTruthy();
    expect(screen.getByRole('status').textContent).toContain(
      'Deneme sürenizin bitmesine 9 gün var.',
    );
    expect(screen.getByText('Salon')).toBeTruthy();
    expect(screen.getByText('aylık · deneme')).toBeTruthy();
    expect(screen.getByText('1 dahil + 1 ek şube')).toBeTruthy();

    const invoices = screen.getByRole('region', { name: 'Faturalar' });
    expect(within(invoices).getByText('e-Arşiv değil, bilgi amaçlı')).toBeTruthy();
    const rows = await within(invoices).findAllByRole('row');
    expect(rows).toHaveLength(3);
    expect(rows[1]?.textContent).toContain('OMV-2026-000184');
    expect(rows[1]?.textContent).toContain('Ekim 2026');
    expect(rows[1]?.textContent).toMatch(/1\.200\s₺/);
    expect(rows[1]?.textContent).toContain('Ödendi');

    const transfer = screen.getByRole('region', { name: 'Havale / EFT ile öde' });
    expect(within(transfer).getByText(/OMVILA hesap bilgisi henüz tanımlı değil/)).toBeTruthy();
    expect(within(transfer).getByText('Fiyat henüz tanımlı değil')).toBeTruthy();
    expect(within(transfer).queryByText('IBAN')).toBeNull();
    expect(within(transfer).queryByRole('button', { name: /Havaleyi bildir/ })).toBeNull();
    // Kart akışı yok (T-06): "Kartla öde", "Kart ekle", "Ödeme yöntemini kaydet" gösterilmez.
    expect(screen.queryByText(/Kartla öde|Kart ekle|Ödeme yöntemini kaydet|yakında/i)).toBeNull();
    expect(document.body.textContent).not.toMatch(/\[[A-ZÇĞİÖŞÜ ]+\]/);
  });

  it('tanımlı hesap: alıcı, IBAN, kod ve tutar kopyalanır', async () => {
    const writeText = vi.fn(() => Promise.resolve());
    Object.defineProperty(navigator, 'clipboard', { value: { writeText }, configurable: true });
    await openPage({ configured: true });
    const transfer = screen.getByRole('region', { name: 'Havale / EFT ile öde' });
    expect(within(transfer).getByText('Örnek Yazılım A.Ş.')).toBeTruthy();
    expect(within(transfer).getByText('TR00 0000 0000 0000 0000 0000 01')).toBeTruthy();
    expect(within(transfer).getByText('1.200 ₺')).toBeTruthy();
    await userEvent.click(within(transfer).getByRole('button', { name: 'IBAN kopyala' }));
    expect(writeText).toHaveBeenCalledWith('TR000000000000000000000001');
    expect(await within(transfer).findByRole('button', { name: 'IBAN kopyalandı' })).toBeTruthy();
    await userEvent.click(within(transfer).getByRole('button', { name: 'Açıklama kopyala' }));
    expect(writeText).toHaveBeenCalledWith('ABN-KE-1125');
  });

  it('havaleyi bildir: gönderen adı ve dekont multipart gider', async () => {
    const { notices } = await openPage({ configured: true });
    await userEvent.click(
      screen.getByRole('button', { name: 'Havaleyi bildir (dekont isteğe bağlı)' }),
    );
    const dialog = screen.getByRole('dialog', { name: 'Havaleyi bildir' });
    await userEvent.type(within(dialog).getByLabelText('Gönderen adı (isteğe bağlı)'), 'Elif Kaya');
    await userEvent.upload(
      within(dialog).getByLabelText('Dekont dosyası'),
      new File(['png'], 'dekont.png', { type: 'image/png' }),
    );
    expect(within(dialog).getByText('dekont.png')).toBeTruthy();
    await userEvent.click(within(dialog).getByRole('button', { name: 'Bildir' }));
    expect(await screen.findByText('Havale bildiriminiz alındı')).toBeTruthy();
    expect(notices).toEqual([
      { purpose: 'Subscription', senderName: 'Elif Kaya', receipt: 'file' },
    ]);
    expect(screen.queryByRole('dialog')).toBeNull();
  });

  it('dekont 5 MB üstü veya biçim dışıysa gönderilmez', async () => {
    const { notices } = await openPage({ configured: true });
    await userEvent.click(
      screen.getByRole('button', { name: 'Havaleyi bildir (dekont isteğe bağlı)' }),
    );
    const dialog = screen.getByRole('dialog', { name: 'Havaleyi bildir' });
    const input = within(dialog).getByLabelText('Dekont dosyası');
    fireEvent.change(input, {
      target: {
        files: [
          new File([new Uint8Array(5 * 1024 * 1024 + 1)], 'buyuk.pdf', { type: 'application/pdf' }),
        ],
      },
    });
    expect(within(dialog).getByRole('alert').textContent).toBe('Dekont en fazla 5 MB olabilir.');
    await userEvent.click(within(dialog).getByRole('button', { name: 'Bildir' }));
    fireEvent.change(input, {
      target: { files: [new File(['x'], 'not.txt', { type: 'text/plain' })] },
    });
    expect(within(dialog).getByRole('alert').textContent).toBe('Dekont PDF, JPG veya PNG olmalı.');
    expect(notices).toEqual([]);
  });

  it('sunucu dekontu reddederse alan hatası; bekleyen bildirim varsa uyarı', async () => {
    await openPage({ configured: true, notifyError: 'receipt_too_large' });
    await userEvent.click(
      screen.getByRole('button', { name: 'Havaleyi bildir (dekont isteğe bağlı)' }),
    );
    let dialog = screen.getByRole('dialog', { name: 'Havaleyi bildir' });
    await userEvent.click(within(dialog).getByRole('button', { name: 'Bildir' }));
    expect((await within(dialog).findByRole('alert')).textContent).toBe(
      'Dekont en fazla 5 MB olabilir.',
    );
    cleanup();

    await openPage({ configured: true, notifyError: 'payment_notice_already_pending' });
    await userEvent.click(
      screen.getByRole('button', { name: 'Havaleyi bildir (dekont isteğe bağlı)' }),
    );
    dialog = screen.getByRole('dialog', { name: 'Havaleyi bildir' });
    await userEvent.click(within(dialog).getByRole('button', { name: 'Bildir' }));
    expect(
      await within(dialog).findByText('Bu ödeme için bildiriminiz zaten inceleniyor.'),
    ).toBeTruthy();
  });

  it('bekleyen bildirim: durum yazar, yeni bildirim butonu yok', async () => {
    await openPage({
      configured: true,
      detail: {
        pendingNotice: {
          id: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f9b01',
          purpose: 'Subscription',
          transferCode: 'ABN-KE-1125',
          expectedAmount: null,
          credits: null,
          senderName: null,
          hasReceipt: false,
          status: 'Pending',
          notifiedUtc: '2026-11-24T08:00:00Z',
        },
      },
    });
    const transfer = screen.getByRole('region', { name: 'Havale / EFT ile öde' });
    expect(within(transfer).getByText('Eşleştirme bekliyor')).toBeTruthy();
    expect(within(transfer).getByText(/24 Kasım tarihinde bildirildi/)).toBeTruthy();
    expect(within(transfer).queryByRole('button', { name: /Havaleyi bildir/ })).toBeNull();
  });

  it('fatura PDF indirilir; fatura yoksa boş hâl', async () => {
    await openPage();
    await userEvent.click(await screen.findByRole('button', { name: 'OMV-2026-000184 PDF indir' }));
    await waitFor(() => {
      expect(downloads).toEqual(['OMV-2026-000184.pdf']);
    });
    cleanup();

    open('/ayarlar/abonelik', { noInvoices: true });
    expect(await screen.findByRole('heading', { name: 'Henüz fatura yok' })).toBeTruthy();
  });

  it('yüklenemezse hata ve tekrar deneme', async () => {
    open('/ayarlar/abonelik', { fails: true });
    expect(
      await screen.findByRole('heading', { name: 'Abonelik bilgisi yüklenemedi' }),
    ).toBeTruthy();
    expect(screen.getByRole('button', { name: 'Tekrar dene' })).toBeTruthy();
  });

  it('subscription.manage yoksa menüde yok ve adres 403', async () => {
    open('/ayarlar/abonelik', {}, { session: sessionFixture({ permissions: managerPermissions }) });
    expect(
      await screen.findByRole('heading', { name: 'Bu sayfayı görme yetkiniz yok' }),
    ).toBeTruthy();
    cleanup();
    open('/ayarlar/isletme', {}, { session: sessionFixture({ permissions: managerPermissions }) });
    await screen.findByRole('heading', { level: 1, name: 'Ayarlar' });
    expect(screen.queryByRole('link', { name: 'Abonelik ve fatura' })).toBeNull();
  });
});

describe('Kontör al (K1: yalnız havale)', () => {
  it('paket seç → KNT- havale bilgisi → havaleyi bildir (purpose Credits)', async () => {
    const { notices } = open('/ayarlar/bildirimler', { configured: true });
    await userEvent.click(await screen.findByRole('button', { name: 'Kontör al' }));
    const dialog = screen.getByRole('dialog', { name: 'Kontör al' });
    expect(await within(dialog).findByText('1.000 SMS')).toBeTruthy();
    expect(within(dialog).getByText('250 ₺')).toBeTruthy();
    await userEvent.click(within(dialog).getByRole('radio', { name: /Orta/ }));
    await userEvent.click(within(dialog).getByRole('button', { name: 'Havale bilgisini göster' }));
    expect(await within(dialog).findByText('KNT-KE-1125')).toBeTruthy();
    expect(within(dialog).getByText('5.000 SMS kontörü · havale ile')).toBeTruthy();
    expect(within(dialog).getByText('1.000 ₺')).toBeTruthy();
    await userEvent.click(within(dialog).getByRole('button', { name: 'Havaleyi bildir' }));
    await userEvent.click(within(dialog).getByRole('button', { name: 'Bildir' }));
    expect(await screen.findByText('Havale bildiriminiz alındı')).toBeTruthy();
    expect(notices).toEqual([
      { purpose: 'Credits', creditPackageId: creditPackagesFixture[1]?.id },
    ]);
  });

  it('fiyat ve hesap tanımlı değilse uydurulmaz; bildirim kapalı', async () => {
    open('/ayarlar/bildirimler');
    await userEvent.click(await screen.findByRole('button', { name: 'Kontör al' }));
    const dialog = screen.getByRole('dialog', { name: 'Kontör al' });
    expect(await within(dialog).findAllByText('Fiyat henüz tanımlı değil')).toHaveLength(3);
    await userEvent.click(within(dialog).getByRole('button', { name: 'Havale bilgisini göster' }));
    expect(
      await within(dialog).findByText(/OMVILA hesap bilgisi henüz tanımlı değil/),
    ).toBeTruthy();
    expect(
      within(dialog).getByRole<HTMLButtonElement>('button', { name: 'Havaleyi bildir' }).disabled,
    ).toBe(true);
  });

  it('kontör alamayan yöneticide üst barda "Kontör al" yok', async () => {
    open(
      '/ayarlar/bildirimler',
      {},
      { session: sessionFixture({ permissions: managerPermissions }) },
    );
    await screen.findByRole('region', { name: 'SMS kontörü' });
    expect(screen.queryByRole('button', { name: 'Kontör al' })).toBeNull();
  });
});

describe('abonelik şeritleri', () => {
  const pastDue = {
    status: 'PastDue' as const,
    trialEndsOn: null,
    pastDueSince: '2026-11-23',
    readOnlyOn: '2026-11-30',
  };

  it('ödeme gecikti: gün sayısı sunucu tarihinden; "Havale ile öde" abonelik sayfasını açar', async () => {
    const { router } = open(
      '/hizmetler',
      {},
      { session: sessionFixture({ subscription: pastDue }) },
    );
    const banner = await screen.findByRole('status');
    expect(banner.textContent).toContain('Abonelik ödemesi alınamadı · 3. gün');
    expect(banner.textContent).toContain(
      '5 gün içinde havale ile ödemezseniz panel salt okunur olur ve online randevu kapanır.',
    );
    await userEvent.click(within(banner).getByRole('button', { name: 'Havale ile öde' }));
    await waitFor(() => {
      expect(router.state.location.pathname).toBe('/ayarlar/abonelik');
    });
    expect(await screen.findByText('ABN-KE-1125')).toBeTruthy();
  });

  it('ödeme gecikti: ödeme izni olmayan yöneticide buton yok, personelde şerit yok', async () => {
    open(
      '/hizmetler',
      {},
      { session: sessionFixture({ subscription: pastDue, permissions: managerPermissions }) },
    );
    const banner = await screen.findByRole('status');
    expect(within(banner).queryByRole('button')).toBeNull();
    cleanup();

    open(
      '/takvim',
      {},
      { session: sessionFixture({ subscription: pastDue, permissions: staffPermissions }) },
    );
    await screen.findByRole('heading', { level: 1 });
    expect(screen.queryByText(/Abonelik ödemesi alınamadı/)).toBeNull();
  });

  it('salt okunur: herkese kırmızı şerit; "Ödemeyi tamamla" yalnız sahipte; yeni randevu pasif', async () => {
    const readOnly = {
      status: 'ReadOnly' as const,
      trialEndsOn: null,
      pastDueSince: '2026-11-18',
      readOnlyOn: null,
    };
    open('/takvim', {}, { session: sessionFixture({ subscription: readOnly }) });
    const banner = await screen.findByRole('alert');
    expect(banner.textContent).toContain('Panel salt okunur');
    expect(banner.textContent).toContain('Abonelik ödemesi 7 gündür alınamadı.');
    expect(within(banner).getByRole('button', { name: 'Ödemeyi tamamla' })).toBeTruthy();
    expect(screen.getByRole<HTMLButtonElement>('button', { name: 'Randevu ekle' }).disabled).toBe(
      true,
    );
    cleanup();

    open(
      '/takvim',
      {},
      { session: sessionFixture({ subscription: readOnly, permissions: staffPermissions }) },
    );
    const staffBanner = await screen.findByRole('alert');
    expect(staffBanner.textContent).toContain('Panel salt okunur');
    expect(within(staffBanner).queryByRole('button')).toBeNull();
  });

  it('iptal edilmiş abonelikte de yeni randevu pasif', async () => {
    open(
      '/takvim',
      {},
      {
        session: sessionFixture({ subscription: { status: 'Cancelled', trialEndsOn: null } }),
      },
    );
    expect((await screen.findByRole('alert')).textContent).toContain('Panel salt okunur');
    expect(screen.getByRole<HTMLButtonElement>('button', { name: 'Randevu ekle' }).disabled).toBe(
      true,
    );
  });
});
