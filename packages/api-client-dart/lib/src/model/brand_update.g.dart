// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'brand_update.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$BrandUpdateCWProxy {
  BrandUpdate name(String? name);

  BrandUpdate industryCode(IndustryCode? industryCode);

  BrandUpdate shortDescription(String? shortDescription);

  BrandUpdate phone(String? phone);

  BrandUpdate email(String? email);

  BrandUpdate instagram(String? instagram);

  BrandUpdate website(String? website);

  BrandUpdate legalName(String? legalName);

  BrandUpdate taxOffice(String? taxOffice);

  BrandUpdate taxNumber(String? taxNumber);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `BrandUpdate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// BrandUpdate(...).copyWith(id: 12, name: "My name")
  /// ````
  BrandUpdate call({
    String? name,
    IndustryCode? industryCode,
    String? shortDescription,
    String? phone,
    String? email,
    String? instagram,
    String? website,
    String? legalName,
    String? taxOffice,
    String? taxNumber,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfBrandUpdate.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfBrandUpdate.copyWith.fieldName(...)`
class _$BrandUpdateCWProxyImpl implements _$BrandUpdateCWProxy {
  const _$BrandUpdateCWProxyImpl(this._value);

  final BrandUpdate _value;

  @override
  BrandUpdate name(String? name) => this(name: name);

  @override
  BrandUpdate industryCode(IndustryCode? industryCode) =>
      this(industryCode: industryCode);

  @override
  BrandUpdate shortDescription(String? shortDescription) =>
      this(shortDescription: shortDescription);

  @override
  BrandUpdate phone(String? phone) => this(phone: phone);

  @override
  BrandUpdate email(String? email) => this(email: email);

  @override
  BrandUpdate instagram(String? instagram) => this(instagram: instagram);

  @override
  BrandUpdate website(String? website) => this(website: website);

  @override
  BrandUpdate legalName(String? legalName) => this(legalName: legalName);

  @override
  BrandUpdate taxOffice(String? taxOffice) => this(taxOffice: taxOffice);

  @override
  BrandUpdate taxNumber(String? taxNumber) => this(taxNumber: taxNumber);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `BrandUpdate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// BrandUpdate(...).copyWith(id: 12, name: "My name")
  /// ````
  BrandUpdate call({
    Object? name = const $CopyWithPlaceholder(),
    Object? industryCode = const $CopyWithPlaceholder(),
    Object? shortDescription = const $CopyWithPlaceholder(),
    Object? phone = const $CopyWithPlaceholder(),
    Object? email = const $CopyWithPlaceholder(),
    Object? instagram = const $CopyWithPlaceholder(),
    Object? website = const $CopyWithPlaceholder(),
    Object? legalName = const $CopyWithPlaceholder(),
    Object? taxOffice = const $CopyWithPlaceholder(),
    Object? taxNumber = const $CopyWithPlaceholder(),
  }) {
    return BrandUpdate(
      name: name == const $CopyWithPlaceholder()
          ? _value.name
          // ignore: cast_nullable_to_non_nullable
          : name as String?,
      industryCode: industryCode == const $CopyWithPlaceholder()
          ? _value.industryCode
          // ignore: cast_nullable_to_non_nullable
          : industryCode as IndustryCode?,
      shortDescription: shortDescription == const $CopyWithPlaceholder()
          ? _value.shortDescription
          // ignore: cast_nullable_to_non_nullable
          : shortDescription as String?,
      phone: phone == const $CopyWithPlaceholder()
          ? _value.phone
          // ignore: cast_nullable_to_non_nullable
          : phone as String?,
      email: email == const $CopyWithPlaceholder()
          ? _value.email
          // ignore: cast_nullable_to_non_nullable
          : email as String?,
      instagram: instagram == const $CopyWithPlaceholder()
          ? _value.instagram
          // ignore: cast_nullable_to_non_nullable
          : instagram as String?,
      website: website == const $CopyWithPlaceholder()
          ? _value.website
          // ignore: cast_nullable_to_non_nullable
          : website as String?,
      legalName: legalName == const $CopyWithPlaceholder()
          ? _value.legalName
          // ignore: cast_nullable_to_non_nullable
          : legalName as String?,
      taxOffice: taxOffice == const $CopyWithPlaceholder()
          ? _value.taxOffice
          // ignore: cast_nullable_to_non_nullable
          : taxOffice as String?,
      taxNumber: taxNumber == const $CopyWithPlaceholder()
          ? _value.taxNumber
          // ignore: cast_nullable_to_non_nullable
          : taxNumber as String?,
    );
  }
}

extension $BrandUpdateCopyWith on BrandUpdate {
  /// Returns a callable class that can be used as follows: `instanceOfBrandUpdate.copyWith(...)` or like so:`instanceOfBrandUpdate.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$BrandUpdateCWProxy get copyWith => _$BrandUpdateCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

BrandUpdate _$BrandUpdateFromJson(Map<String, dynamic> json) =>
    $checkedCreate('BrandUpdate', json, ($checkedConvert) {
      final val = BrandUpdate(
        name: $checkedConvert('name', (v) => v as String?),
        industryCode: $checkedConvert(
          'industryCode',
          (v) => $enumDecodeNullable(
            _$IndustryCodeEnumMap,
            v,
            unknownValue: IndustryCode.unknownDefaultOpenApi,
          ),
        ),
        shortDescription: $checkedConvert(
          'shortDescription',
          (v) => v as String?,
        ),
        phone: $checkedConvert('phone', (v) => v as String?),
        email: $checkedConvert('email', (v) => v as String?),
        instagram: $checkedConvert('instagram', (v) => v as String?),
        website: $checkedConvert('website', (v) => v as String?),
        legalName: $checkedConvert('legalName', (v) => v as String?),
        taxOffice: $checkedConvert('taxOffice', (v) => v as String?),
        taxNumber: $checkedConvert('taxNumber', (v) => v as String?),
      );
      return val;
    });

Map<String, dynamic> _$BrandUpdateToJson(BrandUpdate instance) =>
    <String, dynamic>{
      'name': ?instance.name,
      'industryCode': ?_$IndustryCodeEnumMap[instance.industryCode],
      'shortDescription': ?instance.shortDescription,
      'phone': ?instance.phone,
      'email': ?instance.email,
      'instagram': ?instance.instagram,
      'website': ?instance.website,
      'legalName': ?instance.legalName,
      'taxOffice': ?instance.taxOffice,
      'taxNumber': ?instance.taxNumber,
    };

const _$IndustryCodeEnumMap = {
  IndustryCode.barber: 'barber',
  IndustryCode.hairSalon: 'hair_salon',
  IndustryCode.beauty: 'beauty',
  IndustryCode.spa: 'spa',
  IndustryCode.nail: 'nail',
  IndustryCode.dietitian: 'dietitian',
  IndustryCode.other: 'other',
  IndustryCode.unknownDefaultOpenApi: 'unknown_default_open_api',
};
