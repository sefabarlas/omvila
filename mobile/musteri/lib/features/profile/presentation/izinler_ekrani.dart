import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/app/router.dart';
import 'package:omvila_musteri/features/profile/presentation/profile_providers.dart';
import 'package:omvila_musteri/shared/widgets/durum_desenleri.dart';
import 'package:omvila_musteri/shared/widgets/ic_sayfa.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// İzinlerim (Izinler): randevu mesajları kapatılamaz (açıklama); salon
/// bazında kampanya izni anahtarı; "Hesabımı ve verilerimi sil".
class IzinlerEkrani extends ConsumerStatefulWidget {
  /// Ekran.
  const IzinlerEkrani({super.key});

  @override
  ConsumerState<IzinlerEkrani> createState() => _IzinlerEkraniState();
}

class _IzinlerEkraniState extends ConsumerState<IzinlerEkrani> {
  final _busy = <String>{};

  Future<void> _toggle(MeConsent consent, {required bool value}) async {
    final id = consent.brand.id;
    setState(() => _busy.add(id));
    final messenger = ScaffoldMessenger.of(context);
    try {
      await ref.read(consentsProvider.notifier).set(id, granted: value);
    } on ApiProblem catch (e) {
      messenger.showSnackBar(SnackBar(content: Text(e.message)));
    } finally {
      if (mounted) {
        setState(() => _busy.remove(id));
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final consents = ref.watch(consentsProvider);
    Widget heading(String text) => Padding(
      padding: const EdgeInsets.only(
        top: OmvilaBosluk.bosluk2,
        bottom: OmvilaBosluk.bosluk3,
      ),
      child: Semantics(
        header: true,
        child: Text(
          text,
          style: OmvilaYazi.baslik3.copyWith(
            color: r.metin,
            fontWeight: FontWeight.w800,
          ),
        ),
      ),
    );

    return IcSayfa(
      baslik: 'İzinlerim',
      aciklama:
          'Kampanya mesajlarını her salon için ayrı ayrı açıp '
          'kapatabilirsiniz.',
      onRefresh: () => ref.refresh(consentsProvider.future),
      children: [
        heading('Randevu mesajları'),
        Container(
          padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
          decoration: BoxDecoration(
            color: r.zeminYukseltilmis,
            borderRadius: BorderRadius.circular(OmvilaKose.koseLg),
          ),
          child: Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              IkonKutusu(
                LucideIcons.bell,
                zemin: r.zeminMarkaYumusak,
                renk: r.metinMarka,
              ),
              const SizedBox(width: OmvilaBosluk.bosluk3),
              Expanded(
                child: Text(
                  'Onay, hatırlatma ve değişiklik mesajları randevunun '
                  'parçasıdır, her zaman gönderilir. Uygulama bildirimleri '
                  'açıksa önce bildirim gelir, ulaşmazsa SMS gönderilir.',
                  style: OmvilaYazi.ikincil.copyWith(color: r.metin),
                ),
              ),
            ],
          ),
        ),
        const SizedBox(height: OmvilaBosluk.bosluk4),
        heading('Kampanyalar'),
        ...switch (consents) {
          AsyncData(:final value) when value.isEmpty => [
            Text(
              'Salon eklediğinizde kampanya izinleri burada görünür.',
              style: OmvilaYazi.ikincil.copyWith(color: r.metinIkincil),
            ),
          ],
          AsyncData(:final value) => [
            GrupKarti(
              children: [
                for (final c in value)
                  SwitchListTile(
                    value: c.marketingGranted,
                    onChanged: _busy.contains(c.brand.id)
                        ? null
                        : (v) => _toggle(c, value: v),
                    title: Text(
                      c.brand.name,
                      style: OmvilaYazi.govdeGuclu.copyWith(color: r.metin),
                    ),
                    subtitle: Text(
                      'Kampanya ve indirim mesajları',
                      style: OmvilaYazi.ikincil.copyWith(
                        color: r.metinIkincil,
                      ),
                    ),
                  ),
              ],
            ),
          ],
          AsyncError(:final error) => [
            HataDurumu(
              error: error,
              onRetry: () => ref.invalidate(consentsProvider),
            ),
          ],
          _ => const [YukleniyorDurumu()],
        },
        const SizedBox(height: OmvilaBosluk.bosluk6),
        Center(
          child: TextButton(
            onPressed: () => context.push(Routes.hesapSil),
            style: TextButton.styleFrom(foregroundColor: r.hata),
            child: const Text('Hesabımı ve verilerimi sil'),
          ),
        ),
      ],
    );
  }
}
