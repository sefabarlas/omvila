import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Alt sekmeler: Randevularım, Paketlerim, Profil (keşif sekmesi yok).
/// Kaydırılan içerik yüzen çubuğun altından geçer.
class SekmeKabugu extends StatelessWidget {
  /// Kabuk.
  const SekmeKabugu({
    required this.seciliIndex,
    required this.onSecildi,
    required this.child,
    super.key,
  });

  /// go_router sekme dallarıyla.
  factory SekmeKabugu.shell(StatefulNavigationShell shell) => SekmeKabugu(
    seciliIndex: shell.currentIndex,
    onSecildi: (index) =>
        shell.goBranch(index, initialLocation: index == shell.currentIndex),
    child: shell,
  );

  /// Seçili sekme.
  final int seciliIndex;

  /// Sekmeye dokunuldu.
  final ValueChanged<int> onSecildi;

  /// Sekme içeriği.
  final Widget child;

  static const _ogeler = [
    SekmeOgesi(ikon: LucideIcons.calendar, etiket: 'Randevularım'),
    SekmeOgesi(ikon: LucideIcons.package, etiket: 'Paketlerim'),
    SekmeOgesi(ikon: LucideIcons.user, etiket: 'Profil'),
  ];

  @override
  Widget build(BuildContext context) => Scaffold(
    extendBody: true,
    body: child,
    bottomNavigationBar: YuzenSekmeCubugu(
      ogeler: _ogeler,
      seciliIndex: seciliIndex,
      onSecildi: onSecildi,
    ),
  );
}

/// Sekme kök ekranı: büyük başlık ve içerik; altta çubuk payı (104).
class SekmeSayfasi extends StatelessWidget {
  /// Sayfa.
  const SekmeSayfasi({
    required this.baslik,
    required this.children,
    this.ustBaslik,
    this.onRefresh,
    this.eylem,
    super.key,
  });

  /// Başlığın sağındaki eylem (Randevularım'da bildirim zili).
  final Widget? eylem;

  /// "Randevularım".
  final String baslik;

  /// Başlığın üstündeki küçük satır ("Merhaba Merve").
  final String? ustBaslik;

  /// Başlığın altındaki içerik.
  final List<Widget> children;

  /// Aşağı çekip yenileme.
  final Future<void> Function()? onRefresh;

  /// Son öğenin altındaki boşluk (yüzen sekme çubuğu).
  static const altBosluk = 104.0;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final subtitle = ustBaslik;
    final list = ListView(
      physics: const AlwaysScrollableScrollPhysics(),
      padding: EdgeInsets.fromLTRB(
        OmvilaBosluk.bosluk4,
        MediaQuery.paddingOf(context).top + OmvilaBosluk.bosluk8,
        OmvilaBosluk.bosluk4,
        altBosluk,
      ),
      children: [
        if (subtitle != null)
          Text(
            subtitle,
            style: OmvilaYazi.ikincil.copyWith(
              color: r.metinIkincil,
              fontWeight: FontWeight.w600,
            ),
          ),
        Row(
          children: [
            Expanded(
              child: Semantics(
                header: true,
                child: Text(
                  baslik,
                  style: OmvilaYazi.buyukBaslik.copyWith(color: r.metin),
                ),
              ),
            ),
            ?eylem,
          ],
        ),
        const SizedBox(height: OmvilaBosluk.bosluk4),
        ...children,
      ],
    );
    final refresh = onRefresh;
    return ColoredBox(
      color: r.zemin,
      child: refresh == null
          ? list
          : RefreshIndicator(
              onRefresh: refresh,
              color: r.marka,
              child: list,
            ),
    );
  }
}
