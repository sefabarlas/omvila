import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import '../helpers/appointment_fixtures.dart';
import '../helpers/fake_api.dart';
import '../helpers/fixtures.dart';
import '../helpers/open_tab.dart';
import '../helpers/screen_fixtures.dart';
import 'today_screen_test.dart' show tapCentered;

// Kural: dilim-05 KK11, docs/standartlar/bildirimler.md — Bildirimler,
// BildirimlerBos: gün gruplu, okunmamış noktası, "Okundu say" (all),
// sekmeler (Onay bekleyen: staff.approval_needed; Okunmamış: unreadOnly),
// satır içi "Onayla"/"Reddet" yalnız appointment.approve ve okunmamış,
// süresi dolmamış talepte; kapora bekleyen randevuda deposit_required →
// ayrıntı; satıra dokununca okundu sayılır ve hedef ekran açılır. Personel
// rolünde Bildirimler sekmesi yok (roller-yetkiler.md).
void main() {
  const path = '/api/v1/manage/notifications';
  const readPath = '/api/v1/manage/notifications/read';
  const pendingPath = '/api/v1/manage/appointments/$pendingDenizId';

  Future<Harness> open(
    WidgetTester tester, {
    FutureOr<FakeResponse> Function(RecordedRequest)? list,
    bool settle = true,
  }) => openTab(
    tester,
    'Bildirimler',
    settle: settle,
    setup: (api) => api
      ..on(
        'GET',
        path,
        list ?? (_) => FakeResponse(200, notificationsJson()),
      )
      ..reply('POST', readPath, {'unreadCount': 0})
      ..reply('GET', pendingPath, pendingDetailJson),
  );

  testWidgets('yükleniyor', (tester) async {
    final pending = Completer<FakeResponse>();
    await open(tester, list: (_) => pending.future, settle: false);
    expect(find.byType(CircularProgressIndicator), findsOneWidget);
    pending.complete(FakeResponse(200, notificationsJson()));
    await tester.pumpAndSettle();
    expect(find.text('Yeni online randevu'), findsOneWidget);
  });

  testWidgets('hata: mesaj ve Tekrar dene', (tester) async {
    var fail = true;
    await open(
      tester,
      list: (_) => fail
          ? const FakeResponse(500, {'code': 'try_again'})
          : FakeResponse(200, notificationsJson()),
    );
    expect(find.text('Bildirimler yüklenemedi'), findsOneWidget);
    fail = false;
    await tester.tap(find.text('Tekrar dene'));
    await tester.pumpAndSettle();
    expect(find.text('Yeni online randevu'), findsOneWidget);
  });

  testWidgets('boş: BildirimlerBos ve Bildirim ayarları', (tester) async {
    final h = await open(
      tester,
      list: (_) => FakeResponse(200, notificationsJson(empty: true)),
    );
    h.api.reply(
      'GET',
      '/api/v1/manage/me/notification-preferences',
      preferencesJson(),
    );
    expect(find.text('Yeni bildirim yok'), findsOneWidget);
    expect(find.text('Kadıköy'), findsOneWidget);
    expect(find.text('Okundu say'), findsNothing);
    // BildirimlerBos'ta segment kontrolü yok.
    expect(find.text('Onay bekleyen'), findsNothing);
    await tester.tap(find.text('Bildirim ayarları'));
    await tester.pumpAndSettle();
    expect(find.text('Yeni online randevu'), findsOneWidget);
  });

  testWidgets('dolu: gün grupları, saat, kalan süre, Onayla/Reddet', (
    tester,
  ) async {
    await open(tester);
    expect(find.text('3 okunmamış'), findsOneWidget);
    expect(find.text('Bugün'), findsWidgets);
    expect(find.text('Dün'), findsOneWidget);
    // 09:02Z → 12:02; tutma 10:45Z, saat 09:10Z → 1 sa 35 dk.
    expect(find.text('12:02'), findsOneWidget);
    expect(
      find.text(
        'Pınar Yalçın · Cilt bakımı, 16:00 · Zeynep. Yanıt için 1 sa 35 dk '
        'kaldı.',
      ),
      findsOneWidget,
    );
    expect(find.text('Onayla'), findsOneWidget);
    expect(find.text('Reddet'), findsOneWidget);
    expect(find.bySemanticsLabel(RegExp('Okunmadı')), findsNWidgets(3));
  });

  // Sekme rozeti sunucunun unreadCount'u (limit=1); okundu sayınca yeniden
  // okunur, 0 ise rozet yok (sahte sayı yok).
  testWidgets('sekme rozeti okunmamış sayısı; okundu sayınca kalkar', (
    tester,
  ) async {
    var unread = 3;
    final h = await open(
      tester,
      list: (r) => FakeResponse(200, {
        ...notificationsJson(),
        'unreadCount': unread,
      }),
    );
    expect(find.bySemanticsLabel('Bildirimler, 3 okunmamış'), findsOneWidget);
    expect(
      h.api.to('GET', path).any((r) => r.options.queryParameters['limit'] == 1),
      isTrue,
    );
    unread = 0;
    await tester.tap(find.text('Okundu say'));
    await tester.pumpAndSettle();
    expect(find.bySemanticsLabel('Bildirimler'), findsOneWidget);
    expect(find.bySemanticsLabel(RegExp('okunmamış')), findsNothing);
  });

  testWidgets('Okundu say: hepsini okundu sayar', (tester) async {
    final h = await open(tester);
    await tester.tap(find.text('Okundu say'));
    await tester.pumpAndSettle();
    expect(h.api.to('POST', readPath).single.body, {'all': true});
  });

  testWidgets('sekmeler: Onay bekleyen süzer, Okunmamış sunucudan', (
    tester,
  ) async {
    final h = await open(tester);
    await tester.tap(find.text('Onay bekleyen'));
    await tester.pumpAndSettle();
    expect(find.text('Yeni online randevu'), findsOneWidget);
    expect(find.text('Kapora bekleniyor'), findsNothing);

    await tester.tap(find.text('Okunmamış'));
    await tester.pumpAndSettle();
    expect(
      h.api.to('GET', path).last.options.queryParameters['unreadOnly'],
      true,
    );
  });

  testWidgets('Onayla: If-Match, okundu sayar', (tester) async {
    final h = await open(tester);
    h.api.reply('POST', '$pendingPath/approve', pendingDetailJson);
    await tapCentered(tester, find.text('Onayla'));
    await tester.pumpAndSettle();
    expect(
      h.api.to('POST', '$pendingPath/approve').single.header('If-Match'),
      'v7',
    );
    expect(h.api.to('POST', readPath).single.body, {
      'notificationIds': ['0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5b01'],
    });
    expect(find.text('Talep onaylandı.'), findsOneWidget);
  });

  testWidgets('Onayla: kapora bekleniyorsa ayrıntıya gider', (tester) async {
    final h = await open(tester);
    h.api.reply('POST', '$pendingPath/approve', {
      'code': 'deposit_required',
    }, status: 409);
    await tapCentered(tester, find.text('Onayla'));
    await tester.pumpAndSettle();
    expect(h.api.to('GET', pendingPath), hasLength(greaterThan(1)));
  });

  testWidgets('Reddet: decline ucu If-Match ile', (tester) async {
    final h = await open(tester);
    h.api.reply('POST', '$pendingPath/decline', pendingDetailJson);
    await tapCentered(tester, find.text('Reddet'));
    await tester.pumpAndSettle();
    expect(
      h.api.to('POST', '$pendingPath/decline').single.header('If-Match'),
      'v7',
    );
    expect(find.text('Talep iptal edildi.'), findsOneWidget);
  });

  testWidgets('satıra dokununca okundu sayar ve kaporaya gider', (
    tester,
  ) async {
    final h = await open(tester);
    h.api.reply(
      'GET',
      '/api/v1/manage/appointments/$depositId',
      depositJson(),
    );
    await tester.tap(find.text('Kapora bekleniyor'));
    await tester.pumpAndSettle();
    expect(h.api.to('POST', readPath).single.body, {
      'notificationIds': ['0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5b04'],
    });
    expect(
      h.api.to('GET', '/api/v1/manage/appointments/$depositId'),
      isNotEmpty,
    );
  });

  testWidgets('onay izni yoksa satır içi butonlar yok', (tester) async {
    final h = Harness()
      ..signIn(
        session: {
          ...ownerSessionJson,
          'permissions': [
            for (final p in ownerSessionJson['permissions']! as List<String>)
              if (p != 'appointment.approve') p,
          ],
        },
      );
    h.api
      ..reply('GET', '/api/v1/manage/today', todayJson())
      ..reply('GET', path, notificationsJson());
    await h.pumpApp(tester);
    await tester.tap(find.bySemanticsLabel(RegExp('^Bildirimler')));
    await tester.pumpAndSettle();
    expect(find.text('Yeni online randevu'), findsOneWidget);
    expect(find.text('Onayla'), findsNothing);
    expect(find.text('Reddet'), findsNothing);
  });
}
