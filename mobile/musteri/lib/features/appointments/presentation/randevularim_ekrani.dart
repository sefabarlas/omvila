import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/app/providers.dart';
import 'package:omvila_musteri/app/router.dart';
import 'package:omvila_musteri/features/appointments/data/appointments_repository.dart';
import 'package:omvila_musteri/features/appointments/domain/randevu_metinleri.dart';
import 'package:omvila_musteri/features/appointments/presentation/home_provider.dart';
import 'package:omvila_musteri/features/appointments/presentation/kahraman_karti.dart';
import 'package:omvila_musteri/features/booking/domain/booking_draft.dart';
import 'package:omvila_musteri/features/booking/presentation/booking_controller.dart';
import 'package:omvila_musteri/features/brands/presentation/salon_ekleme.dart';
import 'package:omvila_musteri/features/notifications/presentation/bildirim_kapali_seridi.dart';
import 'package:omvila_musteri/features/notifications/presentation/notification_prefs_controller.dart';
import 'package:omvila_musteri/features/shell/sekme_kabugu.dart';
import 'package:omvila_musteri/shared/widgets/durum_desenleri.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Randevularım (Main, MainBos, MainYaklasanYok, MainBildirimKapali).
/// Kart randevu detayına, "Geçmiş" Geçmiş'e, "Aynısını tekrar al" (KK2)
/// önceki randevuyla dolu randevu alma akışına gider.
class RandevularimEkrani extends ConsumerWidget {
  /// Ekran.
  const RandevularimEkrani({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final home = ref.watch(homeProvider);
    final showBanner =
        ref.watch(notificationPrefsProvider).value?.showBanner ?? false;
    final data = home.value;
    final greeting = switch (data) {
      null => null,
      HomeData(isFirstVisit: true, :final firstName) => _join(
        'Hoş geldiniz',
        firstName,
      ),
      HomeData(:final firstName) => _join('Merhaba', firstName),
    };
    final children = <Widget>[
      if (showBanner) ...[
        const BildirimKapaliSeridi(),
        const SizedBox(height: OmvilaBosluk.bosluk3),
      ],
      ...switch (home) {
        AsyncData(:final value) => _content(context, ref, value),
        AsyncError(:final error) => [
          HataDurumu(error: error, onRetry: () => ref.invalidate(homeProvider)),
        ],
        _ => [
          const Padding(
            padding: EdgeInsets.only(top: OmvilaBosluk.bosluk12),
            child: YukleniyorDurumu(),
          ),
        ],
      },
    ];
    return SekmeSayfasi(
      ustBaslik: greeting,
      baslik: 'Randevularım',
      onRefresh: () => ref.refresh(homeProvider.future),
      children: children,
    );
  }

  static String _join(String word, String? name) =>
      name == null ? word : '$word $name';

  List<Widget> _content(BuildContext context, WidgetRef ref, HomeData data) {
    void yeniRandevu() => context.push(Routes.randevuAl);
    if (data.isFirstVisit) {
      return [
        IlkSalonKarti(
          onQr: () => SalonEkleme.qrIle(context, ref),
          onPaste: () => SalonEkleme.panodan(context, ref),
        ),
        const SizedBox(height: OmvilaBosluk.bosluk6),
        const NasilCalisir(),
      ];
    }

    void gecmis() => context.push(Routes.gecmis);
    final upcoming = data.upcoming;
    final last = data.last;
    if (upcoming.isEmpty) {
      return [
        YaklasanYok(
          son: last,
          onRebook: last == null
              ? yeniRandevu
              : () => aynisiniTekrarAl(
                  context,
                  ref,
                  BookingDraft.rebook(
                    brand: last.brand,
                    branchId: last.branch.id,
                    branchName: last.branch.name,
                    items: last.items,
                  ),
                ),
          onNew: yeniRandevu,
        ),
        if (last != null)
          Align(
            child: TextButton(
              onPressed: gecmis,
              child: const Text('Geçmiş randevular'),
            ),
          ),
      ];
    }

    final clock = ref.watch(clockProvider);
    return [
      KahramanKarti(
        randevu: upcoming.first,
        clock: clock,
        onTap: () => context.push(Routes.randevu(upcoming.first.id)),
      ),
      const SizedBox(height: OmvilaBosluk.bosluk3),
      _YeniRandevuButonu(onPressed: yeniRandevu),
      const SizedBox(height: OmvilaBosluk.bosluk4),
      Row(
        children: [
          Expanded(
            child: upcoming.length > 1
                ? Semantics(
                    header: true,
                    child: Text(
                      'Yaklaşan',
                      style: OmvilaYazi.baslik3.copyWith(
                        color: context.omvilaRenkler.metin,
                        fontWeight: FontWeight.w800,
                      ),
                    ),
                  )
                : const SizedBox.shrink(),
          ),
          TextButton(onPressed: gecmis, child: const Text('Geçmiş')),
        ],
      ),
      for (final a in upcoming.skip(1)) ...[
        _RandevuKartiSatiri(randevu: a),
        const SizedBox(height: OmvilaBosluk.bosluk2),
      ],
    ];
  }
}

/// "Yeni randevu al": ana eylem, metin renginde dolu (Main.png).
class _YeniRandevuButonu extends StatelessWidget {
  const _YeniRandevuButonu({required this.onPressed});

