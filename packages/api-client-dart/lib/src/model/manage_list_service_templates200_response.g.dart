// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'manage_list_service_templates200_response.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ManageListServiceTemplates200ResponseCWProxy {
  ManageListServiceTemplates200Response items(List<ServiceTemplate> items);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageListServiceTemplates200Response(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageListServiceTemplates200Response(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageListServiceTemplates200Response call({List<ServiceTemplate> items});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfManageListServiceTemplates200Response.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfManageListServiceTemplates200Response.copyWith.fieldName(...)`
class _$ManageListServiceTemplates200ResponseCWProxyImpl
    implements _$ManageListServiceTemplates200ResponseCWProxy {
  const _$ManageListServiceTemplates200ResponseCWProxyImpl(this._value);

  final ManageListServiceTemplates200Response _value;

  @override
  ManageListServiceTemplates200Response items(List<ServiceTemplate> items) =>
      this(items: items);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageListServiceTemplates200Response(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageListServiceTemplates200Response(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageListServiceTemplates200Response call({
    Object? items = const $CopyWithPlaceholder(),
  }) {
    return ManageListServiceTemplates200Response(
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<ServiceTemplate>,
    );
  }
}

extension $ManageListServiceTemplates200ResponseCopyWith
    on ManageListServiceTemplates200Response {
  /// Returns a callable class that can be used as follows: `instanceOfManageListServiceTemplates200Response.copyWith(...)` or like so:`instanceOfManageListServiceTemplates200Response.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ManageListServiceTemplates200ResponseCWProxy get copyWith =>
      _$ManageListServiceTemplates200ResponseCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ManageListServiceTemplates200Response
_$ManageListServiceTemplates200ResponseFromJson(Map<String, dynamic> json) =>
    $checkedCreate('ManageListServiceTemplates200Response', json, (
      $checkedConvert,
    ) {
      $checkKeys(json, requiredKeys: const ['items']);
      final val = ManageListServiceTemplates200Response(
        items: $checkedConvert(
          'items',
          (v) => (v as List<dynamic>)
              .map((e) => ServiceTemplate.fromJson(e as Map<String, dynamic>))
              .toList(),
        ),
      );
      return val;
    });

Map<String, dynamic> _$ManageListServiceTemplates200ResponseToJson(
  ManageListServiceTemplates200Response instance,
) => <String, dynamic>{'items': instance.items.map((e) => e.toJson()).toList()};
