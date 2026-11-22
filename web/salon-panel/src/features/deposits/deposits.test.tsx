import { cleanup, screen, waitFor, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { afterEach, describe, expect, it } from 'vitest';
import { ownerPermissions, sessionFixture, staffPermissions } from '../../test/fake-api';
import type { FakeApiOptions } from '../../test/fake-api';
import { depositAppointmentIds } from '../../test/fake-calendar';
import { depositNotificationTarget } from '../../test/fake-notifications';
import { renderApp } from '../../test/render-app';
import { notificationRoute } from '../notifications/notification-view';

// Kural: dilim-04 kabul kriterleri 4 (panelde "Havale bildirildi"), 5 ("Kapora geldi" → Onaylandı), 7 ("Kaporasız
// onayla" → waiveDeposit); V5 (personel kapora işaretlemez), V6 (Hatırlat bir kez), V7 (iade: tutar ≤ alınan, tek
// kez); sözleşme manageMarkDepositReceived / manageRemindDeposit / manageMarkDepositRefunded (If-Match,
// Idempotency-Key, 409 deposit_reminder_already_sent / insufficient_credits); acik-sorular.md T-72 (3), T-84.
// Saat: Çarşamba 25 Kasım 2026 12:00 İstanbul. Gizem Arslan 14:30 (Awaiting, süre 13:20), Buse Kaya 15:30
// (CustomerReported 11:52, süre 13:50), Nur Ateş (Received 150 ₺), Kerem Aydın (Refunded 100 ₺), Hande Er (Waived).
afterEach(() => {
  cleanup();
  localStorage.clear();
});

const path = (id: string) => `/api/v1/manage/appointments/${id}`;
const activeBrand = { ...sessionFixture().brand, status: 'Active' as const };
const owner = sessionFixture({ brand: activeBrand });

async function openFromCalendar(name: RegExp, options: FakeApiOptions = {}) {
  const calls: Record<string, unknown[]> = {};
  renderApp('/takvim', {
    calls,
    session: owner,
    ...options,
    calendar: { deposits: true, ...options.calendar },
  });
  await userEvent.click(await screen.findByRole('button', { name }));
  const drawer = await screen.findByRole('dialog');
  await within(drawer).findByRole('region', { name: 'Randevu durumu' });
  return { calls, drawer };
}

const rowValue = (drawer: HTMLElement, label: string) =>
  within(drawer).getByText(label).nextElementSibling?.textContent;

/** Bento bloğu: etiketin kabı (değer ve alt satır aynı kapta). */
function bentoBlock(label: HTMLElement): HTMLElement {
  const block = label.parentElement;
  if (!block) {
    throw new Error('Bento bloğu bulunamadı');
  }

  return block;
}

describe('takvim: kapora rozeti ve bento', () => {
  it('kapora bekleyen blok "Kapora bekleniyor"; havale bildirildiyse işaret; bento kapora bekleyeni saymaz', async () => {
    renderApp('/takvim', { session: owner, calendar: { deposits: true } });
    // Bloklar aria-label'lı düğmelerdir. Izgarada her boş saat de bir düğme ("Randevu ekle: …") olduğundan
    // getByRole + name bütün düğmelerin erişilebilir adını hesaplar ve yavaş CI'da 5 sn'yi aşıyordu; aynı
    // etiket araması etiketten okunur, düğme olduğu ayrıca denetlenir (beklenti aynı).
    const gizem = await screen.findByLabelText('Gizem Arslan, Fön, 14:30–15:00, Kapora bekleniyor');
    expect(gizem.tagName).toBe('BUTTON');
    const buse = screen.getByLabelText(
      'Buse Kaya, Keratin, 15:30–16:30, Kapora bekleniyor, Havale bildirildi',
    );
    expect(buse.tagName).toBe('BUTTON');
    expect(buse.dataset.status).toBe('kapora');
    expect(within(buse).getByText('Havale bildirildi')).toBeTruthy();
    // Alınmış ve kaporasız onaylanmış randevular düz "Onaylandı".
    expect(screen.getByLabelText('Nur Ateş, Fön, 17:00–17:45, Onaylandı').tagName).toBe('BUTTON');

    // İki kapora bekleyen + Pınar: "Onay bekliyor" yalnız Pınar'ı sayar (T-72 (3)).
    const summary = screen.getByLabelText('Bugünün özeti');
    expect(summary.getAttribute('role')).toBe('group');
    const pendingBlock = bentoBlock(within(summary).getByText('Onay bekliyor'));
    expect(within(pendingBlock).getByText('1')).toBeTruthy();
    expect(within(pendingBlock).queryByText('3')).toBeNull();
    expect(within(summary).getByText('ilki 1 sa 35 dk içinde iptal olur')).toBeTruthy();
  });
});

describe('yan panel: kapora bekleyen randevu', () => {
  it('kapora bölümü; düz "Onayla" yok; "Kapora geldi" If-Match ve Idempotency-Key ile gider (KK5)', async () => {
    const { calls, drawer } = await openFromCalendar(/^Gizem Arslan/);
    const section = within(drawer).getByRole('region', { name: 'Kapora' });
    expect(rowValue(section, 'Tutar')).toBe('150 ₺');
    expect(rowValue(section, 'Açıklama kodu')).toBe('OMV-7K2X');
    expect(rowValue(section, 'Müşteri bildirimi')).toBe('Yok');
    expect(rowValue(section, 'Kalan süre')).toBe('1 sa 20 dk');
    expect(
      within(section).getByText(
        'Banka uygulamanızda açıklama kodunu arayın. İşaretleyince randevu kesinleşir ve müşteriye SMS gider.',
      ),
    ).toBeTruthy();
    expect(within(drawer).getAllByText('Kapora bekleniyor').length).toBeGreaterThan(0);
    expect(within(drawer).queryByRole('button', { name: 'Onayla' })).toBeNull();
    expect(within(drawer).getByRole('button', { name: 'Reddet' })).toBeTruthy();

    await userEvent.click(within(drawer).getByRole('button', { name: 'Kapora geldi' }));
    expect(await screen.findByText('Kapora geldi olarak işaretlendi')).toBeTruthy();
    const received = path(depositAppointmentIds.gizem) + '/deposit/mark-received';
    expect(calls[`POST ${received} If-Match`]).toEqual(['1']);
    expect(calls[`${received} Idempotency-Key`]).toHaveLength(1);
    await waitFor(() => {
      expect(within(drawer).queryByRole('button', { name: 'Kapora geldi' })).toBeNull();
    });
    expect(within(drawer).getByRole('region', { name: 'Randevu durumu' }).textContent).toContain(
      'Onaylandı',
    );
    expect(within(drawer).getByText('Kapora 150 ₺ · havale ile alındı')).toBeTruthy();
    expect(within(drawer).getByText('Kapora geldi')).toBeTruthy();
    expect(
      await screen.findByRole('button', { name: /^Gizem Arslan, Fön, 14:30–15:00, Onaylandı/ }),
    ).toBeTruthy();
  });

  it('müşteri bildirdiyse "Havale bildirildi · kontrol edin" ve bildirim saati (KK4)', async () => {
    const { drawer } = await openFromCalendar(/^Buse Kaya/);
    expect(within(drawer).getByText('Havale bildirildi · kontrol edin')).toBeTruthy();
    expect(within(drawer).getByText("Müşteri havaleyi 11:52'de yaptığını bildirdi.")).toBeTruthy();
    const section = within(drawer).getByRole('region', { name: 'Kapora' });
    expect(rowValue(section, 'Müşteri bildirimi')).toBe('“Havaleyi yaptım” · 11:52');
    expect(rowValue(section, 'Kalan süre')).toBe('1 sa 50 dk');
    expect(within(drawer).getByText('Müşteri kaporayı gönderdiğini bildirdi')).toBeTruthy();
  });

  it('Hatırlat: bir kez gider, sonra pasif "Hatırlatıldı · 12:00" (V6)', async () => {
    const { calls, drawer } = await openFromCalendar(/^Gizem Arslan/);
    await userEvent.click(within(drawer).getByRole('button', { name: 'Hatırlat' }));
    expect(await screen.findByText('Hatırlatma gönderildi')).toBeTruthy();
    expect(calls[`${path(depositAppointmentIds.gizem)}/deposit/remind`]).toEqual([null]);
    const sent = await within(drawer).findByRole('button', { name: 'Hatırlatıldı · 12:00' });
    expect((sent as HTMLButtonElement).disabled).toBe(true);
    expect(within(drawer).getByText('Kapora hatırlatması gönderildi')).toBeTruthy();
  });

  it('Hatırlat: kontör yoksa "SMS kontörünüz az." ve düğme açık kalır', async () => {
    const { drawer } = await openFromCalendar(/^Gizem Arslan/, {
      calendar: { remindError: 'insufficient_credits' },
    });
    await userEvent.click(within(drawer).getByRole('button', { name: 'Hatırlat' }));
    expect(await screen.findByText('SMS kontörünüz az.')).toBeTruthy();
    expect(
      within(drawer).getByRole<HTMLButtonElement>('button', { name: 'Hatırlat' }).disabled,
    ).toBe(false);
  });

  it('Kaporasız onayla: waiveDeposit = true ile onaylanır (KK7)', async () => {
    const { calls, drawer } = await openFromCalendar(/^Gizem Arslan/);
    await userEvent.click(within(drawer).getByRole('button', { name: 'Kaporasız onayla' }));
    expect(await screen.findByText('Randevu kaporasız onaylandı')).toBeTruthy();
    expect(calls[`${path(depositAppointmentIds.gizem)}/approve`]).toEqual([{ waiveDeposit: true }]);
    // Kapora satırı ve geçmişteki DepositWaived.
    await waitFor(() => {
      expect(within(drawer).getAllByText('Kaporasız onaylandı')).toHaveLength(2);
    });
    expect(within(drawer).getByRole('region', { name: 'Randevu durumu' }).textContent).toContain(
      'Onaylandı',
    );
  });

  it('412: "Kayıt çakışması" şeridi', async () => {
    const { drawer } = await openFromCalendar(/^Gizem Arslan/, {
      calendar: { depositError: 'concurrency_conflict' },
    });
    await userEvent.click(within(drawer).getByRole('button', { name: 'Kapora geldi' }));
    expect(await within(drawer).findByText('Kayıt çakışması')).toBeTruthy();
  });

  it('409 hold_expired: "Havale süresi doldu", butonlar kalkar', async () => {
    const { drawer } = await openFromCalendar(/^Gizem Arslan/, {
      calendar: { depositError: 'hold_expired' },
    });
    await userEvent.click(within(drawer).getByRole('button', { name: 'Kapora geldi' }));
    expect(await within(drawer).findByText('Havale süresi doldu')).toBeTruthy();
    expect(within(drawer).queryByRole('button', { name: 'Kapora geldi' })).toBeNull();
    expect(within(drawer).queryByRole('button', { name: 'Hatırlat' })).toBeNull();
    expect(within(drawer).queryByRole('button', { name: 'Kaporasız onayla' })).toBeNull();
  });

  it('appointment.deposit.mark yoksa Kapora geldi ve Hatırlat yok; onay izniyle Kaporasız onayla kalır', async () => {
    const { drawer } = await openFromCalendar(/^Gizem Arslan/, {
      session: sessionFixture({
        brand: activeBrand,
        membership: { role: 'BranchManager', branchIds: [], staffId: null },
        permissions: ownerPermissions.filter((p) => p !== 'appointment.deposit.mark'),
      }),
    });
    expect(within(drawer).queryByRole('button', { name: 'Kapora geldi' })).toBeNull();
    expect(within(drawer).queryByRole('button', { name: 'Hatırlat' })).toBeNull();
    expect(within(drawer).getByRole('button', { name: 'Kaporasız onayla' })).toBeTruthy();
    expect(within(drawer).getByRole('button', { name: 'Reddet' })).toBeTruthy();
  });

  it('personel kapora eylemlerini görmez (V5)', async () => {
    const { drawer } = await openFromCalendar(/^Buse Kaya/, {
      session: sessionFixture({
        brand: activeBrand,
        membership: {
          role: 'Staff',
          branchIds: [],
          staffId: '0193a8f0-0000-7000-8000-0000000000c1',
        },
        permissions: staffPermissions,
      }),
    });
    expect(
      within(drawer).getByText('Kaporayı yalnızca yönetici ve işletme sahibi işaretler.'),
    ).toBeTruthy();
    for (const name of ['Kapora geldi', 'Hatırlat', 'Kaporasız onayla', 'Reddet', 'Onayla']) {
      expect(within(drawer).queryByRole('button', { name })).toBeNull();
    }
  });
});

describe('yan panel: alınmış kapora ve iade (V7)', () => {
  it('"Salon iade etti": tutar alınanı aşamaz; kısmi iade ve not gider, sonra "Salon iade etti · 100 ₺"', async () => {
    const { calls, drawer } = await openFromCalendar(/^Nur Ateş/);
    expect(within(drawer).getByText('Kapora geldi')).toBeTruthy();
    await userEvent.click(
      within(drawer).getByRole('button', { name: /Kapora 150 ₺ · havale ile alındı/ }),
    );
    const modal = await screen.findByRole('dialog', { name: 'Kapora iadesini işaretle' });
    const amount = within(modal).getByLabelText('İade edilen tutar');
    expect((amount as HTMLInputElement).value).toBe('150');

    await userEvent.clear(amount);
    await userEvent.type(amount, '200');
    await userEvent.click(within(modal).getByRole('button', { name: 'Salon iade etti' }));
    expect(within(modal).getByText("Tutar 0'dan büyük ve en fazla 150 ₺ olmalı.")).toBeTruthy();
    const refund = `${path(depositAppointmentIds.nur)}/deposit/refund`;
    expect(calls[refund]).toBeUndefined();

    await userEvent.clear(amount);
    await userEvent.type(amount, '100');
    await userEvent.type(within(modal).getByLabelText('Not (isteğe bağlı)'), 'Randevu ertelendi');
    await userEvent.click(within(modal).getByRole('button', { name: 'Salon iade etti' }));
    expect(await screen.findByText('Kapora iadesi işaretlendi')).toBeTruthy();
    expect(calls[refund]).toEqual([
      { amount: { amountMinor: 10000, currency: 'TRY' }, note: 'Randevu ertelendi' },
    ]);
    expect(calls[`POST ${refund} If-Match`]).toEqual(['1']);
    expect(calls[`${refund} Idempotency-Key`]).toHaveLength(1);

    const reopened = await screen.findByRole('dialog');
    expect(await within(reopened).findByText('Salon iade etti · 100 ₺')).toBeTruthy();
    expect(within(reopened).getByText('Randevu ertelendi')).toBeTruthy();
    expect(within(reopened).queryByRole('button', { name: /havale ile alındı/ })).toBeNull();
  });

  it('iade edilmiş ve kaporasız onaylanmış randevu bilgi satırı gösterir, eylem yok', async () => {
    const { drawer } = await openFromCalendar(/^Kerem Aydın/);
    expect(within(drawer).getByText('Salon iade etti · 100 ₺')).toBeTruthy();
    expect(within(drawer).getByText('Kapora 150 ₺ · havale ile alındı')).toBeTruthy();
    expect(within(drawer).getByText('Salon kaporayı iade etti')).toBeTruthy();
    cleanup();

    const waived = await openFromCalendar(/^Hande Er/);
    expect(within(waived.drawer).getAllByText('Kaporasız onaylandı')).toHaveLength(2);
    expect(within(waived.drawer).getByText('İstenen kapora 150 ₺ alınmadı')).toBeTruthy();
  });

  it('personel iade eylemini görmez', async () => {
    const { drawer } = await openFromCalendar(/^Nur Ateş/, {
      session: sessionFixture({
        brand: activeBrand,
        membership: {
          role: 'Staff',
          branchIds: [],
          staffId: '0193a8f0-0000-7000-8000-0000000000c1',
        },
        permissions: staffPermissions,
      }),
    });
    expect(within(drawer).getByText('Kapora 150 ₺ · havale ile alındı')).toBeTruthy();
    expect(within(drawer).queryByRole('button', { name: /havale ile alındı/ })).toBeNull();
  });
});

describe('genel bakış: kapora satırı', () => {
  it('kapora bekleyenler ayrı satır; bento ve onay satırı yalnız onay taleplerini sayar', async () => {
    renderApp('/', { session: owner, calendar: { deposits: true } });
    const attention = await screen.findByRole('region', { name: 'Dikkat isteyenler' });
    expect(await within(attention).findByText('1 online talep onay bekliyor')).toBeTruthy();
    expect(within(attention).getByText('Pınar Yalçın 16:00')).toBeTruthy();
    expect(within(attention).getByText('Gizem Arslan kapora bekliyor')).toBeTruthy();
    expect(
      within(attention).getByText('14:30 · havale işaretlenmedi · 1 sa 20 dk kaldı'),
    ).toBeTruthy();
    expect(within(attention).getByText('Buse Kaya kapora bekliyor')).toBeTruthy();
    expect(
      within(attention).getByText('15:30 · Havale bildirildi · kontrol edin · 1 sa 50 dk kaldı'),
    ).toBeTruthy();

    const summary = screen.getByRole('group', { name: 'Özet' });
    const pendingBlock = bentoBlock(within(summary).getByText('Onay bekliyor'));
    expect(within(pendingBlock).getByText('1')).toBeTruthy();
    expect(within(summary).getByText('ilki 1 sa 35 dk içinde iptal olur')).toBeTruthy();

    const flow = screen.getByRole('region', { name: 'Gün akışı' });
    expect(within(flow).getByRole('button', { name: /Gizem Arslan/ }).textContent).toContain(
      'Kapora bekleniyor',
    );
  });

  it('"Kapora geldi" ayrıntıyı tazeler, If-Match ve Idempotency-Key ile gider; satır düşer', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/', { session: owner, calls, calendar: { deposits: true } });
    const attention = await screen.findByRole('region', { name: 'Dikkat isteyenler' });
    await userEvent.click(
      await within(attention).findByRole('button', { name: 'Kapora geldi: Gizem Arslan' }),
    );
    expect(await screen.findByText('Kapora geldi olarak işaretlendi')).toBeTruthy();
    const received = `${path(depositAppointmentIds.gizem)}/deposit/mark-received`;
    expect(calls[`POST ${received} If-Match`]).toEqual(['1']);
    expect(calls[`${received} Idempotency-Key`]).toHaveLength(1);
    await waitFor(() => {
      expect(within(attention).queryByText('Gizem Arslan kapora bekliyor')).toBeNull();
    });
    expect(within(attention).getByText('Buse Kaya kapora bekliyor')).toBeTruthy();
  });

  it('"Hatırlat" gider; zaten gönderildiyse 409 metni ve düğme "Hatırlatıldı"', async () => {
    renderApp('/', { session: owner, calendar: { deposits: true } });
    const attention = await screen.findByRole('region', { name: 'Dikkat isteyenler' });
    await userEvent.click(
      await within(attention).findByRole('button', { name: 'Hatırlat: Gizem Arslan' }),
    );
    expect(await screen.findByText('Hatırlatma gönderildi')).toBeTruthy();
    const sent = within(attention).getByRole('button', { name: 'Hatırlatıldı: Gizem Arslan' });
    expect((sent as HTMLButtonElement).disabled).toBe(true);

    // Yan panelden gönderilmiş hatırlatmayı kuyruk satırı bilmez: sunucu 409 deposit_reminder_already_sent döner.
    const flow = screen.getByRole('region', { name: 'Gün akışı' });
    await userEvent.click(within(flow).getByRole('button', { name: /Buse Kaya/ }));
    const drawer = await screen.findByRole('dialog');
    await userEvent.click(await within(drawer).findByRole('button', { name: 'Hatırlat' }));
    await within(drawer).findByRole('button', { name: 'Hatırlatıldı · 12:00' });
    await userEvent.keyboard('{Escape}');
    await waitFor(() => {
      expect(screen.queryByRole('dialog')).toBeNull();
    });
    await userEvent.click(within(attention).getByRole('button', { name: 'Hatırlat: Buse Kaya' }));
    expect(await screen.findByText('Bu randevu için hatırlatma zaten gönderildi.')).toBeTruthy();
    expect(
      within(attention).getByRole<HTMLButtonElement>('button', { name: 'Hatırlatıldı: Buse Kaya' })
        .disabled,
    ).toBe(true);
  });

  it('kapora izni yoksa satırda yalnız "İncele"', async () => {
    renderApp('/', {
      session: sessionFixture({
        brand: activeBrand,
        membership: { role: 'BranchManager', branchIds: [], staffId: null },
        permissions: ownerPermissions.filter((p) => p !== 'appointment.deposit.mark'),
      }),
      calendar: { deposits: true },
    });
    const attention = await screen.findByRole('region', { name: 'Dikkat isteyenler' });
    expect(await within(attention).findByText('Gizem Arslan kapora bekliyor')).toBeTruthy();
    expect(within(attention).queryByRole('button', { name: /Kapora geldi/ })).toBeNull();
    expect(within(attention).queryByRole('button', { name: /Hatırlat/ })).toBeNull();
    await userEvent.click(within(attention).getByRole('button', { name: 'İncele: Gizem Arslan' }));
    const drawer = await screen.findByRole('dialog');
    expect(await within(drawer).findByRole('region', { name: 'Kapora' })).toBeTruthy();
  });
});

