import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:omvila_musteri/app/router.dart';
import 'package:omvila_musteri/features/appointments/presentation/randevularim_ekrani.dart';
import 'package:omvila_musteri/features/booking/presentation/booking_controller.dart';
import 'package:omvila_musteri/features/brands/data/brands_repository.dart';
import 'package:omvila_ui/omvila_ui.dart';

import '../helpers/fakes.dart';
import '../helpers/fixtures.dart';
import '../helpers/harness.dart';

Future<void> _pump(WidgetTester tester, TestDeps deps) async {
  usePhone(tester);
  await tester.pumpWidget(
    screenApp(
      // Uygulamada sekme kabuğunun Scaffold'u içindedir.
      const Scaffold(body: RandevularimEkrani()),
      deps,
      routes: [
        markerRoute(Routes.randevuAl),
        markerRoute(Routes.qr),
        markerRoute(Routes.yrHizmet),
        markerRoute(Routes.yrSaat),
        markerRoute(Routes.gecmis),
        markerRoute('/randevu/:id'),
      ],
    ),
  );
  await tester.pump();
}

void _clipboard(WidgetTester tester, String text) {
  tester.binding.defaultBinaryMessenger.setMockMethodCallHandler(
    SystemChannels.platform,
    (call) async => call.method == 'Clipboard.getData' ? {'text': text} : null,
  );
  addTearDown(
    () => tester.binding.defaultBinaryMessenger.setMockMethodCallHandler(
      SystemChannels.platform,
      null,
    ),
  );
}

