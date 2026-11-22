import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:omvila_isletme/features/push/data/push_service.dart';
import 'package:omvila_ui/omvila_ui.dart';

import '../helpers/fake_api.dart';
import '../helpers/fixtures.dart';

// Kural: ürün kuralı 7 (salon girişi yalnız telefon + SMS kodu),
// kodlama-flutter.md § API (yenileme jetonu güvenli depoda, mobil istemci
// jetonu gövdede alır), § Bildirimler (izin kendi ekranımızla; sistem istemi
// yalnız "Bildirimlere izin ver"e basınca).
void main() {
  const otpRequest = '/api/v1/manage/auth/otp/request';
  const otpVerify = '/api/v1/manage/auth/otp/verify';

  Harness signedOut() {
    final h = Harness()
      ..push = FakePushService()
      ..api.reply('POST', otpRequest, otpChallengeJson, status: 202);
    return h;
  }

  testWidgets('giriş → kod → bildirim izni → Bugün', (tester) async {
    final h = signedOut();
    h.api
      ..reply('POST', otpVerify, signedInJson)
      ..reply('GET', '/api/v1/manage/session', ownerSessionJson)
      ..reply('GET', '/api/v1/manage/today', todayJson());
    await h.pumpApp(tester);

    expect(find.text('İşim\ncebimde.'), findsOneWidget);
    expect(
      find.text(
        'Hesabınız yok mu? İşletme sahibiniz sizi personel olarak eklemeli.',
      ),
      findsOneWidget,
    );

    await tester.enterText(find.byType(TextField), '532 418 77 05');
    await tester.tap(find.text('Kod gönder'));
    await tester.pumpAndSettle();

    final request = h.api.to('POST', otpRequest).single;
    expect(request.body, {'phone': '+905324187705', 'purpose': 'Login'});
    expect(request.header('X-Client-Type'), 'mobile');
    expect(
      find.text('0532 *** ** 05 numarasına 6 haneli kod gönderdik.'),
      findsOneWidget,
    );
    expect(
      find.text('Kod gelmedi mi? 1:00 sonra tekrar gönderebilirsiniz.'),
      findsOneWidget,
    );

    await tester.enterText(find.byType(EditableText).last, '482193');
    await tester.pumpAndSettle();

    final verify = h.api.to('POST', otpVerify).single;
    expect(verify.body, containsPair('code', '482193'));
    expect(h.store.values['auth.refreshToken'], 'rt-1');
    expect(find.text('Talepleri anında görün'), findsOneWidget);

    await tester.tap(find.text('Bildirimlere izin ver'));
    await tester.pumpAndSettle();
    expect(h.store.values['push.asked'], '1');
    expect(find.text('Derya Şahin'), findsOneWidget);
    expect(find.text('Bildirimleri aç'), findsNothing);
  });

  testWidgets('"Şimdi değil" Bugün’de şerit bırakır', (tester) async {
    final h = Harness()
      ..push = FakePushService()
      ..signIn(promptAsked: false);
    h.api.reply('GET', '/api/v1/manage/today', todayJson());
    await h.pumpApp(tester);

    expect(find.text('Talepleri anında görün'), findsOneWidget);
    await tester.tap(find.text('Şimdi değil'));
    await tester.pumpAndSettle();
    expect(find.text('Bildirimleri aç'), findsOneWidget);
  });

  testWidgets('geçersiz numara istek göndermez', (tester) async {
    final h = signedOut();
    await h.pumpApp(tester);
    await tester.enterText(find.byType(TextField), '212 444 55');
    await tester.tap(find.text('Kod gönder'));
    await tester.pumpAndSettle();
    expect(
      find.text('Geçerli bir cep telefonu numarası girin.'),
      findsOneWidget,
    );
    expect(h.api.to('POST', otpRequest), isEmpty);
  });

  testWidgets('kayıtlı hesap yoksa açıklama', (tester) async {
    final h = signedOut();
    h.api.reply('POST', otpVerify, {
      'result': 'SignupRequired',
      'signupToken': 'st',
    });
    await h.pumpApp(tester);
    await tester.enterText(find.byType(TextField), '5324187705');
    await tester.tap(find.text('Kod gönder'));
    await tester.pumpAndSettle();
    await tester.enterText(find.byType(EditableText).last, '482193');
    await tester.pumpAndSettle();
    expect(
      find.textContaining('Bu numarayla kayıtlı bir işletme hesabı yok.'),
      findsOneWidget,
    );
    expect(h.store.values['auth.refreshToken'], isNull);
  });

  testWidgets('yanlış kod sunucu mesajıyla', (tester) async {
    final h = signedOut();
    h.api.reply('POST', otpVerify, {'code': 'otp_invalid'}, status: 422);
    await h.pumpApp(tester);
    await tester.enterText(find.byType(TextField), '5324187705');
    await tester.tap(find.text('Kod gönder'));
    await tester.pumpAndSettle();
    await tester.enterText(find.byType(EditableText).last, '000000');
    await tester.pumpAndSettle();
    expect(find.text(errorMessage('otp_invalid')), findsOneWidget);
  });

  testWidgets('geri sayım bitince kod yeniden gönderilir', (tester) async {
    final h = signedOut();
    await h.pumpApp(tester);
    await tester.enterText(find.byType(TextField), '5324187705');
    await tester.tap(find.text('Kod gönder'));
    await tester.pumpAndSettle();
    await tester.pump(const Duration(seconds: 61));
    await tester.tap(find.text('Kodu tekrar gönder'));
    await tester.pumpAndSettle();
    expect(h.api.to('POST', otpRequest), hasLength(2));
  });

  testWidgets('açılışta bağlantı yoksa Tekrar dene', (tester) async {
    final h = Harness()..signIn();
    h.api.on('POST', '/api/v1/manage/auth/refresh', (_) {
      throw StateError('bağlantı yok');
    });
    await h.pumpApp(tester);
    expect(find.text('Tekrar dene'), findsOneWidget);
    expect(h.store.values['auth.refreshToken'], 'rt-1');
  });

  testWidgets('sunucu oturumu reddederse giriş', (tester) async {
    final h = Harness()..signIn();
    h.api.reply('POST', '/api/v1/manage/auth/refresh', {
      'code': 'unauthorized',
    }, status: 401);
    await h.pumpApp(tester);
    expect(find.text('Kod gönder'), findsOneWidget);
    expect(h.store.values['auth.refreshToken'], isNull);
  });

  testWidgets('Profil → Çıkış yap', (tester) async {
    final h = Harness()..signIn();
    h.api
      ..reply('GET', '/api/v1/manage/today', todayJson())
      ..reply('POST', '/api/v1/manage/auth/logout', null, status: 204);
    await h.pumpApp(tester);
    await tester.tap(find.bySemanticsLabel('Profil'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Çıkış yap'));
    await tester.pumpAndSettle();
    expect(find.text('Kod gönder'), findsOneWidget);
    expect(h.store.values['auth.refreshToken'], isNull);
  });
}
