import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/app/providers.dart';
import 'package:omvila_musteri/app/router.dart';
import 'package:omvila_musteri/features/appointments/presentation/appointment_providers.dart';
import 'package:omvila_musteri/features/appointments/presentation/randevu_detay_ekrani.dart';
import 'package:omvila_musteri/features/booking/presentation/akis_sayfasi.dart';
import 'package:omvila_musteri/shared/widgets/durum_desenleri.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Kalan süre: "1 sa 58 dk kaldı", "12 dk kaldı"; dolduysa null.
String? kalanSure(DateTime dueUtc, Clock clock) {
  final minutes = dueUtc.difference(clock.now()).inSeconds;
  if (minutes <= 0) {
    return null;
  }

  return '${formatDuration((minutes / 60).ceil())} kaldı';
}

/// Kaporayı havale ile gönderin (YRHavale, Dilim 4): geri sayım, dört
/// kopyalanabilir alan, "Havaleyi yaptım, salona bildir". Bildirimden sonra
/// "Salon onayı bekleniyor" (S-03); "onaylandı" yalnız salon onaylayınca.
class HavaleEkrani extends ConsumerStatefulWidget {
  /// Ekran; [yeni] randevu az önce oluşturuldu.
  const HavaleEkrani({required this.id, this.yeni = false, super.key});

  /// Randevu.
  final String id;

  /// Akıştan geliyor ("Daha sonra" başarı ekranına gider).
  final bool yeni;

  @override
  ConsumerState<HavaleEkrani> createState() => _HavaleEkraniState();
}

class _HavaleEkraniState extends ConsumerState<HavaleEkrani> {
  Timer? _tick;
  bool _sending = false;

  @override
  void initState() {
    super.initState();
    _tick = Timer.periodic(const Duration(seconds: 30), (_) {
      if (mounted) {
        setState(() {});
      }
    });
  }

  @override
  void dispose() {
    _tick?.cancel();
    super.dispose();
  }

  void _done() => widget.yeni
      ? context.go(Routes.tamam(widget.id))
      : (context.canPop()
            ? context.pop()
            : context.go(Routes.randevu(widget.id)));

