// Takvim, randevu ayrıntısı ve yeni randevu için test verisi (sözleşme
// örnekleriyle aynı biçim; gerçek kişi yok). Saat 2026-11-25T09:10Z =
// İstanbul 12:10.

/// Şube.
const branchId = '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5a01';

/// Personel kimlikleri (Elif sahip, Can personel oturumundaki staffId).
const elifId = '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5e42';
const canId = '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5e41';
const zeynepId = '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5e43';

/// Gelmiş randevu (RandevuDetay tasarımı).
const checkedInId = '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f6d02';

/// Kapora bekleyen randevu (KaporaOnay).
const depositId = '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5a71';

/// Onaylı randevu.
const confirmedId = '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f6d01';

/// Müşteri kaydı.
const customerId = '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5b12';

/// Hizmetler.
const boyaId = '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5d32';
const fonId = '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5d33';
const kesimId = '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5d31';

Map<String, Object?> _money(int minor) => {
  'amountMinor': minor,
  'currency': 'TRY',
};

Map<String, Object?> _item({
  required String id,
  required String serviceId,
  required String name,
  required String start,
  required String end,
  required int price,
  String staffId = elifId,
  String staffName = 'Elif Kaya',
  bool paidFromPackage = false,
}) => {
  'id': id,
  'serviceId': serviceId,
  'serviceName': name,
  'staffId': staffId,
  'staffName': staffName,
  'startUtc': start,
  'endUtc': end,
  'price': _money(price),
  'paidFromPackage': paidFromPackage,
  'status': 'Active',
  'segments': [
    {'startUtc': start, 'endUtc': end, 'staffBusy': true},
  ],
};

/// Gelmiş randevu: Boya + Fön, kapora 200 ₺ havale ile alındı, kalan 1.240.
Map<String, Object?> checkedInJson({
  String? phone,
  String version = 'v3',
}) => {
  'id': checkedInId,
  'branchId': branchId,
  'status': 'CheckedIn',
  'displayStatus': 'CheckedIn',
  'channel': 'Online',
  'startUtc': '2026-11-25T07:30:00Z',
  'endUtc': '2026-11-25T09:00:00Z',
  'timeZone': 'Europe/Istanbul',
  'customer': {
    'brandCustomerId': customerId,
    'fullName': 'Selin Öztürk',
    'phone': ?phone,
    'phoneMasked': '0532 *** ** 21',
    'visitCount': 12,
    'lastVisitOn': '2026-10-21',
    'isNew': false,
  },
  'items': [
    _item(
      id: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5c22',
      serviceId: boyaId,
      name: 'Boya',
      start: '2026-11-25T07:30:00Z',
      end: '2026-11-25T08:45:00Z',
      price: 125000,
    ),
    _item(
      id: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5c23',
      serviceId: fonId,
      name: 'Fön',
      start: '2026-11-25T08:45:00Z',
      end: '2026-11-25T09:00:00Z',
      price: 19000,
    ),
  ],
  'deposit': {
    'required': _money(20000),
    'status': 'Received',
    'transferCode': 'OMV-3P9A',
    'receivedUtc': '2026-11-21T10:00:00Z',
  },
  'total': _money(144000),
  'amountDue': _money(124000),
  'customerNote': 'Geçen sefer 7.1 kullanıldı, aynısı isteniyor.',
  'isRecurring': false,
  'history': [
    {'occurredUtc': '2026-11-20T09:00:00Z', 'kind': 'Created'},
    {'occurredUtc': '2026-11-25T07:34:00Z', 'kind': 'CheckedIn'},
  ],
  'version': version,
};

/// Kapora bekleyen talep (Gizem Arslan 14:30, 150 ₺).
Map<String, Object?> depositJson({String? remindedUtc}) => {
  'id': depositId,
  'branchId': branchId,
  'status': 'PendingApproval',
  'displayStatus': 'DepositPending',
  'channel': 'Online',
  'startUtc': '2026-11-25T11:30:00Z',
  'endUtc': '2026-11-25T12:15:00Z',
  'timeZone': 'Europe/Istanbul',
  'holdExpiresUtc': '2026-11-25T10:30:00Z',
  'customer': {
    'brandCustomerId': '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5b11',
    'fullName': 'Gizem Arslan',
    'phoneMasked': '0532 *** ** 48',
    'visitCount': 3,
    'lastVisitOn': '2026-09-12',
    'isNew': false,
  },
  'items': [
    _item(
      id: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5c21',
      serviceId: fonId,
      name: 'Fön',
      start: '2026-11-25T11:30:00Z',
      end: '2026-11-25T12:15:00Z',
      price: 45000,
      staffId: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5e44',
      staffName: 'Mert Demir',
    ),
  ],
  'deposit': {
    'required': _money(15000),
    'status': 'CustomerReported',
    'transferCode': 'OMV-7K2X',
    'customerReportedUtc': '2026-11-25T08:52:00Z',
    'remindedUtc': remindedUtc,
  },
  'total': _money(45000),
  'amountDue': _money(45000),
  'isRecurring': false,
  'history': <Object>[],
  'version': 'W/"8839"',
};

