import { personelRenkleri } from '@omvila/design-tokens';
import { cleanup, screen, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { afterEach, describe, expect, it } from 'vitest';
import { branchId, sessionFixture, staffPermissions } from '../../test/fake-api';
import { renderApp } from '../../test/render-app';

// Kural: dilim-01 kabul kriteri 10 (Deniz Kara telefonla eklenir, davet), 11 (Keratin 75 dk özel, ek ücret),
// 12 (Elif'e 2–4 Aralık yıllık izin; etkilenen randevu yok), § Alan kuralları (Staff adımı: "tek başıma çalışıyorum"),
// salon-paneli/Personel, PersonelBos, PersonelEkle, IzinEkle.
afterEach(cleanup);

const elif = '0193a8f0-0000-7000-8000-0000000000c1';

describe('ekip boşken', () => {
  it('sahibin satırı ve davet; "tek başıma çalışıyorum" kurulum adımını bildirir', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/personel', { calls, onlyOwner: true });
    expect(await screen.findByRole('heading', { name: 'Ekibinizi ekleyin' })).toBeTruthy();
    expect(screen.getByText('1 kişi · Merkez')).toBeTruthy();
    const table = screen.getByRole('table', { name: 'Personel' });
    expect(within(table).getByText('(siz)')).toBeTruthy();
    expect(within(table).getByText('İşletme sahibi')).toBeTruthy();
    await userEvent.click(screen.getByRole('button', { name: 'Tek başıma çalışıyorum' }));
    expect(await screen.findByText('Kurulumda ekip adımı tamamlandı')).toBeTruthy();
    expect(calls['/api/v1/manage/onboarding/events']).toEqual([{ event: 'SoloConfirmed' }]);
  });

  it('personel izni olmayan kullanıcı 403 görür', async () => {
    renderApp('/personel', {
      session: sessionFixture({
        permissions: staffPermissions,
        membership: { role: 'Staff', branchIds: [], staffId: null },
      }),
    });
    expect(
      await screen.findByRole('heading', { level: 1, name: 'Bu sayfayı görme yetkiniz yok' }),
    ).toBeTruthy();
  });
});

describe('ekip', () => {
  it('kartlar; seçili kişinin saatleri, verdiği hizmetler (özel süre, ek ücret) ve primi', async () => {
    renderApp('/personel', { withServices: true });
    const cards = await screen.findByRole('list', { name: 'Personel' });
    expect(within(cards).getAllByRole('button')).toHaveLength(3);
    expect(
      within(cards)
        .getByRole('button', { name: /Elif Kaya/ })
        .getAttribute('aria-pressed'),
    ).toBe('true');
    expect(
      await screen.findByRole('heading', { name: 'Elif Kaya · çalışma saatleri' }),
    ).toBeTruthy();
    expect(screen.getByLabelText<HTMLInputElement>('Salı 2. aralık başlangıç').value).toBe('13:45');
    const services = screen.getByRole('region', { name: 'Verdiği hizmetler' });
    expect(within(services).getByText('1 sa 15 dk (özel)')).toBeTruthy();
    expect(within(services).getByText('+200 ₺')).toBeTruthy();
    expect(screen.getByText('%24')).toBeTruthy();
  });

  it('saatler kişinin şube saatine sürümle kaydedilir', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/personel', { calls, withServices: true });
    await screen.findByRole('heading', { name: 'Elif Kaya · çalışma saatleri' });
    await userEvent.click(screen.getByRole('switch', { name: 'Cumartesi açık' }));
    await userEvent.click(screen.getByRole('button', { name: 'Saatleri kaydet' }));
    expect(await screen.findByText('Saatler kaydedildi')).toBeTruthy();
    const path = `/api/v1/manage/staff/${elif}/branches/${branchId}/working-hours`;
    expect(calls[`PUT ${path} If-Match`]).toEqual(['6']);
    expect(
      (calls[path]?.[0] as { days: { dayOfWeek: number }[] }).days.map((d) => d.dayOfWeek),
    ).toEqual([2, 3, 4, 5, 6]);
  });

  it('verdiği hizmetler: özel süre ve ek ücret kaydedilir (kabul kriteri 11)', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/personel', { calls, withServices: true });
    const panel = await screen.findByRole('region', { name: 'Verdiği hizmetler' });
    await userEvent.click(within(panel).getByRole('button', { name: 'Düzenle' }));
    const dialog = await screen.findByRole('dialog', { name: 'Verdiği hizmetler' });
    expect(
      within(dialog).getByRole<HTMLInputElement>('textbox', { name: 'Keratin özel süre' }).value,
    ).toBe('75');
    await userEvent.type(
      within(dialog).getByRole('textbox', { name: 'Saç kesimi ek ücret' }),
      '300',
    );
    await userEvent.click(within(dialog).getByRole('button', { name: 'Hizmetleri kaydet' }));
    expect(await screen.findByText('Hizmetler kaydedildi')).toBeTruthy();
    const path = `/api/v1/manage/staff/${elif}/services`;
    expect(calls[`PUT ${path} If-Match`]).toEqual(['7']);
    expect(calls[path]?.[0]).toEqual({
      items: [
        {
          serviceId: '0193a8f0-0000-7000-8000-0000000000f1',
          durationOverrideMinutes: null,
          surcharge: { amountMinor: 30000, currency: 'TRY' },
        },
        {
          serviceId: '0193a8f0-0000-7000-8000-0000000000f2',
          durationOverrideMinutes: 75,
          surcharge: { amountMinor: 20000, currency: 'TRY' },
        },
      ],
    });
  });

  it('personel ekle: telefonla davet (kabul kriteri 10)', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/personel', { calls, withServices: true });
    await screen.findByRole('list', { name: 'Personel' });
    await userEvent.click(screen.getByRole('button', { name: 'Personel ekle' }));
    const drawer = await screen.findByRole('dialog', { name: 'Personel ekle' });
    await userEvent.type(within(drawer).getByRole('textbox', { name: 'Ad soyad' }), 'Deniz Kara');
    await userEvent.type(
      within(drawer).getByRole('textbox', { name: 'Cep telefonu' }),
      '0538 274 19 05',
    );
    await userEvent.type(within(drawer).getByRole('textbox', { name: 'Unvan' }), 'Renk uzmanı');
    await userEvent.click(within(drawer).getByRole('checkbox', { name: /Saç kesimi/ }));
    expect(within(drawer).getByRole('switch', { name: /Davet SMS'i gönder/ })).toBeTruthy();
    await userEvent.click(within(drawer).getByRole('button', { name: 'Personeli ekle' }));
    expect(await screen.findByText('Deniz Kara eklendi')).toBeTruthy();
    expect(screen.getByText("Davet SMS'i gönderildi.")).toBeTruthy();
    expect(calls['/api/v1/manage/staff']?.[0]).toEqual({
      fullName: 'Deniz Kara',
      phone: '0538 274 19 05',
      title: 'Renk uzmanı',
      role: 'Staff',
      calendarColor: personelRenkleri[0],
      branchIds: [branchId],
      services: [{ serviceId: '0193a8f0-0000-7000-8000-0000000000f1' }],
      copyBranchWorkingHours: true,
      serviceCommissionPercent: 0,
      isBookableOnline: true,
      sendInvite: true,
    });
  });

  it('geçersiz telefon gönderilmez', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/personel', { calls, withServices: true });
    await screen.findByRole('list', { name: 'Personel' });
    await userEvent.click(screen.getByRole('button', { name: 'Personel ekle' }));
    const drawer = await screen.findByRole('dialog', { name: 'Personel ekle' });
    await userEvent.type(within(drawer).getByRole('textbox', { name: 'Ad soyad' }), 'Deniz Kara');
    await userEvent.type(
      within(drawer).getByRole('textbox', { name: 'Cep telefonu' }),
      '0216 330 40 40',
    );
    await userEvent.click(within(drawer).getByRole('button', { name: 'Personeli ekle' }));
    expect(
      await within(drawer).findByText('Cep telefonu 10 haneli olmalı: 0538 274 19 05'),
    ).toBeTruthy();
    expect(calls['/api/v1/manage/staff']).toBeUndefined();
  });
});

