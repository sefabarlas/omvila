import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/app/providers.dart';
import 'package:omvila_musteri/app/router.dart';
import 'package:omvila_musteri/features/appointments/presentation/home_provider.dart';
import 'package:omvila_musteri/features/booking/domain/booking_draft.dart';
import 'package:omvila_musteri/features/booking/presentation/booking_controller.dart';
import 'package:omvila_musteri/shared/widgets/durum_desenleri.dart';
import 'package:omvila_musteri/shared/widgets/ic_sayfa.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Teklifin kapanma nedeni.
enum TeklifKapandi {
  /// Süre doldu (sunucu veya geri sayım).
  expired,

  /// "Bu saat olmaz".
  declined,

  /// Salon geri aldı.
  withdrawn,
}

/// "Cuma 28 Kasım · 16:00" (şubenin saat diliminde).
String teklifZamani(PublicWaitlistOffer o) {
  final day = toBranchDate(o.startUtc, o.timeZone);
  return '${formatDate(day, DateStyle.weekdayDayMonth)} · '
      '${formatTime(o.startUtc, o.timeZone)}';
}

/// "Cuma 16:00".
String teklifKisaZamani(PublicWaitlistOffer o) =>
    '${formatDate(toBranchDate(o.startUtc, o.timeZone), DateStyle.weekday)} '
    '${formatTime(o.startUtc, o.timeZone)}';

/// Bekleme teklifi (`/b/{code}`; BeklemeTeklif, BeklemeDoldu): geri sayım
/// sunucunun `holdExpiresUtc − generatedUtc` farkından ve enjekte saatten
/// (cihaz saati karşılaştırılmaz). "Randevuyu al" → publicAcceptWaitlistOffer
/// (If-Match) → kapora isteniyorsa YRHavale, değilse başarı ekranı.
class BeklemeTeklifEkrani extends ConsumerStatefulWidget {
  /// Ekran.
  const BeklemeTeklifEkrani({required this.code, super.key});

  /// Bağlantı kodu.
  final String code;

  @override
  ConsumerState<BeklemeTeklifEkrani> createState() =>
      _BeklemeTeklifEkraniState();
}

class _BeklemeTeklifEkraniState extends ConsumerState<BeklemeTeklifEkrani> {
  PublicWaitlistOffer? _offer;
  late DateTime _loadedAt;
  Object? _loadError;
  bool _serverExpired = false;
  bool _left = false;
  String? _busy;
  Timer? _tick;

  @override
  void initState() {
    super.initState();
    unawaited(_load());
    _tick = Timer.periodic(const Duration(seconds: 1), (_) {
      if (mounted && _offer?.status == WaitlistOfferStatus.pending) {
        setState(() {});
      }
    });
  }

  @override
  void dispose() {
    _tick?.cancel();
    super.dispose();
  }

  Future<void> _load({String? notice}) async {
    setState(() => _loadError = null);
    try {
      final offer = await ref
          .read(waitlistRepositoryProvider)
          .offer(widget.code);
      if (!mounted) {
        return;
      }

      setState(() {
        _offer = offer;
        _loadedAt = ref.read(clockProvider).now();
        _serverExpired = false;
      });
      if (notice != null) {
        ScaffoldMessenger.of(
          context,
        ).showSnackBar(SnackBar(content: Text(notice)));
      }
    } on ApiProblem catch (e) {
      if (mounted) {
        setState(() => _loadError = e);
      }
    }
  }

  Duration _remaining(PublicWaitlistOffer o) {
    final initial = o.holdExpiresUtc.difference(o.generatedUtc);
    final elapsed = ref.read(clockProvider).now().difference(_loadedAt);
    final left = initial - elapsed;
    return left.isNegative ? Duration.zero : left;
  }

  TeklifKapandi? _closed(PublicWaitlistOffer o) => switch (o.status) {
    WaitlistOfferStatus.pending =>
      _serverExpired || _remaining(o) == Duration.zero
          ? TeklifKapandi.expired
          : null,
    WaitlistOfferStatus.declined => TeklifKapandi.declined,
    WaitlistOfferStatus.withdrawn => TeklifKapandi.withdrawn,
    WaitlistOfferStatus.expired => TeklifKapandi.expired,
    _ => null,
  };

