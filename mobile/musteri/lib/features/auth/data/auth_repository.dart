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
}
