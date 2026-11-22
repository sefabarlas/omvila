import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:omvila_isletme/shared/ui.dart';
import 'package:omvila_ui/omvila_ui.dart';

import '../helpers/appointment_fixtures.dart';
import '../helpers/fake_api.dart';
import '../helpers/fixtures.dart';
import '../helpers/open_tab.dart';
import '../helpers/screen_fixtures.dart';
import 'today_screen_test.dart' show tapCentered;

// Kural: dilim-05 KK4/KK10, mobile/isletme/AGENTS.md § Kapıda sıra — Sira,
// SiraBos, SiraEkle: bekleyen sayısı, "~N dk", bekleme süresi sunucunun
// generatedUtc'sinden; "Sıraya ekle" Idempotency-Key ile, numara önizlenmez
// (sunucunun verdiği numara yazılır); "Başlat" If-Match, bilette hizmet veya
// personel yoksa sorar, personel yalnız kendine başlatır; "Sıradan çıkar".
void main() {
  const queuePath = '/api/v1/manage/walk-in-tickets';

  void catalog(FakeApi api) => api
    ..reply('GET', '/api/v1/manage/services', servicesJson())
    ..reply('GET', '/api/v1/manage/staff', staffJson());

  /// Personel oturumunda Sıra sekmesi.
  Future<Harness> openAsStaff(
    WidgetTester tester, {
    FutureOr<FakeResponse> Function()? queue,
  }) => openTab(
    tester,
    'Kapıda sıra',
    session: staffSessionJson,
    setup: (api) {
      catalog(api);
      api.on(
        'GET',
        queuePath,
        (_) => queue == null ? FakeResponse(200, walkInQueueJson()) : queue(),
      );
    },
  );

  /// Sahipte Bugün'deki "SIRADA" bloğundan.
  Future<Harness> openAsOwner(WidgetTester tester) async {
    final h = Harness()..signIn();
    h.api
      ..reply('GET', '/api/v1/manage/today', todayJson())
      ..reply('GET', queuePath, walkInQueueJson());
    catalog(h.api);
    await h.pumpApp(tester);
    await tapCentered(tester, find.text('SIRADA'));
    await tester.pumpAndSettle();
    return h;
  }

  testWidgets('yükleniyor', (tester) async {
    final pending = Completer<FakeResponse>();
    final h = Harness()..signIn(session: staffSessionJson);
    h.api
      ..reply('GET', '/api/v1/manage/today', todayJson(staff: true))
      ..on('GET', queuePath, (_) => pending.future);
    await h.pumpApp(tester);
    await tester.tap(find.bySemanticsLabel('Kapıda sıra'));
    await tester.pump();
    await tester.pump();
    expect(find.byType(CircularProgressIndicator), findsOneWidget);
    pending.complete(FakeResponse(200, walkInQueueJson()));
    await tester.pumpAndSettle();
    expect(find.text('Okan Er'), findsOneWidget);
  });

  testWidgets('hata: mesaj ve Tekrar dene', (tester) async {
    var fail = true;
    await openAsStaff(
      tester,
      queue: () => fail
          ? const FakeResponse(500, {'code': 'try_again'})
          : FakeResponse(200, walkInQueueJson()),
    );
    expect(find.text('Sıra yüklenemedi'), findsOneWidget);
    expect(find.text(errorMessage('try_again')), findsOneWidget);
    fail = false;
    await tester.tap(find.text('Tekrar dene'));
    await tester.pumpAndSettle();
    expect(find.text('Okan Er'), findsOneWidget);
  });

  testWidgets('boş: SiraBos ve Sıraya ekle', (tester) async {
    await openAsStaff(
      tester,
      queue: () => FakeResponse(200, walkInQueueJson(empty: true)),
    );
    expect(find.text('Sırada kimse yok'), findsOneWidget);
    expect(find.text('BEKLEYEN'), findsNothing);
    await tester.tap(find.text('Sıraya ekle'));
    await tester.pumpAndSettle();
    expect(find.text('Ad'), findsOneWidget);
  });

  testWidgets('dolu: bekleyen, tahmini bekleme ve satırlar', (tester) async {
    await openAsStaff(tester);
    expect(find.text('Randevusuz gelenler'), findsOneWidget);
    expect(find.text('BEKLEYEN'), findsOneWidget);
    expect(find.text('kişi'), findsOneWidget);
    expect(find.text('~35 dk'), findsOneWidget);
    // generatedUtc 09:10, katılma 08:58.
    expect(
      find.text("Saç kesimi · Zeynep · 12 dk'dır bekliyor"),
      findsOneWidget,
    );
    expect(find.text('Sakal · Fark etmez · 8 dk'), findsOneWidget);
    expect(find.text('Emre Yavuz'), findsOneWidget);
    expect(find.text('Başlat'), findsOneWidget);
    // Personelde Sıra sekme: geri oku yok, sekme çubuğu var.
    expect(find.byTooltip('Geri'), findsNothing);
    expect(find.byType(YuzenSekmeCubugu), findsOneWidget);
  });

  testWidgets('Sıraya ekle: anahtarla gönderir, dönen numarayı yazar', (
    tester,
  ) async {
    final h = await openAsStaff(tester);
    h.api.reply('POST', queuePath, createdTicketJson(), status: 201);
    await tester.tap(find.text('Sıraya ekle'));
    await tester.pumpAndSettle();
    // Numara önizlenmez.
    expect(find.textContaining('No '), findsNothing);
    expect(
      find.text('Tahmini bekleme ~35 dk · önünde 3 kişi var.'),
      findsOneWidget,
    );

    await tester.enterText(_field('Ad'), 'Burak');
    await tester.enterText(
      _field('Telefon (isteğe bağlı)'),
      '532 123',
    );
    await tester.pump();
    expect(find.text('Cep telefonu numarası girin'), findsOneWidget);
    await tester.enterText(
      _field('Telefon (isteğe bağlı)'),
      '',
    );
    await tester.tap(find.text('Saç kesimi'));
    await tester.pump();
    await tapCentered(
      tester,
      find.widgetWithText(OmvilaButon, 'Sıraya ekle').last,
    );
    await tester.pumpAndSettle();

    final post = h.api.to('POST', queuePath).single;
    expect(post.header('Idempotency-Key'), isNotEmpty);
    expect(post.body, {
      'branchId': branchId,
      'fullName': 'Burak',
      'serviceId': kesimId,
      'preferredStaffId': null,
    });
    expect(find.text('Burak sıraya eklendi · No 11'), findsOneWidget);
  });

  testWidgets('sahip: Bugün → Sıra, Başlat If-Match ile gövdesiz', (
    tester,
  ) async {
    final h = await openAsOwner(tester);
    h.api.reply(
      'POST',
      '$queuePath/$okanTicketId/start',
      startedJson(okanTicketId),
    );
    // Yöneticide Sıra sekme değil: geri oku var, sekme çubuğu yok.
    expect(find.byTooltip('Geri'), findsOneWidget);
    expect(find.byType(YuzenSekmeCubugu), findsNothing);
    await tester.tap(find.text('Başlat'));
    await tester.pumpAndSettle();

    final post = h.api.to('POST', '$queuePath/$okanTicketId/start').single;
    expect(post.header('If-Match'), 'w1');
    expect(post.body, isNull);
    expect(find.text('Okan Er randevuya çevrildi.'), findsOneWidget);
  });

  testWidgets('Başlat: personel "Fark etmez" ise sorar', (tester) async {
    final h = await openAsOwner(tester);
    h.api.reply(
      'POST',
      '$queuePath/$muratTicketId/start',
      startedJson(muratTicketId),
    );
    await tester.tap(find.byTooltip('Murat Çelik için seçenekler'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Başlat').last);
    await tester.pumpAndSettle();

    expect(find.text('Murat Çelik için başlat'), findsOneWidget);
    // Hizmet bilette var: yalnız personel sorulur, "Fark etmez" yok.
    expect(find.text('Hizmet'), findsNothing);
    expect(find.text('Fark etmez'), findsNothing);
    await tester.tap(find.text('Can'));
    await tester.pump();
    await tapCentered(tester, find.widgetWithText(OmvilaButon, 'Başlat').last);
    await tester.pumpAndSettle();

    final post = h.api.to('POST', '$queuePath/$muratTicketId/start').single;
    expect(post.header('If-Match'), 'w2');
    expect(post.body, {'staffId': canId});
  });

  testWidgets('Sıradan çıkar', (tester) async {
    final h = await openAsOwner(tester);
    h.api.reply('POST', '$queuePath/$emreTicketId/leave', {
      ...(walkInQueueJson()['items']! as List).last as Map<String, Object?>,
      'status': 'Left',
    });
    await tester.tap(find.byTooltip('Emre Yavuz için seçenekler'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Sıradan çıkar'));
    await tester.pumpAndSettle();

    final post = h.api.to('POST', '$queuePath/$emreTicketId/leave').single;
    expect(post.header('If-Match'), 'w3');
    expect(find.text('Emre Yavuz sıradan çıkarıldı.'), findsOneWidget);
  });

  testWidgets('personel: Başlat yalnız kendine', (tester) async {
    final h = await openAsStaff(tester);
    h.api.reply(
      'POST',
      '$queuePath/$okanTicketId/start',
      startedJson(okanTicketId),
    );
    await tester.tap(find.text('Başlat'));
    await tester.pumpAndSettle();
    final post = h.api.to('POST', '$queuePath/$okanTicketId/start').single;
    expect(post.body, {'staffId': canId});
  });

  testWidgets('Başlat: personel meşgulse mesaj', (tester) async {
    final h = await openAsOwner(tester);
    h.api.reply('POST', '$queuePath/$okanTicketId/start', {
      'code': 'slot_taken',
    }, status: 409);
    await tester.tap(find.text('Başlat'));
    await tester.pumpAndSettle();
    expect(find.text(errorMessage('slot_taken')), findsOneWidget);
  });
}

/// Etiketi alanın üstünde olan metin alanı.
Finder _field(String label) => find.descendant(
  of: find.widgetWithText(LabeledField, label),
  matching: find.byType(TextField),
);
