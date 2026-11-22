import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/app/providers.dart';
import 'package:omvila_musteri/app/router.dart';
import 'package:omvila_musteri/features/appointments/presentation/home_provider.dart';
import 'package:omvila_musteri/features/booking/domain/booking_draft.dart';
import 'package:omvila_musteri/features/booking/presentation/akis_sayfasi.dart';
import 'package:omvila_musteri/features/booking/presentation/booking_controller.dart';
import 'package:omvila_musteri/features/booking/presentation/personel_ekrani.dart';
import 'package:omvila_musteri/features/booking/presentation/salon_secimi_ekrani.dart';
import 'package:omvila_musteri/shared/widgets/durum_desenleri.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Profildeki ad (Onay'da ad alanı yalnız ad yoksa).
final FutureProvider<String?> profileNameProvider =
    FutureProvider.autoDispose<String?>(
      (ref) => ref.watch(bookingRepositoryProvider).profileName(),
    );

/// "Fön paketi" → "Fön paketinizden düşülsün".
String paketDusulsun(String packageName) {
  final name = packageName.trim();
  return name.toLowerCase().endsWith('paketi')
      ? '${name.substring(0, name.length - 1)}inizden düşülsün'
      : '$name paketinizden düşülsün';
}

/// Son kontrol (YROnay): özet, paketten düşme, kapora kutusu, salona not;
/// "Randevuyu oluştur". Uygulamadan ödeme alınmaz.
class OnayEkrani extends ConsumerStatefulWidget {
  /// Ekran.
  const OnayEkrani({super.key});

  @override
  ConsumerState<OnayEkrani> createState() => _OnayEkraniState();
}

class _OnayEkraniState extends ConsumerState<OnayEkrani> {
  final _note = TextEditingController();
  final _name = TextEditingController();
  String _key = newIdempotencyKey();
  DateTime? _keySlot;
  bool _sending = false;
  ApiProblem? _error;

  @override
  void dispose() {
    _note.dispose();
    _name.dispose();
    super.dispose();
  }

