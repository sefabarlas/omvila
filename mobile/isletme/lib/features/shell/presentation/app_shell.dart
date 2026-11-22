import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:omvila_isletme/features/auth/data/auth_controller.dart';
import 'package:omvila_isletme/features/shell/presentation/tabs.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Sekme kabuğu: içerik yüzen sekme çubuğunun altından geçer (ekranlar
/// altta [tabBarClearance] boşluk bırakır).
class AppShell extends ConsumerWidget {
  /// Kabuk.
  const AppShell({required this.location, required this.child, super.key});

  /// Geçerli rota.
  final String location;

  /// Sekme içeriği.
  final Widget child;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final session = ref.watch(authControllerProvider).value;
    final tabs = session == null ? const <AppTab>[] : tabsFor(session);
    // Kabuk içinde push edilen sayfada (yöneticide Sıra) `location` alttaki
    // sekmenin adresi kalır; görünen sayfa yönlendiricinin son adresidir.
    final current =
        GoRouter.maybeOf(context)?.state.matchedLocation ?? location;
    final selected = tabs.indexWhere((t) => current.startsWith(t.path));
    return Scaffold(
      body: Stack(
        children: [
          Positioned.fill(child: child),
          // Sekme olmayan kabuk sayfasında (yönetici için Sıra) çubuk yok.
          if (selected >= 0)
            Positioned(
              left: 0,
              right: 0,
              bottom: 0,
              child: YuzenSekmeCubugu(
                ogeler: [
                  for (final tab in tabs)
                    SekmeOgesi(ikon: tab.icon, etiket: tab.label),
                ],
                seciliIndex: selected,
                onSecildi: (i) => context.go(tabs[i].path),
              ),
            ),
        ],
      ),
    );
  }
}

/// Kaydırılan içeriğin son öğesinin altındaki boşluk (mobil README §
/// Yerleşim: 104).
const double tabBarClearance = 104;
