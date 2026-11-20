// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'manage_create_services_from_templates201_response.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ManageCreateServicesFromTemplates201ResponseCWProxy {
  ManageCreateServicesFromTemplates201Response items(List<Service> items);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageCreateServicesFromTemplates201Response(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageCreateServicesFromTemplates201Response(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageCreateServicesFromTemplates201Response call({List<Service> items});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfManageCreateServicesFromTemplates201Response.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfManageCreateServicesFromTemplates201Response.copyWith.fieldName(...)`
class _$ManageCreateServicesFromTemplates201ResponseCWProxyImpl
    implements _$ManageCreateServicesFromTemplates201ResponseCWProxy {
  const _$ManageCreateServicesFromTemplates201ResponseCWProxyImpl(this._value);

  final ManageCreateServicesFromTemplates201Response _value;

  @override
  ManageCreateServicesFromTemplates201Response items(List<Service> items) =>
      this(items: items);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageCreateServicesFromTemplates201Response(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageCreateServicesFromTemplates201Response(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageCreateServicesFromTemplates201Response call({
    Object? items = const $CopyWithPlaceholder(),
  }) {
    return ManageCreateServicesFromTemplates201Response(
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<Service>,
    );
  }
}

extension $ManageCreateServicesFromTemplates201ResponseCopyWith
    on ManageCreateServicesFromTemplates201Response {
  /// Returns a callable class that can be used as follows: `instanceOfManageCreateServicesFromTemplates201Response.copyWith(...)` or like so:`instanceOfManageCreateServicesFromTemplates201Response.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ManageCreateServicesFromTemplates201ResponseCWProxy get copyWith =>
      _$ManageCreateServicesFromTemplates201ResponseCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ManageCreateServicesFromTemplates201Response
_$ManageCreateServicesFromTemplates201ResponseFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('ManageCreateServicesFromTemplates201Response', json, (
  $checkedConvert,
) {
  $checkKeys(json, requiredKeys: const ['items']);
  final val = ManageCreateServicesFromTemplates201Response(
    items: $checkedConvert(
      'items',
      (v) => (v as List<dynamic>)
          .map((e) => Service.fromJson(e as Map<String, dynamic>))
          .toList(),
    ),
  );
  return val;
});

Map<String, dynamic> _$ManageCreateServicesFromTemplates201ResponseToJson(
  ManageCreateServicesFromTemplates201Response instance,
) => <String, dynamic>{'items': instance.items.map((e) => e.toJson()).toList()};