/// Onaylı randevu (Derya Şahin 13:00).
Map<String, Object?> confirmedJson() => {
  'id': confirmedId,
  'branchId': branchId,
  'status': 'Confirmed',
  'displayStatus': 'Confirmed',
  'channel': 'Phone',
  'startUtc': '2026-11-25T10:00:00Z',
  'endUtc': '2026-11-25T10:45:00Z',
  'timeZone': 'Europe/Istanbul',
  'customer': {
    'brandCustomerId': '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5b13',
    'fullName': 'Derya Şahin',
    'phoneMasked': '0532 *** ** 18',
    'visitCount': 6,
    'isNew': false,
  },
  'items': [
    _item(
      id: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5c24',
      serviceId: kesimId,
      name: 'Saç kesimi',
      start: '2026-11-25T10:00:00Z',
      end: '2026-11-25T10:45:00Z',
      price: 45000,
    ),
  ],
  'total': _money(45000),
  'amountDue': _money(45000),
  'isRecurring': false,
  'history': <Object>[],
  'version': 'v5',
};

/// Müşterinin aktif Fön paketi (2 seans kaldı).
Map<String, Object?> fonPackagePageJson() => {
  'items': [
    {
      'id': '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f8a01',
      'customer': {'brandCustomerId': customerId, 'fullName': 'Selin Öztürk'},
      'packageId': '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f8b01',
      'packageName': 'Fön paketi',
      'branchId': branchId,
      'isAvailableAllBranches': false,
      'price': _money(70000),
      'purchasedUtc': '2026-10-01T09:00:00Z',
      'expiresOn': '2026-12-31',
      'status': 'Active',
      'items': [
        {
          'customerPackageItemId': '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f8c01',
          'serviceId': fonId,
          'serviceName': 'Fön',
          'totalSessions': 5,
          'remainingSessions': 2,
        },
      ],
      'totalSessions': 5,
      'remainingSessions': 2,
      'version': 'v1',
    },
  ],
  'nextCursor': null,
};

Map<String, Object?> _staffColumn(String id, String name, String color) => {
  'staffId': id,
  'fullName': name,
  'calendarColor': color,
  'appointmentCount': 2,
  'hasGapAvailability': false,
};

Map<String, Object?> _block({
  required String appointmentId,
  required String itemId,
  required String staffId,
  required String start,
  required String end,
  required String customer,
  required String service,
  String status = 'Confirmed',
  String? displayStatus,
  List<Map<String, Object?>>? segments,
}) => {
  'appointmentId': appointmentId,
  'itemId': itemId,
  'staffId': staffId,
  'startUtc': start,
  'endUtc': end,
  'status': status,
  'displayStatus': displayStatus ?? status,
  'customerName': customer,
  'serviceName': service,
  'isRecurring': false,
  'segments':
      segments ??
      [
        {'startUtc': start, 'endUtc': end, 'staffBusy': true},
      ],
};

Map<String, Object?> _day(String date, {bool closed = false}) => {
  'date': date,
  'isClosed': closed,
  'closureNote': closed ? 'Haftalık kapalı gün.' : null,
  'branchIntervals': closed
      ? <Object>[]
      : [
          {'start': '09:00', 'end': '18:00'},
        ],
  'staffHours': <Object>[],
};

