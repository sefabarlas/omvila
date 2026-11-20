import { ApiProblem } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';

export interface SaveErrorView {
  /** 412: kayıt başkası tarafından değişti; "Kayıt çakışması" şeridi gösterilir. */
  conflict: boolean;
  /** Sözleşmedeki hata kodu (slug_unavailable…). */
  code: string | undefined;
  /** Sunucunun alan hataları (400 validation_failed): alan adı → hata kodları. */
  fields: Record<string, string[]>;
  /** Toast metni. */
  message: string;
}

/** Kaydetme hatasını ekranda gösterilecek biçime çevirir (api-kurallari.md § Hatalar). */
export function describeSaveError(error: unknown): SaveErrorView {
  if (!(error instanceof ApiProblem)) {
    return { conflict: false, code: undefined, fields: {}, message: errorMessage(undefined) };
  }

  return {
    conflict: error.status === 412,
    code: error.code,
    fields: error.code === 'validation_failed' ? error.errors : {},
    message: errorMessage(error.code),
  };
}
