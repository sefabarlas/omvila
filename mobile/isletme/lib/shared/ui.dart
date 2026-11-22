import 'package:flutter/material.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Alt sayfa (RandevuIslem, KaporaOnay, OdemeAl): tutamak, üst köşeler
/// yuvarlak, klavye açılınca yukarı kayar.
Future<T?> showOmvilaSheet<T>(
  BuildContext context, {
  required WidgetBuilder builder,
}) {
  final r = context.omvilaRenkler;
  return showModalBottomSheet<T>(
    context: context,
    isScrollControlled: true,
    useSafeArea: true,
    // Sekme kabuğundaki ekranlarda (Sıra, Müşteriler) yüzen sekme çubuğunun
    // üstünde açılır.
    useRootNavigator: true,
    backgroundColor: r.zeminYukseltilmis,
    barrierColor: r.ustKatman,
    shape: const RoundedRectangleBorder(
      borderRadius: BorderRadius.vertical(
        top: Radius.circular(OmvilaKose.koseXl),
      ),
    ),
    builder: (context) => Padding(
      padding: EdgeInsets.only(
        bottom: MediaQuery.viewInsetsOf(context).bottom,
      ),
      child: SheetBody(child: builder(context)),
    ),
  );
}

/// Alt sayfa içeriği: tutamak ve kenar boşlukları (golden testinde de
/// doğrudan çizilir).
class SheetBody extends StatelessWidget {
  /// İçerik.
  const SheetBody({required this.child, super.key});

  /// İçerik.
  final Widget child;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    return SingleChildScrollView(
      padding: const EdgeInsets.fromLTRB(
        OmvilaBosluk.bosluk5,
        OmvilaBosluk.bosluk3,
        OmvilaBosluk.bosluk5,
        OmvilaBosluk.bosluk6,
      ),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Center(
            child: Container(
              width: 36,
              height: 4,
              decoration: BoxDecoration(
                color: r.tutamak,
                borderRadius: BorderRadius.circular(OmvilaKose.koseTam),
              ),
            ),
          ),
          const SizedBox(height: OmvilaBosluk.bosluk4),
          child,
        ],
      ),
    );
  }
}

/// Bölüm başlığı ("Nasıl geldi?", "Ödeme yöntemi").
class SectionLabel extends StatelessWidget {
  /// Başlık.
  const SectionLabel(this.text, {this.trailing, super.key});

  /// Metin.
  final String text;

  /// Yanındaki ikincil metin ("· Elif için boş").
  final String? trailing;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final extra = trailing;
    return Semantics(
      header: true,
      child: Text.rich(
        TextSpan(
          text: text,
          style: OmvilaYazi.govdeGuclu.copyWith(color: r.metin),
          children: [
            if (extra != null)
              TextSpan(
                text: ' · $extra',
                style: OmvilaYazi.aciklama.copyWith(color: r.metinIkincil),
              ),
          ],
        ),
      ),
    );
  }
}

/// Seçim çipi: seçiliyse koyu zemin ve tik.
class SelectChip extends StatelessWidget {
  /// Çip.
  const SelectChip({
    required this.label,
    required this.selected,
    required this.onTap,
    this.icon,
    super.key,
  });

  /// Metin.
  final String label;

  /// Seçili mi.
  final bool selected;

  /// Dokununca (null ise pasif).
  final VoidCallback? onTap;

