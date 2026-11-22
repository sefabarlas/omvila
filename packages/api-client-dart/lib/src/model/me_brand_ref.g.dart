// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'me_brand_ref.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$MeBrandRefCWProxy {
  MeBrandRef id(String id);

  MeBrandRef name(String name);

  MeBrandRef slug(String slug);

  MeBrandRef logoUrl(String? logoUrl);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeBrandRef(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeBrandRef(...).copyWith(id: 12, name: "My name")
  /// ````
  MeBrandRef call({String id, String name, String slug, String? logoUrl});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfMeBrandRef.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfMeBrandRef.copyWith.fieldName(...)`
class _$MeBrandRefCWProxyImpl implements _$MeBrandRefCWProxy {
  const _$MeBrandRefCWProxyImpl(this._value);

  final MeBrandRef _value;

  @override
  MeBrandRef id(String id) => this(id: id);

  @override
  MeBrandRef name(String name) => this(name: name);

  @override
  MeBrandRef slug(String slug) => this(slug: slug);

  @override
  MeBrandRef logoUrl(String? logoUrl) => this(logoUrl: logoUrl);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeBrandRef(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeBrandRef(...).copyWith(id: 12, name: "My name")
  /// ````
  MeBrandRef call({
    Object? id = const $CopyWithPlaceholder(),
    Object? name = const $CopyWithPlaceholder(),
    Object? slug = const $CopyWithPlaceholder(),
    Object? logoUrl = const $CopyWithPlaceholder(),
  }) {
    return MeBrandRef(
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
    );
  }
}

extension $MeBrandRefCopyWith on MeBrandRef {
  /// Returns a callable class that can be used as follows: `instanceOfMeBrandRef.copyWith(...)` or like so:`instanceOfMeBrandRef.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$MeBrandRefCWProxy get copyWith => _$MeBrandRefCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MeBrandRef _$MeBrandRefFromJson(Map<String, dynamic> json) =>
    $checkedCreate('MeBrandRef', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['id', 'name', 'slug']);
      final val = MeBrandRef(
        id: $checkedConvert('id', (v) => v as String),
        name: $checkedConvert('name', (v) => v as String),
        slug: $checkedConvert('slug', (v) => v as String),
        logoUrl: $checkedConvert('logoUrl', (v) => v as String?),
      );
      return val;
    });

Map<String, dynamic> _$MeBrandRefToJson(MeBrandRef instance) =>
    <String, dynamic>{
      'id': instance.id,
      'name': instance.name,
      'slug': instance.slug,
      'logoUrl': ?instance.logoUrl,
    };
