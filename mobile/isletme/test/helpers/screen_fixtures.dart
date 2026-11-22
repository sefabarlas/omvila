// Kapıda sıra, müşteriler, bildirimler, profil ve bildirim ayarları test
// verisi; sözleşme örnekleri ve tasarım ekranlarıyla aynı (gerçek kişi yok).
import 'appointment_fixtures.dart';

Map<String, Object?> _money(int minor) => {
  'amountMinor': minor,
  'currency': 'TRY',
};

/// Kapıda sıra bileti.
const okanTicketId = '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fb001';
const muratTicketId = '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fb002';
const emreTicketId = '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fb003';

Map<String, Object?> _ticket({
  required String id,
  required int number,
  required String name,
  required String joinedUtc,
  String? serviceId = kesimId,
  String? serviceName = 'Saç kesimi',
  String? staffId = zeynepId,
  String? staffName = 'Zeynep Aksoy',
  String status = 'Waiting',
}) => {
  'id': id,
  'branchId': branchId,
  'queueDate': '2026-11-25',
  'queueNumber': number,
  'status': status,
  'customerName': name,
  'brandCustomerId': null,
  'phoneMasked': null,
  'serviceId': serviceId,
  'serviceName': serviceName,
  'preferredStaffId': staffId,
  'preferredStaffName': staffName,
  'joinedUtc': joinedUtc,
  'estimatedWaitMinutes': 0,
  'appointmentId': null,
  'version': 'w$number',
};

/// Sıra (Sira): üç kişi, yeni gelen için ~35 dk; [empty] SiraBos.
Map<String, Object?> walkInQueueJson({bool empty = false}) => {
  'branchId': branchId,
  'date': '2026-11-25',
  'timeZone': 'Europe/Istanbul',
  'generatedUtc': '2026-11-25T09:10:00Z',
  'items': empty
      ? <Object>[]
      : [
          _ticket(
            id: okanTicketId,
            number: 1,
            name: 'Okan Er',
            joinedUtc: '2026-11-25T08:58:00Z',
          ),
          _ticket(
            id: muratTicketId,
            number: 2,
            name: 'Murat Çelik',
            joinedUtc: '2026-11-25T09:02:00Z',
            serviceName: 'Sakal',
            serviceId: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5d38',
            staffId: null,
            staffName: null,
          ),
          _ticket(
            id: emreTicketId,
            number: 3,
            name: 'Emre Yavuz',
            joinedUtc: '2026-11-25T09:08:00Z',
          ),
        ],
  'waitingCount': empty ? 0 : 3,
  'oldestJoinedUtc': empty ? null : '2026-11-25T08:58:00Z',
  'estimatedWaitMinutesForNew': empty ? 0 : 35,
  'servedCount': 7,
  'nextQueueNumber': empty ? 1 : 4,
};

/// "Sıraya ekle" yanıtı (sunucunun verdiği numara).
Map<String, Object?> createdTicketJson() => _ticket(
  id: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fb004',
  number: 11,
  name: 'Burak',
  joinedUtc: '2026-11-25T09:10:00Z',
);

/// "Başlat" yanıtı.
Map<String, Object?> startedJson(String ticketId) => {
  'ticket': {
    ..._ticket(
      id: ticketId,
      number: 1,
      name: 'Okan Er',
      joinedUtc: '2026-11-25T08:58:00Z',
      status: 'Served',
    ),
  },
  'appointment': confirmedJson(),
};

/// Müşteri kartındaki kişi (Selin Öztürk).
const String selinId = customerId;

/// Yeni müşteri (Deniz Acar).
const denizId = '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5b20';

/// Müşteri özeti; [staff] personel rolü (telefon maskeli, tam numara yok).
Map<String, Object?> selinJson({bool staff = false}) => {
  'id': selinId,
  'fullName': 'Selin Öztürk',
  if (!staff) 'phone': '+905321234521',
  'phoneMasked': '0532 *** ** 21',
  'visitCount': 12,
  'noShowCount': 0,
  'lastVisitOn': '2026-11-21',
  'nextAppointmentUtc': '2026-11-25T07:30:00Z',
  'totalSpent': _money(1486000),
  'isNew': false,
  'isNoShowRisk': false,
  'tags': ['Düzenli', 'Ürün tercihi var'],
  'notes': 'Boya: 7.1, %6 oksidan. Amonyaksız ürün tercih ediyor.',
};

