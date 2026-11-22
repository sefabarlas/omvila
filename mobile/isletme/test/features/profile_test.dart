import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:omvila_ui/omvila_ui.dart';

import '../helpers/fake_api.dart';
import '../helpers/fixtures.dart';
import '../helpers/open_tab.dart';
import '../helpers/screen_fixtures.dart';
import 'today_screen_test.dart' show tapCentered;

// Kural: dilim-05 KK10, K2, mobile/isletme/AGENTS.md — Profil: yalnız kendi
// primi (manageGetMyCommissionSummary), ay adı yanıttan ("KASIM PRİMİM"),
// personel kaydı yoksa 404 → prim bölümü gizli, commission.view yoksa hiç
// istenmez; "Çıkış yap". BildirimAyar: anahtar değişince bütün tercihler
// If-Match ile kaydedilir; 412'de güncel tercihler okunur; sessiz saat
// aralığı salt okunur; kapatılamaz olaylar yalnız sahip/yöneticiye yazılır.
void main() {
  const commissionPath = '/api/v1/manage/me/commission-summary';
  const prefsPath = '/api/v1/manage/me/notification-preferences';

  Future<Harness> open(
    WidgetTester tester, {
    Map<String, Object?>? session,
    FutureOr<FakeResponse> Function()? commission,
    bool settle = true,
  }) => openTab(
    tester,
    'Profil',
    session: session,
    settle: settle,
    setup: (api) => api
      ..on(
        'GET',
        commissionPath,
        (_) => commission == null
            ? FakeResponse(200, commissionJson())
            : commission(),
      )
      ..reply('GET', prefsPath, preferencesJson())
      ..reply('POST', '/api/v1/manage/auth/logout', null, status: 204),
  );

  testWidgets('yükleniyor', (tester) async {
    final pending = Completer<FakeResponse>();
    await open(tester, commission: () => pending.future, settle: false);
    expect(find.byType(CircularProgressIndicator), findsOneWidget);
    pending.complete(FakeResponse(200, commissionJson()));
    await tester.pumpAndSettle();
    expect(find.text('KASIM PRİMİM'), findsOneWidget);
  });

  testWidgets('hata: mesaj ve Tekrar dene', (tester) async {
    var fail = true;
    await open(
      tester,
      commission: () => fail
          ? const FakeResponse(500, {'code': 'try_again'})
          : FakeResponse(200, commissionJson()),
    );
    expect(find.text('Primim yüklenemedi'), findsOneWidget);
    fail = false;
    await tester.tap(find.text('Tekrar dene'));
    await tester.pumpAndSettle();
    expect(find.text('KASIM PRİMİM'), findsOneWidget);
  });

  testWidgets('dolu: prim, sayaçlar, hafta, son primler', (tester) async {
    await open(tester);
    expect(find.text('Elif Kaya'), findsOneWidget);
    expect(find.text('İşletme sahibi · Kadıköy'), findsOneWidget);
    expect(find.text('KASIM PRİMİM'), findsOneWidget);
    expect(find.text('1–25 Kasım'), findsOneWidget);
    expect(find.text(formatMoney(684000)), findsOneWidget);
    expect(
      find.text('Hizmet geliri ${formatMoney(2850000)}'),
      findsOneWidget,
    );
    expect(find.text('94'), findsOneWidget);
    expect(find.text('4,9'), findsOneWidget);
    expect(find.text('%4'), findsOneWidget);
    expect(find.text('Çar en yoğun'), findsOneWidget);
    // "Bu hafta" Pazartesi–Pazar sabit yedi sütun; gelecek günler boş.
    for (final day in ['Pzt', 'Sal', 'Per', 'Cum', 'Cmt', 'Paz']) {
      expect(find.text(day), findsOneWidget);
    }
    expect(find.bySemanticsLabel(RegExp('^Pazar ')), findsNothing);
    expect(find.text('Ayşe Yılmaz'), findsOneWidget);
    expect(find.text('Bugün 09:30 · Saç kesimi'), findsOneWidget);
    expect(find.text('+${formatMoney(9000)}'), findsOneWidget);
    expect(find.text('Dün 17:00 · Keratin'), findsOneWidget);
    expect(find.text('Derya Şahin'), findsNothing);
    await tapCentered(tester, find.text('Tümü'));
    await tester.pumpAndSettle();
    expect(find.text('Derya Şahin'), findsOneWidget);
    expect(
      find.textContaining('Selin Öztürk randevusu bitince eklenecek.'),
      findsOneWidget,
    );
  });

  testWidgets('personel kaydı yoksa (404) prim bölümü gizli', (tester) async {
    await open(
      tester,
      commission: () => const FakeResponse(404, {'code': 'not_found'}),
    );
    expect(find.text('KASIM PRİMİM'), findsNothing);
    expect(find.text('Primim yüklenemedi'), findsNothing);
    expect(find.text('Çıkış yap'), findsOneWidget);
  });

  testWidgets('commission.view yoksa prim istenmez', (tester) async {
    final h = await open(
      tester,
      session: {
        ...ownerSessionJson,
        'permissions': [
          for (final p in ownerSessionJson['permissions']! as List<String>)
            if (p != 'commission.view') p,
        ],
      },
    );
    expect(h.api.to('GET', commissionPath), isEmpty);
    expect(find.text('Çıkış yap'), findsOneWidget);
  });

  testWidgets('personel: yalnız kendi primi, rol adı', (tester) async {
    final h = await open(tester, session: staffSessionJson);
    expect(find.text('Personel · Kadıköy'), findsOneWidget);
    expect(find.text('KASIM PRİMİM'), findsOneWidget);
    final request = h.api.to('GET', commissionPath).single;
    // Kendi özeti: kimlik veya personel parametresi gönderilmez.
    expect(request.options.queryParameters, isEmpty);
  });

  testWidgets('Çıkış yap: girişe döner', (tester) async {
    await open(tester);
    await tapCentered(tester, find.text('Çıkış yap'));
    await tester.pumpAndSettle();
    expect(find.text('Kod gönder'), findsOneWidget);
  });

  group('Bildirim ayarları', () {
    Future<Harness> openSettings(
      WidgetTester tester, {
      Map<String, Object?>? session,
      FutureOr<FakeResponse> Function()? prefs,
    }) async {
      final h = await open(tester, session: session);
      if (prefs != null) {
        h.api.on('GET', prefsPath, (_) => prefs());
      }

      await tester.tap(find.byTooltip('Bildirim ayarları'));
      await tester.pumpAndSettle();
      return h;
    }

    testWidgets('dolu: anahtarlar, saatler, kapatılamaz olaylar', (
      tester,
    ) async {
      await openSettings(tester);
      expect(
        find.text(
          'Yalnızca sizin telefonunuza gelenler. Müşteri mesajları işletme '
          'ayarlarındadır.',
        ),
        findsOneWidget,
      );
      expect(find.byType(Switch), findsNWidgets(7));
      expect(find.text('Her akşam 20:00'), findsOneWidget);
      expect(find.text('22:00–08:00 arası bildirim gelmez'), findsOneWidget);
      expect(find.textContaining('kapatılamaz'), findsOneWidget);
    });

    testWidgets('anahtar: hepsini If-Match ile kaydeder', (tester) async {
      final h = await openSettings(tester);
      h.api.reply(
        'PUT',
        prefsPath,
        {...preferencesJson(version: 'W/"413"'), 'customerCheckedIn': true},
      );
      await tester.tap(find.text('Müşteri geldi'));
      await tester.pumpAndSettle();
      final put = h.api.to('PUT', prefsPath).single;
      expect(put.header('If-Match'), 'W/"412"');
      final body = put.body! as Map<String, Object?>;
      expect(body['customerCheckedIn'], true);
      expect(body['newOnlineAppointment'], true);
      expect(body['dailySummary'], false);
    });

    testWidgets('412: güncel tercihler yeniden okunur', (tester) async {
      final h = await openSettings(tester);
      h.api.reply('PUT', prefsPath, {
        'code': 'precondition_failed',
      }, status: 412);
      final before = h.api.to('GET', prefsPath).length;
      await tester.tap(find.text('Yeni değerlendirme'));
      await tester.pumpAndSettle();
      expect(find.text(errorMessage('precondition_failed')), findsOneWidget);
      expect(h.api.to('GET', prefsPath).length, before + 1);
    });

    testWidgets('hata ve Tekrar dene', (tester) async {
      var fail = true;
      await openSettings(
        tester,
        prefs: () => fail
            ? const FakeResponse(500, {'code': 'try_again'})
            : FakeResponse(200, preferencesJson()),
      );
      expect(find.text('Bildirim ayarları yüklenemedi'), findsOneWidget);
      fail = false;
      await tester.tap(find.text('Tekrar dene'));
      await tester.pumpAndSettle();
      expect(find.byType(Switch), findsNWidgets(7));
    });

    testWidgets('personel: kapatılamaz olay notu yok', (tester) async {
      await openSettings(tester, session: staffSessionJson);
      expect(find.textContaining('kapatılamaz'), findsNothing);
    });
  });
}
