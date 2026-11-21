import { readdirSync, readFileSync } from 'node:fs';
import { dirname, join } from 'node:path';
import { fileURLToPath } from 'node:url';
import { describe, expect, it } from 'vitest';

// Kural: rezervasyon sayfası yalnız mobil ölçeği yükler (web/rezervasyon/AGENTS.md); bileşenlerin kullandığı her
// token @omvila/design-tokens/mobil.css içinde tanımlı olmalı (panel tokenına yanlışlıkla bağlanmayı yakalar).
const here = dirname(fileURLToPath(import.meta.url));
const mobilCss = readFileSync(join(here, '../../../design-tokens/dist/mobil.css'), 'utf8');
const defined = new Set([...mobilCss.matchAll(/--([a-z0-9-]+)\s*:/g)].map((m) => m[1]));

describe('booking CSS tokenları', () => {
  const files = readdirSync(here).filter((f) => f.endsWith('.module.css'));

  it.each(files)('%s yalnız mobil ölçekteki tokenları kullanır', (file) => {
    const css = readFileSync(join(here, file), 'utf8');
    const used = [...css.matchAll(/var\(\s*--([a-z0-9-]+)/g)].map((m) => m[1] ?? '');
    expect(used.filter((name) => !defined.has(name))).toEqual([]);
  });

  it('kapak renkleri mobil ölçekte tanımlı', () => {
    for (const name of ['kapak-petrol', 'kapak-mercan', 'kapak-kum']) {
      expect(defined.has(name)).toBe(true);
    }
  });
});
