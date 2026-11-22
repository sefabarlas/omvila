import 'package:url_launcher/url_launcher.dart';

/// Uygulama dışına çıkan bağlantılar (tel:, sms:). Testte sahtesi.
// Arayüz bilerek: testte sahte uygulama verilir.
// ignore: one_member_abstracts
abstract interface class ExternalLauncher {
  /// Bağlantıyı dış uygulamada açar; açılamazsa false.
  Future<bool> open(Uri url);
}

/// `url_launcher`.
final class PlatformExternalLauncher implements ExternalLauncher {
  /// Platform bağlantıları.
  const PlatformExternalLauncher();

  @override
  Future<bool> open(Uri url) =>
      launchUrl(url, mode: LaunchMode.externalApplication);
}
