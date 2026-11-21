// /r/{kod} alt ekranları adres çubuğunda: `?islem=saat` (Ertele), `?islem=iptal` (İptal sayfası). Yenileme ve
// geri tuşu ekranı korur; sunucu bileşeni de okur (ilk çizim adresle aynı).

export type Action = 'view' | 'reschedule' | 'cancel';

export const actionParam = 'islem';

const values: Record<Exclude<Action, 'view'>, string> = { reschedule: 'saat', cancel: 'iptal' };

export function parseAction(value: string | null | undefined): Action {
  if (value === values.reschedule) return 'reschedule';
  if (value === values.cancel) return 'cancel';
  return 'view';
}

/** "?islem=saat" veya "" (Randevum). */
export function actionSearch(action: Action): string {
  return action === 'view' ? '' : `?${actionParam}=${values[action]}`;
}
