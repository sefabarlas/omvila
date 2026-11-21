// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'walk_in_start_request.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$WalkInStartRequestCWProxy {
  WalkInStartRequest staffId(String? staffId);

  WalkInStartRequest serviceId(String? serviceId);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `WalkInStartRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// WalkInStartRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  WalkInStartRequest call({String? staffId, String? serviceId});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfWalkInStartRequest.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfWalkInStartRequest.copyWith.fieldName(...)`
class _$WalkInStartRequestCWProxyImpl implements _$WalkInStartRequestCWProxy {
  const _$WalkInStartRequestCWProxyImpl(this._value);

  final WalkInStartRequest _value;

  @override
  WalkInStartRequest staffId(String? staffId) => this(staffId: staffId);

  @override
  WalkInStartRequest serviceId(String? serviceId) => this(serviceId: serviceId);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `WalkInStartRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// WalkInStartRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  WalkInStartRequest call({
    Object? staffId = const $CopyWithPlaceholder(),
    Object? serviceId = const $CopyWithPlaceholder(),
  }) {
    return WalkInStartRequest(
      staffId: staffId == const $CopyWithPlaceholder()
          ? _value.staffId
          // ignore: cast_nullable_to_non_nullable
          : staffId as String?,
      serviceId: serviceId == const $CopyWithPlaceholder()
          ? _value.serviceId
          // ignore: cast_nullable_to_non_nullable
          : serviceId as String?,
    );
  }
}

extension $WalkInStartRequestCopyWith on WalkInStartRequest {
  /// Returns a callable class that can be used as follows: `instanceOfWalkInStartRequest.copyWith(...)` or like so:`instanceOfWalkInStartRequest.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$WalkInStartRequestCWProxy get copyWith =>
      _$WalkInStartRequestCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

WalkInStartRequest _$WalkInStartRequestFromJson(Map<String, dynamic> json) =>
    $checkedCreate('WalkInStartRequest', json, ($checkedConvert) {
      final val = WalkInStartRequest(
        staffId: $checkedConvert('staffId', (v) => v as String?),
        serviceId: $checkedConvert('serviceId', (v) => v as String?),
      );
      return val;
    });

Map<String, dynamic> _$WalkInStartRequestToJson(WalkInStartRequest instance) =>
    <String, dynamic>{
      'staffId': ?instance.staffId,
      'serviceId': ?instance.serviceId,
    };
