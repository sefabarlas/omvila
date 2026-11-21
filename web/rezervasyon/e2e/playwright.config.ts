import { defineConfig, devices } from '@playwright/test';

// Uçtan uca testler (docs/standartlar/test-stratejisi.md). Salon kartı sunucuda Prism mock'tan okunur; tarayıcıdaki
// akış uçları (katalog, gün, saat, kod, oluşturma) test verisiyle yanıtlanır (page.route, src/test/booking-fixtures.ts).
// Yerelde kurulu Chrome kullanılır (tarayıcı indirmeden); CI'da Playwright Chromium kurulur.
const ci = Boolean(process.env.CI);
const port = 3100;

export default defineConfig({
  testDir: '.',
  testMatch: '**/*.e2e.ts',
  outputDir: '../e2e-results',
  timeout: 60_000,
  retries: ci ? 1 : 0,
  workers: 1,
  reporter: ci ? [['list'], ['html', { open: 'never', outputFolder: '../e2e-report' }]] : 'list',
  use: {
    ...devices['Pixel 7'],
    ...(ci ? {} : { channel: 'chrome' }),
    baseURL: `http://localhost:${String(port)}`,
    viewport: { width: 390, height: 844 },
    locale: 'tr-TR',
    timezoneId: 'Europe/Istanbul',
    trace: 'retain-on-failure',
  },
  webServer: [
    {
      command: 'pnpm --filter @omvila/contracts mock',
      port: 4010,
      reuseExistingServer: true,
      timeout: 60_000,
    },
    {
      command: `pnpm exec next dev --port ${String(port)}`,
      cwd: '..',
      port,
      reuseExistingServer: !ci,
      env: { NEXT_PUBLIC_API_URL: 'http://localhost:4010' },
      timeout: 120_000,
    },
  ],
});