  Future<void> _act(String kind, PublicWaitlistOffer o) async {
    setState(() => _busy = kind);
    final repo = ref.read(waitlistRepositoryProvider);
    final messenger = ScaffoldMessenger.of(context);
    try {
      switch (kind) {
        case 'accept':
          final appointment = await repo.accept(widget.code, o);
          ref.invalidate(homeProvider);
          if (mounted) {
            context.go(
              appointment.displayStatus == DisplayStatus.depositPending
                  ? Routes.havale(appointment.id, yeni: true)
                  : Routes.tamam(appointment.id),
            );
          }

          return;
        case 'decline':
          final next = await repo.decline(widget.code, o);
          if (mounted) {
            setState(() => _offer = next);
          }
        default:
          await repo.leave(o.waitlistAccessCode);
          if (mounted) {
            setState(() => _left = true);
          }
      }
    } on ApiProblem catch (e) {
      if (e.code == ErrorCode.holdExpired.value) {
        setState(() => _serverExpired = true);
      } else if (e.code == ErrorCode.invalidStateTransition.value ||
          e.code == ErrorCode.concurrencyConflict.value ||
          e.code == ErrorCode.notFound.value) {
        await _load(
          notice: 'Teklif bu arada değişti; güncel hâli gösteriliyor.',
        );
      } else {
        messenger.showSnackBar(SnackBar(content: Text(e.message)));
      }
    } finally {
      if (mounted) {
        setState(() => _busy = null);
      }
    }
  }

  void _otherTime(PublicWaitlistOffer o) {
    ref
        .read(bookingProvider.notifier)
        .start(
          BookingDraft(
            brandSlug: o.brand.slug,
            brandName: o.brand.name,
            branchId: o.branch.id,
            branchName: o.branch.name,
          ),
        );
    context.go(Routes.yrHizmet);
  }

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final offer = _offer;
    final error = _loadError;
    final Widget body;
    Widget? bottom;
    if (offer == null) {
      body = error == null
          ? const YukleniyorDurumu()
          : (error is ApiProblem &&
                (error.code == ErrorCode.notFound.value ||
                    error.code == ErrorCode.accessCodeInvalid.value))
          ? Center(
              child: SingleChildScrollView(
                padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
                child: OmvilaBosDurum(
                  ikon: LucideIcons.link,
                  baslik: 'Bu teklif bulunamadı',
                  aciklama:
                      'Bağlantının süresi dolmuş ya da teklif geri alınmış '
                      'olabilir.',
                  eylemEtiketi: 'Randevularıma git',
                  onEylem: () => context.go(Routes.randevularim),
                ),
              ),
            )
          : HataDurumu(error: error, onRetry: _load);
    } else if (_left) {
      body = const _Kapali(
        ikon: LucideIcons.userRoundX,
        baslik: 'Bekleme listesinden çıktınız',
        aciklama: 'Bu salon için size bekleme listesi mesajı gelmeyecek.',
      );
      bottom = _Eylemler(
        birincil: ('Başka saat seç', () => _otherTime(offer)),
      );
    } else if (offer.status == WaitlistOfferStatus.accepted) {
      final code = offer.appointmentAccessCode;
      body = _Kapali(
        ikon: LucideIcons.calendarCheck,
        baslik: 'Bu saati aldınız',
        aciklama: '${teklifZamani(offer)} randevunuz bu teklifle oluşturuldu.',
      );
      bottom = code == null
          ? null
          : _Eylemler(
              birincil: (
                'Randevunuzu görün',
                () => context.go(Routes.kodluRandevu(code)),
              ),
            );
    } else if (_closed(offer) case final closed?) {
      body = _Doldu(teklif: offer, neden: closed);
      bottom = _Eylemler(
        birincil: ('Başka saat seç', () => _otherTime(offer)),
        ikincil: ('Listeden çık', () => _act('leave', offer)),
        ikincilYukleniyor: _busy == 'leave',
      );
    } else {
      body = _Teklif(teklif: offer, kalan: _remaining(offer));
      bottom = _Eylemler(
        birincil: ('Randevuyu al', () => _act('accept', offer)),
        birincilYukleniyor: _busy == 'accept',
        ikincil: ('Bu saat olmaz', () => _act('decline', offer)),
        ikincilYukleniyor: _busy == 'decline',
        ikincilTehlike: false,
        kilitli: _busy != null,
      );
    }

    return Scaffold(
      backgroundColor: r.zemin,
      appBar: AppBar(
        backgroundColor: r.zemin,
        surfaceTintColor: r.zemin,
        leading: Padding(
          padding: const EdgeInsets.all(OmvilaBosluk.bosluk2),
          child: IconButton.filledTonal(
            tooltip: 'Kapat',
            style: IconButton.styleFrom(backgroundColor: r.zeminIkincil),
            icon: Icon(LucideIcons.x, color: r.metin),
            onPressed: () => context.go(Routes.randevularim),
          ),
        ),
      ),
      body: body,
      bottomNavigationBar: bottom,
    );
  }
}

