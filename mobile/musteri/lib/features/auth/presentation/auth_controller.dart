import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:omvila_musteri/app/providers.dart';

/// Oturum durumu.
enum AuthStatus {
  /// Cihazdaki jetonlar okunuyor (Açılış).
  loading,

  /// Oturum yok (Giriş).
  signedOut,

  /// Oturum açık.
  signedIn,
}

/// Oturum durumu.
final authControllerProvider = NotifierProvider<AuthController, AuthStatus>(
  AuthController.new,
);

/// Açılışta cihazdaki oturumu okur; giriş ve oturum kapanmasını izler.
class AuthController extends Notifier<AuthStatus> {
  @override
  AuthStatus build() {
    final session = ref.watch(customerSessionProvider)..onSignedOut = signedOut;
    Future.microtask(() async {
      final signedIn = await session.load();
      state = signedIn ? AuthStatus.signedIn : AuthStatus.signedOut;
    });
    return AuthStatus.loading;
  }

  /// Kod doğrulandı.
  void signedIn() => state = AuthStatus.signedIn;

  /// Oturum kapandı (yenileme reddedildi veya çıkış).
  void signedOut() => state = AuthStatus.signedOut;

  /// "Çıkış yap" (meLogout): bu telefonun push jetonu da pasifleşir.
  Future<void> logout() async {
    String? pushToken;
    try {
      pushToken = await ref.read(pushServiceProvider).token();
    } on Exception {
      pushToken = null;
    }

    await ref.read(authRepositoryProvider).logout(pushToken: pushToken);
    state = AuthStatus.signedOut;
  }
}
