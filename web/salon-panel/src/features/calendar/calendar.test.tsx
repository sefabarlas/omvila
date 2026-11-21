import { act, cleanup, fireEvent, screen, waitFor, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { afterEach, describe, expect, it, vi } from 'vitest';
import { sessionFixture, staffPermissions } from '../../test/fake-api';
import type { FakeApiOptions } from '../../test/fake-api';
import { closedSunday, staffIds } from '../../test/fake-calendar';
import { renderApp } from '../../test/render-app';

// Kural: dilim-02 kabul kriterleri 3 (bekleme şeridi), 12 (sürükle-bırak, 409 geri dönüş), 13 (tekrarlayan iptal),
// 14 (Geldi, Geldiyse geri al), 16 (kapalı gün, "Yalnızca bu gün için aç"), 17 (salt okunur şerit); roller-yetkiler.md
// (personel yalnız kendi kolonu, Takvimim). Saat: Çarşamba 25 Kasım 2026, 12:00 İstanbul.
afterEach(() => {
  cleanup();
  vi.restoreAllMocks();
});

const blockName = (name: string) => screen.findByRole('button', { name: new RegExp(`^${name}`) });

function openCalendar(path = '/takvim', options: FakeApiOptions = {}) {
  const calls: Record<string, unknown[]> = {};
  const result = renderApp(path, { calls, ...options });
  return { ...result, calls };
}

describe('takvim gün görünümü', () => {
  it('personel kolonları, bloklar, bekleme şeridi ve özet blokları', async () => {
    openCalendar();
    expect(await screen.findByRole('heading', { level: 1, name: 'Takvim' })).toBeTruthy();
    expect(screen.getByText('Çarşamba, 25 Kasım · Merkez')).toBeTruthy();
    const grid = await screen.findByRole('region', { name: 'Günlük takvim' });
    for (const name of ['Elif Kaya', 'Mert Demir', 'Zeynep Aksoy', 'Can Yücel']) {
      expect(within(grid).getByText(name)).toBeTruthy();
    }

    expect(within(grid).getByText('3 randevu · bekleyen var')).toBeTruthy();
    const selin = await blockName('Selin Öztürk, Boya, 10:30–12:30, Geldi');
    expect(within(selin).getByTestId('bekleme-seridi').textContent).toBe('Bekleme · müsait');
    expect(
      await blockName('Derya Şahin, Saç kesimi, 13:00–13:45, Onaylandı, tekrarlayan'),
    ).toBeTruthy();

    const summary = screen.getByRole('group', { name: 'Bugünün özeti' });
    expect(within(summary).getByText('Sıradaki · 1 sa sonra')).toBeTruthy();
    expect(within(summary).getByText('13:00')).toBeTruthy();
    expect(within(summary).getByText('Derya Şahin · Saç kesimi · Elif')).toBeTruthy();
    expect(within(summary).getByText('Onay bekliyor')).toBeTruthy();
    expect(within(summary).getByText('ilki 1 sa 35 dk içinde iptal olur')).toBeTruthy();
    expect(within(summary).getByText('7')).toBeTruthy();
    expect(within(summary).getByText('randevu · 1 tamamlandı')).toBeTruthy();
    // Kapıda sıra ve ciro D5.
    expect(screen.getByRole('button', { name: 'Randevu ekle' })).toBeTruthy();
    expect(screen.queryByText(/Kapıda sırada/i)).toBeNull();
  });

  it('sonraki gün ve Bugün; adres tarihi taşır', async () => {
    const { router } = openCalendar();
    await screen.findByRole('region', { name: 'Günlük takvim' });
    await userEvent.click(screen.getByRole('button', { name: 'Sonraki gün' }));
    expect(
      await screen.findByRole('heading', { level: 2, name: 'Perşembe, 26 Kasım' }),
    ).toBeTruthy();
    expect(router.state.location.search).toEqual({ tarih: '2026-11-26' });
    await userEvent.click(screen.getByRole('button', { name: 'Bugün' }));
    expect(
      await screen.findByRole('heading', { level: 2, name: 'Çarşamba, 25 Kasım' }),
    ).toBeTruthy();
  });

  it('hafta görünümü: kendi kolonu, kapalı pazar gösterilmez', async () => {
    openCalendar('/takvim?gorunum=hafta');
    const week = await screen.findByRole('region', { name: 'Elif Kaya haftalık takvim' });
    expect(screen.getByRole('heading', { level: 2, name: '23–28 Kasım' })).toBeTruthy();
    expect(within(week).getByText('Çarşamba, 25 Kasım, bugün')).toBeTruthy();
    expect(within(week).queryByText(/Pazar, 29 Kasım/)).toBeNull();
    expect(screen.getByText('23–28 Kasım · Elif Kaya · Merkez')).toBeTruthy();
  });

  it('salt okunur abonelikte kırmızı şerit; takvim açık (kriter 17)', async () => {
    openCalendar('/takvim', {
      session: sessionFixture({ subscription: { status: 'ReadOnly', trialEndsOn: null } }),
    });
    expect(await screen.findByText('Online randevu kapalı.')).toBeTruthy();
    expect(await screen.findByRole('region', { name: 'Günlük takvim' })).toBeTruthy();
  });
});

describe('kapalı gün (kriter 16)', () => {
  it('"Kapalı gün" ve "Yalnızca bu gün için aç" → ExtraOpening; gün açılır', async () => {
    const { calls } = openCalendar(`/takvim?tarih=${closedSunday}`);
    expect(await screen.findByRole('heading', { name: 'Kapalı gün' })).toBeTruthy();
    expect(screen.getByText('Salon bu gün kapalı.')).toBeTruthy();
    await userEvent.click(screen.getByRole('button', { name: 'Yalnızca bu gün için aç' }));
    const dialog = await screen.findByRole('dialog', { name: 'Yalnızca bu gün için aç' });
    expect(within(dialog).getByLabelText<HTMLInputElement>('Açılış').value).toBe('09:00');
    await userEvent.click(within(dialog).getByRole('button', { name: 'Günü aç' }));
    expect(await screen.findByRole('region', { name: 'Günlük takvim' })).toBeTruthy();
    expect(calls['/api/v1/manage/branches/0193a8f0-0000-7000-8000-0000000000b1/overrides']).toEqual(
      [
        {
          fromDate: closedSunday,
          kind: 'ExtraOpening',
          isClosed: false,
          intervals: [{ start: '09:00', end: '19:00' }],
          note: null,
        },
      ],
    );
  });

  it('ayar yetkisi olmayan kapalı günü açamaz', async () => {
    openCalendar(`/takvim?tarih=${closedSunday}`, {
      session: sessionFixture({
        permissions: staffPermissions,
        membership: { role: 'Staff', branchIds: [], staffId: staffIds.mert },
      }),
    });
    expect(await screen.findByRole('heading', { name: 'Kapalı gün' })).toBeTruthy();
    expect(screen.getByText('Bu güne randevu alınmaz.')).toBeTruthy();
    expect(screen.queryByRole('button', { name: 'Yalnızca bu gün için aç' })).toBeNull();
  });
});

describe('randevu yan paneli', () => {
  it('Geldi: If-Match ile işaretlenir (kriter 14)', async () => {
    const { calls } = openCalendar();
    await userEvent.click(await blockName('Derya Şahin'));
    const drawer = await screen.findByRole('dialog', { name: 'Randevu · Telefonla' });
    expect(await within(drawer).findByText('13:00–13:45')).toBeTruthy();
    expect(within(drawer).getByText('Bugün · Tekrarlayan')).toBeTruthy();
    expect(within(drawer).getByText('Saç kesimi · Elif Kaya · 450 ₺')).toBeTruthy();
    expect(within(drawer).getByText('0532 760 21 18 · 6 ziyaret')).toBeTruthy();
    expect(within(drawer).getByText('Gelmezse 13:15\'te otomatik "Gelmedi" olur.')).toBeTruthy();
    expect(within(drawer).queryByRole('button', { name: /Tamamla/ })).toBeNull();
    await userEvent.click(within(drawer).getByRole('button', { name: 'Geldi' }));
    await waitFor(() => {
      expect(within(drawer).queryByRole('button', { name: 'Geldi' })).toBeNull();
    });
    expect(
      calls[
        'POST /api/v1/manage/appointments/0193a8f0-0000-7000-8000-00000000a003/check-in If-Match'
      ],
    ).toEqual(['1']);
    expect(await blockName('Derya Şahin, Saç kesimi, 13:00–13:45, Geldi')).toBeTruthy();
  });

  it('gelmeyen için "Geldiyse geri al" (kriter 14)', async () => {
    const { calls } = openCalendar();
    await userEvent.click(await blockName('Ece Koç'));
    const drawer = await screen.findByRole('dialog');
    await userEvent.click(await within(drawer).findByRole('button', { name: 'Geldiyse geri al' }));
    expect(await blockName('Ece Koç, Kaş ve kirpik, 10:30–11:00, Geldi')).toBeTruthy();
    expect(
      calls[
        'POST /api/v1/manage/appointments/0193a8f0-0000-7000-8000-00000000a004/undo-no-show If-Match'
      ],
    ).toEqual(['1']);
  });

  it('412: "Kayıt çakışması" şeridi', async () => {
    openCalendar('/takvim', { calendar: { actionConflict: true } });
    await userEvent.click(await blockName('Derya Şahin'));
    const drawer = await screen.findByRole('dialog');
    await userEvent.click(await within(drawer).findByRole('button', { name: 'Geldi' }));
    expect(await within(drawer).findByText('Kayıt çakışması')).toBeTruthy();
  });

  it('tekrarlayan iptal: "Bu ve sonraki randevular", müşteriye bildirmeden (kriter 13)', async () => {
    const { calls } = openCalendar();
    await userEvent.click(await blockName('Derya Şahin'));
    const drawer = await screen.findByRole('dialog');
    await userEvent.click(
      await within(drawer).findByRole('button', { name: /Randevuyu iptal et/ }),
    );
    const dialog = await screen.findByRole('dialog', { name: 'Tekrarlayan randevuyu iptal et' });
    expect(
      within(dialog).getByText('Derya Şahin · Saç kesimi · Çarşamba 13:00 · Elif Kaya'),
    ).toBeTruthy();
    expect(within(dialog).getByLabelText<HTMLInputElement>(/Yalnızca bu randevu/).checked).toBe(
      true,
    );
    await userEvent.click(within(dialog).getByLabelText(/Bu ve sonraki randevular/));
    await userEvent.click(within(dialog).getByLabelText(/Müşteriye bildir/));
    await userEvent.click(within(dialog).getByRole('button', { name: 'Randevuyu iptal et' }));
    await waitFor(() => {
      expect(screen.queryByRole('button', { name: /^Derya Şahin/ })).toBeNull();
    });
    expect(
      calls['/api/v1/manage/appointments/0193a8f0-0000-7000-8000-00000000a003/cancel'],
    ).toEqual([{ reason: 'CustomerRequested', notifyCustomer: false, scope: 'ThisAndFollowing' }]);
  });

  it('tekrarsız iptalde kapsam yok', async () => {
    const { calls } = openCalendar();
    await userEvent.click(await blockName('Aslı Kurt'));
    const drawer = await screen.findByRole('dialog');
    await userEvent.click(
      await within(drawer).findByRole('button', { name: /Randevuyu iptal et/ }),
    );
    const dialog = await screen.findByRole('dialog', { name: 'Randevuyu iptal et' });
    expect(within(dialog).queryByLabelText(/Yalnızca bu randevu/)).toBeNull();
    await userEvent.selectOptions(
      within(dialog).getByLabelText('İptal nedeni'),
      'StaffUnavailable',
    );
    await userEvent.click(within(dialog).getByRole('button', { name: 'Randevuyu iptal et' }));
    await waitFor(() => {
      expect(
        calls['/api/v1/manage/appointments/0193a8f0-0000-7000-8000-00000000a007/cancel'],
      ).toEqual([{ reason: 'StaffUnavailable', notifyCustomer: true }]);
    });
  });
});

describe('sürükle-bırak (kriter 12)', () => {
  // jsdom ölçü yapmaz: kolonlar 200 px genişlik, ızgara 640 px (09:00–19:00); ui-web takvim testindeki düzen.
  function mockLayout() {
    const columns = [...document.querySelectorAll<HTMLElement>('[data-column]')];
    columns.forEach((column, index) => {
      const rect = { left: 56 + index * 200, top: 0, width: 200, height: 640 };
      column.getBoundingClientRect = () => ({
        ...rect,
        x: rect.left,
        y: rect.top,
        right: rect.left + rect.width,
        bottom: rect.top + rect.height,
        toJSON: () => rect,
      });
    });
    const rect = { left: 60, top: 256, width: 192, height: 48 };
    vi.spyOn(HTMLElement.prototype, 'getBoundingClientRect').mockImplementation(() => ({
      ...rect,
      x: rect.left,
      y: rect.top,
      right: rect.left + rect.width,
      bottom: rect.top + rect.height,
      toJSON: () => rect,
    }));
  }

  async function press(code: string) {
    await act(async () => {
      fireEvent.keyDown(document.activeElement ?? document.body, { code, key: code });
      await new Promise((resolve) => setTimeout(resolve, 0));
    });
  }

  async function dragDeryaToZeynep() {
    const derya = await blockName('Derya Şahin');
    mockLayout();
    derya.focus();
    await press('Space');
    await press('ArrowRight');
    await press('ArrowRight');
    for (let i = 0; i < 4; i += 1) {
      await press('ArrowDown');
    }
  }

  const column = (staffId: string) =>
    document.querySelector<HTMLElement>(`[data-column="${staffId}"]`) ?? document.body;

  it('"Zeynep Aksoy\'a taşı" ipucu, bırakınca If-Match ile kaydedilir', async () => {
    const { calls } = openCalendar();
    await dragDeryaToZeynep();
    expect(
      screen.getAllByText("Zeynep Aksoy'a taşı · 14:00–14:45 · müşteriye SMS gider").length,
    ).toBeGreaterThan(0);
    await press('Space');
    await waitFor(() => {
      expect(
        within(column(staffIds.zeynep)).getByRole('button', { name: /^Derya Şahin/ }),
      ).toBeTruthy();
    });
    expect(
      calls['/api/v1/manage/appointments/0193a8f0-0000-7000-8000-00000000a003/reschedule'],
    ).toEqual([
      {
        startUtc: '2026-11-25T11:00:00.000Z',
        items: [{ itemId: '0193a8f0-0000-7000-8000-00000000b003', staffId: staffIds.zeynep }],
        scope: 'ThisOnly',
      },
    ]);
    expect(
      calls[
        'POST /api/v1/manage/appointments/0193a8f0-0000-7000-8000-00000000a003/reschedule If-Match'
      ],
    ).toEqual(['1']);
  });

  it('409 slot_taken: blok eski yerine döner, "Saat dolu" uyarısı', async () => {
    openCalendar('/takvim', { calendar: { rescheduleError: 'slot_taken' } });
    await dragDeryaToZeynep();
    await press('Space');
    expect(await screen.findByText('Saat dolu: 14:00 artık boş değil.')).toBeTruthy();
    const derya = within(column(staffIds.elif)).getByRole('button', { name: /^Derya Şahin/ });
    expect(derya.style.getPropertyValue('--blok-ust')).toBe('4');
    expect(
      within(column(staffIds.zeynep)).queryByRole('button', { name: /^Derya Şahin/ }),
    ).toBeNull();
  });

  it('çalışma saati dışı: onaylanınca allowOutsideWorkingHours ile tekrar', async () => {
    const { calls } = openCalendar('/takvim', {
      calendar: { rescheduleError: 'outside_working_hours' },
    });
    await dragDeryaToZeynep();
    await press('Space');
    const dialog = await screen.findByRole('dialog', { name: 'Çalışma saati dışında' });
    vi.restoreAllMocks();
    await userEvent.click(within(dialog).getByRole('button', { name: 'Yine de taşı' }));
    await waitFor(() => {
      expect(
        calls['/api/v1/manage/appointments/0193a8f0-0000-7000-8000-00000000a003/reschedule'],
      ).toHaveLength(2);
    });
    expect(
      (
        calls[
          '/api/v1/manage/appointments/0193a8f0-0000-7000-8000-00000000a003/reschedule'
        ]?.[1] as {
          allowOutsideWorkingHours?: boolean;
        }
      ).allowOutsideWorkingHours,
    ).toBe(true);
  });

  it('412: "Kayıt çakışması · Yenile"', async () => {
    openCalendar('/takvim', { calendar: { rescheduleError: 'concurrency_conflict' } });
    await dragDeryaToZeynep();
    await press('Space');
    expect(await screen.findByText('Kayıt çakışması')).toBeTruthy();
    expect(screen.getByRole('button', { name: 'Yenile' })).toBeTruthy();
  });
});

describe('personel rolü', () => {
  it('Takvimim: yalnız kendi kolonu, onay bekleyen bloğu yok', async () => {
    renderApp('/takvim', {
      session: sessionFixture({
        user: {
          id: '0193a8f0-0000-7000-8000-000000000003',
          fullName: 'Mert Demir',
          phoneMasked: '',
        },
        permissions: staffPermissions,
        membership: { role: 'Staff', branchIds: [], staffId: staffIds.mert },
        brand: { ...sessionFixture().brand, status: 'Active' },
      }),
    });
    expect(await screen.findByRole('heading', { level: 1, name: 'Takvimim' })).toBeTruthy();
    const grid = await screen.findByRole('region', { name: 'Günlük takvim' });
    expect(within(grid).getByText('Mert Demir')).toBeTruthy();
    expect(within(grid).queryByText('Elif Kaya')).toBeNull();
    expect(document.querySelectorAll('[data-column]')).toHaveLength(1);
    const nav = screen.getByRole('navigation', { name: 'Ana menü' });
    expect(within(nav).getByRole('link', { name: 'Takvimim' }).getAttribute('aria-current')).toBe(
      'page',
    );
    expect(screen.queryByText('Onay bekliyor')).toBeNull();
  });
});