  Future<void> _create(BookingDraft draft, {required bool askName}) async {
    final slot = draft.slot!;
    if (_keySlot != slot.startUtc) {
      _key = newIdempotencyKey();
      _keySlot = slot.startUtc;
    }

    setState(() {
      _sending = true;
      _error = null;
    });
    final config = ref.read(configProvider);
    final note = _note.text.trim();
    try {
      final created = await ref
          .read(bookingRepositoryProvider)
          .create(
            draft.branchId,
            PublicAppointmentCreate(
              items: draft.items,
              sameStaffForAll: draft.sameStaffForAll,
              startUtc: slot.startUtc,
              // Ad profilden (sunucu da profildekini kullanır); yoksa bu
              // istekle profile yazılır.
              fullName: askName
                  ? _name.text.trim()
                  : (ref.read(profileNameProvider).value ?? ''),
              customerNote: note.isEmpty ? null : note,
              usePackageItems: [
                for (final MapEntry(:key, :value) in draft.packageItems.entries)
                  PublicAppointmentCreateUsePackageItemsInner(
                    serviceId: key,
                    customerPackageItemId: value,
                  ),
              ],
              // Aydınlatma metni girişte onaylandı (Giriş ekranı).
              privacyNoticeAcknowledged: true,
              privacyNoticeVersion: config.privacyNoticeVersion,
            ),
            idempotencyKey: _key,
          );
      if (!mounted) {
        return;
      }

      ref
        ..invalidate(homeProvider)
        ..invalidate(brandsProvider);
      final awaitingDeposit =
          created.deposit?.status == DepositStatus.awaiting &&
          created.deposit?.instructions != null;
      context.go(
        awaitingDeposit
            ? Routes.havale(created.id, yeni: true)
            : Routes.tamam(created.id),
      );
    } on ApiProblem catch (e) {
      if (mounted) {
        setState(() {
          _sending = false;
          _error = e;
        });
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final draft = ref.watch(bookingProvider)!;
    final catalog = ref.watch(catalogProvider(draft.branchId));
    final profile = ref.watch(profileNameProvider);
    final askName = profile.hasValue && profile.value == null;
    return AkisSayfasi(
      baslik: 'Son kontrol',
      adim: AkisAdimi.onay,
      body: switch ((catalog, profile)) {
        (AsyncData(value: final c), AsyncData()) => _body(
          context,
          c,
          draft,
          askName: askName,
        ),
        (AsyncError(:final error), _) ||
        (_, AsyncError(:final error)) => HataDurumu(
          error: error,
          onRetry: () => ref
            ..invalidate(catalogProvider(draft.branchId))
            ..invalidate(profileNameProvider),
        ),
        _ => const YukleniyorDurumu(),
      },
      alt: catalog.hasValue && profile.hasValue
          ? ListenableBuilder(
              listenable: _name,
              builder: (context, _) => AltOzetCubugu(
                eylem: 'Randevuyu oluştur',
                yukleniyor: _sending,
                onPressed: askName && _name.text.trim().length < 2
                    ? null
                    : () => _create(draft, askName: askName),
                not:
                    'Uygulamadan ödeme alınmaz; havaleyi kendi bankanızdan '
                    'yaparsınız.',
              ),
            )
          : null,
    );
  }

  Widget _body(
    BuildContext context,
    PublicCatalog catalog,
    BookingDraft draft, {
    required bool askName,
  }) {
    final r = context.omvilaRenkler;
    final slot = draft.slot!;
    final tz = catalog.timeZone;
    final summary = BookingSummary.of(catalog, draft);
    final packages =
        ref.watch(usablePackagesProvider(draft.branchId)).value ?? const [];
    final end = slot.items.isEmpty
        ? slot.startUtc.add(Duration(minutes: summary.durationMinutes))
        : slot.items
              .map((i) => i.endUtc)
              .reduce((a, b) => a.isAfter(b) ? a : b);
    final error = _error;
    final alternatives = [
      for (final v in (error?.meta['alternatives'] as List<dynamic>?) ?? [])
        if (v is String) DateTime.tryParse(v),
    ].nonNulls.toList();
    final staffNames = {
      for (final i in slot.items)
        if (i.staffName case final n?) n,
    };
    final staff = staffNames.isEmpty
        ? kisiMetni(catalog, draft)
        : staffNames.join(', ');
    return ListView(
      padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
      children: [
        _Kahraman(
          gun: toUpperTr(
            formatDate(
              toBranchDate(slot.startUtc, tz),
              DateStyle.weekdayDayMonth,
            ),
          ),
          saat: '${formatTime(slot.startUtc, tz)}–${formatTime(end, tz)}',
          alt: [draft.brandName, draft.branchName, staff].join(' · '),
        ),
        const SizedBox(height: OmvilaBosluk.bosluk3),
        _Ozet(catalog: catalog, draft: draft, summary: summary),
        for (final id in draft.serviceIds)
          if (packages.where((p) => p.serviceId == id).firstOrNull
              case final p?) ...[
            const SizedBox(height: OmvilaBosluk.bosluk3),
            _PaketAnahtari(
              paket: p,
              acik: draft.packageItems[id] == p.customerPackageItemId,
              onChanged: (on) => ref
                  .read(bookingProvider.notifier)
                  .usePackage(id, on ? p.customerPackageItemId : null),
            ),
          ],
        if (summary.depositMinor > 0) ...[
          const SizedBox(height: OmvilaBosluk.bosluk3),
          _KaporaKutusu(
            tutar: summary.depositMinor,
            salon: draft.brandName,
          ),
        ],
        if (error != null) ...[
          const SizedBox(height: OmvilaBosluk.bosluk3),
          BilgiKutusu(
            ikon: LucideIcons.alertCircle,
            metin: error.message,
            zemin: r.hataZemin,
            renk: r.hata,
          ),
          if (alternatives.isNotEmpty) ...[
            const SizedBox(height: OmvilaBosluk.bosluk2),
            Wrap(
              spacing: OmvilaBosluk.bosluk2,
              runSpacing: OmvilaBosluk.bosluk2,
              children: [
                for (final t in alternatives)
                  ActionChip(
                    label: Text(formatTime(t, tz)),
                    onPressed: () {
                      ref
                          .read(bookingProvider.notifier)
                          .chooseSlot(_shift(slot, t, tz));
                      setState(() => _error = null);
                    },
                  ),
              ],
            ),
          ],
        ],
        if (askName) ...[
          const SizedBox(height: OmvilaBosluk.bosluk4),
          TextField(
            controller: _name,
            textCapitalization: TextCapitalization.words,
            autofillHints: const [AutofillHints.name],
            maxLength: 120,
            decoration: const InputDecoration(
              labelText: 'Adınız soyadınız',
              counterText: '',
            ),
          ),
        ],
        const SizedBox(height: OmvilaBosluk.bosluk4),
        Text(
          'Salona not (isteğe bağlı)',
          style: OmvilaYazi.etiket.copyWith(color: r.metin),
        ),
        const SizedBox(height: OmvilaBosluk.bosluk2),
        TextField(
          controller: _note,
          maxLength: 500,
          maxLines: null,
          decoration: const InputDecoration(
            hintText: 'Örn. geçen seferki renk',
            counterText: '',
          ),
        ),
      ],
    );
  }

  /// Alternatif saate kaydırılmış saat (kalemler aynı süreyle).
  static Slot _shift(Slot slot, DateTime startUtc, String timeZone) {
    final delta = startUtc.difference(slot.startUtc);
    return Slot(
      startUtc: startUtc,
      localTime: formatTime(startUtc, timeZone),
      period: slot.period,
      total: slot.total,
      items: [
        for (final i in slot.items)
          SlotItemsInner(
            serviceId: i.serviceId,
            staffId: i.staffId,
            staffName: i.staffName,
            startUtc: i.startUtc.add(delta),
            endUtc: i.endUtc.add(delta),
            price: i.price,
          ),
      ],
    );
  }
}

class _Kahraman extends StatelessWidget {
  const _Kahraman({required this.gun, required this.saat, required this.alt});

  final String gun;
  final String saat;
  final String alt;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final onCard = r.metinTers;
    return Container(
      padding: const EdgeInsets.all(OmvilaBosluk.bosluk5),
      decoration: BoxDecoration(
        color: r.blokMarka,
        borderRadius: BorderRadius.circular(OmvilaKose.koseXl),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            gun,
            style: OmvilaYazi.aciklama.copyWith(
              color: onCard,
              fontWeight: FontWeight.w700,
              letterSpacing: 0.6,
            ),
          ),
          const SizedBox(height: OmvilaBosluk.bosluk1),
          Text(saat, style: OmvilaYazi.metrik.copyWith(color: onCard)),
          Text(
            alt,
            style: OmvilaYazi.ikincil.copyWith(
              color: onCard.withValues(alpha: 0.85),
            ),
          ),
        ],
      ),
    );
  }
}

