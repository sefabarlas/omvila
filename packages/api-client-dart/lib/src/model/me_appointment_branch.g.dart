// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'me_appointment_branch.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$MeAppointmentBranchCWProxy {
  MeAppointmentBranch id(String id);

  MeAppointmentBranch name(String name);

  MeAppointmentBranch addressLine(String addressLine);

  MeAppointmentBranch district(String district);

  MeAppointmentBranch phone(String? phone);

  MeAppointmentBranch latitude(num? latitude);

  MeAppointmentBranch longitude(num? longitude);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeAppointmentBranch(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeAppointmentBranch(...).copyWith(id: 12, name: "My name")
  /// ````
  MeAppointmentBranch call({
    String id,
    String name,
    String addressLine,
    String district,
    String? phone,
    num? latitude,
    num? longitude,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfMeAppointmentBranch.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfMeAppointmentBranch.copyWith.fieldName(...)`
class _$MeAppointmentBranchCWProxyImpl implements _$MeAppointmentBranchCWProxy {
  const _$MeAppointmentBranchCWProxyImpl(this._value);

  final MeAppointmentBranch _value;

  @override
  MeAppointmentBranch id(String id) => this(id: id);

  @override
  MeAppointmentBranch name(String name) => this(name: name);

  @override
  MeAppointmentBranch addressLine(String addressLine) =>
      this(addressLine: addressLine);

  @override
  MeAppointmentBranch district(String district) => this(district: district);

  @override
  MeAppointmentBranch phone(String? phone) => this(phone: phone);

  @override
  MeAppointmentBranch latitude(num? latitude) => this(latitude: latitude);

  @override
  MeAppointmentBranch longitude(num? longitude) => this(longitude: longitude);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeAppointmentBranch(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeAppointmentBranch(...).copyWith(id: 12, name: "My name")
  /// ````
  MeAppointmentBranch call({
    Object? id = const $CopyWithPlaceholder(),
    Object? name = const $CopyWithPlaceholder(),
    Object? addressLine = const $CopyWithPlaceholder(),
    Object? district = const $CopyWithPlaceholder(),
    Object? phone = const $CopyWithPlaceholder(),
    Object? latitude = const $CopyWithPlaceholder(),
    Object? longitude = const $CopyWithPlaceholder(),
  }) {
    return MeAppointmentBranch(
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as String,
      name: name == const $CopyWithPlaceholder()
          ? _value.name
          // ignore: cast_nullable_to_non_nullable
          : name as String,
      addressLine: addressLine == const $CopyWithPlaceholder()
          ? _value.addressLine
          // ignore: cast_nullable_to_non_nullable
          : addressLine as String,
      district: district == const $CopyWithPlaceholder()
          ? _value.district
          // ignore: cast_nullable_to_non_nullable
          : district as String,
      phone: phone == const $CopyWithPlaceholder()
          ? _value.phone
          // ignore: cast_nullable_to_non_nullable
          : phone as String?,
      latitude: latitude == const $CopyWithPlaceholder()
          ? _value.latitude
          // ignore: cast_nullable_to_non_nullable
          : latitude as num?,
      longitude: longitude == const $CopyWithPlaceholder()
          ? _value.longitude
          // ignore: cast_nullable_to_non_nullable
          : longitude as num?,
    );
  }
}

extension $MeAppointmentBranchCopyWith on MeAppointmentBranch {
  /// Returns a callable class that can be used as follows: `instanceOfMeAppointmentBranch.copyWith(...)` or like so:`instanceOfMeAppointmentBranch.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$MeAppointmentBranchCWProxy get copyWith =>
      _$MeAppointmentBranchCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MeAppointmentBranch _$MeAppointmentBranchFromJson(Map<String, dynamic> json) =>
    $checkedCreate('MeAppointmentBranch', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const ['id', 'name', 'addressLine', 'district'],
      );
      final val = MeAppointmentBranch(
        id: $checkedConvert('id', (v) => v as String),
        name: $checkedConvert('name', (v) => v as String),
        addressLine: $checkedConvert('addressLine', (v) => v as String),
        district: $checkedConvert('district', (v) => v as String),
        phone: $checkedConvert('phone', (v) => v as String?),
        latitude: $checkedConvert('latitude', (v) => v as num?),
        longitude: $checkedConvert('longitude', (v) => v as num?),
      );
      return val;
    });

Map<String, dynamic> _$MeAppointmentBranchToJson(
  MeAppointmentBranch instance,
) => <String, dynamic>{
  'id': instance.id,
  'name': instance.name,
  'addressLine': instance.addressLine,
  'district': instance.district,
  'phone': ?instance.phone,
  'latitude': ?instance.latitude,
  'longitude': ?instance.longitude,
};
