// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'branch_update.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$BranchUpdateCWProxy {
  BranchUpdate name(String? name);

  BranchUpdate addressLine(String? addressLine);

  BranchUpdate district(String? district);

  BranchUpdate city(String? city);

  BranchUpdate postalCode(String? postalCode);

  BranchUpdate latitude(num? latitude);

  BranchUpdate longitude(num? longitude);

  BranchUpdate phone(String? phone);

  BranchUpdate email(String? email);

  BranchUpdate slotIntervalMinutes(
    BranchUpdateSlotIntervalMinutesEnum? slotIntervalMinutes,
  );

  BranchUpdate isActive(bool? isActive);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `BranchUpdate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// BranchUpdate(...).copyWith(id: 12, name: "My name")
  /// ````
  BranchUpdate call({
    String? name,
    String? addressLine,
    String? district,
    String? city,
    String? postalCode,
    num? latitude,
    num? longitude,
    String? phone,
    String? email,
    BranchUpdateSlotIntervalMinutesEnum? slotIntervalMinutes,
    bool? isActive,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfBranchUpdate.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfBranchUpdate.copyWith.fieldName(...)`
class _$BranchUpdateCWProxyImpl implements _$BranchUpdateCWProxy {
  const _$BranchUpdateCWProxyImpl(this._value);

  final BranchUpdate _value;

  @override
  BranchUpdate name(String? name) => this(name: name);

  @override
  BranchUpdate addressLine(String? addressLine) =>
      this(addressLine: addressLine);

  @override
  BranchUpdate district(String? district) => this(district: district);

  @override
  BranchUpdate city(String? city) => this(city: city);

  @override
  BranchUpdate postalCode(String? postalCode) => this(postalCode: postalCode);

  @override
  BranchUpdate latitude(num? latitude) => this(latitude: latitude);

  @override
  BranchUpdate longitude(num? longitude) => this(longitude: longitude);

  @override
  BranchUpdate phone(String? phone) => this(phone: phone);

  @override
  BranchUpdate email(String? email) => this(email: email);

  @override
  BranchUpdate slotIntervalMinutes(
    BranchUpdateSlotIntervalMinutesEnum? slotIntervalMinutes,
  ) => this(slotIntervalMinutes: slotIntervalMinutes);

  @override
  BranchUpdate isActive(bool? isActive) => this(isActive: isActive);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `BranchUpdate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// BranchUpdate(...).copyWith(id: 12, name: "My name")
  /// ````
  BranchUpdate call({
    Object? name = const $CopyWithPlaceholder(),
    Object? addressLine = const $CopyWithPlaceholder(),
    Object? district = const $CopyWithPlaceholder(),
    Object? city = const $CopyWithPlaceholder(),
    Object? postalCode = const $CopyWithPlaceholder(),
    Object? latitude = const $CopyWithPlaceholder(),
    Object? longitude = const $CopyWithPlaceholder(),
    Object? phone = const $CopyWithPlaceholder(),
    Object? email = const $CopyWithPlaceholder(),
    Object? slotIntervalMinutes = const $CopyWithPlaceholder(),
    Object? isActive = const $CopyWithPlaceholder(),
  }) {
    return BranchUpdate(
      name: name == const $CopyWithPlaceholder()
          ? _value.name
          // ignore: cast_nullable_to_non_nullable
          : name as String?,
      addressLine: addressLine == const $CopyWithPlaceholder()
          ? _value.addressLine
          // ignore: cast_nullable_to_non_nullable
          : addressLine as String?,
      district: district == const $CopyWithPlaceholder()
          ? _value.district
          // ignore: cast_nullable_to_non_nullable
          : district as String?,
      city: city == const $CopyWithPlaceholder()
          ? _value.city
          // ignore: cast_nullable_to_non_nullable
          : city as String?,
      postalCode: postalCode == const $CopyWithPlaceholder()
          ? _value.postalCode
          // ignore: cast_nullable_to_non_nullable
          : postalCode as String?,
      latitude: latitude == const $CopyWithPlaceholder()
          ? _value.latitude
          // ignore: cast_nullable_to_non_nullable
          : latitude as num?,
      longitude: longitude == const $CopyWithPlaceholder()
          ? _value.longitude
          // ignore: cast_nullable_to_non_nullable
          : longitude as num?,
      phone: phone == const $CopyWithPlaceholder()
          ? _value.phone
          // ignore: cast_nullable_to_non_nullable
          : phone as String?,
      email: email == const $CopyWithPlaceholder()
          ? _value.email
          // ignore: cast_nullable_to_non_nullable
          : email as String?,
      slotIntervalMinutes: slotIntervalMinutes == const $CopyWithPlaceholder()
          ? _value.slotIntervalMinutes
          // ignore: cast_nullable_to_non_nullable
          : slotIntervalMinutes as BranchUpdateSlotIntervalMinutesEnum?,
      isActive: isActive == const $CopyWithPlaceholder()
          ? _value.isActive
          // ignore: cast_nullable_to_non_nullable
          : isActive as bool?,
    );
  }
}

extension $BranchUpdateCopyWith on BranchUpdate {
  /// Returns a callable class that can be used as follows: `instanceOfBranchUpdate.copyWith(...)` or like so:`instanceOfBranchUpdate.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$BranchUpdateCWProxy get copyWith => _$BranchUpdateCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

BranchUpdate _$BranchUpdateFromJson(Map<String, dynamic> json) =>
    $checkedCreate('BranchUpdate', json, ($checkedConvert) {
      final val = BranchUpdate(
        name: $checkedConvert('name', (v) => v as String?),
        addressLine: $checkedConvert('addressLine', (v) => v as String?),
        district: $checkedConvert('district', (v) => v as String?),
        city: $checkedConvert('city', (v) => v as String?),
        postalCode: $checkedConvert('postalCode', (v) => v as String?),
        latitude: $checkedConvert('latitude', (v) => v as num?),
        longitude: $checkedConvert('longitude', (v) => v as num?),
        phone: $checkedConvert('phone', (v) => v as String?),
        email: $checkedConvert('email', (v) => v as String?),
        slotIntervalMinutes: $checkedConvert(
          'slotIntervalMinutes',
          (v) => $enumDecodeNullable(
            _$BranchUpdateSlotIntervalMinutesEnumEnumMap,
            v,
            unknownValue:
                BranchUpdateSlotIntervalMinutesEnum.unknownDefaultOpenApi,
          ),
        ),
        isActive: $checkedConvert('isActive', (v) => v as bool?),
      );
      return val;
    });

Map<String, dynamic> _$BranchUpdateToJson(BranchUpdate instance) =>
    <String, dynamic>{
      'name': ?instance.name,
      'addressLine': ?instance.addressLine,
      'district': ?instance.district,
      'city': ?instance.city,
      'postalCode': ?instance.postalCode,
      'latitude': ?instance.latitude,
      'longitude': ?instance.longitude,
      'phone': ?instance.phone,
      'email': ?instance.email,
      'slotIntervalMinutes':
          ?_$BranchUpdateSlotIntervalMinutesEnumEnumMap[instance
              .slotIntervalMinutes],
      'isActive': ?instance.isActive,
    };

const _$BranchUpdateSlotIntervalMinutesEnumEnumMap = {
  BranchUpdateSlotIntervalMinutesEnum.number5: 5,
  BranchUpdateSlotIntervalMinutesEnum.number10: 10,
  BranchUpdateSlotIntervalMinutesEnum.number15: 15,
  BranchUpdateSlotIntervalMinutesEnum.unknownDefaultOpenApi: 11184809,
};