class _Teklif extends StatelessWidget {
  const _Teklif({required this.teklif, required this.kalan});

  final PublicWaitlistOffer teklif;
  final Duration kalan;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final o = teklif;
    final initial = o.holdExpiresUtc.difference(o.generatedUtc);
    final ratio = initial.inSeconds <= 0
        ? 0.0
        : (kalan.inSeconds / initial.inSeconds).clamp(0.0, 1.0);
    final clock =
        '${kalan.inMinutes.toString().padLeft(2, '0')}:'
        '${(kalan.inSeconds % 60).toString().padLeft(2, '0')}';
    final staff = {
      for (final i in o.items)
        if (i.staffName case final n? when n.isNotEmpty) n,
    };
    final minutes = o.items.fold<int>(0, (s, i) => s + i.durationMinutes);
    final deposit = o.depositAmount;
    final rows = [
      ('Salon', o.brand.name),
      ('Tarih', teklifZamani(o)),
      (
        'Hizmet',
        '${o.items.map((i) => i.serviceName).join(', ')} · '
            '${formatDuration(minutes)}',
      ),
      if (staff.isNotEmpty) ('Kişi', staff.join(', ')),
      if (deposit != null)
        ('Kapora (havale ile)', formatMoney(deposit.amountMinor)),
      ('Salonda ödenecek', formatMoney(o.amountDue.amountMinor)),
    ];
    final ink = r.metinTers;
    final name = o.customerFirstName;
    return ListView(
      padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
      children: [
        Semantics(
          label: 'Kalan süre $clock',
          container: true,
          excludeSemantics: true,
          child: Container(
            padding: const EdgeInsets.all(OmvilaBosluk.bosluk5),
            decoration: BoxDecoration(
              color: r.blokVurgu,
              borderRadius: BorderRadius.circular(OmvilaKose.koseXl),
            ),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  children: [
                    Expanded(
                      child: Text(
                        'BEKLEME LİSTESİ',
                        style: OmvilaYazi.etiketKucuk.copyWith(
                          color: ink,
                          letterSpacing: 0.8,
                        ),
                      ),
                    ),
                    Icon(LucideIcons.timer, size: 16, color: ink),
                    const SizedBox(width: OmvilaBosluk.bosluk1),
                    Text(
                      'Sizin için tutuluyor',
                      style: OmvilaYazi.etiketKucuk.copyWith(color: ink),
                    ),
                  ],
                ),
                const SizedBox(height: OmvilaBosluk.bosluk2),
                Text(
                  clock,
                  style: OmvilaYazi.buyukBaslik.copyWith(
                    color: ink,
                    fontSize: 64,
                    height: 1.1,
                    fontFeatures: const [FontFeature.tabularFigures()],
                  ),
                ),
                Text(
                  'dakika içinde onaylarsanız saat sizin',
                  style: OmvilaYazi.ikincil.copyWith(color: ink),
                ),
                const SizedBox(height: OmvilaBosluk.bosluk4),
                ClipRRect(
                  borderRadius: BorderRadius.circular(OmvilaKose.koseTam),
                  child: LinearProgressIndicator(
                    value: ratio,
                    minHeight: 4,
                    color: ink,
                    backgroundColor: ink.withValues(alpha: 0.3),
                  ),
                ),
              ],
            ),
          ),
        ),
        const SizedBox(height: OmvilaBosluk.bosluk5),
        Semantics(
          header: true,
          child: Text(
            name == null
                ? 'Beklediğiniz saat boşaldı'
                : '$name, beklediğiniz saat boşaldı',
            style: OmvilaYazi.baslik1.copyWith(color: r.metin),
          ),
        ),
        const SizedBox(height: OmvilaBosluk.bosluk4),
        GrupKarti(
          children: [
            for (final (label, value) in rows)
              Padding(
                padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
                child: Row(
                  children: [
                    Text(
                      label,
                      style: OmvilaYazi.govde.copyWith(color: r.metinIkincil),
                    ),
                    const SizedBox(width: OmvilaBosluk.bosluk3),
                    Expanded(
                      child: Text(
                        value,
                        textAlign: TextAlign.end,
                        style: OmvilaYazi.govdeGuclu.copyWith(color: r.metin),
                      ),
                    ),
                  ],
                ),
              ),
          ],
        ),
        const SizedBox(height: OmvilaBosluk.bosluk3),
        Text(
          [
            if ((o.requiresApproval ?? false) && deposit != null)
              'Randevu kapora havalesinden sonra kesinleşir.'
            else if (o.requiresApproval ?? false)
              'Randevu salon onayından sonra kesinleşir.',
            _listeNotu,
          ].join(' '),
          style: OmvilaYazi.ikincil.copyWith(color: r.metinIkincil),
        ),
      ],
    );
  }
}

