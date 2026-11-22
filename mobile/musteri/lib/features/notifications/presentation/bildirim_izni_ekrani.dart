import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_musteri/features/notifications/presentation/notification_prefs_controller.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Bildirim izni (BildirimIzni.png): sistem istemi yalnız "Bildirimlere izin
/// ver"de açılır; "Şimdi değil" ana ekranda şerit bırakır. Yanıttan sonra
/// router ana ekrana geçer.
class BildirimIzniEkrani extends ConsumerStatefulWidget {
  /// Ekran.
  const BildirimIzniEkrani({super.key});

  @override
  ConsumerState<BildirimIzniEkrani> createState() => _BildirimIzniEkraniState();
}

class _BildirimIzniEkraniState extends ConsumerState<BildirimIzniEkrani> {
  bool _busy = false;

  Future<void> _run(
    Future<void> Function(NotificationPrefsController c) action,
  ) async {
    setState(() => _busy = true);
    try {
      await action(ref.read(notificationPrefsProvider.notifier));
    } finally {
      if (mounted) {
        setState(() => _busy = false);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    return Scaffold(
      backgroundColor: r.zemin,
      body: CustomScrollView(
        slivers: [
          SliverToBoxAdapter(child: _Gorsel(renkler: r)),
          SliverPadding(
            padding: const EdgeInsets.all(OmvilaBosluk.bosluk6),
            sliver: SliverFillRemaining(
              hasScrollBody: false,
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  Semantics(
                    header: true,
                    child: Text(
                      'Randevunuzu kaçırmayın',
                      style: OmvilaYazi.buyukBaslik.copyWith(color: r.metin),
                    ),
                  ),
                  const SizedBox(height: OmvilaBosluk.bosluk3),
                  Text(
                    'Onay, hatırlatma ve boşalan saatleri bildirimle haber '
                    'verelim. Bildirimleri kapatırsanız randevu mesajları SMS '
                    'ile gelir.',
                    style: OmvilaYazi.govde.copyWith(color: r.metinIkincil),
                  ),
                  const Spacer(),
                  const SizedBox(height: OmvilaBosluk.bosluk6),
                  OmvilaButon(
                    etiket: 'Bildirimlere izin ver',
                    boyut: OmvilaButonBoyutu.lg,
                    tamGenislik: true,
                    yukleniyor: _busy,
                    onPressed: () => _run((c) => c.allow()),
                  ),
                  const SizedBox(height: OmvilaBosluk.bosluk2),
                  OmvilaButon(
                    etiket: 'Şimdi değil',
                    tur: OmvilaButonTuru.hayalet,
                    tamGenislik: true,
                    onPressed: _busy ? null : () => _run((c) => c.skip()),
                  ),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }
}

/// Üst görsel: örnek bildirim kartları (tasarımdaki tanıtım görseli; gerçek
/// veri değildir).
class _Gorsel extends StatelessWidget {
  const _Gorsel({required this.renkler});

  final OmvilaRenkler renkler;

  @override
  Widget build(BuildContext context) {
    final r = renkler;
    return ExcludeSemantics(
      child: ClipRRect(
        borderRadius: const BorderRadius.vertical(
          bottom: Radius.circular(OmvilaKose.koseXl * 1.6),
        ),
        child: ColoredBox(
          color: r.blokKum,
          child: Padding(
            padding: EdgeInsets.fromLTRB(
              OmvilaBosluk.bosluk6,
              MediaQuery.paddingOf(context).top + 80,
              OmvilaBosluk.bosluk6,
              OmvilaBosluk.bosluk8,
            ),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                _OrnekBildirim(
                  renkler: r,
                  ikon: LucideIcons.calendarCheck,
                  ikonZemin: r.blokMarka,
                  baslik: 'OMVILA',
                  metin: "Yarın 14:30'da Kuaför Elif'te randevunuz var.",
                  zaman: 'şimdi',
                ),
                const SizedBox(height: OmvilaBosluk.bosluk6),
                Padding(
                  padding: const EdgeInsets.symmetric(
                    horizontal: OmvilaBosluk.bosluk4,
                  ),
                  child: _OrnekBildirim(
                    renkler: r,
                    ikon: LucideIcons.listChecks,
                    ikonZemin: r.vurgu,
                    metin: 'Beklediğiniz saat boşaldı. 15 dakikanız var.',
                    soluk: true,
                  ),
                ),
                const SizedBox(height: OmvilaBosluk.bosluk6),
                Align(
                  alignment: Alignment.centerRight,
                  child: Transform.rotate(
                    angle: 0.12,
                    child: Container(
                      width: 76,
                      height: 76,
                      decoration: BoxDecoration(
                        color: r.blokMarka,
                        borderRadius: BorderRadius.circular(OmvilaKose.koseXl),
                      ),
                      child: Icon(
                        LucideIcons.bellRing,
                        color: r.metinTers,
                        size: 28,
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

class _OrnekBildirim extends StatelessWidget {
  const _OrnekBildirim({
    required this.renkler,
    required this.ikon,
    required this.ikonZemin,
    required this.metin,
    this.baslik,
    this.zaman,
    this.soluk = false,
  });

  final OmvilaRenkler renkler;
  final IconData ikon;
  final Color ikonZemin;
  final String metin;
  final String? baslik;
  final String? zaman;
  final bool soluk;

  @override
  Widget build(BuildContext context) {
    final r = renkler;
    final title = baslik;
    final time = zaman;
    return Container(
      padding: const EdgeInsets.all(OmvilaBosluk.bosluk3),
      decoration: BoxDecoration(
        color: soluk ? r.zemin.withValues(alpha: 0.7) : r.zeminYukseltilmis,
        borderRadius: BorderRadius.circular(OmvilaKose.koseLg),
        boxShadow: soluk ? null : context.omvilaGolgeler.golgeMd,
      ),
      child: Row(
        children: [
          Container(
            width: soluk ? 32 : 40,
            height: soluk ? 32 : 40,
            decoration: BoxDecoration(
              color: ikonZemin,
              borderRadius: BorderRadius.circular(OmvilaKose.koseSm),
            ),
            child: Icon(ikon, size: soluk ? 16 : 20, color: r.metinTers),
          ),
          const SizedBox(width: OmvilaBosluk.bosluk3),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                if (title != null)
                  Row(
                    children: [
                      Expanded(
                        child: Text(
                          title,
                          style: OmvilaYazi.etiketKucuk.copyWith(
                            color: r.metin,
                          ),
                        ),
                      ),
                      if (time != null)
                        Text(
                          time,
                          style: OmvilaYazi.aciklama.copyWith(
                            color: r.metinIkincil,
                          ),
                        ),
                    ],
                  ),
                Text(
                  metin,
                  style: (soluk ? OmvilaYazi.aciklama : OmvilaYazi.ikincil)
                      .copyWith(color: soluk ? r.metinIkincil : r.metin),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
