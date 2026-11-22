import { defineConfig, devices } from '@playwright/test';

// Tanıtım sitesi ekran görüntüleri ve gezinme (test-stratejisi.md). API yok; site statiktir.
// Yerelde kurulu Chrome kullanılır (tarayıcı indirmeden); CI'da Playwright Chromium kurulur.
const ci = Boolean(process.env.CI);
const port = 3101;

export default defineConfig({
  testDir: '.',
  testMatch: '**/*.e2e.ts',
  outputDir: '../e2e-results',
  timeout: 60_000,
  retries: ci ? 1 : 0,
  workers: 1,
  reporter: ci ? [['list'], ['html', { open: 'never', outputFolder: '../e2e-report' }]] : 'list',
  use: {
    ...devices['Desktop Chrome'],
    ...(ci ? {} : { channel: 'chrome' }),
    baseURL: `http://localhost:${String(port)}`,
    viewport: { width: 1440, height: 900 },
    locale: 'tr-TR',
    timezoneId: 'Europe/Istanbul',
    trace: 'retain-on-failure',
  },
  webServer: {
    command: `pnpm exec next dev --port ${String(port)}`,
    cwd: '..',
    port,
    reuseExistingServer: !ci,
    env: { NEXT_PUBLIC_PANEL_URL: 'https://salon.example.test' },
    timeout: 120_000,
  },
});
