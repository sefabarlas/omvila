// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_auth_result.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminAuthResultCWProxy {
  AdminAuthResult accessToken(String accessToken);

  AdminAuthResult expiresInSeconds(int expiresInSeconds);

  AdminAuthResult refreshToken(String refreshToken);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminAuthResult(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminAuthResult(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminAuthResult call({
    String accessToken,
    int expiresInSeconds,
    String refreshToken,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminAuthResult.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminAuthResult.copyWith.fieldName(...)`
class _$AdminAuthResultCWProxyImpl implements _$AdminAuthResultCWProxy {
  const _$AdminAuthResultCWProxyImpl(this._value);

  final AdminAuthResult _value;

  @override
  AdminAuthResult accessToken(String accessToken) =>
      this(accessToken: accessToken);

  @override
  AdminAuthResult expiresInSeconds(int expiresInSeconds) =>
      this(expiresInSeconds: expiresInSeconds);

  @override
  AdminAuthResult refreshToken(String refreshToken) =>
      this(refreshToken: refreshToken);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminAuthResult(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminAuthResult(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminAuthResult call({
    Object? accessToken = const $CopyWithPlaceholder(),
    Object? expiresInSeconds = const $CopyWithPlaceholder(),
    Object? refreshToken = const $CopyWithPlaceholder(),
  }) {
    return AdminAuthResult(
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
    );
  }
}

extension $AdminAuthResultCopyWith on AdminAuthResult {
  /// Returns a callable class that can be used as follows: `instanceOfAdminAuthResult.copyWith(...)` or like so:`instanceOfAdminAuthResult.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminAuthResultCWProxy get copyWith => _$AdminAuthResultCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminAuthResult _$AdminAuthResultFromJson(Map<String, dynamic> json) =>
    $checkedCreate('AdminAuthResult', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const ['accessToken', 'expiresInSeconds', 'refreshToken'],
      );
      final val = AdminAuthResult(
        accessToken: $checkedConvert('accessToken', (v) => v as String),
        expiresInSeconds: $checkedConvert(
          'expiresInSeconds',
          (v) => (v as num).toInt(),
        ),
        refreshToken: $checkedConvert('refreshToken', (v) => v as String),
      );
      return val;
    });

Map<String, dynamic> _$AdminAuthResultToJson(AdminAuthResult instance) =>
    <String, dynamic>{
      'accessToken': instance.accessToken,
      'expiresInSeconds': instance.expiresInSeconds,
      'refreshToken': instance.refreshToken,
    };
