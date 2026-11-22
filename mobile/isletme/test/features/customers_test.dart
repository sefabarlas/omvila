import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:omvila_ui/omvila_ui.dart';

import '../helpers/appointment_fixtures.dart';
import '../helpers/fake_api.dart';
import '../helpers/fixtures.dart';
import '../helpers/open_tab.dart';
import '../helpers/screen_fixtures.dart';
import 'today_screen_test.dart' show tapCentered;

// Kural: dilim-05 KK10/KK11, mobile/isletme/AGENTS.md — Musteriler,
// MusterilerAramaYok, MusterilerBos: arama (ad, telefon, son 4 hane),
// sözleşmedeki süzgeçler, rozetler sunucu etiketlerinden, "Filtreyi
// temizle", "Yeni müşteri olarak ekle" (Idempotency-Key, kayıtlı numarada
// "Mevcut kartı aç"); MusteriKarti/MusteriKartiYeni: sayaçlar, paket, not
// (sağlık bilgisi uyarısı), geçmiş, "Randevu ver" ön dolu; Ara/SMS numara
// yanıtta yoksa manageRevealCustomerPhone; personel rolünde telefon maskeli.
void main() {
  const listPath = '/api/v1/manage/customers';
  const selinPath = '/api/v1/manage/customers/$selinId';

  void card(FakeApi api, {bool staff = false}) => api
    ..reply('GET', selinPath, selinJson(staff: staff))
    ..reply('GET', '$selinPath/appointments', customerAppointmentsJson())
    ..reply('GET', '$selinPath/packages', fonPackagePageJson());

  Future<Harness> openList(
    WidgetTester tester, {
    Map<String, Object?>? session,
    FutureOr<FakeResponse> Function(RecordedRequest)? list,
    bool settle = true,
  }) => openTab(
    tester,
    'Müşteriler',
    session: session,
    settle: settle,
    setup: (api) {
      api.on(
        'GET',
        listPath,
        list ??
            (_) => FakeResponse(
              200,
              customerListJson(staff: session == staffSessionJson),
            ),
      );
      card(api, staff: session == staffSessionJson);
    },
  );

  Future<void> search(WidgetTester tester, String text) async {
    await tester.enterText(find.byType(TextField), text);
    await tester.pump(const Duration(milliseconds: 350));
    await tester.pumpAndSettle();
  }

  testWidgets('yükleniyor', (tester) async {
    final pending = Completer<FakeResponse>();
    await openList(tester, list: (_) => pending.future, settle: false);
    expect(find.byType(CircularProgressIndicator), findsOneWidget);
    pending.complete(FakeResponse(200, customerListJson()));
    await tester.pumpAndSettle();
    expect(find.text('Selin Öztürk'), findsOneWidget);
  });

  testWidgets('hata: mesaj ve Tekrar dene', (tester) async {
    var fail = true;
    await openList(
      tester,
      list: (_) => fail
          ? const FakeResponse(500, {'code': 'try_again'})
          : FakeResponse(200, customerListJson()),
    );
    expect(find.text('Müşteriler yüklenemedi'), findsOneWidget);
    fail = false;
    await tester.tap(find.text('Tekrar dene'));
    await tester.pumpAndSettle();
    expect(find.text('Selin Öztürk'), findsOneWidget);
  });

  testWidgets('boş: MusterilerBos, arama yok, Müşteri ekle', (tester) async {
    await openList(tester, list: (_) => const FakeResponse(200, emptyPageJson));
    expect(find.text('Henüz müşteriniz yok'), findsOneWidget);
    expect(find.text('Kadıköy'), findsOneWidget);
    expect(find.byType(TextField), findsNothing);
    await tester.tap(find.text('Müşteri ekle'));
    await tester.pumpAndSettle();
    expect(find.text('Yeni müşteri'), findsOneWidget);
  });

  testWidgets('dolu: satırlar, rozetler, yaklaşan ve son ziyaret', (
    tester,
  ) async {
    await openList(tester);
    expect(find.text('Selin Öztürk'), findsOneWidget);
    // 07:30Z → 10:30 (Europe/Istanbul).
    expect(find.text('Bugün 10:30 · 12 ziyaret'), findsOneWidget);
    expect(find.text('Düzenli'), findsWidgets);
    expect(find.text('2 kez gelmedi'), findsOneWidget);
    expect(find.text('Son ziyaret 12 Kas · 9 ziyaret'), findsOneWidget);
    // Toplam müşteri sayısı sözleşmede yok: sahte sayı gösterilmez.
    expect(find.textContaining('müşteri ·'), findsNothing);
  });

  testWidgets('arama ve süzgeç; sonuç yoksa Filtreyi temizle', (
    tester,
  ) async {
    final h = await openList(
      tester,
      list: (r) => FakeResponse(
        200,
        r.options.queryParameters.containsKey('q')
            ? emptyPageJson
            : customerListJson(),
      ),
    );
    await tester.tap(find.text('Paketli'));
    await tester.pumpAndSettle();
    await search(tester, 'Kaan Yıl');

    final last = h.api.to('GET', listPath).last.options.queryParameters;
    expect(last['q'], 'Kaan Yıl');
    expect(last['filter'], 'WithPackage');
    expect(find.text('“Kaan Yıl” için sonuç yok'), findsOneWidget);
    expect(find.textContaining('“Paketli” filtresi açık.'), findsOneWidget);

    await tester.tap(find.text('Filtreyi temizle'));
    await tester.pumpAndSettle();
    final cleared = h.api.to('GET', listPath).last.options.queryParameters;
    expect(cleared.containsKey('q'), isFalse);
    expect(cleared.containsKey('filter'), isFalse);
    expect(find.text('Selin Öztürk'), findsOneWidget);
  });

  testWidgets('Daha fazla göster: imleçle sonraki sayfa', (tester) async {
    final h = await openList(
      tester,
      list: (r) => FakeResponse(
        200,
        r.options.queryParameters['cursor'] == 'c2'
            ? customerListPage2Json()
            : customerListJson(nextCursor: 'c2'),
      ),
    );
    await tapCentered(tester, find.text('Daha fazla göster'));
    await tester.pumpAndSettle();
    expect(
      h.api.to('GET', listPath).last.options.queryParameters['cursor'],
      'c2',
    );
    expect(find.text('Nur Ateş'), findsOneWidget);
    expect(find.text('Selin Öztürk'), findsOneWidget);
    expect(find.text('Daha fazla göster'), findsNothing);
  });

  testWidgets('Yeni müşteri olarak ekle: aramadaki ad, anahtar, kart açılır', (
    tester,
  ) async {
    final h = await openList(
      tester,
      list: (r) => FakeResponse(
        200,
        r.options.queryParameters.containsKey('q')
            ? emptyPageJson
            : customerListJson(),
      ),
    );
    h.api.reply('POST', listPath, selinJson(), status: 201);
    await search(tester, 'Selin Öztürk');
    await tester.tap(find.text('Yeni müşteri olarak ekle'));
    await tester.pumpAndSettle();

    expect(find.text('Selin Öztürk'), findsWidgets);
    expect(
      find.text(
        'Notlar yalnızca salon ekibine görünür. Sağlık bilgisi yazmayın.',
      ),
      findsOneWidget,
    );
    await tester.enterText(
      find.widgetWithText(TextField, 'Telefon'),
      '0532 123 45 21',
    );
    await tester.pump();
    await tapCentered(tester, find.text('Müşteriyi ekle'));
    await tester.pumpAndSettle();

    final post = h.api.to('POST', listPath).single;
    expect(post.header('Idempotency-Key'), isNotEmpty);
    expect(post.body, {
      'fullName': 'Selin Öztürk',
      'phone': '+905321234521',
      'birthYear': null,
      'notes': null,
      'sendAppointmentSms': true,
    });
    expect(find.text('ZİYARET'), findsOneWidget);
  });

  testWidgets('Müşteri ekle: kayıtlı numarada Mevcut kartı aç', (
    tester,
  ) async {
    final h = await openList(tester);
    h.api.reply('POST', listPath, {
      'code': 'phone_already_exists',
      'meta': {'brandCustomerId': selinId},
    }, status: 409);
    await tester.tap(find.byTooltip('Müşteri ekle'));
    await tester.pumpAndSettle();
    await tester.enterText(find.widgetWithText(TextField, 'Ad soyad'), 'Selin');
    await tester.enterText(
      find.widgetWithText(TextField, 'Telefon'),
      '05321234521',
    );
    await tester.pump();
    await tapCentered(tester, find.text('Müşteriyi ekle'));
    await tester.pumpAndSettle();
    expect(find.text(errorMessage('phone_already_exists')), findsOneWidget);
    await tapCentered(tester, find.text('Mevcut kartı aç'));
    await tester.pumpAndSettle();
    expect(find.text('ZİYARET'), findsOneWidget);
  });

  testWidgets('kart: sayaçlar, paket, not, geçmiş ve Ara', (tester) async {
    final h = await openList(tester);
    await tester.tap(find.text('Selin Öztürk'));
    await tester.pumpAndSettle();

    expect(find.text('ZİYARET'), findsOneWidget);
    expect(find.text('12'), findsOneWidget);
    expect(find.text(formatMoney(1486000)), findsOneWidget);
    expect(find.text('Ürün tercihi var'), findsOneWidget);
    expect(find.text('0532 123 45 21'), findsOneWidget);
    expect(find.text('Fön paketi'), findsOneWidget);
    expect(find.text(' / 5 seans kaldı', findRichText: true), findsNothing);
    expect(
      find.textContaining('5 seans kaldı', findRichText: true),
      findsOneWidget,
    );
    expect(find.text('Son kullanım 31 Aralık'), findsOneWidget);
    expect(find.textContaining('Amonyaksız'), findsOneWidget);
    expect(find.textContaining('Sağlık bilgisi yazmayın'), findsOneWidget);
    // Not düzenleme ve KVKK izinleri için uç yok (T-111).
    expect(find.text('Düzenle'), findsNothing);
    expect(find.text('İzinler (KVKK)'), findsNothing);

    await tester.scrollUntilVisible(find.text('Geçmiş'), 200);
    expect(find.text('Boya, Fön'), findsOneWidget);
    expect(find.text('Bugün · Elif · ${formatMoney(144000)}'), findsOneWidget);
    expect(find.text('Saç kesimi'), findsNothing);
    await tapCentered(tester, find.text('Tümü'));
    await tester.pumpAndSettle();
    expect(find.text('Saç kesimi'), findsOneWidget);

    await tester.scrollUntilVisible(find.byTooltip('Ara'), -200);
    await tester.tap(find.byTooltip('Ara'));
    await tester.pumpAndSettle();
    // Numara yanıtta var (customer.phone.view): açma ucu çağrılmaz.
    expect(h.api.to('POST', '$selinPath/reveal-phone'), isEmpty);
    expect(h.launcher.opened.single, Uri(scheme: 'tel', path: '+905321234521'));
  });

  testWidgets('kart: Randevu ver müşteri seçili açar', (tester) async {
    final h = await openList(tester);
    h.api
      ..reply('GET', '/api/v1/manage/services', servicesJson())
      ..reply('GET', '/api/v1/manage/staff', staffJson());
    await tester.tap(find.text('Selin Öztürk'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Randevu ver'));
    await tester.pumpAndSettle();
    expect(find.text('Yeni randevu'), findsOneWidget);
    expect(find.text('Selin Öztürk'), findsOneWidget);
    expect(find.text('0532 *** ** 21 · 12 ziyaret'), findsOneWidget);
  });

  testWidgets('kart: hata ve Tekrar dene', (tester) async {
    var fail = true;
    final h = await openList(tester);
    h.api.on(
      'GET',
      selinPath,
      (_) => fail
          ? const FakeResponse(500, {'code': 'try_again'})
          : FakeResponse(200, selinJson()),
    );
    await tester.tap(find.text('Selin Öztürk'));
    await tester.pumpAndSettle();
    expect(find.text('Müşteri kartı yüklenemedi'), findsOneWidget);
    fail = false;
    await tester.tap(find.text('Tekrar dene'));
    await tester.pumpAndSettle();
    expect(find.text('ZİYARET'), findsOneWidget);
  });

  testWidgets('MusteriKartiYeni: ilk ziyaret, geçmiş boş, sayaç yok', (
    tester,
  ) async {
    const denizPath = '/api/v1/manage/customers/$denizId';
    final h = await openList(
      tester,
      list: (_) => FakeResponse(200, {
        'items': [denizJson()],
        'nextCursor': null,
      }),
    );
    h.api
      ..reply('GET', denizPath, denizJson())
      ..reply('GET', '$denizPath/appointments', denizAppointmentsJson())
      ..reply('GET', '$denizPath/packages', emptyPageJson);
    await tester.tap(find.text('Deniz Acar'));
    await tester.pumpAndSettle();

    expect(find.text('Yeni müşteri'), findsOneWidget);
    expect(find.text('0532 *** ** 18'), findsOneWidget);
    expect(find.text('İlk ziyareti Bugün 12:00'), findsOneWidget);
    expect(find.text('Saç kesimi · Can'), findsOneWidget);
    expect(find.text('ZİYARET'), findsNothing);
    expect(
      find.textContaining('Henüz tamamlanan randevusu yok.'),
      findsOneWidget,
    );
    expect(find.textContaining('Sağlık bilgisi yazmayın'), findsOneWidget);
  });

  testWidgets('personel: telefon maskeli, arama çalışır, Ara açma ucuyla', (
    tester,
  ) async {
    final h = await openList(tester, session: staffSessionJson);
    h.api.reply('POST', '$selinPath/reveal-phone', {'phone': '+905321234521'});
    await search(tester, '4521');
    expect(
      h.api.to('GET', listPath).last.options.queryParameters['q'],
      '4521',
    );
    await tester.tap(find.text('Selin Öztürk'));
    await tester.pumpAndSettle();

    expect(find.text('0532 *** ** 21'), findsOneWidget);
    expect(find.text('0532 123 45 21'), findsNothing);
    await tester.tap(find.byTooltip('SMS'));
    await tester.pumpAndSettle();
    final reveal = h.api.to('POST', '$selinPath/reveal-phone').single;
    expect(reveal.body, {'purpose': 'Sms'});
    expect(h.launcher.opened.single, Uri(scheme: 'sms', path: '+905321234521'));
    // Açılan numara ekrana yazılmaz.
    expect(find.text('0532 123 45 21'), findsNothing);
  });
}
