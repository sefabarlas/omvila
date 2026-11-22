import 'dart:async';

import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/features/appointments/data/appointments_repository.dart';
import 'package:omvila_musteri/features/appointments/domain/review_target.dart';
import 'package:omvila_musteri/features/auth/data/auth_repository.dart';
import 'package:omvila_musteri/features/auth/data/sms_code_retriever.dart';
import 'package:omvila_musteri/features/booking/data/booking_repository.dart';
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
  FakeAppointmentsRepository({
    this.home,
    this.error,
    this.pending = false,
    this.detail,
  });

  HomeData? home;
  Object? error;
  bool pending;
  int loads = 0;

  /// meGetAppointment yanıtı; [detailError] varsa hata.
  MeAppointment? detail;
  Object? detailError;
  bool detailPending = false;

  /// Geçmiş sayfaları: (notReviewed, cursor) → sayfa.
  Map<(bool, String?), MeAppointmentPage> pastPages = {};
  Object? pastError;
  bool pastPending = false;
  final pastRequests = <(bool, String?)>[];

  /// `/r/{code}` kodları → randevu.
  Map<String, MeAppointment> byCode = {};

  DaySearchResult? days;
  SlotSearchResult? slots;
  Object? slotsError;
  final slotRequests = <String>[];

  /// İptal, erteleme, havale bildirimi ve değerlendirme hataları.
  Object? actionError;
  final cancelled = <CustomerCancelRequestReasonEnum>[];
  final rescheduled = <DateTime>[];
  final reported = <String>[];

  ReviewTarget? review;
  Object? reviewError;
  final reviews = <(ReviewTarget, int, String?)>[];

  @override
  Future<HomeData> loadHome() {
    loads++;
    return _answer(home, error, pending: pending);
  }

  @override
  Future<MeAppointment> appointment(String id) =>
      _answer(detail, detailError, pending: detailPending);

  @override
  Future<MeAppointmentPage> past({bool notReviewed = false, String? cursor}) {
    pastRequests.add((notReviewed, cursor));
    return _answer(
      pastPages[(notReviewed, cursor)] ??
          MeAppointmentPage(items: const [], nextCursor: null),
      pastError,
      pending: pastPending,
    );
  }

  @override
  Future<MeAppointment> appointmentByCode(String accessCode) {
    final found = byCode[accessCode];
    return _answer(
      found,
      found == null ? const ApiProblem(status: 404, code: 'not_found') : null,
    );
  }

  @override
  Future<MeAppointment> cancel(
    MeAppointment appointment,
    CustomerCancelRequestReasonEnum reason,
  ) {
    cancelled.add(reason);
    return _answer(appointment, actionError);
  }

  @override
  Future<DaySearchResult> rescheduleDays(
    MeAppointment appointment, {
    required String from,
    required String to,
  }) => _answer(days, null);

  @override
  Future<SlotSearchResult> rescheduleSlots(String id, String date) {
    slotRequests.add(date);
    return _answer(slots, slotsError);
  }

  @override
  Future<MeAppointment> reschedule(
    MeAppointment appointment,
    DateTime startUtc,
  ) {
    rescheduled.add(startUtc);
    return _answer(appointment, actionError);
  }

  @override
  Future<MeAppointment> reportDeposit(String id) {
    reported.add(id);
    return _answer(detail, actionError);
  }

  @override
  Future<ReviewTarget> reviewTarget({
    String? appointmentId,
    String? accessCode,
  }) => _answer(review, reviewError);

  @override
  Future<void> submitReview(ReviewTarget target, int rating, String? comment) {
    reviews.add((target, rating, comment));
    return _answer<void>(null, actionError);
  }
}

final class FakeBookingRepository implements BookingRepository {
  FakeBookingRepository({
    this.catalogValue,
    this.catalogError,
    this.usable = const [],
    this.dayResult,
    this.slotResult,
    this.name = 'Merve Yıldız',
  });

  PublicCatalog? catalogValue;
  Object? catalogError;
  bool catalogPending = false;
  List<UsablePackageItem> usable;
  DaySearchResult? dayResult;
  Object? daysError;

  /// Gün → saatler; yoksa [slotResult].
  Map<String, SlotSearchResult> slotsByDate = {};
  SlotSearchResult? slotResult;
  String? name;

  /// Oluşturma yanıtları sırayla (hata veya randevu).
  final createAnswers = <Object>[];
  final created = <(PublicAppointmentCreate, String)>[];
  final dayRequests = <List<AppointmentItemRequest>>[];
  final slotRequests = <String>[];

  @override
  Future<PublicBrand> brand(String brandSlug) =>
      Future.error(const ApiProblem(status: 404, code: 'not_found'));

  @override
  Future<PublicCatalog> catalog(String branchId) =>
      _answer(catalogValue, catalogError, pending: catalogPending);

  @override
  Future<DaySearchResult> days(
    String branchId,
    List<AppointmentItemRequest> items, {
    required bool sameStaffForAll,
    required String from,
    required String to,
  }) {
    dayRequests.add(items);
    return _answer(dayResult, daysError);
  }

  @override
  Future<SlotSearchResult> slots(
    String branchId,
    List<AppointmentItemRequest> items, {
    required bool sameStaffForAll,
    required String date,
  }) {
    slotRequests.add(date);
    return _answer(slotsByDate[date] ?? slotResult, null);
  }

  @override
  Future<List<UsablePackageItem>> packages(String branchId) =>
      Future.value(usable);

  @override
  Future<String?> profileName() => Future.value(name);

  @override
  Future<PublicAppointment> create(
    String branchId,
    PublicAppointmentCreate body, {
    required String idempotencyKey,
  }) {
    created.add((body, idempotencyKey));
    final answer = createAnswers.removeAt(0);
    return answer is PublicAppointment
        ? Future.value(answer)
        : Future.error(answer);
  }
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
