// Bileşen önizlemeleri (packages/ui-web/AGENTS.md): pnpm --filter @omvila/ui-web dev
/** @type {import('@ladle/react').UserConfig} */
export default {
  stories: 'src/**/*.stories.tsx',
  defaultStory: 'panel--button--varyantlar',
  addons: { theme: { enabled: true, defaultState: 'light' }, width: { enabled: true } },
};
