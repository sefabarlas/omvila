import { cleanup, screen, waitFor, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { ownerPermissions, sessionFixture, staffPermissions } from '../../test/fake-api';
import type { FakeApiOptions } from '../../test/fake-api';
import { customerIds, importId } from '../../test/fake-customers';
import { renderApp } from '../../test/render-app';
import { errorRowsCsv, errorRowsFileName, invalidRowsSummary } from './import-view';
import { paymentItemLabel } from './labels';
import type { PaymentView } from './api';

// Kural: dilim-05 KK9 (salon tarafı: müşteri kartı sekmeleri, birleştirme, CSV içe/dışa aktarma, KVKK silme, silme
// talepleri); roller-yetkiler.md (customer.merge: sahip + şube müdürü; customer.import/export/kvkk: yalnız sahip;
// payment.collect, review.view personelde yok); sözleşme T-86 (7). Saat: Çarşamba 25 Kasım 2026, 12:00 İstanbul.
afterEach(() => {
  cleanup();
  vi.restoreAllMocks();
});

const downloads: { name: string; blob: Blob }[] = [];
beforeEach(() => {
  downloads.length = 0;
  let last: Blob | null = null;
  URL.createObjectURL = (blob: Blob) => {
    last = blob;
    return 'blob:test';
  };
  URL.revokeObjectURL = () => undefined;
  vi.spyOn(HTMLAnchorElement.prototype, 'click').mockImplementation(function (
    this: HTMLAnchorElement,
  ) {
    if (last) {
      downloads.push({ name: this.download, blob: last });
    }
  });
});

function open(path: string, options: FakeApiOptions = {}) {
  const calls: Record<string, unknown[]> = {};
  const result = renderApp(path, { calls, ...options });
  return { ...result, calls };
}

const selinCard = `/musteriler/${customerIds.selin}`;
/** Selin'in etkin paketi fake-sales'ten (Kesim paketi, 3/5, son gün 31 Aralık). */
const withPackage: FakeApiOptions = { sales: { packageCustomerId: customerIds.selin } };
const managerPermissions = ownerPermissions.filter(
  (p) => !['customer.import', 'customer.export', 'customer.kvkk'].includes(p),
);
const staff = sessionFixture({
  user: { id: '0193a8f0-0000-7000-8000-000000000002', fullName: 'Mert Demir', phoneMasked: '' },
  membership: { role: 'Staff', branchIds: [], staffId: '0193a8f0-0000-7000-8000-0000000000c2' },
  permissions: staffPermissions,
});

describe('müşteri kartı sekmeleri', () => {
  it('etkin paket kartı ve dört sekme', async () => {
    open(selinCard, withPackage);
    expect(await screen.findByText('3/5')).toBeTruthy();
    expect(screen.getByText('seans kaldı · 31 Ara')).toBeTruthy();
    expect(screen.getByText('Kesim paketi')).toBeTruthy();
    const tabs = screen.getByRole('tablist', { name: 'Müşteri kartı' });
    expect(
      within(tabs)
        .getAllByRole('tab')
        .map((t) => t.textContent),
    ).toEqual(['Randevular', 'Paketler', 'Ödemeler', 'Değerlendirmeler']);
  });

  it('Paketler: kalan seans, son gün, durum', async () => {
    open(selinCard, withPackage);
    await userEvent.click(await screen.findByRole('tab', { name: 'Paketler' }));
    const table = await screen.findByRole('table', { name: 'Paketler' });
    expect(within(table).getByText('3/5 seans')).toBeTruthy();
    expect(within(table).getByText('Saç kesimi 3/5')).toBeTruthy();
    expect(within(table).getByText('31 Ara')).toBeTruthy();
    expect(within(table).getByText('Etkin')).toBeTruthy();
    expect(within(table).getByText('1.700 ₺')).toBeTruthy();
  });

  it('paketi olmayan müşteride paket kartı yok, sekmede boş hâl', async () => {
    open(selinCard);
    await userEvent.click(await screen.findByRole('tab', { name: 'Paketler' }));
    expect(await screen.findByRole('heading', { name: 'Paketi yok' })).toBeTruthy();
    expect(screen.queryByText(/seans kaldı/)).toBeNull();
  });

  it('Ödemeler: kalem metni, yöntem, iade işareti', async () => {
    open(selinCard);
    await userEvent.click(await screen.findByRole('tab', { name: 'Ödemeler' }));
    const table = await screen.findByRole('table', { name: 'Ödemeler' });
    expect(within(table).getByText('Boya, Fön · kalan')).toBeTruthy();
    expect(within(table).getByText('Kapora · 10:30 randevusu')).toBeTruthy();
    expect(within(table).getByText('Fön paketi · 5 seans')).toBeTruthy();
    expect(within(table).getByText('Kart')).toBeTruthy();
    expect(within(table).getByText('Havale')).toBeTruthy();
    expect(within(table).getByText('Nakit')).toBeTruthy();
    expect(within(table).getByText('Kısmen iade edildi')).toBeTruthy();
  });

  it('Değerlendirmeler: müşterinin kartları, yanıt bloğu', async () => {
    open(selinCard);
    await userEvent.click(await screen.findByRole('tab', { name: 'Değerlendirmeler' }));
    const cards = await screen.findAllByRole('article', { name: 'Selin Öztürk değerlendirmesi' });
    expect(cards).toHaveLength(2);
    expect(screen.getByText('Yorum yazılmadı, yalnızca puan.')).toBeTruthy();
    expect(screen.getByText('Yanıtınız · Elif Kaya')).toBeTruthy();
  });

  it('personel: Ödemeler, Değerlendirmeler ve kayıt işlemleri yok', async () => {
    open(selinCard, { session: staff });
    const tabs = await screen.findByRole('tablist', { name: 'Müşteri kartı' });
    expect(
      within(tabs)
        .getAllByRole('tab')
        .map((t) => t.textContent),
    ).toEqual(['Randevular', 'Paketler']);
    expect(screen.queryByRole('heading', { name: 'Kayıt işlemleri' })).toBeNull();
  });

  it('"Paketi olan" filtresi paketli müşteriyi verir', async () => {
    open('/musteriler?filtre=paketi-olan');
    const table = await screen.findByRole('table', { name: 'Müşteriler' });
    await waitFor(() => {
      expect(within(table).getAllByRole('row')).toHaveLength(2);
    });
    expect(within(table).getByText('Selin Öztürk')).toBeTruthy();
  });
});

describe('kayıtları birleştir', () => {
  it('birleşecek kayıt aranır, notlar birleşir; kalacak kart güncellenir', async () => {
    const { calls } = open(selinCard, withPackage);
    await userEvent.click(await screen.findByRole('button', { name: 'Başka kayıtla birleştir' }));
    const dialog = await screen.findByRole('dialog', { name: 'Kayıtları birleştir' });
    const keep = within(dialog).getByRole('region', { name: 'Kalacak kayıt' });
    expect(within(keep).getByText('Selin Öztürk')).toBeTruthy();
    expect(await within(keep).findByText('Kesim paketi · 3 seans')).toBeTruthy();
    const submit = within(dialog).getByRole('button', { name: 'Kayıtları birleştir' });
    expect((submit as HTMLButtonElement).disabled).toBe(true);
    await userEvent.type(
      within(dialog).getByRole('searchbox', { name: 'Birleşecek kaydı ara' }),
      'Gizem',
    );
    await userEvent.click(await within(dialog).findByRole('button', { name: /Gizem Arslan/ }));
    const source = within(dialog).getByRole('region', { name: 'Birleşecek kayıt' });
    expect(within(source).getByText('0535 881 40 09')).toBeTruthy();
    expect(within(dialog).getByLabelText<HTMLInputElement>(/Notları birleştir/).checked).toBe(true);
    await userEvent.click(submit);
    expect(calls[`/api/v1/manage/customers/${customerIds.selin}/merge`]).toEqual([
      { sourceBrandCustomerId: customerIds.gizem, mergeNotes: true },
    ]);
    expect(await screen.findByText('Kayıtlar birleştirildi')).toBeTruthy();
    await waitFor(() => {
      expect(screen.queryByRole('dialog', { name: 'Kayıtları birleştir' })).toBeNull();
    });
    // 12 + 3 ziyaret.
    expect(await screen.findByText('15')).toBeTruthy();
  });

  it('arama kalacak kaydı listelemez', async () => {
    open(selinCard);
    await userEvent.click(await screen.findByRole('button', { name: 'Başka kayıtla birleştir' }));
    const dialog = await screen.findByRole('dialog', { name: 'Kayıtları birleştir' });
    await userEvent.type(within(dialog).getByRole('searchbox'), 'Selin');
    expect(await within(dialog).findByText('Bu aramayla başka kayıt bulunamadı.')).toBeTruthy();
  });

  it('şube müdürü birleştirir ama KVKK silme görmez', async () => {
    open(selinCard, { session: sessionFixture({ permissions: managerPermissions }) });
    expect(await screen.findByRole('button', { name: 'Başka kayıtla birleştir' })).toBeTruthy();
    expect(screen.queryByRole('button', { name: 'KVKK silme talebi' })).toBeNull();
  });
});

describe('KVKK silme talebi', () => {
  it('önizleme; ad yazılınca etkinleşir; anonimleştirip listeye döner', async () => {
    const { calls, router } = open(selinCard);
    await userEvent.click(await screen.findByRole('button', { name: 'KVKK silme talebi' }));
    const dialog = await screen.findByRole('dialog', { name: 'KVKK silme talebi' });
    expect(within(dialog).getByText('Bu işlem geri alınamaz.')).toBeTruthy();
    expect(
      within(dialog).getByText(/Selin Öztürk'ün adı, telefonu ve e-postası maskelenir/),
    ).toBeTruthy();
    expect(await within(dialog).findByText('12 randevu, 9 ödeme')).toBeTruthy();
    expect(within(dialog).getByText('Fön paketi · 2 seans')).toBeTruthy();
    const submit = within(dialog).getByRole('button', { name: 'Kalıcı olarak anonimleştir' });
    const field = within(dialog).getByLabelText('Onaylamak için müşterinin adını yazın');
    await userEvent.type(field, 'Selin');
    expect((submit as HTMLButtonElement).disabled).toBe(true);
    await userEvent.type(field, ' öztürk ');
    expect((submit as HTMLButtonElement).disabled).toBe(false);
    await userEvent.click(submit);
    expect(calls[`/api/v1/manage/customers/${customerIds.selin}/deletion`]).toEqual([
      { confirmFullName: 'Selin öztürk' },
    ]);
    expect(await screen.findByText('Müşteri kaydı anonimleştirildi')).toBeTruthy();
    await waitFor(() => {
      expect(router.state.location.pathname).toBe('/musteriler');
    });
    const table = await screen.findByRole('table', { name: 'Müşteriler' });
    await waitFor(() => {
      expect(within(table).queryByText('Selin Öztürk')).toBeNull();
    });
  });
});

describe('CSV içe ve dışa aktarma', () => {
  const csv = () => new File(['Ad;Telefon\n'], 'musteriler-eski-program.csv', { type: 'text/csv' });

  it('önizleme kartları, hatalı satırları indir, onay kutusu, içe aktar', async () => {
    const { calls } = open('/musteriler');
    await userEvent.click(await screen.findByRole('button', { name: 'İçe aktar' }));
    const dialog = await screen.findByRole('dialog', { name: 'Müşterileri içe aktar' });
    expect(
      within(dialog).getByRole<HTMLButtonElement>('button', { name: 'İçe aktar' }).disabled,
    ).toBe(true);
    await userEvent.upload(within(dialog).getByLabelText('CSV dosyası'), csv());
    expect(await within(dialog).findByText('musteriler-eski-program.csv')).toBeTruthy();
    expect(
      within(dialog).getByText('1.248 satır · Ad, Telefon, Not sütunları eşlendi'),
    ).toBeTruthy();
    const blocks = within(dialog).getByRole('group', { name: 'İçe aktarma özeti' });
    expect(within(blocks).getByText('1.204')).toBeTruthy();
    expect(within(blocks).getByText('38')).toBeTruthy();
    expect(within(blocks).getByText('telefon okunamadı')).toBeTruthy();
    expect(calls['/api/v1/manage/customers/imports Idempotency-Key']).toHaveLength(1);

    await userEvent.click(within(dialog).getByRole('button', { name: 'Hatalı satırları indir' }));
    expect(downloads[0]?.name).toBe('musteriler-eski-program-hatali-satirlar.csv');

    await userEvent.click(within(dialog).getByLabelText(/randevu bilgilendirme SMS/));
    await userEvent.click(within(dialog).getByRole('button', { name: '1.242 kaydı içe aktar' }));
    expect(calls[`/api/v1/manage/customers/imports/${importId}/commit`]).toEqual([
      { sendInfoSms: true },
    ]);
    expect(await screen.findByText('1.242 kayıt içe aktarıldı')).toBeTruthy();
  });

  it('dosya çok büyükse açıklama; süresi dolan önizleme yeniden seçtirir', async () => {
    open('/musteriler', { customers: { importError: 'too_large' } });
    await userEvent.click(await screen.findByRole('button', { name: 'İçe aktar' }));
    let dialog = await screen.findByRole('dialog', { name: 'Müşterileri içe aktar' });
    await userEvent.upload(within(dialog).getByLabelText('CSV dosyası'), csv());
    expect(await within(dialog).findByText(/Dosya çok büyük/)).toBeTruthy();
    cleanup();

    open('/musteriler', { customers: { importError: 'hold_expired' } });
    await userEvent.click(await screen.findByRole('button', { name: 'İçe aktar' }));
    dialog = await screen.findByRole('dialog', { name: 'Müşterileri içe aktar' });
    await userEvent.upload(within(dialog).getByLabelText('CSV dosyası'), csv());
    await userEvent.click(
      await within(dialog).findByRole('button', { name: '1.242 kaydı içe aktar' }),
    );
    expect(
      await within(dialog).findByText('Önizlemenin süresi doldu. Dosyayı yeniden seçin.'),
    ).toBeTruthy();
  });

  it('Dışa aktar seçili filtreyle CSV indirir', async () => {
    open('/musteriler?filtre=duzenli');
    await userEvent.click(await screen.findByRole('button', { name: 'Dışa aktar' }));
    await waitFor(() => {
      expect(downloads[0]?.name).toBe('musteriler-2026-11-25.csv');
    });
    // Yanıtın Blob'u ortama göre Node (text() var) veya jsdom (text() yok, FileReader) Blob'udur.
    const blob = downloads[0]?.blob ?? new Blob();
    const text =
      'text' in blob && typeof blob.text === 'function'
        ? await blob.text()
        : await new Promise<string>((resolve) => {
            const reader = new FileReader();
            reader.onload = () => {
              resolve(reader.result as string);
            };
            reader.readAsText(blob);
          });
    expect(text).toContain('Selin Öztürk');
    expect(text).not.toContain('Gizem Arslan');
  });

  it('şube müdürü ve personel içe/dışa aktarmayı görmez', async () => {
    open('/musteriler', { session: sessionFixture({ permissions: managerPermissions }) });
    await screen.findByRole('table', { name: 'Müşteriler' });
    expect(screen.queryByRole('button', { name: 'İçe aktar' })).toBeNull();
    expect(screen.queryByRole('button', { name: 'Dışa aktar' })).toBeNull();
  });

  it('boş markada "CSV içe aktar" ve tek tek ekle', async () => {
    open('/musteriler', { customers: { empty: true } });
    expect(await screen.findByRole('button', { name: 'CSV içe aktar' })).toBeTruthy();
    expect(screen.getByRole('button', { name: 'Tek tek ekle' })).toBeTruthy();
  });
});

describe('Ayarlar → KVKK ve veri', () => {
  it('silme talepleri: kalan gün, tamamlanan maskeli; yanıt süresi talepten', async () => {
    open('/ayarlar/kvkk');
    const table = await screen.findByRole('table', { name: 'Silme talepleri' });
    expect(within(table).getByText('Tülin Aksoy')).toBeTruthy();
    expect(within(table).getByText('Uygulamadan hesap silme')).toBeTruthy();
    expect(within(table).getByText('Bekliyor · 28 gün')).toBeTruthy();
    expect(within(table).getByText('A•••• K••••')).toBeTruthy();
    expect(within(table).getByText('Panelden anonimleştirme')).toBeTruthy();
    expect(within(table).getByText('Tamamlandı')).toBeTruthy();
    expect(within(table).getByText('23 Kas')).toBeTruthy();
    expect(screen.getByText('30 gün içinde yanıtlanmalı')).toBeTruthy();
    // Bekleyen talep müşteri kartına götürür (MusteriSil ile işlenir).
    expect(within(table).getByRole('link', { name: 'Tülin Aksoy' }).getAttribute('href')).toBe(
      `/musteriler/${customerIds.tulin}`,
    );
    // Dilim 6: üst bardaki kaydetme aydınlatma metni ve saklama süresi formunundur (T-127).
    expect(screen.getByRole('button', { name: 'Değişiklikleri kaydet' })).toBeTruthy();
    expect(
      within(screen.getByRole('navigation', { name: 'Ayar bölümleri' })).getByRole('link', {
        name: 'KVKK ve veri',
      }),
    ).toBeTruthy();
  });

  it('boş tablo', async () => {
    open('/ayarlar/kvkk', { customers: { noDeletionRequests: true } });
    expect(await screen.findByRole('heading', { name: 'Silme talebi yok' })).toBeTruthy();
  });

  it('şube müdürü bölümü görmez, adresten 403', async () => {
    open('/ayarlar/kvkk', { session: sessionFixture({ permissions: managerPermissions }) });
    expect(
      await screen.findByRole('heading', { level: 1, name: 'Bu sayfayı görme yetkiniz yok' }),
    ).toBeTruthy();
  });
});

describe('içe aktarma ve ödeme metinleri', () => {
  it('hatalı satırlar CSV: BOM, başlık, noktalı virgül', () => {
    const text = errorRowsCsv([
      { rowNumber: 14, column: 'Telefon', code: 'invalid_phone' },
      { rowNumber: 20, column: null, code: 'missing_name' },
    ]);
    expect(text).toBe('﻿Satır;Sütun;Hata\r\n14;Telefon;telefon okunamadı\r\n20;;ad yok\r\n');
    expect(errorRowsFileName('liste.CSV')).toBe('liste-hatali-satirlar.csv');
    expect(invalidRowsSummary([{ rowNumber: 1, code: 'invalid_phone' }])).toBe('telefon okunamadı');
    expect(
      invalidRowsSummary([
        { rowNumber: 1, code: 'invalid_phone' },
        { rowNumber: 2, code: 'too_long' },
      ]),
    ).toBe('atlanacak');
  });

  it('ödeme kalemi: düz randevu tahsilatı hizmet adları', () => {
    const payment = {
      purpose: 'Appointment',
      isDeposit: false,
      isBalanceAfterDeposit: false,
      serviceNames: ['Saç kesimi'],
    } as PaymentView;
    expect(paymentItemLabel(payment, 'Europe/Istanbul')).toBe('Saç kesimi');
  });
});
