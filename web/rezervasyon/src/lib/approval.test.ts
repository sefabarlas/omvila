import { fixedClock } from '@omvila/ui-web/time';
import { describe, expect, it } from 'vitest';
import {
  cancellationText,
  isRequestCancellation,
  responseSentence,
  responseWindow,
} from './approval';

// Kural: dilim-03 kriter 1, 3, 4; bildirimler.md (appointment.declined, appointment.approval_expired).
const clock = fixedClock('2026-11-24T07:00:00Z');

describe('responseWindow', () => {
  it.each([
    ['2026-11-24T09:00:00Z', '2 saat'],
    ['2026-11-24T08:59:58Z', '2 saat'],
    ['2026-11-24T08:35:00Z', '1 saat 35 dakika'],
    ['2026-11-24T07:45:00Z', '45 dakika'],
  ])('%s → %s', (holdExpiresUtc, expected) => {
    expect(responseWindow(holdExpiresUtc, clock)).toBe(expected);
  });

  it('süre yoksa veya dolduysa null', () => {
    expect(responseWindow(null, clock)).toBeNull();
    expect(responseWindow(undefined, clock)).toBeNull();
    expect(responseWindow('2026-11-24T07:00:00Z', clock)).toBeNull();
    expect(responseWindow('2026-11-24T06:00:00Z', clock)).toBeNull();
  });
});

it('responseSentence süreli ve süresiz', () => {
  expect(responseSentence('Kuaför Elif', '2 saat')).toBe(
    'Kuaför Elif en geç 2 saat içinde yanıtlar.',
  );
  expect(responseSentence('Kuaför Elif', null)).toBe('Kuaför Elif talebinizi inceleyip yanıtlar.');
});

describe('iptal metni', () => {
  const at = '2026-11-24T06:50:00Z';

  it('talep iptalleri "reddedildi" demez', () => {
    for (const reason of ['DeclinedBySalon', 'ApprovalExpired'] as const) {
      const cancellation = { cancelledUtc: at, cancelledBy: 'Salon' as const, reason };
      expect(isRequestCancellation(cancellation)).toBe(true);
      expect(cancellationText('Kuaför Elif', cancellation)).not.toMatch(/redd/i);
    }
  });

  it('müşteri iptali talep iptali sayılmaz', () => {
    const cancellation = {
      cancelledUtc: at,
      cancelledBy: 'Customer' as const,
      reason: 'PlanChanged' as const,
    };
    expect(isRequestCancellation(cancellation)).toBe(false);
    expect(cancellationText('Kuaför Elif', cancellation)).toBe(
      'İptal salona bildirildi. Yeni randevu için salon sayfasını açabilirsiniz.',
    );
  });
});
