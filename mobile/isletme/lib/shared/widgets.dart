import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Sekme ve alt ekran başlığı: geri oku (yığında önceki sayfa varsa), üst
/// satır ("Randevusuz gelenler", "Kadıköy"), büyük başlık ve sağda eylem.
class ScreenTitle extends StatelessWidget {
  /// Başlık.
  const ScreenTitle({
    required this.title,
    this.eyebrow,
    this.trailing,
    this.showBack = false,
    super.key,
  });

  /// Büyük başlık.
  final String title;

  /// Üst satır.
  final String? eyebrow;

  /// Sağdaki eylem.
  final Widget? trailing;

  /// Geri oku (yığında önceki sayfa varsa çizilir).
  final bool showBack;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final top = eyebrow;
    final action = trailing;
    final canPop = showBack && (GoRouter.maybeOf(context)?.canPop() ?? false);
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        if (canPop)
          Padding(
            padding: const EdgeInsets.only(bottom: OmvilaBosluk.bosluk2),
            child: IconButton(
              onPressed: () => context.pop(),
              tooltip: 'Geri',
              style: IconButton.styleFrom(
                padding: EdgeInsets.zero,
                alignment: Alignment.centerLeft,
              ),
              icon: Icon(LucideIcons.arrowLeft, color: r.metin),
            ),
          ),
        Row(
          crossAxisAlignment: CrossAxisAlignment.end,
          children: [
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  if (top != null)
                    Text(
                      top,
                      style: OmvilaYazi.ikincil.copyWith(
                        color: r.metinIkincil,
                        fontWeight: FontWeight.w600,
                      ),
                    ),
                  Semantics(
                    header: true,
                    child: Text(
                      title,
                      style: OmvilaYazi.buyukBaslik.copyWith(color: r.metin),
                    ),
                  ),
                ],
              ),
            ),
            ?action,
          ],
        ),
      ],
    );
  }
}

/// Etiket rozeti tonu.
enum TagTone {
  /// "Düzenli".
  marka,

  /// "Yeni".
  vurgu,

  /// "2 kez gelmedi".
  hata,

  /// Diğer.
  notr,
}

/// Sunucunun sistem etiketinin tonu (salon paneliyle aynı kural, T-46 (4)).
TagTone tagToneOf(String tag) {
  if (tag == 'Düzenli') {
    return TagTone.marka;
  }

  if (tag == 'Yeni' || tag == 'Yeni müşteri') {
    return TagTone.vurgu;
  }

  return RegExp('gelmedi', caseSensitive: false).hasMatch(tag)
      ? TagTone.hata
      : TagTone.notr;
}

/// Küçük hap rozet ("Düzenli", "Yeni").
class TagPill extends StatelessWidget {
  /// Rozet.
  const TagPill(this.label, {this.tone, super.key});

  /// Metin.
  final String label;

  /// Ton (verilmezse etiketten).
  final TagTone? tone;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final (background, foreground) = switch (tone ?? tagToneOf(label)) {
      TagTone.marka => (r.zeminMarkaYumusak, r.metinMarka),
      TagTone.vurgu => (r.zeminVurguYumusak, r.metinVurgu),
      TagTone.hata => (r.hataZemin, r.hata),
      TagTone.notr => (r.zeminIkincil, r.metinIkincil),
    };
    return Container(
      padding: const EdgeInsets.symmetric(
        horizontal: OmvilaBosluk.bosluk2 + 2,
        vertical: OmvilaBosluk.bosluk1,
      ),
      decoration: BoxDecoration(
        color: background,
        borderRadius: BorderRadius.circular(OmvilaKose.koseTam),
      ),
      child: Text(
        label,
        style: OmvilaYazi.etiketKucuk.copyWith(color: foreground),
      ),
    );
  }
}

/// Sayaç kutusu ("ZİYARET 12", "RANDEVU 94").
class StatTile extends StatelessWidget {
  /// Sayaç.
  const StatTile({
    required this.label,
    required this.value,
    this.tone,
    super.key,
  });

  /// Üst etiket (büyük harfle yazılır).
  final String label;

  /// Değer.
  final String value;

  /// Zemin tonu: null beyaz kart.
  final BentoTonu? tone;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final (background, foreground) = switch (tone) {
      BentoTonu.marka => (r.blokMarka, r.metinTers),
      BentoTonu.vurgu => (r.blokVurgu, r.blokVurguMetin),
      BentoTonu.kum => (r.blokKum, r.blokKumMetin),
      null => (r.zeminYukseltilmis, r.metin),
    };
    return Semantics(
      container: true,
      label: '$label $value',
      excludeSemantics: true,
      child: Container(
        padding: const EdgeInsets.all(OmvilaBosluk.bosluk3 + 2),
        decoration: BoxDecoration(
          color: background,
          borderRadius: BorderRadius.circular(OmvilaKose.koseLg),
          boxShadow: tone == null ? context.omvilaGolgeler.golgeSm : null,
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              toUpperTr(label),
              style: OmvilaYazi.aciklama.copyWith(
                color: foreground,
                fontWeight: FontWeight.w700,
                letterSpacing: 0.6,
              ),
            ),
            const SizedBox(height: OmvilaBosluk.bosluk1),
            // Uzun tutar ("14.860 ₺") kutuya sığacak kadar küçülür.
            FittedBox(
              fit: BoxFit.scaleDown,
              alignment: Alignment.centerLeft,
              child: Text(
                value,
                maxLines: 1,
                style: OmvilaYazi.baslik1.copyWith(
                  color: foreground,
                  fontWeight: FontWeight.w800,
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}

/// Bölüm başlığı ve sağda metin bağlantısı ("Geçmiş · Tümü").
class SectionHeader extends StatelessWidget {
  /// Başlık.
  const SectionHeader(this.title, {this.action, this.onAction, super.key});

  /// Başlık.
  final String title;

  /// Bağlantı metni.
  final String? action;

  /// Bağlantı.
  final VoidCallback? onAction;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final label = action;
    return Row(
      children: [
        Expanded(
          child: Semantics(
            header: true,
            child: Text(
              title,
              style: OmvilaYazi.baslik3.copyWith(
                color: r.metin,
                fontWeight: FontWeight.w800,
              ),
            ),
          ),
        ),
        if (label != null)
          TextButton(
            onPressed: onAction,
            child: Text(
              label,
              style: OmvilaYazi.etiket.copyWith(color: r.metinMarka),
            ),
          ),
      ],
    );
  }
}

/// Yerel gün etiketi: "Bugün", "Dün", "Yarın", aynı yıl "21 Eki".
String relativeDay(String localDate, String today) {
  if (localDate == today) {
    return 'Bugün';
  }

  if (localDate == addDays(today, -1)) {
    return 'Dün';
  }

  if (localDate == addDays(today, 1)) {
    return 'Yarın';
  }

  return localDate.substring(0, 4) == today.substring(0, 4)
      // "21 Kas Cum" → "21 Kas".
      ? formatDate(localDate, DateStyle.short).split(' ').take(2).join(' ')
      : formatDate(localDate, DateStyle.numeric);
}