  final VoidCallback onPressed;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    return FilledButton.icon(
      onPressed: onPressed,
      icon: const Icon(LucideIcons.plus, size: 20),
      label: const Text('Yeni randevu al'),
      style: FilledButton.styleFrom(
        backgroundColor: r.metin,
        foregroundColor: r.zemin,
        minimumSize: const Size.fromHeight(OmvilaBoyut.kontrolLg),
        textStyle: OmvilaYazi.etiket,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(OmvilaKose.koseLg),
        ),
      ),
    );
  }
}

class _RandevuKartiSatiri extends ConsumerWidget {
  const _RandevuKartiSatiri({required this.randevu});

  final MeAppointmentSummary randevu;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final a = randevu;
    final status = RandevuDurumu.fromDisplayStatus(a.displayStatus);
    return RandevuKarti(
      onTap: () => context.push(Routes.randevu(a.id)),
      salonAdi: a.brand.name,
      hizmetVePersonel: ozetSatiri([
        hizmetlerMetni(a.items),
        personelMetni(a.items),
      ]),
      tarih: formatDate(toBranchDate(a.startUtc, a.timeZone), DateStyle.short),
      saat: formatTime(a.startUtc, a.timeZone),
      sure: formatDuration(sureDakika(a.startUtc, a.endUtc)),
      durum: status,
      tutar: a.amountDue.amountMinor > 0
          ? '${formatMoney(a.amountDue.amountMinor)} salonda'
          : null,
      not: status == RandevuDurumu.onayBekliyor
          ? 'Salon onaylayınca SMS ile bildireceğiz'
          : null,
    );
  }
}

/// İlk giriş kartı (MainBos, KK1): salon keşfi yok; QR veya bağlantı.
class IlkSalonKarti extends StatelessWidget {
  /// Kart.
  const IlkSalonKarti({
    required this.onQr,
    required this.onPaste,
    this.baslik = 'İlk randevunuzu alın',
    this.aciklama =
        'Randevu almak için salonunuzun bağlantısını açın ya da '
        'salondaki QR kodu okutun.',
    super.key,
  });

  /// Başlık.
  final String baslik;

  /// Açıklama.
  final String aciklama;

  /// "QR kodu okut".
  final VoidCallback onQr;

