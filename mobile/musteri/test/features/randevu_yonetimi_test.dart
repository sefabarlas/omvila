import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:go_router/go_router.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/app/router.dart';
import 'package:omvila_musteri/features/appointments/presentation/basari_ekrani.dart';
import 'package:omvila_musteri/features/appointments/presentation/degerlendir_ekrani.dart';
import 'package:omvila_musteri/features/appointments/presentation/ertele_ekrani.dart';
import 'package:omvila_musteri/features/appointments/presentation/gecmis_ekrani.dart';
import 'package:omvila_musteri/features/appointments/presentation/havale_ekrani.dart';
import 'package:omvila_musteri/features/appointments/presentation/randevu_detay_ekrani.dart';
import 'package:omvila_ui/omvila_ui.dart';

import '../helpers/booking_fixtures.dart';
import '../helpers/fakes.dart';
import '../helpers/harness.dart';

// Randevu yönetimi: RandevuDetay, Iptal, ErteleKapali, RandevuErtele,
// YRHavale, YRBasari, Gecmis, Degerlendir.

final List<GoRoute> _routes = [
  for (final r in [
    '/randevu/:id/ertele',
    '/randevu/:id/havale',
    '/randevu/:id/tamam',
    '/randevu/:id/degerlendir',
    '/randevu/:id',
    Routes.yrHizmet,
    Routes.yrSaat,
  ])
    markerRoute(r),
];

Future<TestDeps> _pump(
  WidgetTester tester,
  Widget screen,
  FakeAppointmentsRepository appointments,
) async {
  usePhone(tester);
  final deps = TestDeps(appointments: appointments);
  await tester.pumpWidget(screenApp(screen, deps, routes: _routes));
  for (var i = 0; i < 4; i++) {
    await tester.pump();
  }

  return deps;
}

List<String> _clipboard(WidgetTester tester) {
  final copied = <String>[];
  tester.binding.defaultBinaryMessenger.setMockMethodCallHandler(
    SystemChannels.platform,
    (call) async {
      if (call.method == 'Clipboard.setData') {
        copied.add(
          (call.arguments as Map<Object?, Object?>)['text']! as String,
        );
      }

      return null;
    },
  );
  addTearDown(
    () => tester.binding.defaultBinaryMessenger.setMockMethodCallHandler(
      SystemChannels.platform,
      null,
    ),
  );
  return copied;
}

