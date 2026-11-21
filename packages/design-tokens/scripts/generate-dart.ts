// Mobil token kaynağından (src/tokens.mobil.json) Flutter teması üretir: packages/omvila_ui/lib/theme/omvila_theme.dart.
// Sınıf ve alan adları reference/omvila_theme.dart.txt ile aynıdır. Ham palet (petrol-*, kum-*…) dışarı açılmaz;
// hex yalnız bu üretilmiş dosyada geçer. Saf fonksiyonlar; dosya yazımı build.ts'te.
import { resolveColor } from './generate.ts';
import type { Theme, TokenSource } from './generate.ts';

/** Ham palet adımları (`petrol-600`, `gri-0`…); semantik tokenlar bunlara başvurur, Dart'a açılmaz. */
export const primitivePattern = /^(petrol|kum|mercan|gri|basari|uyari|hata|bilgi)-\d+$/;

const themes: Record<Theme, string> = { light: 'acik', dark: 'koyu' };

/** `zemin-yukseltilmis` → `zeminYukseltilmis`, `avatar-1-zemin` → `avatar1Zemin`. */
export function camel(name: string): string {
  return name.replace(/-([a-z0-9])/g, (_, c: string) => c.toUpperCase());
}

function px(value: string): number {
  const match = /^(-?[\d.]+)px$/.exec(value);
  if (!match?.[1]) {
    throw new Error(`px değeri bekleniyor: ${value}`);
  }
  return Number(match[1]);
}

/** Dart sayı sabiti (double bağlamında; prefer_int_literals): 4 → `4`, 1.1176 → `1.118`. */
function dbl(value: number): string {
  return String(Math.round(value * 1000) / 1000);
}

function rgba(args: string): string {
  const parts = args.split(',').map((p) => p.trim());
  if (parts.length !== 4) {
    throw new Error(`rgba(r, g, b, a) bekleniyor: ${args}`);
  }
  return `Color.fromRGBO(${parts.join(', ')})`;
}

function dartColor(value: string): string {
  const hex = /^#([0-9A-Fa-f]{6})$/.exec(value);
  if (hex?.[1]) {
    return `Color(0xFF${hex[1].toUpperCase()})`;
  }
  const match = /^rgba\((.+)\)$/.exec(value);
  if (match?.[1]) {
    return rgba(match[1]);
  }
  throw new Error(`Desteklenmeyen renk: ${value}`);
}

/** CSS gölgesi (`0 1px 2px rgba(…), 0 0 0 2px #FFFFFF`) → Dart `BoxShadow` listesi. */
export function dartShadows(css: string): string {
  const layer =
    /^(-?[\d.]+)(?:px)?\s+(-?[\d.]+)(?:px)?\s+([\d.]+)(?:px)?(?:\s+(-?[\d.]+)px)?\s+(rgba\([^)]+\)|#[0-9A-Fa-f]{6})$/;
  const shadows = css.split(/,(?![^(]*\))/).map((part) => {
    const m = layer.exec(part.trim());
    if (!m) {
      throw new Error(`Gölge çözülemedi: ${css}`);
    }
    const [, x = '0', y = '0', blur = '0', spread, color = ''] = m;
    // Varsayılan değerler (Offset.zero, 0) yazılmaz (avoid_redundant_argument_values).
    const fields = [
      `color: ${dartColor(color)}`,
      ...(Number(x) || Number(y) ? [`offset: Offset(${dbl(Number(x))}, ${dbl(Number(y))})`] : []),
      ...(Number(blur) ? [`blurRadius: ${dbl(Number(blur))}`] : []),
      ...(spread && Number(spread) ? [`spreadRadius: ${dbl(Number(spread))}`] : []),
    ];
    return `BoxShadow(${fields.join(', ')})`;
  });
  return `[${shadows.join(', ')}]`;
}

function doc(usage: string | undefined, indent: string): string[] {
  return usage ? [`${indent}/// ${usage}`] : [];
}

