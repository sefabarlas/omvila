import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_isletme/features/auth/data/auth_controller.dart';
import 'package:omvila_isletme/features/shell/presentation/app_shell.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Ekranı sonraki PR'larda gelecek sekme: büyük başlık ve boş durum
/// (docs/acik-sorular.md T-107). Sahte veri veya sayı gösterilmez.
class TabPlaceholderScreen extends StatelessWidget {
  /// Yer tutucu.
  const TabPlaceholderScreen({
    required this.title,
    required this.icon,
    required this.description,
    this.footer,
    super.key,
  });

  /// Büyük başlık.
  final String title;

  /// Boş durum ikonu.
  final IconData icon;

  /// Boş durum cümlesi.
  final String description;

  /// Alttaki ek içerik (Profil: "Çıkış yap").
  final Widget? footer;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final bottom = footer;
    return SafeArea(
      bottom: false,
      child: ListView(
        padding: const EdgeInsets.fromLTRB(
          OmvilaBosluk.bosluk4,
          OmvilaBosluk.bosluk6,
          OmvilaBosluk.bosluk4,
          tabBarClearance,
        ),
        children: [
          Semantics(
            header: true,
            child: Text(
              title,
              style: OmvilaYazi.buyukBaslik.copyWith(color: r.metin),
            ),
          ),
          OmvilaBosDurum(
            ikon: icon,
            baslik: 'Bu bölüm hazırlanıyor',
            aciklama: description,
          ),
          ?bottom,
        ],
      ),
    );
  }
}

/// Takvim / Takvimim.
class CalendarPlaceholder extends ConsumerWidget {
  /// Takvim.
  const CalendarPlaceholder({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final staff = ref.watch(authControllerProvider).value?.isStaff ?? false;
    return TabPlaceholderScreen(
      title: staff ? 'Takvimim' : 'Takvim',
      icon: LucideIcons.calendar,
      description: 'Gün ve hafta görünümü sonraki sürümde bu sekmede açılacak.',
    );
  }
}

/// Profil; "Çıkış yap" şimdiden çalışır.
class ProfilePlaceholder extends ConsumerWidget {
  /// Profil.
  const ProfilePlaceholder({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    return TabPlaceholderScreen(
      title: 'Profil',
      icon: LucideIcons.user,
      description:
          'Primim, bildirim ayarları ve hesap bilgileri sonraki sürümde bu '
          'sekmede açılacak.',
      footer: OmvilaButon(
        etiket: 'Çıkış yap',
        tur: OmvilaButonTuru.ikincil,
        tamGenislik: true,
        onPressed: () => ref.read(authControllerProvider.notifier).logout(),
      ),
    );
  }
}

/// Sekme dışı ekranlar (randevu ayrıntısı, yeni randevu): geri butonlu.
class PagePlaceholderScreen extends StatelessWidget {
  /// Yer tutucu sayfa.
  const PagePlaceholderScreen({
    required this.title,
    required this.description,
    super.key,
  });

  /// Üst bar başlığı.
  final String title;

  /// Boş durum cümlesi.
  final String description;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text(title)),
      body: Center(
        child: OmvilaBosDurum(
          ikon: LucideIcons.calendarDays,
          baslik: 'Bu bölüm hazırlanıyor',
          aciklama: description,
        ),
      ),
    );
  }
}
