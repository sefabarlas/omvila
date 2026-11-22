import 'dart:async';

/// Bildirim izni durumu.
enum PushPermission {
  /// Henüz sorulmadı.
  notDetermined,

  /// İzin verildi.
  granted,

  /// Reddedildi.
  denied,
}

/// Bildirime dokununca açılacak ekran.
sealed class PushTarget {
  const PushTarget();
}

/// Randevu ayrıntısı (yeni randevu, onay bekliyor, kapora).
final class AppointmentTarget extends PushTarget {
  /// Randevu hedefi.
  const AppointmentTarget(this.appointmentId);

  /// Randevu kimliği.
  final String appointmentId;
}

/// Push katmanı. Gerçek uygulama (FCM/APNs) mağaza hesapları ve Firebase
/// projesi açılınca eklenir (K4, docs/acik-sorular.md T-107); o zamana kadar
/// [FakePushService].
abstract interface class PushService {
  /// Geçerli izin.
  Future<PushPermission> permission();

  /// Sistem izin istemini açar (yalnız "Bildirimlere izin ver"e basınca).
  Future<PushPermission> requestPermission();

  /// Cihazın push jetonu; yoksa null (kayıt yapılmaz).
  Future<String?> token();

  /// Kullanıcının dokunduğu bildirimlerin hedefi.
  Stream<PushTarget> get openedTargets;
}

/// Firebase olmadan çalışan sahte push: izin bellekte tutulur, jeton yoktur.
/// [simulateOpen] geliştirmede ve testte bildirime dokunmayı taklit eder.
class FakePushService implements PushService {
  /// Sahte push.
  FakePushService({PushPermission initial = PushPermission.notDetermined})
    : _permission = initial;

  PushPermission _permission;
  final _opened = StreamController<PushTarget>.broadcast();

  @override
  Future<PushPermission> permission() async => _permission;

  @override
  Future<PushPermission> requestPermission() async {
    if (_permission == PushPermission.notDetermined) {
      _permission = PushPermission.granted;
    }

    return _permission;
  }

  @override
  Future<String?> token() async => null;

  @override
  Stream<PushTarget> get openedTargets => _opened.stream;

  /// Bildirime dokunuldu.
  void simulateOpen(PushTarget target) => _opened.add(target);
}
