import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:go_router/go_router.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/app/router.dart';
import 'package:omvila_musteri/features/notifications/presentation/bildirimler_ekrani.dart';
import 'package:omvila_musteri/features/waitlist/presentation/bekleme_katil_sayfasi.dart';
import 'package:omvila_musteri/features/waitlist/presentation/bekleme_teklif_ekrani.dart';
import 'package:omvila_musteri/shared/widgets/durum_desenleri.dart';
import 'package:omvila_ui/omvila_ui.dart';

import '../helpers/booking_fixtures.dart';
import '../helpers/fakes.dart';
import '../helpers/fixtures.dart';
import '../helpers/harness.dart';
import '../helpers/hesap_fixtures.dart';

// Bildirimler, bekleme listesine katılma (BeklemeKatil) ve bekleme teklifi
// (BeklemeTeklif, BeklemeDoldu).

const _offline = ApiProblem(status: 0, code: 'network_error');

final List<GoRoute> _routes = [
  for (final r in [
    '/randevu/:id',
    '/randevu/:id/degerlendir',
    '/randevu/:id/havale',
    '/randevu/:id/tamam',
    '/b/:code',
    '/r/:code',
    Routes.yrHizmet,
  ])
    markerRoute(r),
];

Future<TestDeps> _pump(
  WidgetTester tester,
  Widget screen,
  TestDeps deps, {
  bool settle = true,
}) async {
  usePhone(tester);
  await tester.pumpWidget(screenApp(screen, deps, routes: _routes));
  if (settle) {
    await tester.pumpAndSettle();
  } else {
    await tester.pump();
  }

  return deps;
}

