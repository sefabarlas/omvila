// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'me_appointment_item.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$MeAppointmentItemCWProxy {
  MeAppointmentItem serviceId(String serviceId);

  MeAppointmentItem serviceName(String serviceName);

  MeAppointmentItem staffId(String? staffId);

  MeAppointmentItem staffName(String? staffName);

  MeAppointmentItem startUtc(DateTime startUtc);

  MeAppointmentItem endUtc(DateTime endUtc);

  MeAppointmentItem price(Money price);

  MeAppointmentItem paidFromPackage(bool paidFromPackage);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeAppointmentItem(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeAppointmentItem(...).copyWith(id: 12, name: "My name")
  /// ````
  MeAppointmentItem call({
    String serviceId,
    String serviceName,
    String? staffId,
    String? staffName,
    DateTime startUtc,
    DateTime endUtc,
    Money price,
    bool paidFromPackage,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfMeAppointmentItem.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfMeAppointmentItem.copyWith.fieldName(...)`
class _$MeAppointmentItemCWProxyImpl implements _$MeAppointmentItemCWProxy {
  const _$MeAppointmentItemCWProxyImpl(this._value);

  final MeAppointmentItem _value;

  @override
  MeAppointmentItem serviceId(String serviceId) => this(serviceId: serviceId);

  @override
  MeAppointmentItem serviceName(String serviceName) =>
      this(serviceName: serviceName);

  @override
  MeAppointmentItem staffId(String? staffId) => this(staffId: staffId);

  @override
  MeAppointmentItem staffName(String? staffName) => this(staffName: staffName);

  @override
  MeAppointmentItem startUtc(DateTime startUtc) => this(startUtc: startUtc);

  @override
  MeAppointmentItem endUtc(DateTime endUtc) => this(endUtc: endUtc);

  @override
  MeAppointmentItem price(Money price) => this(price: price);

  @override
  MeAppointmentItem paidFromPackage(bool paidFromPackage) =>
      this(paidFromPackage: paidFromPackage);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeAppointmentItem(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeAppointmentItem(...).copyWith(id: 12, name: "My name")
  /// ````
  MeAppointmentItem call({
    Object? serviceId = const $CopyWithPlaceholder(),
    Object? serviceName = const $CopyWithPlaceholder(),
    Object? staffId = const $CopyWithPlaceholder(),
    Object? staffName = const $CopyWithPlaceholder(),
    Object? startUtc = const $CopyWithPlaceholder(),
    Object? endUtc = const $CopyWithPlaceholder(),
    Object? price = const $CopyWithPlaceholder(),
    Object? paidFromPackage = const $CopyWithPlaceholder(),
  }) {
    return MeAppointmentItem(
      serviceId: serviceId == const $CopyWithPlaceholder()
          ? _value.serviceId
          // ignore: cast_nullable_to_non_nullable
          : serviceId as String,
      serviceName: serviceName == const $CopyWithPlaceholder()
          ? _value.serviceName
          // ignore: cast_nullable_to_non_nullable
          : serviceName as String,
      staffId: staffId == const $CopyWithPlaceholder()
          ? _value.staffId
          // ignore: cast_nullable_to_non_nullable
          : staffId as String?,
      staffName: staffName == const $CopyWithPlaceholder()
          ? _value.staffName
          // ignore: cast_nullable_to_non_nullable
          : staffName as String?,
      startUtc: startUtc == const $CopyWithPlaceholder()
          ? _value.startUtc
          // ignore: cast_nullable_to_non_nullable
          : startUtc as DateTime,
      endUtc: endUtc == const $CopyWithPlaceholder()
          ? _value.endUtc
          // ignore: cast_nullable_to_non_nullable
          : endUtc as DateTime,
      price: price == const $CopyWithPlaceholder()
          ? _value.price
          // ignore: cast_nullable_to_non_nullable
          : price as Money,
      paidFromPackage: paidFromPackage == const $CopyWithPlaceholder()
          ? _value.paidFromPackage
          // ignore: cast_nullable_to_non_nullable
          : paidFromPackage as bool,
    );
  }
}

extension $MeAppointmentItemCopyWith on MeAppointmentItem {
  /// Returns a callable class that can be used as follows: `instanceOfMeAppointmentItem.copyWith(...)` or like so:`instanceOfMeAppointmentItem.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$MeAppointmentItemCWProxy get copyWith =>
      _$MeAppointmentItemCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MeAppointmentItem _$MeAppointmentItemFromJson(Map<String, dynamic> json) =>
    $checkedCreate('MeAppointmentItem', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'serviceId',
          'serviceName',
          'startUtc',
          'endUtc',
          'price',
          'paidFromPackage',
        ],
      );
      final val = MeAppointmentItem(
        serviceId: $checkedConvert('serviceId', (v) => v as String),
        serviceName: $checkedConvert('serviceName', (v) => v as String),
        staffId: $checkedConvert('staffId', (v) => v as String?),
        staffName: $checkedConvert('staffName', (v) => v as String?),
        startUtc: $checkedConvert(
          'startUtc',
          (v) => DateTime.parse(v as String),
        ),
        endUtc: $checkedConvert('endUtc', (v) => DateTime.parse(v as String)),
        price: $checkedConvert(
          'price',
          (v) => Money.fromJson(v as Map<String, dynamic>),
        ),
        paidFromPackage: $checkedConvert('paidFromPackage', (v) => v as bool),
      );
      return val;
    });

Map<String, dynamic> _$MeAppointmentItemToJson(MeAppointmentItem instance) =>
    <String, dynamic>{
      'serviceId': instance.serviceId,
      'serviceName': instance.serviceName,
      'staffId': ?instance.staffId,
      'staffName': ?instance.staffName,
      'startUtc': instance.startUtc.toIso8601String(),
      'endUtc': instance.endUtc.toIso8601String(),
      'price': instance.price.toJson(),
      'paidFromPackage': instance.paidFromPackage,
    };
