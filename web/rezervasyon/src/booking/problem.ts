import { ApiProblem } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';

/** Sunucu hatasının kodu; ağ hatasında undefined. */
export function problemCode(error: unknown): string | undefined {
  return error instanceof ApiProblem ? error.code : undefined;
}

/** `meta` içindeki sayı (attemptsLeft, retryAfterSeconds). */
export function metaNumber(error: unknown, key: string): number | undefined {
  if (!(error instanceof ApiProblem)) return undefined;
  const value = error.meta[key];
  return typeof value === 'number' ? value : undefined;
}

/** `meta.alternatives`: aynı günün en yakın boş saatleri (UTC). */
export function alternatives(error: unknown): string[] {
  if (!(error instanceof ApiProblem)) return [];
  const value = error.meta.alternatives;
  return Array.isArray(value)
    ? value.filter((item): item is string => typeof item === 'string')
    : [];
}

/** Alan hataları (validation_failed): "phone" → ["invalid_format"]. */
export function fieldErrors(error: unknown): Record<string, string[]> {
  return error instanceof ApiProblem ? error.errors : {};
}

/** Kullanıcı metni `code`'dan (title/detail gösterilmez); ağ hatasında genel metin. */
export function messageFor(error: unknown): string {
  return errorMessage(problemCode(error));
}

/** "5 dakika", "40 saniye". */
export function waitLabel(seconds: number): string {
  return seconds >= 60
    ? `${String(Math.ceil(seconds / 60))} dakika`
    : `${String(Math.max(1, seconds))} saniye`;
}
