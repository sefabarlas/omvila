// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_refresh_request.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminRefreshRequestCWProxy {
  AdminRefreshRequest refreshToken(String refreshToken);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminRefreshRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminRefreshRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminRefreshRequest call({String refreshToken});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminRefreshRequest.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminRefreshRequest.copyWith.fieldName(...)`
class _$AdminRefreshRequestCWProxyImpl implements _$AdminRefreshRequestCWProxy {
  const _$AdminRefreshRequestCWProxyImpl(this._value);

  final AdminRefreshRequest _value;

  @override
  AdminRefreshRequest refreshToken(String refreshToken) =>
      this(refreshToken: refreshToken);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminRefreshRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminRefreshRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminRefreshRequest call({
    Object? refreshToken = const $CopyWithPlaceholder(),
  }) {
    return AdminRefreshRequest(
      refreshToken: refreshToken == const $CopyWithPlaceholder()
          ? _value.refreshToken
          // ignore: cast_nullable_to_non_nullable
          : refreshToken as String,
    );
  }
}

extension $AdminRefreshRequestCopyWith on AdminRefreshRequest {
  /// Returns a callable class that can be used as follows: `instanceOfAdminRefreshRequest.copyWith(...)` or like so:`instanceOfAdminRefreshRequest.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminRefreshRequestCWProxy get copyWith =>
      _$AdminRefreshRequestCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminRefreshRequest _$AdminRefreshRequestFromJson(Map<String, dynamic> json) =>
    $checkedCreate('AdminRefreshRequest', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['refreshToken']);
      final val = AdminRefreshRequest(
        refreshToken: $checkedConvert('refreshToken', (v) => v as String),
      );
      return val;
    });

Map<String, dynamic> _$AdminRefreshRequestToJson(
  AdminRefreshRequest instance,
) => <String, dynamic>{'refreshToken': instance.refreshToken};
