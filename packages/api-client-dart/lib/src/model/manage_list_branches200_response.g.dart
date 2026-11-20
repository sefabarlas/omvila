// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'manage_list_branches200_response.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ManageListBranches200ResponseCWProxy {
  ManageListBranches200Response items(List<Branch> items);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageListBranches200Response(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageListBranches200Response(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageListBranches200Response call({List<Branch> items});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfManageListBranches200Response.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfManageListBranches200Response.copyWith.fieldName(...)`
class _$ManageListBranches200ResponseCWProxyImpl
    implements _$ManageListBranches200ResponseCWProxy {
  const _$ManageListBranches200ResponseCWProxyImpl(this._value);

  final ManageListBranches200Response _value;

  @override
  ManageListBranches200Response items(List<Branch> items) => this(items: items);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageListBranches200Response(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageListBranches200Response(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageListBranches200Response call({
    Object? items = const $CopyWithPlaceholder(),
  }) {
    return ManageListBranches200Response(
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<Branch>,
    );
  }
}

extension $ManageListBranches200ResponseCopyWith
    on ManageListBranches200Response {
  /// Returns a callable class that can be used as follows: `instanceOfManageListBranches200Response.copyWith(...)` or like so:`instanceOfManageListBranches200Response.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ManageListBranches200ResponseCWProxy get copyWith =>
      _$ManageListBranches200ResponseCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ManageListBranches200Response _$ManageListBranches200ResponseFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('ManageListBranches200Response', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['items']);
  final val = ManageListBranches200Response(
    items: $checkedConvert(
      'items',
      (v) => (v as List<dynamic>)
          .map((e) => Branch.fromJson(e as Map<String, dynamic>))
          .toList(),
    ),
  );
  return val;
});

Map<String, dynamic> _$ManageListBranches200ResponseToJson(
  ManageListBranches200Response instance,
) => <String, dynamic>{'items': instance.items.map((e) => e.toJson()).toList()};