/// Satır listesi kartı (Onay özeti, randevu detayı kalemleri).
class SatirKarti extends StatelessWidget {
  /// Kart; [satirlar] (etiket, değer, vurgulu).
  const SatirKarti({required this.satirlar, super.key});

  /// Satırlar.
  final List<(String, String, bool)> satirlar;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    return DecoratedBox(
      decoration: BoxDecoration(
        color: r.zeminYukseltilmis,
        borderRadius: BorderRadius.circular(OmvilaKose.koseLg),
        boxShadow: context.omvilaGolgeler.golgeSm,
      ),
      child: Padding(
        padding: const EdgeInsets.symmetric(horizontal: OmvilaBosluk.bosluk4),
        child: Column(
          children: [
            for (final (i, (label, value, strong)) in satirlar.indexed) ...[
              if (i > 0) Divider(height: 1, color: r.cizgi),
              Padding(
                padding: const EdgeInsets.symmetric(
                  vertical: OmvilaBosluk.bosluk3,
                ),
                child: Row(
                  children: [
                    Expanded(
                      child: Text(
                        label,
                        style:
                            (strong ? OmvilaYazi.govdeGuclu : OmvilaYazi.govde)
                                .copyWith(
                                  color: strong ? r.metin : r.metinIkincil,
                                ),
                      ),
                    ),
                    const SizedBox(width: OmvilaBosluk.bosluk3),
                    Text(
                      value,
                      style: OmvilaYazi.govdeGuclu.copyWith(color: r.metin),
                    ),
                  ],
                ),
              ),
            ],
          ],
        ),
      ),
    );
  }
}

