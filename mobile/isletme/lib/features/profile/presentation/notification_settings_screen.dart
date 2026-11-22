import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_isletme/features/auth/data/auth_controller.dart';
import 'package:omvila_isletme/features/profile/data/profile_repository.dart';
import 'package:omvila_isletme/shared/ui.dart';
import 'package:omvila_isletme/shared/widgets.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Bildirim ayarları (BildirimAyar): yalnız kendi telefonuna gelen push'lar.
/// Her anahtar değişince kaydedilir (If-Match). Sessiz saat aralığı v1'de
/// platform varsayılanı ve salt okunur (sözleşme); yalnız açılıp kapanır.
class NotificationSettingsScreen extends ConsumerStatefulWidget {
  /// Ayarlar.
  const NotificationSettingsScreen({super.key});

  @override
  ConsumerState<NotificationSettingsScreen> createState() =>
      _NotificationSettingsScreenState();
}

class _NotificationSettingsScreenState
    extends ConsumerState<NotificationSettingsScreen> {
  MyNotificationPreferences? _local;
  bool _saving = false;

  Future<void> _save(
    MyNotificationPreferences previous,
    MyNotificationPreferences next,
  ) async {
    setState(() {
      _local = next;
      _saving = true;
    });
    final messenger = ScaffoldMessenger.of(context);
    try {
      final saved = await ref.read(savePreferencesProvider)(next);
      if (mounted) {
        setState(() => _local = saved);
      }
    } on ApiProblem catch (e) {
      messenger.showSnackBar(SnackBar(content: Text(errorMessage(e.code))));
      if (!mounted) {
        return;
      }

      if (e.status == 412) {
        // Başka cihazda değişmiş: güncel tercihler okunur.
        setState(() => _local = null);
        ref.invalidate(myPreferencesProvider);
      } else {
        setState(() => _local = previous);
      }
    } finally {
      if (mounted) {
        setState(() => _saving = false);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final session = ref.watch(authControllerProvider).value;
    final loaded = ref.watch(myPreferencesProvider);
    final prefs = _local ?? loaded.value;

    final List<Widget> body;
    if (prefs != null) {
      Widget toggle(
        String title,
        String subtitle, {
        required bool value,
        required MyNotificationPreferences Function({required bool on}) apply,
      }) => SwitchListTile(
        title: Text(
          title,
          style: OmvilaYazi.govdeGuclu.copyWith(color: r.metin),
        ),
        subtitle: Text(
          subtitle,
          style: OmvilaYazi.aciklama.copyWith(color: r.metinIkincil),
        ),
        value: value,
        onChanged: _saving ? null : (v) => _save(prefs, apply(on: v)),
      );

      final summaryTime = prefs.dailySummaryLocalTime;
      final quietStart = prefs.quietHoursStartLocal;
      final quietEnd = prefs.quietHoursEndLocal;
      body = [
        const SectionHeader('Randevular'),
        const SizedBox(height: OmvilaBosluk.bosluk2),
        _Group(
          children: [
            toggle(
              'Yeni online randevu',
              'Onay bekleyenler dahil',
              value: prefs.newOnlineAppointment,
              apply: ({required on}) =>
                  prefs.copyWith(newOnlineAppointment: on),
            ),
            toggle(
              'İptal ve saat değişikliği',
              'Sizin randevularınızda',
              value: prefs.cancellationAndReschedule,
              apply: ({required on}) =>
                  prefs.copyWith(cancellationAndReschedule: on),
            ),
            toggle(
              'Müşteri geldi',
              'Resepsiyon check-in yapınca',
              value: prefs.customerCheckedIn,
              apply: ({required on}) => prefs.copyWith(customerCheckedIn: on),
            ),
          ],
        ),
        const SizedBox(height: OmvilaBosluk.bosluk5),
        const SectionHeader('Diğer'),
        const SizedBox(height: OmvilaBosluk.bosluk2),
        _Group(
          children: [
            toggle(
              'Kapıda sıra',
              'Sıra size gelince',
              value: prefs.walkInQueue,
              apply: ({required on}) => prefs.copyWith(walkInQueue: on),
            ),
            toggle(
              'Yeni değerlendirme',
              'Sizi puanlayanlar',
              value: prefs.newReview,
              apply: ({required on}) => prefs.copyWith(newReview: on),
            ),
            toggle(
              'Gün sonu özeti',
              summaryTime == null ? 'Her akşam' : 'Her akşam $summaryTime',
              value: prefs.dailySummary,
              apply: ({required on}) => prefs.copyWith(dailySummary: on),
            ),
          ],
        ),
        const SizedBox(height: OmvilaBosluk.bosluk3),
        _Group(
          children: [
            toggle(
              'Sessiz saatler',
              quietStart == null || quietEnd == null
                  ? 'Gece bildirim gelmez'
                  : '$quietStart–$quietEnd arası bildirim gelmez',
              value: prefs.quietHoursEnabled,
              apply: ({required on}) => prefs.copyWith(quietHoursEnabled: on),
            ),
          ],
        ),
        if (!(session?.isStaff ?? true)) ...[
          const SizedBox(height: OmvilaBosluk.bosluk3),
          Text(
            'SMS kontörünün azalması, abonelik ödemesi ve müşterinin havale '
            'bildirimi kapatılamaz.',
            style: OmvilaYazi.aciklama.copyWith(color: r.metinIkincil),
          ),
        ],
      ];
    } else if (loaded.hasError) {
      body = [
        LoadError(
          title: 'Bildirim ayarları yüklenemedi',
          error: loaded.error,
          icon: LucideIcons.bell,
          onRetry: () => ref.invalidate(myPreferencesProvider),
        ),
      ];
    } else {
      body = const [Loading()];
    }

    return Scaffold(
      backgroundColor: r.zemin,
      body: SafeArea(
        child: ListView(
          padding: const EdgeInsets.fromLTRB(
            OmvilaBosluk.bosluk4,
            OmvilaBosluk.bosluk2,
            OmvilaBosluk.bosluk4,
            OmvilaBosluk.bosluk8,
          ),
          children: [
            const ScreenTitle(title: 'Bildirim ayarları', showBack: true),
            const SizedBox(height: OmvilaBosluk.bosluk1),
            Text(
              'Yalnızca sizin telefonunuza gelenler. Müşteri mesajları '
              'işletme ayarlarındadır.',
              style: OmvilaYazi.govde.copyWith(color: r.metinIkincil),
            ),
            const SizedBox(height: OmvilaBosluk.bosluk5),
            ...body,
          ],
        ),
      ),
    );
  }
}

class _Group extends StatelessWidget {
  const _Group({required this.children});

  final List<Widget> children;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    return SurfaceCard(
      padding: const EdgeInsets.symmetric(vertical: OmvilaBosluk.bosluk1),
      child: Column(
        children: [
          for (var i = 0; i < children.length; i++) ...[
            if (i > 0)
              Divider(
                height: 1,
                indent: OmvilaBosluk.bosluk4,
                endIndent: OmvilaBosluk.bosluk4,
                color: r.cizgi,
              ),
            children[i],
          ],
        ],
      ),
    );
  }
}