  /// "Bağlantıyı yapıştır".
  final VoidCallback onPaste;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final buttonShape = RoundedRectangleBorder(
      borderRadius: BorderRadius.circular(OmvilaKose.koseMd),
    );
    return Container(
      padding: const EdgeInsets.all(OmvilaBosluk.bosluk5),
      decoration: BoxDecoration(
        color: r.blokKum,
        borderRadius: BorderRadius.circular(OmvilaKose.koseXl),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Align(
            alignment: Alignment.centerLeft,
            child: Container(
              width: 52,
              height: 52,
              decoration: BoxDecoration(
                color: r.blokKumMetin.withValues(alpha: 0.08),
                borderRadius: BorderRadius.circular(OmvilaKose.koseMd),
              ),
              child: Icon(LucideIcons.store, color: r.blokKumMetin),
            ),
          ),
          const SizedBox(height: OmvilaBosluk.bosluk4),
          Semantics(
            header: true,
            child: Text(
              baslik,
              style: OmvilaYazi.baslik2.copyWith(color: r.blokKumMetin),
            ),
          ),
          const SizedBox(height: OmvilaBosluk.bosluk2),
          Text(
            aciklama,
            style: OmvilaYazi.ikincil.copyWith(color: r.blokKumMetin),
          ),
          const SizedBox(height: OmvilaBosluk.bosluk4),
          FilledButton.icon(
            onPressed: onQr,
            icon: const Icon(LucideIcons.scanLine, size: 20),
            label: const Text('QR kodu okut'),
            style: FilledButton.styleFrom(
              backgroundColor: r.metin,
              foregroundColor: r.zemin,
              minimumSize: const Size.fromHeight(OmvilaBoyut.kontrolLg),
              textStyle: OmvilaYazi.etiket,
              shape: buttonShape,
            ),
          ),
          const SizedBox(height: OmvilaBosluk.bosluk2),
          FilledButton.icon(
            onPressed: onPaste,
            icon: const Icon(LucideIcons.clipboardPaste, size: 20),
            label: const Text('Bağlantıyı yapıştır'),
            style: FilledButton.styleFrom(
              backgroundColor: r.blokKumMetin.withValues(alpha: 0.08),
              foregroundColor: r.blokKumMetin,
              minimumSize: const Size.fromHeight(OmvilaBoyut.kontrolLg),
              textStyle: OmvilaYazi.etiket,
              shape: buttonShape,
            ),
          ),
        ],
      ),
    );
  }
}

/// "Nasıl çalışır?" üç adım (MainBos).
class NasilCalisir extends StatelessWidget {
  /// Anlatım.
  const NasilCalisir({super.key});

  static const _adimlar = [
    ('Salonun bağlantısı', 'SMS, Instagram ya da masadaki QR kart.'),
    ('Saat seçin', 'Hizmet, kişi ve boş saat; telefonla onay.'),
    ('Hepsi burada', 'Randevular, hatırlatmalar ve paketler.'),
  ];

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Semantics(
          header: true,
          child: Text(
            'Nasıl çalışır?',
            style: OmvilaYazi.baslik3.copyWith(
              color: r.metin,
              fontWeight: FontWeight.w800,
            ),
          ),
        ),
        for (final (i, (baslik, aciklama)) in _adimlar.indexed) ...[
          Divider(height: OmvilaBosluk.bosluk6, color: r.cizgi),
          MergeSemantics(
            child: Row(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Container(
                  width: 28,
                  height: 28,
                  alignment: Alignment.center,
                  decoration: BoxDecoration(
                    color: r.zeminIkincil,
                    shape: BoxShape.circle,
                  ),
                  child: Text(
                    '${i + 1}',
                    style: OmvilaYazi.etiketKucuk.copyWith(color: r.metin),
                  ),
                ),
                const SizedBox(width: OmvilaBosluk.bosluk4),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        baslik,
                        style: OmvilaYazi.govdeGuclu.copyWith(color: r.metin),
                      ),
                      Text(
                        aciklama,
                        style: OmvilaYazi.ikincil.copyWith(
                          color: r.metinIkincil,
                        ),
                      ),
                    ],
                  ),
                ),
              ],
            ),
          ),
        ],
      ],
    );
  }
}

/// Yaklaşan randevu yok (MainYaklasanYok, KK2): boş ekran değil, son
/// randevudan "Aynısını tekrar al".
class YaklasanYok extends StatelessWidget {
  /// Bölüm; [son] yoksa (salon eklenmiş, randevu yok) yalnız "Yeni randevu".
  const YaklasanYok({
    required this.son,
    required this.onRebook,
    required this.onNew,
    super.key,
  });

  /// Son randevu.
  final MeAppointmentSummary? son;

  /// "Aynısını tekrar al".
  final VoidCallback onRebook;

