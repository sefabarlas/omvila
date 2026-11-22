import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:omvila_musteri/app/router.dart';
import 'package:omvila_musteri/features/brands/data/brands_repository.dart';
import 'package:omvila_musteri/features/notifications/presentation/bildirimler_ekrani.dart';
import 'package:omvila_ui/omvila_ui.dart';

import '../helpers/booking_fixtures.dart';
import '../helpers/fakes.dart';
import '../helpers/fixtures.dart';
import '../helpers/harness.dart';
import '../helpers/hesap_fixtures.dart';

const _banner = 'Bildirimler kapalı. Hatırlatmalar SMS ile gelecek.';

Future<TestDeps> _start(
  WidgetTester tester, {
  Map<String, String>? store,
  String? location,
  FakeBrandsRepository? brands,
  FakeAppointmentsRepository? appointments,
  FakeWaitlistRepository? waitlist,
}) async {
  usePhone(tester);
  final deps = TestDeps(
    store: store,
    brands: brands,
    appointments: appointments,
    waitlist: waitlist,
  );
  final container = deps.container();
  addTearDown(container.dispose);
  await tester.pumpWidget(routedApp(container));
  await tester.pump();
  if (location != null) {
    container.read(routerProvider).go(location);
  }

  await tester.pumpAndSettle();
  return deps;
}

void main() {
  testWidgets('oturum yoksa Giriş', (tester) async {
    await _start(tester);
    expect(find.text('Kod gönder'), findsOneWidget);
  });

  testWidgets('oturum açıksa Randevularım ve sekmeler', (tester) async {
    await _start(tester, store: signedInStore());
    expect(find.text('Merhaba Merve'), findsOneWidget);
    expect(find.byType(YuzenSekmeCubugu), findsOneWidget);
    await tester.tap(find.bySemanticsLabel('Paketlerim'));
    await tester.pumpAndSettle();
    expect(find.text('Henüz paketiniz yok'), findsOneWidget);
  });

  testWidgets('ilk açılışta bildirim izni, "Şimdi değil" şerit bırakır', (
    tester,
  ) async {
    await _start(tester, store: signedInStore(asked: false));
    expect(find.text('Randevunuzu kaçırmayın'), findsOneWidget);
    await tester.tap(find.text('Şimdi değil'));
    await tester.pumpAndSettle();
    expect(find.text('Merhaba Merve'), findsOneWidget);
    expect(find.text(_banner), findsOneWidget);
  });

  testWidgets('tanımsız adres "Bu bağlantı geçersiz"', (tester) async {
    await _start(tester, store: signedInStore(), location: '/x/y/z');
    expect(find.text('Bu bağlantı geçersiz'), findsOneWidget);
    await tester.tap(find.text('Randevularıma git'));
    await tester.pumpAndSettle();
    expect(find.text('Merhaba Merve'), findsOneWidget);
  });

  testWidgets('bekleme teklifi bağlantısı /b/ teklifi açar', (tester) async {
    await _start(
      tester,
      store: signedInStore(),
      location: '/b/ABC123',
      waitlist: FakeWaitlistRepository(offerValue: teklif()),
    );
    expect(find.text('Beklediğiniz saat boşaldı'), findsOneWidget);
    expect(find.text('12:48'), findsOneWidget);
  });

  testWidgets('zil Bildirimler ekranını açar', (tester) async {
    await _start(tester, store: signedInStore());
    await tester.tap(find.byType(BildirimZili));
    await tester.pumpAndSettle();
    expect(find.text('Bildiriminiz yok'), findsOneWidget);
  });

  testWidgets('/r/ müşterinin randevusunu açar', (tester) async {
    final repo = FakeAppointmentsRepository(home: doluHome, detail: detay())
      ..byCode = {'ABC123': detay()};
    await _start(
      tester,
      store: signedInStore(),
      location: '/r/ABC123',
      appointments: repo,
    );
    expect(find.text('Moda Cad. 42, Kadıköy'), findsOneWidget);
    expect(find.text('Randevuyu iptal et'), findsOneWidget);
  });

  testWidgets('/r/ kodu müşterinin değilse "Bu bağlantı geçersiz"', (
    tester,
  ) async {
    await _start(tester, store: signedInStore(), location: '/r/BASKA1');
    expect(find.text('Bu bağlantı geçersiz'), findsOneWidget);
  });

  testWidgets('/d/ değerlendirme ekranını açar', (tester) async {
    final repo = FakeAppointmentsRepository(home: doluHome)
      ..review = degerlendirme(code: 'D1');
    await _start(
      tester,
      store: signedInStore(),
      location: '/d/D1',
      appointments: repo,
    );
    expect(find.text("Kuaför Elif'te nasıldı?"), findsOneWidget);
  });

  testWidgets('girişsiz /r/ bağlantısı girişten sonra açılır', (tester) async {
    await _start(tester, location: '/r/ABC123');
    expect(find.text('Kod gönder'), findsOneWidget);
  });

  testWidgets('salon bağlantısı salonu ekler', (tester) async {
    final deps = await _start(
      tester,
      store: signedInStore(),
      location: '/kuafor-elif',
      brands: FakeBrandsRepository(brands: [brand('Kuaför Elif')]),
    );
    expect(deps.brands.saved, [('kuafor-elif', BrandSource.link)]);
    expect(find.text('Merhaba Merve'), findsOneWidget);
    expect(find.text('Kuaför Elif salonlarınıza eklendi.'), findsOneWidget);
  });

  testWidgets('bulunmayan salon bağlantısı geçersiz', (tester) async {
    await _start(
      tester,
      store: signedInStore(),
      location: '/yok-boyle-salon',
      brands: FakeBrandsRepository()
        ..saveError = const ApiProblem(status: 404, code: 'not_found'),
    );
    expect(find.text('Bu bağlantı geçersiz'), findsOneWidget);
  });

  testWidgets('girişsiz açılan salon bağlantısına girişten sonra dönülür', (
    tester,
  ) async {
    final deps = await _start(
      tester,
      location: '/kuafor-elif',
      brands: FakeBrandsRepository(brands: [brand('Kuaför Elif')]),
    );
    expect(find.text('Kod gönder'), findsOneWidget);
    expect(deps.brands.saved, isEmpty);

    await tester.enterText(find.byType(TextField), '5326112467');
    await tester.pump();
    await tester.tap(find.text('Kod gönder'));
    await tester.pumpAndSettle();
    await tester.enterText(find.byType(TextField), '482193');
    await tester.pumpAndSettle();
    expect(find.text('Randevunuzu kaçırmayın'), findsOneWidget);
    await tester.tap(find.text('Bildirimlere izin ver'));
    await tester.pumpAndSettle();
    expect(deps.brands.saved, [('kuafor-elif', BrandSource.link)]);
    expect(find.text('Merhaba Merve'), findsOneWidget);
  });
}
