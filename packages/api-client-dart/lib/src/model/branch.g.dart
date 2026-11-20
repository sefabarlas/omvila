// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'branch.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$BranchCWProxy {
  Branch id(String id);

  Branch name(String name);

  Branch slug(String slug);

  Branch addressLine(String addressLine);

  Branch district(String district);

  Branch city(String city);

  Branch postalCode(String? postalCode);

  Branch latitude(num? latitude);

  Branch longitude(num? longitude);

  Branch phone(String? phone);

  Branch email(String? email);

  Branch timeZone(String timeZone);

  Branch slotIntervalMinutes(
    BranchSlotIntervalMinutesEnum? slotIntervalMinutes,
  );

  Branch isActive(bool isActive);

  Branch version(String version);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `Branch(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// Branch(...).copyWith(id: 12, name: "My name")
  /// ````
  Branch call({
    String id,
    String name,
    String slug,
    String addressLine,
    String district,
    String city,
    String? postalCode,
    num? latitude,
    num? longitude,
    String? phone,
    String? email,
    String timeZone,
    BranchSlotIntervalMinutesEnum? slotIntervalMinutes,
    bool isActive,
    String version,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfBranch.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfBranch.copyWith.fieldName(...)`
class _$BranchCWProxyImpl implements _$BranchCWProxy {
  const _$BranchCWProxyImpl(this._value);

  final Branch _value;

  @override
  Branch id(String id) => this(id: id);

  @override
  Branch name(String name) => this(name: name);

  @override
  Branch slug(String slug) => this(slug: slug);

  @override
  Branch addressLine(String addressLine) => this(addressLine: addressLine);

  @override
  Branch district(String district) => this(district: district);

  @override
  Branch city(String city) => this(city: city);

  @override
  Branch postalCode(String? postalCode) => this(postalCode: postalCode);

  @override
  Branch latitude(num? latitude) => this(latitude: latitude);

  @override
  Branch longitude(num? longitude) => this(longitude: longitude);

  @override
  Branch phone(String? phone) => this(phone: phone);

  @override
  Branch email(String? email) => this(email: email);

  @override
  Branch timeZone(String timeZone) => this(timeZone: timeZone);

  @override
  Branch slotIntervalMinutes(
    BranchSlotIntervalMinutesEnum? slotIntervalMinutes,
  ) => this(slotIntervalMinutes: slotIntervalMinutes);

  @override
  Branch isActive(bool isActive) => this(isActive: isActive);

