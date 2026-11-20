// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'manage_list_staff200_response.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ManageListStaff200ResponseCWProxy {
  ManageListStaff200Response items(List<StaffSummary> items);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageListStaff200Response(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageListStaff200Response(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageListStaff200Response call({List<StaffSummary> items});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfManageListStaff200Response.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfManageListStaff200Response.copyWith.fieldName(...)`
class _$ManageListStaff200ResponseCWProxyImpl
    implements _$ManageListStaff200ResponseCWProxy {
  const _$ManageListStaff200ResponseCWProxyImpl(this._value);

  final ManageListStaff200Response _value;

  @override
  ManageListStaff200Response items(List<StaffSummary> items) =>
      this(items: items);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageListStaff200Response(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageListStaff200Response(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageListStaff200Response call({
    Object? items = const $CopyWithPlaceholder(),
  }) {
    return ManageListStaff200Response(
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<StaffSummary>,
    );
  }
}

extension $ManageListStaff200ResponseCopyWith on ManageListStaff200Response {
  /// Returns a callable class that can be used as follows: `instanceOfManageListStaff200Response.copyWith(...)` or like so:`instanceOfManageListStaff200Response.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ManageListStaff200ResponseCWProxy get copyWith =>
      _$ManageListStaff200ResponseCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ManageListStaff200Response _$ManageListStaff200ResponseFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('ManageListStaff200Response', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['items']);
  final val = ManageListStaff200Response(
    items: $checkedConvert(
      'items',
      (v) => (v as List<dynamic>)
          .map((e) => StaffSummary.fromJson(e as Map<String, dynamic>))
          .toList(),
    ),
  );
  return val;
});

Map<String, dynamic> _$ManageListStaff200ResponseToJson(
  ManageListStaff200Response instance,
) => <String, dynamic>{'items': instance.items.map((e) => e.toJson()).toList()};
