// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'manage_list_customers200_response.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ManageListCustomers200ResponseCWProxy {
  ManageListCustomers200Response items(List<CustomerSummary> items);

  ManageListCustomers200Response nextCursor(String? nextCursor);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageListCustomers200Response(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageListCustomers200Response(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageListCustomers200Response call({
    List<CustomerSummary> items,
    String? nextCursor,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfManageListCustomers200Response.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfManageListCustomers200Response.copyWith.fieldName(...)`
class _$ManageListCustomers200ResponseCWProxyImpl
    implements _$ManageListCustomers200ResponseCWProxy {
  const _$ManageListCustomers200ResponseCWProxyImpl(this._value);

  final ManageListCustomers200Response _value;

  @override
  ManageListCustomers200Response items(List<CustomerSummary> items) =>
      this(items: items);

  @override
  ManageListCustomers200Response nextCursor(String? nextCursor) =>
      this(nextCursor: nextCursor);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageListCustomers200Response(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageListCustomers200Response(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageListCustomers200Response call({
    Object? items = const $CopyWithPlaceholder(),
    Object? nextCursor = const $CopyWithPlaceholder(),
  }) {
    return ManageListCustomers200Response(
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<CustomerSummary>,
      nextCursor: nextCursor == const $CopyWithPlaceholder()
          ? _value.nextCursor
          // ignore: cast_nullable_to_non_nullable
          : nextCursor as String?,
    );
  }
}

extension $ManageListCustomers200ResponseCopyWith
    on ManageListCustomers200Response {
  /// Returns a callable class that can be used as follows: `instanceOfManageListCustomers200Response.copyWith(...)` or like so:`instanceOfManageListCustomers200Response.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ManageListCustomers200ResponseCWProxy get copyWith =>
      _$ManageListCustomers200ResponseCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ManageListCustomers200Response _$ManageListCustomers200ResponseFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('ManageListCustomers200Response', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['items', 'nextCursor']);
  final val = ManageListCustomers200Response(
    items: $checkedConvert(
      'items',
      (v) => (v as List<dynamic>)
          .map((e) => CustomerSummary.fromJson(e as Map<String, dynamic>))
          .toList(),
    ),
    nextCursor: $checkedConvert('nextCursor', (v) => v as String?),
  );
  return val;
});

Map<String, dynamic> _$ManageListCustomers200ResponseToJson(
  ManageListCustomers200Response instance,
) => <String, dynamic>{
  'items': instance.items.map((e) => e.toJson()).toList(),
  'nextCursor': instance.nextCursor,
};
