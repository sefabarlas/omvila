import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:omvila_ui/omvila_ui.dart';

import '../helpers/appointment_fixtures.dart';
import '../helpers/fake_api.dart';
import '../helpers/fixtures.dart';

// Kural: dilim-05 KK10/KK11, mobile/isletme/AGENTS.md — Takvim (Takvim,
// TakvimBos, TakvimKapali): gün şeridi, personel kolonları, gün geçişi,
// filtre; personel rolünde "Takvimim" ve yalnız kendi kolonu; "Bekleme ·
// müsait" dilimi; tarih sorguları yalnız "yyyy-aa-gg" (sunucu
// LocalDatePattern.Iso).
void main() {
  const calendarPath = '/api/v1/manage/calendar';

  Future<Harness> openCalendar(
    WidgetTester tester, {
    Map<String, Object?>? session,
    FutureOr<FakeResponse> Function()? calendar,
  }) async {
    final h = Harness()..signIn(session: session);
    h.api
      ..reply('GET', '/api/v1/manage/today', todayJson())
      ..on(
        'GET',
        calendarPath,
        (_) =>
            calendar == null ? FakeResponse(200, calendarJson()) : calendar(),
      );
    await h.pumpApp(tester);
    await tester.tap(
      find.bySemanticsLabel(
        session == staffSessionJson ? 'Takvimim' : 'Takvim',
      ),
    );
    await tester.pumpAndSettle();
    return h;
  }

  testWidgets('yükleniyor', (tester) async {
    final pending = Completer<FakeResponse>();
    final h = Harness()..signIn();
    h.api
      ..reply('GET', '/api/v1/manage/today', todayJson())
      ..on('GET', calendarPath, (_) => pending.future);
    await h.pumpApp(tester);
    await tester.tap(find.bySemanticsLabel('Takvim'));
    await tester.pump();
    await tester.pump();
    expect(find.byType(CircularProgressIndicator), findsOneWidget);
    pending.complete(FakeResponse(200, calendarJson()));
    await tester.pumpAndSettle();
    expect(find.bySemanticsLabel(RegExp('Selin Öztürk')), findsOne);
  });

  testWidgets('hata: mesaj ve Tekrar dene', (tester) async {
    var fail = true;
    await openCalendar(
      tester,
      calendar: () => fail
          ? const FakeResponse(500, {'code': 'try_again'})
          : FakeResponse(200, calendarJson()),
    );
    expect(find.text('Takvim yüklenemedi'), findsOneWidget);
    expect(find.text(errorMessage('try_again')), findsOneWidget);
    fail = false;
    await tester.tap(find.text('Tekrar dene'));
    await tester.pumpAndSettle();
    expect(find.bySemanticsLabel(RegExp('Selin Öztürk')), findsOne);
  });

  testWidgets('dolu: kolonlar, bloklar, bekleme dilimi, mola', (tester) async {
    final h = await openCalendar(tester);

    final request = h.api.to('GET', calendarPath).single.options;
    expect(request.uri.queryParameters['from'], '2026-11-23');
    expect(request.uri.queryParameters['to'], '2026-12-06');
    expect(request.uri.queryParameters['branchId'], branchId);

    expect(find.text('Kasım 2026 · Kadıköy'), findsOneWidget);
    expect(find.text('Takvim'), findsWidgets);
    for (final name in ['Elif', 'Can', 'Zeynep']) {
      expect(find.text(name), findsOneWidget);
    }

    expect(find.bySemanticsLabel(RegExp('^10:30 Selin Öztürk')), findsOne);
    expect(find.text('Bekleme · müsait'), findsOneWidget);
    expect(find.text('Mola'), findsOneWidget);
    expect(
      find.bySemanticsLabel(RegExp('Gizem Arslan.*Kapora bekleniyor')),
      findsOne,
    );
    expect(find.byTooltip('Personeli filtrele'), findsOneWidget);
    expect(find.byTooltip('Yeni randevu'), findsOneWidget);
  });

  testWidgets('bloğa dokununca randevu ayrıntısı açılır', (tester) async {
    final h = await openCalendar(tester);
    h.api.reply(
      'GET',
      '/api/v1/manage/appointments/$confirmedId',
      confirmedJson(),
    );
    await tester.tap(find.bySemanticsLabel(RegExp('^13:00 Derya Şahin')));
    await tester.pumpAndSettle();
    expect(find.text('Randevu'), findsOneWidget);
    expect(find.text('Derya Şahin'), findsOneWidget);
  });

  testWidgets('filtre: staffIds sunucuya virgülle gider, personel gizlenir', (
    tester,
  ) async {
    final h = Harness()..signIn();
    h.api
      ..reply('GET', '/api/v1/manage/today', todayJson())
      ..on('GET', calendarPath, (request) {
        final ids = request.options.uri.queryParameters['staffIds'];
        return FakeResponse(
          200,
          calendarJson(onlyStaff: ids?.split(',').toSet()),
        );
      });
    await h.pumpApp(tester);
    await tester.tap(find.bySemanticsLabel('Takvim'));
    await tester.pumpAndSettle();

    await tester.tap(find.byTooltip('Personeli filtrele'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Can Yücel'));
    await tester.tap(find.text('Uygula'));
    await tester.pumpAndSettle();

    final request = h.api.to('GET', calendarPath).last.options;
    expect(
      request.uri.queryParametersAll['staffIds'],
      [
        ([elifId, zeynepId]..sort()).join(','),
      ],
    );
    expect(find.text('Can'), findsNothing);
    expect(find.bySemanticsLabel(RegExp('Gizem Arslan')), findsNothing);
    expect(find.text('Elif'), findsOneWidget);

    // Filtre sayfası filtreli yanıtta da bütün personeli listeler.
    await tester.tap(find.byTooltip('Personeli filtrele'));
    await tester.pumpAndSettle();
    expect(find.text('Can Yücel'), findsOneWidget);
  });

  testWidgets('boş gün: TakvimBos ve Randevu ekle', (tester) async {
    await openCalendar(
      tester,
      calendar: () => FakeResponse(200, calendarJson(empty: true)),
    );
    expect(find.text('Çarşamba 25 Kasım boş'), findsOneWidget);
    expect(
      find.text('Bu güne henüz randevu yok. Çalışma saatleri 09:00–18:00.'),
      findsOneWidget,
    );
    await tester.tap(find.text('Randevu ekle'));
    await tester.pumpAndSettle();
    expect(find.text('Yeni randevu'), findsOneWidget);
  });

  testWidgets('gün geçişi: kaydırınca sonraki gün', (tester) async {
    await openCalendar(tester);
    await tester.fling(
      find.text('Bekleme · müsait'),
      const Offset(-300, 0),
      1000,
    );
    await tester.pumpAndSettle();
    expect(find.text('Perşembe 26 Kasım boş'), findsOneWidget);
  });

  testWidgets('kapalı gün: TakvimKapali, + pasif', (tester) async {
    await openCalendar(tester);
    final sunday = find.text('Kapalı', skipOffstage: false);
    await tester.ensureVisible(sunday);
    await tester.pumpAndSettle();
    await tester.tap(sunday);
    await tester.pumpAndSettle();
    expect(find.text('Pazar · salon kapalı'), findsOneWidget);
    expect(
      find.text('Haftalık kapalı gün. Bu güne randevu alınmaz.'),
      findsOneWidget,
    );
    final add = tester.widget<IconButton>(
      find.ancestor(
        of: find.byTooltip('Yeni randevu'),
        matching: find.byType(IconButton),
      ),
    );
    expect(add.onPressed, isNull);
  });

  testWidgets('personel: Takvimim, yalnız kendi kolonu, filtre yok', (
    tester,
  ) async {
    await openCalendar(
      tester,
      session: staffSessionJson,
      calendar: () => FakeResponse(200, calendarJson(staffOnly: true)),
    );
    expect(find.text('Takvimim'), findsWidgets);
    expect(find.byTooltip('Personeli filtrele'), findsNothing);
    expect(find.text('Can'), findsNothing);
    expect(find.text('Elif'), findsOneWidget);
  });
}
