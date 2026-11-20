// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'manage_session_brand.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ManageSessionBrandCWProxy {
  ManageSessionBrand id(String id);

  ManageSessionBrand name(String name);

  ManageSessionBrand slug(String slug);

  ManageSessionBrand logoUrl(String? logoUrl);

  ManageSessionBrand status(ManageSessionBrandStatusEnum status);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageSessionBrand(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageSessionBrand(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageSessionBrand call({
    String id,
    String name,
    String slug,
    String? logoUrl,
    ManageSessionBrandStatusEnum status,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfManageSessionBrand.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfManageSessionBrand.copyWith.fieldName(...)`
class _$ManageSessionBrandCWProxyImpl implements _$ManageSessionBrandCWProxy {
  const _$ManageSessionBrandCWProxyImpl(this._value);

  final ManageSessionBrand _value;

  @override
  ManageSessionBrand id(String id) => this(id: id);

  @override
  ManageSessionBrand name(String name) => this(name: name);

  @override
  ManageSessionBrand slug(String slug) => this(slug: slug);

  @override
  ManageSessionBrand logoUrl(String? logoUrl) => this(logoUrl: logoUrl);

  @override
  ManageSessionBrand status(ManageSessionBrandStatusEnum status) =>
      this(status: status);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageSessionBrand(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageSessionBrand(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageSessionBrand call({
    Object? id = const $CopyWithPlaceholder(),
    Object? name = const $CopyWithPlaceholder(),
    Object? slug = const $CopyWithPlaceholder(),
    Object? logoUrl = const $CopyWithPlaceholder(),
    Object? status = const $CopyWithPlaceholder(),
  }) {
    return ManageSessionBrand(
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
      logoUrl: logoUrl == const $CopyWithPlaceholder()
          ? _value.logoUrl
          // ignore: cast_nullable_to_non_nullable
          : logoUrl as String?,
      status: status == const $CopyWithPlaceholder()
          ? _value.status
          // ignore: cast_nullable_to_non_nullable
          : status as ManageSessionBrandStatusEnum,
    );
  }
}

extension $ManageSessionBrandCopyWith on ManageSessionBrand {
  /// Returns a callable class that can be used as follows: `instanceOfManageSessionBrand.copyWith(...)` or like so:`instanceOfManageSessionBrand.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ManageSessionBrandCWProxy get copyWith =>
      _$ManageSessionBrandCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ManageSessionBrand _$ManageSessionBrandFromJson(Map<String, dynamic> json) =>
    $checkedCreate('ManageSessionBrand', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['id', 'name', 'slug', 'status']);
      final val = ManageSessionBrand(
        id: $checkedConvert('id', (v) => v as String),
        name: $checkedConvert('name', (v) => v as String),
        slug: $checkedConvert('slug', (v) => v as String),
        logoUrl: $checkedConvert('logoUrl', (v) => v as String?),
        status: $checkedConvert(
          'status',
          (v) => $enumDecode(
            _$ManageSessionBrandStatusEnumEnumMap,
            v,
            unknownValue: ManageSessionBrandStatusEnum.unknownDefaultOpenApi,
          ),
        ),
      );
      return val;
    });

Map<String, dynamic> _$ManageSessionBrandToJson(ManageSessionBrand instance) =>
    <String, dynamic>{
      'id': instance.id,
      'name': instance.name,
      'slug': instance.slug,
      'logoUrl': ?instance.logoUrl,
      'status': _$ManageSessionBrandStatusEnumEnumMap[instance.status]!,
    };

const _$ManageSessionBrandStatusEnumEnumMap = {
  ManageSessionBrandStatusEnum.onboarding: 'Onboarding',
  ManageSessionBrandStatusEnum.active: 'Active',
  ManageSessionBrandStatusEnum.suspended: 'Suspended',
  ManageSessionBrandStatusEnum.archived: 'Archived',
  ManageSessionBrandStatusEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};
