// Derin bağlantı doğrulama dosyaları (K3): uygulamalar randevu alanındaki /r/, /b/, /d/ ve /{brandSlug} bağlantılarını
// açar. Uygulama kimliği com.omvila.app; Apple Team ID ve Android imza parmak izleri henüz yok (K4), ortam
// değişkeninden okunur. Değer yoksa dosya sunulmaz (404): yanlış değerli dosya doğrulamayı önbellekte bozar (T-100).

export const appId = 'com.omvila.app';

type Env = Record<string, string | undefined>;

/** Apple Team ID: 10 karakter, büyük harf ve rakam. */
const teamIdPattern = /^[A-Z0-9]{10}$/;
/** SHA-256 imza parmak izi: "AB:CD:…" (32 bayt). */
const fingerprintPattern = /^([0-9A-F]{2}:){31}[0-9A-F]{2}$/;

/**
 * apple-app-site-association (Universal Links). Sıra önemlidir, ilk eşleşen kural geçer: SMS bağlantıları, sonra
 * kökü ve iki ya da daha fazla bölümlü adresleri dışarıda bırakıp tek bölümlü salon adresi.
 */
export function appleAppSiteAssociation(env: Env) {
  const teamId = env.APPLE_TEAM_ID?.trim();
  if (!teamId || !teamIdPattern.test(teamId)) return null;
  return {
    applinks: {
      details: [
        {
          appIDs: [`${teamId}.${appId}`],
          components: [
            { '/': '/r/*' },
            { '/': '/b/*' },
            { '/': '/d/*' },
            { '/': '/', exclude: true },
            { '/': '/*/*', exclude: true },
            { '/': '/*' },
          ],
        },
      ],
    },
  };
}

/** assetlinks.json (Android App Links). Yollar uygulamanın intent filtresindedir; dosya yalnız imzayı doğrular. */
export function assetLinks(env: Env) {
  const fingerprints = (env.ANDROID_CERT_SHA256 ?? '')
    .split(',')
    .map((value) => value.trim().toUpperCase())
    .filter((value) => fingerprintPattern.test(value));
  if (fingerprints.length === 0) return null;
  return [
    {
      relation: ['delegate_permission/common.handle_all_urls'],
      target: {
        namespace: 'android_app',
        package_name: appId,
        sha256_cert_fingerprints: fingerprints,
      },
    },
  ];
}

/** JSON yanıtı (yönlendirmesiz, application/json); değer yoksa 404. */
export function wellKnownResponse(body: unknown): Response {
  if (body === null) return new Response(null, { status: 404 });
  return Response.json(body, {
    headers: { 'Cache-Control': 'public, max-age=3600' },
  });
}
