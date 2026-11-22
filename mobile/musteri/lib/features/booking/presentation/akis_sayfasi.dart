import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Randevu alma adımları (components/BookingSteps).
enum AkisAdimi {
  /// Hizmet.
  hizmet('Hizmet'),

  /// Personel.
  personel('Personel'),

  /// Saat.
  saat('Saat'),

  /// Onay.
  onay('Onay');

  const AkisAdimi(this.etiket);

  /// Görünen ad.
  final String etiket;
}

/// Akış ekranlarının ortak iskeleti: başlık (geri, kapat), adım çubukları,
/// içerik ve alt sabit çubuk (components/StickyFooter).
class AkisSayfasi extends StatelessWidget {
  /// Sayfa.
  const AkisSayfasi({
    required this.baslik,
    required this.body,
    this.adim,
    this.alt,
    this.onKapat,
    super.key,
  });

  /// "Kuaför Elif", "Son kontrol".
  final String baslik;

  /// Adım çubuğu; null ise gösterilmez.
  final AkisAdimi? adim;

  /// İçerik (kaydırılabilir olmalı).
  final Widget body;

  /// Alt sabit çubuk.
  final Widget? alt;

  /// Kapat; varsayılan Randevularım.
  final VoidCallback? onKapat;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final step = adim;
    final bottom = alt;
    return Scaffold(
      backgroundColor: r.zemin,
      appBar: AppBar(
        backgroundColor: r.zemin,
        surfaceTintColor: r.zemin,
        centerTitle: true,
        title: Text(baslik, style: OmvilaYazi.ustBarBasligi),
        leading: context.canPop()
            ? IconButton(
                tooltip: 'Geri',
                icon: const Icon(LucideIcons.arrowLeft),
                onPressed: () => context.pop(),
              )
            : null,
        actions: [
          IconButton(
            tooltip: 'Kapat',
            icon: const Icon(LucideIcons.x),
            onPressed: onKapat ?? () => context.go('/'),
          ),
        ],
        bottom: step == null
            ? null
            : PreferredSize(
                preferredSize: Size.fromHeight(
                  36 * MediaQuery.textScalerOf(context).scale(1),
                ),
                child: _Adimlar(adim: step),
              ),
      ),
      body: body,
      bottomNavigationBar: bottom,
    );
  }
}

class _Adimlar extends StatelessWidget {
  const _Adimlar({required this.adim});

  final AkisAdimi adim;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    return Semantics(
      label: 'Adım ${adim.index + 1} / ${AkisAdimi.values.length}',
      excludeSemantics: true,
      child: Padding(
        padding: const EdgeInsets.fromLTRB(
          OmvilaBosluk.bosluk4,
          0,
          OmvilaBosluk.bosluk4,
          OmvilaBosluk.bosluk2,
        ),
        child: Row(
          spacing: OmvilaBosluk.bosluk2,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            for (final a in AkisAdimi.values)
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Container(
                      height: 4,
                      decoration: BoxDecoration(
                        color: a.index <= adim.index ? r.marka : r.cizgi,
                        borderRadius: BorderRadius.circular(
                          OmvilaKose.koseTam,
                        ),
                      ),
                    ),
                    const SizedBox(height: OmvilaBosluk.bosluk2),
                    Text(
                      a.etiket,
                      maxLines: 1,
                      overflow: TextOverflow.ellipsis,
                      style: OmvilaYazi.aciklama.copyWith(
                        color: a == adim ? r.metin : r.metinIkincil,
                        fontWeight: a == adim ? FontWeight.w700 : null,
                      ),
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

/// Alt sabit çubuk: özet (iki satır) ve ana eylem.
class AltOzetCubugu extends StatelessWidget {
  /// Çubuk; [baslik] null ise buton tam genişlik.
  const AltOzetCubugu({
    required this.eylem,
    required this.onPressed,
    this.baslik,
    this.altBaslik,
    this.yukleniyor = false,
    this.not,
    super.key,
  });

  /// "450 ₺", "Çar 26 Kas · 14:30".
  final String? baslik;

  /// "2 hizmet · 1 sa 15 dk".
  final String? altBaslik;

  /// "Devam et".
  final String eylem;

  /// Null ise devre dışı.
  final VoidCallback? onPressed;

  /// Gönderiliyor.
  final bool yukleniyor;

  /// Butonun altındaki not.
  final String? not;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final title = baslik;
    final subtitle = altBaslik;
    final note = not;
    final button = OmvilaButon(
      etiket: eylem,
      onPressed: onPressed,
      yukleniyor: yukleniyor,
      boyut: OmvilaButonBoyutu.lg,
      tamGenislik: title == null,
    );
    return DecoratedBox(
      decoration: BoxDecoration(
        color: r.zeminYukseltilmis,
        borderRadius: const BorderRadius.vertical(
          top: Radius.circular(OmvilaKose.koseXl),
        ),
        boxShadow: context.omvilaGolgeler.golgeMd,
      ),
      child: SafeArea(
        top: false,
        child: Padding(
          padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              if (title == null)
                button
              else
                Row(
                  children: [
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            title,
                            style: OmvilaYazi.baslik3.copyWith(
                              color: r.metin,
                              fontWeight: FontWeight.w800,
                            ),
                          ),
                          if (subtitle != null)
                            Text(
                              subtitle,
                              style: OmvilaYazi.ikincil.copyWith(
                                color: r.metinIkincil,
                              ),
                            ),
                        ],
                      ),
                    ),
                    const SizedBox(width: OmvilaBosluk.bosluk3),
                    button,
                  ],
                ),
              if (note != null) ...[
                const SizedBox(height: OmvilaBosluk.bosluk2),
                Text(
                  note,
                  textAlign: TextAlign.center,
                  style: OmvilaYazi.aciklama.copyWith(color: r.metinIkincil),
                ),
              ],
            ],
          ),
        ),
      ),
    );
  }
}

