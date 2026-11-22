import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_isletme/features/shell/presentation/tabs.dart';
import 'package:omvila_isletme/features/today/presentation/today_format.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// "Salı 25 Kasım · Kadıköy" / "Bugün" ve "+ Randevu".
class TodayHeader extends StatelessWidget {
  /// Başlık.
  const TodayHeader({this.subtitle, this.onNewAppointment, super.key});

  /// Tarih ve şube; veri gelene kadar null.
  final String? subtitle;

  /// "+ Randevu" (appointment.create izni yoksa null, buton gizli).
  final VoidCallback? onNewAppointment;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final line = subtitle;
    final onNew = onNewAppointment;
    return Row(
      crossAxisAlignment: CrossAxisAlignment.end,
      children: [
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              if (line != null)
                Text(
                  line,
                  style: OmvilaYazi.ikincil.copyWith(
                    color: r.metinIkincil,
                    fontWeight: FontWeight.w600,
                  ),
                ),
              Semantics(
                header: true,
                child: Text(
                  'Bugün',
                  style: OmvilaYazi.buyukBaslik.copyWith(color: r.metin),
                ),
              ),
            ],
          ),
        ),
        if (onNew != null)
          OmvilaButon(
            etiket: 'Randevu',
            ikon: LucideIcons.plus,
            boyut: OmvilaButonBoyutu.sm,
            onPressed: onNew,
          ),
      ],
    );
  }
}

/// "Bildirimler kapalı" şeridi (MainBildirimKapali); kapatılabilir.
class NotificationsOffBanner extends StatelessWidget {
  /// Şerit.
  const NotificationsOffBanner({
    required this.canApprove,
    required this.onEnable,
    required this.onDismiss,
    super.key,
  });

  /// Onay yetkisi yoksa (personel) metin yeni randevulardan söz eder.
  final bool canApprove;

  /// "Bildirimleri aç".
  final VoidCallback onEnable;

  /// Kapat.
  final VoidCallback onDismiss;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final style = OmvilaYazi.ikincil.copyWith(color: r.metin);
    return Container(
      padding: const EdgeInsets.fromLTRB(
        OmvilaBosluk.bosluk4,
        OmvilaBosluk.bosluk3,
        OmvilaBosluk.bosluk1,
        OmvilaBosluk.bosluk2,
      ),
      decoration: BoxDecoration(
        color: r.uyariZemin,
        borderRadius: BorderRadius.circular(OmvilaKose.koseLg),
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Padding(
            padding: const EdgeInsets.only(top: 2),
            child: Icon(LucideIcons.bellOff, size: 20, color: r.metin),
          ),
          const SizedBox(width: OmvilaBosluk.bosluk3),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text.rich(
                  TextSpan(
                    children: [
                      TextSpan(
                        text: 'Bildirimler kapalı. ',
                        style: style.copyWith(fontWeight: FontWeight.w700),
                      ),
                      TextSpan(
                        text: canApprove
                            ? 'Onay bekleyen talepleri kaçırabilirsiniz.'
                            : 'Yeni randevuları kaçırabilirsiniz.',
                      ),
                    ],
                  ),
                  style: style,
                ),
                TextButton(
                  onPressed: onEnable,
                  style: TextButton.styleFrom(
                    padding: EdgeInsets.zero,
                    foregroundColor: r.metinMarka,
                    textStyle: OmvilaYazi.ikincil.copyWith(
                      fontWeight: FontWeight.w700,
                      decoration: TextDecoration.underline,
                    ),
                  ),
                  child: const Text('Bildirimleri aç'),
                ),
              ],
            ),
          ),
          IconButton(
            onPressed: onDismiss,
            tooltip: 'Şeridi kapat',
            icon: Icon(LucideIcons.x, size: 20, color: r.metinIkincil),
          ),
        ],
      ),
    );
  }
}

/// Bento: SIRADAKİ (blok-marka), ONAY BEKLİYOR (blok-vurgu), SIRADA
/// (blok-kum). Null kartlar gösterilmez.
class TodayBento extends StatelessWidget {
  /// Bento.
  const TodayBento({required this.today, required this.now, super.key});

  /// Bugün verisi.
  final ManageToday today;

