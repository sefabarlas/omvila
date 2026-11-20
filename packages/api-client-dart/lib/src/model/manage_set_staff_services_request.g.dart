// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'manage_set_staff_services_request.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ManageSetStaffServicesRequestCWProxy {
  ManageSetStaffServicesRequest items(List<StaffServiceAssignment> items);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageSetStaffServicesRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageSetStaffServicesRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageSetStaffServicesRequest call({List<StaffServiceAssignment> items});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfManageSetStaffServicesRequest.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfManageSetStaffServicesRequest.copyWith.fieldName(...)`
class _$ManageSetStaffServicesRequestCWProxyImpl
    implements _$ManageSetStaffServicesRequestCWProxy {
  const _$ManageSetStaffServicesRequestCWProxyImpl(this._value);

  final ManageSetStaffServicesRequest _value;

  @override
  ManageSetStaffServicesRequest items(List<StaffServiceAssignment> items) =>
      this(items: items);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageSetStaffServicesRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageSetStaffServicesRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageSetStaffServicesRequest call({
    Object? items = const $CopyWithPlaceholder(),
  }) {
    return ManageSetStaffServicesRequest(
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<StaffServiceAssignment>,
    );
  }
}

extension $ManageSetStaffServicesRequestCopyWith
    on ManageSetStaffServicesRequest {
  /// Returns a callable class that can be used as follows: `instanceOfManageSetStaffServicesRequest.copyWith(...)` or like so:`instanceOfManageSetStaffServicesRequest.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ManageSetStaffServicesRequestCWProxy get copyWith =>
      _$ManageSetStaffServicesRequestCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ManageSetStaffServicesRequest _$ManageSetStaffServicesRequestFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('ManageSetStaffServicesRequest', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['items']);
  final val = ManageSetStaffServicesRequest(
    items: $checkedConvert(
      'items',
      (v) => (v as List<dynamic>)
          .map(
            (e) => StaffServiceAssignment.fromJson(e as Map<String, dynamic>),
          )
          .toList(),
    ),
  );
  return val;
});

Map<String, dynamic> _$ManageSetStaffServicesRequestToJson(
  ManageSetStaffServicesRequest instance,
) => <String, dynamic>{'items': instance.items.map((e) => e.toJson()).toList()};
