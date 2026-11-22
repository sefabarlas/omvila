import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_isletme/app/providers.dart';
import 'package:omvila_isletme/features/auth/data/auth_tokens.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Numara kayıtlı ama kullanıcı yok (`SignupRequired`): işletme uygulamada
/// oluşturulmaz, işletme sahibi personel olarak ekler.
class NoAccountException implements Exception {
  /// Hesap yok.
  const NoAccountException();
}

/// Oturum: null = giriş yapılmamış. Uygulama açılışında yenileme jetonu
/// varsa sessizce yenilenir ve `manageGetSession` okunur.
final authControllerProvider =
    AsyncNotifierProvider<AuthController, ManageSession?>(AuthController.new);

/// Giriş, çıkış ve oturum.
class AuthController extends AsyncNotifier<ManageSession?> {
  OmvilaApi get _api => ref.read(apiProvider);

  @override
  Future<ManageSession?> build() async {
    final tokens = ref.watch(authTokensProvider)
      ..onSignedOut = () => state = const AsyncData(null);
    return switch (await tokens.refreshOutcome()) {
      RefreshOutcome.signedOut => null,
      RefreshOutcome.failed => throw const ApiProblem(
        status: 0,
        code: ApiProblem.networkErrorCode,
      ),
      RefreshOutcome.refreshed => _loadSession(),
    };
  }

  Future<ManageSession> _loadSession() async {
    try {
      final response = await _api.getManageSessionApi().manageGetSession();
      return response.data!;
    } on DioException catch (e) {
      throw ApiProblem.fromDioException(e);
    }
  }

  /// "Kod gönder".
  Future<OtpChallengeResponse> requestOtp(String phoneE164) async {
    try {
      final response = await _api.getManageAuthApi().manageRequestOtp(
        xClientType: clientType,
        manageOtpRequest: ManageOtpRequest(
          phone: phoneE164,
          purpose: ManageOtpRequestPurposeEnum.login,
        ),
        extra: {skipAuthExtra: true},
      );
      return response.data!;
    } on DioException catch (e) {
      throw ApiProblem.fromDioException(e);
    }
  }

  /// "Giriş yap". Hesap yoksa [NoAccountException].
  Future<void> verifyOtp({required String otpId, required String code}) async {
    final ManageAuthResult result;
    try {
      final response = await _api.getManageAuthApi().manageVerifyOtp(
        xClientType: clientType,
        otpVerifyRequest: OtpVerifyRequest(
          otpId: otpId,
          code: code,
          rememberDevice: true,
        ),
        extra: {skipAuthExtra: true},
      );
      result = response.data!;
    } on DioException catch (e) {
      throw ApiProblem.fromDioException(e);
    }

    final accessToken = result.accessToken;
    if (result.result != ManageAuthResultResultEnum.signedIn ||
        accessToken == null) {
      throw const NoAccountException();
    }

    await ref
        .read(authTokensProvider)
        .save(accessToken: accessToken, refreshToken: result.refreshToken);
    final session = await _loadSession();
    state = AsyncData(session);
  }

  /// "Çıkış yap": sunucu oturumu kapanamasa da yerel jetonlar silinir.
  Future<void> logout() async {
    try {
      await _api.getManageAuthApi().manageLogout();
    } on DioException {
      // Çevrimdışıyken de çıkış yapılabilir; jeton süresiyle düşer.
    }

    await ref.read(authTokensProvider).clear();
    state = const AsyncData(null);
  }
}

/// Oturumdaki kullanıcının izinleri ve rolü.
extension SessionPermissions on ManageSession {
  /// İzin var mı (roller-yetkiler.md).
  bool can(String permission) => permissions.contains(permission);

  /// Personel rolü ("Takvimim", "Primim").
  bool get isStaff => membership.role == ManageSessionMembershipRoleEnum.staff;
}
