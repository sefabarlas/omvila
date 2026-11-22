// Ortak stylelint ayarı. Kurallar: docs/standartlar/kodlama-typescript.md § Bileşenler ve stil
// Renk ve aralık yalnız semantik tasarım tokenlarıyla: var(--zemin), var(--bosluk-4).

// Primitif paletler (packages/design-tokens/src/tokens.marka.json). Bileşen kodunda kullanılmaz.
const rawPalettes = ['basari', 'bilgi', 'gri', 'hata', 'kum', 'mercan', 'petrol', 'uyari'];
const rawPaletteVar = `/var\\(\\s*--(${rawPalettes.join('|')})-\\d+/`;

// Sıfır dışındaki sabit uzunluk (12px, 1.5rem, -4px…)
const fixedLength = '/(^|[\\s(,])-?(\\d*\\.)?\\d*[1-9]\\d*(px|rem|em)\\b/';

/** @type {import('stylelint').Config} */
export default {
  extends: ['stylelint-config-standard'],
  ignoreFiles: [
    '**/node_modules/**',
    '**/dist/**',
    '**/.next/**',
    '**/out/**',
    '**/coverage/**',
    'design/**',
    'packages/design-tokens/**',
  ],
  rules: {
    // Renk: yalnız token
    'color-no-hex': true,
    'color-named': 'never',
    'function-disallowed-list': [
      'rgb',
      'rgba',
      'hsl',
      'hsla',
      'hwb',
      'lab',
      'lch',
      'oklab',
      'oklch',
      'color',
    ],
    // Ham palet ve sabit aralık yasak
    'declaration-property-value-disallowed-list': [
      {
        '/.*/': [rawPaletteVar],
        '/^(margin|padding|gap|row-gap|column-gap|inset)(-.*)?$/': [fixedLength],
      },
      {
        message: (property, value) =>
          `"${property}: ${value}" yasak. Renk ve aralık için semantik token kullanın (var(--…)).`,
      },
    ],

    // CSS Modules: sınıf adları camelCase, :global serbest
    'selector-class-pattern': [
      '^[a-z][a-zA-Z0-9]*$',
      { message: (selector) => `CSS Modules sınıf adı camelCase olmalı: ${selector}` },
    ],
    'selector-pseudo-class-no-unknown': [true, { ignorePseudoClasses: ['global', 'local'] }],
    'property-no-unknown': [true, { ignoreProperties: ['composes'] }],
  },
};
