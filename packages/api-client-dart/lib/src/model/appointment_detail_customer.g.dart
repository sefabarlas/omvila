// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'appointment_detail_customer.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AppointmentDetailCustomerCWProxy {
  AppointmentDetailCustomer brandCustomerId(String brandCustomerId);

  AppointmentDetailCustomer fullName(String fullName);

  AppointmentDetailCustomer phone(String? phone);

  AppointmentDetailCustomer phoneMasked(String? phoneMasked);

  AppointmentDetailCustomer visitCount(int visitCount);

  AppointmentDetailCustomer lastVisitLabel(String? lastVisitLabel);

  AppointmentDetailCustomer lastVisitOn(DateTime? lastVisitOn);

  AppointmentDetailCustomer isNew(bool? isNew);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AppointmentDetailCustomer(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AppointmentDetailCustomer(...).copyWith(id: 12, name: "My name")
  /// ````
  AppointmentDetailCustomer call({
    String brandCustomerId,
    String fullName,
    String? phone,
    String? phoneMasked,
    int visitCount,
    String? lastVisitLabel,
    DateTime? lastVisitOn,
    bool? isNew,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAppointmentDetailCustomer.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAppointmentDetailCustomer.copyWith.fieldName(...)`
class _$AppointmentDetailCustomerCWProxyImpl
    implements _$AppointmentDetailCustomerCWProxy {
  const _$AppointmentDetailCustomerCWProxyImpl(this._value);

  final AppointmentDetailCustomer _value;

  @override
  AppointmentDetailCustomer brandCustomerId(String brandCustomerId) =>
      this(brandCustomerId: brandCustomerId);

  @override
  AppointmentDetailCustomer fullName(String fullName) =>
      this(fullName: fullName);

  @override
  AppointmentDetailCustomer phone(String? phone) => this(phone: phone);

  @override
  AppointmentDetailCustomer phoneMasked(String? phoneMasked) =>
      this(phoneMasked: phoneMasked);

  @override
  AppointmentDetailCustomer visitCount(int visitCount) =>
      this(visitCount: visitCount);

  @override
  AppointmentDetailCustomer lastVisitLabel(String? lastVisitLabel) =>
      this(lastVisitLabel: lastVisitLabel);

  @override
  AppointmentDetailCustomer lastVisitOn(DateTime? lastVisitOn) =>
      this(lastVisitOn: lastVisitOn);

  @override
  AppointmentDetailCustomer isNew(bool? isNew) => this(isNew: isNew);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AppointmentDetailCustomer(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AppointmentDetailCustomer(...).copyWith(id: 12, name: "My name")
  /// ````
  AppointmentDetailCustomer call({
    Object? brandCustomerId = const $CopyWithPlaceholder(),
    Object? fullName = const $CopyWithPlaceholder(),
    Object? phone = const $CopyWithPlaceholder(),
    Object? phoneMasked = const $CopyWithPlaceholder(),
    Object? visitCount = const $CopyWithPlaceholder(),
    Object? lastVisitLabel = const $CopyWithPlaceholder(),
    Object? lastVisitOn = const $CopyWithPlaceholder(),
    Object? isNew = const $CopyWithPlaceholder(),
  }) {
    return AppointmentDetailCustomer(
      brandCustomerId: brandCustomerId == const $CopyWithPlaceholder()
          ? _value.brandCustomerId
          // ignore: cast_nullable_to_non_nullable
          : brandCustomerId as String,
      fullName: fullName == const $CopyWithPlaceholder()
          ? _value.fullName
          // ignore: cast_nullable_to_non_nullable
          : fullName as String,
      phone: phone == const $CopyWithPlaceholder()
          ? _value.phone
          // ignore: cast_nullable_to_non_nullable
          : phone as String?,
      phoneMasked: phoneMasked == const $CopyWithPlaceholder()
          ? _value.phoneMasked
          // ignore: cast_nullable_to_non_nullable
          : phoneMasked as String?,
      visitCount: visitCount == const $CopyWithPlaceholder()
          ? _value.visitCount
          // ignore: cast_nullable_to_non_nullable
          : visitCount as int,
      lastVisitLabel: lastVisitLabel == const $CopyWithPlaceholder()
          ? _value.lastVisitLabel
          // ignore: cast_nullable_to_non_nullable
          : lastVisitLabel as String?,
      lastVisitOn: lastVisitOn == const $CopyWithPlaceholder()
          ? _value.lastVisitOn
          // ignore: cast_nullable_to_non_nullable
          : lastVisitOn as DateTime?,
      isNew: isNew == const $CopyWithPlaceholder()
          ? _value.isNew
          // ignore: cast_nullable_to_non_nullable
          : isNew as bool?,
    );
  }
}

extension $AppointmentDetailCustomerCopyWith on AppointmentDetailCustomer {
  /// Returns a callable class that can be used as follows: `instanceOfAppointmentDetailCustomer.copyWith(...)` or like so:`instanceOfAppointmentDetailCustomer.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AppointmentDetailCustomerCWProxy get copyWith =>
      _$AppointmentDetailCustomerCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AppointmentDetailCustomer _$AppointmentDetailCustomerFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('AppointmentDetailCustomer', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const ['brandCustomerId', 'fullName', 'visitCount'],
  );
  final val = AppointmentDetailCustomer(
    brandCustomerId: $checkedConvert('brandCustomerId', (v) => v as String),
    fullName: $checkedConvert('fullName', (v) => v as String),
    phone: $checkedConvert('phone', (v) => v as String?),
    phoneMasked: $checkedConvert('phoneMasked', (v) => v as String?),
    visitCount: $checkedConvert('visitCount', (v) => (v as num).toInt()),
    lastVisitLabel: $checkedConvert('lastVisitLabel', (v) => v as String?),
    lastVisitOn: $checkedConvert(
      'lastVisitOn',
      (v) => _$JsonConverterFromJson<String, DateTime>(
        v,
        const DateOnlyConverter().fromJson,
      ),
    ),
    isNew: $checkedConvert('isNew', (v) => v as bool?),
  );
  return val;
});

Map<String, dynamic> _$AppointmentDetailCustomerToJson(
  AppointmentDetailCustomer instance,
) => <String, dynamic>{
  'brandCustomerId': instance.brandCustomerId,
  'fullName': instance.fullName,
  'phone': ?instance.phone,
  'phoneMasked': ?instance.phoneMasked,
  'visitCount': instance.visitCount,
  'lastVisitLabel': ?instance.lastVisitLabel,
  'lastVisitOn': ?_$JsonConverterToJson<String, DateTime>(
    instance.lastVisitOn,
    const DateOnlyConverter().toJson,
  ),
  'isNew': ?instance.isNew,
};

Value? _$JsonConverterFromJson<Json, Value>(
  Object? json,
  Value? Function(Json json) fromJson,
) => json == null ? null : fromJson(json as Json);

Json? _$JsonConverterToJson<Json, Value>(
  Value? value,
  Json? Function(Value value) toJson,
) => value == null ? null : toJson(value);
