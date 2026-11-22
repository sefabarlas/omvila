import { readFileSync } from 'node:fs';
import { dirname, join } from 'node:path';
import { fileURLToPath } from 'node:url';
import { describe, expect, it } from 'vitest';
import { dartThemePath, loadSources, outputs } from './build.ts';
import { camel, primitivePattern } from './generate-dart.ts';
import { resolveColor, scales } from './generate.ts';
import type { Theme, TokenSource } from './generate.ts';

// Kural: packages/design-tokens/AGENTS.md — her metin/zemin çifti iki temada ≥ 4.5:1 (WCAG AA); dist/ kaynakla aynı.
const root = join(dirname(fileURLToPath(import.meta.url)), '..');
const sources = loadSources();
const themes: Theme[] = ['light', 'dark'];

/** Metin rengi ve üzerinde kullanıldığı zemin (token kullanım notlarından). Devre dışı metin WCAG'de muaftır. */
const textPairs: [string, string][] = [
  ...[
    'zemin',
    'zemin-yukseltilmis',
    'zemin-ikincil',
    'zemin-marka-yumusak',
    'zemin-hover',
    'zemin-secili',
  ].map((bg): [string, string] => ['metin', bg]),
  ...['zemin', 'zemin-yukseltilmis', 'zemin-ikincil'].map((bg): [string, string] => [
    'metin-ikincil',
    bg,
  ]),
  ['metin-vurgu', 'zemin-vurgu-yumusak'],
  ['metin-marka', 'zemin-marka-yumusak'],
  ['metin-marka', 'zemin-yukseltilmis'],
  ['marka-ustu', 'marka'],
  ['vurgu-ustu', 'vurgu'],
  ['metin-ters', 'zemin-marka'],
  ['blok-kum-metin', 'blok-kum'],
  ['blok-marka-hap-metin', 'blok-marka-hap'],
  ['blok-vurgu-metin', 'blok-vurgu'],
  ...['onay', 'kapora', 'onaylandi', 'geldi', 'tamamlandi', 'gelmedi', 'iptal'].map(
    (s): [string, string] => [`durum-${s}-metin`, `durum-${s}-zemin`],
  ),
  ['sidebar-metin', 'sidebar-zemin'],
  ['sidebar-metin-aktif', 'sidebar-zemin'],
  ['sidebar-metin-aktif', 'sidebar-secili'],
  ...['basari', 'uyari', 'hata', 'bilgi'].map((s): [string, string] => [s, `${s}-zemin`]),
  ...[1, 2, 3].map((n): [string, string] => [
    `avatar-${String(n)}-metin`,
    `avatar-${String(n)}-zemin`,
  ]),
];

function luminance(hex: string): number {
  const channels = [1, 3, 5].map((i) => parseInt(hex.slice(i, i + 2), 16) / 255);
  const [r, g, b] = channels.map((c) =>
    c <= 0.03928 ? c / 12.92 : ((c + 0.055) / 1.055) ** 2.4,
  ) as [number, number, number];
  return 0.2126 * r + 0.7152 * g + 0.0722 * b;
}

function contrast(a: string, b: string): number {
  const [hi, lo] = [luminance(a), luminance(b)].sort((x, y) => y - x) as [number, number];
  return (hi + 0.05) / (lo + 0.05);
}

const has = (source: TokenSource, name: string) => source.color.tokens.some((t) => t.name === name);

