// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'public_appointment_branch.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PublicAppointmentBranchCWProxy {
  PublicAppointmentBranch id(String id);

  PublicAppointmentBranch name(String name);

  PublicAppointmentBranch addressLine(String addressLine);

  PublicAppointmentBranch district(String district);

  PublicAppointmentBranch phone(String? phone);

  PublicAppointmentBranch latitude(num? latitude);

  PublicAppointmentBranch longitude(num? longitude);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicAppointmentBranch(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicAppointmentBranch(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicAppointmentBranch call({
    String id,
    String name,
    String addressLine,
    String district,
    String? phone,
    num? latitude,
    num? longitude,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPublicAppointmentBranch.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPublicAppointmentBranch.copyWith.fieldName(...)`
class _$PublicAppointmentBranchCWProxyImpl
    implements _$PublicAppointmentBranchCWProxy {
  const _$PublicAppointmentBranchCWProxyImpl(this._value);

  final PublicAppointmentBranch _value;

  @override
  PublicAppointmentBranch id(String id) => this(id: id);

  @override
  PublicAppointmentBranch name(String name) => this(name: name);

  @override
  PublicAppointmentBranch addressLine(String addressLine) =>
      this(addressLine: addressLine);

  @override
  PublicAppointmentBranch district(String district) => this(district: district);

  @override
  PublicAppointmentBranch phone(String? phone) => this(phone: phone);

  @override
  PublicAppointmentBranch latitude(num? latitude) => this(latitude: latitude);

  @override
  PublicAppointmentBranch longitude(num? longitude) =>
      this(longitude: longitude);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicAppointmentBranch(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicAppointmentBranch(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicAppointmentBranch call({
    Object? id = const $CopyWithPlaceholder(),
    Object? name = const $CopyWithPlaceholder(),
    Object? addressLine = const $CopyWithPlaceholder(),
    Object? district = const $CopyWithPlaceholder(),
    Object? phone = const $CopyWithPlaceholder(),
    Object? latitude = const $CopyWithPlaceholder(),
    Object? longitude = const $CopyWithPlaceholder(),
  }) {
    return PublicAppointmentBranch(
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

extension $PublicAppointmentBranchCopyWith on PublicAppointmentBranch {
  /// Returns a callable class that can be used as follows: `instanceOfPublicAppointmentBranch.copyWith(...)` or like so:`instanceOfPublicAppointmentBranch.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PublicAppointmentBranchCWProxy get copyWith =>
      _$PublicAppointmentBranchCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PublicAppointmentBranch _$PublicAppointmentBranchFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('PublicAppointmentBranch', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const ['id', 'name', 'addressLine', 'district'],
  );
  final val = PublicAppointmentBranch(
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

Map<String, dynamic> _$PublicAppointmentBranchToJson(
  PublicAppointmentBranch instance,
) => <String, dynamic>{
  'id': instance.id,
  'name': instance.name,
  'addressLine': instance.addressLine,
  'district': instance.district,
  'phone': ?instance.phone,
  'latitude': ?instance.latitude,
  'longitude': ?instance.longitude,
};
