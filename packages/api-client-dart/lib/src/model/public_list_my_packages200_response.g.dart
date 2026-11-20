// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'public_list_my_packages200_response.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PublicListMyPackages200ResponseCWProxy {
  PublicListMyPackages200Response items(List<UsablePackageItem> items);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicListMyPackages200Response(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicListMyPackages200Response(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicListMyPackages200Response call({List<UsablePackageItem> items});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPublicListMyPackages200Response.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPublicListMyPackages200Response.copyWith.fieldName(...)`
class _$PublicListMyPackages200ResponseCWProxyImpl
    implements _$PublicListMyPackages200ResponseCWProxy {
  const _$PublicListMyPackages200ResponseCWProxyImpl(this._value);

  final PublicListMyPackages200Response _value;

  @override
  PublicListMyPackages200Response items(List<UsablePackageItem> items) =>
      this(items: items);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicListMyPackages200Response(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicListMyPackages200Response(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicListMyPackages200Response call({
    Object? items = const $CopyWithPlaceholder(),
  }) {
    return PublicListMyPackages200Response(
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<UsablePackageItem>,
    );
  }
}

extension $PublicListMyPackages200ResponseCopyWith
    on PublicListMyPackages200Response {
  /// Returns a callable class that can be used as follows: `instanceOfPublicListMyPackages200Response.copyWith(...)` or like so:`instanceOfPublicListMyPackages200Response.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PublicListMyPackages200ResponseCWProxy get copyWith =>
      _$PublicListMyPackages200ResponseCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PublicListMyPackages200Response _$PublicListMyPackages200ResponseFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('PublicListMyPackages200Response', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['items']);
  final val = PublicListMyPackages200Response(
    items: $checkedConvert(
      'items',
      (v) => (v as List<dynamic>)
          .map((e) => UsablePackageItem.fromJson(e as Map<String, dynamic>))
          .toList(),
    ),
  );
  return val;
});

Map<String, dynamic> _$PublicListMyPackages200ResponseToJson(
  PublicListMyPackages200Response instance,
) => <String, dynamic>{'items': instance.items.map((e) => e.toJson()).toList()};
