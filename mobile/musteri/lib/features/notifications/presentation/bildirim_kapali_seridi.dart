import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_musteri/features/notifications/presentation/notification_prefs_controller.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// "Bildirimler kapalı" şeridi (MainBildirimKapali.png): "Şimdi değil"
/// sonrası; kapatılabilir.
class BildirimKapaliSeridi extends ConsumerWidget {
  /// Şerit.
  const BildirimKapaliSeridi({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final r = context.omvilaRenkler;
    final controller = ref.read(notificationPrefsProvider.notifier);
    return DecoratedBox(
      decoration: BoxDecoration(
        color: r.zeminIkincil,
        borderRadius: BorderRadius.circular(OmvilaKose.koseLg),
      ),
      child: Padding(
        padding: const EdgeInsets.fromLTRB(
          OmvilaBosluk.bosluk4,
          OmvilaBosluk.bosluk3,
          OmvilaBosluk.bosluk1,
          OmvilaBosluk.bosluk2,
        ),
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Padding(
              padding: const EdgeInsets.only(top: 2),
              child: Icon(LucideIcons.bellOff, size: 20, color: r.metin),
            ),
            const SizedBox(width: OmvilaBosluk.bosluk3),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    'Bildirimler kapalı. Hatırlatmalar SMS ile gelecek.',
                    style: OmvilaYazi.ikincil.copyWith(color: r.metin),
                  ),
                  TextButton(
                    onPressed: controller.openSettings,
                    style: TextButton.styleFrom(
                      foregroundColor: r.metinMarka,
                      padding: EdgeInsets.zero,
                      textStyle: OmvilaYazi.ikincil.copyWith(
                        fontWeight: FontWeight.w700,
                        decoration: TextDecoration.underline,
                      ),
                    ),
                    child: const Text('Ayarlardan aç'),
                  ),
                ],
              ),
            ),
            IconButton(
              tooltip: 'Kapat',
              onPressed: controller.dismissBanner,
              icon: Icon(LucideIcons.x, size: 20, color: r.metin),
            ),
          ],
        ),
      ),
    );
  }
}
