/// Derleme zamanı ayarları (`--dart-define`). Sabit adres kodda yazılmaz.
final class AppConfig {
  /// Ayarlar.
  const AppConfig({
    required this.apiUrl,
    this.privacyNoticeUrl,
    this.linkHosts = defaultLinkHosts,
  });

  /// `--dart-define` değerlerinden.
  factory AppConfig.fromEnvironment() {
    const privacy = String.fromEnvironment('PRIVACY_NOTICE_URL');
    return const AppConfig(
      apiUrl: String.fromEnvironment(
        'API_URL',
        defaultValue: 'http://10.0.2.2:4010',
      ),
    ).copyWithPrivacy(privacy.isEmpty ? null : privacy);
  }

  /// Salon ve SMS bağlantılarının alan adları (K3: randevu.omvila.com.tr;
  /// SMS'te kısa `omvila.com`).
  static const defaultLinkHosts = {
    'randevu.omvila.com.tr',
    'omvila.com.tr',
    'omvila.com',
  };

  /// API kökü (Prism: Android emülatörden `http://10.0.2.2:4010`).
  final String apiUrl;

  /// Aydınlatma metni adresi; yoksa metin bağlantısız yazılır (T-56 (7)).
  final String? privacyNoticeUrl;

  /// Uygulamanın tanıdığı bağlantı alan adları.
  final Set<String> linkHosts;

  /// Aydınlatma metni adresiyle kopya.
  AppConfig copyWithPrivacy(String? url) =>
      AppConfig(apiUrl: apiUrl, privacyNoticeUrl: url, linkHosts: linkHosts);
}