  /// "Başka randevu al" / "Yeni randevu al".
  final VoidCallback onNew;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final last = son;
    final lastDate = last == null
        ? null
        : formatDate(
            toBranchDate(last.startUtc, last.timeZone),
            DateStyle.dayMonth,
          );
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        DecoratedBox(
          decoration: BoxDecoration(
            color: r.zeminYukseltilmis,
            borderRadius: BorderRadius.circular(OmvilaKose.koseXl),
            boxShadow: context.omvilaGolgeler.golgeSm,
          ),
          child: Padding(
            padding: const EdgeInsets.all(OmvilaBosluk.bosluk5),
            child: Column(
              children: [
                Container(
                  width: 64,
                  height: 64,
                  decoration: BoxDecoration(
                    color: r.zeminMarkaYumusak,
                    borderRadius: BorderRadius.circular(OmvilaKose.koseLg),
                  ),
                  child: Icon(LucideIcons.calendar, color: r.metinMarka),
                ),
                const SizedBox(height: OmvilaBosluk.bosluk4),
                Text(
                  'Yaklaşan randevunuz yok',
                  textAlign: TextAlign.center,
                  style: OmvilaYazi.baslik3.copyWith(
                    color: r.metin,
                    fontWeight: FontWeight.w800,
                  ),
                ),
                const SizedBox(height: OmvilaBosluk.bosluk1),
                Text(
                  last == null
                      ? 'Salonunuzdan ilk randevunuzu alın.'
                      : 'Son randevunuz ${bulunma(lastDate!)} '
                            '${bulunmaGecmis(last.brand.name)}.',
                  textAlign: TextAlign.center,
                  style: OmvilaYazi.ikincil.copyWith(color: r.metinIkincil),
                ),
              ],
            ),
          ),
        ),
        const SizedBox(height: OmvilaBosluk.bosluk3),
        if (last != null && last.canRebook) ...[
          _TekrarAlKarti(randevu: last, onTap: onRebook),
          const SizedBox(height: OmvilaBosluk.bosluk3),
        ],
        FilledButton.icon(
          onPressed: onNew,
          icon: const Icon(LucideIcons.plus, size: 20),
          label: Text(last == null ? 'Yeni randevu al' : 'Başka randevu al'),
          style: FilledButton.styleFrom(
            backgroundColor: r.zeminIkincil,
            foregroundColor: r.metin,
            minimumSize: const Size.fromHeight(OmvilaBoyut.kontrolLg),
            textStyle: OmvilaYazi.etiket,
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(OmvilaKose.koseLg),
            ),
          ),
        ),
      ],
    );
  }
}

class _TekrarAlKarti extends StatelessWidget {
  const _TekrarAlKarti({required this.randevu, required this.onTap});

  final MeAppointmentSummary randevu;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final radius = BorderRadius.circular(OmvilaKose.koseXl);
    return Material(
      color: r.blokMarka,
      borderRadius: radius,
      child: InkWell(
        onTap: onTap,
        borderRadius: radius,
        child: Padding(
          padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
          child: Row(
            children: [
              Container(
                width: 48,
                height: 48,
                decoration: BoxDecoration(
                  color: r.metinTers.withValues(alpha: 0.12),
                  borderRadius: BorderRadius.circular(OmvilaKose.koseMd),
                ),
                child: Icon(LucideIcons.repeat, color: r.metinTers),
              ),
              const SizedBox(width: OmvilaBosluk.bosluk4),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      'Aynısını tekrar al',
                      style: OmvilaYazi.baslik3.copyWith(color: r.metinTers),
                    ),
                    Text(
                      ozetSatiri([
                        hizmetlerMetni(randevu.items),
                        personelMetni(randevu.items),
                        randevu.brand.name,
                      ]),
                      style: OmvilaYazi.ikincil.copyWith(
                        color: r.metinTers.withValues(alpha: 0.85),
                      ),
                    ),
                  ],
                ),
              ),
              Icon(LucideIcons.chevronRight, size: 20, color: r.metinTers),
            ],
          ),
        ),
      ),
    );
  }
}