class _Ozet extends StatelessWidget {
  const _Ozet({
    required this.catalog,
    required this.draft,
    required this.summary,
  });

  final PublicCatalog catalog;
  final BookingDraft draft;
  final BookingSummary summary;

  @override
  Widget build(BuildContext context) {
    final rows = <(String, String, bool)>[
      for (final s in summary.services)
        (
          s.name,
          draft.packageItems.containsKey(s.id)
              ? 'Paketten'
              : s.price == null
              ? ''
              : formatMoney(
                  s.price!.amountMinor +
                      BookingSummary.surchargeOf(
                        catalog,
                        draft.staff[s.id],
                        s.id,
                      ),
                ),
          false,
        ),
      if (summary.depositMinor > 0)
        (
          'Kapora (havale ile)',
          formatMoney(-summary.depositMinor),
          false,
        ),
      if (!summary.pricesHidden)
        (
          'Salonda ödenecek',
          formatPrice(
            summary.totalMinor - summary.depositMinor,
            display: summary.startingFrom
                ? PriceDisplay.startingFrom
                : PriceDisplay.fixed,
          ),
          true,
        ),
    ];
    return SatirKarti(satirlar: rows);
  }
}

class _PaketAnahtari extends StatelessWidget {
  const _PaketAnahtari({
    required this.paket,
    required this.acik,
    required this.onChanged,
  });

  final UsablePackageItem paket;
  final bool acik;
  final ValueChanged<bool> onChanged;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final p = paket;
    return DecoratedBox(
      decoration: BoxDecoration(
        color: r.zeminYukseltilmis,
        borderRadius: BorderRadius.circular(OmvilaKose.koseLg),
        boxShadow: context.omvilaGolgeler.golgeSm,
      ),
      child: SwitchListTile(
        value: acik,
        onChanged: onChanged,
        secondary: Container(
          width: 40,
          height: 40,
          decoration: BoxDecoration(
            color: r.blokKum,
            borderRadius: BorderRadius.circular(OmvilaKose.koseSm),
          ),
          child: Icon(LucideIcons.package, size: 20, color: r.blokKumMetin),
        ),
        title: Text(
          paketDusulsun(p.packageName),
          style: OmvilaYazi.govdeGuclu.copyWith(color: r.metin),
        ),
        subtitle: Text(
          acik
              ? '${p.totalSessions} seansın '
                    '${sayiIyelik(p.remainingSessions - 1)} kalacak'
              : '${p.totalSessions} seansın '
                    '${sayiIyelik(p.remainingSessions)} kaldı',
          style: OmvilaYazi.aciklama.copyWith(color: r.metinIkincil),
        ),
      ),
    );
  }
}

class _KaporaKutusu extends StatelessWidget {
  const _KaporaKutusu({required this.tutar, required this.salon});

  final int tutar;
  final String salon;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final amount = formatMoney(tutar);
    return DecoratedBox(
      decoration: BoxDecoration(
        color: r.uyariZemin,
        borderRadius: BorderRadius.circular(OmvilaKose.koseLg),
      ),
      child: Padding(
        padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Expanded(
                  child: Text(
                    'Kapora',
                    style: OmvilaYazi.baslik3.copyWith(color: r.metin),
                  ),
                ),
                Text(
                  amount,
                  style: OmvilaYazi.baslik2.copyWith(color: r.metin),
                ),
              ],
            ),
            const SizedBox(height: OmvilaBosluk.bosluk1),
            Text(
              'Bu randevu $amount kapora ile kesinleşir. Randevuyu '
              'oluşturunca $salon salonunun IBAN bilgisini göstereceğiz; '
              'havaleyi belirtilen süre içinde yapmanız gerekir. Kapora '
              'salondaki tutardan düşülür. Kesin tutar randevu oluşunca '
              'gösterilir.',
              style: OmvilaYazi.ikincil.copyWith(color: r.metin),
            ),
          ],
        ),
      ),
    );
  }
}
