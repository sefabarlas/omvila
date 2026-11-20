// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'public_appointment_brand.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PublicAppointmentBrandCWProxy {
  PublicAppointmentBrand name(String name);

  PublicAppointmentBrand slug(String slug);

  PublicAppointmentBrand logoUrl(String? logoUrl);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicAppointmentBrand(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicAppointmentBrand(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicAppointmentBrand call({String name, String slug, String? logoUrl});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPublicAppointmentBrand.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPublicAppointmentBrand.copyWith.fieldName(...)`
class _$PublicAppointmentBrandCWProxyImpl
    implements _$PublicAppointmentBrandCWProxy {
  const _$PublicAppointmentBrandCWProxyImpl(this._value);

  final PublicAppointmentBrand _value;

  @override
  PublicAppointmentBrand name(String name) => this(name: name);

  @override
  PublicAppointmentBrand slug(String slug) => this(slug: slug);

  @override
  PublicAppointmentBrand logoUrl(String? logoUrl) => this(logoUrl: logoUrl);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicAppointmentBrand(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicAppointmentBrand(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicAppointmentBrand call({
    Object? name = const $CopyWithPlaceholder(),
    Object? slug = const $CopyWithPlaceholder(),
    Object? logoUrl = const $CopyWithPlaceholder(),
  }) {
    return PublicAppointmentBrand(
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
    );
  }
}

extension $PublicAppointmentBrandCopyWith on PublicAppointmentBrand {
  /// Returns a callable class that can be used as follows: `instanceOfPublicAppointmentBrand.copyWith(...)` or like so:`instanceOfPublicAppointmentBrand.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PublicAppointmentBrandCWProxy get copyWith =>
      _$PublicAppointmentBrandCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PublicAppointmentBrand _$PublicAppointmentBrandFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('PublicAppointmentBrand', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['name', 'slug']);
  final val = PublicAppointmentBrand(
    name: $checkedConvert('name', (v) => v as String),
    slug: $checkedConvert('slug', (v) => v as String),
    logoUrl: $checkedConvert('logoUrl', (v) => v as String?),
  );
  return val;
});

Map<String, dynamic> _$PublicAppointmentBrandToJson(
  PublicAppointmentBrand instance,
) => <String, dynamic>{
  'name': instance.name,
  'slug': instance.slug,
  'logoUrl': ?instance.logoUrl,
};
