// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'token_pair.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$TokenPairCWProxy {
  TokenPair accessToken(String accessToken);

  TokenPair expiresInSeconds(int expiresInSeconds);

  TokenPair refreshToken(String? refreshToken);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `TokenPair(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// TokenPair(...).copyWith(id: 12, name: "My name")
  /// ````
  TokenPair call({
    String accessToken,
    int expiresInSeconds,
    String? refreshToken,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfTokenPair.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfTokenPair.copyWith.fieldName(...)`
class _$TokenPairCWProxyImpl implements _$TokenPairCWProxy {
  const _$TokenPairCWProxyImpl(this._value);

  final TokenPair _value;

  @override
  TokenPair accessToken(String accessToken) => this(accessToken: accessToken);

  @override
  TokenPair expiresInSeconds(int expiresInSeconds) =>
      this(expiresInSeconds: expiresInSeconds);

  @override
  TokenPair refreshToken(String? refreshToken) =>
      this(refreshToken: refreshToken);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `TokenPair(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// TokenPair(...).copyWith(id: 12, name: "My name")
  /// ````
  TokenPair call({
    Object? accessToken = const $CopyWithPlaceholder(),
    Object? expiresInSeconds = const $CopyWithPlaceholder(),
    Object? refreshToken = const $CopyWithPlaceholder(),
  }) {
    return TokenPair(
      accessToken: accessToken == const $CopyWithPlaceholder()
          ? _value.accessToken
          // ignore: cast_nullable_to_non_nullable
          : accessToken as String,
      expiresInSeconds: expiresInSeconds == const $CopyWithPlaceholder()
          ? _value.expiresInSeconds
          // ignore: cast_nullable_to_non_nullable
          : expiresInSeconds as int,
      refreshToken: refreshToken == const $CopyWithPlaceholder()
          ? _value.refreshToken
          // ignore: cast_nullable_to_non_nullable
          : refreshToken as String?,
    );
  }
}

extension $TokenPairCopyWith on TokenPair {
  /// Returns a callable class that can be used as follows: `instanceOfTokenPair.copyWith(...)` or like so:`instanceOfTokenPair.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$TokenPairCWProxy get copyWith => _$TokenPairCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

TokenPair _$TokenPairFromJson(Map<String, dynamic> json) =>
    $checkedCreate('TokenPair', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['accessToken', 'expiresInSeconds']);
      final val = TokenPair(
        accessToken: $checkedConvert('accessToken', (v) => v as String),
        expiresInSeconds: $checkedConvert(
          'expiresInSeconds',
          (v) => (v as num).toInt(),
        ),
        refreshToken: $checkedConvert('refreshToken', (v) => v as String?),
      );
      return val;
    });

Map<String, dynamic> _$TokenPairToJson(TokenPair instance) => <String, dynamic>{
  'accessToken': instance.accessToken,
  'expiresInSeconds': instance.expiresInSeconds,
  'refreshToken': ?instance.refreshToken,
};
