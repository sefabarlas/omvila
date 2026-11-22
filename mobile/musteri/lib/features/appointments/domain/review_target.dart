import 'package:flutter/foundation.dart';

/// Değerlendirilecek randevu (Degerlendir): uygulama içinden randevu
/// kimliğiyle (`meCreateReview`) veya değerlendirme bağlantısından
/// (`/d/{code}`, `publicCreateReview`) açılır.
@immutable
class ReviewTarget {
  /// Hedef; [appointmentId] veya [accessCode] dolu.
  const ReviewTarget({
    required this.brandName,
    required this.serviceNames,
    required this.startUtc,
    required this.timeZone,
    this.appointmentId,
    this.accessCode,
    this.staffName,
    this.displayName,
    this.existingRating,
    this.existingComment,
  });

  /// Randevu kimliği (uygulama içi).
  final String? appointmentId;

  /// Değerlendirme bağlantısının kodu.
  final String? accessCode;

  /// "Kuaför Elif".
  final String brandName;

  /// "Boya ve fön".
  final List<String> serviceNames;

  /// "Elif Kaya"; personelsiz randevuda null.
  final String? staffName;

  /// Randevu başlangıcı.
  final DateTime startUtc;

  /// Şubenin saat dilimi.
  final String timeZone;

  /// Yorumda görünen ad ("Merve Y.").
  final String? displayName;

  /// Gönderilmiş puan; değerlendirilmemişse null.
  final int? existingRating;

  /// Gönderilmiş yorum.
  final String? existingComment;

  /// Daha önce değerlendirilmiş.
  bool get alreadyReviewed => existingRating != null;
}
