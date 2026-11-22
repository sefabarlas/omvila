import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:go_router/go_router.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/app/router.dart';
import 'package:omvila_musteri/features/booking/domain/booking_draft.dart';
import 'package:omvila_musteri/features/booking/presentation/booking_controller.dart';
import 'package:omvila_musteri/features/booking/presentation/hizmet_ekrani.dart';
import 'package:omvila_musteri/features/booking/presentation/onay_ekrani.dart';
import 'package:omvila_musteri/features/booking/presentation/personel_ekrani.dart';
import 'package:omvila_musteri/features/booking/presentation/saat_ekrani.dart';
import 'package:omvila_musteri/features/booking/presentation/salon_secimi_ekrani.dart';
import 'package:omvila_ui/omvila_ui.dart';

import '../helpers/booking_fixtures.dart';
import '../helpers/fakes.dart';
import '../helpers/fixtures.dart';
import '../helpers/harness.dart';

// Randevu alma (YRSalon → YRHizmet → YRPersonel → YRSaat → YROnay):
// rezervasyon sayfasıyla aynı akış ve kurallar; kod adımı yok, kart formu yok.

final List<GoRoute> _routes = [
  for (final r in [
    Routes.yrHizmet,
    Routes.yrPersonel,
    Routes.yrSaat,
    Routes.yrOnay,
    '/randevu/:id/havale',
    '/randevu/:id/tamam',
  ])
    markerRoute(r),
];

Future<TestDeps> _pump(
  WidgetTester tester,
  Widget screen, {
  BookingDraft? draft,
  FakeBookingRepository? booking,
  FakeBrandsRepository? brands,
}) async {
  usePhone(tester);
  final deps = TestDeps(booking: booking, brands: brands);
  await tester.pumpWidget(
    screenApp(
      screen,
      deps,
      routes: _routes,
      extraOverrides: [if (draft != null) seedBooking(draft)],
    ),
  );
  for (var i = 0; i < 4; i++) {
    await tester.pump();
  }

  return deps;
}

BookingDraft _read(WidgetTester tester, Type screen) =>
    ProviderScope.containerOf(
      tester.element(find.byType(screen)),
    ).read(bookingProvider)!;

