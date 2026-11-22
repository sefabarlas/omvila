import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Profil ve bildirim alt ekranlarının iskeleti (Odemelerim, Izinler,
/// Bildirimler, ProfilDuzenle): geri oku, büyük başlık, açıklama, içerik;
/// [alt] sabit alt alan ("Kaydet").
class IcSayfa extends StatelessWidget {
  /// Sayfa.
  const IcSayfa({
    required this.baslik,
    required this.children,
    this.aciklama,
    this.onRefresh,
    this.alt,
    super.key,
  });

  /// "Ödemelerim".
  final String baslik;

  /// Başlığın altındaki metin.
  final String? aciklama;

  /// İçerik.
  final List<Widget> children;

  /// Aşağı çekip yenileme.
  final Future<void> Function()? onRefresh;

  /// Sabit alt alan.
  final Widget? alt;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final text = aciklama;
    final list = ListView(
      physics: const AlwaysScrollableScrollPhysics(),
      padding: const EdgeInsets.fromLTRB(
        OmvilaBosluk.bosluk4,
        0,
        OmvilaBosluk.bosluk4,
        OmvilaBosluk.bosluk8,
      ),
      children: [
        Semantics(
          header: true,
          child: Text(
            baslik,
            style: OmvilaYazi.buyukBaslik.copyWith(color: r.metin),
          ),
        ),
        if (text != null) ...[
          const SizedBox(height: OmvilaBosluk.bosluk2),
          Text(text, style: OmvilaYazi.govde.copyWith(color: r.metinIkincil)),
        ],
        const SizedBox(height: OmvilaBosluk.bosluk4),
        ...children,
      ],
    );
    final refresh = onRefresh;
    final bottom = alt;
    return Scaffold(
      backgroundColor: r.zemin,
      appBar: AppBar(
        backgroundColor: r.zemin,
        surfaceTintColor: r.zemin,
        leading: IconButton(
          tooltip: 'Geri',
          icon: const Icon(LucideIcons.arrowLeft),
          onPressed: () => context.canPop() ? context.pop() : context.go('/'),
        ),
      ),
      body: refresh == null
          ? list
          : RefreshIndicator(onRefresh: refresh, color: r.marka, child: list),
      bottomNavigationBar: bottom == null
          ? null
          : SafeArea(
              child: Padding(
                padding: EdgeInsets.fromLTRB(
                  OmvilaBosluk.bosluk4,
                  OmvilaBosluk.bosluk2,
                  OmvilaBosluk.bosluk4,
                  OmvilaBosluk.bosluk4 +
                      MediaQuery.viewInsetsOf(context).bottom,
                ),
                child: bottom,
              ),
            ),
    );
  }
}

/// Beyaz, yuvarlak köşeli grup kartı; satırlar arasında çizgi.
class GrupKarti extends StatelessWidget {
  /// Kart.
  const GrupKarti({required this.children, super.key});

  /// Satırlar.
  final List<Widget> children;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final radius = BorderRadius.circular(OmvilaKose.koseLg);
    return Material(
      color: r.zeminYukseltilmis,
      borderRadius: radius,
      clipBehavior: Clip.antiAlias,
      child: Column(
        children: [
          for (final (i, child) in children.indexed) ...[
            if (i > 0) Divider(height: 1, color: r.cizgi),
            child,
          ],
        ],
      ),
    );
  }
}

/// Küçük durum çipi ("Az kaldı", "Bitti", "Salon onayı bekleniyor").
class EtiketCipi extends StatelessWidget {
  /// Çip.
  const EtiketCipi(
    this.metin, {
    required this.zemin,
    required this.renk,
    super.key,
  });

  /// Metin.
  final String metin;

  /// Zemin.
  final Color zemin;

  /// Metin rengi.
  final Color renk;

  @override
  Widget build(BuildContext context) => DecoratedBox(
    decoration: BoxDecoration(
      color: zemin,
      borderRadius: BorderRadius.circular(OmvilaKose.koseTam),
    ),
    child: Padding(
      padding: const EdgeInsets.symmetric(
        horizontal: OmvilaBosluk.bosluk3,
        vertical: OmvilaBosluk.bosluk1,
      ),
      child: Text(
        metin,
        style: OmvilaYazi.aciklama.copyWith(
          color: renk,
          fontWeight: FontWeight.w700,
        ),
      ),
    ),
  );
}

/// Kare ikon kutusu (menü ve liste satırları).
class IkonKutusu extends StatelessWidget {
  /// Kutu.
  const IkonKutusu(
    this.ikon, {
    this.zemin,
    this.renk,
    this.boyut = 40,
    super.key,
  });

  /// İkon.
  final IconData ikon;

  /// Zemin; varsayılan ikincil zemin.
  final Color? zemin;

  /// İkon rengi; varsayılan metin.
  final Color? renk;

  /// Kenar.
  final double boyut;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    return Container(
      width: boyut,
      height: boyut,
      decoration: BoxDecoration(
        color: zemin ?? r.zeminIkincil,
        borderRadius: BorderRadius.circular(OmvilaKose.koseSm),
      ),
      child: Icon(ikon, size: 20, color: renk ?? r.metin),
    );
  }
}
