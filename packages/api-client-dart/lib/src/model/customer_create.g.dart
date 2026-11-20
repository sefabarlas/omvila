// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'customer_create.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$CustomerCreateCWProxy {
  CustomerCreate fullName(String fullName);

  CustomerCreate phone(String phone);

  CustomerCreate birthYear(int? birthYear);

  CustomerCreate notes(String? notes);

  CustomerCreate sendAppointmentSms(bool? sendAppointmentSms);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CustomerCreate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CustomerCreate(...).copyWith(id: 12, name: "My name")
  /// ````
  CustomerCreate call({
    String fullName,
    String phone,
    int? birthYear,
    String? notes,
    bool? sendAppointmentSms,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfCustomerCreate.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfCustomerCreate.copyWith.fieldName(...)`
class _$CustomerCreateCWProxyImpl implements _$CustomerCreateCWProxy {
  const _$CustomerCreateCWProxyImpl(this._value);

  final CustomerCreate _value;

  @override
  CustomerCreate fullName(String fullName) => this(fullName: fullName);

  @override
  CustomerCreate phone(String phone) => this(phone: phone);

  @override
  CustomerCreate birthYear(int? birthYear) => this(birthYear: birthYear);

  @override
  CustomerCreate notes(String? notes) => this(notes: notes);

  @override
  CustomerCreate sendAppointmentSms(bool? sendAppointmentSms) =>
      this(sendAppointmentSms: sendAppointmentSms);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CustomerCreate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CustomerCreate(...).copyWith(id: 12, name: "My name")
  /// ````
  CustomerCreate call({
    Object? fullName = const $CopyWithPlaceholder(),
    Object? phone = const $CopyWithPlaceholder(),
    Object? birthYear = const $CopyWithPlaceholder(),
    Object? notes = const $CopyWithPlaceholder(),
    Object? sendAppointmentSms = const $CopyWithPlaceholder(),
  }) {
    return CustomerCreate(
      fullName: fullName == const $CopyWithPlaceholder()
          ? _value.fullName
          // ignore: cast_nullable_to_non_nullable
          : fullName as String,
      phone: phone == const $CopyWithPlaceholder()
          ? _value.phone
          // ignore: cast_nullable_to_non_nullable
          : phone as String,
      birthYear: birthYear == const $CopyWithPlaceholder()
          ? _value.birthYear
          // ignore: cast_nullable_to_non_nullable
          : birthYear as int?,
      notes: notes == const $CopyWithPlaceholder()
          ? _value.notes
          // ignore: cast_nullable_to_non_nullable
          : notes as String?,
      sendAppointmentSms: sendAppointmentSms == const $CopyWithPlaceholder()
          ? _value.sendAppointmentSms
          // ignore: cast_nullable_to_non_nullable
          : sendAppointmentSms as bool?,
    );
  }
}

extension $CustomerCreateCopyWith on CustomerCreate {
  /// Returns a callable class that can be used as follows: `instanceOfCustomerCreate.copyWith(...)` or like so:`instanceOfCustomerCreate.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$CustomerCreateCWProxy get copyWith => _$CustomerCreateCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CustomerCreate _$CustomerCreateFromJson(Map<String, dynamic> json) =>
    $checkedCreate('CustomerCreate', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['fullName', 'phone']);
      final val = CustomerCreate(
        fullName: $checkedConvert('fullName', (v) => v as String),
        phone: $checkedConvert('phone', (v) => v as String),
        birthYear: $checkedConvert('birthYear', (v) => (v as num?)?.toInt()),
        notes: $checkedConvert('notes', (v) => v as String?),
        sendAppointmentSms: $checkedConvert(
          'sendAppointmentSms',
          (v) => v as bool? ?? true,
        ),
      );
      return val;
    });

Map<String, dynamic> _$CustomerCreateToJson(CustomerCreate instance) =>
    <String, dynamic>{
      'fullName': instance.fullName,
      'phone': instance.phone,
      'birthYear': ?instance.birthYear,
      'notes': ?instance.notes,
      'sendAppointmentSms': ?instance.sendAppointmentSms,
    };