void main() {
  test('sayıya iyelik eki', () {
    expect(
      [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 20, 40, 60, 100].map(sayiIyelik),
      [
        "1'i",
        "2'si",
        "3'ü",
        "4'ü",
        "5'i",
        "6'sı",
        "7'si",
        "8'i",
        "9'u",
        "10'u",
        "20'si",
        "40'ı",
        "60'ı",
        "100'ü",
      ],
    );
  });

  test('tutar: paketten düşülen hariç, kişi farkı dahil', () {
    final s = BookingSummary.of(
      kuaforElifKatalog(depositLira: 150),
      taslak(
        services: ['kesim', 'fon'],
        staff: {'kesim': 'zeynep', 'fon': 'zeynep'},
        packages: {'fon': 'cpi-fon'},
      ),
    );
    expect(s.totalMinor, 50000);
    expect(s.depositMinor, 15000);
    expect(s.durationMinutes, 75);
  });

  group('Hangi salon? (YRSalon)', () {
    testWidgets('salon seçilince akış başlar, hizmet adımı açılır', (
      tester,
    ) async {
      await _pump(
        tester,
        const SalonSecimiEkrani(),
        brands: FakeBrandsRepository(brands: [brand('Kuaför Elif')]),
      );
      await tester.tap(find.text('Kuaför Elif'));
      await tester.pumpAndSettle();
      expect(find.text('adres:${Routes.yrHizmet}'), findsOneWidget);
      final draft = ProviderScope.containerOf(
        tester.element(find.text('adres:${Routes.yrHizmet}')),
      ).read(bookingProvider)!;
      expect(draft.branchId, 'br-kuafor-elif');
      expect(draft.branchName, 'Kadıköy');
    });

    testWidgets('online randevu kapalı salon seçilemez', (tester) async {
      final closed = MeBrand(
        id: 'b',
        name: 'Spa Moda',
        slug: 'spa-moda',
        logoUrl: null,
        coverColor: MeBrandCoverColorEnum.kum,
        bookingEnabled: false,
        hasBooked: true,
        savedUtc: null,
        lastVisitOn: null,
        branches: [MeBranchRef(id: 'x', name: 'Moda', district: 'Moda')],
        activePackages: const [],
      );
      await _pump(
        tester,
        const SalonSecimiEkrani(),
        brands: FakeBrandsRepository(brands: [closed]),
      );
      expect(
        find.text('Online randevu kapalı · Salonu arayın'),
        findsOneWidget,
      );
      await tester.tap(find.text('Spa Moda'));
      await tester.pumpAndSettle();
      expect(find.text('adres:${Routes.yrHizmet}'), findsNothing);
    });
  });

  group('Hizmet seçin (YRHizmet)', () {
    testWidgets('yükleniyor', (tester) async {
      await _pump(
        tester,
        const HizmetEkrani(),
        draft: taslak(),
        booking: FakeBookingRepository()..catalogPending = true,
      );
      expect(find.byType(CircularProgressIndicator), findsOneWidget);
    });

    testWidgets('hata ve "Tekrar dene"', (tester) async {
      final booking = FakeBookingRepository(
        catalogError: const ApiProblem(status: 0, code: 'network_error'),
      );
      await _pump(
        tester,
        const HizmetEkrani(),
        draft: taslak(),
        booking: booking,
      );
      expect(find.text('Bağlantı yok'), findsOneWidget);
      booking
        ..catalogError = null
        ..catalogValue = kuaforElifKatalog();
      await tester.tap(find.text('Tekrar dene'));
      await settle(tester);
      expect(find.text('Saç kesimi'), findsOneWidget);
    });

    testWidgets('boş: online hizmet yok', (tester) async {
      final empty = PublicCatalog(
        branchId: 'b',
        timeZone: istanbul,
        allowStaffSelection: true,
        allowAnyStaffOption: true,
        categories: const [],
        services: const [],
        staff: const [],
      );
      await _pump(
        tester,
        const HizmetEkrani(),
        draft: taslak(),
        booking: FakeBookingRepository(catalogValue: empty),
      );
      expect(find.text('Online hizmet yok'), findsOneWidget);
    });

    testWidgets('kategori, fiyat, "Paketten" ve alt çubuk', (tester) async {
      await _pump(
        tester,
        const HizmetEkrani(),
        draft: taslak(),
        booking: FakeBookingRepository(
          catalogValue: kuaforElifKatalog(),
          usable: [fonPaketi],
        ),
      );
      expect(
        find.text('Kadıköy şubesi · birden fazla seçebilirsiniz'),
        findsOneWidget,
      );
      expect(find.text('450\u00a0₺'), findsOneWidget);
      expect(find.text("1.100\u00a0₺'den"), findsOneWidget);
      expect(find.text('Paketten'), findsOneWidget);
      expect(find.text("30 dk · 5 seansın 2'si kaldı"), findsOneWidget);
      expect(find.text('Devam et'), findsNothing);

      await tester.tap(find.text('Saç kesimi'));
      await tester.pump();
      await tester.tap(find.text('Fön'));
      await tester.pump();
      expect(find.text('2 hizmet · 1 sa 15 dk'), findsOneWidget);
      // Fön paketten: tutar yalnız saç kesimi.
      expect(find.text('450\u00a0₺'), findsNWidgets(2));
      final draft = _read(tester, HizmetEkrani);
      expect(draft.serviceIds, ['kesim', 'fon']);
      expect(draft.packageItems, {'fon': 'cpi-fon'});

      await tester.tap(find.text('Renk').first);
      await tester.pump();
      expect(find.text('Saç kesimi'), findsNothing);
      expect(find.text('Dip boya'), findsOneWidget);

      await tester.tap(find.text('Devam et'));
      await tester.pumpAndSettle();
      expect(find.text('adres:${Routes.yrPersonel}'), findsOneWidget);
    });
  });

  group('Kimden olsun? (YRPersonel)', () {
    testWidgets('"Fark etmez", kişi farkı ve aynı kişi anahtarı', (
      tester,
    ) async {
      await _pump(
        tester,
        const PersonelEkrani(),
        draft: taslak(services: ['kesim', 'fon']),
        booking: FakeBookingRepository(catalogValue: kuaforElifKatalog()),
      );
      expect(find.text('Fark etmez'), findsOneWidget);
      expect(find.text('En erken uygun saat'), findsOneWidget);
      expect(find.text('+50\u00a0₺'), findsOneWidget);
      expect(find.text('İki hizmet de aynı kişiden'), findsOneWidget);
      // Kişi seçilmeden ilerlenmez.
      final button = find.widgetWithText(FilledButton, 'Devam et');
      expect(tester.widget<FilledButton>(button).onPressed, isNull);

      await tester.tap(find.text('Zeynep Aksoy'));
      await tester.pump();
      expect(find.text('750\u00a0₺ · 1 sa 15 dk'), findsOneWidget);
      expect(_read(tester, PersonelEkrani).staff, {
        'kesim': 'zeynep',
        'fon': 'zeynep',
      });

      await tester.tap(find.byType(Switch));
      await tester.pump();
      expect(find.text('Kişi seçin'), findsOneWidget);
      // Her hizmete ayrı seçim.
      expect(find.text('Fark etmez'), findsNWidgets(2));
      await tester.tap(find.text('Fark etmez').first);
      await tester.pump();
      await tester.ensureVisible(find.text('Mert Demir').last);
      await tester.pumpAndSettle();
      await tester.tap(find.text('Mert Demir').last);
      await tester.pump();
      expect(_read(tester, PersonelEkrani).items.map((i) => i.staffId), [
        null,
        'mert',
      ]);
      await tester.tap(find.text('Devam et'));
      await tester.pumpAndSettle();
      expect(find.text('adres:${Routes.yrSaat}'), findsOneWidget);
    });
  });

  group('Ne zaman? (YRSaat)', () {
    BookingDraft draft() => taslak(
      services: ['kesim', 'fon'],
      staff: {'kesim': 'elif', 'fon': 'elif'},
    );

    testWidgets('gün şeridi, ızgara ve saat seçimi', (tester) async {
      final booking = FakeBookingRepository(
        catalogValue: kuaforElifKatalog(),
        dayResult: gunler(),
        slotResult: saatler(),
      );
      await _pump(tester, const SaatEkrani(), draft: draft(), booking: booking);
      expect(find.text('Elif Kaya · 1 sa 15 dk'), findsOneWidget);
      // İlk boş gün (25 Kasım) seçili.
      expect(booking.slotRequests, ['2026-11-25']);
      expect(find.text('Öğleden sonra'), findsOneWidget);
      expect(find.bySemanticsLabel('13:30, dolu'), findsOneWidget);
      expect(find.text('Uygun saat yok mu?'), findsOneWidget);

      await tester.tap(find.text('14:30'));
      await tester.pump();
      expect(find.text('Çar 25 Kas · 14:30'), findsOneWidget);
      expect(find.text("Elif Kaya · 15:45'te biter"), findsOneWidget);
      await tester.tap(find.text('Devam et'));
      await tester.pumpAndSettle();
      expect(find.text('adres:${Routes.yrOnay}'), findsOneWidget);
    });

    testWidgets('dolu gün: en yakın boş gün', (tester) async {
      final booking = FakeBookingRepository(
        catalogValue: kuaforElifKatalog(),
        dayResult: gunler(),
        slotResult: saatler(),
      )..slotsByDate['2026-11-25'] = bosSaatler('2026-11-25');
      await _pump(tester, const SaatEkrani(), draft: draft(), booking: booking);
      expect(
        find.text('Bu gün dolu. En yakın boş gün: Perşembe 26 Kasım'),
        findsOneWidget,
      );
      await tester.tap(find.textContaining('En yakın boş gün'));
      await settle(tester);
      expect(booking.slotRequests.last, '2026-11-26');
    });

    testWidgets('hiç boş gün yoksa en yakın uygun saat', (tester) async {
      await _pump(
        tester,
        const SaatEkrani(),
        draft: draft(),
        booking: FakeBookingRepository(
          catalogValue: kuaforElifKatalog(),
          dayResult: gunler(
            allFull: true,
            nextUtc: DateTime.utc(2026, 12, 28, 7, 30),
          ),
          slotResult: bosSaatler('2026-11-24'),
        ),
      );
      expect(
        find.text(
          'Bu aralıkta boş saat yok. En yakın uygun saat Pazartesi 28 Aralık '
          '10:30',
        ),
        findsOneWidget,
      );
    });

    testWidgets('gün şeridi okunamazsa hata', (tester) async {
      await _pump(
        tester,
        const SaatEkrani(),
        draft: draft(),
        booking: FakeBookingRepository(catalogValue: kuaforElifKatalog())
          ..daysError = const ApiProblem(status: 500, code: 'try_again'),
      );
      expect(find.text('Sayfa yüklenemedi'), findsOneWidget);
    });

    testWidgets('"Uygun saat yok mu?" bekleme listesi sayfasını açar', (
      tester,
    ) async {
      await _pump(
        tester,
        const SaatEkrani(),
        draft: draft(),
        booking: FakeBookingRepository(
          catalogValue: kuaforElifKatalog(),
          dayResult: gunler(),
          slotResult: saatler(),
        ),
      );
      await tester.tap(find.text('Uygun saat yok mu?'));
      await tester.pumpAndSettle();
      expect(find.text('Bekleme listesine katılın'), findsOneWidget);
    });
  });

  group('Son kontrol (YROnay)', () {
    BookingDraft draft({Map<String, String> packages = const {}}) => taslak(
      services: ['kesim', 'fon'],
      staff: {'kesim': 'elif', 'fon': 'elif'},
      packages: packages,
      date: '2026-11-25',
      slot: saat('2026-11-25', '14:30'),
    );

    testWidgets('özet, paket anahtarı, kapora kutusu ve not', (tester) async {
      await _pump(
        tester,
        const OnayEkrani(),
        draft: draft(packages: {'fon': 'cpi-fon'}),
        booking: FakeBookingRepository(
          catalogValue: kuaforElifKatalog(depositLira: 150),
          usable: [fonPaketi],
        ),
      );
      expect(find.text('ÇARŞAMBA 25 KASIM'), findsOneWidget);
      expect(find.text('14:30–15:45'), findsOneWidget);
      expect(find.text('Kuaför Elif · Kadıköy · Elif Kaya'), findsOneWidget);
      expect(find.text('Paketten'), findsOneWidget);
      expect(find.text('-150\u00a0₺'), findsOneWidget);
      expect(find.text('300\u00a0₺'), findsOneWidget);
      expect(find.text('Fön paketinizden düşülsün'), findsOneWidget);
      expect(find.text("5 seansın 1'i kalacak"), findsOneWidget);
      expect(
        find.textContaining('150\u00a0₺ kapora ile kesinleşir'),
        findsOneWidget,
      );
      expect(
        find.text(
          'Uygulamadan ödeme alınmaz; havaleyi kendi bankanızdan yaparsınız.',
        ),
        findsOneWidget,
      );
      // Ad profilde var: ad alanı yok.
      expect(find.text('Adınız soyadınız'), findsNothing);

      await tester.tap(find.byType(Switch));
      await tester.pump();
      expect(find.text('250\u00a0₺'), findsOneWidget);
      expect(find.text("5 seansın 2'si kaldı"), findsOneWidget);
      expect(_read(tester, OnayEkrani).packageItems, isEmpty);
    });

    testWidgets('oluşturunca onaylı randevuda başarı ekranı', (tester) async {
      final booking = FakeBookingRepository(
        catalogValue: kuaforElifKatalog(),
        usable: [fonPaketi],
      )..createAnswers.add(olusan());
      await _pump(
        tester,
        const OnayEkrani(),
        draft: draft(packages: {'fon': 'cpi-fon'}),
        booking: booking,
      );
      await tester.enterText(find.byType(TextField), 'Geçen seferki renk');
      await tester.tap(find.text('Randevuyu oluştur'));
      await tester.pumpAndSettle();
      expect(find.text('adres:/randevu/yeni-1/tamam'), findsOneWidget);
      final (body, key) = booking.created.single;
      expect(key, isNotEmpty);
      expect(body.fullName, 'Merve Yıldız');
      expect(body.startUtc, DateTime.utc(2026, 11, 25, 11, 30));
      expect(body.customerNote, 'Geçen seferki renk');
      expect(body.privacyNoticeAcknowledged, isTrue);
      expect(body.privacyNoticeVersion, 'musteri-v1');
      expect(body.items.map((i) => (i.serviceId, i.staffId)), [
        ('kesim', 'elif'),
        ('fon', 'elif'),
      ]);
      expect(
        body.usePackageItems!.map(
          (p) => (p.serviceId, p.customerPackageItemId),
        ),
        [('fon', 'cpi-fon')],
      );
    });

    testWidgets('kapora bekleniyorsa havale ekranı', (tester) async {
      final booking = FakeBookingRepository(catalogValue: kuaforElifKatalog())
        ..createAnswers.add(
          olusan(status: DisplayStatus.depositPending, deposit: true),
        );
      await _pump(tester, const OnayEkrani(), draft: draft(), booking: booking);
      await tester.tap(find.text('Randevuyu oluştur'));
      await tester.pumpAndSettle();
      expect(find.text('adres:/randevu/yeni-1/havale?yeni=1'), findsOneWidget);
    });

    testWidgets('profilde ad yoksa ad sorulur', (tester) async {
      final booking = FakeBookingRepository(
        catalogValue: kuaforElifKatalog(),
        name: null,
      )..createAnswers.add(olusan());
      await _pump(tester, const OnayEkrani(), draft: draft(), booking: booking);
      final button = find.widgetWithText(FilledButton, 'Randevuyu oluştur');
      expect(tester.widget<FilledButton>(button).onPressed, isNull);
      await tester.enterText(
        find.widgetWithText(TextField, 'Adınız soyadınız'),
        'Deniz Acar',
      );
      await tester.pump();
      await tester.tap(button);
      await tester.pumpAndSettle();
      expect(booking.created.single.$1.fullName, 'Deniz Acar');
    });

    testWidgets('saat dolduysa alternatif saatler; yeni anahtarla tekrar', (
      tester,
    ) async {
      final booking = FakeBookingRepository(catalogValue: kuaforElifKatalog())
        ..createAnswers.addAll([
          const ApiProblem(
            status: 409,
            code: 'slot_taken',
            meta: {
              'alternatives': ['2026-11-25T12:15:00Z'],
            },
          ),
          olusan(),
        ]);
      await _pump(tester, const OnayEkrani(), draft: draft(), booking: booking);
      await tester.tap(find.text('Randevuyu oluştur'));
      await tester.pumpAndSettle();
      expect(
        find.text('Bu saat az önce doldu. Başka bir saat seçin.'),
        findsOneWidget,
      );
      await tester.tap(find.text('15:15'));
      await tester.pump();
      expect(find.text('15:15–16:30'), findsOneWidget);
      await tester.tap(find.text('Randevuyu oluştur'));
      await tester.pumpAndSettle();
      final [(_, first), (second, secondKey)] = booking.created;
      expect(second.startUtc, DateTime.utc(2026, 11, 25, 12, 15));
      expect(secondKey, isNot(first));
    });

    testWidgets('online randevu kapalıysa mesaj', (tester) async {
      final booking = FakeBookingRepository(catalogValue: kuaforElifKatalog())
        ..createAnswers.add(
          const ApiProblem(status: 403, code: 'booking_disabled'),
        );
      await _pump(tester, const OnayEkrani(), draft: draft(), booking: booking);
      await tester.tap(find.text('Randevuyu oluştur'));
      await tester.pumpAndSettle();
      expect(
        find.text('Online randevu kapalı. Salonu arayın.'),
        findsOneWidget,
      );
    });
  });
}