const _listeNotu =
    'Onaylamazsanız saat listedeki bir sonraki kişiye önerilir. Siz listede '
    'kalmaya devam edersiniz.';

class _Doldu extends StatelessWidget {
  const _Doldu({required this.teklif, required this.neden});

  final PublicWaitlistOffer teklif;
  final TeklifKapandi neden;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final o = teklif;
    final when = '${o.brand.name} · ${teklifKisaZamani(o)}';
    final (title, reason) = switch (neden) {
      TeklifKapandi.expired => (
        'Bu saatin süresi doldu',
        '$when için ayrılan süre geçti, saat sıradaki kişiye önerildi.',
      ),
      TeklifKapandi.declined => (
        'Bu saati geri çevirdiniz',
        '$when sıradaki kişiye önerildi.',
      ),
      TeklifKapandi.withdrawn => (
        'Bu teklif geri alındı',
        '$when saatini salon başka bir düzenleme için geri aldı.',
      ),
    };
    return _Kapali(
      ikon: LucideIcons.hourglass,
      baslik: title,
      aciklama: reason,
      alt: Container(
        padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
        decoration: BoxDecoration(
          color: r.blokKum,
          borderRadius: BorderRadius.circular(OmvilaKose.koseLg),
        ),
        child: Row(
          children: [
            IkonKutusu(
              LucideIcons.listChecks,
              zemin: r.blokKumMetin.withValues(alpha: 0.08),
              renk: r.blokKumMetin,
            ),
            const SizedBox(width: OmvilaBosluk.bosluk3),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    'Listede kalmaya devam ediyorsunuz',
                    style: OmvilaYazi.govdeGuclu.copyWith(
                      color: r.blokKumMetin,
                    ),
                  ),
                  Text(
                    'Yeni bir saat boşalınca tekrar haber vereceğiz · '
                    '${o.items.map((i) => i.serviceName).join(', ')}',
                    style: OmvilaYazi.ikincil.copyWith(color: r.blokKumMetin),
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _Kapali extends StatelessWidget {
  const _Kapali({
    required this.ikon,
    required this.baslik,
    required this.aciklama,
    this.alt,
  });

  final IconData ikon;
  final String baslik;
  final String aciklama;
  final Widget? alt;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final extra = alt;
    return ListView(
      padding: const EdgeInsets.all(OmvilaBosluk.bosluk5),
      children: [
        Align(
          alignment: Alignment.centerLeft,
          child: IkonKutusu(ikon, boyut: 72),
        ),
        const SizedBox(height: OmvilaBosluk.bosluk5),
        Semantics(
          header: true,
          child: Text(
            baslik,
            style: OmvilaYazi.baslik1.copyWith(color: r.metin),
          ),
        ),
        const SizedBox(height: OmvilaBosluk.bosluk2),
        Text(aciklama, style: OmvilaYazi.govde.copyWith(color: r.metinIkincil)),
        if (extra != null) ...[
          const SizedBox(height: OmvilaBosluk.bosluk5),
          extra,
        ],
      ],
    );
  }
}

class _Eylemler extends StatelessWidget {
  const _Eylemler({
    required this.birincil,
    this.ikincil,
    this.birincilYukleniyor = false,
    this.ikincilYukleniyor = false,
    this.ikincilTehlike = true,
    this.kilitli = false,
  });

  final (String, VoidCallback) birincil;
  final (String, VoidCallback)? ikincil;
  final bool birincilYukleniyor;
  final bool ikincilYukleniyor;
  final bool ikincilTehlike;
  final bool kilitli;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final second = ikincil;
    return SafeArea(
      child: Padding(
        padding: const EdgeInsets.fromLTRB(
          OmvilaBosluk.bosluk4,
          OmvilaBosluk.bosluk2,
          OmvilaBosluk.bosluk4,
          OmvilaBosluk.bosluk2,
        ),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            OmvilaButon(
              etiket: birincil.$1,
              boyut: OmvilaButonBoyutu.lg,
              tamGenislik: true,
              yukleniyor: birincilYukleniyor,
              onPressed: kilitli && !birincilYukleniyor ? null : birincil.$2,
            ),
            if (second != null)
              TextButton(
                onPressed: kilitli || ikincilYukleniyor ? null : second.$2,
                style: TextButton.styleFrom(
                  foregroundColor: ikincilTehlike ? r.hata : r.metinMarka,
                  minimumSize: const Size.fromHeight(OmvilaBoyut.kontrolMd),
                ),
                child: Text(second.$1),
              ),
          ],
        ),
      ),
    );
  }
}
