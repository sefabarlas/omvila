import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/app/providers.dart';
import 'package:omvila_musteri/app/router.dart';
import 'package:omvila_musteri/features/auth/presentation/auth_controller.dart';
import 'package:omvila_musteri/features/booking/presentation/salon_secimi_ekrani.dart';
import 'package:omvila_musteri/features/profile/presentation/profile_providers.dart';
import 'package:omvila_musteri/features/shell/sekme_kabugu.dart';
import 'package:omvila_musteri/shared/widgets/durum_desenleri.dart';
import 'package:omvila_musteri/shared/widgets/ic_sayfa.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Profil sekmesi (Profil): ad ve telefon, yatay "Salonlarım" (dokununca
/// randevu alma), menü (Profil bilgilerim, Ödemelerim, İzinlerim, Yardım ve
/// iletişim) ve "Çıkış yap".
class ProfilEkrani extends ConsumerWidget {
  /// Ekran.
  const ProfilEkrani({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final profile = ref.watch(profileProvider);
    return SekmeSayfasi(
      baslik: 'Profil',
      onRefresh: () {
        ref
          ..invalidate(brandsProvider)
          ..invalidate(consentsProvider);
        return ref.refresh(profileProvider.future);
      },
      children: switch (profile) {
        AsyncData(:final value) => [
          _Baslik(profil: value),
          const SizedBox(height: OmvilaBosluk.bosluk6),
          const _Salonlarim(),
          const SizedBox(height: OmvilaBosluk.bosluk5),
          const _Menu(),
          const SizedBox(height: OmvilaBosluk.bosluk3),
          const _CikisYap(),
        ],
        AsyncError(:final error) => [
          HataDurumu(
            error: error,
            onRetry: () => ref.invalidate(profileProvider),
          ),
        ],
        _ => const [
          Padding(
            padding: EdgeInsets.only(top: OmvilaBosluk.bosluk12),
            child: YukleniyorDurumu(),
          ),
        ],
      },
    );
  }
}

class _Baslik extends StatelessWidget {
  const _Baslik({required this.profil});

  final MeProfile profil;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final name = profil.fullName?.trim();
    final hasName = name != null && name.isNotEmpty;
    return MergeSemantics(
      child: Row(
        children: [
          CircleAvatar(
            radius: 36,
            backgroundColor: r.avatar2Zemin,
            child: hasName
                ? Text(
                    initials(name),
                    style: OmvilaYazi.baslik2.copyWith(color: r.avatar2Metin),
                  )
                : Icon(LucideIcons.user, color: r.avatar2Metin),
          ),
          const SizedBox(width: OmvilaBosluk.bosluk4),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  hasName ? name : formatPhone(profil.phone),
                  style: OmvilaYazi.baslik1.copyWith(color: r.metin),
                ),
                if (hasName)
                  Text(
                    formatPhone(profil.phone),
                    style: OmvilaYazi.ikincil.copyWith(color: r.metinIkincil),
                  ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _Salonlarim extends ConsumerWidget {
  const _Salonlarim();

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final r = context.omvilaRenkler;
    final brands = ref.watch(brandsProvider);
    final items = brands.value ?? const <MeBrand>[];
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Semantics(
          header: true,
          child: Text(
            'Salonlarım',
            style: OmvilaYazi.baslik3.copyWith(
              color: r.metin,
              fontWeight: FontWeight.w800,
            ),
          ),
        ),
        const SizedBox(height: OmvilaBosluk.bosluk3),
        Material(
          color: r.zeminYukseltilmis,
          borderRadius: BorderRadius.circular(OmvilaKose.koseLg),
          child: Padding(
            padding: const EdgeInsets.symmetric(
              vertical: OmvilaBosluk.bosluk3,
            ),
            child: switch (brands) {
              AsyncData() when items.isEmpty => Padding(
                padding: const EdgeInsets.symmetric(
                  horizontal: OmvilaBosluk.bosluk4,
                ),
                child: Text(
                  'Kayıtlı salonunuz yok. Salonun bağlantısını açın ya da '
                  'salondaki QR kodu okutun.',
                  style: OmvilaYazi.ikincil.copyWith(color: r.metinIkincil),
                ),
              ),
              AsyncData() => SingleChildScrollView(
                scrollDirection: Axis.horizontal,
                padding: const EdgeInsets.symmetric(
                  horizontal: OmvilaBosluk.bosluk3,
                ),
                child: Row(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    for (final b in items) _SalonKutusu(brand: b),
                  ],
                ),
              ),
              AsyncError() => Padding(
                padding: const EdgeInsets.symmetric(
                  horizontal: OmvilaBosluk.bosluk4,
                ),
                child: Text(
                  'Salonlarınız yüklenemedi.',
                  style: OmvilaYazi.ikincil.copyWith(color: r.metinIkincil),
                ),
              ),
              _ => const SizedBox(height: 84, child: YukleniyorDurumu()),
            },
          ),
        ),
        if (items.isNotEmpty) ...[
          const SizedBox(height: OmvilaBosluk.bosluk2),
          Text(
            'Bir salona dokunun, tekrar randevu alın.',
            style: OmvilaYazi.ikincil.copyWith(color: r.metinIkincil),
          ),
        ],
      ],
    );
  }
}

class _SalonKutusu extends ConsumerWidget {
  const _SalonKutusu({required this.brand});

