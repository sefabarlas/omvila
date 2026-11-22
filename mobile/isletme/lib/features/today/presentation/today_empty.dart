import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_isletme/features/shell/presentation/tabs.dart';
import 'package:omvila_isletme/features/today/data/today_repository.dart';
import 'package:omvila_ui/omvila_ui.dart';
import 'package:share_plus/share_plus.dart';

/// Randevusuz gün (MainBos). "ÇALIŞMA 10–20" bloğu çalışma saati Bugün
/// yanıtında olmadığı için gösterilmez (T-107).
class TodayEmpty extends ConsumerWidget {
  /// Boş gün.
  const TodayEmpty({required this.canQueueWalkIn, super.key});

  /// walkin.manage.
  final bool canQueueWalkIn;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final url = ref.watch(bookingUrlProvider).value;
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        const Row(
          spacing: OmvilaBosluk.bosluk3,
          children: [
            Expanded(
              child: BentoBlok(
                ton: BentoTonu.kum,
                ustEtiket: 'Randevu',
                deger: '0',
                aciklama: 'bugün',
              ),
            ),
            Spacer(),
          ],
        ),
        const OmvilaBosDurum(
          ikon: LucideIcons.calendar,
          baslik: 'Bugün randevunuz yok',
          aciklama:
              'Yeni randevular burada sırayla görünür. Boş saatleri '
              'doldurmak için bağlantınızı paylaşabilirsiniz.',
        ),
        if (url != null) ...[
          ActionCard(
            emphasized: true,
            icon: LucideIcons.share2,
            title: 'Rezervasyon bağlantısını paylaş',
            subtitle: url.replaceFirst(RegExp('^https?://'), ''),
            onTap: () =>
                SharePlus.instance.share(ShareParams(uri: Uri.parse(url))),
          ),
          const SizedBox(height: OmvilaBosluk.bosluk2),
        ],
        if (canQueueWalkIn)
          ActionCard(
            icon: LucideIcons.users,
            title: 'Kapıdan gelen müşteriyi sıraya al',
            subtitle: 'Randevusuz gelenler için',
            onTap: () => context.push(Routes.walkIns),
          ),
      ],
    );
  }
}

/// Boş ekrandaki eylem kartı (ikon kutusu, başlık, alt satır, ok).
class ActionCard extends StatelessWidget {
  /// Kart.
  const ActionCard({
    required this.icon,
    required this.title,
    required this.subtitle,
    required this.onTap,
    this.emphasized = false,
    super.key,
  });

  /// İkon.
  final IconData icon;

  /// Başlık (fiille başlar).
  final String title;

  /// Alt satır.
  final String subtitle;

  /// Dokununca.
  final VoidCallback onTap;

  /// Petrol zeminli birincil kart.
  final bool emphasized;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final background = emphasized ? r.blokMarka : r.zeminYukseltilmis;
    final foreground = emphasized ? r.metinTers : r.metin;
    final secondary = emphasized ? r.metinTers : r.metinIkincil;
    final radius = BorderRadius.circular(OmvilaKose.koseXl);
    return DecoratedBox(
      decoration: BoxDecoration(
        borderRadius: radius,
        boxShadow: context.omvilaGolgeler.golgeSm,
      ),
      child: Material(
        color: background,
        borderRadius: radius,
        child: InkWell(
          onTap: onTap,
          borderRadius: radius,
          child: Padding(
            padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
            child: Row(
              children: [
                Container(
                  width: 40,
                  height: 40,
                  decoration: BoxDecoration(
                    color: emphasized
                        ? r.metinTers.withValues(alpha: 0.12)
                        : r.zeminIkincil,
                    borderRadius: BorderRadius.circular(OmvilaKose.koseSm),
                  ),
                  child: Icon(icon, size: 20, color: foreground),
                ),
                const SizedBox(width: OmvilaBosluk.bosluk3),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        title,
                        style: OmvilaYazi.govdeGuclu.copyWith(
                          color: foreground,
                        ),
                      ),
                      Text(
                        subtitle,
                        style: OmvilaYazi.ikincil.copyWith(color: secondary),
                      ),
                    ],
                  ),
                ),
                Icon(LucideIcons.chevronRight, size: 20, color: foreground),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
