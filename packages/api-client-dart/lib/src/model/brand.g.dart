// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'brand.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$BrandCWProxy {
  Brand id(String id);

  Brand name(String name);

  Brand slug(String slug);

  Brand industryCode(IndustryCode industryCode);

  Brand status(BrandStatusEnum status);

  Brand shortDescription(String? shortDescription);

  Brand logoUrl(String? logoUrl);

  Brand phone(String? phone);

  Brand email(String? email);

  Brand instagram(String? instagram);

  Brand website(String? website);

  Brand legalName(String? legalName);

  Brand taxOffice(String? taxOffice);

  Brand taxNumber(String? taxNumber);

  Brand version(String version);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `Brand(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// Brand(...).copyWith(id: 12, name: "My name")
  /// ````
  Brand call({
    String id,
    String name,
    String slug,
    IndustryCode industryCode,
    BrandStatusEnum status,
    String? shortDescription,
    String? logoUrl,
    String? phone,
    String? email,
    String? instagram,
    String? website,
    String? legalName,
    String? taxOffice,
    String? taxNumber,
    String version,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfBrand.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfBrand.copyWith.fieldName(...)`
class _$BrandCWProxyImpl implements _$BrandCWProxy {
  const _$BrandCWProxyImpl(this._value);

  final Brand _value;

  @override
  Brand id(String id) => this(id: id);

  @override
  Brand name(String name) => this(name: name);

  @override
  Brand slug(String slug) => this(slug: slug);

  @override
  Brand industryCode(IndustryCode industryCode) =>
      this(industryCode: industryCode);

  @override
  Brand status(BrandStatusEnum status) => this(status: status);

  @override
  Brand shortDescription(String? shortDescription) =>
      this(shortDescription: shortDescription);

  @override
  Brand logoUrl(String? logoUrl) => this(logoUrl: logoUrl);

  @override
  Brand phone(String? phone) => this(phone: phone);

  @override
  Brand email(String? email) => this(email: email);

  @override
  Brand instagram(String? instagram) => this(instagram: instagram);

  @override
  Brand website(String? website) => this(website: website);

  @override
  Brand legalName(String? legalName) => this(legalName: legalName);

  @override
  Brand taxOffice(String? taxOffice) => this(taxOffice: taxOffice);

  @override
  Brand taxNumber(String? taxNumber) => this(taxNumber: taxNumber);

  @override
  Brand version(String version) => this(version: version);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `Brand(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// Brand(...).copyWith(id: 12, name: "My name")
  /// ````
  Brand call({
    Object? id = const $CopyWithPlaceholder(),
    Object? name = const $CopyWithPlaceholder(),
    Object? slug = const $CopyWithPlaceholder(),
    Object? industryCode = const $CopyWithPlaceholder(),
    Object? status = const $CopyWithPlaceholder(),
    Object? shortDescription = const $CopyWithPlaceholder(),
    Object? logoUrl = const $CopyWithPlaceholder(),
    Object? phone = const $CopyWithPlaceholder(),
    Object? email = const $CopyWithPlaceholder(),
    Object? instagram = const $CopyWithPlaceholder(),
    Object? website = const $CopyWithPlaceholder(),
    Object? legalName = const $CopyWithPlaceholder(),
    Object? taxOffice = const $CopyWithPlaceholder(),
    Object? taxNumber = const $CopyWithPlaceholder(),
    Object? version = const $CopyWithPlaceholder(),
  }) {
    return Brand(
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
      industryCode: industryCode == const $CopyWithPlaceholder()
          ? _value.industryCode
          // ignore: cast_nullable_to_non_nullable
          : industryCode as IndustryCode,
      status: status == const $CopyWithPlaceholder()
          ? _value.status
          // ignore: cast_nullable_to_non_nullable
          : status as BrandStatusEnum,
      shortDescription: shortDescription == const $CopyWithPlaceholder()
          ? _value.shortDescription
          // ignore: cast_nullable_to_non_nullable
          : shortDescription as String?,
      logoUrl: logoUrl == const $CopyWithPlaceholder()
          ? _value.logoUrl
          // ignore: cast_nullable_to_non_nullable
          : logoUrl as String?,
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
      version: version == const $CopyWithPlaceholder()
          ? _value.version
          // ignore: cast_nullable_to_non_nullable
          : version as String,
    );
  }
}

extension $BrandCopyWith on Brand {
  /// Returns a callable class that can be used as follows: `instanceOfBrand.copyWith(...)` or like so:`instanceOfBrand.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$BrandCWProxy get copyWith => _$BrandCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

Brand _$BrandFromJson(Map<String, dynamic> json) =>
    $checkedCreate('Brand', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'id',
          'name',
          'slug',
          'industryCode',
          'status',
          'version',
        ],
      );
      final val = Brand(
        id: $checkedConvert('id', (v) => v as String),
        name: $checkedConvert('name', (v) => v as String),
        slug: $checkedConvert('slug', (v) => v as String),
        industryCode: $checkedConvert(
          'industryCode',
          (v) => $enumDecode(
            _$IndustryCodeEnumMap,
            v,
            unknownValue: IndustryCode.unknownDefaultOpenApi,
          ),
        ),
        status: $checkedConvert(
          'status',
          (v) => $enumDecode(
            _$BrandStatusEnumEnumMap,
            v,
            unknownValue: BrandStatusEnum.unknownDefaultOpenApi,
          ),
        ),
        shortDescription: $checkedConvert(
          'shortDescription',
          (v) => v as String?,
        ),
        logoUrl: $checkedConvert('logoUrl', (v) => v as String?),
        phone: $checkedConvert('phone', (v) => v as String?),
        email: $checkedConvert('email', (v) => v as String?),
        instagram: $checkedConvert('instagram', (v) => v as String?),
        website: $checkedConvert('website', (v) => v as String?),
        legalName: $checkedConvert('legalName', (v) => v as String?),
        taxOffice: $checkedConvert('taxOffice', (v) => v as String?),
        taxNumber: $checkedConvert('taxNumber', (v) => v as String?),
        version: $checkedConvert('version', (v) => v as String),
      );
      return val;
    });

Map<String, dynamic> _$BrandToJson(Brand instance) => <String, dynamic>{
  'id': instance.id,
  'name': instance.name,
  'slug': instance.slug,
  'industryCode': _$IndustryCodeEnumMap[instance.industryCode]!,
  'status': _$BrandStatusEnumEnumMap[instance.status]!,
  'shortDescription': ?instance.shortDescription,
  'logoUrl': ?instance.logoUrl,
  'phone': ?instance.phone,
  'email': ?instance.email,
  'instagram': ?instance.instagram,
  'website': ?instance.website,
  'legalName': ?instance.legalName,
  'taxOffice': ?instance.taxOffice,
  'taxNumber': ?instance.taxNumber,
  'version': instance.version,
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

const _$BrandStatusEnumEnumMap = {
  BrandStatusEnum.onboarding: 'Onboarding',
  BrandStatusEnum.active: 'Active',
  BrandStatusEnum.suspended: 'Suspended',
  BrandStatusEnum.archived: 'Archived',
  BrandStatusEnum.unknownDefaultOpenApi: 'unknown_default_open_api',
};
