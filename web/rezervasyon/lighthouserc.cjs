// Lighthouse CI: salon sayfasının performans bütçesi (web/rezervasyon/AGENTS.md § Performans).
// Ön koşul: derlenmiş uygulama (`pnpm build`) ve Prism mock (`pnpm --filter @omvila/contracts mock`).
// Varsayılan ayar mobil cihaz + yavaş 4G benzetimidir (Lighthouse "simulate" kısıtlaması).
/** @type {import('@lhci/cli').LighthouseRcFile} */
module.exports = {
  ci: {
    collect: {
      startServerCommand: 'pnpm start',
      startServerReadyPattern: 'Ready in',
      // Salon sayfası ve SMS bağlantısı (/r/{kod}; Prism örnek randevuyu döner, kod sözleşme desenine uyar).
      url: ['http://localhost:3000/kuafor-elif', 'http://localhost:3000/r/7Kx2Pq9a'],
      numberOfRuns: 3,
      settings: {
        // Yerelde ve CI'da kök olmayan kapsayıcıda Chrome'un açılabilmesi için.
        chromeFlags: '--no-sandbox --headless=new',
      },
    },
    assert: {
      assertions: {
        'categories:performance': ['error', { minScore: 0.9, aggregationMethod: 'median-run' }],
        'categories:accessibility': ['error', { minScore: 0.95 }],
        'largest-contentful-paint': [
          'error',
          { maxNumericValue: 2500, aggregationMethod: 'median-run' },
        ],
        'cumulative-layout-shift': [
          'error',
          { maxNumericValue: 0.1, aggregationMethod: 'median-run' },
        ],
        // İlk yüklemede aktarılan JS (gzip): < 120 KB. Şimdilik uyarı: Next 16 + React 19 çatısı tek başına
        // ~133 KB gzip (~114 KB brotli) yüklüyor; karar docs/acik-sorular.md T-54'te.
        'resource-summary:script:size': ['warn', { maxNumericValue: 122880 }],
      },
    },
    upload: {
      // Rapor herkese açık depolamaya gönderilmez; CI'da yapıt olarak saklanır.
      target: 'filesystem',
      outputDir: '.lighthouseci',
    },
  },
};
