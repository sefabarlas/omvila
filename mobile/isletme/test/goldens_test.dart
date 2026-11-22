import 'package:flutter_riverpod/misc.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_isletme/app/providers.dart';
import 'package:omvila_isletme/features/auth/data/auth_controller.dart';
import 'package:omvila_isletme/features/auth/presentation/login_code_screen.dart';
import 'package:omvila_isletme/features/auth/presentation/login_screen.dart';
import 'package:omvila_isletme/features/auth/presentation/splash_screen.dart';
import 'package:omvila_isletme/features/push/data/push_prompt_controller.dart';
import 'package:omvila_isletme/features/push/data/push_service.dart';
import 'package:omvila_isletme/features/push/presentation/notification_permission_screen.dart';
import 'package:omvila_isletme/features/shell/presentation/app_shell.dart';
import 'package:omvila_isletme/features/today/data/today_repository.dart';
import 'package:omvila_isletme/features/today/presentation/today_screen.dart';
import 'package:omvila_ui/omvila_ui.dart';

import 'helpers/fixtures.dart';
import 'helpers/screen_golden.dart';

// Kural: dilim-05 KK11 — ana ekranlar açık/koyu ve 1.3× yazı ölçeğinde
// golden (design/ekranlar/isletme-uygulamasi/ ile karşılaştırma için).
void main() {
  final clock = FixedClock.parse('2026-11-25T09:10:00Z');

  List<Override> todayOverrides(
    Map<String, Object?> json, {
    PushPermission permission = PushPermission.granted,
  }) => [
    clockProvider.overrideWithValue(clock),
    authControllerProvider.overrideWith(
      () => FixedAuth(session(ownerSessionJson)),
    ),
    pushPromptProvider.overrideWith(
      () => FixedPushPrompt(permission: permission),
    ),
    todayProvider.overrideWith(
      (ref) => TodaySnapshot(today: today(json), fetchedAt: clock.now()),
    ),
    bookingUrlProvider.overrideWith(
      (ref) => bookingPageJson['url']! as String,
    ),
  ];

  screenGoldenTest(
    'Açılış',
    fileName: 'acilis',
    overrides: [authControllerProvider.overrideWith(() => FixedAuth(null))],
    builder: (_) => const SplashScreen(),
  );

  screenGoldenTest(
    'Giriş',
    fileName: 'giris',
    builder: (_) => const LoginScreen(),
  );

  screenGoldenTest(
    'Giriş kodu',
    fileName: 'giris_kod',
    builder: (_) => LoginCodeScreen(
      step: OtpStep(
        phoneE164: '+905324187705',
        challenge: OtpChallengeResponse.fromJson(otpChallengeJson),
      ),
    ),
  );

  screenGoldenTest(
    'Bildirim izni',
    fileName: 'bildirim_izni',
    builder: (_) => const NotificationPermissionScreen(),
  );

  screenGoldenTest(
    'Bugün',
    fileName: 'bugun',
    overrides: todayOverrides(todayJson()),
    builder: (_) => const AppShell(location: '/bugun', child: TodayScreen()),
  );

  screenGoldenTest(
    'Bugün boş',
    fileName: 'bugun_bos',
    overrides: todayOverrides(emptyTodayJson()),
    builder: (_) => const AppShell(location: '/bugun', child: TodayScreen()),
  );

  screenGoldenTest(
    'Bugün bildirim kapalı',
    fileName: 'bugun_bildirim_kapali',
    overrides: todayOverrides(
      todayJson(),
      permission: PushPermission.denied,
    ),
    builder: (_) => const AppShell(location: '/bugun', child: TodayScreen()),
  );
}
