import { ApiProblem } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';

/** Karar uçlarının (eşleştir, bulunamadı, reddet, SMS adı) hata metni; 412 ve 409'lar ayrı yazılır. */
export function decisionError(problem: unknown): { title: string; text: string; stale: boolean } {
  if (problem instanceof ApiProblem && problem.status === 412) {
    return {
      title: 'Kayıt çakışması.',
      text: 'Kayıt başka biri tarafından değiştirildi. Güncel bilgilerle yeniden deneyin.',
      stale: true,
    };
  }

  if (problem instanceof ApiProblem && problem.code === 'invalid_state_transition') {
    return {
      title: 'Bu kayıt zaten sonuçlandı.',
      text: 'Başka bir ekip üyesi karar verdi. Güncel durumu görmek için yenileyin.',
      stale: true,
    };
  }

  if (problem instanceof ApiProblem && problem.code === 'bank_transaction_already_matched') {
    return {
      title: 'Hareket başka bildirime bağlı.',
      text: errorMessage(problem.code),
      stale: true,
    };
  }

  return {
    title: 'İşlem tamamlanamadı.',
    text: errorMessage(problem instanceof ApiProblem ? problem.code : undefined),
    stale: false,
  };
}
