import { cleanup, screen, waitFor, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { afterEach, describe, expect, it } from 'vitest';
import type { AppointmentDetail } from '@/features/calendar/api';
import { ownerPermissions, sessionFixture } from '../../test/fake-api';
import type { FakeApiOptions } from '../../test/fake-api';
import { appointmentIds } from '../../test/fake-calendar';
import { renderApp } from '../../test/render-app';
import type { CustomerPackage } from './api';
import {
  amountToCollect,
  checkRows,
  depositLine,
  redemptionMatches,
  remainderFor,
} from './checkout';

// Kural: dilim-05 KK5 ("Tamamla ve ödeme al": Boya 1.250 + Fön 190 − kapora 200 = 1.240 ₺ Kart → Completed), A11
// (yalnız CheckedIn), sözleşme manageCompleteAppointment (If-Match + Idempotency-Key; payments Cash/Card/Transfer en
// çok 3; packageRedemptions; 409 payment_total_mismatch meta.expectedAmountMinor), roller-yetkiler.md
// (appointment.complete: personel yok; AppointmentDetail.payments yalnız payment.collect). OMVILA ödeme almaz: "Kart"
// yalnız yöntem kaydıdır, kart alanı yoktur. Saat: Çarşamba 25 Kasım 2026 12:00 İstanbul; Selin Öztürk ve Okan Er
// Geldi (450 ₺, kaporasız).
afterEach(() => {
  cleanup();
  localStorage.clear();
});

const owner = sessionFixture({ brand: { ...sessionFixture().brand, status: 'Active' } });
const selinPath = `/api/v1/manage/appointments/${appointmentIds.selin}`;
const selinCustomer = `${appointmentIds.selin.slice(0, -4)}c${appointmentIds.selin.slice(-3)}`;

async function openCheckout(options: FakeApiOptions = {}) {
  const calls: Record<string, unknown[]> = {};
  renderApp('/takvim', { calls, session: owner, ...options });
  await userEvent.click(await screen.findByRole('button', { name: /^Selin Öztürk/ }));
  const drawer = await screen.findByRole('dialog');
  await userEvent.click(await within(drawer).findByRole('button', { name: 'Tamamla ve ödeme al' }));
  const modal = await screen.findByRole('dialog', { name: 'Tamamla ve ödeme al' });
  return { calls, modal };
}

const money = (amountMinor: number) => ({ amountMinor, currency: 'TRY' as const });

/** KK5'teki randevu: Boya 1.250 + Fön 190, 11 Kasım'da alınan 200\u00a0₺ kapora; alınacak 1.240\u00a0₺. */
function kk5Detail(overrides: Partial<AppointmentDetail> = {}): AppointmentDetail {
  const item = (id: string, serviceId: string, serviceName: string, price: number) => ({
    id,
    serviceId,
    serviceName,
    staffId: 's1',
    staffName: 'Elif Kaya',
    startUtc: '2026-11-25T07:30:00Z',
    endUtc: '2026-11-25T09:00:00Z',
    price: money(price),
    paidFromPackage: false,
    status: 'Active' as const,
    segments: [],
  });
  return {
    id: 'a1',
    branchId: 'b1',
    status: 'CheckedIn',
    displayStatus: 'CheckedIn',
    channel: 'Online',
    startUtc: '2026-11-25T07:30:00Z',
    endUtc: '2026-11-25T09:00:00Z',
    timeZone: 'Europe/Istanbul',
    customer: { brandCustomerId: 'c1', fullName: 'Selin Öztürk', visitCount: 12 },
    items: [item('i1', 'boya', 'Boya', 125000), item('i2', 'fon', 'Fön', 19000)],
    deposit: {
      required: money(20000),
      status: 'Received',
      receivedUtc: '2026-11-11T10:00:00Z',
    },
    total: money(144000),
    amountDue: money(124000),
    isRecurring: false,
    history: [],
    version: '4',
    ...overrides,
  };
}

function fonPackage(overrides: Partial<CustomerPackage> = {}): CustomerPackage {
  return {
    id: 'p1',
    customer: { brandCustomerId: 'c1', fullName: 'Selin Öztürk' },
    packageId: 'pk',
    packageName: 'Fön paketi',
    branchId: 'b1',
    isAvailableAllBranches: false,
    price: money(85000),
    purchasedUtc: '2026-10-01T09:00:00Z',
    expiresOn: '2026-12-31',
    status: 'Active',
    items: [
      {
        customerPackageItemId: 'pi1',
        serviceId: 'fon',
        serviceName: 'Fön',
        totalSessions: 5,
        remainingSessions: 2,
      },
    ],
    totalSessions: 5,
    remainingSessions: 2,
    version: '1',
    ...overrides,
  };
}

describe('tamamla: hesaplar', () => {
  it('KK5: kapora satırı "Kapora (havale, 11 Kas)" 200\u00a0₺; alınacak 1.240\u00a0₺', () => {
    const detail = kk5Detail();
    expect(depositLine(detail)).toEqual({ amountMinor: 20000, label: 'Kapora (havale, 11 Kas)' });
    expect(amountToCollect(detail, [])).toBe(124000);
  });

  it('iade edilen kapora düşülür; ödemeler görünüyorsa kapora kaydından okunur', () => {
    expect(
      depositLine(
        kk5Detail({
          deposit: {
            required: money(20000),
            status: 'Refunded',
            receivedUtc: '2026-11-11T10:00:00Z',
            refundedAmount: money(15000),
          },
        }),
      )?.amountMinor,
    ).toBe(5000);
    expect(
      depositLine(
        kk5Detail({
          payments: [
            {
              id: 'pay1',
              branchId: 'b1',
              method: 'Transfer',
              amount: money(20000),
              isDeposit: true,
              status: 'Recorded',
              paidUtc: '2026-11-12T10:00:00Z',
              purpose: 'Appointment',
            },
          ],
        }),
      ),
    ).toEqual({ amountMinor: 20000, label: 'Kapora (havale, 12 Kas)' });
    expect(depositLine(kk5Detail({ deposit: null }))).toBeNull();
  });

  it('"Paketten": aynı hizmetin kalan seansı olan, etkin, süresi geçmemiş ve şubede geçerli paket', () => {
    const detail = kk5Detail();
    const [match] = redemptionMatches(detail, [fonPackage()], '2026-11-25');
    expect(match).toMatchObject({
      appointmentItemId: 'i2',
      customerPackageItemId: 'pi1',
      amountMinor: 19000,
      remainingAfter: 1,
    });
    expect(amountToCollect(detail, match ? [match] : [])).toBe(105000);
    const none = (p: CustomerPackage) => redemptionMatches(detail, [p], '2026-11-25');
    expect(none(fonPackage({ status: 'Expired' }))).toEqual([]);
    expect(none(fonPackage({ expiresOn: '2026-11-24' }))).toEqual([]);
    expect(none(fonPackage({ branchId: 'b2' }))).toEqual([]);
    expect(none(fonPackage({ branchId: 'b2', isAvailableAllBranches: true }))).toHaveLength(1);
    const used = fonPackage();
    used.items = used.items.map((i) => ({ ...i, remainingSessions: 0 }));
    expect(none(used)).toEqual([]);
  });

  it('satırlar: tutar zorunlu ve toplam alınacağa eşit; alınacak 0 ise satır gönderilmez', () => {
    expect(checkRows([{ method: 'Card', amount: '1.240' }], 124000)).toEqual({
      ok: true,
      amounts: [124000],
    });
    expect(
      checkRows(
        [
          { method: 'Cash', amount: '1.000' },
          { method: 'Card', amount: '240' },
        ],
        124000,
      ),
    ).toEqual({ ok: true, amounts: [100000, 24000] });
    expect(checkRows([{ method: 'Cash', amount: '' }], 124000)).toMatchObject({
      ok: false,
      rowErrors: ['Tutarı girin, ör. 450.'],
    });
    expect(checkRows([{ method: 'Cash', amount: '1.000' }], 124000)).toMatchObject({
      ok: false,
      totalError: 'Ödemelerin toplamı (1.000\u00a0₺) alınacak tutarla (1.240\u00a0₺) aynı olmalı.',
    });
    expect(checkRows([{ method: 'Cash', amount: '' }], 0)).toEqual({ ok: true, amounts: [] });
    expect(remainderFor([{ method: 'Cash', amount: '1.000' }], 124000)).toBe('240');
    expect(remainderFor([{ method: 'Cash', amount: '1.240' }], 124000)).toBe('');
  });
});

describe('tamamla ve ödeme al (OdemeAl)', () => {
  it('Geldi randevuda buton; Kart ile 450\u00a0₺ → Tamamlandı, If-Match + Idempotency-Key, yan panelde ödemeler', async () => {
    const { calls, modal } = await openCheckout();
    expect(within(modal).getByText('Selin Öztürk')).toBeTruthy();
    expect(within(modal).getByText(/bugün 10:30–12:30 · Geldi/)).toBeTruthy();
    const total = within(modal).getByText('Alınacak').nextElementSibling;
    expect(total?.textContent).toBe('450\u00a0₺');
    // Müşterinin paketi yok: "Paketten" seçeneği görünmez. Kart yalnız yöntem; kart numarası alanı yok.
    const methods = within(modal).getByRole('group', { name: 'Ödeme yöntemi' });
    expect(within(methods).queryByRole('radio', { name: 'Paketten' })).toBeNull();
    await userEvent.click(within(methods).getByRole('radio', { name: 'Kart' }));
    expect(within(modal).queryByLabelText(/kart numarası/i)).toBeNull();
    expect(within(modal).getByLabelText<HTMLInputElement>('Tutar').value).toBe('450');
    expect(
      await within(modal).findByText(
        "Tamamlandığında Elif Kaya'nın primi hak edilir ve müşteriye değerlendirme daveti gider.",
      ),
    ).toBeTruthy();

    await userEvent.click(within(modal).getByRole('button', { name: 'Tamamla ve kaydet' }));
    await waitFor(() => {
      expect(screen.queryByRole('dialog', { name: 'Tamamla ve ödeme al' })).toBeNull();
    });
    expect(calls[`${selinPath}/complete`]).toEqual([
      { payments: [{ method: 'Card', amount: { amountMinor: 45000, currency: 'TRY' } }] },
    ]);
    expect(calls[`POST ${selinPath}/complete If-Match`]).toEqual(['1']);
    expect(calls[`${selinPath}/complete Idempotency-Key`]).toHaveLength(1);
    expect(await screen.findByText('Randevu tamamlandı')).toBeTruthy();
    const drawer = await screen.findByRole('dialog', { name: /^Randevu/ });
    expect(within(drawer).getByText('Tamamlandı')).toBeTruthy();
    const paid = within(drawer).getByRole('region', { name: 'Ödemeler' });
    expect(within(paid).getByText(/^Kart · /)).toBeTruthy();
    expect(within(paid).getByText('450 ₺')).toBeTruthy();
    expect(within(drawer).queryByRole('button', { name: 'Tamamla ve ödeme al' })).toBeNull();
  });

  it('ikinci ödeme: kalan önerilir; toplam tutmazsa gönderilmez; en çok üç satır', async () => {
    const { calls, modal } = await openCheckout();
    const amount = within(modal).getByLabelText('Tutar');
    await userEvent.clear(amount);
    await userEvent.type(amount, '300');
    await userEvent.click(within(modal).getByRole('button', { name: 'İkinci ödeme ekle' }));
    const second = within(modal).getByRole('region', { name: 'İkinci ödeme' });
    expect(within(second).getByLabelText<HTMLInputElement>('Tutar').value).toBe('150');
    await userEvent.click(within(second).getByRole('radio', { name: 'Havale' }));
    // Paketten yalnız ilk satırda olabilir; Nakit/Kart/Havale.
    expect(within(second).queryByRole('radio', { name: 'Paketten' })).toBeNull();
    await userEvent.click(within(modal).getByRole('button', { name: 'Üçüncü ödeme ekle' }));
    expect(within(modal).queryByRole('button', { name: /ödeme ekle$/ })).toBeNull();
    const third = within(modal).getByRole('region', { name: 'Üçüncü ödeme' });
    await userEvent.type(within(third).getByLabelText('Tutar'), '50');

    await userEvent.click(within(modal).getByRole('button', { name: 'Tamamla ve kaydet' }));
    expect(within(modal).getByRole('alert').textContent).toBe(
      'Ödemelerin toplamı (500\u00a0₺) alınacak tutarla (450\u00a0₺) aynı olmalı.',
    );
    expect(calls[`${selinPath}/complete`]).toBeUndefined();

    await userEvent.click(within(third).getByRole('button', { name: 'Kaldır' }));
    await userEvent.click(within(modal).getByRole('button', { name: 'Tamamla ve kaydet' }));
    await waitFor(() => {
      expect(calls[`${selinPath}/complete`]).toEqual([
        {
          payments: [
            { method: 'Cash', amount: { amountMinor: 30000, currency: 'TRY' } },
            { method: 'Transfer', amount: { amountMinor: 15000, currency: 'TRY' } },
          ],
        },
      ]);
    });
  });

  it('sunucu toplam uyuşmazlığı: beklenen tutar yazılır, pencere açık kalır, yeni anahtarla tekrar', async () => {
    const { calls, modal } = await openCheckout({
      calendar: { completeError: 'payment_total_mismatch' },
    });
    await userEvent.click(within(modal).getByRole('button', { name: 'Tamamla ve kaydet' }));
    expect((await within(modal).findByRole('alert')).textContent).toBe(
      'Alınacak tutar 450\u00a0₺ olmalı. Tutarları kontrol edin.',
    );
    await userEvent.click(within(modal).getByRole('button', { name: 'Tamamla ve kaydet' }));
    await waitFor(() => {
      expect(calls[`${selinPath}/complete Idempotency-Key`]).toHaveLength(2);
    });
    const [first, second] = calls[`${selinPath}/complete Idempotency-Key`] ?? [];
    expect(first).not.toBe(second);
  });

  it('412: pencere kapanır, yan panelde "Kayıt çakışması"', async () => {
    const { modal } = await openCheckout({ calendar: { completeError: 'concurrency_conflict' } });
    await userEvent.click(within(modal).getByRole('button', { name: 'Tamamla ve kaydet' }));
    const drawer = await screen.findByRole('dialog', { name: /^Randevu/ });
    expect(await within(drawer).findByText('Kayıt çakışması')).toBeTruthy();
  });

  it('"Paketten": paket kalemi varsa görünür; seçilince kalem paketten düşer ve ödeme satırı kalkar', async () => {
    const { calls, modal } = await openCheckout({ sales: { packageCustomerId: selinCustomer } });
    const methods = within(modal).getByRole('group', { name: 'Ödeme yöntemi' });
    await userEvent.click(await within(methods).findByRole('radio', { name: 'Paketten' }));
    expect(within(modal).getByText('Alınacak').nextElementSibling?.textContent).toBe('0\u00a0₺');
    expect(within(modal).getByText('Paketten (Kesim paketi · Boya)')).toBeTruthy();
    expect(within(modal).queryByLabelText('Tutar')).toBeNull();
    await userEvent.click(within(modal).getByRole('button', { name: 'Tamamla ve kaydet' }));
    await waitFor(() => {
      expect(calls[`${selinPath}/complete`]).toEqual([
        {
          payments: [],
          packageRedemptions: [
            {
              appointmentItemId: `${appointmentIds.selin.slice(0, -4)}b${appointmentIds.selin.slice(-3)}`,
              customerPackageItemId: '0193a8f0-0000-7000-8000-0000000f7a04',
            },
          ],
        },
      ]);
    });
  });

  it('appointment.complete izni yoksa buton yok (personel)', async () => {
    renderApp('/takvim', {
      session: sessionFixture({
        brand: owner.brand,
        permissions: ownerPermissions.filter((p) => p !== 'appointment.complete'),
      }),
    });
    await userEvent.click(await screen.findByRole('button', { name: /^Selin Öztürk/ }));
    const drawer = await screen.findByRole('dialog');
    await within(drawer).findByRole('region', { name: 'Randevu durumu' });
    expect(within(drawer).queryByRole('button', { name: 'Tamamla ve ödeme al' })).toBeNull();
  });
});