/// Seçim satırı kartı: seçiliyse marka kenar ve yumuşak zemin, sağda onay
/// dairesi (YRHizmet, YRPersonel).
class SecimKarti extends StatelessWidget {
  /// Kart.
  const SecimKarti({
    required this.secili,
    required this.onTap,
    required this.child,
    this.sag,
    super.key,
  });

  /// Seçili.
  final bool secili;

  /// Seçer/bırakır.
  final VoidCallback onTap;

  /// Sol içerik.
  final Widget child;

  /// Onay dairesinin solundaki değer ("450 ₺", "Paketten").
  final Widget? sag;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final radius = BorderRadius.circular(OmvilaKose.koseLg);
    final right = sag;
    return Semantics(
      selected: secili,
      button: true,
      child: Material(
        color: secili ? r.zeminSecili : r.zeminYukseltilmis,
        shape: RoundedRectangleBorder(
          borderRadius: radius,
          side: BorderSide(
            color: secili ? r.marka : Colors.transparent,
            width: 2,
          ),
        ),
        child: InkWell(
          borderRadius: radius,
          onTap: onTap,
          child: Padding(
            padding: const EdgeInsets.symmetric(
              horizontal: OmvilaBosluk.bosluk4,
              vertical: OmvilaBosluk.bosluk3,
            ),
            child: Row(
              children: [
                Expanded(child: child),
                if (right != null) ...[
                  const SizedBox(width: OmvilaBosluk.bosluk2),
                  right,
                ],
                const SizedBox(width: OmvilaBosluk.bosluk3),
                ExcludeSemantics(
                  child: Container(
                    width: 24,
                    height: 24,
                    decoration: BoxDecoration(
                      shape: BoxShape.circle,
                      color: secili ? r.marka : Colors.transparent,
                      border: secili
                          ? null
                          : Border.all(color: r.cizgiGuclu, width: 1.5),
                    ),
                    child: secili
                        ? Icon(
                            LucideIcons.check,
                            size: 16,
                            color: r.markaUstu,
                          )
                        : null,
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

/// Bölüm başlığı ("Saç", "Öğleden sonra").
class BolumBasligi extends StatelessWidget {
  /// Başlık.
  const BolumBasligi(this.metin, {super.key});

  /// Metin.
  final String metin;

  @override
  Widget build(BuildContext context) => Padding(
    padding: const EdgeInsets.only(
      top: OmvilaBosluk.bosluk4,
      bottom: OmvilaBosluk.bosluk2,
    ),
    child: Semantics(
      header: true,
      child: Text(
        metin,
        style: OmvilaYazi.baslik3.copyWith(
          color: context.omvilaRenkler.metin,
          fontWeight: FontWeight.w800,
        ),
      ),
    ),
  );
}

/// Ekran başlığı ("Hizmet seçin") ve isteğe bağlı açıklama.
class EkranBasligi extends StatelessWidget {
  /// Başlık.
  const EkranBasligi(
    this.baslik, {
    this.aciklama,
    this.ikincil = false,
    super.key,
  });

  /// "Hizmet seçin".
  final String baslik;

  /// Sayfanın ilk başlığı değilse (YRHavale'de kartın altı) `baslik-1` ve
  /// açıklamayla arasında boşluk.
  final bool ikincil;

  /// "Kadıköy şubesi · birden fazla seçebilirsiniz".
  final String? aciklama;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final text = aciklama;
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Semantics(
          header: true,
          child: Text(
            baslik,
            style: (ikincil ? OmvilaYazi.baslik1 : OmvilaYazi.buyukBaslik)
                .copyWith(color: r.metin),
          ),
        ),
        if (text != null) ...[
          if (ikincil) const SizedBox(height: OmvilaBosluk.bosluk2),
          Text(text, style: OmvilaYazi.ikincil.copyWith(color: r.metinIkincil)),
        ],
      ],
    );
  }
}

/// Bilgi kutusu (ikon + metin).
class BilgiKutusu extends StatelessWidget {
  /// Kutu.
  const BilgiKutusu({
    required this.metin,
    this.ikon = LucideIcons.info,
    this.zemin,
    this.renk,
    super.key,
  });

  /// Metin.
  final String metin;

  /// İkon.
  final IconData ikon;

  /// Zemin; varsayılan ikincil zemin.
  final Color? zemin;

  /// Metin rengi; varsayılan ikincil metin.
  final Color? renk;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final color = renk ?? r.metinIkincil;
    return DecoratedBox(
      decoration: BoxDecoration(
        color: zemin ?? r.zeminIkincil,
        borderRadius: BorderRadius.circular(OmvilaKose.koseLg),
      ),
      child: Padding(
        padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Icon(ikon, size: 20, color: color),
            const SizedBox(width: OmvilaBosluk.bosluk3),
            Expanded(
              child: Text(
                metin,
                style: OmvilaYazi.ikincil.copyWith(color: color),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
