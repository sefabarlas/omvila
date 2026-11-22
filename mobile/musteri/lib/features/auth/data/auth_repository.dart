import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/features/auth/data/customer_session.dart';
import 'package:omvila_musteri/shared/api_call.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Giriş ve SMS kodu (meRequestOtp, meVerifyOtp). Hatalar [ApiProblem].
abstract interface class AuthRepository {
  /// "Kod gönder": [phone] E.164.
  Future<OtpChallengeResponse> requestOtp(String phone);

  /// Kod doğruysa oturumu saklar.
  Future<void> verifyOtp({required String otpId, required String code});

  /// "Çıkış yap": yenileme jetonu iptal edilir, [pushToken] verilirse cihaz
  /// pasifleşir. Sunucuya ulaşılamasa da oturum cihazdan silinir.
  Future<void> logout({String? pushToken});

  /// Oturumu yalnız cihazdan siler (hesap silindi; sunucu oturumları
  /// kapattı).
  Future<void> forget();
}

/// İstemci başlığı (sözleşme ClientHeader).
const customerAppClient = 'customer-app';

/// Üretilmiş istemciyle.
final class ApiAuthRepository implements AuthRepository {
  /// [api] oturumsuz istekler için; [session] jetonları saklar.
  ApiAuthRepository({required this.api, required this.session});

  /// API.
  final OmvilaApi api;

  /// Oturum.
  final CustomerSession session;

  static final Map<String, bool> _noAuth = {skipAuthExtra: true};

  @override
  Future<OtpChallengeResponse> requestOtp(String phone) => apiCall(() async {
    final response = await api.getMeAuthApi().meRequestOtp(
      meOtpRequest: MeOtpRequest(phone: phone),
      xClient: customerAppClient,
      extra: _noAuth,
    );
    return response.data!;
  });

  @override
  Future<void> verifyOtp({required String otpId, required String code}) =>
      apiCall(() async {
        final response = await api.getMeAuthApi().meVerifyOtp(
          otpVerifyRequest: OtpVerifyRequest(otpId: otpId, code: code),
          xClient: customerAppClient,
          extra: _noAuth,
        );
        final result = response.data!;
        await session.save(
          accessToken: result.accessToken,
          refreshToken: result.refreshToken,
        );
      });

  @override
  Future<void> logout({String? pushToken}) async {
    try {
      await apiCall(
        () => api.getMeAuthApi().meLogout(
          meLogoutRequest: MeLogoutRequest(
            refreshToken: session.refreshToken,
            pushToken: pushToken,
          ),
        ),
      );
    } on ApiProblem {
      // Jeton zaten geçersiz veya bağlantı yok: oturum yine de kapanır.
    } finally {
      await session.clear();
    }
  }

  @override
  Future<void> forget() => session.clear();
}