describe.each(scales)('%s', (scale) => {
  const source = sources[scale];

  it.each(themes)('metin/zemin çiftleri %s temada en az 4.5:1', (theme) => {
    const failures = textPairs
      .filter(([fg, bg]) => has(source, fg) && has(source, bg))
      .map(([fg, bg]) => ({
        fg,
        bg,
        ratio: contrast(resolveColor(source, fg, theme), resolveColor(source, bg, theme)),
      }))
      .filter((p) => p.ratio < 4.5)
      .map((p) => `${p.fg} / ${p.bg}: ${p.ratio.toFixed(2)}`);
    expect(failures).toEqual([]);
  });

  it('her renk iki temada da çözülür ve geçerli bir CSS rengidir', () => {
    for (const token of source.color.tokens) {
      for (const theme of themes) {
        expect(resolveColor(source, token.name, theme), `${token.name} [${theme}]`).toMatch(
          /^(#[0-9A-Fa-f]{6}|rgba\(.+\))$/,
        );
      }
    }
  });

  it('token adları tekil', () => {
    const names = [
      ...source.color.tokens,
      ...source.shadow.tokens,
      ...source.spacing.tokens,
      ...source.radius.tokens,
      ...(source.boyut?.tokens ?? []),
      ...(source.hareket?.tokens ?? []),
    ].map((t) => t.name);
    expect(new Set(names).size).toBe(names.length);
  });
});

it('çiftlerdeki tokenlar en az bir ölçekte var (yazım hatası yok)', () => {
  const missing = textPairs
    .flat()
    .filter((name) => !scales.some((scale) => has(sources[scale], name)));
  expect(missing).toEqual([]);
});

describe('Flutter teması (omvila_ui/lib/theme/omvila_theme.dart)', () => {
  const dart = outputs()[dartThemePath] ?? '';
  const mobil = sources.mobil;

  it('referans dosyadaki sınıfları üretir', () => {
    for (const name of [
      'OmvilaRenkler',
      'OmvilaBosluk',
      'OmvilaKose',
      'OmvilaBoyut',
      'OmvilaYazi',
      'OmvilaTema',
    ]) {
      expect(dart, name).toMatch(new RegExp(`class ${name}\\b`));
    }
  });

  it('her semantik renk açık ve koyu temada alan olarak var', () => {
    for (const token of mobil.color.tokens.filter((t) => !primitivePattern.test(t.name))) {
      expect(dart, token.name).toContain(`final Color ${camel(token.name)};`);
      expect(
        dart.match(new RegExp(`^    ${camel(token.name)}: Color`, 'gm')),
        token.name,
      ).toHaveLength(2);
    }
  });

  it('ham palet adları dışarı açılmaz', () => {
    const primitives = mobil.color.tokens
      .filter((t) => primitivePattern.test(t.name))
      .map((t) => camel(t.name));
    expect(primitives.length).toBeGreaterThan(0);
    for (const name of primitives) {
      expect(dart, name).not.toMatch(new RegExp(`\\b${name}\\b`));
    }
    expect(dart).not.toContain('OmvilaRampa');
  });

  it('boşluk, köşe, boyut ve hareket tokenlarının hepsi sabit olarak var', () => {
    const names = [
      ...mobil.spacing.tokens,
      ...mobil.radius.tokens,
      ...(mobil.boyut?.tokens ?? []),
      ...(mobil.hareket?.tokens ?? []),
    ].map((t) => camel(t.name));
    expect(names).toContain('sureAnlik');
    for (const name of names) {
      expect(dart, name).toMatch(new RegExp(`static const \\w+ ${name} = `));
    }
    expect(dart).toContain('static const Duration sureOrta = Duration(milliseconds: 240);');
    expect(dart).toContain('static const Cubic egriStandart = Cubic(0.2, 0, 0, 1);');
  });

  it('yazı stilleri: satır yüksekliği oranı ve em harf aralığı piksele çevrilir', () => {
    expect(dart).toMatch(
      /buyukBaslik = TextStyle\([^)]*fontSize: 34,\s+height: 1\.118,\s+fontWeight: FontWeight\.w800,\s+letterSpacing: -1\.19,/,
    );
  });
});

it('üretilen dosyalar kaynakla güncel (pnpm --filter @omvila/design-tokens build)', () => {
  for (const [path, content] of Object.entries(outputs())) {
    expect(readFileSync(join(root, path), 'utf8'), path).toBe(content);
  }
});
