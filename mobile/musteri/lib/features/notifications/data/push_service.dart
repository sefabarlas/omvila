/// Bildirim izninin durumu.
enum PushPermission {
  /// Henüz sorulmadı (sistem istemi açılabilir).
  notDetermined,

  /// İzin verildi.
  granted,

  /// Reddedildi (yalnız sistem ayarlarından açılır).
  denied,
}

/// Push katmanı (kodlama-flutter.md § Bildirimler). Firebase/APNs hesapları
/// açılana kadar (K4) yalnız [FakePushService] vardır; gerçek uygulama
/// `firebase_messaging` ile bu arayüzü karşılar ve jetonu meRegisterDevice'a
/// verir.
abstract interface class PushService {
  /// Şu anki izin.
  Future<PushPermission> permission();

  /// Sistem istemini açar; yalnız "Bildirimlere izin ver"den çağrılır.
  Future<PushPermission> requestPermission();

  /// Cihazın push jetonu; izin yoksa veya altyapı yoksa null.
  Future<String?> token();

  /// Uygulamanın sistem bildirim ayarlarını açar.
  Future<void> openSystemSettings();
}

/// Altyapısız push: istemi kabul edilmiş sayar, jeton vermez (cihaz kaydı
/// yapılmaz, hatırlatmalar SMS ile gider).
final class FakePushService implements PushService {
  /// Sahte push.
  FakePushService({this.grantOnRequest = true});

  /// İstemde verilecek yanıt.
  final bool grantOnRequest;

  PushPermission _permission = PushPermission.notDetermined;

  @override
  Future<PushPermission> permission() async => _permission;

  @override
  Future<PushPermission> requestPermission() async => _permission =
      grantOnRequest ? PushPermission.granted : PushPermission.denied;

  @override
  Future<String?> token() async => null;

  @override
  Future<void> openSystemSettings() async {}
}
