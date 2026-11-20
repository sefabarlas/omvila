// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'manage_auth_result.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ManageAuthResultCWProxy {
  ManageAuthResult result(ManageAuthResultResultEnum result);

  ManageAuthResult accessToken(String? accessToken);

  ManageAuthResult expiresInSeconds(int? expiresInSeconds);

  ManageAuthResult refreshToken(String? refreshToken);

  ManageAuthResult signupToken(String? signupToken);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageAuthResult(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageAuthResult(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageAuthResult call({
    ManageAuthResultResultEnum result,
    String? accessToken,
    int? expiresInSeconds,
    String? refreshToken,
    String? signupToken,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfManageAuthResult.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfManageAuthResult.copyWith.fieldName(...)`
class _$ManageAuthResultCWProxyImpl implements _$ManageAuthResultCWProxy {
  const _$ManageAuthResultCWProxyImpl(this._value);

  final ManageAuthResult _value;

  @override
  ManageAuthResult result(ManageAuthResultResultEnum result) =>
      this(result: result);

  @override
  ManageAuthResult accessToken(String? accessToken) =>
      this(accessToken: accessToken);

  @override
  ManageAuthResult expiresInSeconds(int? expiresInSeconds) =>
      this(expiresInSeconds: expiresInSeconds);

  @override
  ManageAuthResult refreshToken(String? refreshToken) =>
      this(refreshToken: refreshToken);

  @override
  ManageAuthResult signupToken(String? signupToken) =>
      this(signupToken: signupToken);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageAuthResult(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageAuthResult(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageAuthResult call({
    Object? result = const $CopyWithPlaceholder(),
    Object? accessToken = const $CopyWithPlaceholder(),
    Object? expiresInSeconds = const $CopyWithPlaceholder(),
    Object? refreshToken = const $CopyWithPlaceholder(),
    Object? signupToken = const $CopyWithPlaceholder(),
  }) {
    return ManageAuthResult(
      result: result == const $CopyWithPlaceholder()
          ? _value.result
          // ignore: cast_nullable_to_non_nullable
          : result as ManageAuthResultResultEnum,
      accessToken: accessToken == const $CopyWithPlaceholder()
          ? _value.accessToken
          // ignore: cast_nullable_to_non_nullable
          : accessToken as String?,
      expiresInSeconds: expiresInSeconds == const $CopyWithPlaceholder()
          ? _value.expiresInSeconds
          // ignore: cast_nullable_to_non_nullable
          : expiresInSeconds as int?,
      refreshToken: refreshToken == const $CopyWithPlaceholder()
          ? _value.refreshToken
          // ignore: cast_nullable_to_non_nullable
          : refreshToken as String?,
      signupToken: signupToken == const $CopyWithPlaceholder()
          ? _value.signupToken
          // ignore: cast_nullable_to_non_nullable
          : signupToken as String?,
    );
  }
}

extension $ManageAuthResultCopyWith on ManageAuthResult {
  /// Returns a callable class that can be used as follows: `instanceOfManageAuthResult.copyWith(...)` or like so:`instanceOfManageAuthResult.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ManageAuthResultCWProxy get copyWith => _$ManageAuthResultCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ManageAuthResult _$ManageAuthResultFromJson(Map<String, dynamic> json) =>
    $checkedCreate('ManageAuthResult', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['result']);
      final val = ManageAuthResult(
        result: $checkedConvert(
          'result',
          (v) => $enumDecode(
            _$ManageAuthResultResultEnumEnumMap,
            v,
            unknownValue: ManageAuthResultResultEnum.unknownDefaultOpenApi,
          ),
        ),
        accessToken: $checkedConvert('accessToken', (v) => v as String?),
        expiresInSeconds: $checkedConvert(
          'expiresInSeconds',
          (v) => (v as num?)?.toInt(),
        ),
        refreshToken: $checkedConvert('refreshToken', (v) => v as String?),
        signupToken: $checkedConvert('signupToken', (v) => v as String?),
      );
      return val;
    });

Map<String, dynamic> _$ManageAuthResultToJson(ManageAuthResult instance) =>
    <String, dynamic>{
      'result': _$ManageAuthResultResultEnumEnumMap[instance.result]!,
      'accessToken': ?instance.accessToken,
      'expiresInSeconds': ?instance.expiresInSeconds,
      'refreshToken': ?instance.refreshToken,
      'signupToken': ?instance.signupToken,
    };

const _$ManageAuthResultResultEnumEnumMap = {
  ManageAuthResultResultEnum.signedIn: 'SignedIn',
  ManageAuthResultResultEnum.signupRequired: 'SignupRequired',
  ManageAuthResultResultEnum.unknownDefaultOpenApi: 'unknown_default_open_api',
};
