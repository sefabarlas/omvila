import 'package:flutter_secure_storage/flutter_secure_storage.dart';

/// Küçük kalıcı değerler (yenileme jetonu, bildirim izni tercihi).
abstract interface class KeyValueStore {
  /// Değer; yoksa null.
  Future<String?> read(String key);

  /// [value] null ise siler.
  Future<void> write(String key, String? value);
}

/// Cihazın güvenli deposu (iOS Keychain, Android Keystore şifreli).
class SecureKeyValueStore implements KeyValueStore {
  /// Güvenli depo.
  const SecureKeyValueStore();

  static const _storage = FlutterSecureStorage();

  @override
  Future<String?> read(String key) => _storage.read(key: key);

  @override
  Future<void> write(String key, String? value) => value == null
      ? _storage.delete(key: key)
      : _storage.write(key: key, value: value);
}
