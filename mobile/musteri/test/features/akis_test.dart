import 'package:flutter_test/flutter_test.dart';
import 'package:omvila_api/omvila_api.dart';

import '../helpers/booking_fixtures.dart';
import '../helpers/fakes.dart';
import '../helpers/fixtures.dart';
import '../helpers/harness.dart';

// Uçtan uca akışlar gerçek router ve sahte depolarla.

Future<TestDeps> _start(WidgetTester tester, TestDeps deps) async {
  usePhone(tester);
  final container = deps.container();
  addTearDown(container.dispose);
  await tester.pumpWidget(routedApp(container));
  await tester.pumpAndSettle();
  return deps;
}

Future<void> _tap(WidgetTester tester, String text) async {
  final finder = find.text(text).last;
  await tester.ensureVisible(finder);
  await tester.pumpAndSettle();
  await tester.tap(finder);
  await tester.pumpAndSettle();
}

void main() {
  testWidgets(
    'yeni randevu: salon → hizmet → kişi → saat → onay → kapora → başarı',
    (tester) async {
      final appointments = FakeAppointmentsRepository(
        home: doluHome,
        detail: detay(
          status: DisplayStatus.depositPending,
          deposit: kapora(DepositStatus.awaiting),
        ),
      );
      final booking =
          FakeBookingRepository(
              catalogValue: kuaforElifKatalog(depositLira: 150),
              dayResult: gunler(),
              slotResult: saatler(),
            )
            ..createAnswers.add(
              olusan(status: DisplayStatus.depositPending, deposit: true),
            );
      await _start(
        tester,
        TestDeps(
          store: signedInStore(),
          appointments: appointments,
          booking: booking,
          brands: FakeBrandsRepository(brands: [brand('Kuaför Elif')]),
        ),
      );

      await _tap(tester, 'Yeni randevu al');
      expect(find.text('Hangi salon?'), findsOneWidget);
      await _tap(tester, 'Kuaför Elif');
      expect(find.text('Hizmet seçin'), findsOneWidget);
      await _tap(tester, 'Saç kesimi');
      await _tap(tester, 'Devam et');
      expect(find.text('Kimden olsun?'), findsOneWidget);
      await _tap(tester, 'Elif Kaya');
      await _tap(tester, 'Devam et');
      expect(find.text('Ne zaman?'), findsOneWidget);
      await _tap(tester, '14:30');
      await _tap(tester, 'Devam et');
      expect(find.text('Son kontrol'), findsOneWidget);
      await _tap(tester, 'Randevuyu oluştur');

      // Kapora: havale ekranı; bildirim sonrası başarı ekranında "onaylandı"
      // denmez.
      expect(find.text('Kaporayı havale ile gönderin'), findsOneWidget);
      appointments.detail = detay(
        status: DisplayStatus.depositPending,
        deposit: kapora(DepositStatus.customerReported),
      );
      await _tap(tester, 'Havaleyi yaptım, salona bildir');
      expect(appointments.reported, ['yeni-1']);
      expect(find.text('Havaleniz salona bildirildi'), findsOneWidget);
      expect(find.text('Randevunuz onaylandı'), findsNothing);
      await _tap(tester, 'Randevularıma dön');
      expect(find.text('Merhaba Merve'), findsOneWidget);
      expect(booking.created.single.$1.items.single.staffId, 'elif');
    },
  );

  testWidgets('randevu detayından iptal', (tester) async {
    final appointments = FakeAppointmentsRepository(
      home: doluHome,
      detail: detay(),
    );
    await _start(
      tester,
      TestDeps(store: signedInStore(), appointments: appointments),
    );
    await _tap(tester, '14:30');
    expect(find.text('Moda Cad. 42, Kadıköy'), findsOneWidget);
    await _tap(tester, 'Randevuyu iptal et');
    await _tap(tester, 'Planım değişti');
    await _tap(tester, 'İptal et');
    expect(appointments.cancelled, [
      CustomerCancelRequestReasonEnum.planChanged,
    ]);
    expect(find.text('Randevunuz iptal edildi.'), findsOneWidget);
    // Detay ve Randevularım yeniden okunur.
    expect(appointments.loads, greaterThanOrEqualTo(1));
    final back = find.byTooltip('Geri');
    await tester.tap(back);
    await tester.pumpAndSettle();
    expect(find.text('Merhaba Merve'), findsOneWidget);
  });
}