function themeExtension(
  className: string,
  summary: string,
  type: string,
  fields: { name: string; usage?: string | undefined; values: Record<Theme, string> }[],
  lerp: (field: string) => string,
): string[] {
  const lines = [
    `/// ${summary}`,
    '@immutable',
    `class ${className} extends ThemeExtension<${className}> {`,
  ];
  lines.push('  /// Temalar için `acik` ve `koyu` sabitlerini kullanın.', `  const ${className}({`);
  lines.push(...fields.map((f) => `    required this.${f.name},`));
  lines.push('  });', '');
  for (const theme of ['light', 'dark'] as const) {
    lines.push(
      `  /// ${theme === 'light' ? 'Açık' : 'Koyu'} tema değerleri.`,
      `  static const ${themes[theme]} = ${className}(`,
    );
    lines.push(...fields.map((f) => `    ${f.name}: ${f.values[theme]},`));
    lines.push('  );', '');
  }
  for (const f of fields) {
    lines.push(...doc(f.usage, '  '), `  final ${type} ${f.name};`);
  }
  lines.push('', '  @override', `  ${className} copyWith({`);
  lines.push(...fields.map((f) => `    ${type}? ${f.name},`));
  lines.push('  }) {', `    return ${className}(`);
  lines.push(...fields.map((f) => `      ${f.name}: ${f.name} ?? this.${f.name},`));
  lines.push('    );', '  }', '');
  lines.push('  @override', `  ${className} lerp(ThemeExtension<${className}>? other, double t) {`);
  lines.push(
    `    if (other is! ${className}) {`,
    '      return this;',
    '    }',
    `    return ${className}(`,
  );
  lines.push(...fields.map((f) => `      ${f.name}: ${lerp(f.name)},`));
  lines.push('    );', '  }', '}', '');
  return lines;
}

interface DartConstant {
  name: string;
  usage?: string | undefined;
  value: string;
  type: string;
}

function constants(className: string, summary: string, tokens: DartConstant[]): string[] {
  const lines = [`/// ${summary}`, `abstract final class ${className} {`];
  tokens.forEach((t, i) => {
    if (i > 0) lines.push('');
    lines.push(...doc(t.usage, '  '), `  static const ${t.type} ${camel(t.name)} = ${t.value};`);
  });
  lines.push('}', '');
  return lines;
}

function motion(source: TokenSource): string[] {
  const tokens = (source.hareket?.tokens ?? []).map((t): DartConstant => {
    const ms = /^(\d+)ms$/.exec(t.value);
    if (ms?.[1]) {
      return { ...t, type: 'Duration', value: `Duration(milliseconds: ${ms[1]})` };
    }
    const curve = /^cubic-bezier\((.+)\)$/.exec(t.value);
    if (curve?.[1]) {
      return { ...t, type: 'Cubic', value: `Cubic(${curve[1]})` };
    }
    throw new Error(`Hareket tokenı ms veya cubic-bezier(…) olmalı: ${t.name}`);
  });
  return constants(
    'OmvilaHareket',
    'Süreler ve eğriler. Azaltılmış hareket açıkken kayma ve büyüme yerine yalnız `sureAzaltilmis` opaklık geçişi kalır.',
    tokens,
  );
}

function typography(source: TokenSource): string[] {
  const families = source.type.fonts.map((f) => f.family.replace(/\s+/g, ''));
  const [family, ...fallback] = families;
  if (!family) {
    throw new Error('Yazı tipi tanımı yok.');
  }
  const lines = [
    '/// Yazı stilleri. Renk vermez; rengi bağlamdan alır. Yazı tipi `omvila_ui` paketinin varlığıdır.',
    'abstract final class OmvilaYazi {',
    '  /// Yazı ailesi (Latin alt kümesi).',
    `  static const String aile = '${family}';`,
    '',
    '  /// Latin Extended alt kümesi (ğ, ş, İ, ı…); aile ile birlikte yüklenir.',
    `  static const List<String> yedekAileler = [${fallback.map((f) => `'${f}'`).join(', ')}];`,
    '',
    '  /// Varlıkların bulunduğu paket.',
    "  static const String paket = 'omvila_ui';",
  ];
  for (const style of source.type.groups.flatMap((g) => g.styles)) {
    const size = px(style.fontSize);
    const fields = [
      'fontFamily: aile',
      'fontFamilyFallback: yedekAileler',
      'package: paket',
      `fontSize: ${dbl(size)}`,
      `height: ${dbl(px(style.lineHeight) / size)}`,
      `fontWeight: FontWeight.w${String(style.fontWeight)}`,
    ];
    if (style.letterSpacing) {
      const em = /^(-?[\d.]+)em$/.exec(style.letterSpacing);
      fields.push(
        `letterSpacing: ${dbl(em?.[1] ? Number(em[1]) * size : px(style.letterSpacing))}`,
      );
    }
    if (style.usage?.includes('tabular-nums')) {
      fields.push('fontFeatures: [FontFeature.tabularFigures()]');
    }
    lines.push(
      '',
      ...doc(style.usage, '  '),
      `  static const TextStyle ${camel(style.name)} = TextStyle(`,
    );
    lines.push(...fields.map((f) => `    ${f},`), '  );');
  }
  lines.push('}', '');
  return lines;
}