  /// Seçili değilken ikon.
  final IconData? icon;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final foreground = selected
        ? r.metinTers
        : (onTap == null ? r.metinDevreDisi : r.metin);
    final shownIcon = selected ? LucideIcons.check : icon;
    final radius = BorderRadius.circular(OmvilaKose.koseTam);
    return Semantics(
      selected: selected,
      button: true,
      child: Material(
        color: selected ? r.zeminMarka : r.zeminYukseltilmis,
        shape: RoundedRectangleBorder(
          borderRadius: radius,
          side: BorderSide(color: selected ? r.zeminMarka : r.cizgi),
        ),
        child: InkWell(
          onTap: onTap,
          borderRadius: radius,
          child: ConstrainedBox(
            constraints: const BoxConstraints(
              minHeight: OmvilaBoyut.kontrolSm,
            ),
            child: Padding(
              padding: const EdgeInsets.symmetric(
                horizontal: OmvilaBosluk.bosluk3 + 2,
                vertical: OmvilaBosluk.bosluk2,
              ),
              child: Row(
                mainAxisSize: MainAxisSize.min,
                children: [
                  if (shownIcon != null) ...[
                    Icon(shownIcon, size: 16, color: foreground),
                    const SizedBox(width: OmvilaBosluk.bosluk2),
                  ],
                  Flexible(
                    child: Text(
                      label,
                      style: OmvilaYazi.etiket.copyWith(color: foreground),
                    ),
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}

/// Beyaz kart (RandevuDetay bölümleri).
class SurfaceCard extends StatelessWidget {
  /// Kart.
  const SurfaceCard({
    required this.child,
    this.padding = const EdgeInsets.all(OmvilaBosluk.bosluk4),
    this.color,
    super.key,
  });

  /// İçerik.
  final Widget child;

  /// İç boşluk.
  final EdgeInsetsGeometry padding;

  /// Zemin (varsayılan yükseltilmiş yüzey).
  final Color? color;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final radius = BorderRadius.circular(OmvilaKose.koseLg);
    // Material: içteki ListTile/Switch dalgası kartın zemininde görünür.
    return DecoratedBox(
      decoration: BoxDecoration(
        borderRadius: radius,
        boxShadow: color == null ? context.omvilaGolgeler.golgeSm : null,
      ),
      child: Material(
        color: color ?? r.zeminYukseltilmis,
        borderRadius: radius,
        child: Padding(padding: padding, child: child),
      ),
    );
  }
}

/// Etiket ve değer satırı ("Toplam 1.440 ₺"); satırlar arası çizgi.
class LabelValueRows extends StatelessWidget {
  /// Satırlar.
  const LabelValueRows({required this.rows, super.key});

  /// (etiket, değer, değer stili) üçlüleri.
  final List<LabelValue> rows;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    return Column(
      children: [
        for (var i = 0; i < rows.length; i++) ...[
          if (i > 0) Divider(height: 1, color: r.cizgi),
          Padding(
            padding: const EdgeInsets.symmetric(
              vertical: OmvilaBosluk.bosluk3,
            ),
            child: Row(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  rows[i].label,
                  style:
                      (rows[i].strong
                              ? OmvilaYazi.govdeGuclu
                              : OmvilaYazi.govde)
                          .copyWith(
                            color: rows[i].strong ? r.metin : r.metinIkincil,
                          ),
                ),
                const SizedBox(width: OmvilaBosluk.bosluk3),
                Expanded(
                  child: Text(
                    rows[i].value,
                    textAlign: TextAlign.end,
                    style: OmvilaYazi.govdeGuclu.copyWith(
                      color: rows[i].valueColor ?? r.metin,
                    ),
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

/// Etiket–değer satırı.
@immutable
class LabelValue {
  /// Satır.
  const LabelValue(
    this.label,
    this.value, {
    this.valueColor,
    this.strong = false,
  });

  /// Sol metin.
  final String label;

  /// Sağ metin.
  final String value;

  /// Değer rengi (kapora düşümü yeşil).
  final Color? valueColor;

  /// Kalın satır ("Alınacak").
  final bool strong;
}

/// Ad baş harfleri ("Selin Öztürk" → "SÖ").
String initialsOf(String fullName) {
  final parts = fullName.trim().split(RegExp(r'\s+'));
  final letters = [
    for (final p in parts.take(2))
      if (p.isNotEmpty) p.characters.first,
  ];
  return toUpperTr(letters.join());
}

/// Baş harf avatarı; ton addan türetilir (`OmvilaAvatar`, web ile aynı).
class InitialsAvatar extends StatelessWidget {
  /// Avatar.
  const InitialsAvatar({required this.name, this.size = 40, super.key});

  /// Ad.
  final String name;

  /// Çap.
  final double size;

  @override
  Widget build(BuildContext context) => OmvilaAvatar(ad: name, boyut: size);
}

/// Sunucudan gelen personel takvim rengi ("#0F6E5C"; veri, sabit renk
/// değil). Geçersizse marka rengi.
Color staffColor(BuildContext context, String hex) {
  final value = int.tryParse(hex.replaceFirst('#', ''), radix: 16);
  return value == null || hex.length != 7
      ? context.omvilaRenkler.marka
      : Color(value | 0xFF000000);
}

/// Hata gösterimi: boş durum ve "Tekrar dene".
class LoadError extends StatelessWidget {
  /// Hata.
  const LoadError({
    required this.title,
    required this.error,
    required this.onRetry,
    this.icon = LucideIcons.calendarDays,
    super.key,
  });

  /// "Takvim yüklenemedi".
  final String title;

  /// Hata nesnesi.
  final Object? error;

  /// "Tekrar dene".
  final VoidCallback onRetry;

  /// İkon.
  final IconData icon;

  @override
  Widget build(BuildContext context) {
    final e = error;
    return OmvilaBosDurum(
      ikon: icon,
      baslik: title,
      aciklama: errorMessage(e is ApiProblem ? e.code : null),
      eylemEtiketi: 'Tekrar dene',
      onEylem: onRetry,
    );
  }
}

/// Yükleniyor göstergesi.
class Loading extends StatelessWidget {
  /// Gösterge.
  const Loading({super.key});

  @override
  Widget build(BuildContext context) => const Padding(
    padding: EdgeInsets.all(OmvilaBosluk.bosluk12),
    child: Center(
      child: CircularProgressIndicator(semanticsLabel: 'Yükleniyor'),
    ),
  );
}

/// Hata mesajını alt bilgi olarak gösterir.
void showProblem(BuildContext context, Object error) {
  ScaffoldMessenger.of(context).showSnackBar(
    SnackBar(
      content: Text(errorMessage(error is ApiProblem ? error.code : null)),
    ),
  );
}

/// Sayfa kenar boşluğunu yok sayıp ekran genişliğine taşan, sabit
/// yükseklikli içerik (gün şeridi kendi kenar boşluğunu verir).
class FullBleed extends StatelessWidget {
  /// Taşan içerik.
  const FullBleed({required this.child, required this.height, super.key});

  /// İçerik.
  final Widget child;

  /// Yükseklik.
  final double height;

  @override
  Widget build(BuildContext context) {
    final width = MediaQuery.sizeOf(context).width;
    return SizedBox(
      height: height,
      child: OverflowBox(minWidth: width, maxWidth: width, child: child),
    );
  }
}