void main() {
  group('Randevu detayı (RandevuDetay)', () {
    testWidgets('yükleniyor', (tester) async {
      await _pump(
        tester,
        const RandevuDetayEkrani(id: 'a1'),
        FakeAppointmentsRepository()..detailPending = true,
      );
      expect(find.byType(CircularProgressIndicator), findsOneWidget);
    });

    testWidgets('hata ve "Tekrar dene"', (tester) async {
      final repo = FakeAppointmentsRepository()
        ..detailError = const ApiProblem(status: 0, code: 'network_error');
      await _pump(tester, const RandevuDetayEkrani(id: 'a1'), repo);
      expect(find.text('Bağlantı yok'), findsOneWidget);
      repo
        ..detailError = null
        ..detail = detay();
      await tester.tap(find.text('Tekrar dene'));
      await settle(tester);
      expect(find.text('Kuaför Elif'), findsOneWidget);
    });

    testWidgets('kalemler, kapora, süre notu, Ara ve Yol tarifi', (
      tester,
    ) async {
      final deps = await _pump(
        tester,
        const RandevuDetayEkrani(id: 'a1'),
        FakeAppointmentsRepository(
          detail: detay(
            deposit: kapora(DepositStatus.received, instructions: false),
          ),
        ),
      );
      expect(find.text('YARIN · 25 KASIM ÇARŞAMBA'), findsOneWidget);
      expect(find.text('Onaylandı'), findsOneWidget);
      expect(find.text("1 sa 15 dk · 15:45'te biter"), findsOneWidget);
      expect(find.text('Moda Cad. 42, Kadıköy'), findsOneWidget);
      expect(find.text('Saç kesimi · Elif'), findsOneWidget);
      expect(find.text('150\u00a0₺ alındı · havale'), findsOneWidget);
      expect(find.text('500\u00a0₺'), findsOneWidget);
      expect(
        find.text('İptal 24 saat, saat değişikliği 12 saat öncesine kadar.'),
        findsOneWidget,
      );
      await tester.tap(find.text('Ara'));
      await tester.tap(find.text('Yol tarifi'));
      await tester.pump();
      expect(deps.external.urls.first, Uri.parse('tel:+902163450012'));
      expect(deps.external.urls.last.host, 'www.google.com');
    });

    testWidgets('iptal: neden zorunlu, "Rahatsızlandım", kapora uyarısı', (
      tester,
    ) async {
      final repo = FakeAppointmentsRepository(
        detail: detay(
          deposit: kapora(DepositStatus.received, instructions: false),
        ),
      );
      await _pump(tester, const RandevuDetayEkrani(id: 'a1'), repo);
      await tester.ensureVisible(find.text('Randevuyu iptal et'));
      await tester.tap(find.text('Randevuyu iptal et'));
      await tester.pumpAndSettle();
      expect(find.text('Randevu iptal edilsin mi?'), findsOneWidget);
      expect(
        find.text(
          'Kuaför Elif · Yarın 14:30. Saatiniz başka bir müşteriye açılır.',
        ),
        findsOneWidget,
      );
      expect(
        find.textContaining('150\u00a0₺ kaporanın iadesi'),
        findsOneWidget,
      );
      expect(find.text('Hastayım'), findsNothing);
      final cancel = find.widgetWithText(FilledButton, 'İptal et');
      expect(tester.widget<FilledButton>(cancel).onPressed, isNull);
      await tester.tap(find.text('Rahatsızlandım'));
      await tester.pump();
      await tester.tap(cancel);
      await tester.pumpAndSettle();
      expect(repo.cancelled, [CustomerCancelRequestReasonEnum.sick]);
      expect(find.text('Randevunuz iptal edildi.'), findsOneWidget);
    });

    testWidgets('iptal son süresi geçtiyse "Salonu ara"', (tester) async {
      final deps = await _pump(
        tester,
        const RandevuDetayEkrani(id: 'a1'),
        FakeAppointmentsRepository(detail: detay(canCancel: false)),
      );
      await tester.ensureVisible(find.text('Randevuyu iptal et'));
      await tester.tap(find.text('Randevuyu iptal et'));
      await tester.pumpAndSettle();
      expect(find.text('İptal için geç kaldınız'), findsOneWidget);
      expect(find.text('24 saat önceye kadar'), findsOneWidget);
      await tester.tap(find.text('Salonu ara'));
      await tester.pump();
      expect(deps.external.urls.single.scheme, 'tel');
    });

    testWidgets('saat değişikliği kapalıysa ErteleKapali', (tester) async {
      await _pump(
        tester,
        const RandevuDetayEkrani(id: 'a1'),
        FakeAppointmentsRepository(detail: detay(canReschedule: false)),
      );
      await tester.tap(find.text('Saati değiştir'));
      await tester.pumpAndSettle();
      expect(find.text('Saat değişikliği için geç kaldınız'), findsOneWidget);
      expect(
        find.text(
          'Kuaför Elif, randevudan 12 saat öncesine kadar değişiklik kabul '
          'ediyor. Randevunuza 18 saat kaldı.',
        ),
        findsOneWidget,
      );
      await tester.tap(find.text('Kapat'));
      await tester.pumpAndSettle();
      expect(find.text('Saat değişikliği için geç kaldınız'), findsNothing);
    });

    testWidgets('saat değişikliği açıksa Ertele ekranı', (tester) async {
      await _pump(
        tester,
        const RandevuDetayEkrani(id: 'a1'),
        FakeAppointmentsRepository(detail: detay()),
      );
      await tester.tap(find.text('Saati değiştir'));
      await tester.pumpAndSettle();
      expect(find.text('adres:/randevu/a1/ertele'), findsOneWidget);
    });

    testWidgets('tamamlanan: "Değerlendir" ve "Aynısını tekrar al"', (
      tester,
    ) async {
      await _pump(
        tester,
        const RandevuDetayEkrani(id: 'a1'),
        FakeAppointmentsRepository(
          detail: detay(
            status: DisplayStatus.completed,
            canReview: true,
            canCancel: false,
            canReschedule: false,
          ),
        ),
      );
      expect(find.text('Randevuyu iptal et'), findsNothing);
      await tester.tap(find.text('Aynısını tekrar al'));
      await tester.pumpAndSettle();
      expect(find.text('adres:${Routes.yrSaat}'), findsOneWidget);
    });

    testWidgets('kapora bekleniyorsa havale ekranına gider', (tester) async {
      await _pump(
        tester,
        const RandevuDetayEkrani(id: 'a1'),
        FakeAppointmentsRepository(
          detail: detay(
            status: DisplayStatus.depositPending,
            deposit: kapora(DepositStatus.awaiting),
          ),
        ),
      );
      expect(find.text('Kapora bekleniyor'), findsOneWidget);
      expect(find.text('150\u00a0₺ · bekleniyor'), findsOneWidget);
      await tester.tap(find.text('Kaporayı gönder'));
      await tester.pumpAndSettle();
      expect(find.text('adres:/randevu/a1/havale'), findsOneWidget);
    });
  });

  group('Saati değiştir (RandevuErtele)', () {
    testWidgets('yeni saat; kapora aktarılır notu', (tester) async {
      final repo =
          FakeAppointmentsRepository(
              detail: detay(
                deposit: kapora(DepositStatus.received, instructions: false),
              ),
            )
            ..days = gunler()
            ..slots = saatler();
      await _pump(tester, const ErteleEkrani(id: 'a1'), repo);
      expect(find.text('Şu anki randevunuz'), findsOneWidget);
      expect(find.text('Çar 25 Kas · 14:30'), findsOneWidget);
      expect(
        find.text('Aynı hizmetler ve aynı kişi · 1 sa 15 dk'),
        findsOneWidget,
      );
      expect(
        find.text(
          '150\u00a0₺ kaporanız yeni saate aktarılır, tekrar ödeme '
          'yapmazsınız.',
        ),
        findsOneWidget,
      );
      expect(
        find.text(
          'Saat değişikliği randevudan 12 saat öncesine kadar yapılabilir. '
          'Salon onaylı çalışıyorsa yeni saat onaya düşer.',
        ),
        findsOneWidget,
      );
      await tester.tap(find.text('15:15'));
      await tester.pump();
      expect(find.text("16:30'da biter"), findsOneWidget);
      await tester.tap(find.widgetWithText(FilledButton, 'Saati değiştir'));
      await tester.pumpAndSettle();
      expect(repo.rescheduled, [DateTime.utc(2026, 11, 25, 12, 15)]);
    });

    testWidgets('son süre geçtiyse ErteleKapali', (tester) async {
      final repo = FakeAppointmentsRepository(detail: detay())
        ..days = gunler()
        ..slots = saatler()
        ..actionError = const ApiProblem(status: 409, code: 'cutoff_passed');
      await _pump(tester, const ErteleEkrani(id: 'a1'), repo);
      await tester.tap(find.text('15:15'));
      await tester.pump();
      await tester.tap(find.widgetWithText(FilledButton, 'Saati değiştir'));
      await tester.pumpAndSettle();
      expect(find.text('Saat değişikliği için geç kaldınız'), findsOneWidget);
    });

    testWidgets('saatler okunamazsa mesaj', (tester) async {
      final repo = FakeAppointmentsRepository(detail: detay())
        ..days = gunler()
        ..slotsError = const ApiProblem(status: 409, code: 'cutoff_passed');
      await _pump(tester, const ErteleEkrani(id: 'a1'), repo);
      expect(
        find.text('Değişiklik süresi geçti. Salonu arayın.'),
        findsOneWidget,
      );
    });
  });

  group('Kapora havalesi (YRHavale)', () {
    MeAppointment awaiting() => detay(
      status: DisplayStatus.depositPending,
      deposit: kapora(DepositStatus.awaiting),
    );

    testWidgets('geri sayım, alanlar, IBAN boşluksuz kopyalanır', (
      tester,
    ) async {
      final copied = _clipboard(tester);
      await _pump(
        tester,
        const HavaleEkrani(id: 'a1', yeni: true),
        FakeAppointmentsRepository(detail: awaiting()),
      );
      expect(find.text('KAPORA BEKLENİYOR'), findsOneWidget);
      expect(find.text('1 sa 58 dk kaldı'), findsOneWidget);
      expect(find.text('Elif Kaya'), findsOneWidget);
      expect(find.text('TR12 0006 4000 0011 2345 6789 01'), findsOneWidget);
      expect(find.text('OMV-7K2X'), findsOneWidget);
      await tester.tap(find.bySemanticsLabel('IBAN kopyala'));
      await tester.pump();
      await tester.tap(find.bySemanticsLabel('Tutar kopyala'));
      await tester.pump();
      expect(copied, ['TR120006400000112345678901', '150']);
      expect(find.text('IBAN kopyalandı.'), findsOneWidget);
    });

    testWidgets('"Havaleyi yaptım" salona bildirir, başarı ekranı açılır', (
      tester,
    ) async {
      final repo = FakeAppointmentsRepository(detail: awaiting());
      await _pump(tester, const HavaleEkrani(id: 'a1', yeni: true), repo);
      await tester.tap(find.text('Havaleyi yaptım, salona bildir'));
      await tester.pumpAndSettle();
      expect(repo.reported, ['a1']);
      expect(find.text('adres:/randevu/a1/tamam'), findsOneWidget);
    });

    testWidgets('"Daha sonra" yeni randevuda başarı ekranı', (tester) async {
      final repo = FakeAppointmentsRepository(detail: awaiting());
      await _pump(tester, const HavaleEkrani(id: 'a1', yeni: true), repo);
      await tester.tap(find.text('Daha sonra'));
      await tester.pumpAndSettle();
      expect(repo.reported, isEmpty);
      expect(find.text('adres:/randevu/a1/tamam'), findsOneWidget);
    });

    testWidgets('bildirilmişse "Salon onayı bekleniyor"', (tester) async {
      await _pump(
        tester,
        const HavaleEkrani(id: 'a1'),
        FakeAppointmentsRepository(
          detail: detay(
            status: DisplayStatus.depositPending,
            deposit: kapora(DepositStatus.customerReported),
          ),
        ),
      );
      expect(find.text('SALON ONAYI BEKLENİYOR'), findsOneWidget);
      expect(find.text('Havaleyi yaptım, salona bildir'), findsNothing);
    });

    testWidgets('süre dolduysa mesaj', (tester) async {
      final repo = FakeAppointmentsRepository(detail: awaiting())
        ..actionError = const ApiProblem(status: 409, code: 'hold_expired');
      await _pump(tester, const HavaleEkrani(id: 'a1'), repo);
      await tester.tap(find.text('Havaleyi yaptım, salona bildir'));
      await tester.pump();
      expect(find.text('Bu saatin süresi doldu.'), findsOneWidget);
    });

    testWidgets('kapora beklenmiyorsa boş durum', (tester) async {
      await _pump(
        tester,
        const HavaleEkrani(id: 'a1'),
        FakeAppointmentsRepository(detail: detay()),
      );
      expect(find.text('Kapora beklenmiyor'), findsOneWidget);
    });
  });

  group('Randevu oluşturuldu (YRBasari)', () {
    for (final (name, appointment, title, badge) in [
      ('onaylı', detay(), 'Randevunuz onaylandı', 'Onaylandı'),
      (
        'onay bekleyen',
        detay(status: DisplayStatus.pendingApproval),
        'Talebiniz alındı',
        'Onay bekliyor',
      ),
      (
        'kapora bekleyen',
        detay(
          status: DisplayStatus.depositPending,
          deposit: kapora(DepositStatus.awaiting),
        ),
        'Kaporanız bekleniyor',
        'Kapora bekleniyor',
      ),
      (
        'havalesi bildirilmiş',
        detay(
          status: DisplayStatus.depositPending,
          deposit: kapora(DepositStatus.customerReported),
        ),
        'Havaleniz salona bildirildi',
        'Kapora bekleniyor',
      ),
    ]) {
      testWidgets('$name randevu: durum doğru', (tester) async {
        await _pump(
          tester,
          const BasariEkrani(id: 'a1'),
          FakeAppointmentsRepository(detail: appointment),
        );
        expect(find.text(title), findsOneWidget);
        expect(find.text(badge), findsOneWidget);
        if (title != 'Randevunuz onaylandı') {
          expect(find.text('Randevunuz onaylandı'), findsNothing);
        }
      });
    }

    testWidgets('takvim, yol tarifi, paylaş ve dönüş', (tester) async {
      final copied = _clipboard(tester);
      final deps = await _pump(
        tester,
        const BasariEkrani(id: 'a1'),
        FakeAppointmentsRepository(detail: detay()),
      );
      expect(find.text('500\u00a0₺ salonda'), findsOneWidget);
      await tester.tap(find.text('Takvime ekle'));
      await tester.tap(find.text('Yol tarifi'));
      await tester.tap(find.text('Paylaş'));
      await tester.pump();
      expect(deps.external.calendar.single.title, 'Kuaför Elif randevusu');
      expect(deps.external.urls, hasLength(1));
      expect(copied.single, contains('Kuaför Elif randevusu'));
    });

    testWidgets('hata', (tester) async {
      await _pump(
        tester,
        const BasariEkrani(id: 'a1'),
        FakeAppointmentsRepository()
          ..detailError = const ApiProblem(status: 500, code: 'try_again'),
      );
      expect(find.text('Sayfa yüklenemedi'), findsOneWidget);
    });
  });

  group('Geçmiş (Gecmis)', () {
    testWidgets('yükleniyor', (tester) async {
      await _pump(
        tester,
        const GecmisEkrani(),
        FakeAppointmentsRepository()..pastPending = true,
      );
      expect(find.byType(CircularProgressIndicator), findsOneWidget);
    });

    testWidgets('boş (GecmisBos)', (tester) async {
      await _pump(tester, const GecmisEkrani(), FakeAppointmentsRepository());
      expect(find.text('Henüz geçmiş randevunuz yok'), findsOneWidget);
      // GecmisBos'ta süzgeç (segment) yok.
      expect(find.text('Değerlendirilmedi'), findsNothing);
    });

    testWidgets('hata', (tester) async {
      await _pump(
        tester,
        const GecmisEkrani(),
        FakeAppointmentsRepository()
          ..pastError = const ApiProblem(status: 0, code: 'network_error'),
      );
      expect(find.text('Bağlantı yok'), findsOneWidget);
    });

    testWidgets('ay başlıkları, Değerlendir, yıldızlar, İptal', (
      tester,
    ) async {
      final repo = FakeAppointmentsRepository()
        ..pastPages = {
          (false, null): MeAppointmentPage(
            items: gecmisListesi(),
            nextCursor: null,
          ),
          (true, null): MeAppointmentPage(
            items: [gecmisListesi().first],
            nextCursor: null,
          ),
        };
      await _pump(tester, const GecmisEkrani(), repo);
      expect(find.text('Kasım'), findsOneWidget);
      expect(find.text('Ekim'), findsOneWidget);
      expect(find.text('Boya ve fön · 12 Kas · 1.440\u00a0₺'), findsOneWidget);
      expect(
        tester.widgetList<Yildizlar>(find.byType(Yildizlar)).map((y) => y.puan),
        [5, 4],
      );
      expect(find.text('İptal'), findsOneWidget);

      await tester.tap(find.text('Değerlendirilmedi'));
      await settle(tester);
      expect(repo.pastRequests.last, (true, null));
      expect(find.text('Nail Studio Moda'), findsNothing);

      await tester.tap(find.widgetWithText(FilledButton, 'Değerlendir'));
      await tester.pumpAndSettle();
      expect(find.text('adres:/randevu/g1/degerlendir'), findsOneWidget);
    });

    testWidgets('sonraki sayfa liste sonunda okunur', (tester) async {
      final repo = FakeAppointmentsRepository()
        ..pastPages = {
          (false, null): MeAppointmentPage(
            items: [gecmisListesi().first],
            nextCursor: 'c2',
          ),
          (false, 'c2'): MeAppointmentPage(
            items: [gecmisListesi()[1]],
            nextCursor: null,
          ),
        };
      await _pump(tester, const GecmisEkrani(), repo);
      await settle(tester);
      expect(repo.pastRequests, [(false, null), (false, 'c2')]);
      expect(find.text('Nail Studio Moda'), findsOneWidget);
    });
  });

  group('Değerlendir (Degerlendir)', () {
    testWidgets('puan, personel, yorum ve gönderim', (tester) async {
      final repo = FakeAppointmentsRepository()..review = degerlendirme();
      await _pump(tester, const DegerlendirEkrani(appointmentId: 'g1'), repo);
      expect(find.text("Kuaför Elif'te nasıldı?"), findsOneWidget);
      expect(find.text('Boya ve fön · Elif Kaya · 12 Kasım'), findsOneWidget);
      expect(find.text('Sizi bu randevuda ağırlayan kişi'), findsOneWidget);
      expect(
        find.text(
          'Yorumunuz salonla paylaşılır. Adınız "Merve Y." olarak görünür.',
        ),
        findsOneWidget,
      );
      final send = find.widgetWithText(FilledButton, 'Gönder');
      expect(tester.widget<FilledButton>(send).onPressed, isNull);
      await tester.tap(find.bySemanticsLabel('4 yıldız, Çok iyi'));
      await tester.pump();
      expect(find.text('Çok iyi'), findsOneWidget);
      await tester.enterText(
        find.byType(TextField),
        'Boya tam istediğim gibi.',
      );
      await tester.tap(send);
      await tester.pump();
      await tester.pump();
      expect(find.text('Değerlendirmeniz gönderildi.'), findsOneWidget);
      final (target, rating, comment) = repo.reviews.single;
      expect(target.appointmentId, 'g1');
      expect(rating, 4);
      expect(comment, 'Boya tam istediğim gibi.');
    });

    testWidgets('değerlendirilmişse gönderilen puan (already_reviewed)', (
      tester,
    ) async {
      final repo = FakeAppointmentsRepository()
        ..review = degerlendirme()
        ..actionError = const ApiProblem(status: 409, code: 'already_reviewed');
      await _pump(tester, const DegerlendirEkrani(appointmentId: 'g1'), repo);
      await tester.tap(find.bySemanticsLabel('5 yıldız, Mükemmel'));
      await tester.pump();
      repo.review = degerlendirme(existing: 5);
      await tester.tap(find.widgetWithText(FilledButton, 'Gönder'));
      await settle(tester);
      expect(find.text('Bu randevuyu değerlendirdiniz'), findsOneWidget);
      expect(find.text('Gönder'), findsNothing);
    });

    testWidgets('bağlantı kodu geçersizse "Bu bağlantı geçersiz"', (
      tester,
    ) async {
      await _pump(
        tester,
        const DegerlendirEkrani(accessCode: 'YOK'),
        FakeAppointmentsRepository()
          ..reviewError = const ApiProblem(status: 404, code: 'not_found'),
      );
      expect(find.text('Bu bağlantı geçersiz'), findsOneWidget);
    });

    testWidgets('yükleniyor ve hata', (tester) async {
      await _pump(
        tester,
        const DegerlendirEkrani(appointmentId: 'g1'),
        FakeAppointmentsRepository()
          ..reviewError = const ApiProblem(status: 500, code: 'try_again'),
      );
      expect(find.text('Sayfa yüklenemedi'), findsOneWidget);
    });
  });
}
