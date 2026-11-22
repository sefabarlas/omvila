import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_isletme/features/auth/data/auth_tokens.dart';
import 'package:omvila_isletme/features/push/data/push_service.dart';
import 'package:omvila_isletme/shared/key_value_store.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// API adresi: `--dart-define=API_URL=http://10.0.2.2:4010` (Prism mock).
const apiUrl = String.fromEnvironment(
  'API_URL',
  defaultValue: 'http://localhost:4010',
);

/// Mobil istemci: yenileme jetonu gövdede döner.
const clientType = 'mobile';

/// "Şimdi" yalnız buradan (AGENTS.md § 5).
final clockProvider = Provider<Clock>((ref) => const SystemClock());

/// Kalıcı küçük değerler.
final keyValueStoreProvider = Provider<KeyValueStore>(
  (ref) => const SecureKeyValueStore(),
);

/// Push katmanı (K4: Firebase açılana kadar sahte).
final pushServiceProvider = Provider<PushService>((ref) => FakePushService());

/// Testte sahte HTTP bağdaştırıcısı; üretimde null (Dio varsayılanı).
final httpAdapterProvider = Provider<HttpClientAdapter?>((ref) => null);

/// Kimliksiz istemci: yalnız jeton yenileme (oturumlu istemciyle döngü
/// kurmamak için ayrı).
final _refreshApiProvider = Provider<OmvilaApi>(
  (ref) => createOmvilaApi(
    createOmvilaDio(
      baseUrl: apiUrl,
      httpClientAdapter: ref.watch(httpAdapterProvider),
    ),
  ),
);

/// Jetonlar ve yenileme.
final authTokensProvider = Provider<AuthTokens>(
  (ref) => AuthTokens(
    store: ref.watch(keyValueStoreProvider),
    refresher: (refreshToken) async {
      final response = await ref
          .read(_refreshApiProvider)
          .getManageAuthApi()
          .manageRefreshToken(
            xClientType: clientType,
            refreshRequest: RefreshRequest(refreshToken: refreshToken),
            extra: {skipAuthExtra: true},
          );
      return response.data!;
    },
  ),
);

/// Üretilmiş istemci (elle `http` çağrısı yok).
final apiProvider = Provider<OmvilaApi>((ref) {
  final dio = createOmvilaDio(
    baseUrl: apiUrl,
    session: ref.watch(authTokensProvider),
    httpClientAdapter: ref.watch(httpAdapterProvider),
  );
  return createOmvilaApi(dio);
});
