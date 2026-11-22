// Sözleşme örnekleriyle aynı biçimde test verisi (gerçek kişi yok).
import 'package:omvila_api/omvila_api.dart';

/// Yenileme yanıtı.
const Map<String, Object?> tokenPairJson = {
  'accessToken': 'at-2',
  'expiresInSeconds': 900,
  'refreshToken': 'rt-2',
};

const _ownerPermissions = [
  'calendar.view',
  'calendar.view.all-staff',
  'appointment.create',
  'appointment.approve',
  'customer.view',
  'walkin.manage',
  'commission.view',
  'settings.manage',
];

/// İşletme sahibi oturumu.
const Map<String, Object?> ownerSessionJson = {
  'user': {
    'id': '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f0001',
    'fullName': 'Elif Kaya',
    'phoneMasked': '0532 *** ** 05',
  },
  'brand': {
    'id': '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f0002',
    'name': 'Kuaför Elif',
    'slug': 'kuafor-elif',
    'status': 'Active',
  },
  'membership': {'role': 'Owner', 'branchIds': <String>[]},
  'permissions': _ownerPermissions,
  'branches': [
    {'id': '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5a01', 'name': 'Kadıköy'},
  ],
  'subscription': {'status': 'Active'},
};

/// Personel oturumu (roller-yetkiler.md: Takvimim, Müşteriler, Kapıda sıra,
/// Primim; onay yok).
const Map<String, Object?> staffSessionJson = {
  'user': {
    'id': '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f0003',
    'fullName': 'Can Yücel',
    'phoneMasked': '0533 *** ** 11',
  },
  'brand': {
    'id': '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f0002',
    'name': 'Kuaför Elif',
    'slug': 'kuafor-elif',
    'status': 'Active',
  },
  'membership': {
    'role': 'Staff',
    'branchIds': ['0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5a01'],
    'staffId': '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5e41',
  },
  'permissions': [
    'calendar.view',
    'appointment.create',
    'customer.view',
    'walkin.manage',
    'commission.view',
  ],
  'branches': [
    {'id': '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5a01', 'name': 'Kadıköy'},
  ],
  'subscription': {'status': 'Active'},
};

/// Oturum modeli.
ManageSession session(Map<String, Object?> json) =>
    ManageSession.fromJson(json);

/// Bekleyen randevu kimliği.
const pendingId = '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5a62';

/// Bugün, dolu (sözleşme örneği).
Map<String, Object?> todayJson({
  bool staff = false,
  List<Map<String, Object?>>? items,
}) => {
  'branchId': '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5a01',
  'branchName': 'Kadıköy',
  'timeZone': 'Europe/Istanbul',
  'date': '2026-11-25',
  'generatedUtc': '2026-11-25T09:10:00Z',
  'next': {
    'appointmentId': '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f6d01',
    'startUtc': '2026-11-25T10:00:00Z',
    'endUtc': '2026-11-25T10:45:00Z',
    'status': 'Confirmed',
    'displayStatus': 'Confirmed',
    'customerName': 'Derya Şahin',
    'isNewCustomer': false,
    'serviceSummary': 'Saç kesimi',
    'staffNames': ['Elif Kaya'],
  },
  'pendingApprovalCount': staff ? null : 2,
  'walkIns': {'waitingCount': 3, 'estimatedWaitMinutesForNew': 35},
  'commission': {
    'month': '2026-11',
    'totalMinor': 684000,
    'currency': 'TRY',
    'daily': [
      {'date': '2026-11-03', 'amountMinor': 120000},
      {'date': '2026-11-10', 'amountMinor': 180000},
      {'date': '2026-11-17', 'amountMinor': 333000},
      {'date': '2026-11-24', 'amountMinor': 42000},
      {'date': '2026-11-25', 'amountMinor': 9000},
    ],
  },
  'completedCount': 4,
  'totalCount': 12,
  'items':
      items ??
      [
        {
          'appointmentId': pendingId,
          'startUtc': '2026-11-25T09:00:00Z',
          'endUtc': '2026-11-25T09:45:00Z',
          'status': 'PendingApproval',
          'displayStatus': 'PendingApproval',
          'customerName': 'Deniz Acar',
          'isNewCustomer': true,
          'serviceSummary': 'Saç kesimi',
          'staffNames': ['Can Yücel'],
          'holdExpiresUtc': '2026-11-25T11:10:00Z',
        },
        {
          'appointmentId': '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f6d02',
          'startUtc': '2026-11-25T07:30:00Z',
          'endUtc': '2026-11-25T09:00:00Z',
          'status': 'CheckedIn',
          'displayStatus': 'CheckedIn',
          'customerName': 'Selin Öztürk',
          'isNewCustomer': false,
          'serviceSummary': 'Boya ve fön',
          'staffNames': ['Elif Kaya'],
        },
      ],
};

/// Randevusuz gün (MainBos): yetkiye bağlı kartlar yine dolu olabilir.
Map<String, Object?> emptyTodayJson() => {
  ...todayJson(items: const []),
  'next': null,
  'pendingApprovalCount': 0,
  'walkIns': {'waitingCount': 0, 'estimatedWaitMinutesForNew': null},
  'commission': null,
  'completedCount': 0,
  'totalCount': 0,
};

/// Bugün modeli.
ManageToday today(Map<String, Object?> json) => ManageToday.fromJson(json);

/// Rezervasyon sayfası ayarları.
const Map<String, Object?> bookingPageJson = {
  'slug': 'kuafor-elif',
  'url': 'https://omvila.com/kuafor-elif',
  'coverColor': 'petrol',
  'showReviews': true,
  'showPrices': true,
  'version': 'v1',
};

/// Onay öncesi okunan randevu (If-Match sürümü "v7").
const Map<String, Object?> pendingDetailJson = {
  'id': pendingId,
  'branchId': '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5a01',
  'status': 'PendingApproval',
  'displayStatus': 'PendingApproval',
  'channel': 'Online',
  'startUtc': '2026-11-25T09:00:00Z',
  'endUtc': '2026-11-25T09:45:00Z',
  'timeZone': 'Europe/Istanbul',
  'customer': {
    'brandCustomerId': '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f7c01',
    'fullName': 'Deniz Acar',
    'visitCount': 0,
  },
  'items': <Object>[],
  'total': {'amountMinor': 45000, 'currency': 'TRY'},
  'amountDue': {'amountMinor': 45000, 'currency': 'TRY'},
  'isRecurring': false,
  'history': <Object>[],
  'version': 'v7',
};

/// Kod gönderildi.
const Map<String, Object?> otpChallengeJson = {
  'otpId': '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f9001',
  'expiresInSeconds': 180,
  'resendAfterSeconds': 60,
  'phoneMasked': '0532 *** ** 05',
};

/// Doğrulama: oturum açıldı.
const Map<String, Object?> signedInJson = {
  'result': 'SignedIn',
  'accessToken': 'at-1',
  'expiresInSeconds': 900,
  'refreshToken': 'rt-1',
};
