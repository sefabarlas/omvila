import { cleanup, screen, waitFor, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { afterEach, describe, expect, it } from 'vitest';
import { ownerPermissions, sessionFixture } from '../../test/fake-api';
import type { FakeApiOptions } from '../../test/fake-api';
import { appointmentIds } from '../../test/fake-calendar';
import { renderApp } from '../../test/render-app';
import { firstExpiresLabel, remainingMinutes, replyWithinLabel } from './remaining';

// Kural: dilim-03 kabul kriterleri 2 (Onayla → Onaylandı), 3 (Reddet → iptal; "reddedildi" yok), 5 (personel onay
// butonlarını görmez); salon-paneli/RandevuPanel ve GenelBakis; T-62 (If-Match, 409 hold_expired, kuyruk generatedUtc);
// kodlama-typescript.md (412 → "Kayıt çakışması" + Yenile). Saat: Çarşamba 25 Kasım 2026 12:00 İstanbul; Pınar
// Yalçın'ın talebi 13:35'te düşer (1 sa 35 dk).
afterEach(() => {
  cleanup();
  localStorage.clear();
});

const pinarPath = `/api/v1/manage/appointments/${appointmentIds.pinar}`;
const activeBrand = { ...sessionFixture().brand, status: 'Active' as const };
const managerWithoutApproval = sessionFixture({
  brand: activeBrand,
  membership: { role: 'BranchManager', branchIds: [], staffId: null },
  permissions: ownerPermissions.filter((p) => p !== 'appointment.approve'),
});

async function openPinarFromCalendar(options: FakeApiOptions = {}) {
  const calls: Record<string, unknown[]> = {};
  renderApp('/takvim', { calls, ...options });
  await userEvent.click(await screen.findByRole('button', { name: /^Pınar Yalçın/ }));
  const drawer = await screen.findByRole('dialog');
  await within(drawer).findByText('Pınar Yalçın');
  return { calls, drawer };
}

describe('kalan süre biçimi', () => {
  it('dakika yukarı yuvarlanır, süre dolunca 0', () => {
    expect(remainingMinutes('2026-11-25T10:35:00Z', '2026-11-25T09:00:00Z')).toBe(95);
    expect(remainingMinutes('2026-11-25T10:35:00Z', new Date('2026-11-25T09:00:30Z'))).toBe(95);
    expect(remainingMinutes('2026-11-25T09:00:00Z', '2026-11-25T09:00:00Z')).toBe(0);
    expect(remainingMinutes('2026-11-25T08:59:00Z', '2026-11-25T09:00:00Z')).toBe(0);
  });

  it('"1 sa 35 dk içinde yanıtlayın." ve bento metni', () => {
    expect(replyWithinLabel(95)).toBe('1 sa 35 dk içinde yanıtlayın.');
    expect(replyWithinLabel(45)).toBe('45 dk içinde yanıtlayın.');
    expect(replyWithinLabel(120)).toBe('2 sa içinde yanıtlayın.');
    expect(firstExpiresLabel(95)).toBe('ilki 1 sa 35 dk içinde iptal olur');
  });
});

describe('onay yan paneli (RandevuPanel)', () => {
  it('kalan süre, ücret, not ve geçmiş; Onayla If-Match ile gider ve randevu Onaylandı olur', async () => {
    const { calls, drawer } = await openPinarFromCalendar();
    expect(within(drawer).getByText('1 sa 35 dk içinde yanıtlayın.')).toBeTruthy();
    expect(within(drawer).getByText('Bugün · Online randevu')).toBeTruthy();
    expect(within(drawer).getByText('Cilt bakımı · Zeynep Aksoy · 1 sa')).toBeTruthy();
    expect(within(drawer).getByText('Salonda ödenecek')).toBeTruthy();
    expect(within(drawer).getByText('Parfümsüz ürün tercih ediyorum.')).toBeTruthy();
    expect(within(drawer).getByText('Online talep oluşturuldu, SMS ile doğrulandı')).toBeTruthy();
    expect(within(drawer).getByText('11:35')).toBeTruthy();
    // Onay bekleyen talepte iptal/taşıma eylemleri yerine Reddet / Onayla.
    expect(within(drawer).queryByText('Randevuyu iptal et')).toBeNull();

    await userEvent.click(within(drawer).getByRole('button', { name: 'Onayla' }));
    expect(await screen.findByText('Randevu onaylandı')).toBeTruthy();
    expect(calls[`POST ${pinarPath}/approve If-Match`]).toEqual(['1']);
    await waitFor(() => {
      expect(within(drawer).queryByRole('button', { name: 'Onayla' })).toBeNull();
    });
    expect(within(drawer).queryByRole('button', { name: 'Reddet' })).toBeNull();
    expect(within(drawer).getAllByText('Onaylandı').length).toBeGreaterThan(0);
    expect(
      await screen.findByRole('button', {
        name: /^Pınar Yalçın, Cilt bakımı, 16:00–17:00, Onaylandı/,
      }),
    ).toBeTruthy();
  });

  it('Reddet If-Match ile gider; metinde "iptal" var, "reddedildi" yok', async () => {
    const { calls, drawer } = await openPinarFromCalendar();
    await userEvent.click(within(drawer).getByRole('button', { name: 'Reddet' }));
    expect(await screen.findByText('Talep iptal edildi')).toBeTruthy();
    expect(screen.getByText('Müşteriye iptal bildirimi gider.')).toBeTruthy();
    expect(calls[`POST ${pinarPath}/decline If-Match`]).toEqual(['1']);
    expect(calls[`${pinarPath}/decline`]).toEqual([null]);
    await waitFor(() => {
      expect(screen.queryByRole('button', { name: /^Pınar Yalçın/ })).toBeNull();
    });
    expect(document.body.textContent).not.toMatch(/reddedildi/i);
  });

  it('412: "Kayıt çakışması" şeridi ve Yenile', async () => {
    const { drawer } = await openPinarFromCalendar({
      calendar: { approvalError: 'concurrency_conflict' },
    });
    await userEvent.click(within(drawer).getByRole('button', { name: 'Onayla' }));
    expect(await within(drawer).findByText('Kayıt çakışması')).toBeTruthy();
    await userEvent.click(within(drawer).getByRole('button', { name: 'Yenile' }));
    await waitFor(() => {
      expect(within(drawer).queryByText('Kayıt çakışması')).toBeNull();
    });
  });

  it('409 hold_expired: "Yanıt süresi doldu" ve butonlar kalkar', async () => {
    const { drawer } = await openPinarFromCalendar({ calendar: { approvalError: 'hold_expired' } });
    await userEvent.click(within(drawer).getByRole('button', { name: 'Reddet' }));
    expect(await within(drawer).findByText('Yanıt süresi doldu')).toBeTruthy();
    expect(within(drawer).queryByText('1 sa 35 dk içinde yanıtlayın.')).toBeNull();
    expect(within(drawer).queryByRole('button', { name: 'Onayla' })).toBeNull();
    expect(within(drawer).queryByRole('button', { name: 'Reddet' })).toBeNull();
  });

  it('appointment.approve izni yoksa Onayla / Reddet görünmez (kriter 5)', async () => {
    const { drawer } = await openPinarFromCalendar({ session: managerWithoutApproval });
    expect(
      within(drawer).getByText(
        'Onay bekleyen talepleri yalnızca yönetici ve işletme sahibi onaylar.',
      ),
    ).toBeTruthy();
    expect(within(drawer).queryByRole('button', { name: 'Onayla' })).toBeNull();
    expect(within(drawer).queryByRole('button', { name: 'Reddet' })).toBeNull();
    expect(within(drawer).queryByText(/içinde yanıtlayın/)).toBeNull();
  });
});

describe('genel bakış (GenelBakis)', () => {
  it('gün akışı, onay bekliyor bentosu, dikkat isteyenler; İncele → Onayla kuyruğu boşaltır', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/', { session: sessionFixture({ brand: activeBrand }), calls });
    const flow = await screen.findByRole('region', { name: 'Gün akışı' });
    const rows = await within(flow).findAllByRole('button');
    expect(rows.map((row) => row.textContent)).toEqual([
      '09:30Ayşe Yılmaz · Saç kesimi · ElifTamamlandı',
      '10:30Selin Öztürk · Boya · ElifGeldi',
      '10:30Ece Koç · Kaş ve kirpik · ZeynepGelmedi',
      '11:00Okan Er · Saç kesimi · MertGeldi',
      '13:00Derya Şahin · Saç kesimi · ElifOnaylandı',
      '14:00Aslı Kurt · Balyaj · CanOnaylandı',
      '16:00Pınar Yalçın · Cilt bakımı · ZeynepOnay bekliyor',
    ]);
    expect(within(flow).getByRole('link', { name: 'Takvime git' }).getAttribute('href')).toBe(
      '/takvim',
    );

    const summary = screen.getByRole('group', { name: 'Özet' });
    // Kasayı görebilen sahipte "Bugünkü ciro" (manageGetCashDay, T-97).
    expect(await within(summary).findByText('3.340 ₺')).toBeTruthy();
    expect(within(summary).getByText('7 randevu')).toBeTruthy();
    expect(within(summary).getByText('Onay bekliyor')).toBeTruthy();
    expect(await within(summary).findByText('ilki 1 sa 35 dk içinde iptal olur')).toBeTruthy();

    const attention = screen.getByRole('region', { name: 'Dikkat isteyenler' });
    expect(await within(attention).findByText('1 online talep onay bekliyor')).toBeTruthy();
    expect(within(attention).getByText('Pınar Yalçın 16:00')).toBeTruthy();
    expect(screen.queryByText('Kurulum tamam')).toBeNull();

    await userEvent.click(within(attention).getByRole('button', { name: 'İncele: Pınar Yalçın' }));
    const drawer = await screen.findByRole('dialog');
    expect(await within(drawer).findByText('1 sa 35 dk içinde yanıtlayın.')).toBeTruthy();
    await userEvent.click(within(drawer).getByRole('button', { name: 'Onayla' }));
    expect(await screen.findByText('Randevu onaylandı')).toBeTruthy();
    expect(calls[`POST ${pinarPath}/approve If-Match`]).toEqual(['1']);
    expect(await within(attention).findByText('Dikkat isteyen bir şey yok')).toBeTruthy();
    expect(within(summary).getByText('Bekleyen talep yok')).toBeTruthy();
  });

  it('gün akışı satırı yan paneli açar', async () => {
    renderApp('/', { session: sessionFixture({ brand: activeBrand }) });
    const flow = await screen.findByRole('region', { name: 'Gün akışı' });
    await userEvent.click(await within(flow).findByRole('button', { name: /Derya Şahin/ }));
    const drawer = await screen.findByRole('dialog');
    expect(await within(drawer).findByText('Derya Şahin')).toBeTruthy();
  });

  it('onay izni yoksa onay bentosu ve kuyruk satırı yok', async () => {
    renderApp('/', { session: managerWithoutApproval });
    await screen.findByRole('region', { name: 'Gün akışı' });
    const summary = screen.getByRole('group', { name: 'Özet' });
    expect(within(summary).queryByText('Onay bekliyor')).toBeNull();
    const attention = screen.getByRole('region', { name: 'Dikkat isteyenler' });
    expect(within(attention).getByText('Dikkat isteyen bir şey yok')).toBeTruthy();
    expect(within(attention).queryByRole('button', { name: /İncele/ })).toBeNull();
  });

  it('kuyruk yüklenemezse hata ve Tekrar dene', async () => {
    renderApp('/', {
      session: sessionFixture({ brand: activeBrand }),
      calendar: { queueFails: true },
    });
    const attention = await screen.findByRole('region', { name: 'Dikkat isteyenler' });
    expect(await within(attention).findByText('Onay bekleyen talepler yüklenemedi.')).toBeTruthy();
    expect(within(attention).getByRole('button', { name: 'Tekrar dene' })).toBeTruthy();
  });
});
