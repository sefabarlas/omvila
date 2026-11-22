import { defineConfig, devices } from '@playwright/test';

// Uçtan uca testler Prism mock'a karşı (docs/standartlar/test-stratejisi.md). Prism'in örnek yanıtlarının akışa
// uymadığı yerlerde (yönetici rolü, tasarımdaki marka listesi) test kendi yanıtını verir (page.route).
// Yerelde kurulu Chrome kullanılır (tarayıcı indirmeden); CI'da Playwright Chromium kurulur.
const ci = Boolean(process.env.CI);
// Yerelde başka bir çalışma kopyasının Prism'i 4010'u tutuyorsa: E2E_API_PORT=4012 (Prism elle o portta açılır).
const apiPort = process.env.E2E_API_PORT ?? '4010';

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
    baseURL: 'http://localhost:5181',
    viewport: { width: 1440, height: 900 },
    locale: 'tr-TR',
    timezoneId: 'Europe/Istanbul',
    trace: 'retain-on-failure',
  },
  webServer: [
    {
      command: 'pnpm --filter @omvila/contracts mock',
      port: Number(apiPort),
      reuseExistingServer: true,
      timeout: 60_000,
    },
    {
      command: 'pnpm exec vite --port 5181 --strictPort',
      cwd: '..',
      port: 5181,
      reuseExistingServer: !ci,
      env: { VITE_API_URL: `http://localhost:${apiPort}` },
      timeout: 60_000,
    },
  ],
});
