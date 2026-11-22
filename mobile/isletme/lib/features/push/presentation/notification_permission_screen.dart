import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_isletme/features/push/data/push_prompt_controller.dart';
import 'package:omvila_isletme/features/shell/presentation/tabs.dart';
import 'package:omvila_isletme/shared/brand_backdrop.dart';
import 'package:omvila_isletme/shared/full_height_scroll.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Bildirim izni (BildirimIzni): sistem istemi yalnız "Bildirimlere izin
/// ver"e basınca açılır; "Şimdi değil" Bugün'de şerit bırakır. Onay süresi
/// ayardan gelir; metinde sabit süre yazılmaz (T-107).
class NotificationPermissionScreen extends ConsumerWidget {
  /// İzin ekranı.
  const NotificationPermissionScreen({super.key});

  Future<void> _answer(
    BuildContext context,
    Future<void> Function() action,
  ) async {
    await action();
    if (context.mounted) {
      context.go(Routes.today);
    }
  }

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final r = context.omvilaRenkler;
    final controller = ref.read(pushPromptProvider.notifier);
    return Scaffold(
      body: FullHeightScroll(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            BrandBackdrop(
              secondShape: false,
              borderRadius: const BorderRadius.vertical(
                bottom: Radius.circular(28),
              ),
              child: SafeArea(
                bottom: false,
                child: Padding(
                  padding: const EdgeInsets.fromLTRB(
                    OmvilaBosluk.bosluk6,
                    OmvilaBosluk.bosluk12 + OmvilaBosluk.bosluk6,
                    OmvilaBosluk.bosluk6,
                    120,
                  ),
                  child: ExcludeSemantics(
                    child: Column(
                      children: [
                        _SampleNotification(
                          icon: LucideIcons.clock,
                          title: 'Onay bekliyor',
                          body: 'Selin A. · Yarın 11:00 boya',
                          background: r.zeminYukseltilmis,
                          foreground: r.metin,
                        ),
                        const SizedBox(height: OmvilaBosluk.bosluk2),
                        Padding(
                          padding: const EdgeInsets.symmetric(
                            horizontal: OmvilaBosluk.bosluk4,
                          ),
                          child: _SampleNotification(
                            icon: LucideIcons.calendarPlus,
                            body: 'Yeni randevu: Derya Ş. · 13:00 kesim',
                            background: r.zeminYukseltilmis.withValues(
                              alpha: 0.8,
                            ),
                            foreground: r.metin,
                          ),
                        ),
                      ],
                    ),
                  ),
                ),
              ),
            ),
            Expanded(
              child: SafeArea(
                top: false,
                child: Padding(
                  padding: const EdgeInsets.all(OmvilaBosluk.bosluk6),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.stretch,
                    children: [
                      Semantics(
                        header: true,
                        child: Text(
                          'Talepleri anında görün',
                          style: OmvilaYazi.baslik1.copyWith(
                            color: r.metin,
                            fontSize: 32,
                            fontWeight: FontWeight.w800,
                          ),
                        ),
                      ),
                      const SizedBox(height: OmvilaBosluk.bosluk3),
                      Text(
                        'Yeni randevu, onay bekleyen talep ve sıra '
                        'bildirimleri gelsin. Onay bekleyen bir talep '
                        'süresi içinde yanıtlanmazsa otomatik iptal olur.',
                        style: OmvilaYazi.ikincil.copyWith(
                          color: r.metinIkincil,
                        ),
                      ),
                      const Spacer(),
                      const SizedBox(height: OmvilaBosluk.bosluk6),
                      OmvilaButon(
                        etiket: 'Bildirimlere izin ver',
                        boyut: OmvilaButonBoyutu.lg,
                        tamGenislik: true,
                        onPressed: () => _answer(context, controller.request),
                      ),
                      const SizedBox(height: OmvilaBosluk.bosluk2),
                      OmvilaButon(
                        etiket: 'Şimdi değil',
                        tur: OmvilaButonTuru.hayalet,
                        boyut: OmvilaButonBoyutu.lg,
                        tamGenislik: true,
                        onPressed: () => _answer(context, controller.skip),
                      ),
                    ],
                  ),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _SampleNotification extends StatelessWidget {
  const _SampleNotification({
    required this.icon,
    required this.body,
    required this.background,
    required this.foreground,
    this.title,
  });

  final IconData icon;
  final String? title;
  final String body;
  final Color background;
  final Color foreground;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final heading = title;
    return Container(
      padding: const EdgeInsets.all(OmvilaBosluk.bosluk3),
      decoration: BoxDecoration(
        color: background,
        borderRadius: BorderRadius.circular(OmvilaKose.koseLg),
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Container(
            width: 36,
            height: 36,
            decoration: BoxDecoration(
              color: r.bilgiZemin,
              borderRadius: BorderRadius.circular(OmvilaKose.koseSm),
            ),
            child: Icon(icon, size: 20, color: r.bilgi),
          ),
          const SizedBox(width: OmvilaBosluk.bosluk3),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                if (heading != null)
                  Text(
                    heading,
                    style: OmvilaYazi.govdeGuclu.copyWith(color: foreground),
                  ),
                Text(
                  body,
                  style: OmvilaYazi.ikincil.copyWith(color: foreground),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
