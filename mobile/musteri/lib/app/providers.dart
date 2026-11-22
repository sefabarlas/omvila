import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/app/config.dart';
import 'package:omvila_musteri/features/appointments/data/appointments_repository.dart';
import 'package:omvila_musteri/features/auth/data/auth_repository.dart';
import 'package:omvila_musteri/features/auth/data/customer_session.dart';
import 'package:omvila_musteri/features/auth/data/sms_code_retriever.dart';
import 'package:omvila_musteri/features/brands/data/brands_repository.dart';
import 'package:omvila_musteri/features/notifications/data/device_repository.dart';
import 'package:omvila_musteri/features/notifications/data/push_service.dart';
import 'package:omvila_musteri/shared/external_actions.dart';
import 'package:omvila_musteri/shared/key_value_store.dart';
import 'package:omvila_ui/omvila_ui.dart';

// Bağımlılıklar. Testler bu sağlayıcıları sahteleriyle değiştirir
// (ProviderScope overrides).

/// Derleme ayarları.
final configProvider = Provider<AppConfig>((_) => AppConfig.fromEnvironment());

/// "Şimdi" yalnız buradan (AGENTS.md § 5).
final clockProvider = Provider<Clock>((_) => const SystemClock());

/// Cihaz deposu.
final keyValueStoreProvider = Provider<KeyValueStore>(
  (_) => const SecureKeyValueStore(),
);

/// Oturumsuz API (jeton yenileme; interceptor'sız).
final _bareApiProvider = Provider<OmvilaApi>(
  (ref) => createOmvilaApi(
    createOmvilaDio(
      baseUrl: ref.watch(configProvider).apiUrl,
      headers: const {'X-Client': customerAppClient},
    ),
  ),
);

/// Müşteri oturumu.
final customerSessionProvider = Provider<CustomerSession>((ref) {
  final bare = ref.watch(_bareApiProvider);
  return CustomerSession(
    store: ref.watch(keyValueStoreProvider),
    refresher: (refreshToken) async {
      final response = await bare.getMeAuthApi().meRefreshToken(
        meRefreshRequest: MeRefreshRequest(refreshToken: refreshToken),
      );
      return response.data!;
    },
  );
});

/// Müşteri jetonuyla API (Authorization, Idempotency-Key, yenileme).
final omvilaApiProvider = Provider<OmvilaApi>(
  (ref) => createOmvilaApi(
    createOmvilaDio(
      baseUrl: ref.watch(configProvider).apiUrl,
      session: ref.watch(customerSessionProvider),
      headers: const {'X-Client': customerAppClient},
    ),
  ),
);

/// Giriş.
final authRepositoryProvider = Provider<AuthRepository>(
  (ref) => ApiAuthRepository(
    api: ref.watch(omvilaApiProvider),
    session: ref.watch(customerSessionProvider),
  ),
);

/// Randevular.
final appointmentsRepositoryProvider = Provider<AppointmentsRepository>(
  (ref) => ApiAppointmentsRepository(ref.watch(omvilaApiProvider)),
);

/// Salonlar.
final brandsRepositoryProvider = Provider<BrandsRepository>(
  (ref) => ApiBrandsRepository(ref.watch(omvilaApiProvider)),
);

/// Cihaz kaydı.
final deviceRepositoryProvider = Provider<DeviceRepository>(
  (ref) => ApiDeviceRepository(ref.watch(omvilaApiProvider)),
);

/// Push katmanı; Firebase/APNs açılana kadar sahte (K4).
final pushServiceProvider = Provider<PushService>((_) => FakePushService());

/// SMS kodu otomatik okuma.
final smsCodeRetrieverProvider = Provider<SmsCodeRetriever>(
  (_) => platformSmsCodeRetriever(),
);

/// Harita, tarayıcı, takvim.
final externalActionsProvider = Provider<ExternalActions>(
  (_) => const PlatformExternalActions(),
);
