// pnpm --filter @omvila/design-tokens build: dist/ dosyalarını src/*.json kaynağından yeniden üretir.
import { mkdirSync, readFileSync, writeFileSync } from 'node:fs';
import { dirname, join } from 'node:path';
import { fileURLToPath } from 'node:url';
import { generateCss, generateFontsCss, generateTypeScript, scales } from './generate.ts';
import type { Scale, TokenSource } from './generate.ts';
import { generateDart } from './generate-dart.ts';

const root = join(dirname(fileURLToPath(import.meta.url)), '..');

export function loadSources(): Record<Scale, TokenSource> {
  return Object.fromEntries(
    scales.map((scale) => [
      scale,
      JSON.parse(readFileSync(join(root, 'src', `tokens.${scale}.json`), 'utf8')) as TokenSource,
    ]),
  ) as Record<Scale, TokenSource>;
}

/** Flutter teması (paket köküne göre yol); omvila_ui elle düzenlemez, buradan alır. */
export const dartThemePath = '../omvila_ui/lib/theme/omvila_theme.dart';

/** Üretilecek dosyalar (paket köküne göre yol → içerik); testler çıktıların güncel olduğunu bununla denetler. */
export function outputs(): Record<string, string> {
  const sources = loadSources();
  const files: Record<string, string> = {
    'dist/tokens.ts': generateTypeScript(sources),
    'dist/fonts.css': generateFontsCss(sources.panel),
    [dartThemePath]: generateDart(sources.mobil),
  };
  for (const scale of scales) {
    files[`dist/${scale}.css`] = generateCss(scale, sources[scale]);
  }

  return files;
}

if (process.argv[1] === fileURLToPath(import.meta.url)) {
  for (const [path, content] of Object.entries(outputs())) {
    mkdirSync(dirname(join(root, path)), { recursive: true });
    writeFileSync(join(root, path), content);
  }
}
