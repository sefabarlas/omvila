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

/// Kimden olsun? (YRPersonel): "Fark etmez" (marka izin veriyorsa), hizmeti
/// yapabilen kişiler, kişi farkı; birden çok hizmette "aynı kişiden".
class PersonelEkrani extends ConsumerWidget {
  /// Ekran.
  const PersonelEkrani({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final draft = ref.watch(bookingProvider)!;
    final catalog = ref.watch(catalogProvider(draft.branchId));
    final value = catalog.value;
    final summary = value == null ? null : BookingSummary.of(value, draft);
    return AkisSayfasi(
      baslik: draft.brandName,
      adim: AkisAdimi.personel,
      body: switch (catalog) {
        AsyncData(:final value) => _list(context, ref, value, draft),
        AsyncError(:final error) => HataDurumu(
          error: error,
          onRetry: () => ref.invalidate(catalogProvider(draft.branchId)),
        ),
        _ => const YukleniyorDurumu(),
      },
      alt: summary == null || value == null
          ? null
          : AltOzetCubugu(
              baslik: kisiMetni(value, draft),
              altBaslik:
                  '${tutarMetni(summary)} · '
                  '${formatDuration(summary.durationMinutes)}',
              eylem: 'Devam et',
              onPressed: draft.staffChosen
                  ? () => context.push(Routes.yrSaat)
                  : null,
            ),
    );
  }

  Widget _list(
    BuildContext context,
    WidgetRef ref,
    PublicCatalog catalog,
    BookingDraft draft,
  ) {
    final r = context.omvilaRenkler;
    final controller = ref.read(bookingProvider.notifier);
    final services = {for (final s in catalog.services) s.id: s};
    final multiple = draft.serviceIds.length > 1;
    List<PublicCatalogStaffInner> staffFor(Iterable<String> ids) => [
      for (final st in catalog.staff)
        if (ids.every((id) => services[id]?.staffIds.contains(st.id) ?? false))
          st,
    ];

    List<Widget> options(
      Iterable<String> ids,
      String? current, {
      required bool chosen,
      required ValueChanged<String?> onChoose,
    }) => [
      if (catalog.allowAnyStaffOption)
        _KisiSatiri(
          ad: 'Fark etmez',
          aciklama: 'En erken uygun saat',
          ikon: LucideIcons.users,
          secili: chosen && current == null,
          onTap: () => onChoose(null),
        ),
      for (final st in staffFor(ids))
        _KisiSatiri(
          ad: st.displayName,
          aciklama: st.title,
          fark: ids.fold<int>(
            0,
            (sum, id) => sum + BookingSummary.surchargeOf(catalog, st.id, id),
          ),
          secili: chosen && current == st.id,
          onTap: () => onChoose(st.id),
        ),
    ];

    final first = draft.serviceIds.firstOrNull;
    return ListView(
      padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
      children: [
        const EkranBasligi('Kimden olsun?'),
        const SizedBox(height: OmvilaBosluk.bosluk4),
        if (draft.sameStaffForAll)
          ...options(
            draft.serviceIds,
            first == null ? null : draft.staff[first],
            chosen: draft.staffChosen,
            onChoose: controller.chooseStaff,
          ).separated()
        else
          for (final id in draft.serviceIds) ...[
            BolumBasligi(services[id]?.name ?? ''),
            ...options(
              [id],
              draft.staff[id],
              chosen: draft.staff.containsKey(id),
              onChoose: (staffId) => controller.chooseStaffFor(id, staffId),
            ).separated(),
          ],
        if (multiple) ...[
          const SizedBox(height: OmvilaBosluk.bosluk2),
          Material(
            color: r.zeminYukseltilmis,
            borderRadius: BorderRadius.circular(OmvilaKose.koseLg),
            clipBehavior: Clip.antiAlias,
            child: SwitchListTile(
              value: draft.sameStaffForAll,
              onChanged: (v) => controller.setSameStaff(value: v),
              title: Text(
                draft.serviceIds.length == 2
                    ? 'İki hizmet de aynı kişiden'
                    : 'Bütün hizmetler aynı kişiden',
                style: OmvilaYazi.govdeGuclu.copyWith(color: r.metin),
              ),
              subtitle: Text(
                'Kapatırsanız her hizmete ayrı kişi seçersiniz',
                style: OmvilaYazi.aciklama.copyWith(color: r.metinIkincil),
              ),
            ),
          ),
        ],
      ],
    );
  }
}

/// Alt çubuktaki kişi: "Elif Kaya", "Fark etmez"; ayrı kişilerde "2 kişi".
String kisiMetni(PublicCatalog catalog, BookingDraft draft) {
  final names = {
    for (final id in draft.serviceIds)
      if (draft.staff.containsKey(id))
        catalog.staff
                .where((s) => s.id == draft.staff[id])
                .firstOrNull
                ?.displayName ??
            'Fark etmez',
  };
  return switch (names.length) {
    0 => 'Kişi seçin',
    1 => names.single,
    _ => names.join(', '),
  };
}

extension on List<Widget> {
  List<Widget> separated() => [
    for (final w in this)
      Padding(
        padding: const EdgeInsets.only(bottom: OmvilaBosluk.bosluk2),
        child: w,
      ),
  ];
}

class _KisiSatiri extends StatelessWidget {
  const _KisiSatiri({
    required this.ad,
    required this.secili,
    required this.onTap,
    this.aciklama,
    this.ikon,
    this.fark = 0,
  });

  final String ad;
  final String? aciklama;
  final IconData? ikon;
  final int fark;
  final bool secili;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final note = aciklama;
    return SecimKarti(
      secili: secili,
      onTap: onTap,
      sag: fark > 0
          ? Text(
              '+${formatMoney(fark)}',
              style: OmvilaYazi.etiketKucuk.copyWith(color: r.metin),
            )
          : null,
      child: Row(
        children: [
          Container(
            width: 44,
            height: 44,
            alignment: Alignment.center,
            decoration: BoxDecoration(
              color: ikon == null ? r.avatar1Zemin : r.zeminIkincil,
              shape: ikon == null ? BoxShape.circle : BoxShape.rectangle,
              borderRadius: ikon == null
                  ? null
                  : BorderRadius.circular(OmvilaKose.koseMd),
            ),
            child: ikon == null
                ? Text(
                    initials(ad),
                    style: OmvilaYazi.etiket.copyWith(color: r.avatar1Metin),
                  )
                : Icon(ikon, size: 20, color: r.metin),
          ),
          const SizedBox(width: OmvilaBosluk.bosluk3),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(ad, style: OmvilaYazi.govdeGuclu.copyWith(color: r.metin)),
                if (note != null && note.isNotEmpty)
                  Text(
                    note,
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
