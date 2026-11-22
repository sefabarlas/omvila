import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:omvila_isletme/features/push/data/push_service.dart';
import 'package:omvila_ui/omvila_ui.dart';

import '../helpers/fake_api.dart';
import '../helpers/fixtures.dart';
import '../helpers/screen_fixtures.dart';

// Kural: dilim-05 KK10/KK11, mobile/isletme/AGENTS.md § Bugün — bento
// (Sıradaki, Onay bekliyor, Sırada), prim kartı yetkiye bağlı (null ise
// gizli), gün akışı, satır içi "Onayla" yalnız appointment.approve; kalan
// süre sunucunun generatedUtc'sinden (cihaz saati yok); boş durum MainBos.
void main() {
  const todayPath = '/api/v1/manage/today';

  testWidgets('yükleniyor', (tester) async {
    final h = Harness()..signIn();
    final pending = Completer<FakeResponse>();
    h.api.on('GET', todayPath, (_) => pending.future);
    await h.pumpApp(tester, settle: false);
    expect(find.text('Bugün'), findsWidgets);
    expect(find.byType(CircularProgressIndicator), findsOneWidget);
    pending.complete(FakeResponse(200, todayJson()));
    await tester.pumpAndSettle();
    expect(find.byType(CircularProgressIndicator), findsNothing);
  });

  testWidgets('hata: mesaj ve Tekrar dene', (tester) async {
    final h = Harness()..signIn();
    var fail = true;
    h.api.on(
      'GET',
      todayPath,
      (_) => fail
          ? const FakeResponse(500, {'code': 'try_again'})
          : FakeResponse(200, todayJson()),
    );
    await h.pumpApp(tester);
    expect(find.text('Bugün yüklenemedi'), findsOneWidget);
    expect(find.text(errorMessage('try_again')), findsOneWidget);

    fail = false;
    await tester.tap(find.text('Tekrar dene'));
    await tester.pumpAndSettle();
    expect(find.text('Derya Şahin'), findsOneWidget);
  });

  testWidgets('dolu: bento, prim, gün akışı ve Onayla', (tester) async {
    final h = Harness()..signIn();
    h.api.reply('GET', todayPath, todayJson());
    await h.pumpApp(tester);

    expect(find.text('Çarşamba 25 Kasım · Kadıköy'), findsOneWidget);
    expect(find.text('Randevu'), findsOneWidget);
    expect(find.text('SIRADAKİ'), findsOneWidget);
    expect(find.text('13:00'), findsOneWidget);
    expect(find.text('Saç kesimi · 45 dk'), findsOneWidget);
    // generatedUtc 09:10, başlangıç 10:00.
    expect(find.text('50 dk sonra'), findsOneWidget);
    expect(find.text('ONAY BEKLİYOR'), findsOneWidget);
    expect(find.text('SIRADA'), findsOneWidget);
    expect(find.text('~35 dk bekleme'), findsOneWidget);
    expect(find.text('KASIM PRİMİ'), findsOneWidget);
    expect(find.text(formatMoney(684000)), findsOneWidget);
    expect(find.text('4/12 bitti'), findsOneWidget);
    expect(find.text('Deniz Acar'), findsOneWidget);
    expect(find.text('Saç kesimi · Can'), findsOneWidget);
    expect(find.text('12:00'), findsNWidgets(2));
    expect(find.text('Geldi'), findsOneWidget);
    expect(find.text('Onayla'), findsOneWidget);
    // İzin verilmiş: şerit yok.
    expect(find.text('Bildirimleri aç'), findsNothing);
  });

  testWidgets('Onayla: güncel sürümle If-Match gönderir ve yeniler', (
    tester,
  ) async {
    final h = Harness()..signIn();
    var approved = false;
    h.api
      ..on(
        'GET',
        todayPath,
        (_) => FakeResponse(
          200,
          approved ? todayJson(items: const []) : todayJson(),
        ),
      )
      ..reply(
        'GET',
        '/api/v1/manage/appointments/$pendingId',
        pendingDetailJson,
      )
      ..on('POST', '/api/v1/manage/appointments/$pendingId/approve', (_) {
        approved = true;
        return const FakeResponse(200, pendingDetailJson);
      });
    await h.pumpApp(tester);

    await tapCentered(tester, find.text('Onayla'));
    await tester.pumpAndSettle();

    final post = h.api.to(
      'POST',
      '/api/v1/manage/appointments/$pendingId/approve',
    );
    expect(post, hasLength(1));
    expect(post.single.header('If-Match'), 'v7');
    expect(post.single.header('Authorization'), 'Bearer at-2');
    expect(find.text('Talep onaylandı.'), findsOneWidget);
    expect(find.text('Onayla'), findsNothing);
  });

  testWidgets('Onayla: kapora bekleniyorsa ayrıntıya gider', (tester) async {
    final h = Harness()..signIn();
    h.api
      ..reply('GET', todayPath, todayJson())
      ..reply(
        'GET',
        '/api/v1/manage/appointments/$pendingId',
        pendingDetailJson,
      )
      ..reply(
        'POST',
        '/api/v1/manage/appointments/$pendingId/approve',
        {'code': 'deposit_required'},
        status: 409,
      );
    await h.pumpApp(tester);

    await tapCentered(tester, find.text('Onayla'));
    await tester.pumpAndSettle();
    expect(find.text(errorMessage('deposit_required')), findsOneWidget);
    expect(find.text('Randevu'), findsWidgets);
    expect(find.text('Bugün'), findsNothing);
  });

  testWidgets('personel: onay ve onay kartı yok, sekmeler rol bazlı', (
    tester,
  ) async {
    final h = Harness()..signIn(session: staffSessionJson);
    h.api.reply('GET', todayPath, todayJson(staff: true));
    await h.pumpApp(tester);

    expect(find.text('ONAY BEKLİYOR'), findsNothing);
    expect(find.text('Onayla'), findsNothing);
    expect(find.text('Onay bekliyor'), findsOneWidget);
    expect(find.bySemanticsLabel('Takvimim'), findsOneWidget);
    expect(find.bySemanticsLabel('Kapıda sıra'), findsOneWidget);
    expect(find.bySemanticsLabel('Bildirimler'), findsNothing);
  });

  // MainSaltOkunur: kırmızı şerit; "+ Randevu" görünür ama pasif.
  testWidgets('salt okunur abonelik: şerit ve pasif + Randevu', (
    tester,
  ) async {
    final h = Harness()
      ..signIn(
        session: {
          ...ownerSessionJson,
          'subscription': {'status': 'ReadOnly'},
        },
      );
    h.api.reply('GET', todayPath, todayJson());
    await h.pumpApp(tester);
    expect(
      find.textContaining('Salon salt okunur.', findRichText: true),
      findsOneWidget,
    );
    final button = tester.widget<OmvilaButon>(
      find.widgetWithText(OmvilaButon, 'Randevu'),
    );
    expect(button.onPressed, isNull);
    expect(find.text('Derya Şahin'), findsOneWidget);
  });

  testWidgets('etkin abonelikte şerit yok', (tester) async {
    final h = Harness()..signIn();
    h.api.reply('GET', todayPath, todayJson());
    await h.pumpApp(tester);
    expect(
      find.textContaining('Salon salt okunur.', findRichText: true),
      findsNothing,
    );
    final button = tester.widget<OmvilaButon>(
      find.widgetWithText(OmvilaButon, 'Randevu'),
    );
    expect(button.onPressed, isNotNull);
  });

  testWidgets('prim kartı null ise gizli', (tester) async {
    final h = Harness()..signIn();
    h.api.reply('GET', todayPath, {...todayJson(), 'commission': null});
    await h.pumpApp(tester);
    expect(find.text('KASIM PRİMİ'), findsNothing);
  });

  testWidgets('boş: MainBos, bağlantı paylaşma ve kapıda sıra', (tester) async {
    final h = Harness()..signIn();
    h.api
      ..reply('GET', todayPath, emptyTodayJson())
      ..reply('GET', '/api/v1/manage/brand/booking-page', bookingPageJson)
      ..reply(
        'GET',
        '/api/v1/manage/walk-in-tickets',
        walkInQueueJson(empty: true),
      );
    await h.pumpApp(tester);

    expect(find.text('RANDEVU'), findsOneWidget);
    expect(find.text('Bugün randevunuz yok'), findsOneWidget);
    expect(find.text('Rezervasyon bağlantısını paylaş'), findsOneWidget);
    expect(find.text('omvila.com/kuafor-elif'), findsOneWidget);
    expect(find.text('Gün akışı'), findsNothing);

    await tester.tap(find.text('Kapıdan gelen müşteriyi sıraya al'));
    await tester.pumpAndSettle();
    expect(find.text('Randevusuz gelenler'), findsOneWidget);
    expect(find.text('Sırada kimse yok'), findsOneWidget);
    // Yöneticide Sıra sekme değil: geri oku var, sekme çubuğu yok.
    await tester.tap(find.byTooltip('Geri'));
    await tester.pumpAndSettle();
    expect(find.text('Bugün randevunuz yok'), findsOneWidget);
  });

  testWidgets('bildirim kapalı şeridi: aç ve kapat', (tester) async {
    final h = Harness()
      ..push = FakePushService(initial: PushPermission.denied)
      ..signIn();
    h.api.reply('GET', todayPath, todayJson());
    await h.pumpApp(tester);

    expect(find.text('Bildirimleri aç'), findsOneWidget);
    expect(
      find.textContaining('Onay bekleyen talepleri kaçırabilirsiniz.'),
      findsOneWidget,
    );
    await tester.tap(find.byTooltip('Şeridi kapat'));
    await tester.pumpAndSettle();
    expect(find.text('Bildirimleri aç'), findsNothing);
    expect(h.store.values['push.bannerDismissed'], '1');
  });

  testWidgets('bildirime dokununca randevu ayrıntısı açılır', (tester) async {
    final h = Harness()..signIn();
    h.api.reply('GET', todayPath, todayJson());
    await h.pumpApp(tester);

    h.push.simulateOpen(const AppointmentTarget(pendingId));
    await tester.pumpAndSettle();
    expect(find.text('Randevu'), findsWidgets);
    expect(find.text('Bugün'), findsNothing);
  });
}

/// Öğeyi ekranın ortasına kaydırıp dokunur (yüzen sekme çubuğunun altında
/// kalmasın).
Future<void> tapCentered(WidgetTester tester, Finder finder) async {
  await Scrollable.ensureVisible(tester.element(finder), alignment: 0.5);
  await tester.pumpAndSettle();
  await tester.tap(finder);
}
