import { cleanup, fireEvent, screen, waitFor, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { afterEach, describe, expect, it } from 'vitest';
import { branchId, sessionFixture } from '../../test/fake-api';
import type { FakeApiOptions } from '../../test/fake-api';
import { staffIds } from '../../test/fake-calendar';
import { customerIds } from '../../test/fake-customers';
import { renderApp } from '../../test/render-app';

// Kural: dilim-02 kabul kriterleri 11 (panel yeni randevu, Telefonla, SMS; 20:00 çalışma saati dışı onayı), 13 (her
// 4 haftada bir, atlanan tarihler), 17 (salt okunurda buton pasif ve nedeni yazar); T-50 (brandCustomerId / newCustomer,
// kapıda sıra tekrarlanmaz, slot_taken alternatifleri). Saat: Çarşamba 25 Kasım 2026, 12:00 İstanbul.
afterEach(() => {
  cleanup();
});

const createPath = '/api/v1/manage/appointments';
const haircut = '0193a8f0-0000-7000-8000-0000000000f1';

interface CreateBody {
  branchId: string;
  channel: string;
  startUtc: string;
  items: { serviceId: string; staffId: string }[];
  brandCustomerId?: string;
  newCustomer?: { fullName: string; phone: string };
  recurrence?: { frequency: string; intervalWeeks: number };
  notifyCustomer?: boolean;
  allowOutsideWorkingHours?: boolean;
  staffNote?: string;
}

function start(path = '/takvim', options: FakeApiOptions = {}) {
  const calls: Record<string, unknown[]> = {};
  renderApp(path, { calls, withServices: true, ...options });
  return calls;
}

async function openModal(options: FakeApiOptions = {}) {
  const calls = start('/takvim', options);
  await userEvent.click(await screen.findByRole('button', { name: 'Randevu ekle' }));
  const dialog = await screen.findByRole('dialog', { name: 'Yeni randevu' });
  return { calls, dialog };
}

async function pickDerya(dialog: HTMLElement) {
  await userEvent.type(within(dialog).getByRole('searchbox', { name: 'Müşteri ara' }), 'Derya');
  await userEvent.click(
    await within(dialog).findByRole('button', { name: /Derya Şahin · 0532 760 21 18/ }),
  );
}

async function pickHaircut(dialog: HTMLElement) {
  await userEvent.click(await within(dialog).findByRole('button', { name: 'Saç kesimi · 45 dk' }));
}

const bodies = (calls: Record<string, unknown[]>) => (calls[createPath] ?? []) as CreateBody[];

describe('yeni randevu (kriter 11)', () => {
  it('Derya Şahin · Saç kesimi · Elif · Çar 14:30, Telefonla, SMS açık', async () => {
    const { calls, dialog } = await openModal();
    await pickDerya(dialog);
    expect(within(dialog).getByText('Derya Şahin')).toBeTruthy();
    expect(
      within(dialog).getByRole('button', { name: 'Müşteriyi değiştir: Derya Şahin' }),
    ).toBeTruthy();
    await pickHaircut(dialog);
    expect(
      within(dialog)
        .getByRole('button', { name: 'Saç kesimi · 45 dk' })
        .getAttribute('aria-pressed'),
    ).toBe('true');
    expect(within(dialog).getByLabelText<HTMLSelectElement>('Personel').value).toBe(staffIds.elif);
    await userEvent.selectOptions(within(dialog).getByLabelText('Nasıl geldi?'), 'Telefonla');
    const slots = await within(dialog).findByRole('radiogroup', {
      name: 'Çarşamba, 25 Kasım · Elif Kaya için boş saatler',
    });
    expect(within(dialog).getByText('Çarşamba, 25 Kasım · Elif için boş saatler')).toBeTruthy();
    expect(within(slots).getByText('Sabah')).toBeTruthy();
    expect(within(slots).getByText('Öğleden sonra')).toBeTruthy();
    expect(
      within(slots).getByRole<HTMLInputElement>('radio', { name: '09:00, dolu' }).disabled,
    ).toBe(true);
    await userEvent.click(within(slots).getByRole('radio', { name: '14:30' }));
    expect(within(dialog).getByText('Çar 25 Kas · 14:30–15:15')).toBeTruthy();
    expect(within(dialog).getByText(/^Saç kesimi · Elif Kaya · 450/)).toBeTruthy();
    expect(
      within(dialog).getByRole<HTMLInputElement>('switch', { name: /Müşteriye SMS gönder/ })
        .checked,
    ).toBe(true);

    await userEvent.click(within(dialog).getByRole('button', { name: 'Randevu oluştur' }));
    expect(await screen.findByText('Randevu oluşturuldu')).toBeTruthy();
    await waitFor(() => {
      expect(screen.queryByRole('dialog', { name: 'Yeni randevu' })).toBeNull();
    });
    const [body] = bodies(calls);
    expect(body).toMatchObject({
      branchId,
      channel: 'Phone',
      brandCustomerId: customerIds.derya,
      items: [{ serviceId: haircut, staffId: staffIds.elif }],
      notifyCustomer: true,
    });
    expect(Date.parse(body?.startUtc ?? '')).toBe(Date.parse('2026-11-25T11:30:00Z'));
    expect(body?.newCustomer).toBeUndefined();
    expect(body?.recurrence).toBeUndefined();
    expect(body?.allowOutsideWorkingHours).toBeUndefined();
    expect(calls[`${createPath} Idempotency-Key`]).toHaveLength(1);
    // Takvim yenilenir: yeni blok görünür.
    expect(
      await screen.findByRole('button', { name: /^Derya Şahin, Saç kesimi, 14:30–15:15/ }),
    ).toBeTruthy();
  });

  it('20:00 çalışma saati dışı: önce onay, sonra allowOutsideWorkingHours ile eklenir', async () => {
    const { calls, dialog } = await openModal();
    await pickDerya(dialog);
    await pickHaircut(dialog);
    await within(dialog).findByRole('radiogroup');
    await userEvent.click(within(dialog).getByRole('button', { name: 'Listede olmayan saat' }));
    fireEvent.change(within(dialog).getByLabelText('Saat'), { target: { value: '20:00' } });
    expect(within(dialog).getByText('Çar 25 Kas · 20:00–20:45')).toBeTruthy();
    await userEvent.click(within(dialog).getByRole('button', { name: 'Randevu oluştur' }));

    const confirm = await screen.findByRole('dialog', { name: 'Çalışma saati dışında' });
    expect(within(confirm).getByText(/Seçilen saat çalışma saatleri dışında/)).toBeTruthy();
    await userEvent.click(within(confirm).getByRole('button', { name: 'Yine de ekle' }));
    expect(await screen.findByText('Randevu oluşturuldu')).toBeTruthy();
    const [first, second] = bodies(calls);
    expect(first?.allowOutsideWorkingHours).toBeUndefined();
    expect(second?.allowOutsideWorkingHours).toBe(true);
    expect(Date.parse(second?.startUtc ?? '')).toBe(Date.parse('2026-11-25T17:00:00Z'));
    // Gövde değiştiği için yeni Idempotency-Key (aynı anahtar + farklı gövde 422).
    const keys = calls[`${createPath} Idempotency-Key`] ?? [];
    expect(keys).toHaveLength(2);
    expect(keys[0]).not.toBe(keys[1]);
  });

  it('onay penceresinde Vazgeç: istek tekrarlanmaz, form açık kalır', async () => {
    const { calls, dialog } = await openModal();
    await pickDerya(dialog);
    await pickHaircut(dialog);
    await within(dialog).findByRole('radiogroup');
    await userEvent.click(within(dialog).getByRole('button', { name: 'Listede olmayan saat' }));
    fireEvent.change(within(dialog).getByLabelText('Saat'), { target: { value: '20:00' } });
    await userEvent.click(within(dialog).getByRole('button', { name: 'Randevu oluştur' }));
    const confirm = await screen.findByRole('dialog', { name: 'Çalışma saati dışında' });
    await userEvent.click(within(confirm).getByRole('button', { name: 'Vazgeç' }));
    const reopened = await screen.findByRole('dialog', { name: 'Yeni randevu' });
    expect(within(reopened).getByText('Derya Şahin')).toBeTruthy();
    expect(bodies(calls)).toHaveLength(1);
  });

  it('eksik alanlar: hata mesajları, istek gitmez', async () => {
    const { calls, dialog } = await openModal();
    await userEvent.click(within(dialog).getByRole('button', { name: 'Randevu oluştur' }));
    expect(within(dialog).getByText('Müşteri seçin ya da yeni müşteri ekleyin.')).toBeTruthy();
    expect(within(dialog).getByText('En az bir hizmet seçin.')).toBeTruthy();
    expect(within(dialog).getByText('Saat seçin.')).toBeTruthy();
    expect(bodies(calls)).toHaveLength(0);
  });

  it('yeni müşteri: ad + telefon newCustomer olarak gider; Kapıdan tekrarlanamaz', async () => {
    const { calls, dialog } = await openModal();
    await userEvent.type(
      within(dialog).getByRole('searchbox', { name: 'Müşteri ara' }),
      'Kaan Yıldız',
    );
    expect(await within(dialog).findByText('"Kaan Yıldız" ile kayıtlı müşteri yok.')).toBeTruthy();
    await userEvent.click(within(dialog).getByRole('button', { name: 'Yeni müşteri ekle' }));
    expect(within(dialog).getByLabelText<HTMLInputElement>('Ad soyad').value).toBe('Kaan Yıldız');
    await userEvent.type(within(dialog).getByLabelText('Cep telefonu'), '0532 111 22 33');
    await pickHaircut(dialog);
    await userEvent.click(await within(dialog).findByRole('radio', { name: '14:30' }));
    await userEvent.selectOptions(within(dialog).getByLabelText('Nasıl geldi?'), 'Kapıdan');
    const repeat = within(dialog).getByLabelText<HTMLSelectElement>('Tekrar');
    expect(repeat.disabled).toBe(true);
    expect(within(dialog).getByText('Kapıdan gelen randevu tekrarlanmaz.')).toBeTruthy();
    await userEvent.click(within(dialog).getByRole('button', { name: 'Randevu oluştur' }));
    expect(await screen.findByText('Randevu oluşturuldu')).toBeTruthy();
    const [body] = bodies(calls);
    expect(body).toMatchObject({
      channel: 'WalkIn',
      newCustomer: { fullName: 'Kaan Yıldız', phone: '0532 111 22 33' },
    });
    expect(body?.brandCustomerId).toBeUndefined();
    expect(body?.recurrence).toBeUndefined();
  });

  it('SMS kapalı: notifyCustomer false, hatırlatmaların yine gittiği yazar', async () => {
    const { calls, dialog } = await openModal();
    await pickDerya(dialog);
    await pickHaircut(dialog);
    await userEvent.click(await within(dialog).findByRole('radio', { name: '14:30' }));
    await userEvent.click(within(dialog).getByRole('switch', { name: /Müşteriye SMS gönder/ }));
    expect(within(dialog).getByText('Onay mesajı gitmez; hatırlatmalar yine gider.')).toBeTruthy();
    await userEvent.type(within(dialog).getByLabelText('Not (isteğe bağlı)'), 'Kısa kesim');
    await userEvent.click(within(dialog).getByRole('button', { name: 'Randevu oluştur' }));
    expect(await screen.findByText('Randevu oluşturuldu')).toBeTruthy();
    expect(bodies(calls)[0]).toMatchObject({ notifyCustomer: false, staffNote: 'Kısa kesim' });
  });
});

describe('tekrarlayan (kriter 13)', () => {
  it('4 haftada bir: recurrence gider, atlanan tarihler gösterilir', async () => {
    const { calls, dialog } = await openModal({
      calendar: { skippedOccurrences: ['2026-12-23', '2027-01-20'] },
    });
    await pickDerya(dialog);
    await pickHaircut(dialog);
    await userEvent.click(await within(dialog).findByRole('radio', { name: '14:30' }));
    await userEvent.selectOptions(within(dialog).getByLabelText('Tekrar'), '4 haftada bir');
    expect(
      within(dialog).getByText(
        'Her hafta seçilirse ilk 3 ayın randevuları oluşturulur, sonrası otomatik eklenir.',
      ),
    ).toBeTruthy();
    await userEvent.click(within(dialog).getByRole('button', { name: 'Randevu oluştur' }));
    expect(
      await screen.findByText('2 tarih dolu olduğu için atlandı: 23 Aralık, 20 Ocak.'),
    ).toBeTruthy();
    expect(bodies(calls)[0]?.recurrence).toEqual({ frequency: 'Weekly', intervalWeeks: 4 });
  });
});

describe('hata hâlleri', () => {
  it('409 slot_taken: "Saat az önce doldu" ve alternatif saat çipleri', async () => {
    const { dialog } = await openModal({ calendar: { createError: 'slot_taken' } });
    await pickDerya(dialog);
    await pickHaircut(dialog);
    await userEvent.click(await within(dialog).findByRole('radio', { name: '14:30' }));
    await userEvent.click(within(dialog).getByRole('button', { name: 'Randevu oluştur' }));
    expect(await within(dialog).findByText('14:30 artık boş değil.')).toBeTruthy();
    expect(within(dialog).getByText(/En yakın boş saatler: 14:45, 15:15\./)).toBeTruthy();
    await userEvent.click(within(dialog).getByRole('button', { name: '14:45 seç' }));
    expect(within(dialog).queryByText('14:30 artık boş değil.')).toBeNull();
    expect(within(dialog).getByLabelText<HTMLInputElement>('Saat').value).toBe('14:45');
    expect(within(dialog).getByText('Çar 25 Kas · 14:45–15:30')).toBeTruthy();
  });
});

describe('salt okunur abonelik (kriter 17)', () => {
  it('Randevu ekle pasif ve nedeni yazar; boş alan tıklanmaz', async () => {
    start('/takvim', {
      session: sessionFixture({ subscription: { status: 'ReadOnly', trialEndsOn: null } }),
    });
    const button = await screen.findByRole('button', { name: 'Randevu ekle' });
    expect((button as HTMLButtonElement).disabled).toBe(true);
    expect(screen.getByText('Abonelik salt okunur; yeni randevu eklenemez.')).toBeTruthy();
    expect(button.getAttribute('aria-describedby')).toBeTruthy();
    await screen.findByRole('region', { name: 'Günlük takvim' });
    await userEvent.click(screen.getByRole('button', { name: 'Randevu ekle: Elif Kaya, 14:30' }));
    expect(screen.queryByRole('dialog', { name: 'Yeni randevu' })).toBeNull();
  });

  it('yetkisi olmayan kullanıcı butonu görmez', async () => {
    start('/takvim', {
      session: sessionFixture({
        permissions: ['calendar.view', 'calendar.view.all-staff', 'appointment.update'],
      }),
    });
    await screen.findByRole('region', { name: 'Günlük takvim' });
    expect(screen.queryByRole('button', { name: 'Randevu ekle' })).toBeNull();
  });
});

describe('ön doldurma', () => {
  it('takvimde boş alana tıklama: personel, gün ve saat gelir', async () => {
    const calls = start();
    await userEvent.click(
      await screen.findByRole('button', { name: 'Randevu ekle: Mert Demir, 14:30' }),
    );
    const dialog = await screen.findByRole('dialog', { name: 'Yeni randevu' });
    await pickDerya(dialog);
    await pickHaircut(dialog);
    expect(within(dialog).getByLabelText<HTMLSelectElement>('Personel').value).toBe(staffIds.mert);
    const slot = await within(dialog).findByRole('radio', { name: '14:30' });
    expect((slot as HTMLInputElement).checked).toBe(true);
    await userEvent.click(within(dialog).getByRole('button', { name: 'Randevu oluştur' }));
    expect(await screen.findByText('Randevu oluşturuldu')).toBeTruthy();
    expect(bodies(calls)[0]?.items).toEqual([{ serviceId: haircut, staffId: staffIds.mert }]);
  });

  it('müşteri kartında Randevu ver: müşteri seçili açılır', async () => {
    start(`/musteriler/${customerIds.derya}`);
    await userEvent.click(await screen.findByRole('button', { name: 'Randevu ver' }));
    const dialog = await screen.findByRole('dialog', { name: 'Yeni randevu' });
    expect(
      within(dialog).getByRole('button', { name: 'Müşteriyi değiştir: Derya Şahin' }),
    ).toBeTruthy();
    expect(within(dialog).getByText(/0532 760 21 18 · 6 ziyaret · son 21 Ekim/)).toBeTruthy();
    expect(within(dialog).queryByRole('searchbox', { name: 'Müşteri ara' })).toBeNull();
  });
});
