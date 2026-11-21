import type { ErrorCode } from '@omvila/api-client-ts';

/**
 * Hata kodu → kullanıcı mesajı (docs/sozlesme/api-kurallari.md § Hata kodu kataloğu). Sözleşmedeki her kod için
 * bir girdi (tip düzeyinde Record). Uygulamalar sunucunun title/detail metnini göstermez.
 * Metin kuralları: "siz" hitabı, ünlem ve "lütfen" yok, ne olduğunu ve ne yapılacağını söyler.
 */
export const errorMessages: Record<ErrorCode, string> = {
  validation_failed: 'Bazı alanlar eksik veya hatalı. İşaretli alanları düzeltin.',
  unauthorized: 'Oturumunuz kapandı. Yeniden giriş yapın.',
  token_expired: 'Oturumunuzun süresi doldu. Yeniden giriş yapın.',
  forbidden: 'Bu sayfayı görme yetkiniz yok.',
  booking_disabled: 'Online randevu kapalı. Salonu arayın.',
  subscription_read_only: 'Aboneliğiniz salt okunur. Değişiklik yapmak için ödemeyi tamamlayın.',
  customer_blocked: 'Randevu için salonu arayın.',
  not_found: 'Aradığınız kayıt bulunamadı.',
  access_code_invalid: 'Bu randevu bulunamadı.',
  slot_taken: 'Bu saat az önce doldu. Başka bir saat seçin.',
  invalid_state_transition: 'Kayıt bu arada değişti. Sayfayı yenileyip tekrar deneyin.',
  cutoff_passed: 'Değişiklik süresi geçti. Salonu arayın.',
  hold_expired: 'Bu saatin süresi doldu.',
  already_reviewed: 'Bu randevuyu zaten değerlendirdiniz.',
  phone_already_exists: 'Bu numara zaten kayıtlı.',
  slug_unavailable: 'Bu bağlantı adı kullanılamıyor.',
  deposit_unavailable_no_iban: 'Kapora alınamaz. Önce IBAN ekleyin.',
  deposit_required: 'Kapora henüz gelmedi. Kapora geldiyse işaretleyin ya da kaporasız onaylayın.',
  deposit_reminder_already_sent: 'Bu randevu için hatırlatma zaten gönderildi.',
  payment_total_mismatch:
    'Ödemelerin toplamı alınacak tutarla aynı olmalı. Tutarları kontrol edin.',
  package_session_unavailable:
    'Bu paketten seans düşülemiyor. Paketin süresini ve kalan seansını kontrol edin.',
  cash_day_already_closed: 'Bu gün zaten kapatıldı. Sayfayı yenileyin.',
  outside_working_hours: 'Seçilen saat çalışma saatleri dışında.',
  staff_on_time_off: 'Personel bu saatte izinli.',
  insufficient_credits: 'SMS kontörünüz az.',
  concurrency_conflict: 'Kayıt çakışması. Sayfayı yenileyin.',
  idempotency_key_reused: 'İstek tamamlanamadı. Tekrar deneyin.',
  otp_invalid: 'Kod hatalı.',
  otp_expired: 'Kodun süresi doldu. Yeni kod isteyin.',
  precondition_required: 'İstek tamamlanamadı. Sayfayı yenileyip tekrar deneyin.',
  otp_attempts_exceeded: 'Çok fazla deneme yaptınız. Biraz bekleyip tekrar deneyin.',
  otp_resend_too_soon: 'Yeni kod için biraz bekleyin.',
  rate_limited: 'Çok fazla istek gönderildi. Biraz bekleyip tekrar deneyin.',
  try_again: 'Bir sorun oluştu. Tekrar deneyin.',
};

/** Bilinmeyen kod (yeni sunucu, eski istemci) için genel mesaj. */
export const unknownErrorMessage = 'Bir sorun oluştu. Tekrar deneyin.';

export function errorMessage(code: string | undefined): string {
  return code !== undefined && code in errorMessages
    ? errorMessages[code as ErrorCode]
    : unknownErrorMessage;
}
