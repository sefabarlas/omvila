import type { UsablePackageItem } from '@/booking/booking-api';
import type { ReviewInvite } from '@/review/review-api';
import type { WaitlistEntry, WaitlistOffer } from '@/waitlist/waitlist-api';
import { branchId, ids, nowUtc } from './booking-fixtures';

// Dilim 5 rezervasyon sayfası kurgusal verisi (gerçek kişi yok): bekleme listesi kaydı ve teklifi (BeklemeTeklif:
// Spa Moda, Masaj · 60 dk, Seda Kurt, 900 ₺), değerlendirme daveti (Degerlendir) ve paket seansı (Onay.png).
// "Şimdi" 24 Kasım 2026 Salı 10:00 İstanbul (nowUtc).

const tl = (lira: number) => ({ amountMinor: lira * 100, currency: 'TRY' as const });

export const offerCode = 'W8fQ2mZk4R';
export const waitlistCode = 'W3nB8xQ2';
export const reviewCode = 'D7kP2xQ9mA';

export function entryFixture(overrides: Partial<WaitlistEntry> = {}): WaitlistEntry {
  return {
    id: '0191a000-0000-7000-8000-000000000001',
    accessCode: waitlistCode,
    status: 'Waiting',
    branchId,
    timeZone: 'Europe/Istanbul',
    serviceNames: ['Saç kesimi', 'Fön'],
    preferredStaffName: 'Elif Kaya',
    acceptOtherStaff: false,
    dates: ['2026-11-26'],
    timeWindows: ['Afternoon', 'Evening'],
    ...overrides,
  };
}

/** Bekleyen teklif: saat 9 dk 5 sn tutulur (holdExpiresUtc − generatedUtc), 27 Kasım Cuma 16:00. */
export function offerFixture(overrides: Partial<WaitlistOffer> = {}): WaitlistOffer {
  return {
    id: '0191b000-0000-7000-8000-000000000001',
    status: 'Pending',
    brand: { name: 'Spa Moda', slug: 'spa-moda', logoUrl: null },
    branch: { id: branchId, name: 'Moda' },
    customerFirstName: 'Merve',
    startUtc: '2026-11-27T13:00:00Z',
    endUtc: '2026-11-27T14:00:00Z',
    timeZone: 'Europe/Istanbul',
    holdExpiresUtc: '2026-11-24T07:09:05Z',
    generatedUtc: nowUtc,
    items: [{ serviceName: 'Masaj', staffName: 'Seda Kurt', durationMinutes: 60, price: tl(900) }],
    total: tl(900),
    amountDue: tl(900),
    requiresApproval: false,
    appointmentAccessCode: null,
    waitlistAccessCode: waitlistCode,
    version: 'W/"9420"',
    ...overrides,
  };
}

export function reviewInviteFixture(overrides: Partial<ReviewInvite> = {}): ReviewInvite {
  return {
    brand: { name: 'Kuaför Elif', slug: 'kuafor-elif', logoUrl: null },
    branchName: 'Kadıköy',
    appointmentStartUtc: '2026-11-12T11:00:00Z',
    timeZone: 'Europe/Istanbul',
    serviceNames: ['Boya ve fön'],
    staff: { name: 'Elif Kaya', photoUrl: null },
    customerDisplayName: 'Merve Y.',
    alreadyReviewed: false,
    review: null,
    ...overrides,
  };
}

/** Gönderilmiş değerlendirme (alreadyReviewed). */
export function reviewedFixture(comment: string | null = 'Boya tam istediğim gibi oldu.') {
  return reviewInviteFixture({
    alreadyReviewed: true,
    review: { rating: 4, comment, createdUtc: '2026-11-24T06:00:00Z' },
  });
}

/** Fön paketi: 10 seansın 7'si kaldı, son kullanım 30 Mart (Onay.png). */
export function packageFixture(overrides: Partial<UsablePackageItem> = {}): UsablePackageItem {
  return {
    customerPackageItemId: '0191c000-0000-7000-8000-000000000001',
    serviceId: ids.fon,
    packageName: 'Fön paketi',
    remainingSessions: 7,
    totalSessions: 10,
    expiresOn: '2027-03-30',
    ...overrides,
  };
}
