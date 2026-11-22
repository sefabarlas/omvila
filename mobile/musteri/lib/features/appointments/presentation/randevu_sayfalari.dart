import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/app/providers.dart';
import 'package:omvila_musteri/features/appointments/presentation/appointment_providers.dart';
import 'package:omvila_musteri/features/booking/presentation/akis_sayfasi.dart';
import 'package:omvila_musteri/features/booking/presentation/hizmet_ekrani.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Salonu arar (şube telefonu).
Future<void> salonuAra(WidgetRef ref, String phone) =>
    ref.read(externalActionsProvider).openUrl(Uri(scheme: 'tel', path: phone));

/// Alt sayfa iskeleti: ikon kutusu, başlık, açıklama, içerik.
class AltSayfa extends StatelessWidget {
  /// Sayfa.
  const AltSayfa({
    required this.ikon,
    required this.ikonZemin,
    required this.ikonRenk,
    required this.baslik,
    required this.aciklama,
    required this.children,
    super.key,
  });

  /// İkon.
  final IconData ikon;

  /// İkon kutusu zemini.
  final Color ikonZemin;

  /// İkon rengi.
  final Color ikonRenk;

  /// Başlık.
  final String baslik;

  /// Açıklama.
  final String aciklama;

  /// Alt içerik.
  final List<Widget> children;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    return SafeArea(
      child: SingleChildScrollView(
        padding: EdgeInsets.fromLTRB(
          OmvilaBosluk.bosluk5,
          0,
          OmvilaBosluk.bosluk5,
          OmvilaBosluk.bosluk5 + MediaQuery.viewInsetsOf(context).bottom,
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Align(
              alignment: Alignment.centerLeft,
              child: Container(
                width: 56,
                height: 56,
                decoration: BoxDecoration(
                  color: ikonZemin,
                  borderRadius: BorderRadius.circular(OmvilaKose.koseLg),
                ),
                child: Icon(ikon, color: ikonRenk),
              ),
            ),
            const SizedBox(height: OmvilaBosluk.bosluk4),
            Semantics(
              header: true,
              child: Text(
                baslik,
                style: OmvilaYazi.baslik1.copyWith(color: r.metin),
              ),
            ),
            const SizedBox(height: OmvilaBosluk.bosluk2),
            Text(
              aciklama,
              style: OmvilaYazi.ikincil.copyWith(color: r.metinIkincil),
            ),
            const SizedBox(height: OmvilaBosluk.bosluk4),
            ...children,
          ],
        ),
      ),
    );
  }
}

Future<T?> _sheet<T>(BuildContext context, Widget child) =>
    showModalBottomSheet<T>(
      context: context,
      isScrollControlled: true,
      showDragHandle: true,
      backgroundColor: context.omvilaRenkler.zeminYukseltilmis,
      builder: (_) => child,
    );

/// Son süre geçti (ErteleKapali): "Salonu ara" veya "Kapat".
Future<void> gecKaldinizGoster(
  BuildContext context,
  MeAppointment a, {
  required bool iptal,
}) => _sheet<void>(context, GecKaldinizSayfasi(randevu: a, iptal: iptal));

/// İptal sayfası (Iptal); iptal edildiyse true.
Future<bool?> iptalGoster(BuildContext context, MeAppointment a) =>
    _sheet<bool>(context, IptalSayfasi(randevu: a));

String _kabulMetni(String salon, int saat, {required bool iptal}) {
  final what = iptal ? 'iptal' : 'değişiklik';
  return '$salon, randevudan $saat saat öncesine kadar $what kabul ediyor.';
}

/// ErteleKapali.
class GecKaldinizSayfasi extends ConsumerWidget {
  /// Sayfa; [iptal] true ise iptal son süresi.
  const GecKaldinizSayfasi({
    required this.randevu,
    required this.iptal,
    super.key,
  });

  /// Randevu.
  final MeAppointment randevu;

  /// İptal için mi.
  final bool iptal;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final r = context.omvilaRenkler;
    final a = randevu;
    final cancel = deadlineHours(a, a.cancelDeadlineUtc);
    final change = deadlineHours(a, a.rescheduleDeadlineUtc);
    final limit = iptal ? cancel : change;
    final left = a.startUtc.difference(ref.watch(clockProvider).now()).inHours;
    final phone = a.branch.phone;
    return AltSayfa(
      ikon: LucideIcons.clockAlert,
      ikonZemin: r.uyariZemin,
      ikonRenk: r.uyari,
      baslik: iptal
          ? 'İptal için geç kaldınız'
          : 'Saat değişikliği için geç kaldınız',
      aciklama: [
        if (limit != null) _kabulMetni(a.brand.name, limit, iptal: iptal),
        if (left >= 1)
          'Randevunuza $left saat kaldı.'
        else
          'Randevunuza bir saatten az kaldı.',
      ].join(' '),
      children: [
        if (cancel != null || change != null)
          DecoratedBox(
            decoration: BoxDecoration(
              color: r.zeminIkincil,
              borderRadius: BorderRadius.circular(OmvilaKose.koseLg),
            ),
            child: Padding(
              padding: const EdgeInsets.symmetric(
                horizontal: OmvilaBosluk.bosluk4,
              ),
              child: Column(
                children: [
                  for (final (i, (label, hours)) in [
                    if (cancel != null) ('İptal', cancel),
                    if (change != null) ('Saat değişikliği', change),
                  ].indexed) ...[
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
                              style: OmvilaYazi.govde.copyWith(
                                color: r.metinIkincil,
                              ),
                            ),
                          ),
                          Text(
                            '$hours saat önceye kadar',
                            style: OmvilaYazi.govdeGuclu.copyWith(
                              color: r.metin,
                            ),
                          ),
                        ],
                      ),
                    ),
                  ],
                ],
              ),
            ),
          ),
        const SizedBox(height: OmvilaBosluk.bosluk5),
        if (phone != null)
          OmvilaButon(
            etiket: 'Salonu ara',
            ikon: LucideIcons.phone,
            boyut: OmvilaButonBoyutu.lg,
            tamGenislik: true,
            onPressed: () => salonuAra(ref, phone),
          ),
        TextButton(
          onPressed: () => Navigator.of(context).pop(),
          child: const Text('Kapat'),
        ),
      ],
    );
  }
}