/// MusteriKartiYeni.
Map<String, Object?> denizJson() => {
  'id': denizId,
  'fullName': 'Deniz Acar',
  'phoneMasked': '0532 *** ** 18',
  'visitCount': 0,
  'noShowCount': 0,
  'lastVisitOn': null,
  'nextAppointmentUtc': '2026-11-25T09:00:00Z',
  'isNew': true,
  'isNoShowRisk': false,
  'tags': ['Yeni müşteri'],
  'notes': null,
};

/// Müşteri listesi (Musteriler); [staff] telefon maskeli.
Map<String, Object?> customerListJson({
  bool staff = false,
  String? nextCursor,
}) => {
  'items': [
    selinJson(staff: staff),
    {
      'id': '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5b14',
      'fullName': 'Gizem Arslan',
      'phoneMasked': '0533 *** ** 40',
      'visitCount': 3,
      'noShowCount': 2,
      'nextAppointmentUtc': '2026-11-25T11:30:00Z',
      'isNew': false,
      'isNoShowRisk': true,
      'tags': ['2 kez gelmedi'],
    },
    {
      'id': '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5b15',
      'fullName': 'Buse Kaya',
      'phoneMasked': '0535 *** ** 77',
      'visitCount': 9,
      'noShowCount': 0,
      'lastVisitOn': '2026-11-12',
      'isNew': false,
      'isNoShowRisk': false,
      'tags': <String>[],
    },
  ],
  'nextCursor': nextCursor,
};

/// İkinci sayfa.
Map<String, Object?> customerListPage2Json() => {
  'items': [
    {
      'id': '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5b16',
      'fullName': 'Nur Ateş',
      'phoneMasked': '0536 *** ** 12',
      'visitCount': 0,
      'noShowCount': 0,
      'isNew': true,
      'isNoShowRisk': false,
      'tags': ['Yeni'],
    },
  ],
  'nextCursor': null,
};

/// Boş sayfa.
const Map<String, Object?> emptyPageJson = {
  'items': <Object>[],
  'nextCursor': null,
};

/// Kart geçmişi.
Map<String, Object?> customerAppointmentsJson() => {
  'items': [
    {
      'appointmentId': checkedInId,
      'branchId': branchId,
      'startUtc': '2026-11-25T07:30:00Z',
      'status': 'CheckedIn',
      'displayStatus': 'CheckedIn',
      'serviceNames': ['Boya', 'Fön'],
      'staffNames': ['Elif Kaya'],
      'total': _money(144000),
    },
    {
      'appointmentId': '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f6d09',
      'branchId': branchId,
      'startUtc': '2026-10-21T11:00:00Z',
      'status': 'Completed',
      'displayStatus': 'Completed',
      'serviceNames': ['Fön'],
      'staffNames': ['Elif Kaya'],
      'total': _money(19000),
    },
    {
      'appointmentId': '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f6d08',
      'branchId': branchId,
      'startUtc': '2026-09-12T11:00:00Z',
      'status': 'Completed',
      'displayStatus': 'Completed',
      'serviceNames': ['Saç kesimi'],
      'staffNames': ['Elif Kaya'],
      'total': _money(45000),
    },
  ],
  'nextCursor': null,
};

/// Yeni müşterinin yaklaşan ilk randevusu.
Map<String, Object?> denizAppointmentsJson() => {
  'items': [
    {
      'appointmentId': pendingDenizId,
      'branchId': branchId,
      'startUtc': '2026-11-25T09:00:00Z',
      'status': 'PendingApproval',
      'displayStatus': 'PendingApproval',
      'serviceNames': ['Saç kesimi'],
      'staffNames': ['Can Yücel'],
      'total': _money(45000),
    },
  ],
  'nextCursor': null,
};

/// Bildirimdeki onay bekleyen talep.
const pendingDenizId = '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5a61';

