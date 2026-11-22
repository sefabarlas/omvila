import 'package:flutter_secure_storage/flutter_secure_storage.dart';

/// Cihazda kalan küçük değerler (jetonlar, bildirim izni tercihi). Testte
/// bellekte depo.
abstract interface class KeyValueStore {
  /// Değer; yoksa null.
  Future<String?> read(String key);

  /// Yazar.
  Future<void> write(String key, String value);

  /// Siler.
  Future<void> delete(String key);
}

/// `flutter_secure_storage` (iOS Keychain, Android şifreli depo).
final class SecureKeyValueStore implements KeyValueStore {
  /// Güvenli depo.
  const SecureKeyValueStore([this._storage = const FlutterSecureStorage()]);

  final FlutterSecureStorage _storage;

  @override
  Future<String?> read(String key) => _storage.read(key: key);

  @override
  Future<void> write(String key, String value) =>
      _storage.write(key: key, value: value);

  @override
  Future<void> delete(String key) => _storage.delete(key: key);
}
