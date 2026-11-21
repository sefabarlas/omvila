import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, expect, it } from 'vitest';
import { json, mockApi, problem } from '@/test/api-mock';
import { reviewCode, reviewInviteFixture, reviewedFixture } from '@/test/d5-fixtures';
import type { ReviewInvite } from './review-api';
import { ReviewForm } from './ReviewForm';

// Değerlendirme bağlantısı (/d/{kod}; KK8): yalnız tamamlanan randevu, bir kez.

const path = `/api/v1/public/reviews/${reviewCode}`;

function setup(
  invite: ReviewInvite | null,
  post: (request: Request) => Response = () => json(201, reviewedFixture()),
) {
  const calls = mockApi((request) => {
    if (new URL(request.url).pathname !== path) return problem(404, 'not_found');
    if (request.method === 'POST') return post(request);
    return invite ? json(200, invite) : problem(404, 'access_code_invalid');
  });
  const user = userEvent.setup();
  render(<ReviewForm code={reviewCode} />);
  return { calls, user };
}

describe('değerlendirme', () => {
  it('başlık, randevu bilgisi, personel kartı ve görünecek ad', async () => {
    setup(reviewInviteFixture());
    expect(await screen.findByRole('heading', { name: "Kuaför Elif'te nasıldı?" })).toBeTruthy();
    expect(screen.getByText('Boya ve fön · Elif Kaya · 12 Kasım')).toBeTruthy();
    expect(screen.getByText('Sizi bu randevuda ağırlayan kişi')).toBeTruthy();
    expect(screen.getByText(/Adınız "Merve Y." olarak görünür./)).toBeTruthy();
  });

  it('puan seçilmeden gönderilmez; 4 yıldız + yorum gönderilir, teşekkür gösterilir', async () => {
    const { calls, user } = setup(reviewInviteFixture());
    await user.click(await screen.findByRole('button', { name: 'Gönder' }));
    expect(screen.getByText('Göndermek için bir puan seçin.')).toBeTruthy();
    expect(calls.filter((request) => request.method === 'POST')).toHaveLength(0);

    await user.click(screen.getByRole('radio', { name: '4 yıldız, Çok iyi' }));
    expect(screen.getByText('Çok iyi')).toBeTruthy();
    await user.type(
      screen.getByRole('textbox', { name: 'Yorum (isteğe bağlı)' }),
      'Boya tam istediğim gibi oldu.',
    );
    await user.click(screen.getByRole('button', { name: 'Gönder' }));

    expect(await screen.findByRole('heading', { name: 'Teşekkür ederiz' })).toBeTruthy();
    expect(screen.getByText('Değerlendirmeniz Kuaför Elif ile paylaşıldı.')).toBeTruthy();
    const post = calls.find((request) => request.method === 'POST');
    expect(post?.headers.get('Idempotency-Key')).toBeTruthy();
    expect(await post?.clone().json()).toEqual({
      rating: 4,
      comment: 'Boya tam istediğim gibi oldu.',
    });
  });

  it('ağ hatasından sonra tekrar denemede aynı Idempotency-Key gider', async () => {
    let attempts = 0;
    const { calls, user } = setup(reviewInviteFixture(), () => {
      attempts += 1;
      return attempts === 1 ? problem(422, 'validation_failed') : json(201, reviewedFixture(null));
    });
    await user.click(await screen.findByRole('radio', { name: '5 yıldız, Mükemmel' }));
    await user.click(screen.getByRole('button', { name: 'Gönder' }));
    expect(await screen.findByRole('alert')).toBeTruthy();
    await user.click(screen.getByRole('button', { name: 'Gönder' }));
    expect(await screen.findByRole('heading', { name: 'Teşekkür ederiz' })).toBeTruthy();
    const keys = calls
      .filter((request) => request.method === 'POST')
      .map((request) => request.headers.get('Idempotency-Key'));
    expect(keys).toHaveLength(2);
    expect(keys[0]).toBe(keys[1]);
  });

  it('daha önce değerlendirilmişse form yerine gönderilmiş değerlendirme', async () => {
    setup(reviewedFixture());
    expect(
      await screen.findByRole('heading', { name: 'Bu randevuyu zaten değerlendirdiniz' }),
    ).toBeTruthy();
    expect(screen.getByRole('img', { name: '5 üzerinden 4 yıldız' })).toBeTruthy();
    expect(screen.getByText('Boya tam istediğim gibi oldu.')).toBeTruthy();
    expect(screen.queryByRole('button', { name: 'Gönder' })).toBeNull();
  });

  it('409 already_reviewed (başka sekmede gönderildi): yeniden okunur', async () => {
    let reads = 0;
    mockApi((request) => {
      if (request.method === 'POST') return problem(409, 'already_reviewed');
      reads += 1;
      return json(200, reads === 1 ? reviewInviteFixture() : reviewedFixture());
    });
    const user = userEvent.setup();
    render(<ReviewForm code={reviewCode} />);
    await user.click(await screen.findByRole('radio', { name: '3 yıldız, İyi' }));
    await user.click(screen.getByRole('button', { name: 'Gönder' }));
    expect(
      await screen.findByRole('heading', { name: 'Bu randevuyu zaten değerlendirdiniz' }),
    ).toBeTruthy();
  });

  it('personelsiz randevuda personel kartı yok; ek kurulamayan adda genel başlık', async () => {
    setup(reviewInviteFixture({ staff: null, brand: { name: 'Salon 34', slug: 'salon-34' } }));
    expect(await screen.findByRole('heading', { name: 'Randevunuz nasıldı?' })).toBeTruthy();
    expect(screen.queryByText('Sizi bu randevuda ağırlayan kişi')).toBeNull();
  });

  it('geçersiz veya süresi dolmuş kod', async () => {
    setup(null);
    expect(await screen.findByRole('heading', { name: 'Bu bağlantı geçersiz' })).toBeTruthy();
  });
});
