// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'walk_in_create.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$WalkInCreateCWProxy {
  WalkInCreate branchId(String branchId);

  WalkInCreate brandCustomerId(String? brandCustomerId);

  WalkInCreate fullName(String fullName);

  WalkInCreate phone(String? phone);

  WalkInCreate serviceId(String? serviceId);

  WalkInCreate preferredStaffId(String? preferredStaffId);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `WalkInCreate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// WalkInCreate(...).copyWith(id: 12, name: "My name")
  /// ````
  WalkInCreate call({
    String branchId,
    String? brandCustomerId,
    String fullName,
    String? phone,
    String? serviceId,
    String? preferredStaffId,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfWalkInCreate.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfWalkInCreate.copyWith.fieldName(...)`
class _$WalkInCreateCWProxyImpl implements _$WalkInCreateCWProxy {
  const _$WalkInCreateCWProxyImpl(this._value);

  final WalkInCreate _value;

  @override
  WalkInCreate branchId(String branchId) => this(branchId: branchId);

  @override
  WalkInCreate brandCustomerId(String? brandCustomerId) =>
      this(brandCustomerId: brandCustomerId);

  @override
  WalkInCreate fullName(String fullName) => this(fullName: fullName);

  @override
  WalkInCreate phone(String? phone) => this(phone: phone);

  @override
  WalkInCreate serviceId(String? serviceId) => this(serviceId: serviceId);

  @override
  WalkInCreate preferredStaffId(String? preferredStaffId) =>
      this(preferredStaffId: preferredStaffId);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `WalkInCreate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// WalkInCreate(...).copyWith(id: 12, name: "My name")
  /// ````
  WalkInCreate call({
    Object? branchId = const $CopyWithPlaceholder(),
    Object? brandCustomerId = const $CopyWithPlaceholder(),
    Object? fullName = const $CopyWithPlaceholder(),
    Object? phone = const $CopyWithPlaceholder(),
    Object? serviceId = const $CopyWithPlaceholder(),
    Object? preferredStaffId = const $CopyWithPlaceholder(),
  }) {
    return WalkInCreate(
      branchId: branchId == const $CopyWithPlaceholder()
          ? _value.branchId
          // ignore: cast_nullable_to_non_nullable
          : branchId as String,
      brandCustomerId: brandCustomerId == const $CopyWithPlaceholder()
          ? _value.brandCustomerId
          // ignore: cast_nullable_to_non_nullable
          : brandCustomerId as String?,
      fullName: fullName == const $CopyWithPlaceholder()
          ? _value.fullName
          // ignore: cast_nullable_to_non_nullable
          : fullName as String,
      phone: phone == const $CopyWithPlaceholder()
          ? _value.phone
          // ignore: cast_nullable_to_non_nullable
          : phone as String?,
      serviceId: serviceId == const $CopyWithPlaceholder()
          ? _value.serviceId
          // ignore: cast_nullable_to_non_nullable
          : serviceId as String?,
      preferredStaffId: preferredStaffId == const $CopyWithPlaceholder()
          ? _value.preferredStaffId
          // ignore: cast_nullable_to_non_nullable
          : preferredStaffId as String?,
    );
  }
}

extension $WalkInCreateCopyWith on WalkInCreate {
  /// Returns a callable class that can be used as follows: `instanceOfWalkInCreate.copyWith(...)` or like so:`instanceOfWalkInCreate.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$WalkInCreateCWProxy get copyWith => _$WalkInCreateCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

WalkInCreate _$WalkInCreateFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('WalkInCreate', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['branchId', 'fullName']);
  final val = WalkInCreate(
    branchId: $checkedConvert('branchId', (v) => v as String),
    brandCustomerId: $checkedConvert('brandCustomerId', (v) => v as String?),
    fullName: $checkedConvert('fullName', (v) => v as String),
    phone: $checkedConvert('phone', (v) => v as String?),
    serviceId: $checkedConvert('serviceId', (v) => v as String?),
    preferredStaffId: $checkedConvert('preferredStaffId', (v) => v as String?),
  );
  return val;
});

Map<String, dynamic> _$WalkInCreateToJson(WalkInCreate instance) =>
    <String, dynamic>{
      'branchId': instance.branchId,
      'brandCustomerId': ?instance.brandCustomerId,
      'fullName': instance.fullName,
      'phone': ?instance.phone,
      'serviceId': ?instance.serviceId,
      'preferredStaffId': ?instance.preferredStaffId,
    };
