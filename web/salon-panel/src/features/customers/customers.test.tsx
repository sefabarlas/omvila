import { cleanup, screen, waitFor, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { afterEach, describe, expect, it } from 'vitest';
import { sessionFixture, staffPermissions } from '../../test/fake-api';
import type { FakeApiOptions } from '../../test/fake-api';
import { customerIds } from '../../test/fake-customers';
import { renderApp } from '../../test/render-app';

// Kural: dilim-02 kabul kriteri 15 (kayıtlı numara → phone_already_exists + "Mevcut kartı aç"; "4418" son 4 haneyle
// bulur; personel rolünde liste maskeli telefon); roller-yetkiler.md (customer.view, customer.edit); T-46 (arama).
// Saat: Çarşamba 25 Kasım 2026, 12:00 İstanbul.
afterEach(() => {
  cleanup();
});

function open(path = '/musteriler', options: FakeApiOptions = {}) {
  const calls: Record<string, unknown[]> = {};
  const result = renderApp(path, { calls, ...options });
  return { ...result, calls };
}

const table = () => screen.findByRole('table', { name: 'Müşteriler' });
const rowOf = async (name: string) => {
  const cell = await within(await table()).findByText(name);
  const row = cell.closest('tr');
  if (!row) {
    throw new Error(`${name} satırı yok`);
  }

  return row;
};

const staff = sessionFixture({
  user: { id: '0193a8f0-0000-7000-8000-000000000002', fullName: 'Mert Demir', phoneMasked: '' },
  membership: { role: 'Staff', branchIds: [], staffId: '0193a8f0-0000-7000-8000-0000000000c2' },
  permissions: staffPermissions,
});

describe('müşteri listesi', () => {
  it('satırlar: telefon, son ziyaret, sonraki randevu, ziyaret, toplam, etiket', async () => {
    open();
    expect(await screen.findByRole('heading', { level: 1, name: 'Müşteriler' })).toBeTruthy();
    const selin = await rowOf('Selin Öztürk');
    expect(within(selin).getByText('0532 418 33 21')).toBeTruthy();
    expect(within(selin).getByText('Bugün')).toBeTruthy();
    expect(within(selin).getByText('12')).toBeTruthy();
    expect(within(selin).getByText('14.860 ₺')).toBeTruthy();
    expect(within(selin).getByText('Düzenli')).toBeTruthy();
    const derya = await rowOf('Derya Şahin');
    expect(within(derya).getByText('21 Ekim')).toBeTruthy();
    expect(within(derya).getByText('Bugün 13:00')).toBeTruthy();
    expect(within(await rowOf('Gizem Arslan')).getByText('2 kez gelmedi')).toBeTruthy();
    expect(within(await rowOf('Nur Ateş')).getByText('0 ₺')).toBeTruthy();
    // "Paketi olan" paketlerle (D5) gelir.
    const chips = screen.getByRole('group', { name: 'Müşteri filtresi' });
    expect(within(chips).queryByLabelText('Paketi olan')).toBeNull();
    expect(screen.getByRole('button', { name: 'Müşteri ekle' })).toBeTruthy();
  });

  it('imleçle sayfalama: Daha fazla göster', async () => {
    open();
    await rowOf('Selin Öztürk');
    expect(screen.getByText('4 müşteri gösteriliyor')).toBeTruthy();
    expect(screen.queryByText('Tülin Aksoy')).toBeNull();
    await userEvent.click(screen.getByRole('button', { name: 'Daha fazla göster' }));
    expect(await rowOf('Tülin Aksoy')).toBeTruthy();
    expect(screen.getByText('5 müşteri gösteriliyor')).toBeTruthy();
    expect(screen.queryByRole('button', { name: 'Daha fazla göster' })).toBeNull();
  });

  it('"4418" son 4 haneyle bulur; adres aramayı taşır (kriter 15)', async () => {
    const { router } = open();
    await rowOf('Selin Öztürk');
    await userEvent.type(screen.getByRole('searchbox', { name: 'Müşteri ara' }), '4418');
    await waitFor(() => {
      expect(screen.queryByText('Selin Öztürk')).toBeNull();
    });
    expect(await rowOf('Tülin Aksoy')).toBeTruthy();
    expect(router.state.location.search).toEqual({ q: '4418' });
  });

  it('ada göre arama', async () => {
    open('/musteriler?q=derya');
    expect(await rowOf('Derya Şahin')).toBeTruthy();
    expect(screen.queryByText('Selin Öztürk')).toBeNull();
    expect(screen.getByRole<HTMLInputElement>('searchbox', { name: 'Müşteri ara' }).value).toBe(
      'derya',
    );
  });

  it('filtre: Gelmeme riski', async () => {
    const { router } = open();
    await rowOf('Selin Öztürk');
    await userEvent.click(screen.getByLabelText('Gelmeme riski'));
    await waitFor(() => {
      expect(screen.queryByText('Selin Öztürk')).toBeNull();
    });
    expect(await rowOf('Gizem Arslan')).toBeTruthy();
    expect(router.state.location.search).toEqual({ filtre: 'gelmeme-riski' });
  });

  it('personel: telefon maskeli, menüde Müşteriler (kriter 15)', async () => {
    open('/musteriler', { session: staff });
    const selin = await rowOf('Selin Öztürk');
    expect(within(selin).getByText('0532 *** ** 21')).toBeTruthy();
    expect(within(selin).queryByText('0532 418 33 21')).toBeNull();
    const nav = screen.getByRole('navigation', { name: 'Ana menü' });
    expect(within(nav).getByRole('link', { name: 'Müşteriler' }).getAttribute('aria-current')).toBe(
      'page',
    );
  });

  it('yetkisiz kullanıcı: 403', async () => {
    open('/musteriler', {
      session: sessionFixture({ permissions: ['calendar.view', 'report.view'] }),
    });
    expect(
      await screen.findByRole('heading', { level: 1, name: 'Bu sayfayı görme yetkiniz yok' }),
    ).toBeTruthy();
  });

  it('hiç müşteri yok: MusterilerBos', async () => {
    open('/musteriler', { customers: { empty: true } });
    expect(
      await screen.findByRole('heading', { level: 2, name: 'Henüz müşteriniz yok' }),
    ).toBeTruthy();
    expect(screen.getByText('Telefon numarası anahtardır')).toBeTruthy();
    expect(screen.queryByRole('group', { name: 'Müşteri filtresi' })).toBeNull();
    await userEvent.click(screen.getByRole('button', { name: 'Tek tek ekle' }));
    expect(await screen.findByRole('dialog', { name: 'Müşteri ekle' })).toBeTruthy();
  });

  it('sonuç yok: filtreyi temizle ve aramadaki adla yeni müşteri', async () => {
    const { router } = open('/musteriler?q=Kaan%20Y%C4%B1l&filtre=duzenli');
    expect(
      await screen.findByRole('heading', { level: 2, name: '“Kaan Yıl” için sonuç yok' }),
    ).toBeTruthy();
    expect(
      screen.getByText(
        'Ad, soyad ya da telefonun son 4 hanesiyle arayabilirsiniz. Seçili filtre: Düzenli.',
      ),
    ).toBeTruthy();
    await userEvent.click(screen.getByRole('button', { name: 'Yeni müşteri olarak ekle' }));
    const dialog = await screen.findByRole('dialog', { name: 'Müşteri ekle' });
    expect(within(dialog).getByLabelText<HTMLInputElement>('Ad soyad').value).toBe('Kaan Yıl');
    await userEvent.click(within(dialog).getByRole('button', { name: 'Vazgeç' }));
    await userEvent.click(screen.getByRole('button', { name: 'Filtreyi temizle' }));
    expect(await rowOf('Selin Öztürk')).toBeTruthy();
    expect(router.state.location.search).toEqual({});
  });
});

describe('müşteri ekle', () => {
  it('ekler ve kartı açar; Idempotency-Key ve SMS tercihi gider', async () => {
    const { router, calls } = open();
    await rowOf('Selin Öztürk');
    await userEvent.click(screen.getByRole('button', { name: 'Müşteri ekle' }));
    const dialog = await screen.findByRole('dialog', { name: 'Müşteri ekle' });
    expect(within(dialog).getByText('Sağlık bilgisi yazmayın.')).toBeTruthy();
    expect(within(dialog).getByText(/Kampanya mesajı izni müşteriden alınır/)).toBeTruthy();
    await userEvent.type(within(dialog).getByLabelText('Ad soyad'), 'Kaan Yıldız');
    await userEvent.type(within(dialog).getByLabelText('Cep telefonu'), '0533 222 11 00');
    await userEvent.type(within(dialog).getByLabelText('Doğum yılı (isteğe bağlı)'), '1990');
    await userEvent.click(
      within(dialog).getByLabelText(/Randevu bilgilendirme SMS'leri gönderilsin/),
    );
    await userEvent.click(within(dialog).getByRole('button', { name: 'Müşteriyi ekle' }));
    expect(await screen.findByRole('heading', { level: 1, name: 'Kaan Yıldız' })).toBeTruthy();
    expect(router.state.location.pathname).toBe(`/musteriler/${customerIds.created}`);
    expect(calls['/api/v1/manage/customers']).toEqual([
      {
        fullName: 'Kaan Yıldız',
        phone: '0533 222 11 00',
        birthYear: 1990,
        notes: null,
        sendAppointmentSms: false,
      },
    ]);
  });

  it('eksik alanlar: hata mesajları, istek gitmez', async () => {
    const { calls } = open();
    await rowOf('Selin Öztürk');
    await userEvent.click(screen.getByRole('button', { name: 'Müşteri ekle' }));
    const dialog = await screen.findByRole('dialog', { name: 'Müşteri ekle' });
    await userEvent.type(within(dialog).getByLabelText('Cep telefonu'), '123');
    await userEvent.type(within(dialog).getByLabelText('Doğum yılı (isteğe bağlı)'), '2030');
    await userEvent.click(within(dialog).getByRole('button', { name: 'Müşteriyi ekle' }));
    expect(await within(dialog).findByText('Ad soyad en az 2 karakter olmalı.')).toBeTruthy();
    expect(within(dialog).getByText('Cep telefonu 10 haneli olmalı: 0532 901 44 18')).toBeTruthy();
    expect(within(dialog).getByText('Doğum yılı 1900 ile 2026 arasında olmalı.')).toBeTruthy();
    expect(calls['/api/v1/manage/customers']).toBeUndefined();
  });

  it('kayıtlı numara: "Bu numara zaten kayıtlı" ve Mevcut kartı aç (kriter 15)', async () => {
    const { router } = open();
    await rowOf('Selin Öztürk');
    await userEvent.click(screen.getByRole('button', { name: 'Müşteri ekle' }));
    const dialog = await screen.findByRole('dialog', { name: 'Müşteri ekle' });
    await userEvent.type(within(dialog).getByLabelText('Ad soyad'), 'Tülin A.');
    await userEvent.type(within(dialog).getByLabelText('Cep telefonu'), '0532 901 44 18');
    await userEvent.click(within(dialog).getByRole('button', { name: 'Müşteriyi ekle' }));
    expect(await within(dialog).findByText('Bu numara zaten kayıtlı.')).toBeTruthy();
    expect(
      await within(dialog).findByText(
        'Tülin Aksoy, 3 ziyaret, son 14 Eylül. Yeni kayıt açılmaz, mevcut karta gidersiniz.',
      ),
    ).toBeTruthy();
    expect(within(dialog).queryByRole('button', { name: 'Müşteriyi ekle' })).toBeNull();
    await userEvent.click(within(dialog).getByRole('button', { name: 'Mevcut kartı aç' }));
    expect(await screen.findByRole('heading', { level: 1, name: 'Tülin Aksoy' })).toBeTruthy();
    expect(router.state.location.pathname).toBe(`/musteriler/${customerIds.tulin}`);
    expect(screen.queryByRole('dialog', { name: 'Müşteri ekle' })).toBeNull();
  });

  it('kayıtlı numara uyarısı numara değişince kalkar', async () => {
    open();
    await rowOf('Selin Öztürk');
    await userEvent.click(screen.getByRole('button', { name: 'Müşteri ekle' }));
    const dialog = await screen.findByRole('dialog', { name: 'Müşteri ekle' });
    await userEvent.type(within(dialog).getByLabelText('Ad soyad'), 'Tülin A.');
    const phone = within(dialog).getByLabelText('Cep telefonu');
    await userEvent.type(phone, '0532 901 44 18');
    await userEvent.click(within(dialog).getByRole('button', { name: 'Müşteriyi ekle' }));
    await within(dialog).findByText('Bu numara zaten kayıtlı.');
    await userEvent.type(phone, '{Backspace}9');
    await waitFor(() => {
      expect(within(dialog).queryByText('Bu numara zaten kayıtlı.')).toBeNull();
    });
    expect(within(dialog).getByRole('button', { name: 'Müşteriyi ekle' })).toBeTruthy();
  });
});

describe('müşteri kartı', () => {
  it('kimlik, metrikler, randevular sekmesi ve notlar', async () => {
    open(`/musteriler/${customerIds.selin}`);
    expect(await screen.findByRole('heading', { level: 1, name: 'Selin Öztürk' })).toBeTruthy();
    expect(screen.getByText('Müşteriler / Müşteri kartı')).toBeTruthy();
    const card = screen.getByRole('region', { name: 'Müşteri' });
    expect(within(card).getByText('0532 418 33 21')).toBeTruthy();
    expect(within(card).getByText('Düzenli')).toBeTruthy();
    expect(within(card).getByText('1994')).toBeTruthy();
    const summary = screen.getByRole('group', { name: 'Müşteri özeti' });
    expect(within(summary).getByText('son: bugün')).toBeTruthy();
    expect(within(summary).getByText('14.860 ₺')).toBeTruthy();
    expect(within(summary).getByText('ort. 1.238 ₺')).toBeTruthy();
    expect(within(summary).getByText('hiç')).toBeTruthy();
    expect(screen.getByRole('tab', { name: 'Randevular' }).getAttribute('aria-selected')).toBe(
      'true',
    );
    const appointments = await screen.findByRole('table', { name: 'Randevular' });
    const rows = within(appointments).getAllByRole('row').slice(1);
    expect(rows.map((r) => within(r).getAllByRole('cell')[0]?.textContent)).toEqual([
      'Bugün 10:30',
      '2 Ekim 11:00',
      '20 Ağustos 10:00',
    ]);
    const [first, , cancelled] = rows;
    if (!first || !cancelled) {
      throw new Error('randevu satırları eksik');
    }

    expect(within(first).getByText('Geldi')).toBeTruthy();
    expect(within(first).getByText('1.440 ₺')).toBeTruthy();
    expect(within(cancelled).getByText('İptal')).toBeTruthy();
    expect(within(cancelled).getByText('—')).toBeTruthy();
    expect(screen.getByText(/Amonyaksız ürün tercih ediyor/)).toBeTruthy();
    // Randevu ver açık (T-58); paket, KVKK, kayıt işlemleri ve SMS gönder sonraki dilimlerde.
    expect(screen.getByRole('button', { name: 'Randevu ver' })).toBeTruthy();
    expect(screen.queryByRole('button', { name: 'SMS gönder' })).toBeNull();
  });

  it('randevusu olmayan müşteri: boş hâl', async () => {
    open(`/musteriler/${customerIds.nur}`);
    expect(
      await screen.findByRole('heading', { level: 3, name: 'Henüz randevu yok' }),
    ).toBeTruthy();
    const summary = screen.getByRole('group', { name: 'Müşteri özeti' });
    expect(within(summary).getByText('henüz ziyaret yok')).toBeTruthy();
    expect(within(summary).queryByText(/ort\./)).toBeNull();
  });

  it('personel: kartta telefon maskeli', async () => {
    open(`/musteriler/${customerIds.selin}`, { session: staff });
    const card = await screen.findByRole('region', { name: 'Müşteri' });
    expect(within(card).getByText('0532 *** ** 21')).toBeTruthy();
  });

  it('bilinmeyen müşteri: 404', async () => {
    open('/musteriler/0193a8f0-0000-7000-8000-00000000cfff');
    expect(
      await screen.findByRole('heading', { level: 1, name: 'Bu kayıt bulunamadı' }),
    ).toBeTruthy();
  });

  it('listeden satıra tıklayınca kart açılır', async () => {
    const { router } = open();
    await userEvent.click(await rowOf('Derya Şahin'));
    expect(await screen.findByRole('heading', { level: 1, name: 'Derya Şahin' })).toBeTruthy();
    expect(router.state.location.pathname).toBe(`/musteriler/${customerIds.derya}`);
  });

  it('takvim yan panelindeki Müşteri kartı kartı açar', async () => {
    const { router } = open('/takvim');
    await userEvent.click(await screen.findByRole('button', { name: /^Derya Şahin/ }));
    const drawer = await screen.findByRole('dialog', { name: 'Randevu · Telefonla' });
    await userEvent.click(await within(drawer).findByRole('button', { name: 'Müşteri kartı' }));
    expect(await screen.findByRole('heading', { level: 1, name: 'Derya Şahin' })).toBeTruthy();
    expect(router.state.location.pathname).toBe(`/musteriler/${customerIds.derya}`);
    expect(screen.queryByRole('dialog')).toBeNull();
  });
});