  @override
  Branch version(String version) => this(version: version);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `Branch(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// Branch(...).copyWith(id: 12, name: "My name")
  /// ````
  Branch call({
    Object? id = const $CopyWithPlaceholder(),
    Object? name = const $CopyWithPlaceholder(),
    Object? slug = const $CopyWithPlaceholder(),
    Object? addressLine = const $CopyWithPlaceholder(),
    Object? district = const $CopyWithPlaceholder(),
    Object? city = const $CopyWithPlaceholder(),
    Object? postalCode = const $CopyWithPlaceholder(),
    Object? latitude = const $CopyWithPlaceholder(),
    Object? longitude = const $CopyWithPlaceholder(),
    Object? phone = const $CopyWithPlaceholder(),
    Object? email = const $CopyWithPlaceholder(),
    Object? timeZone = const $CopyWithPlaceholder(),
    Object? slotIntervalMinutes = const $CopyWithPlaceholder(),
    Object? isActive = const $CopyWithPlaceholder(),
    Object? version = const $CopyWithPlaceholder(),
  }) {
    return Branch(
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as String,
      name: name == const $CopyWithPlaceholder()
          ? _value.name
          // ignore: cast_nullable_to_non_nullable
          : name as String,
      slug: slug == const $CopyWithPlaceholder()
          ? _value.slug
          // ignore: cast_nullable_to_non_nullable
          : slug as String,
      addressLine: addressLine == const $CopyWithPlaceholder()
          ? _value.addressLine
          // ignore: cast_nullable_to_non_nullable
          : addressLine as String,
      district: district == const $CopyWithPlaceholder()
          ? _value.district
          // ignore: cast_nullable_to_non_nullable
          : district as String,
      city: city == const $CopyWithPlaceholder()
          ? _value.city
          // ignore: cast_nullable_to_non_nullable
          : city as String,
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
      timeZone: timeZone == const $CopyWithPlaceholder()
          ? _value.timeZone
          // ignore: cast_nullable_to_non_nullable
          : timeZone as String,
      slotIntervalMinutes: slotIntervalMinutes == const $CopyWithPlaceholder()
          ? _value.slotIntervalMinutes
          // ignore: cast_nullable_to_non_nullable
          : slotIntervalMinutes as BranchSlotIntervalMinutesEnum?,
      isActive: isActive == const $CopyWithPlaceholder()
          ? _value.isActive
          // ignore: cast_nullable_to_non_nullable
          : isActive as bool,
      version: version == const $CopyWithPlaceholder()
          ? _value.version
          // ignore: cast_nullable_to_non_nullable
          : version as String,
    );
  }
}

extension $BranchCopyWith on Branch {
  /// Returns a callable class that can be used as follows: `instanceOfBranch.copyWith(...)` or like so:`instanceOfBranch.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$BranchCWProxy get copyWith => _$BranchCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

Branch _$BranchFromJson(Map<String, dynamic> json) =>
    $checkedCreate('Branch', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'id',
          'name',
          'slug',
          'addressLine',
          'district',
          'city',
          'timeZone',
          'isActive',
          'version',
        ],
      );
      final val = Branch(
        id: $checkedConvert('id', (v) => v as String),
        name: $checkedConvert('name', (v) => v as String),
        slug: $checkedConvert('slug', (v) => v as String),
        addressLine: $checkedConvert('addressLine', (v) => v as String),
        district: $checkedConvert('district', (v) => v as String),
        city: $checkedConvert('city', (v) => v as String),
        postalCode: $checkedConvert('postalCode', (v) => v as String?),
        latitude: $checkedConvert('latitude', (v) => v as num?),
        longitude: $checkedConvert('longitude', (v) => v as num?),
        phone: $checkedConvert('phone', (v) => v as String?),
        email: $checkedConvert('email', (v) => v as String?),
        timeZone: $checkedConvert('timeZone', (v) => v as String),
        slotIntervalMinutes: $checkedConvert(
          'slotIntervalMinutes',
          (v) => $enumDecodeNullable(
            _$BranchSlotIntervalMinutesEnumEnumMap,
            v,
            unknownValue: BranchSlotIntervalMinutesEnum.unknownDefaultOpenApi,
          ),
        ),
        isActive: $checkedConvert('isActive', (v) => v as bool),
        version: $checkedConvert('version', (v) => v as String),
      );
      return val;
    });

Map<String, dynamic> _$BranchToJson(Branch instance) => <String, dynamic>{
  'id': instance.id,
  'name': instance.name,
  'slug': instance.slug,
  'addressLine': instance.addressLine,
  'district': instance.district,
  'city': instance.city,
  'postalCode': ?instance.postalCode,
  'latitude': ?instance.latitude,
  'longitude': ?instance.longitude,
  'phone': ?instance.phone,
  'email': ?instance.email,
  'timeZone': instance.timeZone,
  'slotIntervalMinutes':
      ?_$BranchSlotIntervalMinutesEnumEnumMap[instance.slotIntervalMinutes],
  'isActive': instance.isActive,
  'version': instance.version,
};

const _$BranchSlotIntervalMinutesEnumEnumMap = {
  BranchSlotIntervalMinutesEnum.number5: 5,
  BranchSlotIntervalMinutesEnum.number10: 10,
  BranchSlotIntervalMinutesEnum.number15: 15,
  BranchSlotIntervalMinutesEnum.unknownDefaultOpenApi: 11184809,
};
