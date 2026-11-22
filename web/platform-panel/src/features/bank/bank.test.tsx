import { cleanup, screen, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { afterEach, describe, expect, it } from 'vitest';
import { sessionFixture } from '@/test/fake-api';
import type { FakeApiOptions } from '@/test/fake-api';
import type { PlatformFakeOptions } from '@/test/fake-platform';
import { renderApp } from '@/test/render-app';

// Kural: banka hareketleri (KK2 ön koşulu): arama ve eşleşmemiş süzgeci; elle ekleme ve CSV içe aktarma yalnız
// platform.billing.manage (Yönetici, Finans); Destek'te butonlar kilitli ve nedeni yazar. Aynı referans 409
// bank_transaction_duplicate; CSV önce önizleme (dryRun), sonra uygula; hatalı satırlar listelenir.
afterEach(cleanup);

const finance = { session: sessionFixture('PlatformFinance') };

function open(path: string, options: FakeApiOptions = {}, platform: PlatformFakeOptions = {}) {
  const calls: Record<string, unknown[]> = {};
  renderApp(path, { calls, platform, ...options });
  return calls;
}

describe('banka hareketleri', () => {
  it('liste: tarih, tutar, eşleşme durumu ve kaynak', async () => {
    open('/onay/banka-hareketleri');
    const table = await screen.findByRole('table', { name: 'Banka hareketleri' });
    expect(within(table).getByText('18 Kas 14:05')).toBeTruthy();
    expect(within(table).getByText('ABN-KE-1125')).toBeTruthy();
    expect(within(table).getByText('Eşleşti · Spa Moda')).toBeTruthy();
    expect(within(table).getByText('Eşleşmedi')).toBeTruthy();
    expect(within(table).getByText('Elle')).toBeTruthy();
  });

  it('arama ve eşleşmemiş süzgeci sözleşme parametrelerini gönderir', async () => {
    const calls = open('/onay/banka-hareketleri?q=ABN-KE-1125');
    const table = await screen.findByRole('table', { name: 'Banka hareketleri' });
    expect(calls['/api/v1/admin/bank-transactions']?.at(-1)).toEqual({
      limit: '25',
      q: 'ABN-KE-1125',
    });
    await userEvent.click(screen.getByRole('switch', { name: 'Yalnız eşleşmemiş' }));
    await within(table).findByText('ELİF KAYA');
    expect(calls['/api/v1/admin/bank-transactions']?.at(-1)).toEqual({
      limit: '25',
      q: 'ABN-KE-1125',
      unmatchedOnly: 'true',
    });
  });

  it('sonuç yoksa boş hâl', async () => {
    open('/onay/banka-hareketleri?q=yok');
    expect(await screen.findByRole('heading', { name: 'Süzgece uyan hareket yok' })).toBeTruthy();
  });

  it('Destek rolünde ekleme ve içe aktarma kilitli', async () => {
    open('/onay/banka-hareketleri');
    for (const name of ['CSV içe aktar', 'Hareket ekle']) {
      const button = await screen.findByRole('button', { name });
      expect((button as HTMLButtonElement).disabled).toBe(true);
      expect(
        document.getElementById(button.getAttribute('aria-describedby') ?? '')?.textContent,
      ).toBe('Yönetici veya Finans yetkisi ister');
    }
  });

  it('Finans hareketi elle ekler', async () => {
    const calls = open('/onay/banka-hareketleri', finance);
    await userEvent.click(await screen.findByRole('button', { name: 'Hareket ekle' }));
    const dialog = await screen.findByRole('dialog', { name: 'Banka hareketi ekle' });
    await userEvent.click(within(dialog).getByRole('button', { name: 'Hareketi ekle' }));
    expect(await within(dialog).findByText('Tutarı yazın (ör. 1.000 veya 850,50).')).toBeTruthy();
    await userEvent.type(within(dialog).getByLabelText('Tutar (₺)'), '1.000');
    await userEvent.type(within(dialog).getByLabelText('Gönderen'), 'ELİF KAYA');
    await userEvent.type(within(dialog).getByLabelText('Açıklama'), 'ABN-KE-1125');
    await userEvent.type(within(dialog).getByLabelText('Banka referansı'), '20261120-0001');
    await userEvent.click(within(dialog).getByRole('button', { name: 'Hareketi ekle' }));
    expect(await screen.findByText('Hareket eklendi')).toBeTruthy();
    expect(
      calls['/api/v1/admin/bank-transactions']?.find(
        (c) => typeof c === 'object' && c !== null && 'amount' in c,
      ),
    ).toEqual({
      valueDate: '2026-11-20',
      amount: { amountMinor: 100000, currency: 'TRY' },
      senderName: 'ELİF KAYA',
      description: 'ABN-KE-1125',
      bankReference: '20261120-0001',
    });
  });

  it('aynı referans (409 bank_transaction_duplicate) alanın altında yazılır', async () => {
    open('/onay/banka-hareketleri', finance, { duplicateReference: true });
    await userEvent.click(await screen.findByRole('button', { name: 'Hareket ekle' }));
    const dialog = await screen.findByRole('dialog', { name: 'Banka hareketi ekle' });
    await userEvent.type(within(dialog).getByLabelText('Tutar (₺)'), '500');
    await userEvent.type(within(dialog).getByLabelText('Gönderen'), 'SPA MODA');
    await userEvent.type(within(dialog).getByLabelText('Banka referansı'), '20261117-000913');
    await userEvent.click(within(dialog).getByRole('button', { name: 'Hareketi ekle' }));
    expect(
      await within(dialog).findByText('Bu banka referansıyla kayıtlı bir hareket var.'),
    ).toBeTruthy();
  });

  it('CSV: önizleme (dryRun) → hatalı satırlar → uygula', async () => {
    const calls = open('/onay/banka-hareketleri', finance);
    await userEvent.click(await screen.findByRole('button', { name: 'CSV içe aktar' }));
    const dialog = await screen.findByRole('dialog', { name: 'Banka dökümünü içe aktar' });
    const file = new File(['Tarih;Tutar\n'], 'ekstre-2026-11-18.csv', { type: 'text/csv' });
    await userEvent.upload(within(dialog).getByLabelText('CSV dosyası'), file);
    expect(await within(dialog).findByText('ekstre-2026-11-18.csv')).toBeTruthy();
    const summary = within(dialog).getByRole('group', { name: 'İçe aktarma özeti' });
    expect(within(summary).getByText('38')).toBeTruthy();
    const errors = within(dialog).getByRole('table', { name: 'Hatalı satırlar' });
    expect(within(errors).getByText('17')).toBeTruthy();
    expect(within(errors).getByText('Biçim hatalı')).toBeTruthy();
    await userEvent.click(within(dialog).getByRole('button', { name: '38 hareketi içe aktar' }));
    expect(await screen.findByText('38 hareket içe aktarıldı')).toBeTruthy();
    expect(calls['/api/v1/admin/bank-transactions/import']).toHaveLength(2);
  });

  it('CSV olmayan dosya nedenini yazar', async () => {
    open('/onay/banka-hareketleri', finance, { importInvalidFile: true });
    await userEvent.click(await screen.findByRole('button', { name: 'CSV içe aktar' }));
    const dialog = await screen.findByRole('dialog', { name: 'Banka dökümünü içe aktar' });
    await userEvent.upload(
      within(dialog).getByLabelText('CSV dosyası'),
      new File(['x'], 'dekont.csv', { type: 'text/csv' }),
    );
    expect(
      await within(dialog).findByText('Dosya okunamadı. UTF-8 CSV seçin; ilk satır başlık olmalı.'),
    ).toBeTruthy();
  });
});