/** Material ThemeData; renkler yalnız semantik tokenlardan. Şablon reference/omvila_theme.dart.txt'teki OmvilaTema. */
const themeData = `/// Uygulamanın ThemeData'sı. \`MaterialApp(theme: OmvilaTema.acik(), darkTheme: OmvilaTema.koyu())\`
abstract final class OmvilaTema {
  /// Açık tema.
  static ThemeData acik() =>
      _tema(OmvilaRenkler.acik, OmvilaGolgeler.acik, Brightness.light);

  /// Koyu tema.
  static ThemeData koyu() =>
      _tema(OmvilaRenkler.koyu, OmvilaGolgeler.koyu, Brightness.dark);

  static RoundedRectangleBorder _kose(double k) =>
      RoundedRectangleBorder(borderRadius: BorderRadius.circular(k));

  static OutlineInputBorder _girisKenari(Color renk, [double kalinlik = 1]) =>
      OutlineInputBorder(
        borderRadius: BorderRadius.circular(OmvilaKose.koseMd),
        borderSide: BorderSide(color: renk, width: kalinlik),
      );

  static ThemeData _tema(OmvilaRenkler r, OmvilaGolgeler g, Brightness b) {
    final scheme = ColorScheme(
      brightness: b,
      primary: r.marka,
      onPrimary: r.markaUstu,
      primaryContainer: r.zeminSecili,
      onPrimaryContainer: r.metinMarka,
      secondary: r.vurgu,
      onSecondary: r.vurguUstu,
      error: r.hata,
      onError: r.markaUstu,
      surface: r.zeminYukseltilmis,
      onSurface: r.metin,
      onSurfaceVariant: r.metinIkincil,
      surfaceContainerHighest: r.zeminIkincil,
      outline: r.cizgiGuclu,
      outlineVariant: r.cizgi,
      scrim: r.ustKatman,
    );
    return ThemeData(
      brightness: b,
      colorScheme: scheme,
      fontFamily: OmvilaYazi.aile,
      fontFamilyFallback: OmvilaYazi.yedekAileler,
      package: OmvilaYazi.paket,
      scaffoldBackgroundColor: r.zemin,
      materialTapTargetSize: MaterialTapTargetSize.padded,
      extensions: [r, g],
      textTheme: const TextTheme(
        displaySmall: OmvilaYazi.buyukBaslik,
        headlineSmall: OmvilaYazi.baslik1,
        titleLarge: OmvilaYazi.baslik2,
        titleMedium: OmvilaYazi.baslik3,
        bodyLarge: OmvilaYazi.govde,
        bodyMedium: OmvilaYazi.ikincil,
        labelLarge: OmvilaYazi.etiket,
        labelMedium: OmvilaYazi.etiketKucuk,
        labelSmall: OmvilaYazi.aciklama,
      ).apply(bodyColor: r.metin, displayColor: r.metin),
      appBarTheme: AppBarTheme(
        backgroundColor: r.zemin,
        foregroundColor: r.metin,
        elevation: 0,
        scrolledUnderElevation: 0,
        centerTitle: true,
        toolbarHeight: OmvilaBoyut.ustBar,
        titleTextStyle: OmvilaYazi.ustBarBasligi.copyWith(color: r.metin),
      ),
      filledButtonTheme: FilledButtonThemeData(
        style: FilledButton.styleFrom(
          backgroundColor: r.marka,
          foregroundColor: r.markaUstu,
          disabledBackgroundColor: r.zeminDevreDisi,
          disabledForegroundColor: r.metinDevreDisi,
          minimumSize: const Size(64, OmvilaBoyut.kontrolMd),
          textStyle: OmvilaYazi.etiket,
          shape: _kose(OmvilaKose.koseMd),
        ),
      ),
      outlinedButtonTheme: OutlinedButtonThemeData(
        style: OutlinedButton.styleFrom(
          foregroundColor: r.metin,
          disabledForegroundColor: r.metinDevreDisi,
          side: BorderSide(color: r.cizgiGuclu),
          minimumSize: const Size(64, OmvilaBoyut.kontrolMd),
          textStyle: OmvilaYazi.etiket,
          shape: _kose(OmvilaKose.koseMd),
        ),
      ),
      textButtonTheme: TextButtonThemeData(
        style: TextButton.styleFrom(
          foregroundColor: r.metinMarka,
          minimumSize: const Size(OmvilaBoyut.dokunmaMin, OmvilaBoyut.dokunmaMin),
          textStyle: OmvilaYazi.etiket,
        ),
      ),
      inputDecorationTheme: InputDecorationTheme(
        filled: true,
        fillColor: r.zeminYukseltilmis,
        contentPadding: const EdgeInsets.symmetric(
          horizontal: OmvilaBosluk.bosluk4,
          vertical: 14,
        ),
        border: _girisKenari(r.cizgiGuclu),
        enabledBorder: _girisKenari(r.cizgiGuclu),
        focusedBorder: _girisKenari(r.odak, 2),
        errorBorder: _girisKenari(r.hata),
        focusedErrorBorder: _girisKenari(r.hata, 2),
        labelStyle: OmvilaYazi.ikincil.copyWith(color: r.metinIkincil),
        hintStyle: OmvilaYazi.govde.copyWith(color: r.metinIkincil),
        errorStyle: OmvilaYazi.aciklama.copyWith(color: r.hata),
      ),
      cardTheme: CardThemeData(
        color: r.zeminYukseltilmis,
        elevation: 0,
        margin: EdgeInsets.zero,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(OmvilaKose.koseLg),
          side: BorderSide(color: r.cizgi),
        ),
      ),
      navigationBarTheme: NavigationBarThemeData(
        backgroundColor: r.tabbarZemin,
        height: OmvilaBoyut.tabbar,
        indicatorColor: r.tabbarAktifZemin,
        elevation: 0,
        iconTheme: WidgetStateProperty.resolveWith(
          (s) => IconThemeData(
            color: s.contains(WidgetState.selected) ? r.tabbarAktif : r.tabbarPasif,
            size: 24,
          ),
        ),
        labelTextStyle: WidgetStateProperty.resolveWith(
          (s) => OmvilaYazi.sekme.copyWith(
            color: s.contains(WidgetState.selected) ? r.tabbarAktif : r.tabbarPasif,
          ),
        ),
      ),
      bottomSheetTheme: BottomSheetThemeData(
        backgroundColor: r.zeminYukseltilmis,
        showDragHandle: true,
        dragHandleColor: r.tutamak,
        modalBarrierColor: r.ustKatman,
        shape: const RoundedRectangleBorder(
          borderRadius: BorderRadius.vertical(
            top: Radius.circular(OmvilaKose.koseXl),
          ),
        ),
      ),
      dialogTheme: DialogThemeData(
        backgroundColor: r.zeminYukseltilmis,
        barrierColor: r.ustKatman,
        shape: _kose(OmvilaKose.koseXl),
      ),
      chipTheme: ChipThemeData(
        shape: _kose(OmvilaKose.koseTam),
        side: BorderSide(color: r.cizgiGuclu),
        selectedColor: r.zeminSecili,
        labelStyle: OmvilaYazi.etiketKucuk.copyWith(color: r.metin),
      ),
      snackBarTheme: SnackBarThemeData(
        behavior: SnackBarBehavior.floating,
        backgroundColor: r.zeminMarka,
        contentTextStyle: OmvilaYazi.ikincil.copyWith(color: r.metinTers),
        actionTextColor: r.metinTers,
        shape: _kose(OmvilaKose.koseMd),
      ),
      dividerTheme: DividerThemeData(color: r.cizgi, thickness: 1, space: 1),
      progressIndicatorTheme: ProgressIndicatorThemeData(color: r.marka),
      switchTheme: SwitchThemeData(
        trackColor: WidgetStateProperty.resolveWith(
          (s) => s.contains(WidgetState.selected) ? r.marka : r.cizgiGuclu,
        ),
        thumbColor: WidgetStatePropertyAll(r.zeminYukseltilmis),
        trackOutlineColor: const WidgetStatePropertyAll(Colors.transparent),
      ),
    );
  }
}

/// Bağlamdan tema uzantıları: \`context.omvilaRenkler.marka\`.
extension OmvilaTemaBaglami on BuildContext {
  /// Semantik renkler.
  OmvilaRenkler get omvilaRenkler => Theme.of(this).extension<OmvilaRenkler>()!;

  /// Gölgeler.
  OmvilaGolgeler get omvilaGolgeler =>
      Theme.of(this).extension<OmvilaGolgeler>()!;
}
`;