/// Randevu iptal edilsin mi? (Iptal): neden zorunlu.
class IptalSayfasi extends ConsumerStatefulWidget {
  /// Sayfa.
  const IptalSayfasi({required this.randevu, super.key});

  /// Randevu.
  final MeAppointment randevu;

  @override
  ConsumerState<IptalSayfasi> createState() => _IptalSayfasiState();
}

class _IptalSayfasiState extends ConsumerState<IptalSayfasi> {
  // Tasarımdaki "Hastayım" yerine "Rahatsızlandım" (sağlık bilgisi izlenimi
  // vermemek için; sözleşme meCancelAppointment).
  static const List<(CustomerCancelRequestReasonEnum, String)> _reasons = [
    (CustomerCancelRequestReasonEnum.planChanged, 'Planım değişti'),
    (CustomerCancelRequestReasonEnum.sick, 'Rahatsızlandım'),
    (
      CustomerCancelRequestReasonEnum.wantsDifferentTime,
      'Başka saat istiyorum',
    ),
  ];

  CustomerCancelRequestReasonEnum? _reason;
  bool _sending = false;
  ApiProblem? _error;

  Future<void> _cancel() async {
    setState(() {
      _sending = true;
      _error = null;
    });
    try {
      await ref
          .read(appointmentsRepositoryProvider)
          .cancel(widget.randevu, _reason!);
      if (mounted) {
        Navigator.of(context).pop(true);
      }
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
    final r = context.omvilaRenkler;
    final a = widget.randevu;
    final deposit = a.deposit;
    final paid =
        deposit != null &&
        (deposit.status == DepositStatus.received ||
            deposit.status == DepositStatus.customerReported);
    final error = _error;
    return AltSayfa(
      ikon: LucideIcons.calendarX,
      ikonZemin: r.hataZemin,
      ikonRenk: r.hata,
      baslik: 'Randevu iptal edilsin mi?',
      aciklama:
          '${randevuKisaMetni(a, ref.watch(clockProvider))}. Saatiniz başka '
          'bir müşteriye açılır.',
      children: [
        if (paid) ...[
          BilgiKutusu(
            metin:
                '${formatMoney(deposit.required_.amountMinor)} kaporanın '
                'iadesi salonun iade koşullarına bağlıdır. Salon sizinle '
                'iletişime geçecek.',
            zemin: r.uyariZemin,
            renk: r.metin,
          ),
          const SizedBox(height: OmvilaBosluk.bosluk4),
        ],
        Text.rich(
          TextSpan(
            text: 'İptal nedeni ',
            children: [
              TextSpan(
                text: '*',
                style: TextStyle(color: r.hata),
              ),
            ],
          ),
          style: OmvilaYazi.etiket.copyWith(color: r.metin),
        ),
        const SizedBox(height: OmvilaBosluk.bosluk2),
        Wrap(
          spacing: OmvilaBosluk.bosluk2,
          runSpacing: OmvilaBosluk.bosluk2,
          children: [
            for (final (reason, label) in _reasons)
              SecimCipi(
                etiket: label,
                secili: _reason == reason,
                onTap: () => setState(() => _reason = reason),
              ),
          ],
        ),
        if (error != null) ...[
          const SizedBox(height: OmvilaBosluk.bosluk4),
          BilgiKutusu(
            ikon: LucideIcons.alertCircle,
            metin: error.message,
            zemin: r.hataZemin,
            renk: r.hata,
          ),
        ],
        const SizedBox(height: OmvilaBosluk.bosluk5),
        OmvilaButon(
          etiket: 'İptal et',
          tur: OmvilaButonTuru.tehlike,
          boyut: OmvilaButonBoyutu.lg,
          tamGenislik: true,
          yukleniyor: _sending,
          onPressed: _reason == null ? null : _cancel,
        ),
        TextButton(
          onPressed: () => Navigator.of(context).pop(false),
          child: const Text('Vazgeç'),
        ),
      ],
    );
  }
}
