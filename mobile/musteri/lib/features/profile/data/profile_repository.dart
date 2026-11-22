import 'package:flutter/foundation.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/shared/api_call.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Hesap silmeden önce gösterilenler (HesapSil).
@immutable
class DeletionPreview {
  /// Önizleme.
  const DeletionPreview({required this.upcoming, required this.usablePackages});

  /// İptal edilecek yaklaşan randevular.
  final List<MeAppointmentSummary> upcoming;

  /// Kullanılamaz hale gelecek paketler.
  final int usablePackages;
}

/// Profil, izinler ve hesap silme (meGetProfile, meUpdateProfile,
/// meListConsents, meUpdateMarketingConsent, meRequestAccountDeletionOtp,
/// meRequestAccountDeletion).
abstract interface class ProfileRepository {
  /// Profil.
  Future<MeProfile> profile();

  /// "Kaydet" (If-Match); [email] null ise e-posta silinir (gövdede
  /// `"email": null`).
  Future<MeProfile> update(
    MeProfile current, {
    required String fullName,
    required String? email,
  });

  /// Salon bazında kampanya izinleri.
  Future<List<MeConsent>> consents();

  /// Bir salonun kampanya izni anahtarı.
  Future<List<MeConsent>> setMarketing(String brandId, {required bool granted});

  /// Yaklaşan randevular ve kullanılabilir paketler.
  Future<DeletionPreview> deletionPreview();

  /// Hesap silme kodu (kayıtlı numaraya SMS).
  Future<OtpChallengeResponse> requestDeletionOtp();

  /// Kod doğruysa hesabı hemen siler (W7).
  Future<MeAccountDeletionResult> deleteAccount({
    required String otpId,
    required String code,
  });
}

/// Üretilmiş istemciyle.
final class ApiProfileRepository implements ProfileRepository {
  /// [api] müşteri jetonuyla.
  const ApiProfileRepository(this.api);

  /// API.
  final OmvilaApi api;

  @override
  Future<MeProfile> profile() => apiCall(() async {
    final response = await api.getMeProfileApi().meGetProfile();
    return response.data!;
  });

  @override
  Future<MeProfile> update(
    MeProfile current, {
    required String fullName,
    required String? email,
  }) => apiCall(() async {
    final response = await api.getMeProfileApi().meUpdateProfile(
      ifMatch: ifMatch(current.version),
      meProfileUpdate: MeProfileUpdate(fullName: fullName, email: email),
    );
    return response.data!;
  });

  @override
  Future<List<MeConsent>> consents() => apiCall(() async {
    final response = await api.getMeProfileApi().meListConsents();
    return response.data!.items;
  });

  @override
  Future<List<MeConsent>> setMarketing(
    String brandId, {
    required bool granted,
  }) => apiCall(() async {
    final response = await api.getMeProfileApi().meUpdateMarketingConsent(
      meMarketingConsentUpdate: MeMarketingConsentUpdate(
        brandId: brandId,
        granted: granted,
      ),
    );
    return response.data!.items;
  });

  @override
  Future<DeletionPreview> deletionPreview() => apiCall(() async {
    final (upcoming, packages) = await (
      api.getMeAppointmentsApi().meListAppointments(scope: 'Upcoming'),
      api.getMeWalletApi().meListPackages(),
    ).wait;
    return DeletionPreview(
      upcoming: upcoming.data!.items,
      usablePackages: packages.data!.items.where((p) => p.isUsable).length,
    );
  });

  @override
  Future<OtpChallengeResponse> requestDeletionOtp() => apiCall(() async {
    final response = await api.getMeProfileApi().meRequestAccountDeletionOtp();
    return response.data!;
  });

  @override
  Future<MeAccountDeletionResult> deleteAccount({
    required String otpId,
    required String code,
  }) => apiCall(() async {
    final response = await api.getMeProfileApi().meRequestAccountDeletion(
      meAccountDeletionConfirm: MeAccountDeletionConfirm(
        otpId: otpId,
        code: code,
      ),
    );
    return response.data!;
  });
}
