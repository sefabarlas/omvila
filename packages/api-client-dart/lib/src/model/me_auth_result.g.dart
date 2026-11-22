// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'me_auth_result.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$MeAuthResultCWProxy {
  MeAuthResult accessToken(String accessToken);

  MeAuthResult expiresInSeconds(int expiresInSeconds);

  MeAuthResult refreshToken(String refreshToken);

  MeAuthResult isNewAccount(bool isNewAccount);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeAuthResult(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeAuthResult(...).copyWith(id: 12, name: "My name")
  /// ````
  MeAuthResult call({
    String accessToken,
    int expiresInSeconds,
    String refreshToken,
    bool isNewAccount,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfMeAuthResult.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfMeAuthResult.copyWith.fieldName(...)`
class _$MeAuthResultCWProxyImpl implements _$MeAuthResultCWProxy {
  const _$MeAuthResultCWProxyImpl(this._value);

  final MeAuthResult _value;

  @override
  MeAuthResult accessToken(String accessToken) =>
      this(accessToken: accessToken);

  @override
  MeAuthResult expiresInSeconds(int expiresInSeconds) =>
      this(expiresInSeconds: expiresInSeconds);

  @override
  MeAuthResult refreshToken(String refreshToken) =>
      this(refreshToken: refreshToken);

  @override
  MeAuthResult isNewAccount(bool isNewAccount) =>
      this(isNewAccount: isNewAccount);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeAuthResult(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeAuthResult(...).copyWith(id: 12, name: "My name")
  /// ````
  MeAuthResult call({
    Object? accessToken = const $CopyWithPlaceholder(),
    Object? expiresInSeconds = const $CopyWithPlaceholder(),
    Object? refreshToken = const $CopyWithPlaceholder(),
    Object? isNewAccount = const $CopyWithPlaceholder(),
  }) {
    return MeAuthResult(
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
          : refreshToken as String,
      isNewAccount: isNewAccount == const $CopyWithPlaceholder()
          ? _value.isNewAccount
          // ignore: cast_nullable_to_non_nullable
          : isNewAccount as bool,
    );
  }
}

extension $MeAuthResultCopyWith on MeAuthResult {
  /// Returns a callable class that can be used as follows: `instanceOfMeAuthResult.copyWith(...)` or like so:`instanceOfMeAuthResult.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$MeAuthResultCWProxy get copyWith => _$MeAuthResultCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MeAuthResult _$MeAuthResultFromJson(Map<String, dynamic> json) =>
    $checkedCreate('MeAuthResult', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'accessToken',
          'expiresInSeconds',
          'refreshToken',
          'isNewAccount',
        ],
      );
      final val = MeAuthResult(
        accessToken: $checkedConvert('accessToken', (v) => v as String),
        expiresInSeconds: $checkedConvert(
          'expiresInSeconds',
          (v) => (v as num).toInt(),
        ),
        refreshToken: $checkedConvert('refreshToken', (v) => v as String),
        isNewAccount: $checkedConvert('isNewAccount', (v) => v as bool),
      );
      return val;
    });

Map<String, dynamic> _$MeAuthResultToJson(MeAuthResult instance) =>
    <String, dynamic>{
      'accessToken': instance.accessToken,
      'expiresInSeconds': instance.expiresInSeconds,
      'refreshToken': instance.refreshToken,
      'isNewAccount': instance.isNewAccount,
    };