void main() {
  testWidgets('yükleniyor', (tester) async {
    await _pump(
      tester,
      TestDeps(appointments: FakeAppointmentsRepository(pending: true)),
    );
    expect(find.byType(CircularProgressIndicator), findsOneWidget);
    expect(find.text('Randevularım'), findsWidgets);
  });

  testWidgets('bağlantı yoksa "Tekrar dene" yeniden yükler', (tester) async {
    final repo = FakeAppointmentsRepository(
      error: const ApiProblem(status: 0, code: ApiProblem.networkErrorCode),
    );
    await _pump(tester, TestDeps(appointments: repo));
    expect(find.text('Bağlantı yok'), findsOneWidget);
    repo
      ..error = null
      ..home = doluHome;
    await tester.tap(find.text('Tekrar dene'));
    await tester.pump();
    await tester.pump();
    expect(repo.loads, 2);
    expect(find.text('Kuaför Elif'), findsOneWidget);
  });

  testWidgets('sunucu hatası metni', (tester) async {
    await _pump(
      tester,
      TestDeps(
        appointments: FakeAppointmentsRepository(
          error: const ApiProblem(status: 500, code: 'try_again'),
        ),
      ),
    );
    expect(find.text('Sayfa yüklenemedi'), findsOneWidget);
    expect(find.text('Bir sorun oluştu. Tekrar deneyin.'), findsOneWidget);
  });

  group('dolu (Main)', () {
    testWidgets('kart detaya, "Geçmiş" Geçmiş\'e gider', (tester) async {
      await _pump(tester, TestDeps());
      await tester.tap(find.text('Nail Studio Moda'));
      await tester.pumpAndSettle();
      expect(find.text('adres:/randevu/a2'), findsOneWidget);
      tester.state<NavigatorState>(find.byType(Navigator).last).pop();
      await tester.pumpAndSettle();
      await tester.tap(find.text('14:30'));
      await tester.pumpAndSettle();
      expect(find.text('adres:/randevu/a1'), findsOneWidget);
      tester.state<NavigatorState>(find.byType(Navigator).last).pop();
      await tester.pumpAndSettle();
      await tester.tap(find.text('Geçmiş'));
      await tester.pumpAndSettle();
      expect(find.text('adres:${Routes.gecmis}'), findsOneWidget);
    });

    testWidgets('kahraman kart ve yaklaşanlar', (tester) async {
      await _pump(tester, TestDeps());
      expect(find.text('Merhaba Merve'), findsOneWidget);
      expect(find.text('YARIN · ONAYLANDI'), findsOneWidget);
      expect(find.text('18 saat sonra'), findsOneWidget);
      expect(find.text('14:30'), findsOneWidget);
      expect(
        find.text('Saç kesimi, Fön · Elif Kaya · Kadıköy'),
        findsOneWidget,
      );
      expect(find.text('Yaklaşan'), findsOneWidget);
      expect(find.text('Nail Studio Moda'), findsOneWidget);
      expect(find.text('Onay bekliyor'), findsOneWidget);
      expect(
        find.text('Salon onaylayınca SMS ile bildireceğiz'),
        findsOneWidget,
      );
      expect(
        find.text('Bildirimler kapalı. Hatırlatmalar SMS ile gelecek.'),
        findsNothing,
      );
    });

    testWidgets('tek yaklaşan randevuda "Yaklaşan" yok', (tester) async {
      await _pump(
        tester,
        TestDeps(
          appointments: FakeAppointmentsRepository(
            home: homeData(upcoming: [yarinKuaforElif]),
          ),
        ),
      );
      expect(find.text('Yaklaşan'), findsNothing);
    });

    testWidgets('Yol tarifi şube konumuyla harita açar', (tester) async {
      final deps = TestDeps();
      deps.appointments.detail = appointmentDetail(
        latitude: 40.98,
        longitude: 29.02,
      );
      await _pump(tester, deps);
      await tester.tap(find.text('Yol tarifi'));
      await tester.pump();
      expect(deps.external.urls.single.toString(), contains('40.98%2C29.02'));
    });

    testWidgets('Takvime ekle', (tester) async {
      final deps = TestDeps();
      await _pump(tester, deps);
      await tester.tap(find.text('Takvime ekle'));
      await tester.pump();
      final entry = deps.external.calendar.single;
      expect(entry.title, 'Kuaför Elif randevusu');
      expect(entry.startUtc, yarinKuaforElif.startUtc);
      expect(entry.timeZone, 'Europe/Istanbul');
    });

    testWidgets('Yeni randevu al akışın başlangıcına gider', (tester) async {
      await _pump(tester, TestDeps());
      await tester.tap(find.text('Yeni randevu al'));
      await tester.pumpAndSettle();
      expect(find.text('adres:${Routes.randevuAl}'), findsOneWidget);
    });
  });

  group('ilk giriş (MainBos, KK1)', () {
    testWidgets('QR ve bağlantı seçenekleri, nasıl çalışır', (tester) async {
      await _pump(
        tester,
        TestDeps(appointments: FakeAppointmentsRepository(home: bosHome)),
      );
      expect(find.text('Hoş geldiniz Merve'), findsOneWidget);
      expect(find.text('İlk randevunuzu alın'), findsOneWidget);
      expect(find.text('QR kodu okut'), findsOneWidget);
      expect(find.text('Bağlantıyı yapıştır'), findsOneWidget);
      expect(find.text('Nasıl çalışır?'), findsOneWidget);
      expect(find.text('Hepsi burada'), findsOneWidget);
    });

    testWidgets('yapıştırılan bağlantıyla salon eklenir', (tester) async {
      final deps = TestDeps(
        appointments: FakeAppointmentsRepository(home: bosHome),
        brands: FakeBrandsRepository(brands: [brand('Kuaför Elif')]),
      );
      _clipboard(tester, 'https://randevu.omvila.com.tr/kuafor-elif');
      await _pump(tester, deps);
      await tester.tap(find.text('Bağlantıyı yapıştır'));
      await tester.pump();
      await tester.pump();
      expect(deps.brands.saved, [('kuafor-elif', BrandSource.link)]);
      expect(find.text('Kuaför Elif salonlarınıza eklendi.'), findsOneWidget);
      expect(deps.appointments.loads, 2);
    });

    testWidgets('salon bağlantısı olmayan metin eklenmez', (tester) async {
      final deps = TestDeps(
        appointments: FakeAppointmentsRepository(home: bosHome),
      );
      _clipboard(tester, 'merhaba');
      await _pump(tester, deps);
      await tester.tap(find.text('Bağlantıyı yapıştır'));
      await tester.pump();
      await tester.pump();
      expect(deps.brands.saved, isEmpty);
      expect(
        find.textContaining('bir salon bağlantısı değil'),
        findsOneWidget,
      );
    });

    testWidgets('bilinmeyen salon bağlantısı', (tester) async {
      final deps = TestDeps(
        appointments: FakeAppointmentsRepository(home: bosHome),
        brands: FakeBrandsRepository()
          ..saveError = const ApiProblem(status: 404, code: 'not_found'),
      );
      _clipboard(tester, 'omvila.com/yok-boyle-salon');
      await _pump(tester, deps);
      await tester.tap(find.text('Bağlantıyı yapıştır'));
      await tester.pump();
      await tester.pump();
      expect(find.text('Bu bağlantıda salon bulunamadı.'), findsOneWidget);
    });

    testWidgets('QR kodu okut tarama ekranını açar', (tester) async {
      await _pump(
        tester,
        TestDeps(appointments: FakeAppointmentsRepository(home: bosHome)),
      );
      await tester.tap(find.text('QR kodu okut'));
      await tester.pumpAndSettle();
      expect(find.text('adres:${Routes.qr}'), findsOneWidget);
    });
  });

  group('yaklaşan yok (MainYaklasanYok, KK2)', () {
    testWidgets('son randevudan "Aynısını tekrar al"', (tester) async {
      await _pump(
        tester,
        TestDeps(
          appointments: FakeAppointmentsRepository(home: yaklasanYokHome),
        ),
      );
      expect(find.text('Yaklaşan randevunuz yok'), findsOneWidget);
      expect(
        find.text("Son randevunuz 12 Kasım'da Kuaför Elif'teydi."),
        findsOneWidget,
      );
      expect(
        find.text('Saç kesimi, Fön · Elif Kaya · Kuaför Elif'),
        findsOneWidget,
      );
      await tester.tap(find.text('Aynısını tekrar al'));
      await tester.pumpAndSettle();
      // Akış önceki randevuyla dolar, saat adımı açılır; geri hizmet adımı.
      expect(find.text('adres:${Routes.yrSaat}'), findsOneWidget);
      final container = ProviderScope.containerOf(
        tester.element(find.text('adres:${Routes.yrSaat}')),
      );
      final draft = container.read(bookingProvider)!;
      expect(draft.brandName, 'Kuaför Elif');
      expect(draft.serviceIds, ['svc-Saç kesimi', 'svc-Fön']);
      expect(draft.staff.values.toSet(), {'stf-Elif Kaya'});
      expect(draft.sameStaffForAll, isTrue);
      tester.state<NavigatorState>(find.byType(Navigator).last).pop();
      await tester.pumpAndSettle();
      expect(find.text('adres:${Routes.yrHizmet}'), findsOneWidget);
    });

    testWidgets('"Geçmiş randevular" Geçmiş\'i açar', (tester) async {
      await _pump(
        tester,
        TestDeps(
          appointments: FakeAppointmentsRepository(home: yaklasanYokHome),
        ),
      );
      await tester.tap(find.text('Geçmiş randevular'));
      await tester.pumpAndSettle();
      expect(find.text('adres:${Routes.gecmis}'), findsOneWidget);
    });

    testWidgets('tekrar alınamıyorsa yalnız "Başka randevu al"', (
      tester,
    ) async {
      await _pump(
        tester,
        TestDeps(
          appointments: FakeAppointmentsRepository(
            home: homeData(
              last: appointment(
                id: 'x',
                brand: 'Spa Moda',
                start: '2026-11-02T08:00:00Z',
                items: [item('Masaj', '2026-11-02T08:00:00Z', 60)],
                canRebook: false,
              ),
            ),
          ),
        ),
      );
      expect(find.text('Aynısını tekrar al'), findsNothing);
      expect(find.text('Başka randevu al'), findsOneWidget);
    });

    testWidgets('salon var, randevu hiç yok', (tester) async {
      await _pump(
        tester,
        TestDeps(appointments: FakeAppointmentsRepository(home: homeData())),
      );
      expect(find.text('Salonunuzdan ilk randevunuzu alın.'), findsOneWidget);
      expect(find.text('Yeni randevu al'), findsOneWidget);
    });
  });

  group('bildirim kapalı şeridi', () {
    testWidgets('"Şimdi değil" sonrası görünür, kapatılır', (tester) async {
      final deps = TestDeps(store: signedInStore(skipped: true));
      await _pump(tester, deps);
      await tester.pump();
      expect(
        find.text('Bildirimler kapalı. Hatırlatmalar SMS ile gelecek.'),
        findsOneWidget,
      );
      await tester.tap(find.byTooltip('Kapat'));
      await tester.pump();
      expect(
        find.text('Bildirimler kapalı. Hatırlatmalar SMS ile gelecek.'),
        findsNothing,
      );
      expect(deps.store.values['push.bannerDismissed'], '1');
    });

    testWidgets('"Ayarlardan aç" sistem istemini açar', (tester) async {
      final deps = TestDeps(store: signedInStore(skipped: true));
      await _pump(tester, deps);
      await tester.pump();
      await tester.tap(find.text('Ayarlardan aç'));
      await tester.pump();
      expect(deps.push.requests, 1);
      expect(deps.store.values['push.skipped'], '0');
    });
  });
}
