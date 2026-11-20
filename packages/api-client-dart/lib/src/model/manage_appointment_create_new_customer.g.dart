// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'manage_appointment_create_new_customer.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ManageAppointmentCreateNewCustomerCWProxy {
  ManageAppointmentCreateNewCustomer fullName(String fullName);

  ManageAppointmentCreateNewCustomer phone(String phone);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageAppointmentCreateNewCustomer(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageAppointmentCreateNewCustomer(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageAppointmentCreateNewCustomer call({String fullName, String phone});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfManageAppointmentCreateNewCustomer.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfManageAppointmentCreateNewCustomer.copyWith.fieldName(...)`
class _$ManageAppointmentCreateNewCustomerCWProxyImpl
    implements _$ManageAppointmentCreateNewCustomerCWProxy {
  const _$ManageAppointmentCreateNewCustomerCWProxyImpl(this._value);

  final ManageAppointmentCreateNewCustomer _value;

  @override
  ManageAppointmentCreateNewCustomer fullName(String fullName) =>
      this(fullName: fullName);

  @override
  ManageAppointmentCreateNewCustomer phone(String phone) => this(phone: phone);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageAppointmentCreateNewCustomer(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageAppointmentCreateNewCustomer(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageAppointmentCreateNewCustomer call({
    Object? fullName = const $CopyWithPlaceholder(),
    Object? phone = const $CopyWithPlaceholder(),
  }) {
    return ManageAppointmentCreateNewCustomer(
      fullName: fullName == const $CopyWithPlaceholder()
          ? _value.fullName
          // ignore: cast_nullable_to_non_nullable
          : fullName as String,
      phone: phone == const $CopyWithPlaceholder()
          ? _value.phone
          // ignore: cast_nullable_to_non_nullable
          : phone as String,
    );
  }
}

extension $ManageAppointmentCreateNewCustomerCopyWith
    on ManageAppointmentCreateNewCustomer {
  /// Returns a callable class that can be used as follows: `instanceOfManageAppointmentCreateNewCustomer.copyWith(...)` or like so:`instanceOfManageAppointmentCreateNewCustomer.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ManageAppointmentCreateNewCustomerCWProxy get copyWith =>
      _$ManageAppointmentCreateNewCustomerCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ManageAppointmentCreateNewCustomer _$ManageAppointmentCreateNewCustomerFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('ManageAppointmentCreateNewCustomer', json, (
  $checkedConvert,
) {
  $checkKeys(json, requiredKeys: const ['fullName', 'phone']);
  final val = ManageAppointmentCreateNewCustomer(
    fullName: $checkedConvert('fullName', (v) => v as String),
    phone: $checkedConvert('phone', (v) => v as String),
  );
  return val;
});

Map<String, dynamic> _$ManageAppointmentCreateNewCustomerToJson(
  ManageAppointmentCreateNewCustomer instance,
) => <String, dynamic>{'fullName': instance.fullName, 'phone': instance.phone};