  /// Sunucu saatine göre şimdi.
  final DateTime now;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final next = today.next;
    final pending = today.pendingApprovalCount;
    final walkIns = today.walkIns;
    final wait = walkIns?.estimatedWaitMinutesForNew;
    final side = <Widget>[
      if (pending != null)
        BentoBlok(
          ton: BentoTonu.vurgu,
          ustEtiket: 'Onay bekliyor',
          deger: formatCount(pending),
          onTap: pending > 0 ? () => context.go(Routes.calendar) : null,
        ),
      if (walkIns != null)
        BentoBlok(
          ton: BentoTonu.kum,
          ustEtiket: 'Sırada',
          deger: formatCount(walkIns.waitingCount),
          aciklama: wait == null ? null : '~${formatDuration(wait)} bekleme',
          onTap: () => context.go(Routes.walkIns),
        ),
    ];
    final nextBlock = next == null
        ? null
        : BentoBlok(
            ton: BentoTonu.marka,
            ustEtiket: 'Sıradaki',
            deger: formatTime(next.startUtc, today.timeZone),
            baslik: next.customerName,
            aciklama: '${next.serviceSummary} · ${durationOf(next)}',
            alt: Container(
              constraints: const BoxConstraints(
                minHeight: OmvilaBoyut.kontrolSm,
              ),
              alignment: Alignment.center,
              decoration: BoxDecoration(
                color: r.zeminMarkaYumusak,
                borderRadius: BorderRadius.circular(OmvilaKose.koseSm),
              ),
              child: Text(
                startsIn(next.startUtc, now),
                textAlign: TextAlign.center,
                style: OmvilaYazi.etiketKucuk.copyWith(color: r.metinMarka),
              ),
            ),
          );
    // Ok gösterilmez (tasarımda yok, 1.3× yazıda saate yer kalır); dokununca
    // randevu ayrıntısı.
    final nextTile = next == null
        ? null
        : Semantics(
            button: true,
            child: GestureDetector(
              behavior: HitTestBehavior.opaque,
              onTap: () => context.push(Routes.appointment(next.appointmentId)),
              child: nextBlock,
            ),
          );
    if (nextTile == null && side.isEmpty) {
      return const SizedBox.shrink();
    }

    return IntrinsicHeight(
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        spacing: OmvilaBosluk.bosluk3,
        children: [
          ?(nextTile == null ? null : Expanded(child: nextTile)),
          if (side.isNotEmpty)
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                spacing: OmvilaBosluk.bosluk3,
                children: side,
              ),
            ),
        ],
      ),
    );
  }
}

/// "KASIM PRİMİ 6.840 ₺" kartı ve ay içi kümülatif çizgi.
class CommissionCard extends StatelessWidget {
  /// Prim kartı.
  const CommissionCard({required this.commission, this.onTap, super.key});

  /// Kişisel prim.
  final TodayCommission commission;

  /// Profil'deki Primim'e gider.
  final VoidCallback? onTap;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final radius = BorderRadius.circular(OmvilaKose.koseXl);
    return DecoratedBox(
      decoration: BoxDecoration(
        borderRadius: radius,
        boxShadow: context.omvilaGolgeler.golgeSm,
      ),
      child: Material(
        color: r.zeminYukseltilmis,
        borderRadius: radius,
        child: InkWell(
          onTap: onTap,
          borderRadius: radius,
          child: Padding(
            padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
            child: Row(
              children: [
                Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      toUpperTr(commissionLabel(commission.month)),
                      style: OmvilaYazi.aciklama.copyWith(
                        color: r.metinIkincil,
                        fontWeight: FontWeight.w700,
                        letterSpacing: 0.6,
                      ),
                    ),
                    Text(
                      formatMoney(
                        commission.totalMinor,
                        currency: commission.currency.value,
                      ),
                      style: OmvilaYazi.baslik1.copyWith(
                        color: r.metin,
                        fontWeight: FontWeight.w800,
                      ),
                    ),
                  ],
                ),
                const SizedBox(width: OmvilaBosluk.bosluk4),
                Expanded(
                  child: ExcludeSemantics(
                    child: SizedBox(
                      height: 40,
                      child: CustomPaint(
                        painter: _SparklinePainter(
                          values: cumulativeCommission(commission.daily),
                          color: r.marka,
                        ),
                      ),
                    ),
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

class _SparklinePainter extends CustomPainter {
  const _SparklinePainter({required this.values, required this.color});

  final List<int> values;
  final Color color;

  @override
  void paint(Canvas canvas, Size size) {
    if (values.length < 2) {
      return;
    }

    final min = values.reduce((a, b) => a < b ? a : b);
    final max = values.reduce((a, b) => a > b ? a : b);
    final span = (max - min) == 0 ? 1 : max - min;
    final path = Path();
    for (var i = 0; i < values.length; i++) {
      final x = size.width * i / (values.length - 1);
      final y = size.height - size.height * (values[i] - min) / span;
      i == 0 ? path.moveTo(x, y) : path.lineTo(x, y);
    }

    canvas.drawPath(
      path,
      Paint()
        ..color = color
        ..style = PaintingStyle.stroke
        ..strokeWidth = 2
        ..strokeJoin = StrokeJoin.round
        ..strokeCap = StrokeCap.round,
    );
  }

  @override
  bool shouldRepaint(_SparklinePainter old) =>
      old.color != color || old.values != values;
}
