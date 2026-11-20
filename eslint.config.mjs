// Ortak ESLint ayarı (flat config). Kurallar: docs/standartlar/kodlama-typescript.md
// Projeler bu dosyayı kullanır; proje özel ayar gerekiyorsa aşağıya `files` ile eklenir.
import { fixupConfigRules } from '@eslint/compat';
import js from '@eslint/js';
import prettier from 'eslint-config-prettier';
import jsxA11y from 'eslint-plugin-jsx-a11y';
import reactHooks from 'eslint-plugin-react-hooks';
import globals from 'globals';
import tseslint from 'typescript-eslint';

// "Şimdi" yalnız enjekte edilen saatten; cihaz saat dilimine bağlı hesap yok (AGENTS.md § 5).
const localTimeMethods = [
  'getDate',
  'getDay',
  'getFullYear',
  'getHours',
  'getMinutes',
  'getMonth',
  'getSeconds',
  'getTimezoneOffset',
  'setDate',
  'setFullYear',
  'setHours',
  'setMinutes',
  'setMonth',
  'setSeconds',
  'toLocaleDateString',
  'toLocaleString',
  'toLocaleTimeString',
];

const timeRestrictions = [
  {
    selector: "NewExpression[callee.name='Date'][arguments.length=0]",
    message: 'Şimdiki zaman yalnız enjekte edilen saatten alınır (@omvila/ui-web/time).',
  },
  {
    selector: "CallExpression[callee.object.name='Date'][callee.property.name='now']",
    message: 'Şimdiki zaman yalnız enjekte edilen saatten alınır (@omvila/ui-web/time).',
  },
  {
    selector: `CallExpression[callee.property.name=/^(${localTimeMethods.join('|')})$/]`,
    message:
      'Cihaz saat dilimine bağlı tarih yöntemi yasak. Dönüşüm @omvila/ui-web/time ile ve şubenin timeZone değeriyle yapılır.',
  },
];

const jsxRestrictions = [
  {
    selector: "JSXAttribute[name.name='dangerouslySetInnerHTML']",
    message: 'dangerouslySetInnerHTML yasak (salonun girdiği metinler dahil).',
  },
];

export default tseslint.config(
  {
    ignores: [
      '**/node_modules/**',
      '**/dist/**',
      '**/.next/**',
      '**/out/**',
      '**/coverage/**',
      '**/generated/**',
      '**/.turbo/**',
      'api/**',
      'contracts/**',
      'design/**',
      'docs/**',
      'mobile/**',
      'packages/api-client-dart/**',
      'packages/omvila_ui/**',
      'scripts/**',
    ],
  },
  {
    linterOptions: {
      reportUnusedDisableDirectives: 'error',
    },
  },

  js.configs.recommended,
  ...tseslint.configs.strictTypeChecked,
  ...tseslint.configs.stylisticTypeChecked,
  {
    languageOptions: {
      ecmaVersion: 'latest',
      sourceType: 'module',
      globals: { ...globals.browser },
      parserOptions: {
        projectService: true,
        tsconfigRootDir: import.meta.dirname,
      },
    },
    rules: {
      '@typescript-eslint/no-explicit-any': 'error',
      '@typescript-eslint/consistent-type-imports': 'error',
      'no-restricted-syntax': ['error', ...timeRestrictions, ...jsxRestrictions],
      // API yalnız üretilmiş istemciyle çağrılır (AGENTS.md § 6).
      'no-restricted-globals': [
        'error',
        { name: 'fetch', message: 'API yalnız @omvila/api-client-ts ile çağrılır.' },
      ],
    },
  },

  // React (JSX/TSX)
  {
    files: ['**/*.tsx', '**/*.jsx'],
    ...reactHooks.configs.flat.recommended,
  },
  ...fixupConfigRules(
    /** @type {import('eslint').Linter.Config[]} */ ([
      { files: ['**/*.tsx', '**/*.jsx'], ...jsxA11y.flatConfigs.strict },
    ]),
  ),

  // Zaman dönüşümünün tek yeri ve fetch'in tek kullanıcısı
  {
    files: ['packages/ui-web/src/time/**'],
    rules: { 'no-restricted-syntax': ['error', ...jsxRestrictions] },
  },
  {
    files: ['packages/api-client-ts/src/**'],
    rules: { 'no-restricted-globals': 'off' },
  },

  // Testler: Node globalleri açık (zaman kuralları testte de geçerli)
  {
    files: ['**/*.test.{ts,tsx}', '**/*.spec.{ts,tsx}', '**/e2e/**'],
    languageOptions: { globals: { ...globals.node } },
  },

  // Yapılandırma dosyaları (tip bilgisi olmadan)
  {
    files: ['**/*.{js,mjs,cjs}', '**/*.config.{ts,mts}'],
    ...tseslint.configs.disableTypeChecked,
    languageOptions: {
      globals: { ...globals.node },
      parserOptions: { projectService: false },
    },
  },

  prettier,
);
