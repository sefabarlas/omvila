// Token kaynağından (src/tokens.<ölçek>.json) CSS değişkenleri ve tip güvenli ad listeleri üretir.
// Kurallar: packages/design-tokens/AGENTS.md. Saf fonksiyonlar; dosya yazımı build.ts'te.

export type Theme = 'light' | 'dark';
type Themed = string | Record<Theme, string>;

interface Token<T> {
  name: string;
  value: T;
  usage?: string;
}

interface TypeStyle {
  name: string;
  fontSize: string;
  lineHeight: string;
  fontWeight: number;
  letterSpacing?: string;
}

export interface TokenSource {
  name: string;
  color: { tokens: Token<Themed>[] };
  type: {
    fonts: { family: string; file: string; weight: string; style: string }[];
    families: Record<string, string>;
    groups: { name: string; styles: TypeStyle[] }[];
  };
  spacing: { tokens: Token<string>[] };
  radius: { tokens: Token<string>[] };
  shadow: { tokens: Token<Themed>[] };
  boyut?: { tokens: Token<string>[] };
}

export const scales = ['marka', 'panel', 'mobil'] as const;
export type Scale = (typeof scales)[number];

const reference = /^\{([a-z0-9-]+)\}$/;

/** Renk değerini temaya göre çözer; "{gri-0}" gibi başvuruları izler. */
export function resolveColor(
  source: TokenSource,
  name: string,
  theme: Theme,
  seen: string[] = [],
): string {
  if (seen.includes(name)) {
    throw new Error(`Döngüsel başvuru: ${[...seen, name].join(' → ')}`);
  }

  const token = source.color.tokens.find((t) => t.name === name);
  if (!token) {
    throw new Error(`${source.name}: renk tokenı yok: ${name}`);
  }

  const raw = typeof token.value === 'string' ? token.value : token.value[theme];
  const match = reference.exec(raw);
  return match?.[1] ? resolveColor(source, match[1], theme, [...seen, name]) : raw;
}

function themed(value: Themed, theme: Theme): string {
  return typeof value === 'string' ? value : value[theme];
}

function isThemed(value: Themed): boolean {
  return typeof value !== 'string' && value.light !== value.dark;
}

function block(selector: string, lines: string[]): string {
  return `${selector} {\n${lines.map((l) => `  ${l}`).join('\n')}\n}\n`;
}

/** Ölçeğin CSS'i: :root (açık tema ve temasız tokenlar) + [data-theme='dark'] (koyu temada değişenler). */
export function generateCss(scale: Scale, source: TokenSource): string {
  const light: string[] = ['color-scheme: light;'];
  const dark: string[] = ['color-scheme: dark;'];

  for (const token of source.color.tokens) {
    light.push(`--${token.name}: ${resolveColor(source, token.name, 'light')};`);
    const darkValue = resolveColor(source, token.name, 'dark');
    if (darkValue !== resolveColor(source, token.name, 'light')) {
      dark.push(`--${token.name}: ${darkValue};`);
    }
  }

  for (const token of source.shadow.tokens) {
    light.push(`--${token.name}: ${themed(token.value, 'light')};`);
    if (isThemed(token.value)) {
      dark.push(`--${token.name}: ${themed(token.value, 'dark')};`);
    }
  }

  for (const token of [
    ...source.spacing.tokens,
    ...source.radius.tokens,
    ...(source.boyut?.tokens ?? []),
  ]) {
    light.push(`--${token.name}: ${token.value};`);
  }

  for (const [family, stack] of Object.entries(source.type.families)) {
    light.push(`--yazi-aile-${family}: ${stack};`);
  }

  for (const style of source.type.groups.flatMap((g) => g.styles)) {
    light.push(`--yazi-${style.name}-boyut: ${style.fontSize};`);
    light.push(`--yazi-${style.name}-satir: ${style.lineHeight};`);
    light.push(`--yazi-${style.name}-agirlik: ${String(style.fontWeight)};`);
    light.push(`--yazi-${style.name}-harf: ${style.letterSpacing ?? 'normal'};`);
  }

  return [
    `/* Üretildi (${scale}): pnpm --filter @omvila/design-tokens build. Elle düzenlemeyin; kaynak src/tokens.${scale}.json. */\n`,
    block(':root', light),
    `\n${block("[data-theme='dark']", dark)}`,
  ].join('');
}

/** Yazı tipi tanımları (bütün ölçeklerde aynı dosyalar). Yollar dist/'e göredir. */
export function generateFontsCss(source: TokenSource): string {
  const faces = source.type.fonts.map((font) =>
    block('@font-face', [
      `font-family: '${font.family}';`,
      `src: url('../${font.file}') format('woff2');`,
      `font-weight: ${font.weight};`,
      `font-style: ${font.style};`,
      'font-display: swap;',
    ]),
  );
  return `/* Üretildi: pnpm --filter @omvila/design-tokens build. Elle düzenlemeyin. */\n${faces.join('\n')}`;
}

/** Ölçek başına token adları (CSS değişkeni adları, "--" olmadan) ve tipleri. */
export function generateTypeScript(sources: Record<Scale, TokenSource>): string {
  const lines = [
    '// Üretildi: pnpm --filter @omvila/design-tokens build. Elle düzenlemeyin.',
    '// Token adları CSS değişkeni adlarıdır: var(--<ad>).',
    '',
  ];
  for (const scale of scales) {
    const s = sources[scale];
    const groups: Record<string, string[]> = {
      renk: s.color.tokens.map((t) => t.name),
      golge: s.shadow.tokens.map((t) => t.name),
      bosluk: s.spacing.tokens.map((t) => t.name),
      kose: s.radius.tokens.map((t) => t.name),
      boyut: s.boyut?.tokens.map((t) => t.name) ?? [],
      yazi: s.type.groups.flatMap((g) => g.styles.map((st) => st.name)),
    };
    lines.push(`export const ${scale}Tokens = {`);
    for (const [group, names] of Object.entries(groups)) {
      lines.push(`  ${group}: [${names.map((n) => `'${n}'`).join(', ')}],`);
    }
    lines.push('} as const;', '');
    const type = scale.charAt(0).toUpperCase() + scale.slice(1);
    lines.push(`export type ${type}ColorToken = (typeof ${scale}Tokens.renk)[number];`, '');
  }

  return `${lines.join('\n').trimEnd()}\n`;
}
