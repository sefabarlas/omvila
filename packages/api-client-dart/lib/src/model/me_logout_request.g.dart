// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'me_logout_request.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$MeLogoutRequestCWProxy {
  MeLogoutRequest refreshToken(String? refreshToken);

  MeLogoutRequest pushToken(String? pushToken);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeLogoutRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeLogoutRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  MeLogoutRequest call({String? refreshToken, String? pushToken});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfMeLogoutRequest.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfMeLogoutRequest.copyWith.fieldName(...)`
class _$MeLogoutRequestCWProxyImpl implements _$MeLogoutRequestCWProxy {
  const _$MeLogoutRequestCWProxyImpl(this._value);

  final MeLogoutRequest _value;

  @override
  MeLogoutRequest refreshToken(String? refreshToken) =>
      this(refreshToken: refreshToken);

  @override
  MeLogoutRequest pushToken(String? pushToken) => this(pushToken: pushToken);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeLogoutRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeLogoutRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  MeLogoutRequest call({
    Object? refreshToken = const $CopyWithPlaceholder(),
    Object? pushToken = const $CopyWithPlaceholder(),
  }) {
    return MeLogoutRequest(
      refreshToken: refreshToken == const $CopyWithPlaceholder()
          ? _value.refreshToken
          // ignore: cast_nullable_to_non_nullable
          : refreshToken as String?,
      pushToken: pushToken == const $CopyWithPlaceholder()
          ? _value.pushToken
          // ignore: cast_nullable_to_non_nullable
          : pushToken as String?,
    );
  }
}

extension $MeLogoutRequestCopyWith on MeLogoutRequest {
  /// Returns a callable class that can be used as follows: `instanceOfMeLogoutRequest.copyWith(...)` or like so:`instanceOfMeLogoutRequest.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$MeLogoutRequestCWProxy get copyWith => _$MeLogoutRequestCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MeLogoutRequest _$MeLogoutRequestFromJson(Map<String, dynamic> json) =>
    $checkedCreate('MeLogoutRequest', json, ($checkedConvert) {
      final val = MeLogoutRequest(
        refreshToken: $checkedConvert('refreshToken', (v) => v as String?),
        pushToken: $checkedConvert('pushToken', (v) => v as String?),
      );
      return val;
    });

Map<String, dynamic> _$MeLogoutRequestToJson(MeLogoutRequest instance) =>
    <String, dynamic>{
      'refreshToken': ?instance.refreshToken,
      'pushToken': ?instance.pushToken,
    };
