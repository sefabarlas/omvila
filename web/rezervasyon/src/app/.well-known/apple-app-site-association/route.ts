import { appleAppSiteAssociation, wellKnownResponse } from '@/lib/app-links';

// Ortam değişkeni (APPLE_TEAM_ID) kapsayıcı çalışırken okunur; derlemede sabitlenmez.
export const dynamic = 'force-dynamic';

/** Universal Links doğrulama dosyası (uzantısız, application/json). APPLE_TEAM_ID yoksa 404. */
export function GET(): Response {
  return wellKnownResponse(appleAppSiteAssociation(process.env));
}
