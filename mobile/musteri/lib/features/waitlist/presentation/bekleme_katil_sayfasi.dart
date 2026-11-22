import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/app/providers.dart';
import 'package:omvila_musteri/features/appointments/presentation/randevu_sayfalari.dart';
import 'package:omvila_musteri/features/booking/domain/booking_draft.dart';
import 'package:omvila_musteri/features/booking/presentation/akis_sayfasi.dart';
import 'package:omvila_musteri/features/booking/presentation/booking_controller.dart';
import 'package:omvila_musteri/features/booking/presentation/hizmet_ekrani.dart';
import 'package:omvila_musteri/features/booking/presentation/onay_ekrani.dart';
import 'package:omvila_musteri/features/booking/presentation/personel_ekrani.dart';
import 'package:omvila_musteri/shared/widgets/durum_desenleri.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Gün çiplerinin sayısı (bugünden itibaren).
const waitlistDayCount = 7;

/// "Uygun saat yok mu?" → BeklemeKatil alt sayfası; katıldıysa true.
Future<bool?> beklemeKatilGoster(BuildContext context) =>
    showModalBottomSheet<bool>(
      context: context,
      isScrollControlled: true,
      showDragHandle: true,
      backgroundColor: context.omvilaRenkler.zeminYukseltilmis,
      builder: (_) => const BeklemeKatilSayfasi(),
    );

/// Bekleme listesine katılın (BeklemeKatil): gün ve saat dilimi çipleri
/// (hiçbiri seçilmezse her gün / her saat uyar), tek kişi seçildiyse "Başka
/// personel de olur"; "Listeye katıl" → publicJoinWaitlist (müşteri
/// jetonuyla). Tutma süresi sözleşmede olmadığı için sayı yazılmaz.
class BeklemeKatilSayfasi extends ConsumerStatefulWidget {
  /// Sayfa.
  const BeklemeKatilSayfasi({super.key});

  @override
  ConsumerState<BeklemeKatilSayfasi> createState() =>
      _BeklemeKatilSayfasiState();
}

class _BeklemeKatilSayfasiState extends ConsumerState<BeklemeKatilSayfasi> {
  static const List<(WaitlistTimeWindow, String)> _windows = [
    (WaitlistTimeWindow.morning, 'Sabah'),
    (WaitlistTimeWindow.afternoon, 'Öğleden sonra'),
    (WaitlistTimeWindow.evening, 'Akşam'),
  ];

  late final Set<String> _dates = {?ref.read(bookingProvider)?.date};
  final _windowsChosen = <WaitlistTimeWindow>{};
  final _name = TextEditingController();
  bool _otherStaff = false;
  bool _sending = false;
  ApiProblem? _error;
  String? _phone;
  String _key = newIdempotencyKey();

  @override
  void dispose() {
    _name.dispose();
    super.dispose();
  }

  String? _preferredStaff(BookingDraft d) {
    final ids = {for (final id in d.serviceIds) d.staff[id]};
    return ids.length == 1 ? ids.single : null;
  }

  Future<void> _join(BookingDraft draft, {required bool askName}) async {
    setState(() {
      _sending = true;
      _error = null;
    });
    final staff = _preferredStaff(draft);
    try {
      await ref
          .read(waitlistRepositoryProvider)
          .join(
            draft.branchId,
            PublicWaitlistJoin(
              preferences: WaitlistPreferences(
                serviceIds: draft.serviceIds,
                preferredStaffId: staff,
                acceptOtherStaff: staff == null || _otherStaff,
                dates: [
                  for (final d in _dates.toList()..sort()) parseLocalDate(d),
                ],
                timeWindows: [
                  for (final (w, _) in _windows)
                    if (_windowsChosen.contains(w)) w,
                ],
              ),
              fullName: askName
                  ? _name.text.trim()
                  : (ref.read(profileNameProvider).value ?? ''),
              // Aydınlatma metni girişte onaylandı (Giriş ekranı).
              privacyNoticeAcknowledged: true,
              privacyNoticeVersion: ref
                  .read(configProvider)
                  .privacyNoticeVersion,
            ),
            idempotencyKey: _key,
          );
      if (mounted) {
        Navigator.of(context).pop(true);
      }
    } on ApiProblem catch (e) {
      String? phone;
      if (isBookingDisabled(e)) {
        phone = await _branchPhone(draft);
      }

      if (mounted) {
        setState(() {
          _sending = false;
          _error = e;
          _phone = phone;
          _key = newIdempotencyKey();
        });
      }
    }
  }

  Future<String?> _branchPhone(BookingDraft draft) async {
    try {
      final brand = await ref
          .read(bookingRepositoryProvider)
          .brand(draft.brandSlug);
      return brand.branches
          .where((b) => b.id == draft.branchId)
          .firstOrNull
          ?.phone;
    } on ApiProblem {
      return null;
    }
  }

