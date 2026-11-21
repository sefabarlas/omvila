import { assetLinks, wellKnownResponse } from '@/lib/app-links';

// Ortam değişkeni (ANDROID_CERT_SHA256) kapsayıcı çalışırken okunur; derlemede sabitlenmez.
export const dynamic = 'force-dynamic';

/** Android App Links doğrulama dosyası. ANDROID_CERT_SHA256 yoksa 404. */
export function GET(): Response {
  return wellKnownResponse(assetLinks(process.env));
}
