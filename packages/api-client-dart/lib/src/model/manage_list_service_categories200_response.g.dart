// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'manage_list_service_categories200_response.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ManageListServiceCategories200ResponseCWProxy {
  ManageListServiceCategories200Response items(List<ServiceCategory> items);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageListServiceCategories200Response(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageListServiceCategories200Response(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageListServiceCategories200Response call({List<ServiceCategory> items});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfManageListServiceCategories200Response.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfManageListServiceCategories200Response.copyWith.fieldName(...)`
class _$ManageListServiceCategories200ResponseCWProxyImpl
    implements _$ManageListServiceCategories200ResponseCWProxy {
  const _$ManageListServiceCategories200ResponseCWProxyImpl(this._value);

  final ManageListServiceCategories200Response _value;

  @override
  ManageListServiceCategories200Response items(List<ServiceCategory> items) =>
      this(items: items);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageListServiceCategories200Response(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageListServiceCategories200Response(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageListServiceCategories200Response call({
    Object? items = const $CopyWithPlaceholder(),
  }) {
    return ManageListServiceCategories200Response(
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<ServiceCategory>,
    );
  }
}

extension $ManageListServiceCategories200ResponseCopyWith
    on ManageListServiceCategories200Response {
  /// Returns a callable class that can be used as follows: `instanceOfManageListServiceCategories200Response.copyWith(...)` or like so:`instanceOfManageListServiceCategories200Response.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ManageListServiceCategories200ResponseCWProxy get copyWith =>
      _$ManageListServiceCategories200ResponseCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ManageListServiceCategories200Response
_$ManageListServiceCategories200ResponseFromJson(Map<String, dynamic> json) =>
    $checkedCreate('ManageListServiceCategories200Response', json, (
      $checkedConvert,
    ) {
      $checkKeys(json, requiredKeys: const ['items']);
      final val = ManageListServiceCategories200Response(
        items: $checkedConvert(
          'items',
          (v) => (v as List<dynamic>)
              .map((e) => ServiceCategory.fromJson(e as Map<String, dynamic>))
              .toList(),
        ),
      );
      return val;
    });

Map<String, dynamic> _$ManageListServiceCategories200ResponseToJson(
  ManageListServiceCategories200Response instance,
) => <String, dynamic>{'items': instance.items.map((e) => e.toJson()).toList()};