  void _toggle<T>(Set<T> set, T value) =>
      setState(() => set.contains(value) ? set.remove(value) : set.add(value));

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final draft = ref.watch(bookingProvider);
    if (draft == null) {
      return const SizedBox.shrink();
    }

    final catalog = ref.watch(catalogProvider(draft.branchId));
    final profile = ref.watch(profileNameProvider);
    final c = catalog.value;
    if (c == null || !profile.hasValue) {
      return SizedBox(
        height: 240,
        child: catalog.hasError || profile.hasError
            ? HataDurumu(
                error: catalog.error ?? profile.error!,
                onRetry: () => ref
                  ..invalidate(catalogProvider(draft.branchId))
                  ..invalidate(profileNameProvider),
              )
            : const YukleniyorDurumu(),
      );
    }

    final askName = profile.value == null;
    final today = branchToday(ref.watch(clockProvider), c.timeZone);
    final services = [
      for (final id in draft.serviceIds)
        if (c.services.where((s) => s.id == id).firstOrNull case final s?)
          s.name,
    ];
    final staffId = _preferredStaff(draft);
    final error = _error;
    final phone = _phone;
    Widget label(String text) => Padding(
      padding: const EdgeInsets.only(
        top: OmvilaBosluk.bosluk4,
        bottom: OmvilaBosluk.bosluk2,
      ),
      child: Text(text, style: OmvilaYazi.etiket.copyWith(color: r.metin)),
    );

    return AltSayfa(
      ikon: LucideIcons.listChecks,
      ikonZemin: r.blokKum,
      ikonRenk: r.blokKumMetin,
      baslik: 'Bekleme listesine katılın',
      aciklama: [
        services.join(', '),
        if (staffId != null) kisiMetni(c, draft),
        draft.brandName,
      ].where((p) => p.isNotEmpty).join(' · '),
      children: [
        label('Hangi günler?'),
        Wrap(
          spacing: OmvilaBosluk.bosluk2,
          runSpacing: OmvilaBosluk.bosluk2,
          children: [
            for (var i = 0; i < waitlistDayCount; i++)
              if (addDays(today, i) case final day)
                SecimCipi(
                  etiket: formatDate(
                    day,
                    DateStyle.compact,
                  ).split(' ').take(2).join(' '),
                  secili: _dates.contains(day),
                  onTap: () => _toggle(_dates, day),
                ),
          ],
        ),
        label('Hangi saatler?'),
        Wrap(
          spacing: OmvilaBosluk.bosluk2,
          runSpacing: OmvilaBosluk.bosluk2,
          children: [
            for (final (w, text) in _windows)
              SecimCipi(
                etiket: text,
                secili: _windowsChosen.contains(w),
                onTap: () => _toggle(_windowsChosen, w),
              ),
          ],
        ),
        if (staffId != null)
          SwitchListTile(
            contentPadding: EdgeInsets.zero,
            value: _otherStaff,
            onChanged: (v) => setState(() => _otherStaff = v),
            title: Text(
              'Başka personel de olur',
              style: OmvilaYazi.govdeGuclu.copyWith(color: r.metin),
            ),
          ),
        if (askName) ...[
          label('Adınız soyadınız'),
          TextField(
            controller: _name,
            textCapitalization: TextCapitalization.words,
            autofillHints: const [AutofillHints.name],
            onChanged: (_) => setState(() {}),
          ),
        ],
        const SizedBox(height: OmvilaBosluk.bosluk4),
        BilgiKutusu(
          ikon: LucideIcons.timer,
          metin:
              'Uygun bir saat boşalınca bildirim ve SMS ile haber veririz. '
              'Saat kısa bir süre sizin için tutulur; süreyi mesajda '
              'görürsünüz. Gün veya saat seçmezseniz hepsi uyar sayılır.',
          renk: r.metin,
        ),
        if (error != null) ...[
          const SizedBox(height: OmvilaBosluk.bosluk4),
          if (isBookingDisabled(error))
            OnlineRandevuKapaliKutusu(
              salonAdi: draft.brandName,
              onAra: phone == null ? null : () => salonuAra(ref, phone),
            )
          else
            BilgiKutusu(
              ikon: LucideIcons.alertCircle,
              metin: error.message,
              zemin: r.hataZemin,
              renk: r.hata,
            ),
        ],
        const SizedBox(height: OmvilaBosluk.bosluk5),
        OmvilaButon(
          etiket: 'Listeye katıl',
          yukleniyorEtiketi: 'Ekleniyor…',
          boyut: OmvilaButonBoyutu.lg,
          tamGenislik: true,
          yukleniyor: _sending,
          onPressed: askName && _name.text.trim().length < 2
              ? null
              : () => _join(draft, askName: askName),
        ),
        TextButton(
          onPressed: () => Navigator.of(context).pop(false),
          child: const Text('Vazgeç'),
        ),
      ],
    );
  }
}
