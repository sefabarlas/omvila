// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'me_refresh_request.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$MeRefreshRequestCWProxy {
  MeRefreshRequest refreshToken(String refreshToken);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeRefreshRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeRefreshRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  MeRefreshRequest call({String refreshToken});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfMeRefreshRequest.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfMeRefreshRequest.copyWith.fieldName(...)`
class _$MeRefreshRequestCWProxyImpl implements _$MeRefreshRequestCWProxy {
  const _$MeRefreshRequestCWProxyImpl(this._value);

  final MeRefreshRequest _value;

  @override
  MeRefreshRequest refreshToken(String refreshToken) =>
      this(refreshToken: refreshToken);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeRefreshRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeRefreshRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  MeRefreshRequest call({Object? refreshToken = const $CopyWithPlaceholder()}) {
    return MeRefreshRequest(
      refreshToken: refreshToken == const $CopyWithPlaceholder()
          ? _value.refreshToken
          // ignore: cast_nullable_to_non_nullable
          : refreshToken as String,
    );
  }
}

extension $MeRefreshRequestCopyWith on MeRefreshRequest {
  /// Returns a callable class that can be used as follows: `instanceOfMeRefreshRequest.copyWith(...)` or like so:`instanceOfMeRefreshRequest.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$MeRefreshRequestCWProxy get copyWith => _$MeRefreshRequestCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MeRefreshRequest _$MeRefreshRequestFromJson(Map<String, dynamic> json) =>
    $checkedCreate('MeRefreshRequest', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['refreshToken']);
      final val = MeRefreshRequest(
        refreshToken: $checkedConvert('refreshToken', (v) => v as String),
      );
      return val;
    });

Map<String, dynamic> _$MeRefreshRequestToJson(MeRefreshRequest instance) =>
    <String, dynamic>{'refreshToken': instance.refreshToken};
