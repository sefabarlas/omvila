// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'public_waitlist_offer_brand.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PublicWaitlistOfferBrandCWProxy {
  PublicWaitlistOfferBrand name(String name);

  PublicWaitlistOfferBrand slug(String slug);

  PublicWaitlistOfferBrand logoUrl(String? logoUrl);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicWaitlistOfferBrand(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicWaitlistOfferBrand(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicWaitlistOfferBrand call({String name, String slug, String? logoUrl});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPublicWaitlistOfferBrand.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPublicWaitlistOfferBrand.copyWith.fieldName(...)`
class _$PublicWaitlistOfferBrandCWProxyImpl
    implements _$PublicWaitlistOfferBrandCWProxy {
  const _$PublicWaitlistOfferBrandCWProxyImpl(this._value);

  final PublicWaitlistOfferBrand _value;

  @override
  PublicWaitlistOfferBrand name(String name) => this(name: name);

  @override
  PublicWaitlistOfferBrand slug(String slug) => this(slug: slug);

  @override
  PublicWaitlistOfferBrand logoUrl(String? logoUrl) => this(logoUrl: logoUrl);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicWaitlistOfferBrand(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicWaitlistOfferBrand(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicWaitlistOfferBrand call({
    Object? name = const $CopyWithPlaceholder(),
    Object? slug = const $CopyWithPlaceholder(),
    Object? logoUrl = const $CopyWithPlaceholder(),
  }) {
    return PublicWaitlistOfferBrand(
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

extension $PublicWaitlistOfferBrandCopyWith on PublicWaitlistOfferBrand {
  /// Returns a callable class that can be used as follows: `instanceOfPublicWaitlistOfferBrand.copyWith(...)` or like so:`instanceOfPublicWaitlistOfferBrand.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PublicWaitlistOfferBrandCWProxy get copyWith =>
      _$PublicWaitlistOfferBrandCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PublicWaitlistOfferBrand _$PublicWaitlistOfferBrandFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('PublicWaitlistOfferBrand', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['name', 'slug']);
  final val = PublicWaitlistOfferBrand(
    name: $checkedConvert('name', (v) => v as String),
    slug: $checkedConvert('slug', (v) => v as String),
    logoUrl: $checkedConvert('logoUrl', (v) => v as String?),
  );
  return val;
});

Map<String, dynamic> _$PublicWaitlistOfferBrandToJson(
  PublicWaitlistOfferBrand instance,
) => <String, dynamic>{
  'name': instance.name,
  'slug': instance.slug,
  'logoUrl': ?instance.logoUrl,
};
