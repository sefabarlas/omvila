// Müşteri aydınlatma metni (KVKK). Metnin adresi ve yürürlükteki sürümü yapılandırmadan gelir; sürüm randevu
// isteğinde `privacyNoticeVersion` olarak gönderilir (T-51 (10): sunucu henüz değeri doğrulamıyor; T-56).

/** Aydınlatma metninin adresi; tanımlı değilse onay kutusunda bağlantı gösterilmez. */
export const privacyNoticeUrl: string | undefined = process.env.NEXT_PUBLIC_PRIVACY_NOTICE_URL;

/** Yürürlükteki sürüm (en çok 20 karakter). */
export const privacyNoticeVersion: string =
  process.env.NEXT_PUBLIC_PRIVACY_NOTICE_VERSION ?? 'musteri-v1';
