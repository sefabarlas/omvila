import { cleanup, screen, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { afterEach, describe, expect, it } from 'vitest';
import { sessionFixture, staffPermissions } from '../../test/fake-api';
import { renderApp } from '../../test/render-app';
import {
  axisLabel,
  axisStep,
  closedDaysLabel,
  formatChange,
  periodLabel,
  periodRange,
} from './period';

// Kural: dilim-06 KK6 (Raporlar ve prim: kartlar, günlük tahsilat, personel ve prim, Primim), salon-paneli/Raporlar,
// RaporlarBos, Durumlar ("Yetkiniz yok"), roller-yetkiler.md (report.view, commission.view K), T-122.
afterEach(cleanup);

const owner = sessionFixture({ brand: { ...sessionFixture().brand, status: 'Active' } });
const staff = sessionFixture({
  brand: owner.brand,
  permissions: staffPermissions,
  membership: { role: 'Staff', branchIds: [], staffId: '0193a8f0-0000-7000-8000-0000000000c1' },
});

describe('dönem ve eksen', () => {
  it('Gün / Hafta / Ay bugünde biter; hafta Pazartesi başlar', () => {
    expect(periodRange('gun', '2026-11-25')).toEqual({
      from: '2026-11-25',
      to: '2026-11-25',
      end: '2026-11-25',
    });
    expect(periodRange('hafta', '2026-11-25')).toEqual({
      from: '2026-11-23',
      to: '2026-11-25',
      end: '2026-11-29',
    });
    expect(periodRange('ay', '2026-11-25')).toEqual({
      from: '2026-11-01',
      to: '2026-11-25',
      end: '2026-11-30',
    });
    expect(periodRange('ay', '2026-12-31').end).toBe('2026-12-31');
    expect(periodRange('hafta', '2026-11-29').from).toBe('2026-11-23');
  });

  it('etiketler', () => {
    expect(periodLabel('2026-11-18', '2026-11-24')).toBe('18–24 Kasım');
    expect(periodLabel('2026-11-25', '2026-11-25')).toBe('25 Kasım');
    expect(periodLabel('2026-11-30', '2026-12-02')).toBe('30 Kasım – 2 Aralık');
    expect(formatChange(9)).toBe('+%9');
    expect(formatChange(-3.4)).toBe('−%3');
    expect(closedDaysLabel(['2026-11-23'])).toBe('Pazartesi kapalı');
    expect(closedDaysLabel(['2026-11-23', '2026-11-29', '2026-11-30'])).toBe(
      'Pazartesi ve Pazar kapalı',
    );
    expect(closedDaysLabel([])).toBeNull();
  });

  it('eksen 1-2-5 adımlı, bin lira "B" ile', () => {
    expect(axisStep(1845000)).toBe(500000);
    expect(axisLabel(2000000)).toBe('20B ₺');
    expect(axisLabel(250000)).toBe('2,5B ₺');
    expect(axisStep(0)).toBe(0);
    expect(axisLabel(50000)).toBe('500 ₺');
  });
});

describe('Raporlar ve prim', () => {
  it('kartlar, günlük tahsilat ve personel ve prim tablosu', async () => {
    renderApp('/raporlar', { session: owner });
    expect(await screen.findByRole('heading', { level: 1, name: 'Raporlar ve prim' })).toBeTruthy();
    expect(screen.getByText('Merkez · 23–25 Kasım')).toBeTruthy();

    const summary = await screen.findByRole('group', { name: 'Özet' });
    expect(await within(summary).findByText('61.200 ₺')).toBeTruthy();
    expect(within(summary).getByText('tamamlanan 118 randevu · +%9')).toBeTruthy();
    expect(within(summary).getByText('67.950 ₺')).toBeTruthy();
    expect(within(summary).getByText('paket satışları ve kaporalar dahil')).toBeTruthy();
    expect(within(summary).getByText('%4 · 2.150 ₺ kayıp')).toBeTruthy();
    expect(within(summary).getByText('%46')).toBeTruthy();
    expect(within(summary).getByText('54 randevu online')).toBeTruthy();

    const daily = screen.getByRole('region', { name: 'Günlük tahsilat' });
    expect(within(daily).getByText('Pazartesi kapalı')).toBeTruthy();
    expect(within(daily).getByRole('img', { name: /Günlük tahsilat grafiği/ })).toBeTruthy();
    await userEvent.click(within(daily).getByText('Tablo olarak göster'));
    const table = within(daily).getByRole('table', { name: 'Günlük tahsilat' });
    // Gelecek günler (26–29 Kasım) tabloda yok; kapalı gün "Kapalı".
    expect(within(table).getAllByRole('row')).toHaveLength(4);
    expect(within(table).getByText('Kapalı')).toBeTruthy();
    expect(within(table).getByText('11.200 ₺')).toBeTruthy();

    const commission = screen.getByRole('region', { name: 'Personel ve prim' });
    const rows = await within(commission).findAllByRole('row');
    // Tutarda bölünmez boşluk (formatMoney): metin karşılaştırması için normal boşluğa çevrilir.
    const first = rows[1]?.textContent.replaceAll('\u00A0', ' ') ?? '';
    expect(first).toContain('Elif Kaya');
    expect(first).toContain('24.300 ₺');
    expect(first).toContain('%24');
    expect(first).toContain('5.832 ₺');
    expect(
      within(commission).getByText('Gelmeyen ve iptal edilen randevular prime dahil değil.'),
    ).toBeTruthy();
    expect(within(commission).getByText('15.665 ₺')).toBeTruthy();
    expect(
      within(commission).getByRole('link', { name: 'Prim kuralları' }).getAttribute('href'),
    ).toBe('/personel');
    // "Dışa aktar" sözleşmede yok.
    expect(screen.queryByRole('button', { name: 'Dışa aktar' })).toBeNull();
  });

  it('Ay seçimi adrese yazılır ve aralığı ayın 1’inden başlatır', async () => {
    const { router } = renderApp('/raporlar', { session: owner });
    await screen.findByText('Merkez · 23–25 Kasım');
    await userEvent.click(screen.getByRole('radio', { name: 'Ay' }));
    expect(await screen.findByText('Merkez · 1–25 Kasım')).toBeTruthy();
    expect(router.state.location.search).toEqual({ donem: 'ay' });
    await userEvent.click(screen.getByRole('radio', { name: 'Gün' }));
    expect(await screen.findByText('Merkez · 25 Kasım')).toBeTruthy();
  });

  it('veri yoksa kartlar "—" ve rapor yok daveti (RaporlarBos)', async () => {
    renderApp('/raporlar', { session: owner, reports: { empty: true } });
    expect(await screen.findByRole('heading', { name: 'Rapor için henüz veri yok' })).toBeTruthy();
    const summary = screen.getByRole('group', { name: 'Özet' });
    expect(within(summary).getAllByText('—')).toHaveLength(4);
    expect(within(summary).getByText('tamamlanan randevu yok')).toBeTruthy();
    expect(screen.queryByRole('region', { name: 'Günlük tahsilat' })).toBeNull();
    expect(screen.getByRole('button', { name: 'Prim kurallarını ayarla' })).toBeTruthy();
  });

  it('yüklenemezse hata ve Tekrar dene', async () => {
    renderApp('/raporlar', { session: owner, reports: { fails: true } });
    expect(await screen.findByRole('heading', { name: 'Rapor yüklenemedi' })).toBeTruthy();
    expect(screen.getByRole('button', { name: 'Tekrar dene' })).toBeTruthy();
  });

  it('prim izni yoksa tablo yok; personel ayarı yoksa "Prim kuralları" yok', async () => {
    renderApp('/raporlar', {
      session: sessionFixture({ brand: owner.brand, permissions: ['report.view'] }),
    });
    expect(await screen.findByRole('region', { name: 'Günlük tahsilat' })).toBeTruthy();
    expect(screen.queryByRole('region', { name: 'Personel ve prim' })).toBeNull();
    expect(screen.queryByRole('link', { name: 'Prim kuralları' })).toBeNull();
  });
});

describe('Primim', () => {
  it('ay toplamı, bu hafta ve son primler', async () => {
    renderApp('/primim', { session: staff });
    expect(await screen.findByRole('heading', { level: 1, name: 'Primim' })).toBeTruthy();
    expect(await screen.findByText('Elif Kaya · 1–25 Kasım')).toBeTruthy();
    const summary = screen.getByRole('group', { name: 'Özet' });
    expect(within(summary).getByText('Kasım primim')).toBeTruthy();
    expect(within(summary).getByText('6.840 ₺')).toBeTruthy();
    expect(within(summary).getByText('Hizmet geliri 28.500 ₺')).toBeTruthy();
    expect(within(summary).getByText('94')).toBeTruthy();
    expect(within(summary).getByText('4,9')).toBeTruthy();
    expect(within(summary).getByText('%4')).toBeTruthy();

    const week = screen.getByRole('region', { name: 'Bu hafta' });
    expect(within(week).getByText('Sal en yoğun')).toBeTruthy();

    const recent = screen.getByRole('region', { name: 'Son primler' });
    expect(within(recent).getAllByRole('listitem')).toHaveLength(5);
    expect(within(recent).getByText('Bugün 09:30 · Saç kesimi')).toBeTruthy();
    expect(within(recent).getByText('+90 ₺')).toBeTruthy();
    expect(within(recent).getByText('Dün 17:00 · Keratin')).toBeTruthy();
    // Ters kayıt eksi gösterilir.
    expect(within(recent).getByText('−90 ₺')).toBeTruthy();
    expect(
      within(recent).getByText(
        'Prim randevu tamamlandığında hak edilir. Selin Öztürk randevusu bitince eklenecek.',
      ),
    ).toBeTruthy();
    await userEvent.click(within(recent).getByRole('button', { name: 'Tümünü göster' }));
    expect(within(recent).getAllByRole('listitem')).toHaveLength(6);
  });

  it('bu ay prim yoksa boş satır ve "—"', async () => {
    renderApp('/primim', { session: staff, reports: { empty: true } });
    expect(await screen.findByText('Bu ay henüz prim yok.')).toBeTruthy();
    const summary = screen.getByRole('group', { name: 'Özet' });
    expect(within(summary).getAllByText('—')).toHaveLength(2);
  });

  it('personel kaydı yoksa açıklama', async () => {
    renderApp('/primim', { session: staff, reports: { noStaffRecord: true } });
    expect(await screen.findByRole('heading', { name: 'Personel kaydınız yok' })).toBeTruthy();
  });

  it('prim izni yoksa 403', async () => {
    renderApp('/primim', {
      session: sessionFixture({ brand: owner.brand, permissions: ['calendar.view'] }),
    });
    expect(
      await screen.findByRole('heading', { level: 1, name: 'Bu sayfayı görme yetkiniz yok' }),
    ).toBeTruthy();
  });
});
