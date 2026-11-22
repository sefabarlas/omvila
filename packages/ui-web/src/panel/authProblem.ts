import { ApiProblem } from '@omvila/api-client-ts';
import { errorMessage } from '../errors';

/** Kullanıcıya gösterilecek metin ve sayaç (api-kurallari.md: meta.attemptsLeft, meta.retryAfterSeconds). */
export function describeAuthProblem(error: unknown): {
  message: string;
  retryAfterSeconds?: number;
} {
  if (!(error instanceof ApiProblem)) {
    return { message: errorMessage(undefined) };
  }

  const retry =
    typeof error.meta.retryAfterSeconds === 'number' ? error.meta.retryAfterSeconds : undefined;
  if (error.code === 'otp_invalid' && typeof error.meta.attemptsLeft === 'number') {
    return {
      message: `${errorMessage(error.code)} ${String(error.meta.attemptsLeft)} deneme hakkınız kaldı.`,
    };
  }

  if (error.code === 'validation_failed' && error.errors.phone) {
    return { message: 'Telefon numarası 10 haneli olmalı.' };
  }

  return {
    message: errorMessage(error.code),
    ...(retry !== undefined ? { retryAfterSeconds: retry } : {}),
  };
}

/** "4:32" */
export function formatCountdown(seconds: number): string {
  const m = Math.floor(seconds / 60);
  const s = seconds % 60;
  return `${String(m)}:${String(s).padStart(2, '0')}`;
}
