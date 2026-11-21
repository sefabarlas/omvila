// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'public_review_invite_brand.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PublicReviewInviteBrandCWProxy {
  PublicReviewInviteBrand name(String name);

  PublicReviewInviteBrand slug(String slug);

  PublicReviewInviteBrand logoUrl(String? logoUrl);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicReviewInviteBrand(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicReviewInviteBrand(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicReviewInviteBrand call({String name, String slug, String? logoUrl});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPublicReviewInviteBrand.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPublicReviewInviteBrand.copyWith.fieldName(...)`
class _$PublicReviewInviteBrandCWProxyImpl
    implements _$PublicReviewInviteBrandCWProxy {
  const _$PublicReviewInviteBrandCWProxyImpl(this._value);

  final PublicReviewInviteBrand _value;

  @override
  PublicReviewInviteBrand name(String name) => this(name: name);

  @override
  PublicReviewInviteBrand slug(String slug) => this(slug: slug);

  @override
  PublicReviewInviteBrand logoUrl(String? logoUrl) => this(logoUrl: logoUrl);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicReviewInviteBrand(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicReviewInviteBrand(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicReviewInviteBrand call({
    Object? name = const $CopyWithPlaceholder(),
    Object? slug = const $CopyWithPlaceholder(),
    Object? logoUrl = const $CopyWithPlaceholder(),
  }) {
    return PublicReviewInviteBrand(
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

extension $PublicReviewInviteBrandCopyWith on PublicReviewInviteBrand {
  /// Returns a callable class that can be used as follows: `instanceOfPublicReviewInviteBrand.copyWith(...)` or like so:`instanceOfPublicReviewInviteBrand.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PublicReviewInviteBrandCWProxy get copyWith =>
      _$PublicReviewInviteBrandCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PublicReviewInviteBrand _$PublicReviewInviteBrandFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('PublicReviewInviteBrand', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['name', 'slug']);
  final val = PublicReviewInviteBrand(
    name: $checkedConvert('name', (v) => v as String),
    slug: $checkedConvert('slug', (v) => v as String),
    logoUrl: $checkedConvert('logoUrl', (v) => v as String?),
  );
  return val;
});

Map<String, dynamic> _$PublicReviewInviteBrandToJson(
  PublicReviewInviteBrand instance,
) => <String, dynamic>{
  'name': instance.name,
  'slug': instance.slug,
  'logoUrl': ?instance.logoUrl,
};
