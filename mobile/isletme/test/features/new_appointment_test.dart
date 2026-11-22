import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:omvila_ui/omvila_ui.dart';

import '../helpers/appointment_fixtures.dart';
import '../helpers/fake_api.dart';
import '../helpers/fixtures.dart';
import 'today_screen_test.dart' show tapCentered;

// Kural: manageCreateAppointment (panel/telefon kanalı onaylı, kapora yok;
// Idempotency-Key; slot_taken asla geçilmez; çalışma saati dışı onayla
// tekrar), manageSearchSlots (dolu saat üstü çizili; tarih "yyyy-aa-gg"),
// roller-yetkiler.md (personel yalnız kendine randevu ekler), salt okunur
// abonelikte yeni randevu pasif (mobile/isletme/AGENTS.md).
void main() {
  const slotsPath = '/api/v1/manage/branches/$branchId/availability/slots';
  const createPath = '/api/v1/manage/appointments';

  Future<Harness> openForm(
    WidgetTester tester, {
    Map<String, Object?>? session,
    FutureOr<FakeResponse> Function()? services,
    bool settle = true,
  }) async {
    final h = Harness()..signIn(session: session);
    h.api
      ..reply('GET', '/api/v1/manage/today', todayJson())
      ..on(
        'GET',
        '/api/v1/manage/services',
        (_) =>
            services == null ? FakeResponse(200, servicesJson()) : services(),
      )
      ..reply('GET', '/api/v1/manage/staff', staffJson())
      ..reply('GET', '/api/v1/manage/customers', customersJson())
      ..on('POST', slotsPath, (r) {
        final items = (r.body! as Map)['items'] as List;
        final staffId = (items.first as Map)['staffId'] as String?;
        return FakeResponse(200, slotsJson(staffId: staffId ?? elifId));
      });
    await h.pumpApp(tester);
    await tester.tap(find.text('Randevu'));
    if (settle) {
      await tester.pumpAndSettle();
    } else {
      for (var i = 0; i < 5; i++) {
        await tester.pump(const Duration(milliseconds: 100));
      }
    }

    return h;
  }

  Future<void> fill(WidgetTester tester) async {
    await tester.enterText(find.byType(TextField).first, 'Der');
    await tester.pump(const Duration(milliseconds: 400));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Derya Şahin'));
    await tester.pumpAndSettle();
    await tapCentered(tester, find.text('Saç kesimi · 45 dk'));
    await tester.pumpAndSettle();
    await tapCentered(tester, find.bySemanticsLabel('Elif'));
    await tester.pumpAndSettle();
    final thursday = find.bySemanticsLabel('Perşembe 26 Kasım');
    await tapCentered(tester, thursday);
    await tester.pumpAndSettle();
    await tapCentered(tester, find.bySemanticsLabel('14:00'));
    await tester.pumpAndSettle();
  }

  testWidgets('yükleniyor', (tester) async {
    final pending = Completer<FakeResponse>();
    await openForm(tester, services: () => pending.future, settle: false);
    expect(find.text('Yeni randevu'), findsOneWidget);
    expect(find.byType(CircularProgressIndicator), findsOneWidget);
    pending.complete(FakeResponse(200, servicesJson()));
    await tester.pumpAndSettle();
    expect(find.text('Nasıl geldi?'), findsOneWidget);
  });

  testWidgets('hata: mesaj ve Tekrar dene', (tester) async {
    var fail = true;
    await openForm(
      tester,
      services: () => fail
          ? const FakeResponse(500, {'code': 'try_again'})
          : FakeResponse(200, servicesJson()),
    );
    expect(find.text('Randevu formu yüklenemedi'), findsOneWidget);
    fail = false;
    await tester.tap(find.text('Tekrar dene'));
    await tester.pumpAndSettle();
    expect(find.text('Nasıl geldi?'), findsOneWidget);
  });

  testWidgets('boş: hizmet yoksa yönlendirme metni, Oluştur pasif', (
    tester,
  ) async {
    await openForm(
      tester,
      services: () => const FakeResponse(200, {'items': <Object>[]}),
    );
    expect(
      find.text('Henüz hizmet yok. Hizmetleri salon panelinden ekleyin.'),
      findsOneWidget,
    );
    final create = tester.widget<OmvilaButon>(
      find.widgetWithText(OmvilaButon, 'Oluştur'),
    );
    expect(create.onPressed, isNull);
  });

  testWidgets('dolu: seçimler, dolu saat, Oluştur gövdesi', (tester) async {
    final h = await openForm(tester);
    h.api
      ..on(
        'POST',
        createPath,
        (_) => FakeResponse(201, {
          'appointment': confirmedJson(),
          'skippedOccurrences': <String>[],
        }),
      )
      ..reply(
        'GET',
        '/api/v1/manage/appointments/$confirmedId',
        confirmedJson(),
      );
    expect(find.text('Telefonla'), findsOneWidget);
    expect(find.text('Kapıdan'), findsOneWidget);
    expect(find.text('Tümü'), findsOneWidget);
    expect(find.text('Fark etmez'), findsOneWidget);

    await fill(tester);
    final slotRequest = h.api.to('POST', slotsPath).last;
    expect((slotRequest.body! as Map)['date'], '2026-11-26');
    expect(
      find.text(
        'Saat · Elif için boş',
        findRichText: true,
        skipOffstage: false,
      ),
      findsOneWidget,
    );
    // Dolu saat üstü çizili ve seçilemez.
    final taken = tester.widget<Text>(find.text('11:30', skipOffstage: false));
    expect(taken.style?.decoration, TextDecoration.lineThrough);
    expect(find.text('Per 26 Kas · 14:00'), findsOneWidget);
    expect(find.text('Saç kesimi · Elif · 450 ₺'), findsOneWidget);

    await tapCentered(tester, find.text('Her hafta'));
    await tester.pump();
    await tester.tap(find.text('Oluştur'));
    await tester.pumpAndSettle();

    final post = h.api.to('POST', createPath).single;
    expect(post.header('Idempotency-Key'), isNotEmpty);
    expect(post.body, {
      'branchId': branchId,
      'brandCustomerId': '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5b13',
      'channel': 'Phone',
      'startUtc': '2026-11-26T11:00:00.000Z',
      'items': [
        {'serviceId': kesimId, 'staffId': elifId},
      ],
      'staffNote': null,
      'recurrence': {'frequency': 'Weekly', 'intervalWeeks': 1, 'endsOn': null},
      'notifyCustomer': true,
    });
    expect(find.text('Randevu oluşturuldu.'), findsOneWidget);
    expect(find.text('Randevu'), findsOneWidget);
  });

  testWidgets('slot_taken: mesaj, saat seçimi sıfırlanır', (tester) async {
    final h = await openForm(tester);
    h.api.on(
      'POST',
      createPath,
      (_) => const FakeResponse(409, {'code': 'slot_taken'}),
    );
    await fill(tester);
    await tester.tap(find.text('Oluştur'));
    await tester.pumpAndSettle();
    expect(find.text(errorMessage('slot_taken')), findsOneWidget);
    expect(find.text('Müşteri, hizmet ve saat seçin'), findsOneWidget);
  });

  testWidgets('çalışma saati dışı: onaylanınca izinle tekrar gönderilir', (
    tester,
  ) async {
    final h = await openForm(tester);
    var calls = 0;
    h.api
      ..on('POST', createPath, (_) {
        calls++;
        return calls == 1
            ? const FakeResponse(409, {'code': 'outside_working_hours'})
            : FakeResponse(201, {
                'appointment': confirmedJson(),
                'skippedOccurrences': <String>[],
              });
      })
      ..reply(
        'GET',
        '/api/v1/manage/appointments/$confirmedId',
        confirmedJson(),
      );
    await fill(tester);
    await tester.tap(find.text('Oluştur'));
    await tester.pumpAndSettle();
    expect(find.text(errorMessage('outside_working_hours')), findsOneWidget);
    await tester.tap(find.text('Yine de oluştur'));
    await tester.pumpAndSettle();
    final posts = h.api.to('POST', createPath);
    expect(posts, hasLength(2));
    expect((posts.last.body! as Map)['allowOutsideWorkingHours'], isTrue);
    expect(
      posts.first.header('Idempotency-Key'),
      isNot(posts.last.header('Idempotency-Key')),
    );
  });

  testWidgets('yeni müşteri: ad ve telefonla', (tester) async {
    final h = await openForm(tester);
    await tester.tap(find.text('Yeni müşteri ekle'));
    await tester.pumpAndSettle();
    await tester.enterText(
      find.widgetWithText(TextField, 'Ad soyad'),
      'Ada Ak',
    );
    await tester.enterText(
      find.widgetWithText(TextField, 'Cep telefonu'),
      '0532 000 00 01',
    );
    await tapCentered(tester, find.text('Saç kesimi · 45 dk'));
    await tester.pumpAndSettle();
    await tapCentered(tester, find.bySemanticsLabel('13:15'));
    await tester.pumpAndSettle();
    h.api.on(
      'POST',
      createPath,
      (_) => const FakeResponse(409, {'code': 'slot_taken'}),
    );
    await tester.tap(find.text('Oluştur'));
    await tester.pumpAndSettle();
    final body = h.api.to('POST', createPath).single.body! as Map;
    expect(body['newCustomer'], {
      'fullName': 'Ada Ak',
      'phone': '+905320000001',
    });
    expect(body.containsKey('brandCustomerId'), isFalse);
  });

  testWidgets('personel: yalnız kendisi, Fark etmez yok', (tester) async {
    await openForm(tester, session: staffSessionJson);
    expect(find.text('Fark etmez'), findsNothing);
    expect(find.bySemanticsLabel('Can'), findsOneWidget);
    expect(find.bySemanticsLabel('Elif'), findsNothing);
  });

  testWidgets('salt okunur abonelik: şerit ve Oluştur pasif', (tester) async {
    await openForm(
      tester,
      session: {
        ...ownerSessionJson,
        'subscription': {'status': 'ReadOnly'},
      },
    );
    expect(find.text(errorMessage('subscription_read_only')), findsOneWidget);
    await fill(tester);
    final create = tester.widget<OmvilaButon>(
      find.widgetWithText(OmvilaButon, 'Oluştur'),
    );
    expect(create.onPressed, isNull);
  });
}