/// Bildirimler (sözleşme örneği + kapora + dünden eşleşme).
Map<String, Object?> notificationsJson({bool empty = false}) => {
  'unreadCount': empty ? 0 : 3,
  'nextCursor': null,
  'items': empty
      ? <Object>[]
      : [
          {
            'id': '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5b01',
            'event': 'staff.approval_needed',
            'title': 'Yeni online randevu',
            'body': 'Pınar Yalçın · Cilt bakımı, 16:00 · Zeynep.',
            'createdUtc': '2026-11-25T09:02:00Z',
            'readUtc': null,
            'branchId': branchId,
            'target': {
              'type': 'Appointment',
              'id': pendingDenizId,
              'holdExpiresUtc': '2026-11-25T10:45:00Z',
            },
          },
          {
            'id': '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5b04',
            'event': 'staff.deposit_reported',
            'title': 'Kapora bekleniyor',
            'body':
                'Gizem Arslan · Fön, 14:30. Müşteri havaleyi yaptığını '
                'bildirdi; gelince işaretleyin.',
            'createdUtc': '2026-11-25T08:52:00Z',
            'readUtc': null,
            'branchId': branchId,
            'target': {'type': 'Deposit', 'id': depositId},
          },
          {
            'id': '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5b02',
            'event': 'staff.appointment_changed',
            'title': 'Müşteri iptal etti',
            'body': 'Hande Er · Saç kesimi, 15:00 · Mert. Saat yeniden açıldı.',
            'createdUtc': '2026-11-25T07:15:00Z',
            'readUtc': null,
            'branchId': branchId,
            'target': {'type': 'Calendar', 'id': confirmedId},
          },
          {
            'id': '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5b03',
            'event': 'staff.walkin_queue',
            'title': 'Kapıda sıra',
            'body': 'Sırada 3 kişi var.',
            'createdUtc': '2026-11-24T15:20:00Z',
            'readUtc': '2026-11-24T15:30:00Z',
            'branchId': branchId,
            'target': {'type': 'WalkInQueue', 'id': null},
          },
        ],
};

/// Primim (sözleşme örneği; Elif Kaya).
Map<String, Object?> commissionJson() => {
  'staffId': elifId,
  'staffName': 'Elif Kaya',
  'timeZone': 'Europe/Istanbul',
  'month': '2026-11',
  'from': '2026-11-01',
  'to': '2026-11-25',
  'totalMinor': 684000,
  'serviceRevenueMinor': 2850000,
  'currency': 'TRY',
  'appointmentCount': 94,
  'noShowPercent': 4,
  'averageRating': 4.9,
  'daily': [
    {'date': '2026-11-24', 'amountMinor': 42000},
    {'date': '2026-11-25', 'amountMinor': 9000},
  ],
  'thisWeek': [
    {'date': '2026-11-24', 'amountMinor': 42000, 'appointmentCount': 6},
    {'date': '2026-11-25', 'amountMinor': 9000, 'appointmentCount': 9},
  ],
  'entries': [
    {
      'id': '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f6d10',
      'type': 'Service',
      'amountMinor': 9000,
      'status': 'Accrued',
      'occurredUtc': '2026-11-25T06:30:00Z',
      'customerName': 'Ayşe Yılmaz',
      'description': 'Saç kesimi',
      'appointmentId': '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f6d03',
    },
    {
      'id': '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f6d11',
      'type': 'Service',
      'amountMinor': 42000,
      'status': 'Accrued',
      'occurredUtc': '2026-11-24T14:00:00Z',
      'customerName': 'Buse Kaya',
      'description': 'Keratin',
      'appointmentId': '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f6d04',
    },
    {
      'id': '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f6d12',
      'type': 'Service',
      'amountMinor': 19000,
      'status': 'Approved',
      'occurredUtc': '2026-11-20T10:00:00Z',
      'customerName': 'Derya Şahin',
      'description': 'Fön',
      'appointmentId': '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f6d05',
    },
  ],
  'nextAccrual': {
    'appointmentId': checkedInId,
    'customerName': 'Selin Öztürk',
  },
};

/// Kişisel bildirim tercihleri (sözleşme örneği).
Map<String, Object?> preferencesJson({String version = 'W/"412"'}) => {
  'newOnlineAppointment': true,
  'cancellationAndReschedule': true,
  'customerCheckedIn': false,
  'walkInQueue': true,
  'newReview': true,
  'dailySummary': false,
  'dailySummaryLocalTime': '20:00',
  'quietHoursEnabled': true,
  'quietHoursStartLocal': '22:00',
  'quietHoursEndLocal': '08:00',
  'version': version,
};
