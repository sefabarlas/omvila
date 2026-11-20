// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'manage_reschedule_request_items_inner.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ManageRescheduleRequestItemsInnerCWProxy {
  ManageRescheduleRequestItemsInner itemId(String itemId);

  ManageRescheduleRequestItemsInner staffId(String staffId);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageRescheduleRequestItemsInner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageRescheduleRequestItemsInner(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageRescheduleRequestItemsInner call({String itemId, String staffId});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfManageRescheduleRequestItemsInner.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfManageRescheduleRequestItemsInner.copyWith.fieldName(...)`
class _$ManageRescheduleRequestItemsInnerCWProxyImpl
    implements _$ManageRescheduleRequestItemsInnerCWProxy {
  const _$ManageRescheduleRequestItemsInnerCWProxyImpl(this._value);

  final ManageRescheduleRequestItemsInner _value;

  @override
  ManageRescheduleRequestItemsInner itemId(String itemId) =>
      this(itemId: itemId);

  @override
  ManageRescheduleRequestItemsInner staffId(String staffId) =>
      this(staffId: staffId);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageRescheduleRequestItemsInner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageRescheduleRequestItemsInner(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageRescheduleRequestItemsInner call({
    Object? itemId = const $CopyWithPlaceholder(),
    Object? staffId = const $CopyWithPlaceholder(),
  }) {
    return ManageRescheduleRequestItemsInner(
      itemId: itemId == const $CopyWithPlaceholder()
          ? _value.itemId
          // ignore: cast_nullable_to_non_nullable
          : itemId as String,
      staffId: staffId == const $CopyWithPlaceholder()
          ? _value.staffId
          // ignore: cast_nullable_to_non_nullable
          : staffId as String,
    );
  }
}

extension $ManageRescheduleRequestItemsInnerCopyWith
    on ManageRescheduleRequestItemsInner {
  /// Returns a callable class that can be used as follows: `instanceOfManageRescheduleRequestItemsInner.copyWith(...)` or like so:`instanceOfManageRescheduleRequestItemsInner.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ManageRescheduleRequestItemsInnerCWProxy get copyWith =>
      _$ManageRescheduleRequestItemsInnerCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ManageRescheduleRequestItemsInner _$ManageRescheduleRequestItemsInnerFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('ManageRescheduleRequestItemsInner', json, (
  $checkedConvert,
) {
  $checkKeys(json, requiredKeys: const ['itemId', 'staffId']);
  final val = ManageRescheduleRequestItemsInner(
    itemId: $checkedConvert('itemId', (v) => v as String),
    staffId: $checkedConvert('staffId', (v) => v as String),
  );
  return val;
});

Map<String, dynamic> _$ManageRescheduleRequestItemsInnerToJson(
  ManageRescheduleRequestItemsInner instance,
) => <String, dynamic>{'itemId': instance.itemId, 'staffId': instance.staffId};
