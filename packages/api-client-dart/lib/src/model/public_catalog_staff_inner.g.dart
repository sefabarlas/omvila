// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'public_catalog_staff_inner.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PublicCatalogStaffInnerCWProxy {
  PublicCatalogStaffInner id(String id);

  PublicCatalogStaffInner displayName(String displayName);

  PublicCatalogStaffInner title(String? title);

  PublicCatalogStaffInner photoUrl(String? photoUrl);

  PublicCatalogStaffInner surcharges(
    List<PublicCatalogStaffInnerSurchargesInner>? surcharges,
  );

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicCatalogStaffInner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicCatalogStaffInner(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicCatalogStaffInner call({
    String id,
    String displayName,
    String? title,
    String? photoUrl,
    List<PublicCatalogStaffInnerSurchargesInner>? surcharges,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPublicCatalogStaffInner.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPublicCatalogStaffInner.copyWith.fieldName(...)`
class _$PublicCatalogStaffInnerCWProxyImpl
    implements _$PublicCatalogStaffInnerCWProxy {
  const _$PublicCatalogStaffInnerCWProxyImpl(this._value);

  final PublicCatalogStaffInner _value;

  @override
  PublicCatalogStaffInner id(String id) => this(id: id);

  @override
  PublicCatalogStaffInner displayName(String displayName) =>
      this(displayName: displayName);

  @override
  PublicCatalogStaffInner title(String? title) => this(title: title);

  @override
  PublicCatalogStaffInner photoUrl(String? photoUrl) =>
      this(photoUrl: photoUrl);

  @override
  PublicCatalogStaffInner surcharges(
    List<PublicCatalogStaffInnerSurchargesInner>? surcharges,
  ) => this(surcharges: surcharges);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicCatalogStaffInner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicCatalogStaffInner(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicCatalogStaffInner call({
    Object? id = const $CopyWithPlaceholder(),
    Object? displayName = const $CopyWithPlaceholder(),
    Object? title = const $CopyWithPlaceholder(),
    Object? photoUrl = const $CopyWithPlaceholder(),
    Object? surcharges = const $CopyWithPlaceholder(),
  }) {
    return PublicCatalogStaffInner(
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as String,
      displayName: displayName == const $CopyWithPlaceholder()
          ? _value.displayName
          // ignore: cast_nullable_to_non_nullable
          : displayName as String,
      title: title == const $CopyWithPlaceholder()
          ? _value.title
          // ignore: cast_nullable_to_non_nullable
          : title as String?,
      photoUrl: photoUrl == const $CopyWithPlaceholder()
          ? _value.photoUrl
          // ignore: cast_nullable_to_non_nullable
          : photoUrl as String?,
      surcharges: surcharges == const $CopyWithPlaceholder()
          ? _value.surcharges
          // ignore: cast_nullable_to_non_nullable
          : surcharges as List<PublicCatalogStaffInnerSurchargesInner>?,
    );
  }
}

extension $PublicCatalogStaffInnerCopyWith on PublicCatalogStaffInner {
  /// Returns a callable class that can be used as follows: `instanceOfPublicCatalogStaffInner.copyWith(...)` or like so:`instanceOfPublicCatalogStaffInner.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PublicCatalogStaffInnerCWProxy get copyWith =>
      _$PublicCatalogStaffInnerCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PublicCatalogStaffInner _$PublicCatalogStaffInnerFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('PublicCatalogStaffInner', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['id', 'displayName']);
  final val = PublicCatalogStaffInner(
    id: $checkedConvert('id', (v) => v as String),
    displayName: $checkedConvert('displayName', (v) => v as String),
    title: $checkedConvert('title', (v) => v as String?),
    photoUrl: $checkedConvert('photoUrl', (v) => v as String?),
    surcharges: $checkedConvert(
      'surcharges',
      (v) => (v as List<dynamic>?)
          ?.map(
            (e) => PublicCatalogStaffInnerSurchargesInner.fromJson(
              e as Map<String, dynamic>,
            ),
          )
          .toList(),
    ),
  );
  return val;
});

Map<String, dynamic> _$PublicCatalogStaffInnerToJson(
  PublicCatalogStaffInner instance,
) => <String, dynamic>{
  'id': instance.id,
  'displayName': instance.displayName,
  'title': ?instance.title,
  'photoUrl': ?instance.photoUrl,
  'surcharges': ?instance.surcharges?.map((e) => e.toJson()).toList(),
};
