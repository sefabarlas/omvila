// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'manage_list_time_off200_response.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ManageListTimeOff200ResponseCWProxy {
  ManageListTimeOff200Response items(List<TimeOff> items);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageListTimeOff200Response(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageListTimeOff200Response(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageListTimeOff200Response call({List<TimeOff> items});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfManageListTimeOff200Response.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfManageListTimeOff200Response.copyWith.fieldName(...)`
class _$ManageListTimeOff200ResponseCWProxyImpl
    implements _$ManageListTimeOff200ResponseCWProxy {
  const _$ManageListTimeOff200ResponseCWProxyImpl(this._value);

  final ManageListTimeOff200Response _value;

  @override
  ManageListTimeOff200Response items(List<TimeOff> items) => this(items: items);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageListTimeOff200Response(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageListTimeOff200Response(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageListTimeOff200Response call({
    Object? items = const $CopyWithPlaceholder(),
  }) {
    return ManageListTimeOff200Response(
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<TimeOff>,
    );
  }
}

extension $ManageListTimeOff200ResponseCopyWith
    on ManageListTimeOff200Response {
  /// Returns a callable class that can be used as follows: `instanceOfManageListTimeOff200Response.copyWith(...)` or like so:`instanceOfManageListTimeOff200Response.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ManageListTimeOff200ResponseCWProxy get copyWith =>
      _$ManageListTimeOff200ResponseCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ManageListTimeOff200Response _$ManageListTimeOff200ResponseFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('ManageListTimeOff200Response', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['items']);
  final val = ManageListTimeOff200Response(
    items: $checkedConvert(
      'items',
      (v) => (v as List<dynamic>)
          .map((e) => TimeOff.fromJson(e as Map<String, dynamic>))
          .toList(),
    ),
  );
  return val;
});

Map<String, dynamic> _$ManageListTimeOff200ResponseToJson(
  ManageListTimeOff200Response instance,
) => <String, dynamic>{'items': instance.items.map((e) => e.toJson()).toList()};
