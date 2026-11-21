import { cleanup, screen, waitFor, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { afterEach, describe, expect, it } from 'vitest';
import { sessionFixture, staffPermissions } from '../../test/fake-api';
import type { FakeApiOptions } from '../../test/fake-api';
import { reviewIds } from '../../test/fake-reviews';
import { renderApp } from '../../test/render-app';
import { formatRating, reviewAge } from './labels';

// Kural: dilim-05 KK8 (salon tarafı: liste, filtreler, yanıt), roller-yetkiler.md (review.view / review.reply;
// personel görmez), sözleşme manageListReviews (AwaitingReply düşük puan önce), DegerlendirmelerBos (eşik 5).
// Saat: Çarşamba 25 Kasım 2026, 12:00 İstanbul.
afterEach(() => {
  cleanup();
});

function open(path = '/degerlendirmeler', options: FakeApiOptions = {}) {
  const calls: Record<string, unknown[]> = {};
  const result = renderApp(path, { calls, ...options });
  return { ...result, calls };
}

const card = (name: string) => screen.findByRole('article', { name: `${name} değerlendirmesi` });

describe('değerlendirmeler', () => {
  it('üst kartlar: ortalama, yanıt bekleyen, bu ay davet oranı, en çok övülen', async () => {
    open();
    expect(await screen.findByRole('heading', { level: 1, name: 'Değerlendirmeler' })).toBeTruthy();
    const blocks = await screen.findByRole('group', { name: 'Değerlendirme özeti' });
    expect(within(blocks).getByText('4,8')).toBeTruthy();
    expect(within(blocks).getByText('son 90 gün · 214 değerlendirme')).toBeTruthy();
    expect(within(blocks).getByText('düşük puanlılar önce')).toBeTruthy();
    expect(within(blocks).getByText('31')).toBeTruthy();
    expect(within(blocks).getByText("davet edilenlerin %38'i")).toBeTruthy();
    expect(within(blocks).getByText('Elif Kaya')).toBeTruthy();
    expect(within(blocks).getByText('4,9 · 96 değerlendirme')).toBeTruthy();
    expect(screen.getByText('Merkez · 214 değerlendirme')).toBeTruthy();
  });

  it('yanıt bekleyen: düşük puanlı önce, ilk kartta yanıt alanı açık; puan yalnızsa metin', async () => {
    open();
    const first = await card('Gizem Arslan');
    const cards = screen.getAllByRole('article');
    expect(cards[0]).toBe(first);
    expect(within(first).getByRole('img', { name: '5 üzerinden 3 yıldız' })).toBeTruthy();
    expect(within(first).getByText(/Kaş ve kirpik · Mert · 3 gün önce/)).toBeTruthy();
    expect(within(first).getByPlaceholderText('Yanıtınızı yazın, müşteri görür')).toBeTruthy();
    const second = await card('Selin Öztürk');
    expect(within(second).getByText('Yorum yazılmadı, yalnızca puan.')).toBeTruthy();
    expect(within(second).getByText(/Keratin · Elif · dün/)).toBeTruthy();
    // Yanıtlanmış değerlendirme bu filtrede yok.
    expect(screen.queryByText(/Yanıtınız ·/)).toBeNull();
  });

  it('Yanıtla: metin gönderilir, kart "Yanıtınız · Elif Kaya" bloğunu gösterir', async () => {
    const { calls } = open();
    const first = await card('Gizem Arslan');
    const submit = within(first).getByRole('button', { name: 'Yanıtla' });
    expect((submit as HTMLButtonElement).disabled).toBe(true);
    await userEvent.type(
      within(first).getByPlaceholderText('Yanıtınızı yazın, müşteri görür'),
      'Geri bildiriminiz için teşekkür ederiz.',
    );
    await userEvent.click(submit);
    expect(calls[`/api/v1/manage/reviews/${reviewIds.ece}/reply`]).toEqual([
      { text: 'Geri bildiriminiz için teşekkür ederiz.' },
    ]);
    expect(await screen.findByText('Yanıtınız yayımlandı')).toBeTruthy();
    // Yanıtlanan kart "Yanıt bekleyen"den çıkar; Tümü'nde yanıt bloğuyla görünür.
    await waitFor(() => {
      expect(screen.queryByRole('article', { name: 'Gizem Arslan değerlendirmesi' })).toBeNull();
    });
    await userEvent.click(screen.getByLabelText('Tümü'));
    const answered = await card('Gizem Arslan');
    expect(within(answered).getByText('Yanıtınız · Elif Kaya')).toBeTruthy();
    expect(within(answered).getByText('Geri bildiriminiz için teşekkür ederiz.')).toBeTruthy();
  });

  it('Vazgeç yanıt alanını kapatır; "Yanıtla" yeniden açar', async () => {
    open();
    const first = await card('Gizem Arslan');
    await userEvent.click(within(first).getByRole('button', { name: 'Vazgeç' }));
    expect(within(first).queryByPlaceholderText('Yanıtınızı yazın, müşteri görür')).toBeNull();
    await userEvent.click(within(first).getByRole('button', { name: 'Yanıtla' }));
    expect(within(first).getByPlaceholderText('Yanıtınızı yazın, müşteri görür')).toBeTruthy();
  });

  it('Tümü yeniden eskiye, imleçle sayfalanır; 3 yıldız ve altı süzer; filtre adreste', async () => {
    const { router } = open('/degerlendirmeler?filtre=tumu');
    await card('Selin Öztürk');
    expect(screen.getByText('2 değerlendirme gösteriliyor')).toBeTruthy();
    await userEvent.click(screen.getByRole('button', { name: 'Daha fazla göster' }));
    await waitFor(() => {
      expect(screen.getAllByRole('article')).toHaveLength(3);
    });
    expect(screen.getByText('Yanıtınız · Elif Kaya')).toBeTruthy();
    await userEvent.click(screen.getByLabelText('3 yıldız ve altı'));
    await waitFor(() => {
      expect(screen.getAllByRole('article')).toHaveLength(1);
    });
    expect(router.state.location.search).toEqual({ filtre: 'dusuk-puan' });
  });

  it('boş durum: iki bilgi kutusu, eşik 5', async () => {
    open('/degerlendirmeler', { reviews: { empty: true } });
    expect(await screen.findByRole('heading', { name: 'Henüz değerlendirme yok' })).toBeTruthy();
    expect(screen.getByText(/randevudan 2 saat sonra/)).toBeTruthy();
    expect(screen.getByText('Yanıt verebilirsiniz')).toBeTruthy();
    expect(screen.getByText('En az 5 değerlendirme olunca ortalama puan gösterilir.')).toBeTruthy();
    expect(screen.queryByRole('group', { name: 'Değerlendirme özeti' })).toBeNull();
  });

  it('yanıt hatası alanın altında yazar', async () => {
    open('/degerlendirmeler', { reviews: { replyError: true } });
    const first = await card('Gizem Arslan');
    await userEvent.type(within(first).getByRole('textbox'), 'Teşekkürler');
    await userEvent.click(within(first).getByRole('button', { name: 'Yanıtla' }));
    expect(await within(first).findByText('Bir sorun oluştu. Tekrar deneyin.')).toBeTruthy();
    expect(within(first).getByRole('textbox').getAttribute('aria-invalid')).toBe('true');
  });

  it('personel menüde görmez, adresten 403', async () => {
    open('/degerlendirmeler', {
      session: sessionFixture({
        membership: {
          role: 'Staff',
          branchIds: [],
          staffId: '0193a8f0-0000-7000-8000-0000000000c2',
        },
        permissions: staffPermissions,
      }),
    });
    expect(
      await screen.findByRole('heading', { level: 1, name: 'Bu sayfayı görme yetkiniz yok' }),
    ).toBeTruthy();
    expect(screen.queryByRole('link', { name: /Değerlendirmeler/ })).toBeNull();
  });

  it('menüde Değerlendirmeler sahibe açık', async () => {
    open('/');
    expect(await screen.findByRole('link', { name: /Değerlendirmeler/ })).toBeTruthy();
  });
});

describe('değerlendirme etiketleri', () => {
  it('yaş: bugün, dün, gün, hafta, tarih', () => {
    const today = '2026-11-25';
    expect(reviewAge('2026-11-25T08:00:00Z', today, 'Europe/Istanbul')).toBe('bugün');
    expect(reviewAge('2026-11-24T08:00:00Z', today, 'Europe/Istanbul')).toBe('dün');
    expect(reviewAge('2026-11-22T08:00:00Z', today, 'Europe/Istanbul')).toBe('3 gün önce');
    expect(reviewAge('2026-11-18T08:00:00Z', today, 'Europe/Istanbul')).toBe('1 hafta önce');
    expect(reviewAge('2026-10-01T08:00:00Z', today, 'Europe/Istanbul')).toBe('1 Ekim');
    // Gece yarısından sonra İstanbul'da ertesi gün.
    expect(reviewAge('2026-11-24T21:30:00Z', today, 'Europe/Istanbul')).toBe('bugün');
  });

  it('puan tek ondalık, yoksa —', () => {
    expect(formatRating(4.8)).toBe('4,8');
    expect(formatRating(5)).toBe('5,0');
    expect(formatRating(null)).toBe('—');
  });
});
