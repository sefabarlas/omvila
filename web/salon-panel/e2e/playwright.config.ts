import { defineConfig, devices } from '@playwright/test';

// Uçtan uca testler Prism mock'a karşı (docs/standartlar/test-stratejisi.md). Prism'in örnek yanıtlarının akışa
// uymadığı yerlerde (yenileme çerezi yok, kayıtsız numara, personel oturumu) test kendi yanıtını verir (page.route).
// Yerelde kurulu Chrome kullanılır (tarayıcı indirmeden); CI'da Playwright Chromium kurulur.
const ci = Boolean(process.env.CI);

export default defineConfig({
  testDir: '.',
  testMatch: '**/*.e2e.ts',
  outputDir: '../e2e-results',
  timeout: 30_000,
  retries: ci ? 1 : 0,
  workers: 1,
  reporter: ci ? [['list'], ['html', { open: 'never', outputFolder: '../e2e-report' }]] : 'list',
  use: {
    ...devices['Desktop Chrome'],
    ...(ci ? {} : { channel: 'chrome' }),
    baseURL: 'http://localhost:5180',
    viewport: { width: 1440, height: 900 },
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
      command: 'pnpm exec vite --port 5180 --strictPort',
      cwd: '..',
      port: 5180,
      reuseExistingServer: !ci,
      env: { VITE_API_URL: 'http://localhost:4010' },
      timeout: 60_000,
    },
  ],
});