/// 23 Kasım – 6 Aralık penceresi (29 Kasım Pazar kapalı); randevular
/// 25 Kasım'da.
Map<String, Object?> calendarJson({
  bool staffOnly = false,
  bool empty = false,
}) {
  final days = [
    for (var d = 23; d <= 30; d++) _day('2026-11-$d', closed: d == 29),
    for (var d = 1; d <= 6; d++) _day('2026-12-0$d'),
  ];
  final blocks = empty
      ? <Map<String, Object?>>[]
      : [
          _block(
            appointmentId: checkedInId,
            itemId: 'i1',
            staffId: elifId,
            start: '2026-11-25T07:30:00Z',
            end: '2026-11-25T09:00:00Z',
            customer: 'Selin Öztürk',
            service: 'Boya ve fön',
            status: 'CheckedIn',
            segments: [
              {
                'startUtc': '2026-11-25T07:30:00Z',
                'endUtc': '2026-11-25T08:00:00Z',
                'staffBusy': true,
              },
              {
                'startUtc': '2026-11-25T08:00:00Z',
                'endUtc': '2026-11-25T08:40:00Z',
                'staffBusy': false,
              },
              {
                'startUtc': '2026-11-25T08:40:00Z',
                'endUtc': '2026-11-25T09:00:00Z',
                'staffBusy': true,
              },
            ],
          ),
          _block(
            appointmentId: confirmedId,
            itemId: 'i2',
            staffId: elifId,
            start: '2026-11-25T10:00:00Z',
            end: '2026-11-25T10:45:00Z',
            customer: 'Derya Şahin',
            service: 'Saç kesimi',
          ),
          if (!staffOnly) ...[
            _block(
              appointmentId: depositId,
              itemId: 'i3',
              staffId: canId,
              start: '2026-11-25T11:30:00Z',
              end: '2026-11-25T12:15:00Z',
              customer: 'Gizem Arslan',
              service: 'Fön',
              status: 'PendingApproval',
              displayStatus: 'DepositPending',
            ),
            _block(
              appointmentId: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f6d09',
              itemId: 'i4',
              staffId: zeynepId,
              start: '2026-11-25T07:00:00Z',
              end: '2026-11-25T08:30:00Z',
              customer: 'Melis Tunç',
              service: 'Manikür ve pedikür',
            ),
          ],
        ];
  return {
    'branchId': branchId,
    'timeZone': 'Europe/Istanbul',
    'from': '2026-11-23',
    'to': '2026-12-06',
    'staff': [
      _staffColumn(elifId, 'Elif Kaya', '#0F6E5C'),
      if (!staffOnly) ...[
        _staffColumn(canId, 'Can Yücel', '#C2553D'),
        _staffColumn(zeynepId, 'Zeynep Aksoy', '#8A7A5C'),
      ],
    ],
    'days': days,
    'appointments': blocks,
    'timeOffs': [
      if (!staffOnly)
        {
          'id': '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f9b01',
          'staffId': zeynepId,
          'type': 'Break',
          'startUtc': '2026-11-25T09:00:00Z',
          'endUtc': '2026-11-25T09:30:00Z',
          'isAllDay': false,
        },
    ],
  };
}

Map<String, Object?> _service(String id, String name, int minutes, int price) =>
    {
      'id': id,
      'categoryId': '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f7001',
      'name': name,
      'durationMinutes': minutes,
      'price': _money(price),
      'priceDisplay': 'Fixed',
      'bufferBeforeMinutes': 0,
      'bufferAfterMinutes': 0,
      'deposit': {'mode': 'Never'},
      'isOnlineBookable': true,
      'isActive': true,
      'stages': <Object>[],
      'staffIds': <String>[],
      'version': 'v1',
    };

/// Hizmet listesi.
Map<String, Object?> servicesJson() => {
  'items': [
    _service(kesimId, 'Saç kesimi', 45, 45000),
    _service(fonId, 'Fön', 30, 19000),
    _service(boyaId, 'Boya', 75, 125000),
    _service('0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5d34', 'Keratin', 90, 250000),
  ],
};

Map<String, Object?> _staff(String id, String name) => {
  'id': id,
  'fullName': name,
  'calendarColor': '#0F6E5C',
  'isBookableOnline': true,
  'isActive': true,
  'branchIds': [branchId],
};

/// Personel listesi.
Map<String, Object?> staffJson() => {
  'items': [
    _staff(elifId, 'Elif Kaya'),
    _staff(canId, 'Can Yücel'),
    _staff(zeynepId, 'Zeynep Aksoy'),
  ],
};

/// Boş saatler (26 Kasım, Elif): 13:15 ve 14:00 boş, 11:30 dolu.
Map<String, Object?> slotsJson({String staffId = elifId}) => {
  'timeZone': 'Europe/Istanbul',
  'date': '2026-11-26',
  'slots': [
    for (final (local, utc) in const [
      ('13:15', '2026-11-26T10:15:00Z'),
      ('14:00', '2026-11-26T11:00:00Z'),
    ])
      {
        'startUtc': utc,
        'localTime': local,
        'period': 'Afternoon',
        'items': [
          {
            'serviceId': kesimId,
            'staffId': staffId,
            'staffName': 'Elif Kaya',
            'startUtc': utc,
            'endUtc': utc,
          },
        ],
        'total': _money(45000),
      },
  ],
  'unavailableTimes': ['11:30'],
};

/// Müşteri araması.
Map<String, Object?> customersJson() => {
  'items': [
    {
      'id': '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5b13',
      'fullName': 'Derya Şahin',
      'phoneMasked': '0532 *** ** 18',
      'visitCount': 6,
      'noShowCount': 0,
      'tags': <String>[],
    },
  ],
  'nextCursor': null,
};
