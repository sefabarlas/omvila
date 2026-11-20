// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'manage_list_branch_overrides200_response.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ManageListBranchOverrides200ResponseCWProxy {
  ManageListBranchOverrides200Response items(List<WorkingHoursOverride> items);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageListBranchOverrides200Response(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageListBranchOverrides200Response(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageListBranchOverrides200Response call({List<WorkingHoursOverride> items});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfManageListBranchOverrides200Response.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfManageListBranchOverrides200Response.copyWith.fieldName(...)`
class _$ManageListBranchOverrides200ResponseCWProxyImpl
    implements _$ManageListBranchOverrides200ResponseCWProxy {
  const _$ManageListBranchOverrides200ResponseCWProxyImpl(this._value);

  final ManageListBranchOverrides200Response _value;

  @override
  ManageListBranchOverrides200Response items(
    List<WorkingHoursOverride> items,
  ) => this(items: items);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageListBranchOverrides200Response(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageListBranchOverrides200Response(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageListBranchOverrides200Response call({
    Object? items = const $CopyWithPlaceholder(),
  }) {
    return ManageListBranchOverrides200Response(
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<WorkingHoursOverride>,
    );
  }
}

extension $ManageListBranchOverrides200ResponseCopyWith
    on ManageListBranchOverrides200Response {
  /// Returns a callable class that can be used as follows: `instanceOfManageListBranchOverrides200Response.copyWith(...)` or like so:`instanceOfManageListBranchOverrides200Response.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ManageListBranchOverrides200ResponseCWProxy get copyWith =>
      _$ManageListBranchOverrides200ResponseCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ManageListBranchOverrides200Response
_$ManageListBranchOverrides200ResponseFromJson(Map<String, dynamic> json) =>
    $checkedCreate('ManageListBranchOverrides200Response', json, (
      $checkedConvert,
    ) {
      $checkKeys(json, requiredKeys: const ['items']);
      final val = ManageListBranchOverrides200Response(
        items: $checkedConvert(
          'items',
          (v) => (v as List<dynamic>)
              .map(
                (e) => WorkingHoursOverride.fromJson(e as Map<String, dynamic>),
              )
              .toList(),
        ),
      );
      return val;
    });

Map<String, dynamic> _$ManageListBranchOverrides200ResponseToJson(
  ManageListBranchOverrides200Response instance,
) => <String, dynamic>{'items': instance.items.map((e) => e.toJson()).toList()};
