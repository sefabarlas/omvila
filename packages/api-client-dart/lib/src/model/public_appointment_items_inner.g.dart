// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'public_appointment_items_inner.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PublicAppointmentItemsInnerCWProxy {
  PublicAppointmentItemsInner serviceName(String serviceName);

  PublicAppointmentItemsInner staffName(String? staffName);

  PublicAppointmentItemsInner startUtc(DateTime startUtc);

  PublicAppointmentItemsInner endUtc(DateTime endUtc);

  PublicAppointmentItemsInner price(Money price);

  PublicAppointmentItemsInner paidFromPackage(bool paidFromPackage);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicAppointmentItemsInner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicAppointmentItemsInner(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicAppointmentItemsInner call({
    String serviceName,
    String? staffName,
    DateTime startUtc,
    DateTime endUtc,
    Money price,
    bool paidFromPackage,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPublicAppointmentItemsInner.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPublicAppointmentItemsInner.copyWith.fieldName(...)`
class _$PublicAppointmentItemsInnerCWProxyImpl
    implements _$PublicAppointmentItemsInnerCWProxy {
  const _$PublicAppointmentItemsInnerCWProxyImpl(this._value);

  final PublicAppointmentItemsInner _value;

  @override
  PublicAppointmentItemsInner serviceName(String serviceName) =>
      this(serviceName: serviceName);

  @override
  PublicAppointmentItemsInner staffName(String? staffName) =>
      this(staffName: staffName);

  @override
  PublicAppointmentItemsInner startUtc(DateTime startUtc) =>
      this(startUtc: startUtc);

  @override
  PublicAppointmentItemsInner endUtc(DateTime endUtc) => this(endUtc: endUtc);

  @override
  PublicAppointmentItemsInner price(Money price) => this(price: price);

  @override
  PublicAppointmentItemsInner paidFromPackage(bool paidFromPackage) =>
      this(paidFromPackage: paidFromPackage);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicAppointmentItemsInner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicAppointmentItemsInner(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicAppointmentItemsInner call({
    Object? serviceName = const $CopyWithPlaceholder(),
    Object? staffName = const $CopyWithPlaceholder(),
    Object? startUtc = const $CopyWithPlaceholder(),
    Object? endUtc = const $CopyWithPlaceholder(),
    Object? price = const $CopyWithPlaceholder(),
    Object? paidFromPackage = const $CopyWithPlaceholder(),
  }) {
    return PublicAppointmentItemsInner(
      serviceName: serviceName == const $CopyWithPlaceholder()
          ? _value.serviceName
          // ignore: cast_nullable_to_non_nullable
          : serviceName as String,
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

extension $PublicAppointmentItemsInnerCopyWith on PublicAppointmentItemsInner {
  /// Returns a callable class that can be used as follows: `instanceOfPublicAppointmentItemsInner.copyWith(...)` or like so:`instanceOfPublicAppointmentItemsInner.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PublicAppointmentItemsInnerCWProxy get copyWith =>
      _$PublicAppointmentItemsInnerCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PublicAppointmentItemsInner _$PublicAppointmentItemsInnerFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('PublicAppointmentItemsInner', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const [
      'serviceName',
      'startUtc',
      'endUtc',
      'price',
      'paidFromPackage',
    ],
  );
  final val = PublicAppointmentItemsInner(
    serviceName: $checkedConvert('serviceName', (v) => v as String),
    staffName: $checkedConvert('staffName', (v) => v as String?),
    startUtc: $checkedConvert('startUtc', (v) => DateTime.parse(v as String)),
    endUtc: $checkedConvert('endUtc', (v) => DateTime.parse(v as String)),
    price: $checkedConvert(
      'price',
      (v) => Money.fromJson(v as Map<String, dynamic>),
    ),
    paidFromPackage: $checkedConvert('paidFromPackage', (v) => v as bool),
  );
  return val;
});

Map<String, dynamic> _$PublicAppointmentItemsInnerToJson(
  PublicAppointmentItemsInner instance,
) => <String, dynamic>{
  'serviceName': instance.serviceName,
  'staffName': ?instance.staffName,
  'startUtc': instance.startUtc.toIso8601String(),
  'endUtc': instance.endUtc.toIso8601String(),
  'price': instance.price.toJson(),
  'paidFromPackage': instance.paidFromPackage,
};