void main() {
  group('Bildirimler', () {
    testWidgets('dolu: satırlar, okunmamış noktası, hepsi okundu sayılır', (
      tester,
    ) async {
      final repo = FakeNotificationsRepository(page: bildirimler());
      await _pump(
        tester,
        const BildirimlerEkrani(),
        TestDeps(notifications: repo),
      );
      expect(find.text('Randevunuz onaylandı'), findsOneWidget);
      expect(find.text('Şimdi'), findsOneWidget);
      expect(find.text('Bugün'), findsOneWidget);
      expect(find.text('Dün'), findsOneWidget);
      expect(find.text('12 Kas'), findsOneWidget);
      expect(
        find.byWidgetPredicate(
          (w) => w is Semantics && w.properties.label == 'Okunmadı',
        ),
        findsNWidgets(2),
      );
      expect(repo.markAll, 1);
    });

    testWidgets('satıra dokununca hedef ekran', (tester) async {
      await _pump(
        tester,
        const BildirimlerEkrani(),
        TestDeps(
          notifications: FakeNotificationsRepository(page: bildirimler()),
        ),
      );
      await tester.tap(find.text('Beklediğiniz saat boşaldı'));
      await tester.pumpAndSettle();
      expect(find.text('adres:/b/TEKLIF1'), findsOneWidget);
    });

    test('hedef adresleri', () {
      expect(
        bildirimHedefi(
          MeNotificationTarget(
            type: MeNotificationTargetTypeEnum.review,
            appointmentId: 'a0',
          ),
        ),
        '/randevu/a0/degerlendir',
      );
      expect(
        bildirimHedefi(
          MeNotificationTarget(
            type: MeNotificationTargetTypeEnum.booking,
            brandSlug: 'spa-moda',
          ),
        ),
        '/spa-moda',
      );
      expect(
        bildirimHedefi(
          MeNotificationTarget(type: MeNotificationTargetTypeEnum.appointment),
        ),
        isNull,
      );
    });

    testWidgets('boş: "Bildirim ayarları"', (tester) async {
      final d = await _pump(tester, const BildirimlerEkrani(), TestDeps());
      expect(find.text('Bildiriminiz yok'), findsOneWidget);
      await tester.tap(find.text('Bildirim ayarları'));
      await tester.pumpAndSettle();
      expect(d.push.requests, 1);
      expect(d.notifications.markAll, 0);
    });

    testWidgets('yükleniyor ve hata', (tester) async {
      await _pump(
        tester,
        const BildirimlerEkrani(),
        TestDeps(notifications: FakeNotificationsRepository()..pending = true),
        settle: false,
      );
      expect(find.byType(YukleniyorDurumu), findsOneWidget);
      await _pump(
        tester,
        const BildirimlerEkrani(),
        TestDeps(
          notifications: FakeNotificationsRepository()..error = _offline,
        ),
      );
      expect(find.text('Bağlantı yok'), findsOneWidget);
    });

    testWidgets('zil okunmamışta nokta gösterir', (tester) async {
      await _pump(
        tester,
        const Scaffold(body: BildirimZili()),
        TestDeps(
          notifications: FakeNotificationsRepository(page: bildirimler()),
        ),
      );
      expect(
        find.byWidgetPredicate(
          (w) =>
              w is Semantics &&
              w.properties.label == 'Bildirimler, 2 okunmamış',
        ),
        findsOneWidget,
      );
    });
  });

  group('Bekleme listesine katıl (BeklemeKatil)', () {
    Widget opener() => Builder(
      builder: (context) => Scaffold(
        body: TextButton(
          onPressed: () => beklemeKatilGoster(context),
          child: const Text('aç'),
        ),
      ),
    );

    Future<TestDeps> open(WidgetTester tester, TestDeps deps) async {
      usePhone(tester);
      await tester.pumpWidget(
        screenApp(
          opener(),
          deps,
          extraOverrides: [
            seedBooking(
              taslak(
                services: ['kesim'],
                staff: {'kesim': 'elif'},
                date: '2026-11-26',
              ),
            ),
          ],
        ),
      );
      await tester.tap(find.text('aç'));
      await tester.pumpAndSettle();
      return deps;
    }

    TestDeps deps({Object? joinError, String? name = 'Merve Yıldız'}) =>
        TestDeps(
          booking: FakeBookingRepository(
            catalogValue: kuaforElifKatalog(),
            name: name,
          ),
          waitlist: FakeWaitlistRepository(entry: beklemeKaydi())
            ..joinError = joinError,
        );

    testWidgets('gün ve saat dilimi, "Başka personel de olur", katılır', (
      tester,
    ) async {
      final d = await open(tester, deps());
      expect(find.text('Bekleme listesine katılın'), findsOneWidget);
      expect(find.text('Saç kesimi · Elif Kaya · Kuaför Elif'), findsOneWidget);
      // Tutma süresi sözleşmede yok: sayı yazılmaz.
      expect(find.textContaining('dakika'), findsNothing);
      await tester.tap(find.text('Cum 27'));
      await tester.tap(find.text('Öğleden sonra'));
      await tester.tap(find.text('Başka personel de olur'));
      await tester.pump();
      await tester.ensureVisible(find.text('Listeye katıl'));
      await tester.tap(find.text('Listeye katıl'));
      await tester.pumpAndSettle();
      final (branch, body, _) = d.waitlist.joins.single;
      expect(branch, 'br-kuafor-elif');
      expect(body.fullName, 'Merve Yıldız');
      expect(body.preferences.serviceIds, ['kesim']);
      expect(body.preferences.preferredStaffId, 'elif');
      expect(body.preferences.acceptOtherStaff, isTrue);
      expect(
        body.preferences.dates.map(localDateOf),
        ['2026-11-26', '2026-11-27'],
      );
      expect(body.preferences.timeWindows, [WaitlistTimeWindow.afternoon]);
      expect(find.text('Bekleme listesine katılın'), findsNothing);
    });

    testWidgets('profilde ad yoksa sorulur', (tester) async {
      await open(tester, deps(name: null));
      expect(find.text('Adınız soyadınız'), findsOneWidget);
      await tester.ensureVisible(find.text('Listeye katıl'));
      await tester.tap(find.text('Listeye katıl'));
      await tester.pump();
      expect(find.text('Bekleme listesine katılın'), findsOneWidget);
    });

    testWidgets('online randevu kapalıysa "Salonu ara" kutusu', (
      tester,
    ) async {
      await open(
        tester,
        deps(
          joinError: const ApiProblem(status: 403, code: 'booking_disabled'),
        ),
      );
      await tester.ensureVisible(find.text('Listeye katıl'));
      await tester.tap(find.text('Listeye katıl'));
      await tester.pumpAndSettle();
      expect(
        find.text('Kuaför Elif şu an online randevu almıyor.'),
        findsOneWidget,
      );
      expect(find.byType(OnlineRandevuKapaliKutusu), findsOneWidget);
    });
  });

  group('Bekleme teklifi (BeklemeTeklif, BeklemeDoldu)', () {
    Future<TestDeps> pump(
      WidgetTester tester,
      FakeWaitlistRepository repo, {
      bool settle = true,
    }) => _pump(
      tester,
      const BeklemeTeklifEkrani(code: 'TEKLIF1'),
      TestDeps(waitlist: repo),
      settle: settle,
    );

    testWidgets('geri sayım sunucu farkından, ayrıntılar', (tester) async {
      await pump(tester, FakeWaitlistRepository(offerValue: teklif()));
      expect(find.text('12:48'), findsOneWidget);
      expect(find.text('Sizin için tutuluyor'), findsOneWidget);
      expect(find.text('Beklediğiniz saat boşaldı'), findsOneWidget);
      expect(find.text('Cuma 27 Kasım · 16:00'), findsOneWidget);
      expect(find.text('Masaj · 1 sa'), findsOneWidget);
      expect(find.text('Seda Kurt'), findsOneWidget);
      expect(find.text('900\u00a0₺'), findsOneWidget);
    });

    testWidgets('süre dolunca BeklemeDoldu, "Listeden çık"', (tester) async {
      final clock = MutableClock(testClock.now());
      final repo = FakeWaitlistRepository(offerValue: teklif());
      usePhone(tester);
      await tester.pumpWidget(
        screenApp(
          const BeklemeTeklifEkrani(code: 'TEKLIF1'),
          TestDeps(waitlist: repo, clock: clock),
          routes: _routes,
        ),
      );
      await tester.pumpAndSettle();
      clock.advance(const Duration(minutes: 13));
      await tester.pump(const Duration(seconds: 1));
      expect(find.text('Bu saatin süresi doldu'), findsOneWidget);
      expect(find.text('Listede kalmaya devam ediyorsunuz'), findsOneWidget);
      await tester.tap(find.text('Listeden çık'));
      await tester.pumpAndSettle();
      expect(repo.leaves, ['W3nB8xQ2']);
      expect(find.text('Bekleme listesinden çıktınız'), findsOneWidget);
    });

    testWidgets('"Randevuyu al" (If-Match): kaporasız başarı ekranı', (
      tester,
    ) async {
      final repo = FakeWaitlistRepository(offerValue: teklif())
        ..accepted = olusan();
      await pump(tester, repo);
      await tester.tap(find.text('Randevuyu al'));
      await tester.pumpAndSettle();
      expect(repo.acceptVersions, ['o1']);
      expect(find.text('adres:/randevu/yeni-1/tamam'), findsOneWidget);
    });

    testWidgets('"Randevuyu al": kapora isteniyorsa havale', (tester) async {
      final repo = FakeWaitlistRepository(offerValue: teklif(depositLira: 200))
        ..accepted = olusan(status: DisplayStatus.depositPending);
      await pump(tester, repo);
      expect(find.text('Kapora (havale ile)'), findsOneWidget);
      expect(
        find.textContaining('kapora havalesinden sonra kesinleşir'),
        findsOneWidget,
      );
      await tester.tap(find.text('Randevuyu al'));
      await tester.pumpAndSettle();
      expect(find.text('adres:/randevu/yeni-1/havale?yeni=1'), findsOneWidget);
    });

    testWidgets('sunucu süresi doldu derse BeklemeDoldu', (tester) async {
      final repo = FakeWaitlistRepository(offerValue: teklif())
        ..actionError = const ApiProblem(status: 409, code: 'hold_expired');
      await pump(tester, repo);
      await tester.tap(find.text('Randevuyu al'));
      await tester.pumpAndSettle();
      expect(find.text('Bu saatin süresi doldu'), findsOneWidget);
    });

    testWidgets('"Bu saat olmaz" → geri çevrildi, "Başka saat seç"', (
      tester,
    ) async {
      final repo = FakeWaitlistRepository(offerValue: teklif());
      await pump(tester, repo);
      await tester.tap(find.text('Bu saat olmaz'));
      await tester.pumpAndSettle();
      expect(repo.declines, ['o1']);
      expect(find.text('Bu saati geri çevirdiniz'), findsOneWidget);
      await tester.tap(find.text('Başka saat seç'));
      await tester.pumpAndSettle();
      expect(find.text('adres:${Routes.yrHizmet}'), findsOneWidget);
    });

    testWidgets('kabul edilmiş teklif "Bu saati aldınız"', (tester) async {
      await pump(
        tester,
        FakeWaitlistRepository(
          offerValue: teklif(
            status: WaitlistOfferStatus.accepted,
            appointmentCode: 'R1',
          ),
        ),
      );
      expect(find.text('Bu saati aldınız'), findsOneWidget);
      await tester.tap(find.text('Randevunuzu görün'));
      await tester.pumpAndSettle();
      expect(find.text('adres:/r/R1'), findsOneWidget);
    });

    testWidgets('yükleniyor, bulunamadı, hata', (tester) async {
      await pump(
        tester,
        FakeWaitlistRepository()..offerPending = true,
        settle: false,
      );
      expect(find.byType(YukleniyorDurumu), findsOneWidget);

      await pump(
        tester,
        FakeWaitlistRepository()
          ..offerError = const ApiProblem(status: 404, code: 'not_found'),
      );
      expect(find.text('Bu teklif bulunamadı'), findsOneWidget);

      await pump(tester, FakeWaitlistRepository()..offerError = _offline);
      expect(find.text('Bağlantı yok'), findsOneWidget);
    });
  });

  testWidgets('OTP sınırı süresiz', (tester) async {
    await _pump(
      tester,
      const Scaffold(
        body: OtpSiniriKutusu(
          problem: ApiProblem(status: 429, code: 'otp_attempts_exceeded'),
        ),
      ),
      TestDeps(),
    );
    expect(
      find.text(
        'Güvenliğiniz için biraz bekledikten sonra yeni kod '
        'isteyebilirsiniz.',
      ),
      findsOneWidget,
    );
  });
}
