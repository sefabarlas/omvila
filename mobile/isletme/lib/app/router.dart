import 'package:flutter/foundation.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_isletme/features/appointments/presentation/appointment_detail_screen.dart';
import 'package:omvila_isletme/features/auth/data/auth_controller.dart';
import 'package:omvila_isletme/features/auth/presentation/login_code_screen.dart';
import 'package:omvila_isletme/features/auth/presentation/login_screen.dart';
import 'package:omvila_isletme/features/auth/presentation/splash_screen.dart';
import 'package:omvila_isletme/features/calendar/presentation/calendar_screen.dart';
import 'package:omvila_isletme/features/customers/presentation/customer_card_screen.dart';
import 'package:omvila_isletme/features/customers/presentation/customers_screen.dart';
import 'package:omvila_isletme/features/new_appointment/presentation/new_appointment_screen.dart';
import 'package:omvila_isletme/features/new_appointment/presentation/reschedule_screen.dart';
import 'package:omvila_isletme/features/notifications/presentation/notifications_screen.dart';
import 'package:omvila_isletme/features/profile/presentation/notification_settings_screen.dart';
import 'package:omvila_isletme/features/profile/presentation/profile_screen.dart';
import 'package:omvila_isletme/features/push/data/push_prompt_controller.dart';
import 'package:omvila_isletme/features/push/presentation/notification_permission_screen.dart';
import 'package:omvila_isletme/features/shell/presentation/app_shell.dart';
import 'package:omvila_isletme/features/shell/presentation/tabs.dart';
import 'package:omvila_isletme/features/today/presentation/today_screen.dart';
import 'package:omvila_isletme/features/walk_ins/presentation/walk_in_screen.dart';

/// Oturum ve izin durumuna göre yönlendirme. Bildirimden randevu
/// ayrıntısına derin bağlantı `Routes.appointment`.
final routerProvider = Provider<GoRouter>((ref) {
  final refresh = ValueNotifier<int>(0);
  ref
    ..listen(authControllerProvider, (_, _) => refresh.value++)
    ..listen(pushPromptProvider, (_, _) => refresh.value++)
    ..onDispose(refresh.dispose);

  final router = GoRouter(
    refreshListenable: refresh,
    redirect: (context, state) => redirectFor(
      location: state.matchedLocation,
      auth: ref.read(authControllerProvider),
      prompt: ref.read(pushPromptProvider),
    ),
    routes: [
      GoRoute(
        path: Routes.splash,
        builder: (_, _) => const SplashScreen(),
      ),
      GoRoute(
        path: Routes.login,
        builder: (_, _) => const LoginScreen(),
        routes: [
          GoRoute(
            path: 'kod',
            builder: (_, state) =>
                LoginCodeScreen(step: state.extra! as OtpStep),
            redirect: (_, state) =>
                state.extra is OtpStep ? null : Routes.login,
          ),
        ],
      ),
      GoRoute(
        path: Routes.notificationPermission,
        builder: (_, _) => const NotificationPermissionScreen(),
      ),
      ShellRoute(
        builder: (_, state, child) =>
            AppShell(location: state.matchedLocation, child: child),
        routes: [
          GoRoute(
            path: Routes.today,
            builder: (_, _) => const TodayScreen(),
          ),
          GoRoute(
            path: Routes.calendar,
            builder: (_, _) => const CalendarScreen(),
          ),
          GoRoute(
            path: Routes.customers,
            builder: (_, _) => const CustomersScreen(),
          ),
          GoRoute(
            path: Routes.walkIns,
            builder: (_, _) => const WalkInScreen(),
          ),
          GoRoute(
            path: Routes.notifications,
            builder: (_, _) => const NotificationsScreen(),
          ),
          GoRoute(
            path: Routes.profile,
            builder: (_, _) => const ProfileScreen(),
          ),
        ],
      ),
      GoRoute(
        path: Routes.newAppointment,
        builder: (_, state) => NewAppointmentScreen(
          initialDate: _localDate(state.uri.queryParameters['gun']),
          initialCustomer: state.extra is CustomerSummary
              ? state.extra! as CustomerSummary
              : null,
        ),
      ),
      GoRoute(
        path: '/musteriler/:id',
        builder: (_, state) =>
            CustomerCardScreen(customerId: state.pathParameters['id']!),
      ),
      GoRoute(
        path: Routes.notificationSettings,
        builder: (_, _) => const NotificationSettingsScreen(),
      ),
      GoRoute(
        path: '/randevu/:id',
        builder: (_, state) =>
            AppointmentDetailScreen(appointmentId: state.pathParameters['id']!),
        routes: [
          GoRoute(
            path: 'degistir',
            builder: (_, state) =>
                RescheduleScreen(appointmentId: state.pathParameters['id']!),
          ),
        ],
      ),
    ],
  );
  ref.onDispose(router.dispose);
  return router;
});

/// Yönlendirme kuralı (saf işlev; testte doğrudan denenir).
@visibleForTesting
String? redirectFor({
  required String location,
  required AsyncValue<ManageSession?> auth,
  required AsyncValue<PushPromptState> prompt,
}) {
  final promptState = prompt.value;
  if (!auth.hasValue || promptState == null) {
    // Açılış: oturum okunuyor veya bağlantı hatası (Açılış "Tekrar dene").
    return location == Routes.splash ? null : Routes.splash;
  }

  final session = auth.value;
  final onLogin = location.startsWith(Routes.login);
  if (session == null) {
    return onLogin ? null : Routes.login;
  }

  if (onLogin || location == Routes.splash) {
    return promptState.asked ? Routes.today : Routes.notificationPermission;
  }

  const tabRoots = [
    Routes.calendar,
    Routes.customers,
    Routes.walkIns,
    Routes.notifications,
  ];
  final allowed = {
    for (final tab in tabsFor(session)) tab.path,
    if (session.can('walkin.manage')) Routes.walkIns,
  };
  if (tabRoots.contains(location) && !allowed.contains(location)) {
    return Routes.today;
  }

  if (location.startsWith('${Routes.customers}/') &&
      !session.can('customer.view')) {
    return Routes.today;
  }

  if (location == Routes.newAppointment && !session.can('appointment.create')) {
    return Routes.today;
  }

  return null;
}

/// Sorgudaki gün yalnız "yyyy-aa-gg" ise kullanılır.
String? _localDate(String? value) =>
    value != null && RegExp(r'^\d{4}-\d{2}-\d{2}$').hasMatch(value)
    ? value
    : null;
