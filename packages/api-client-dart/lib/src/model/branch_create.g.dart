// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'branch_create.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$BranchCreateCWProxy {
  BranchCreate name(String name);

  BranchCreate addressLine(String addressLine);

  BranchCreate district(String district);

  BranchCreate city(String city);

  BranchCreate phone(String? phone);

  BranchCreate copyWorkingHoursFromBranchId(
    String? copyWorkingHoursFromBranchId,
  );

  BranchCreate copyServicePricesFromBranchId(
    String? copyServicePricesFromBranchId,
  );

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `BranchCreate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// BranchCreate(...).copyWith(id: 12, name: "My name")
  /// ````
  BranchCreate call({
    String name,
    String addressLine,
    String district,
    String city,
    String? phone,
    String? copyWorkingHoursFromBranchId,
    String? copyServicePricesFromBranchId,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfBranchCreate.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfBranchCreate.copyWith.fieldName(...)`
class _$BranchCreateCWProxyImpl implements _$BranchCreateCWProxy {
  const _$BranchCreateCWProxyImpl(this._value);

  final BranchCreate _value;

  @override
  BranchCreate name(String name) => this(name: name);

  @override
  BranchCreate addressLine(String addressLine) =>
      this(addressLine: addressLine);

  @override
  BranchCreate district(String district) => this(district: district);

  @override
  BranchCreate city(String city) => this(city: city);

  @override
  BranchCreate phone(String? phone) => this(phone: phone);

  @override
  BranchCreate copyWorkingHoursFromBranchId(
    String? copyWorkingHoursFromBranchId,
  ) => this(copyWorkingHoursFromBranchId: copyWorkingHoursFromBranchId);

  @override
  BranchCreate copyServicePricesFromBranchId(
    String? copyServicePricesFromBranchId,
  ) => this(copyServicePricesFromBranchId: copyServicePricesFromBranchId);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `BranchCreate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// BranchCreate(...).copyWith(id: 12, name: "My name")
  /// ````
  BranchCreate call({
    Object? name = const $CopyWithPlaceholder(),
    Object? addressLine = const $CopyWithPlaceholder(),
    Object? district = const $CopyWithPlaceholder(),
    Object? city = const $CopyWithPlaceholder(),
    Object? phone = const $CopyWithPlaceholder(),
    Object? copyWorkingHoursFromBranchId = const $CopyWithPlaceholder(),
    Object? copyServicePricesFromBranchId = const $CopyWithPlaceholder(),
  }) {
    return BranchCreate(
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
      city: city == const $CopyWithPlaceholder()
          ? _value.city
          // ignore: cast_nullable_to_non_nullable
          : city as String,
      phone: phone == const $CopyWithPlaceholder()
          ? _value.phone
          // ignore: cast_nullable_to_non_nullable
          : phone as String?,
      copyWorkingHoursFromBranchId:
          copyWorkingHoursFromBranchId == const $CopyWithPlaceholder()
          ? _value.copyWorkingHoursFromBranchId
          // ignore: cast_nullable_to_non_nullable
          : copyWorkingHoursFromBranchId as String?,
      copyServicePricesFromBranchId:
          copyServicePricesFromBranchId == const $CopyWithPlaceholder()
          ? _value.copyServicePricesFromBranchId
          // ignore: cast_nullable_to_non_nullable
          : copyServicePricesFromBranchId as String?,
    );
  }
}

extension $BranchCreateCopyWith on BranchCreate {
  /// Returns a callable class that can be used as follows: `instanceOfBranchCreate.copyWith(...)` or like so:`instanceOfBranchCreate.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$BranchCreateCWProxy get copyWith => _$BranchCreateCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

BranchCreate _$BranchCreateFromJson(Map<String, dynamic> json) =>
    $checkedCreate('BranchCreate', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const ['name', 'addressLine', 'district', 'city'],
      );
      final val = BranchCreate(
        name: $checkedConvert('name', (v) => v as String),
        addressLine: $checkedConvert('addressLine', (v) => v as String),
        district: $checkedConvert('district', (v) => v as String),
        city: $checkedConvert('city', (v) => v as String),
        phone: $checkedConvert('phone', (v) => v as String?),
        copyWorkingHoursFromBranchId: $checkedConvert(
          'copyWorkingHoursFromBranchId',
          (v) => v as String?,
        ),
        copyServicePricesFromBranchId: $checkedConvert(
          'copyServicePricesFromBranchId',
          (v) => v as String?,
        ),
      );
      return val;
    });

Map<String, dynamic> _$BranchCreateToJson(BranchCreate instance) =>
    <String, dynamic>{
      'name': instance.name,
      'addressLine': instance.addressLine,
      'district': instance.district,
      'city': instance.city,
      'phone': instance.phone,
      'copyWorkingHoursFromBranchId': instance.copyWorkingHoursFromBranchId,
      'copyServicePricesFromBranchId': instance.copyServicePricesFromBranchId,
    };
