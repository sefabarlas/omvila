import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/app/router.dart';
import 'package:omvila_musteri/features/booking/domain/booking_draft.dart';
import 'package:omvila_musteri/features/booking/presentation/akis_sayfasi.dart';
import 'package:omvila_musteri/features/booking/presentation/booking_controller.dart';
import 'package:omvila_musteri/features/booking/presentation/fiyat_metinleri.dart';
import 'package:omvila_musteri/shared/widgets/durum_desenleri.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Hizmet seçin (YRHizmet): kategori çipleri, kategori başlıkları, birden
/// fazla seçim; paketi olan hizmette "Paketten".
class HizmetEkrani extends ConsumerStatefulWidget {
  /// Ekran.
  const HizmetEkrani({super.key});

  @override
  ConsumerState<HizmetEkrani> createState() => _HizmetEkraniState();
}

class _HizmetEkraniState extends ConsumerState<HizmetEkrani> {
  String? _category;

  @override
  Widget build(BuildContext context) {
    final draft = ref.watch(bookingProvider)!;
    final catalog = ref.watch(catalogProvider(draft.branchId));
    final packages =
        ref.watch(usablePackagesProvider(draft.branchId)).value ?? const [];
    final value = catalog.value;
    final summary = value == null ? null : BookingSummary.of(value, draft);
    return AkisSayfasi(
      baslik: draft.brandName,
      adim: AkisAdimi.hizmet,
      body: switch (catalog) {
        AsyncData(:final value) => _list(context, value, draft, packages),
        AsyncError(:final error) => HataDurumu(
          error: error,
          onRetry: () => ref.invalidate(catalogProvider(draft.branchId)),
        ),
        _ => const YukleniyorDurumu(),
      },
      alt: summary == null || draft.serviceIds.isEmpty
          ? null
          : AltOzetCubugu(
              baslik: tutarMetni(summary),
              altBaslik: ozetMetni(summary),
              eylem: 'Devam et',
              onPressed: () => context.push(
                value!.allowStaffSelection ? Routes.yrPersonel : Routes.yrSaat,
              ),
            ),
    );
  }

  Widget _list(
    BuildContext context,
    PublicCatalog catalog,
    BookingDraft draft,
    List<UsablePackageItem> packages,
  ) {
    final r = context.omvilaRenkler;
    if (catalog.services.isEmpty) {
      return const SingleChildScrollView(
        child: OmvilaBosDurum(
          ikon: LucideIcons.scissors,
          baslik: 'Online hizmet yok',
          aciklama:
              'Bu şubede şu an online randevu alınabilen hizmet yok. '
              'Randevu için salonu arayabilirsiniz.',
        ),
      );
    }

    final categories = [
      for (final c in catalog.categories)
        if (catalog.services.any((s) => s.categoryId == c.id)) c,
    ];
    final visible = _category == null
        ? categories
        : categories.where((c) => c.id == _category).toList();
    return ListView(
      padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
      children: [
        EkranBasligi(
          'Hizmet seçin',
          aciklama: '${draft.branchName} şubesi · birden fazla seçebilirsiniz',
        ),
        const SizedBox(height: OmvilaBosluk.bosluk4),
        if (categories.length > 1)
          SingleChildScrollView(
            scrollDirection: Axis.horizontal,
            child: Row(
              spacing: OmvilaBosluk.bosluk2,
              children: [
                _Cip(
                  etiket: 'Tümü',
                  secili: _category == null,
                  onTap: () => setState(() => _category = null),
                ),
                for (final c in categories)
                  _Cip(
                    etiket: c.name,
                    secili: _category == c.id,
                    onTap: () => setState(() => _category = c.id),
                  ),
              ],
            ),
          ),
        for (final c in visible) ...[
          BolumBasligi(c.name),
          for (final s in catalog.services.where((s) => s.categoryId == c.id))
            Padding(
              padding: const EdgeInsets.only(bottom: OmvilaBosluk.bosluk2),
              child: _HizmetSatiri(
                service: s,
                draft: draft,
                package: packages.where((p) => p.serviceId == s.id).firstOrNull,
              ),
            ),
        ],
        if (catalog.priceNote case final note?) ...[
          const SizedBox(height: OmvilaBosluk.bosluk2),
          Text(
            note,
            style: OmvilaYazi.aciklama.copyWith(color: r.metinIkincil),
          ),
        ],
      ],
    );
  }
}

class _HizmetSatiri extends ConsumerWidget {
  const _HizmetSatiri({
    required this.service,
    required this.draft,
    required this.package,
  });

  final PublicCatalogServicesInner service;
  final BookingDraft draft;
  final UsablePackageItem? package;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final r = context.omvilaRenkler;
    final s = service;
    final secili = draft.serviceIds.contains(s.id);
    final fromPackage = secili
        ? draft.packageItems.containsKey(s.id)
        : package != null;
    final p = package;
    final detail = [
      formatDuration(s.durationMinutes),
      if (p != null)
        '${p.totalSessions} seansın ${sayiIyelik(p.remainingSessions)} kaldı'
      else if (s.description case final d? when d.isNotEmpty)
        d,
    ].join(' · ');
    final price = fromPackage ? 'Paketten' : hizmetFiyati(s);
    return SecimKarti(
      secili: secili,
      onTap: () => ref
          .read(bookingProvider.notifier)
          .toggleService(s.id, packageItemId: p?.customerPackageItemId),
      sag: price == null
          ? null
          : Text(
              price,
              style: OmvilaYazi.govdeGuclu.copyWith(color: r.metin),
            ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(s.name, style: OmvilaYazi.govdeGuclu.copyWith(color: r.metin)),
          Text(
            detail,
            style: OmvilaYazi.ikincil.copyWith(color: r.metinIkincil),
          ),
        ],
      ),
    );
  }
}

class _Cip extends StatelessWidget {
  const _Cip({required this.etiket, required this.secili, required this.onTap});

  final String etiket;
  final bool secili;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    return Semantics(
      selected: secili,
      button: true,
      excludeSemantics: true,
      label: etiket,
      child: Material(
        color: secili ? r.zeminMarka : r.zeminYukseltilmis,
        shape: StadiumBorder(
          side: BorderSide(color: secili ? r.zeminMarka : r.cizgi),
        ),
        child: InkWell(
          customBorder: const StadiumBorder(),
          onTap: onTap,
          child: Padding(
            padding: const EdgeInsets.symmetric(
              horizontal: OmvilaBosluk.bosluk4,
              vertical: OmvilaBosluk.bosluk2,
            ),
            child: Row(
              mainAxisSize: MainAxisSize.min,
              children: [
                if (secili) ...[
                  Icon(LucideIcons.check, size: 16, color: r.metinTers),
                  const SizedBox(width: OmvilaBosluk.bosluk1),
                ],
                Text(
                  etiket,
                  style: OmvilaYazi.etiket.copyWith(
                    color: secili ? r.metinTers : r.metin,
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

/// Seçim çipi (iptal nedeni, Geçmiş sekmesi); [_Cip] ile aynı görünüm.
class SecimCipi extends StatelessWidget {
  /// Çip.
  const SecimCipi({
    required this.etiket,
    required this.secili,
    required this.onTap,
    super.key,
  });

  /// Metin.
  final String etiket;

  /// Seçili.
  final bool secili;

  /// Seçer.
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) =>
      _Cip(etiket: etiket, secili: secili, onTap: onTap);
}
