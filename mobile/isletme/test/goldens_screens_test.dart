import 'package:flutter/material.dart';
import 'package:flutter_riverpod/misc.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_isletme/app/providers.dart';
import 'package:omvila_isletme/features/appointments/data/appointment_repository.dart';
import 'package:omvila_isletme/features/auth/data/auth_controller.dart';
import 'package:omvila_isletme/features/customers/data/customer_repository.dart';
import 'package:omvila_isletme/features/customers/presentation/customer_card_screen.dart';
import 'package:omvila_isletme/features/customers/presentation/customers_screen.dart';
import 'package:omvila_isletme/features/new_appointment/data/booking_repository.dart';
import 'package:omvila_isletme/features/notifications/data/notification_repository.dart';
import 'package:omvila_isletme/features/notifications/presentation/notifications_screen.dart';
import 'package:omvila_isletme/features/profile/data/profile_repository.dart';
import 'package:omvila_isletme/features/profile/presentation/notification_settings_screen.dart';
import 'package:omvila_isletme/features/profile/presentation/profile_screen.dart';
import 'package:omvila_isletme/features/shell/presentation/app_shell.dart';
import 'package:omvila_isletme/features/walk_ins/data/walk_in_repository.dart';
import 'package:omvila_isletme/features/walk_ins/presentation/walk_in_screen.dart';
import 'package:omvila_isletme/features/walk_ins/presentation/walk_in_sheets.dart';
import 'package:omvila_isletme/shared/ui.dart';
import 'package:omvila_ui/omvila_ui.dart';

import 'helpers/appointment_fixtures.dart';
import 'helpers/fixtures.dart';
import 'helpers/screen_fixtures.dart';
import 'helpers/screen_golden.dart';

// Kural: dilim-05 KK11 — Sira, SiraBos, SiraEkle, Musteriler,
// MusterilerAramaYok, MusterilerBos, MusteriKarti, MusteriKartiYeni,
// Bildirimler, BildirimlerBos, Profil, BildirimAyar açık/koyu ve 1.3× yazı
// ölçeğinde golden (design/ekranlar/isletme-uygulamasi/ ile karşılaştırma).
void main() {
  final clock = FixedClock.parse('2026-11-25T09:10:00Z');

  List<Override> base({
    Map<String, Object?> sessionJson = ownerSessionJson,
    int unread = 3,
  }) => [
    clockProvider.overrideWithValue(clock),
    authControllerProvider.overrideWith(
      () => FixedAuth(session(sessionJson)),
    ),
    // Sekme çubuğundaki okunmamış rozeti (sunucunun unreadCount'u).
    unreadNotificationCountProvider.overrideWith((ref) => unread),
  ];

  List<Override> catalog() => [
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
  ];

  List<Override> queue({bool empty = false}) => [
    ...base(),
    walkInQueueProvider.overrideWith(
      (ref, id) => WalkInQueue.fromJson(walkInQueueJson(empty: empty)),
    ),
  ];

  screenGoldenTest(
    'Sıra',
    fileName: 'sira',
    overrides: queue(),
    builder: (_) => const WalkInScreen(),
  );

  screenGoldenTest(
    'Sıra boş',
    fileName: 'sira_bos',
    overrides: queue(empty: true),
    builder: (_) => const WalkInScreen(),
  );

  screenGoldenTest(
    'Sıraya ekle',
    fileName: 'sira_ekle',
    overrides: [...base(), ...catalog()],
    builder: (context) {
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
            child: SheetBody(
              child: WalkInAddForm(
                branchId: branchId,
                queue: WalkInQueue.fromJson(walkInQueueJson()),
              ),
            ),
          ),
        ),
      );
    },
  );

  List<Override> customers(Map<String, Object?> json) => [
    ...base(),
    customerListProvider.overrideWithBuild(
      (ref, notifier) => CustomerPage(
        ManageListCustomers200Response.fromJson(json).items,
        null,
      ),
    ),
  ];

  screenGoldenTest(
    'Müşteriler',
    fileName: 'musteriler',
    overrides: customers(customerListJson()),
    builder: (_) =>
        const AppShell(location: '/musteriler', child: CustomersScreen()),
  );

  screenGoldenTest(
    'Müşteriler arama sonuçsuz',
    fileName: 'musteriler_arama_yok',
    overrides: customers(emptyPageJson),
    builder: (_) => const AppShell(
      location: '/musteriler',
      child: CustomersScreen(
        initialQuery: 'Kaan Yıl',
        initialFilter: CustomerFilter.withPackage,
      ),
    ),
  );

  screenGoldenTest(
    'Müşteriler boş',
    fileName: 'musteriler_bos',
    overrides: customers(emptyPageJson),
    builder: (_) =>
        const AppShell(location: '/musteriler', child: CustomersScreen()),
  );

  List<Override> card(
    Map<String, Object?> customer,
    Map<String, Object?> history, {
    bool withPackage = true,
  }) => [
    ...base(),
    customerProvider.overrideWith(
      (ref, id) => CustomerSummary.fromJson(customer),
    ),
    customerAppointmentsProvider.overrideWith(
      (ref, id) => ManageListCustomerAppointments200Response.fromJson(
        history,
      ).items,
    ),
    customerPackagesProvider.overrideWith(
      (ref, id) => withPackage
          ? CustomerPackagePage.fromJson(fonPackagePageJson()).items
          : const <CustomerPackage>[],
    ),
  ];

  screenGoldenTest(
    'Müşteri kartı',
    fileName: 'musteri_karti',
    overrides: card(selinJson(), customerAppointmentsJson()),
    builder: (_) => const CustomerCardScreen(customerId: selinId),
  );

  screenGoldenTest(
    'Müşteri kartı yeni',
    fileName: 'musteri_karti_yeni',
    overrides: card(denizJson(), denizAppointmentsJson(), withPackage: false),
    builder: (_) => const CustomerCardScreen(customerId: denizId),
  );

  List<Override> notifications({bool empty = false}) => [
    ...base(unread: empty ? 0 : 3),
    notificationsProvider.overrideWith(
      (ref, unreadOnly) =>
          InAppNotificationPage.fromJson(notificationsJson(empty: empty)),
    ),
  ];

  screenGoldenTest(
    'Bildirimler',
    fileName: 'bildirimler',
    overrides: notifications(),
    builder: (_) => const AppShell(
      location: '/bildirimler',
      child: NotificationsScreen(),
    ),
  );

  screenGoldenTest(
    'Bildirimler boş',
    fileName: 'bildirimler_bos',
    overrides: notifications(empty: true),
    builder: (_) => const AppShell(
      location: '/bildirimler',
      child: NotificationsScreen(),
    ),
  );

  screenGoldenTest(
    'Profil',
    fileName: 'profil',
    overrides: [
      ...base(),
      myCommissionProvider.overrideWith(
        (ref) => MyCommissionSummary.fromJson(commissionJson()),
      ),
    ],
    builder: (_) => const AppShell(location: '/profil', child: ProfileScreen()),
  );

  screenGoldenTest(
    'Bildirim ayarları',
    fileName: 'bildirim_ayar',
    overrides: [
      ...base(),
      myPreferencesProvider.overrideWith(
        (ref) => MyNotificationPreferences.fromJson(preferencesJson()),
      ),
    ],
    builder: (_) => const NotificationSettingsScreen(),
  );
}
