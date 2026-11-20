// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'manage_list_customer_appointments200_response.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ManageListCustomerAppointments200ResponseCWProxy {
  ManageListCustomerAppointments200Response items(
    List<CustomerAppointmentRow> items,
  );

  ManageListCustomerAppointments200Response nextCursor(String? nextCursor);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageListCustomerAppointments200Response(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageListCustomerAppointments200Response(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageListCustomerAppointments200Response call({
    List<CustomerAppointmentRow> items,
    String? nextCursor,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfManageListCustomerAppointments200Response.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfManageListCustomerAppointments200Response.copyWith.fieldName(...)`
class _$ManageListCustomerAppointments200ResponseCWProxyImpl
    implements _$ManageListCustomerAppointments200ResponseCWProxy {
  const _$ManageListCustomerAppointments200ResponseCWProxyImpl(this._value);

  final ManageListCustomerAppointments200Response _value;

  @override
  ManageListCustomerAppointments200Response items(
    List<CustomerAppointmentRow> items,
  ) => this(items: items);

  @override
  ManageListCustomerAppointments200Response nextCursor(String? nextCursor) =>
      this(nextCursor: nextCursor);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageListCustomerAppointments200Response(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageListCustomerAppointments200Response(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageListCustomerAppointments200Response call({
    Object? items = const $CopyWithPlaceholder(),
    Object? nextCursor = const $CopyWithPlaceholder(),
  }) {
    return ManageListCustomerAppointments200Response(
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<CustomerAppointmentRow>,
      nextCursor: nextCursor == const $CopyWithPlaceholder()
          ? _value.nextCursor
          // ignore: cast_nullable_to_non_nullable
          : nextCursor as String?,
    );
  }
}

extension $ManageListCustomerAppointments200ResponseCopyWith
    on ManageListCustomerAppointments200Response {
  /// Returns a callable class that can be used as follows: `instanceOfManageListCustomerAppointments200Response.copyWith(...)` or like so:`instanceOfManageListCustomerAppointments200Response.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ManageListCustomerAppointments200ResponseCWProxy get copyWith =>
      _$ManageListCustomerAppointments200ResponseCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ManageListCustomerAppointments200Response
_$ManageListCustomerAppointments200ResponseFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('ManageListCustomerAppointments200Response', json, (
  $checkedConvert,
) {
  $checkKeys(json, requiredKeys: const ['items', 'nextCursor']);
  final val = ManageListCustomerAppointments200Response(
    items: $checkedConvert(
      'items',
      (v) => (v as List<dynamic>)
          .map(
            (e) => CustomerAppointmentRow.fromJson(e as Map<String, dynamic>),
          )
          .toList(),
    ),
    nextCursor: $checkedConvert('nextCursor', (v) => v as String?),
  );
  return val;
});

Map<String, dynamic> _$ManageListCustomerAppointments200ResponseToJson(
  ManageListCustomerAppointments200Response instance,
) => <String, dynamic>{
  'items': instance.items.map((e) => e.toJson()).toList(),
  'nextCursor': instance.nextCursor,
};