  Future<void> _report() async {
    setState(() => _sending = true);
    final messenger = ScaffoldMessenger.of(context);
    try {
      await ref.read(appointmentsRepositoryProvider).reportDeposit(widget.id);
      if (!mounted) {
        return;
      }

      tazele(ref, widget.id);
      messenger.showSnackBar(
        const SnackBar(content: Text('Havaleniz salona bildirildi.')),
      );
      _done();
    } on ApiProblem catch (e) {
      messenger.showSnackBar(SnackBar(content: Text(e.message)));
      if (mounted) {
        setState(() => _sending = false);
        ref.invalidate(appointmentProvider(widget.id));
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final appointment = ref.watch(appointmentProvider(widget.id));
    final a = appointment.value;
    final instructions = a?.deposit?.instructions;
    final reported = a?.deposit?.status == DepositStatus.customerReported;
    return AkisSayfasi(
      baslik: 'Kapora',
      onKapat: () => context.go(Routes.randevularim),
      body: switch (appointment) {
        AsyncData(:final value) when instructions != null => _body(
          context,
          value,
          instructions,
          reported: reported,
        ),
        AsyncData() => Center(
          child: SingleChildScrollView(
            child: OmvilaBosDurum(
              ikon: LucideIcons.badgeCheck,
              baslik: 'Kapora beklenmiyor',
              aciklama: 'Bu randevu için havale bilgisi gösterilmiyor.',
              eylemEtiketi: 'Randevuya dön',
              onEylem: () => context.go(Routes.randevu(widget.id)),
            ),
          ),
        ),
        AsyncError(:final error) => HataDurumu(
          error: error,
          onRetry: () => ref.invalidate(appointmentProvider(widget.id)),
        ),
        _ => const YukleniyorDurumu(),
      },
      alt: instructions == null
          ? null
          : SafeArea(
              top: false,
              child: Padding(
                padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
                child: Column(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    if (!reported)
                      OmvilaButon(
                        etiket: 'Havaleyi yaptım, salona bildir',
                        boyut: OmvilaButonBoyutu.lg,
                        tamGenislik: true,
                        yukleniyor: _sending,
                        onPressed: _report,
                      ),
                    TextButton(
                      onPressed: _done,
                      child: Text(reported ? 'Tamam' : 'Daha sonra'),
                    ),
                  ],
                ),
              ),
            ),
    );
  }

  Widget _body(
    BuildContext context,
    MeAppointment a,
    DepositInstructions d, {
    required bool reported,
  }) {
    final r = context.omvilaRenkler;
    final clock = ref.watch(clockProvider);
    final left = kalanSure(d.dueUtc, clock);
    final onCard = r.metinTers;
    return ListView(
      padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
      children: [
        Container(
          padding: const EdgeInsets.all(OmvilaBosluk.bosluk5),
          decoration: BoxDecoration(
            color: reported ? r.blokMarka : r.blokVurgu,
            borderRadius: BorderRadius.circular(OmvilaKose.koseXl),
          ),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  Expanded(
                    child: Text(
                      reported ? 'SALON ONAYI BEKLENİYOR' : 'KAPORA BEKLENİYOR',
                      style: OmvilaYazi.aciklama.copyWith(
                        color: onCard,
                        fontWeight: FontWeight.w700,
                        letterSpacing: 0.6,
                      ),
                    ),
                  ),
                  if (left != null && !reported) ...[
                    Icon(LucideIcons.timer, size: 16, color: onCard),
                    const SizedBox(width: OmvilaBosluk.bosluk1),
                    Text(
                      left,
                      style: OmvilaYazi.aciklama.copyWith(
                        color: onCard,
                        fontWeight: FontWeight.w700,
                      ),
                    ),
                  ],
                ],
              ),
              const SizedBox(height: OmvilaBosluk.bosluk2),
              Text(
                formatMoney(d.amount.amountMinor),
                style: OmvilaYazi.metrik.copyWith(color: onCard, fontSize: 48),
              ),
              Text(
                [
                  formatDate(
                    toBranchDate(a.startUtc, a.timeZone),
                    DateStyle.compact,
                  ),
                  formatTime(a.startUtc, a.timeZone),
                  a.brand.name,
                ].join(' · '),
                style: OmvilaYazi.ikincil.copyWith(color: onCard),
              ),
            ],
          ),
        ),
        const SizedBox(height: OmvilaBosluk.bosluk5),
        EkranBasligi(
          ikincil: true,
          reported
              ? 'Havaleniz salona bildirildi'
              : 'Kaporayı havale ile gönderin',
          aciklama: reported
              ? 'Salon havaleyi görünce onaylar, size SMS gelir. Bu arada '
                    'bilgileri yeniden kopyalayabilirsiniz.'
              : 'Kendi bankanızdan aşağıdaki hesaba gönderin. Açıklamaya '
                    'kodu yazmayı unutmayın; salon havalenizi bu kodla '
                    'eşleştirir.',
        ),
        const SizedBox(height: OmvilaBosluk.bosluk4),
        DecoratedBox(
          decoration: BoxDecoration(
            color: r.zeminYukseltilmis,
            borderRadius: BorderRadius.circular(OmvilaKose.koseLg),
            boxShadow: context.omvilaGolgeler.golgeSm,
          ),
          child: Padding(
            padding: const EdgeInsets.symmetric(
              horizontal: OmvilaBosluk.bosluk4,
            ),
            child: Column(
              children: [
                for (final (i, (label, shown, copy)) in [
                  ('Alıcı', d.accountHolderName, d.accountHolderName),
                  ('IBAN', formatIban(d.iban), normalizeIban(d.iban)),
                  ('Açıklama kodu', d.transferCode, d.transferCode),
                  (
                    'Tutar',
                    formatMoney(d.amount.amountMinor),
                    moneyInputValue(d.amount.amountMinor),
                  ),
                ].indexed) ...[
                  if (i > 0) Divider(height: 1, color: r.cizgi),
                  KopyalanabilirAlan(etiket: label, deger: shown, kopya: copy),
                ],
              ],
            ),
          ),
        ),
        const SizedBox(height: OmvilaBosluk.bosluk4),
        const BilgiKutusu(
          metin:
              'Salon havaleyi görünce onaylar, size SMS gelir. Süre içinde '
              'gelmezse randevu otomatik iptal olur ve saat başkasına açılır.',
        ),
      ],
    );
  }
}

/// Kopyalanabilir alan (Alıcı, IBAN, Açıklama kodu, Tutar).
class KopyalanabilirAlan extends StatelessWidget {
  /// Alan; [kopya] panoya yazılan değer (IBAN boşluksuz).
  const KopyalanabilirAlan({
    required this.etiket,
    required this.deger,
    required this.kopya,
    super.key,
  });

  /// "IBAN".
  final String etiket;

  /// Gösterilen.
  final String deger;

  /// Kopyalanan.
  final String kopya;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: OmvilaBosluk.bosluk3),
      child: Row(
        children: [
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  etiket,
                  style: OmvilaYazi.aciklama.copyWith(color: r.metinIkincil),
                ),
                Text(
                  deger,
                  style: OmvilaYazi.govdeGuclu.copyWith(color: r.metin),
                ),
              ],
            ),
          ),
          const SizedBox(width: OmvilaBosluk.bosluk2),
          Semantics(
            label: '$etiket kopyala',
            button: true,
            excludeSemantics: true,
            child: FilledButton.icon(
              onPressed: () async {
                final messenger = ScaffoldMessenger.of(context);
                await Clipboard.setData(ClipboardData(text: kopya));
                messenger.showSnackBar(
                  SnackBar(content: Text('$etiket kopyalandı.')),
                );
              },
              icon: const Icon(LucideIcons.copy, size: 18),
              label: const Text('Kopyala'),
              style: FilledButton.styleFrom(
                backgroundColor: r.zeminIkincil,
                foregroundColor: r.metin,
                minimumSize: const Size(44, OmvilaBoyut.kontrolSm),
                textStyle: OmvilaYazi.etiket,
              ),
            ),
          ),
        ],
      ),
    );
  }
}