  final MeBrand brand;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final r = context.omvilaRenkler;
    final (zemin, metin) = switch (brand.coverColor) {
      MeBrandCoverColorEnum.mercan => (r.avatar3Zemin, r.avatar3Metin),
      MeBrandCoverColorEnum.petrol => (r.avatar2Zemin, r.avatar2Metin),
      _ => (r.avatar1Zemin, r.avatar1Metin),
    };
    final enabled = brand.bookingEnabled && brand.branches.isNotEmpty;
    return Semantics(
      button: enabled,
      label: enabled ? '${brand.name}, randevu al' : brand.name,
      excludeSemantics: true,
      child: InkWell(
        borderRadius: BorderRadius.circular(OmvilaKose.koseMd),
        onTap: enabled ? () => salonSecildi(context, ref, brand) : null,
        child: SizedBox(
          width: 84,
          child: Padding(
            padding: const EdgeInsets.symmetric(
              horizontal: OmvilaBosluk.bosluk1,
            ),
            child: Column(
              children: [
                Container(
                  width: 56,
                  height: 56,
                  alignment: Alignment.center,
                  decoration: BoxDecoration(
                    color: zemin,
                    borderRadius: BorderRadius.circular(OmvilaKose.koseMd),
                  ),
                  child: Text(
                    initials(brand.name),
                    style: OmvilaYazi.govdeGuclu.copyWith(color: metin),
                  ),
                ),
                const SizedBox(height: OmvilaBosluk.bosluk2),
                Text(
                  brand.name,
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                  textAlign: TextAlign.center,
                  style: OmvilaYazi.aciklama.copyWith(
                    color: r.metin,
                    fontWeight: FontWeight.w700,
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}

class _Menu extends ConsumerWidget {
  const _Menu();

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final consents = ref.watch(consentsProvider).value;
    return GrupKarti(
      children: [
        MenuSatiri(
          ikon: LucideIcons.user,
          baslik: 'Profil bilgilerim',
          altBaslik: 'Ad ve e-posta',
          onTap: () => context.push(Routes.profilBilgileri),
        ),
        MenuSatiri(
          ikon: LucideIcons.receipt,
          baslik: 'Ödemelerim',
          altBaslik: 'Kapora ve salon ödemeleri',
          onTap: () => context.push(Routes.odemeler),
        ),
        MenuSatiri(
          ikon: LucideIcons.shieldCheck,
          baslik: 'İzinlerim (KVKK)',
          altBaslik: consents == null
              ? 'Kampanya mesajları'
              : izinOzeti(consents),
          onTap: () => context.push(Routes.izinler),
        ),
        MenuSatiri(
          ikon: LucideIcons.messageSquare,
          baslik: 'Yardım ve iletişim',
          onTap: () => yardimGoster(context),
        ),
      ],
    );
  }
}

/// Menü satırı: ikon kutusu, başlık, alt metin, ok.
class MenuSatiri extends StatelessWidget {
  /// Satır.
  const MenuSatiri({
    required this.ikon,
    required this.baslik,
    required this.onTap,
    this.altBaslik,
    super.key,
  });

  /// İkon.
  final IconData ikon;

  /// Başlık.
  final String baslik;

  /// Alt metin.
  final String? altBaslik;

  /// Açar.
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final sub = altBaslik;
    return InkWell(
      onTap: onTap,
      child: Padding(
        padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
        child: Row(
          children: [
            IkonKutusu(ikon),
            const SizedBox(width: OmvilaBosluk.bosluk3),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    baslik,
                    style: OmvilaYazi.govdeGuclu.copyWith(color: r.metin),
                  ),
                  if (sub != null)
                    Text(
                      sub,
                      style: OmvilaYazi.ikincil.copyWith(
                        color: r.metinIkincil,
                      ),
                    ),
                ],
              ),
            ),
            Icon(LucideIcons.chevronRight, size: 20, color: r.metin),
          ],
        ),
      ),
    );
  }
}

class _CikisYap extends ConsumerStatefulWidget {
  const _CikisYap();

  @override
  ConsumerState<_CikisYap> createState() => _CikisYapState();
}

class _CikisYapState extends ConsumerState<_CikisYap> {
  bool _busy = false;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final radius = BorderRadius.circular(OmvilaKose.koseLg);
    return Material(
      color: r.zeminYukseltilmis,
      borderRadius: radius,
      child: InkWell(
        borderRadius: radius,
        onTap: _busy
            ? null
            : () async {
                setState(() => _busy = true);
                await ref.read(authControllerProvider.notifier).logout();
              },
        child: Padding(
          padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
          child: Row(
            children: [
              IkonKutusu(LucideIcons.logOut, zemin: r.hataZemin, renk: r.hata),
              const SizedBox(width: OmvilaBosluk.bosluk3),
              Text(
                'Çıkış yap',
                style: OmvilaYazi.govdeGuclu.copyWith(color: r.hata),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

/// "Yardım ve iletişim": salt bilgi; destek adresi tanımlıysa "E-posta
/// gönder" (mailto), aydınlatma metni adresi tanımlıysa bağlantısı.
Future<void> yardimGoster(BuildContext context) => showModalBottomSheet<void>(
  context: context,
  showDragHandle: true,
  isScrollControlled: true,
  backgroundColor: context.omvilaRenkler.zeminYukseltilmis,
  builder: (_) => const YardimSayfasi(),
);

/// Yardım alt sayfası.
class YardimSayfasi extends ConsumerWidget {
  /// Sayfa.
  const YardimSayfasi({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final r = context.omvilaRenkler;
    final config = ref.watch(configProvider);
    final email = config.supportEmail;
    final privacy = config.privacyNoticeUrl;
    final external = ref.read(externalActionsProvider);
    return SafeArea(
      child: Padding(
        padding: const EdgeInsets.fromLTRB(
          OmvilaBosluk.bosluk5,
          0,
          OmvilaBosluk.bosluk5,
          OmvilaBosluk.bosluk5,
        ),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Semantics(
              header: true,
              child: Text(
                'Yardım ve iletişim',
                style: OmvilaYazi.baslik1.copyWith(color: r.metin),
              ),
            ),
            const SizedBox(height: OmvilaBosluk.bosluk2),
            Text(
              'Randevu, kapora ve paketlerle ilgili sorularınız için '
              'randevu aldığınız salonla görüşün; salonun telefonu randevu '
              'ayrıntısında yer alır. Telefon numaranızı değiştirmek veya '
              'uygulamayla ilgili bir sorun bildirmek için bize yazın.',
              style: OmvilaYazi.govde.copyWith(color: r.metinIkincil),
            ),
            const SizedBox(height: OmvilaBosluk.bosluk5),
            if (email != null && email.isNotEmpty)
              OmvilaButon(
                etiket: 'E-posta gönder',
                ikon: LucideIcons.mail,
                boyut: OmvilaButonBoyutu.lg,
                tamGenislik: true,
                onPressed: () =>
                    external.openUrl(Uri(scheme: 'mailto', path: email)),
              ),
            if (privacy != null)
              TextButton(
                onPressed: () => external.openUrl(Uri.parse(privacy)),
                child: const Text('Aydınlatma metnini aç'),
              ),
            TextButton(
              onPressed: () => Navigator.of(context).pop(),
              child: const Text('Kapat'),
            ),
          ],
        ),
      ),
    );
  }
}