/** packages/omvila_ui/lib/theme/omvila_theme.dart içeriği. */
export function generateDart(source: TokenSource): string {
  const semantic = source.color.tokens.filter((t) => !primitivePattern.test(t.name));
  const colorFields = semantic.map((t) => ({
    name: camel(t.name),
    usage: t.usage,
    values: {
      light: dartColor(resolveColor(source, t.name, 'light')),
      dark: dartColor(resolveColor(source, t.name, 'dark')),
    },
  }));
  const shadowFields = source.shadow.tokens.map((t) => ({
    name: camel(t.name),
    usage: t.usage,
    values: {
      light: dartShadows(typeof t.value === 'string' ? t.value : t.value.light),
      dark: dartShadows(typeof t.value === 'string' ? t.value : t.value.dark),
    },
  }));
  const pxTokens = (tokens: { name: string; usage?: string | undefined; value: string }[]) =>
    tokens.map((t): DartConstant => ({ ...t, type: 'double', value: dbl(px(t.value)) }));

  const lines = [
    '// Üretildi: pnpm --filter @omvila/design-tokens build. Elle düzenlemeyin; kaynak',
    '// packages/design-tokens/src/tokens.mobil.json. Renkler yalnız semantik adlarla açılır.',
    '// ignore_for_file: lines_longer_than_80_chars, sort_constructors_first',
    '// dart format off',
    "import 'package:flutter/material.dart';",
    '',
    ...themeExtension(
      'OmvilaRenkler',
      'Semantik renkler. Kullanım: `context.omvilaRenkler.marka`.',
      'Color',
      colorFields,
      (f) => `Color.lerp(${f}, other.${f}, t)!`,
    ),
    ...themeExtension(
      'OmvilaGolgeler',
      'Gölgeler (açık ve koyu temada farklı). Kullanım: `context.omvilaGolgeler.golgeSm`.',
      'List<BoxShadow>',
      shadowFields,
      (f) => `BoxShadow.lerpList(${f}, other.${f}, t)!`,
    ),
    ...constants(
      'OmvilaBosluk',
      'Boşluk ölçeği (mantıksal piksel).',
      pxTokens(source.spacing.tokens),
    ),
    ...constants('OmvilaKose', 'Köşe yarıçapları.', pxTokens(source.radius.tokens)),
    ...constants(
      'OmvilaBoyut',
      'Boyutlar: dokunma alanı, kontrol yükseklikleri, çubuklar.',
      pxTokens(source.boyut?.tokens ?? []),
    ),
    ...motion(source),
    ...typography(source),
    themeData,
  ];
  return lines
    .join('\n')
    .replace(/\n{3,}/g, '\n\n')
    .replace(/\n*$/, '\n');
}
