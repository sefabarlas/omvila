// Hata kodu → kullanıcı mesajı (docs/sozlesme/api-kurallari.md § Hata kodu
// kataloğu). Metinler packages/ui-web/src/errors/tr.ts ile birebir aynıdır
// (test eşleşmeyi denetler). Uygulamalar sunucunun title/detail metnini
// göstermez.
import 'package:omvila_api/omvila_api.dart';

/// Sözleşmedeki her kod için mesaj ("siz" hitabı, ünlem ve "lütfen" yok).
const Map<ErrorCode, String> errorMessages = {
  ErrorCode.validationFailed:
      'Bazı alanlar eksik veya hatalı. İşaretli alanları düzeltin.',
  ErrorCode.unauthorized: 'Oturumunuz kapandı. Yeniden giriş yapın.',
  ErrorCode.tokenExpired: 'Oturumunuzun süresi doldu. Yeniden giriş yapın.',
  ErrorCode.forbidden: 'Bu sayfayı görme yetkiniz yok.',
  ErrorCode.bookingDisabled: 'Online randevu kapalı. Salonu arayın.',
  ErrorCode.subscriptionReadOnly:
      'Aboneliğiniz salt okunur. Değişiklik yapmak için ödemeyi tamamlayın.',
  ErrorCode.customerBlocked: 'Randevu için salonu arayın.',
  ErrorCode.notFound: 'Aradığınız kayıt bulunamadı.',
  ErrorCode.accessCodeInvalid: 'Bu randevu bulunamadı.',
  ErrorCode.slotTaken: 'Bu saat az önce doldu. Başka bir saat seçin.',
  ErrorCode.invalidStateTransition:
      'Kayıt bu arada değişti. Sayfayı yenileyip tekrar deneyin.',
  ErrorCode.cutoffPassed: 'Değişiklik süresi geçti. Salonu arayın.',
  ErrorCode.holdExpired: 'Bu saatin süresi doldu.',
  ErrorCode.alreadyReviewed: 'Bu randevuyu zaten değerlendirdiniz.',
  ErrorCode.phoneAlreadyExists: 'Bu numara zaten kayıtlı.',
  ErrorCode.slugUnavailable: 'Bu bağlantı adı kullanılamıyor.',
  ErrorCode.depositUnavailableNoIban: 'Kapora alınamaz. Önce IBAN ekleyin.',
  ErrorCode.depositRequired:
      'Kapora henüz gelmedi. Kapora geldiyse işaretleyin ya da kaporasız '
      'onaylayın.',
  ErrorCode.depositReminderAlreadySent:
      'Bu randevu için hatırlatma zaten gönderildi.',
  ErrorCode.paymentTotalMismatch:
      'Ödemelerin toplamı alınacak tutarla aynı olmalı. Tutarları kontrol '
      'edin.',
  ErrorCode.packageSessionUnavailable:
      'Bu paketten seans düşülemiyor. Paketin süresini ve kalan seansını '
      'kontrol edin.',
  ErrorCode.cashDayAlreadyClosed: 'Bu gün zaten kapatıldı. Sayfayı yenileyin.',
  ErrorCode.outsideWorkingHours: 'Seçilen saat çalışma saatleri dışında.',
  ErrorCode.staffOnTimeOff: 'Personel bu saatte izinli.',
  ErrorCode.insufficientCredits: 'SMS kontörünüz az.',
  ErrorCode.concurrencyConflict: 'Kayıt çakışması. Sayfayı yenileyin.',
  ErrorCode.idempotencyKeyReused: 'İstek tamamlanamadı. Tekrar deneyin.',
  ErrorCode.otpInvalid: 'Kod hatalı.',
  ErrorCode.otpExpired: 'Kodun süresi doldu. Yeni kod isteyin.',
  ErrorCode.preconditionRequired:
      'İstek tamamlanamadı. Sayfayı yenileyip tekrar deneyin.',
  ErrorCode.otpAttemptsExceeded:
      'Çok fazla deneme yaptınız. Biraz bekleyip tekrar deneyin.',
  ErrorCode.otpResendTooSoon: 'Yeni kod için biraz bekleyin.',
  ErrorCode.rateLimited:
      'Çok fazla istek gönderildi. Biraz bekleyip tekrar deneyin.',
  ErrorCode.tryAgain: 'Bir sorun oluştu. Tekrar deneyin.',
  ErrorCode.paymentNoticeAlreadyPending:
      'Bu ödeme için bildiriminiz zaten inceleniyor.',
  ErrorCode.bankTransactionAlreadyMatched:
      'Bu banka hareketi başka bir bildirimle eşleştirildi. Sayfayı yenileyin.',
  ErrorCode.bankTransactionDuplicate:
      'Bu banka referansıyla kayıtlı bir hareket var.',
  ErrorCode.lastOwnerRequired: 'İşletmede en az bir işletme sahibi kalmalı.',
  ErrorCode.planLimitExceeded:
      'Markanın personel veya şube sayısı bu planın sınırını aşıyor.',
};

/// Bilinmeyen kod (yeni sunucu, eski istemci) için genel mesaj.
const unknownErrorMessage = 'Bir sorun oluştu. Tekrar deneyin.';

/// Sunucunun `code` değerine (snake_case) göre mesaj.
String errorMessage(String? code) {
  for (final entry in errorMessages.entries) {
    if (entry.key.value == code) {
      return entry.value;
    }
  }

  return unknownErrorMessage;
}