describe('bildirim: Havale bildirildi', () => {
  it('Deposit hedefi takvimde randevunun yan panelini açar', () => {
    const can = () => true;
    expect(notificationRoute({ type: 'Deposit', id: depositNotificationTarget }, can)).toEqual({
      to: '/takvim',
      search: { randevu: depositNotificationTarget },
    });
    expect(notificationRoute({ type: 'Deposit', id: null }, can)).toEqual({ to: '/takvim' });
    expect(
      notificationRoute({ type: 'Deposit', id: depositNotificationTarget }, () => false),
    ).toEqual({
      to: '/',
    });
  });

  it('bildirime dokununca kapora bölümü açılır; kapatınca adresten silinir', async () => {
    const { router } = renderApp('/hizmetler', {
      session: owner,
      calendar: { deposits: true },
      notifications: { depositReported: true },
    });
    await userEvent.click(await screen.findByRole('button', { name: 'Bildirimler, 4 okunmamış' }));
    const panel = screen.getByRole('dialog', { name: 'Bildirimler' });
    await userEvent.click(
      await within(panel).findByRole('button', { name: /Havale bildirildi · Buse Kaya/ }),
    );
    await waitFor(() => {
      expect(router.state.location.pathname).toBe('/takvim');
    });
    expect(router.state.location.search).toEqual({ randevu: depositNotificationTarget });
    const drawer = await screen.findByRole('dialog', { name: /^Randevu/ });
    expect(await within(drawer).findByText('Havale bildirildi · kontrol edin')).toBeTruthy();
    await userEvent.click(within(drawer).getByRole('button', { name: 'Kapat' }));
    await waitFor(() => {
      expect(router.state.location.search).toEqual({});
    });
    expect(screen.queryByRole('dialog', { name: /^Randevu/ })).toBeNull();
  });
});