describe('izin', () => {
  it("Elif'e 2–4 Aralık yıllık izin (kabul kriteri 12)", async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/personel', { calls, withServices: true });
    await screen.findByRole('list', { name: 'Personel' });
    await userEvent.click(screen.getByRole('button', { name: 'İzin ve tatil' }));
    const dialog = await screen.findByRole('dialog', { name: 'İzin veya tatil ekle' });
    await userEvent.type(within(dialog).getByLabelText('Başlangıç'), '2026-12-02');
    await userEvent.type(within(dialog).getByLabelText('Bitiş'), '2026-12-04');
    await userEvent.click(within(dialog).getByRole('button', { name: 'İzni kaydet' }));
    expect(await screen.findByText('İzin eklendi')).toBeTruthy();
    expect(calls[`/api/v1/manage/staff/${elif}/time-off`]?.[0]).toEqual({
      type: 'Leave',
      isAllDay: true,
      fromDate: '2026-12-02',
      toDate: '2026-12-04',
      note: null,
    });
  });

  it('saatli izin şube saatinden UTC olur; etkilenen randevular şeritte sayılır', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/personel', { calls, withServices: true, affectedAppointments: 5 });
    await screen.findByRole('list', { name: 'Personel' });
    await userEvent.click(screen.getByRole('button', { name: 'İzin ve tatil' }));
    const dialog = await screen.findByRole('dialog', { name: 'İzin veya tatil ekle' });
    await userEvent.click(within(dialog).getByRole('switch', { name: 'Tüm gün' }));
    await userEvent.type(within(dialog).getByLabelText('Tarih'), '2026-12-02');
    await userEvent.type(within(dialog).getByLabelText('Başlangıç saati'), '14:00');
    await userEvent.type(within(dialog).getByLabelText('Bitiş saati'), '16:00');
    await userEvent.click(within(dialog).getByRole('button', { name: 'İzni kaydet' }));
    expect(
      await within(dialog).findByText('Bu tarihlerde Elif Kaya 5 randevusu var.'),
    ).toBeTruthy();
    expect(calls[`/api/v1/manage/staff/${elif}/time-off`]?.[0]).toMatchObject({
      isAllDay: false,
      startUtc: '2026-12-02T11:00:00.000Z',
      endUtc: '2026-12-02T13:00:00.000Z',
    });
  });
});
