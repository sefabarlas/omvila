import { cleanup, screen, waitFor, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { afterEach, describe, expect, it } from 'vitest';
import { sessionFixture, staffPermissions } from '../../test/fake-api';
import type { FakeApiOptions } from '../../test/fake-api';
import { customerIds } from '../../test/fake-customers';
import { waitlistIds } from '../../test/fake-queues';
import { renderApp } from '../../test/render-app';
import { preferenceLabel, waitingSinceLabel } from './labels';

// Kural: dilim-05 KK3 (bekleme listesi, teklif 15 dk tutulur, "Saat öner", "Teklifi geri al"); T-90 (geri alınan saat
// sıradakine gitmez; kontör yoksa 409 insufficient_credits); roller-yetkiler.md (waitlist.manage: personel yok).
// Saat: Çarşamba 25 Kasım 2026, 12:00 İstanbul (listenin generatedUtc'si de bu an).
afterEach(cleanup);

const elif = '0193a8f0-0000-7000-8000-0000000000c1';
const haircut = '0193a8f0-0000-7000-8000-0000000000f1';

function openWaitlist(options: FakeApiOptions = {}) {
  const calls: Record<string, unknown[]> = {};
  const result = renderApp('/bekleme', { calls, withServices: true, ...options });
  return { ...result, calls };
}

describe('bekleme listesi', () => {
  it('özet, bekleyen teklif şeridi (kalan süre sunucu saatinden) ve tablo', async () => {
    openWaitlist();
    expect(await screen.findByRole('heading', { level: 1, name: 'Bekleme listesi' })).toBeTruthy();
    const table = await screen.findByRole('table', { name: 'Bekleyenler' });
    expect(screen.getByText('Merkez · 4 kişi bekliyor')).toBeTruthy();

    const kpis = screen.getByRole('group', { name: 'Bekleme özeti' });
    expect(within(kpis).getByText('4')).toBeTruthy();
    expect(within(kpis).getByText('en eskisi 6 gündür')).toBeTruthy();
    expect(within(kpis).getByText('11')).toBeTruthy();
    expect(within(kpis).getByText('15 dk')).toBeTruthy();
    expect(within(kpis).getByText('sonra sıradakine geçer')).toBeTruthy();

    expect(screen.getByText('Perşembe 11:00 boşaldı.')).toBeTruthy();
    expect(
      screen.getByText("Nur Ateş'e teklif gönderildi, 11 dakika içinde yanıt bekleniyor."),
    ).toBeTruthy();

    const rows = within(table).getAllByRole('row').slice(1);
    expect(rows.map((row) => row.textContent)).toEqual([
      expect.stringMatching(/^NANur Ateş3 gündür bekliyorFönPer–Cum · sabahElifTeklif gönderildi/),
      expect.stringMatching(
        /^TETuğba Er6 gündür bekliyorKeratinHerhangi bir gün · öğleden sonra veya akşamElifBekliyor/,
      ),
      expect.stringMatching(/^KPKaan Polat2 gündür bekliyorSakal ve kesimCmtMertBekliyor/),
      expect.stringMatching(
        /^İSİrem Sarı1 gündür bekliyorManikürHerhangi bir günFark etmezBekliyor/,
      ),
    ]);
    expect(within(table).getByRole('button', { name: 'Teklifi geri al: Nur Ateş' })).toBeTruthy();
    expect(within(table).getByRole('button', { name: 'Saat öner: Tuğba Er' })).toBeTruthy();
    expect(
      screen.getByText(
        'Bir randevu iptal edildiğinde uygun bekleyenlere sırayla teklif gider. İlk onaylayan saati alır.',
      ),
    ).toBeTruthy();
  });

  it('"Teklifi geri al": If-Match ile gider, kişi listede kalır', async () => {
    const { calls } = openWaitlist();
    await userEvent.click(await screen.findByRole('button', { name: 'Teklifi geri al: Nur Ateş' }));
    expect(await screen.findByText('Teklif geri alındı')).toBeTruthy();
    const path = `/api/v1/manage/waitlist/${waitlistIds.nur}/offer/withdraw`;
    expect(calls[`POST ${path} If-Match`]).toEqual(['W/"1"']);
    await waitFor(() => {
      expect(screen.queryByText(/boşaldı\.$/)).toBeNull();
    });
    expect(screen.getAllByText('Bekliyor')).toHaveLength(4);
    expect(screen.getByRole('button', { name: 'Saat öner: Nur Ateş' })).toBeTruthy();
  });

  it('"Saat öner": boş saat ızgarasından seçilen saat ve personel If-Match ile gider', async () => {
    const { calls } = openWaitlist();
    await userEvent.click(await screen.findByRole('button', { name: 'Saat öner: Tuğba Er' }));
    const dialog = await screen.findByRole('dialog', { name: 'Saat öner' });
    expect(within(dialog).getByText(/Tuğba Er · Keratin/)).toBeTruthy();
    expect(within(dialog).getByText(/Saat 15 dakika Tuğba Er için tutulur/)).toBeTruthy();
    await userEvent.click(await within(dialog).findByRole('radio', { name: '14:30' }));
    await userEvent.click(within(dialog).getByRole('button', { name: 'Teklifi gönder' }));
    await waitFor(() => {
      expect(screen.queryByRole('dialog', { name: 'Saat öner' })).toBeNull();
    });
    expect(screen.getAllByText('Teklif gönderildi')).toHaveLength(3);
    const path = `/api/v1/manage/waitlist/${waitlistIds.tugba}/offer`;
    expect(calls[path]).toEqual([{ startUtc: '2026-11-25T11:30:00.000Z', staffId: elif }]);
    expect(calls[`POST ${path} If-Match`]).toEqual(['W/"1"']);
  });

  it('kontör yoksa teklif oluşmaz; pencerede neden yazar', async () => {
    openWaitlist({ queues: { offerError: 'insufficient_credits' } });
    await userEvent.click(await screen.findByRole('button', { name: 'Saat öner: Kaan Polat' }));
    const dialog = await screen.findByRole('dialog', { name: 'Saat öner' });
    await userEvent.click(await within(dialog).findByRole('radio', { name: '10:30' }));
    await userEvent.click(within(dialog).getByRole('button', { name: 'Teklifi gönder' }));
    expect(
      await within(dialog).findByText(/SMS kontörünüz az\. Teklif müşteriye ulaşamayacağı için/),
    ).toBeTruthy();
  });

  it('"Saat öner" saat doldu: ızgara tazelenir, seçim kalkar', async () => {
    openWaitlist({ queues: { offerError: 'slot_taken' } });
    await userEvent.click(await screen.findByRole('button', { name: 'Saat öner: İrem Sarı' }));
    const dialog = await screen.findByRole('dialog', { name: 'Saat öner' });
    const radio = await within(dialog).findByRole<HTMLInputElement>('radio', { name: '10:30' });
    await userEvent.click(radio);
    await userEvent.click(within(dialog).getByRole('button', { name: 'Teklifi gönder' }));
    expect(
      await within(dialog).findByText('Bu saat az önce doldu. Başka bir saat seçin.'),
    ).toBeTruthy();
    expect(within(dialog).getByRole<HTMLInputElement>('radio', { name: '10:30' }).checked).toBe(
      false,
    );
  });

  it('"Listeye ekle": kayıtlı müşteri, hizmet, personel fark etmez, gün ve saat dilimi', async () => {
    const { calls } = openWaitlist();
    await screen.findByRole('table', { name: 'Bekleyenler' });
    await userEvent.click(screen.getByRole('button', { name: 'Listeye ekle' }));
    const dialog = await screen.findByRole('dialog', { name: 'Listeye ekle' });
    await userEvent.type(within(dialog).getByRole('searchbox', { name: 'Müşteri ara' }), 'Derya');
    await userEvent.click(await within(dialog).findByRole('button', { name: /Derya Şahin/ }));
    await userEvent.click(within(dialog).getByRole('button', { name: 'Saç kesimi' }));
    expect(within(dialog).getByLabelText<HTMLSelectElement>('Personel').value).toBe('');
    await userEvent.click(within(dialog).getByRole('button', { name: 'Cum 27' }));
    await userEvent.click(within(dialog).getByRole('button', { name: 'Per 26' }));
    await userEvent.click(within(dialog).getByRole('button', { name: 'Sabah' }));
    await userEvent.click(within(dialog).getByRole('button', { name: 'Listeye ekle' }));
    expect(await screen.findByText('Listeye eklendi')).toBeTruthy();
    expect(calls['/api/v1/manage/waitlist']).toEqual([
      {
        branchId: '0193a8f0-0000-7000-8000-0000000000b1',
        brandCustomerId: customerIds.derya,
        preferences: {
          serviceIds: [haircut],
          preferredStaffId: null,
          acceptOtherStaff: true,
          dates: ['2026-11-26', '2026-11-27'],
          timeWindows: ['Morning'],
        },
      },
    ]);
    expect(calls['/api/v1/manage/waitlist Idempotency-Key']).toHaveLength(1);
  });

  it('"Listeye ekle" eksik alanları yazar; kayıtlı numarada var olan müşteri seçilir', async () => {
    openWaitlist();
    await screen.findByRole('table', { name: 'Bekleyenler' });
    await userEvent.click(screen.getByRole('button', { name: 'Listeye ekle' }));
    const dialog = await screen.findByRole('dialog', { name: 'Listeye ekle' });
    await userEvent.click(within(dialog).getByRole('button', { name: 'Listeye ekle' }));
    expect(within(dialog).getByText('Müşteri seçin ya da yeni müşteri ekleyin.')).toBeTruthy();
    expect(within(dialog).getByText('En az bir hizmet seçin.')).toBeTruthy();

    await userEvent.click(within(dialog).getByRole('button', { name: 'Yeni müşteri ekle' }));
    await userEvent.type(within(dialog).getByLabelText('Ad soyad'), 'Selin Ö');
    await userEvent.type(within(dialog).getByLabelText('Cep telefonu'), '0532 418 33 21');
    await userEvent.click(within(dialog).getByRole('button', { name: 'Saç kesimi' }));
    await userEvent.click(within(dialog).getByRole('button', { name: 'Listeye ekle' }));
    expect(
      await within(dialog).findByText(/Bu numara kayıtlı; kayıtlı müşteri seçildi/),
    ).toBeTruthy();
    expect(within(dialog).getByText('Selin Öztürk')).toBeTruthy();
  });

  it('boş liste: BeklemeListesiBos (teklif süresi ayardan)', async () => {
    openWaitlist({ queues: { waitlistEmpty: true } });
    expect(await screen.findByRole('heading', { name: 'Bekleme listesi boş' })).toBeTruthy();
    expect(screen.getByText(/saat 15 dakika onun için tutulur\.$/)).toBeTruthy();
    expect(screen.getByText('Merkez')).toBeTruthy();
    expect(screen.getAllByRole('button', { name: 'Listeye ekle' })).toHaveLength(2);
  });

  it('personel bekleme listesini göremez: menüde yok, adres 403', async () => {
    renderApp('/bekleme', {
      session: sessionFixture({
        permissions: staffPermissions,
        membership: { role: 'Staff', branchIds: [], staffId: elif },
      }),
    });
    expect(
      await screen.findByRole('heading', { level: 1, name: 'Bu sayfayı görme yetkiniz yok' }),
    ).toBeTruthy();
    const nav = screen.getByRole('navigation', { name: 'Ana menü' });
    expect(within(nav).queryByRole('link', { name: 'Bekleme listesi' })).toBeNull();
  });
});

describe('bekleme listesi metinleri', () => {
  it('tercih: ardışık günler aralık, ayrık günler virgülle; üç saat dilimi = her saat', () => {
    expect(preferenceLabel(['2026-11-27', '2026-11-26'], ['Morning'])).toBe('Per–Cum · sabah');
    expect(preferenceLabel(['2026-11-26', '2026-11-28'], [])).toBe('Per, Cmt');
    expect(preferenceLabel([], ['Morning', 'Afternoon', 'Evening'])).toBe('Herhangi bir gün');
  });

  it('bekleme süresi gün olarak; aynı gün "bugün eklendi"', () => {
    expect(waitingSinceLabel('2026-11-22', '2026-11-25')).toBe('3 gündür bekliyor');
    expect(waitingSinceLabel('2026-11-25', '2026-11-25')).toBe('bugün eklendi');
  });
});
