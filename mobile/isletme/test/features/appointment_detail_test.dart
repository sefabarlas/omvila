import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:omvila_isletme/features/push/data/push_service.dart';
import 'package:omvila_ui/omvila_ui.dart';

import '../helpers/appointment_fixtures.dart';
import '../helpers/fake_api.dart';
import '../helpers/fixtures.dart';

// Kural: dilim-05 KK5 (OdemeAl: kapora havale düşümü, yöntem çipleri, ikinci
// ödeme, payment_total_mismatch; kart formu yok), KK10 (personel: telefon
// maskeli, arama manageRevealCustomerPhone ile çalışır; kapora, onay ve
// tamamla eylemleri yok), Dilim 4 (KaporaOnay: If-Match + Idempotency-Key,
// hatırlatma bir kez), roller-yetkiler.md (offerToWaitlist yalnız
// waitlist.manage), mobile/isletme/AGENTS.md § Push (bildirimden ayrıntı).
void main() {
  String path(String id, [String action = '']) =>
      '/api/v1/manage/appointments/$id$action';

  Future<Harness> openDetail(
    WidgetTester tester,
    String id, {
    Map<String, Object?>? session,
    FutureOr<FakeResponse> Function()? detail,
    Map<String, Object?>? json,
    bool settle = true,
  }) async {
    final h = Harness()..signIn(session: session);
    h.api
      ..reply('GET', '/api/v1/manage/today', todayJson())
      ..reply(
        'GET',
        '/api/v1/manage/customers/$customerId/packages',
        {'items': <Object>[], 'nextCursor': null},
      )
      ..on(
        'GET',
        path(id),
        (_) => detail == null ? FakeResponse(200, json) : detail(),
      );
    await h.pumpApp(tester);
    h.push.simulateOpen(AppointmentTarget(id));
    if (settle) {
      await tester.pumpAndSettle();
    } else {
      for (var i = 0; i < 5; i++) {
        await tester.pump(const Duration(milliseconds: 100));
      }
    }

    return h;
  }

  testWidgets('yükleniyor', (tester) async {
    final pending = Completer<FakeResponse>();
    await openDetail(
      tester,
      checkedInId,
      detail: () => pending.future,
      settle: false,
    );
    expect(find.byType(CircularProgressIndicator), findsOneWidget);
    pending.complete(FakeResponse(200, checkedInJson()));
    await tester.pumpAndSettle();
    expect(find.text('Selin Öztürk'), findsOneWidget);
  });

  testWidgets('hata: mesaj ve Tekrar dene', (tester) async {
    var fail = true;
    await openDetail(
      tester,
      checkedInId,
      detail: () => fail
          ? const FakeResponse(404, {'code': 'not_found'})
          : FakeResponse(200, checkedInJson()),
    );
    expect(find.text('Randevu yüklenemedi'), findsOneWidget);
    expect(find.text(errorMessage('not_found')), findsOneWidget);
    fail = false;
    await tester.tap(find.text('Tekrar dene'));
    await tester.pumpAndSettle();
    expect(find.text('Selin Öztürk'), findsOneWidget);
  });

  testWidgets('dolu: rozet, hizmetler, toplam/kapora/kalan, not', (
    tester,
  ) async {
    await openDetail(tester, checkedInId, json: checkedInJson());
    expect(find.text('ÇAR 25 KASIM · ONLİNE'), findsOneWidget);
    expect(find.text('Geldi · 10:34'), findsOneWidget);
    expect(find.text('10:30–12:00'), findsOneWidget);
    expect(find.text('Boya ve Fön · Elif · 1 sa 30 dk'), findsOneWidget);
    expect(find.text('12. ziyaret · son 21 Ekim'), findsOneWidget);
    expect(find.text('10:30–11:45 · Elif'), findsOneWidget);
    expect(find.text(formatMoney(125000)), findsOneWidget);
    expect(find.text(formatMoney(144000)), findsOneWidget);
    expect(find.text('200 ₺ · havale ile alındı'), findsOneWidget);
    expect(find.text(formatMoney(124000)), findsOneWidget);
    expect(
      find.text('Geçen sefer 7.1 kullanıldı, aynısı isteniyor.'),
      findsOneWidget,
    );
    expect(find.text('Tamamla ve ödeme al'), findsOneWidget);
    expect(find.byTooltip('Diğer işlemler'), findsOneWidget);
  });

  testWidgets('Ara: numara yanıttaysa doğrudan tel: açılır', (tester) async {
    final h = await openDetail(
      tester,
      checkedInId,
      json: checkedInJson(phone: '+905321234521'),
    );
    await tester.tap(find.byTooltip('Ara'));
    await tester.pumpAndSettle();
    expect(h.launcher.opened, [Uri.parse('tel:+905321234521')]);
    expect(
      h.api.to('POST', '/api/v1/manage/customers/$customerId/reveal-phone'),
      isEmpty,
    );
  });

  testWidgets(
    'personel: telefon maskeli, Ara/SMS numarayı açar, yönetim eylemleri yok',
    (tester) async {
      final h = await openDetail(
        tester,
        checkedInId,
        session: staffSessionJson,
        json: checkedInJson(),
      );
      h.api.on(
        'POST',
        '/api/v1/manage/customers/$customerId/reveal-phone',
        (_) => const FakeResponse(200, {'phone': '+905321234521'}),
      );
      expect(find.textContaining('1234521'), findsNothing);
      expect(find.text('0532 123 45 21'), findsNothing);
      expect(find.text('Tamamla ve ödeme al'), findsNothing);

      await tester.tap(find.byTooltip('Ara'));
      await tester.pumpAndSettle();
      await tester.tap(find.byTooltip('SMS'));
      await tester.pumpAndSettle();

      final reveals = h.api.to(
        'POST',
        '/api/v1/manage/customers/$customerId/reveal-phone',
      );
      expect(
        [for (final r in reveals) r.body],
        [
          {'purpose': 'Call'},
          {'purpose': 'Sms'},
        ],
      );
      expect(h.launcher.opened, [
        Uri.parse('tel:+905321234521'),
        Uri.parse('sms:+905321234521'),
      ]);
      // Numara ekrana yazılmaz.
      expect(find.textContaining('1234521'), findsNothing);
    },
  );

  testWidgets('personel: kapora bekleyen randevuda kapora eylemi yok', (
    tester,
  ) async {
    await openDetail(
      tester,
      depositId,
      session: staffSessionJson,
      json: depositJson(),
    );
    expect(find.text('150 ₺ · havale bildirildi'), findsOneWidget);
    expect(find.text('Kaporayı kontrol et'), findsNothing);
    expect(find.text('Onayla'), findsNothing);
  });

  testWidgets('Geldi: If-Match ile check-in', (tester) async {
    final h = await openDetail(tester, confirmedId, json: confirmedJson());
    h.api.reply('POST', path(confirmedId, '/check-in'), confirmedJson());
    await tester.tap(find.text('Geldi'));
    await tester.pumpAndSettle();
    final post = h.api.to('POST', path(confirmedId, '/check-in')).single;
    expect(post.header('If-Match'), 'v5');
    expect(find.text('Müşteri geldi olarak işaretlendi.'), findsOneWidget);
  });

  group('OdemeAl', () {
    Future<Harness> openCheckout(
      WidgetTester tester, {
      Map<String, Object?>? packages,
    }) async {
      final h = await openDetail(tester, checkedInId, json: checkedInJson());
      if (packages != null) {
        h.api.reply(
          'GET',
          '/api/v1/manage/customers/$customerId/packages',
          packages,
        );
      }

      await tester.tap(find.text('Tamamla ve ödeme al'));
      await tester.pumpAndSettle();
      return h;
    }

    testWidgets('kalemler, kapora (havale) düşümü, kart formu yok', (
      tester,
    ) async {
      await openCheckout(tester);
      expect(find.text('Boya · Elif'), findsOneWidget);
      expect(find.text('Fön · Elif'), findsOneWidget);
      expect(find.text('Kapora (havale)'), findsOneWidget);
      expect(find.text('Kapora (online)'), findsNothing);
      expect(find.text('−200 ₺'), findsOneWidget);
      expect(find.text('Alınacak'), findsOneWidget);
      for (final m in ['Nakit', 'Kart', 'Havale']) {
        expect(find.text(m), findsOneWidget);
      }

      expect(find.text('Paketten'), findsNothing);
      expect(find.text('1.240 ₺ al ve tamamla'), findsOneWidget);
      // OMVILA ödeme almaz: kart numarası alanı yok.
      expect(find.byType(TextField), findsNothing);
    });

    testWidgets('Kart ile tamamla: If-Match + Idempotency-Key', (
      tester,
    ) async {
      final h = await openCheckout(tester);
      h.api.reply('POST', path(checkedInId, '/complete'), checkedInJson());
      await tester.tap(find.text('Kart'));
      await tester.pump();
      await tester.tap(find.text('1.240 ₺ al ve tamamla'));
      await tester.pumpAndSettle();
      final post = h.api.to('POST', path(checkedInId, '/complete')).single;
      expect(post.header('If-Match'), 'v3');
      expect(post.header('Idempotency-Key'), isNotEmpty);
      expect(post.body, {
        'payments': [
          {
            'method': 'Card',
            'amount': {'amountMinor': 124000, 'currency': 'TRY'},
          },
        ],
      });
      expect(
        find.text('Randevu tamamlandı, ödeme kasaya işlendi.'),
        findsOneWidget,
      );
    });

    testWidgets('İkinci ödeme ve payment_total_mismatch', (tester) async {
      final h = await openCheckout(tester);
      h.api.reply(
        'POST',
        path(checkedInId, '/complete'),
        {
          'code': 'payment_total_mismatch',
          'meta': {'expectedAmountMinor': 100000},
        },
        status: 409,
      );
      await tester.tap(find.text('İkinci ödeme ekle'));
      await tester.pumpAndSettle();
      final fields = find.byType(TextField);
      expect(fields, findsNWidgets(2));
      await tester.enterText(fields.first, '1000');
      await tester.enterText(fields.last, '240');
      await tester.tap(find.text('1.240 ₺ al ve tamamla'));
      await tester.pumpAndSettle();
      final first = h.api.to('POST', path(checkedInId, '/complete')).single;
      expect(first.body, {
        'payments': [
          {
            'method': 'Cash',
            'amount': {'amountMinor': 100000, 'currency': 'TRY'},
          },
          {
            'method': 'Card',
            'amount': {'amountMinor': 24000, 'currency': 'TRY'},
          },
        ],
      });
      expect(
        find.text('Alınacak tutar 1.000 ₺ olmalı. Tutarları kontrol edin.'),
        findsOneWidget,
      );

      // Toplam tutmazsa istek gitmez.
      await tester.enterText(fields.last, '100');
      await tester.tap(find.text('1.240 ₺ al ve tamamla'));
      await tester.pumpAndSettle();
      expect(h.api.to('POST', path(checkedInId, '/complete')), hasLength(1));
      expect(find.textContaining('aynı olmalı'), findsOneWidget);
    });

    testWidgets('Paketten: eşleşen kalem düşülür', (tester) async {
      final h = await openCheckout(tester, packages: fonPackagePageJson());
      h.api.reply('POST', path(checkedInId, '/complete'), checkedInJson());
      await tester.tap(find.text('Paketten'));
      await tester.pumpAndSettle();
      expect(find.text('Paketten (Fön paketi)'), findsOneWidget);
      expect(find.text('1.050 ₺ al ve tamamla'), findsOneWidget);
      await tester.tap(find.text('1.050 ₺ al ve tamamla'));
      await tester.pumpAndSettle();
      final post = h.api.to('POST', path(checkedInId, '/complete')).single;
      expect(post.body, {
        'payments': [
          {
            'method': 'Cash',
            'amount': {'amountMinor': 105000, 'currency': 'TRY'},
          },
        ],
        'packageRedemptions': [
          {
            'appointmentItemId': '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5c23',
            'customerPackageItemId': '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f8c01',
          },
        ],
      });
    });
  });

  group('KaporaOnay', () {
    testWidgets('bilgiler ve Kapora geldi', (tester) async {
      final h = await openDetail(tester, depositId, json: depositJson());
      h.api.reply(
        'POST',
        path(depositId, '/deposit/mark-received'),
        confirmedJson(),
      );
      await tester.tap(find.text('Kaporayı kontrol et'));
      await tester.pumpAndSettle();
      expect(find.text('Gizem Arslan · 14:30'), findsOneWidget);
      expect(find.text('Fön · Mert Demir'), findsOneWidget);
      expect(find.text('150 ₺'), findsOneWidget);
      expect(find.text('OMV-7K2X'), findsOneWidget);
      expect(find.text('“Havaleyi yaptım” · 11:52'), findsOneWidget);
      // Tutma 10:30Z, şimdi 09:10Z.
      expect(find.text('1 sa 20 dk'), findsOneWidget);

      await tester.tap(find.text('Kapora geldi'));
      await tester.pumpAndSettle();
      final post = h.api
          .to('POST', path(depositId, '/deposit/mark-received'))
          .single;
      expect(post.header('If-Match'), 'W/"8839"');
      expect(post.header('Idempotency-Key'), isNotEmpty);
      expect(find.text('Kapora işaretlendi, randevu onaylandı.'), findsOne);
    });

    testWidgets('hatırlatma zaten gönderildiyse buton pasif', (tester) async {
      final h = await openDetail(tester, depositId, json: depositJson());
      h.api.reply(
        'POST',
        path(depositId, '/deposit/remind'),
        {'code': 'deposit_reminder_already_sent'},
        status: 409,
      );
      await tester.tap(find.text('Kaporayı kontrol et'));
      await tester.pumpAndSettle();
      await tester.tap(find.text('Henüz gelmedi, hatırlat'));
      await tester.pumpAndSettle();
      expect(
        find.text(errorMessage('deposit_reminder_already_sent')),
        findsOneWidget,
      );
      expect(find.text('Hatırlatma gönderildi'), findsOneWidget);
    });
  });

  group('RandevuIslem', () {
    testWidgets('seçenekler; uç olmayanlar gösterilmez', (tester) async {
      await openDetail(tester, confirmedId, json: confirmedJson());
      await tester.tap(find.byTooltip('Diğer işlemler'));
      await tester.pumpAndSettle();
      expect(find.text('Derya Şahin · 13:00'), findsOneWidget);
      expect(find.text('Saati veya personeli değiştir'), findsOneWidget);
      expect(find.text('Randevuyu iptal et'), findsOneWidget);
      expect(find.text('Hizmet ekle'), findsNothing);
      expect(find.text('Müşteriye SMS gönder'), findsNothing);
    });

    testWidgets('iptal: bekleme listesi anahtarı (sahip)', (tester) async {
      final h = await openDetail(tester, confirmedId, json: confirmedJson());
      h.api.reply('POST', path(confirmedId, '/cancel'), confirmedJson());
      await tester.tap(find.byTooltip('Diğer işlemler'));
      await tester.pumpAndSettle();
      await tester.tap(find.text('Randevuyu iptal et'));
      await tester.pumpAndSettle();
      expect(find.text('Saati bekleme listesine aç'), findsOneWidget);
      await tester.tap(find.text('Saati bekleme listesine aç'));
      await tester.pump();
      await tester.tap(find.widgetWithText(OmvilaButon, 'Randevuyu iptal et'));
      await tester.pumpAndSettle();
      final post = h.api.to('POST', path(confirmedId, '/cancel')).single;
      expect(post.header('If-Match'), 'v5');
      expect(post.body, {
        'reason': 'CustomerRequested',
        'note': null,
        'notifyCustomer': true,
        'offerToWaitlist': false,
      });
      expect(find.text('Randevu iptal edildi.'), findsOneWidget);
    });

    testWidgets('iptal: personelde bekleme listesi anahtarı yok', (
      tester,
    ) async {
      await openDetail(
        tester,
        confirmedId,
        session: staffSessionJson,
        json: confirmedJson(),
      );
      await tester.tap(find.byTooltip('Diğer işlemler'));
      await tester.pumpAndSettle();
      await tester.tap(find.text('Randevuyu iptal et'));
      await tester.pumpAndSettle();
      expect(find.text('Müşteriye bildir'), findsOneWidget);
      expect(find.text('Saati bekleme listesine aç'), findsNothing);
    });

    testWidgets('gelmiş randevuyu personel iptal edemez', (tester) async {
      await openDetail(
        tester,
        checkedInId,
        session: staffSessionJson,
        json: checkedInJson(),
      );
      expect(find.byTooltip('Diğer işlemler'), findsNothing);
    });
  });
}
