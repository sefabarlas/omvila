// Lighthouse CI: tanıtım sitesinin performans, erişilebilirlik ve SEO bütçesi (rezervasyon sayfasıyla aynı eşikler).
// Ön koşul: derlenmiş statik çıktı (`pnpm build` → out/). Sunucu scripts/serve.mjs (API yok).
// Varsayılan ayar mobil cihaz + yavaş 4G benzetimidir (Lighthouse "simulate" kısıtlaması).
/** @type {import('@lhci/cli').LighthouseRcFile} */
module.exports = {
  ci: {
    collect: {
      startServerCommand: 'pnpm start',
      startServerReadyPattern: 'Ready on',
      url: ['http://localhost:3001/', 'http://localhost:3001/fiyatlar/'],
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
        'categories:seo': ['error', { minScore: 0.9 }],
        'largest-contentful-paint': [
          'error',
          { maxNumericValue: 2500, aggregationMethod: 'median-run' },
        ],
        'cumulative-layout-shift': [
          'error',
          { maxNumericValue: 0.1, aggregationMethod: 'median-run' },
        ],
      },
    },
    upload: {
      // Rapor herkese açık depolamaya gönderilmez; CI'da yapıt olarak saklanır.
      target: 'filesystem',
      outputDir: '.lighthouseci',
    },
  },
};
