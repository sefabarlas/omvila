import 'package:flutter/material.dart';
import 'package:flutter_riverpod/misc.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_isletme/app/providers.dart';
import 'package:omvila_isletme/features/appointments/data/appointment_repository.dart';
import 'package:omvila_isletme/features/appointments/presentation/actions_sheet.dart';
import 'package:omvila_isletme/features/appointments/presentation/appointment_detail_screen.dart';
import 'package:omvila_isletme/features/appointments/presentation/checkout_sheet.dart';
import 'package:omvila_isletme/features/appointments/presentation/deposit_sheet.dart';
import 'package:omvila_isletme/features/auth/data/auth_controller.dart';
import 'package:omvila_isletme/features/auth/presentation/login_code_screen.dart';
import 'package:omvila_isletme/features/auth/presentation/login_screen.dart';
import 'package:omvila_isletme/features/auth/presentation/splash_screen.dart';
import 'package:omvila_isletme/features/calendar/data/calendar_repository.dart';
import 'package:omvila_isletme/features/calendar/presentation/calendar_screen.dart';
import 'package:omvila_isletme/features/new_appointment/data/booking_repository.dart';
import 'package:omvila_isletme/features/new_appointment/presentation/new_appointment_screen.dart';
import 'package:omvila_isletme/features/notifications/data/notification_repository.dart';
import 'package:omvila_isletme/features/push/data/push_prompt_controller.dart';
import 'package:omvila_isletme/features/push/data/push_service.dart';
import 'package:omvila_isletme/features/push/presentation/notification_permission_screen.dart';
import 'package:omvila_isletme/features/shell/presentation/app_shell.dart';
import 'package:omvila_isletme/features/today/data/today_repository.dart';
import 'package:omvila_isletme/features/today/presentation/today_screen.dart';
import 'package:omvila_isletme/shared/ui.dart';
import 'package:omvila_ui/omvila_ui.dart';

import 'helpers/appointment_fixtures.dart';
import 'helpers/fixtures.dart';
import 'helpers/screen_golden.dart';

// Kural: dilim-05 KK11 — ana ekranlar açık/koyu ve 1.3× yazı ölçeğinde
// golden (design/ekranlar/isletme-uygulamasi/ ile karşılaştırma için).
void main() {
  final clock = FixedClock.parse('2026-11-25T09:10:00Z');

  List<Override> todayOverrides(
    Map<String, Object?> json, {
    PushPermission permission = PushPermission.granted,
    Map<String, Object?> sessionJson = ownerSessionJson,
  }) => [
    clockProvider.overrideWithValue(clock),
    authControllerProvider.overrideWith(
      () => FixedAuth(session(sessionJson)),
    ),
    // Sekme çubuğundaki okunmamış rozeti (tasarımda 3).
    unreadNotificationCountProvider.overrideWith((ref) => 3),
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

  screenGoldenTest(
    'Bugün salt okunur',
    fileName: 'bugun_salt_okunur',
    overrides: todayOverrides(
      todayJson(),
      sessionJson: {
        ...ownerSessionJson,
        'subscription': {'status': 'ReadOnly'},
      },
    ),
    builder: (_) => const AppShell(location: '/bugun', child: TodayScreen()),
  );

  List<Override> base({Map<String, Object?> sessionJson = ownerSessionJson}) =>
      [
        clockProvider.overrideWithValue(clock),
        authControllerProvider.overrideWith(
          () => FixedAuth(session(sessionJson)),
        ),
        unreadNotificationCountProvider.overrideWith((ref) => 3),
      ];

  List<Override> calendarOverrides(Map<String, Object?> json) => [
    ...base(),
    calendarProvider.overrideWith(
      (ref, range) => CalendarView.fromJson(json),
    ),
  ];

  screenGoldenTest(
    'Takvim',
    fileName: 'takvim',
    overrides: calendarOverrides(calendarJson()),
    builder: (_) =>
        const AppShell(location: '/takvim', child: CalendarScreen()),
  );

  screenGoldenTest(
    'Takvim boş',
    fileName: 'takvim_bos',
    overrides: calendarOverrides(calendarJson(empty: true)),
    builder: (_) =>
        const AppShell(location: '/takvim', child: CalendarScreen()),
  );

  // Bugün (25 Kasım) kapalı gün.
  Map<String, Object?> closeToday(Map<String, Object?> day) =>
      day['date'] != '2026-11-25'
      ? day
      : {
          ...day,
          'isClosed': true,
          'closureNote': 'Haftalık kapalı gün.',
          'branchIntervals': <Object>[],
        };
  final closedJson = calendarJson();
  closedJson['days'] = (closedJson['days']! as List<Map<String, Object?>>)
      .map(closeToday)
      .toList();
  screenGoldenTest(
    'Takvim kapalı',
    fileName: 'takvim_kapali',
    overrides: calendarOverrides(closedJson),
    builder: (_) =>
        const AppShell(location: '/takvim', child: CalendarScreen()),
  );

  List<Override> detailOverrides(Map<String, Object?> json) => [
    ...base(),
    appointmentProvider.overrideWith(
      (ref, id) => AppointmentDetail.fromJson(json),
    ),
    customerPackagesProvider.overrideWith(
      (ref, id) => CustomerPackagePage.fromJson(fonPackagePageJson()).items,
    ),
  ];

  screenGoldenTest(
    'Randevu ayrıntısı',
    fileName: 'randevu_detay',
    overrides: detailOverrides(checkedInJson()),
    builder: (_) => const AppointmentDetailScreen(appointmentId: checkedInId),
  );

  Widget sheet(BuildContext context, Widget child) {
    final r = context.omvilaRenkler;
    return ColoredBox(
      color: r.ustKatman,
      child: Align(
        alignment: Alignment.bottomCenter,
        child: Material(
          color: r.zeminYukseltilmis,
          shape: const RoundedRectangleBorder(
            borderRadius: BorderRadius.vertical(
              top: Radius.circular(OmvilaKose.koseXl),
            ),
          ),
          child: SheetBody(child: child),
        ),
      ),
    );
  }

  screenGoldenTest(
    'Randevu işlemleri',
    fileName: 'randevu_islem',
    overrides: base(),
    builder: (context) => sheet(
      context,
      ActionsSheet(
        appointment: AppointmentDetail.fromJson(checkedInJson()),
        actions: AppointmentAction.values,
      ),
    ),
  );

  screenGoldenTest(
    'Ödeme al',
    fileName: 'odeme_al',
    overrides: detailOverrides(checkedInJson()),
    builder: (context) => sheet(
      context,
      CheckoutSheet(appointment: AppointmentDetail.fromJson(checkedInJson())),
    ),
  );

  screenGoldenTest(
    'Kapora onayı',
    fileName: 'kapora_onay',
    overrides: base(),
    builder: (context) => sheet(
      context,
      DepositSheet(appointment: AppointmentDetail.fromJson(depositJson())),
    ),
  );

  screenGoldenTest(
    'Yeni randevu',
    fileName: 'yeni_randevu',
    overrides: [
      ...base(),
      servicesProvider.overrideWith(
        (ref, id) => [
          for (final s in servicesJson()['items']! as List<Object?>)
            Service.fromJson(s! as Map<String, dynamic>),
        ],
      ),
      staffProvider.overrideWith(
        (ref, id) => [
          for (final s in staffJson()['items']! as List<Object?>)
            StaffSummary.fromJson(s! as Map<String, dynamic>),
        ],
      ),
    ],
    builder: (_) => const NewAppointmentScreen(initialDate: '2026-11-26'),
  );
}
