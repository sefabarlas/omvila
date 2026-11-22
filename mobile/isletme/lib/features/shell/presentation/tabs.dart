import 'package:flutter/widgets.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_isletme/features/auth/data/auth_controller.dart';

/// Alt sekme.
@immutable
class AppTab {
  /// Sekme.
  const AppTab(this.path, this.label, this.icon);

  /// Rota.
  final String path;

  /// Ad.
  final String label;

  /// İkon.
  final IconData icon;
}

/// Rota adları.
abstract final class Routes {
  /// Açılış.
  static const splash = '/';

  /// Telefon girişi.
  static const login = '/giris';

  /// SMS kodu.
  static const loginCode = '/giris/kod';

  /// Bildirim izni.
  static const notificationPermission = '/bildirim-izni';

  /// Bugün.
  static const today = '/bugun';

  /// Takvim / Takvimim.
  static const calendar = '/takvim';

  /// Müşteriler.
  static const customers = '/musteriler';

  /// Kapıda sıra.
  static const walkIns = '/sira';

  /// Bildirimler.
  static const notifications = '/bildirimler';

  /// Profil (Primim dahil).
  static const profile = '/profil';

  /// Yeni randevu.
  static const newAppointment = '/yeni-randevu';

  /// Randevu ayrıntısı.
  static String appointment(String id) => '/randevu/$id';
}

/// Rol ve izne göre sekmeler (mobile/isletme/AGENTS.md, roller-yetkiler.md §
/// Menü görünürlüğü, teslim-notlari.md § Rol farkı). Yönetici ve sahip:
/// Bugün, Takvim, Müşteriler, Bildirimler, Profil. Personel: Bugün, Takvimim,
/// Müşteriler, Kapıda sıra, Profil (Primim Profil'de; T-107).
List<AppTab> tabsFor(ManageSession session) => [
  const AppTab(Routes.today, 'Bugün', LucideIcons.house),
  if (session.can('calendar.view'))
    AppTab(
      Routes.calendar,
      session.isStaff ? 'Takvimim' : 'Takvim',
      LucideIcons.calendar,
    ),
  if (session.can('customer.view'))
    const AppTab(Routes.customers, 'Müşteriler', LucideIcons.users),
  if (session.isStaff) ...[
    if (session.can('walkin.manage'))
      const AppTab(Routes.walkIns, 'Kapıda sıra', LucideIcons.listOrdered),
  ] else
    const AppTab(Routes.notifications, 'Bildirimler', LucideIcons.bell),
  const AppTab(Routes.profile, 'Profil', LucideIcons.user),
];
