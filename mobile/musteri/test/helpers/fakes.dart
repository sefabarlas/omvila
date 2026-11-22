import 'dart:async';

import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/features/appointments/data/appointments_repository.dart';
import 'package:omvila_musteri/features/auth/data/auth_repository.dart';
import 'package:omvila_musteri/features/auth/data/sms_code_retriever.dart';
import 'package:omvila_musteri/features/brands/data/brands_repository.dart';
import 'package:omvila_musteri/features/notifications/data/device_repository.dart';
import 'package:omvila_musteri/features/notifications/data/push_service.dart';
import 'package:omvila_musteri/shared/external_actions.dart';
import 'package:omvila_musteri/shared/key_value_store.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Bellekte depo.
final class MemoryKeyValueStore implements KeyValueStore {
  MemoryKeyValueStore([Map<String, String>? initial]) : values = {...?initial};

  final Map<String, String> values;

  @override
  Future<String?> read(String key) async => values[key];

  @override
  Future<void> write(String key, String value) async => values[key] = value;

  @override
  Future<void> delete(String key) async => values.remove(key);
}

/// Oturum açık ve bildirim izni sorulmuş cihaz.
Map<String, String> signedInStore({bool asked = true, bool skipped = false}) =>
    {
      'me.accessToken': 'erisim',
      'me.refreshToken': 'yenileme',
      if (asked) 'push.asked': '1',
      if (skipped) 'push.skipped': '1',
    };

/// Yanıtı ayarlanabilen sahte; [pending] true ise hiç tamamlanmaz.
Future<T> _answer<T>(T? value, Object? error, {bool pending = false}) {
  if (pending) {
    return Completer<T>().future;
  }

  if (error != null) {
    return Future.error(error);
  }

  return Future.value(value as T);
}

final class FakeAuthRepository implements AuthRepository {
  FakeAuthRepository({this.store});

  /// Doğrulamada jetonların yazılacağı depo.
  final MemoryKeyValueStore? store;

  OtpChallengeResponse challenge = OtpChallengeResponse(
    otpId: 'otp-1',
    expiresInSeconds: 180,
    resendAfterSeconds: 60,
    phoneMasked: '0532 *** ** 67',
  );
  Object? requestError;
  Object? verifyError;
  final requested = <String>[];
  final verified = <String>[];

  @override
  Future<OtpChallengeResponse> requestOtp(String phone) {
    requested.add(phone);
    return _answer(challenge, requestError);
  }

  @override
  Future<void> verifyOtp({required String otpId, required String code}) async {
    verified.add(code);
    await _answer<void>(null, verifyError);
    store?.values.addAll(signedInStore(asked: false));
  }
}

final class FakeAppointmentsRepository implements AppointmentsRepository {
  FakeAppointmentsRepository({this.home, this.error, this.pending = false});

  HomeData? home;
  Object? error;
  bool pending;
  MeAppointment? detail;
  int loads = 0;

  @override
  Future<HomeData> loadHome() {
    loads++;
    return _answer(home, error, pending: pending);
  }

  @override
  Future<MeAppointment> appointment(String id) => _answer(detail, null);
}

final class FakeBrandsRepository implements BrandsRepository {
  FakeBrandsRepository({
    this.brands = const [],
    this.error,
    this.pending = false,
  });

  List<MeBrand> brands;
  Object? error;
  Object? saveError;
  bool pending;
  final saved = <(String, BrandSource)>[];

  @override
  Future<List<MeBrand>> list() => _answer(brands, error, pending: pending);

  @override
  Future<MeBrand> save(String brandSlug, BrandSource source) {
    saved.add((brandSlug, source));
    return _answer(
      brands.isEmpty ? null : brands.first,
      saveError,
      pending: pending,
    );
  }
}

final class FakeDeviceRepository implements DeviceRepository {
  final tokens = <String>[];

  @override
  Future<void> register(String pushToken) async => tokens.add(pushToken);
}

final class FakePush implements PushService {
  FakePush({this.grant = true, this.pushToken});

  final bool grant;
  final String? pushToken;
  PushPermission current = PushPermission.notDetermined;
  int requests = 0;
  int settingsOpened = 0;

  @override
  Future<PushPermission> permission() async => current;

  @override
  Future<PushPermission> requestPermission() async {
    requests++;
    return current = grant ? PushPermission.granted : PushPermission.denied;
  }

  @override
  Future<String?> token() async => pushToken;

  @override
  Future<void> openSystemSettings() async => settingsOpened++;
}

final class FakeSmsCodeRetriever implements SmsCodeRetriever {
  final controller = StreamController<String>.broadcast();
  bool stopped = false;

  @override
  Stream<String> codes() => controller.stream;

  @override
  Future<void> stop() async => stopped = true;
}

final class FakeExternalActions implements ExternalActions {
  final urls = <Uri>[];
  final calendar = <CalendarEntry>[];

  @override
  Future<bool> openUrl(Uri url) async {
    urls.add(url);
    return true;
  }

  @override
  Future<bool> addToCalendar(CalendarEntry entry) async {
    calendar.add(entry);
    return true;
  }
}

/// İlerletilebilen saat.
final class MutableClock implements Clock {
  MutableClock(this._now);

  DateTime _now;

  void advance(Duration by) => _now = _now.add(by);

  @override
  DateTime now() => _now;
}
