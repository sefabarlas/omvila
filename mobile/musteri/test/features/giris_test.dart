import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:omvila_musteri/app/config.dart';
import 'package:omvila_musteri/app/router.dart';
import 'package:omvila_musteri/features/auth/presentation/acilis_ekrani.dart';
import 'package:omvila_musteri/features/auth/presentation/giris_ekrani.dart';
import 'package:omvila_musteri/features/auth/presentation/giris_kod_ekrani.dart';
import 'package:omvila_ui/omvila_ui.dart';

import '../helpers/fakes.dart';
import '../helpers/harness.dart';

GirisKodArgs _args(TestDeps deps) =>
    GirisKodArgs(phone: '+905326112467', challenge: deps.auth.challenge);

void main() {
  group('Açılış', () {
    testWidgets('logo ve slogan', (tester) async {
      await tester.pumpWidget(screenApp(const AcilisEkrani(), TestDeps()));
      await tester.pump();
      expect(find.bySemanticsLabel('OMVILA'), findsOneWidget);
      expect(find.text('Randevularım bir arada'), findsOneWidget);
    });
  });

  group('Giriş', () {
    testWidgets('geçersiz numarada "Kod gönder" kapalı', (tester) async {
      await tester.pumpWidget(screenApp(const GirisEkrani(), TestDeps()));
      await tester.enterText(find.byType(TextField), '532 61');
      await tester.pump();
      final button = tester.widget<OmvilaButon>(find.byType(OmvilaButon));
      expect(button.onPressed, isNull);
    });

    testWidgets('kod istenir ve kod ekranına geçilir', (tester) async {
      final deps = TestDeps();
      await tester.pumpWidget(
        screenApp(
          const GirisEkrani(),
          deps,
          routes: [markerRoute(Routes.girisKod)],
        ),
      );
      await tester.enterText(find.byType(TextField), '05326112467');
      await tester.pump();
      expect(find.text('532 611 24 67'), findsOneWidget);
      await tester.tap(find.text('Kod gönder'));
      await tester.pumpAndSettle();
      expect(deps.auth.requested, ['+905326112467']);
      expect(find.text('adres:${Routes.girisKod}'), findsOneWidget);
    });

    testWidgets('sunucu hatası alanın altında', (tester) async {
      final deps = TestDeps()
        ..auth.requestError = const ApiProblem(
          status: 429,
          code: 'otp_resend_too_soon',
        );
      await tester.pumpWidget(screenApp(const GirisEkrani(), deps));
      await tester.enterText(find.byType(TextField), '5326112467');
      await tester.pump();
      await tester.tap(find.text('Kod gönder'));
      await tester.pumpAndSettle();
      expect(find.text('Yeni kod için biraz bekleyin.'), findsOneWidget);
    });

    testWidgets('aydınlatma metni bağlantısı ayardan', (tester) async {
      final deps = TestDeps(
        config: const AppConfig(
          apiUrl: 'http://test',
          privacyNoticeUrl: 'https://omvila.com.tr/aydinlatma',
        ),
      );
      usePhone(tester);
      await tester.pumpWidget(screenApp(const GirisEkrani(), deps));
      await tester.tapOnText(find.textRange.ofSubstring('aydınlatma metnini'));
      await tester.pump();
      expect(deps.external.urls.single.path, '/aydinlatma');
    });
  });

  group('Kod', () {
    testWidgets('maskeli numara ve geri sayım', (tester) async {
      final deps = TestDeps();
      await tester.pumpWidget(
        screenApp(GirisKodEkrani(args: _args(deps)), deps),
      );
      await tester.pump();
      expect(
        find.text('0532 *** ** 67 numarasına 6 haneli kod gönderdik.'),
        findsOneWidget,
      );
      expect(
        find.text('Kod gelmedi mi? 1:00 sonra tekrar gönderebilirsiniz.'),
        findsOneWidget,
      );
    });

    testWidgets('altı hane girilince doğrular', (tester) async {
      final deps = TestDeps();
      await tester.pumpWidget(
        screenApp(GirisKodEkrani(args: _args(deps)), deps),
      );
      await tester.enterText(find.byType(TextField), '482193');
      await tester.pump();
      expect(deps.auth.verified, ['482193']);
      expect(deps.store.values['me.refreshToken'], isNotNull);
    });

    testWidgets('hatalı kodda kalan deneme', (tester) async {
      final deps = TestDeps()
        ..auth.verifyError = const ApiProblem(
          status: 422,
          code: 'otp_invalid',
          meta: {'attemptsLeft': 2},
        );
      await tester.pumpWidget(
        screenApp(GirisKodEkrani(args: _args(deps)), deps),
      );
      await tester.enterText(find.byType(TextField), '111111');
      await tester.pump();
      await tester.pump();
      expect(find.text('Kod hatalı. 2 deneme hakkınız kaldı.'), findsOneWidget);
    });

    testWidgets('Android SMS kodu kendiliğinden doldurulur', (tester) async {
      final deps = TestDeps();
      await tester.pumpWidget(
        screenApp(GirisKodEkrani(args: _args(deps)), deps),
      );
      deps.sms.controller.add('654321');
      await tester.pump();
      expect(deps.auth.verified, ['654321']);
    });

    testWidgets('süre dolunca yeniden gönderilir', (tester) async {
      final clock = MutableClock(DateTime.utc(2026, 11, 24, 17, 30));
      final deps = TestDeps(clock: clock);
      await tester.pumpWidget(
        screenApp(GirisKodEkrani(args: _args(deps)), deps),
      );
      clock.advance(const Duration(seconds: 61));
      await tester.pump(const Duration(seconds: 1));
      await tester.tap(find.text('Kodu tekrar gönder'));
      await tester.pump();
      expect(deps.auth.requested, ['+905326112467']);
      await tester.pump();
      expect(
        find.text('Kod gelmedi mi? 1:00 sonra tekrar gönderebilirsiniz.'),
        findsOneWidget,
      );
    });
  });
}
