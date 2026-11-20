// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'slot_items_inner.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$SlotItemsInnerCWProxy {
  SlotItemsInner serviceId(String serviceId);

  SlotItemsInner staffId(String staffId);

  SlotItemsInner staffName(String? staffName);

  SlotItemsInner startUtc(DateTime startUtc);

  SlotItemsInner endUtc(DateTime endUtc);

  SlotItemsInner price(Money? price);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `SlotItemsInner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// SlotItemsInner(...).copyWith(id: 12, name: "My name")
  /// ````
  SlotItemsInner call({
    String serviceId,
    String staffId,
    String? staffName,
    DateTime startUtc,
    DateTime endUtc,
    Money? price,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfSlotItemsInner.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfSlotItemsInner.copyWith.fieldName(...)`
class _$SlotItemsInnerCWProxyImpl implements _$SlotItemsInnerCWProxy {
  const _$SlotItemsInnerCWProxyImpl(this._value);

  final SlotItemsInner _value;

  @override
  SlotItemsInner serviceId(String serviceId) => this(serviceId: serviceId);

  @override
  SlotItemsInner staffId(String staffId) => this(staffId: staffId);

  @override
  SlotItemsInner staffName(String? staffName) => this(staffName: staffName);

  @override
  SlotItemsInner startUtc(DateTime startUtc) => this(startUtc: startUtc);

  @override
  SlotItemsInner endUtc(DateTime endUtc) => this(endUtc: endUtc);

  @override
  SlotItemsInner price(Money? price) => this(price: price);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `SlotItemsInner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// SlotItemsInner(...).copyWith(id: 12, name: "My name")
  /// ````
  SlotItemsInner call({
    Object? serviceId = const $CopyWithPlaceholder(),
    Object? staffId = const $CopyWithPlaceholder(),
    Object? staffName = const $CopyWithPlaceholder(),
    Object? startUtc = const $CopyWithPlaceholder(),
    Object? endUtc = const $CopyWithPlaceholder(),
    Object? price = const $CopyWithPlaceholder(),
  }) {
    return SlotItemsInner(
      serviceId: serviceId == const $CopyWithPlaceholder()
          ? _value.serviceId
          // ignore: cast_nullable_to_non_nullable
          : serviceId as String,
      staffId: staffId == const $CopyWithPlaceholder()
          ? _value.staffId
          // ignore: cast_nullable_to_non_nullable
          : staffId as String,
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
          : price as Money?,
    );
  }
}

extension $SlotItemsInnerCopyWith on SlotItemsInner {
  /// Returns a callable class that can be used as follows: `instanceOfSlotItemsInner.copyWith(...)` or like so:`instanceOfSlotItemsInner.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$SlotItemsInnerCWProxy get copyWith => _$SlotItemsInnerCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

SlotItemsInner _$SlotItemsInnerFromJson(Map<String, dynamic> json) =>
    $checkedCreate('SlotItemsInner', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const ['serviceId', 'staffId', 'startUtc', 'endUtc'],
      );
      final val = SlotItemsInner(
        serviceId: $checkedConvert('serviceId', (v) => v as String),
        staffId: $checkedConvert('staffId', (v) => v as String),
        staffName: $checkedConvert('staffName', (v) => v as String?),
        startUtc: $checkedConvert(
          'startUtc',
          (v) => DateTime.parse(v as String),
        ),
        endUtc: $checkedConvert('endUtc', (v) => DateTime.parse(v as String)),
        price: $checkedConvert(
          'price',
          (v) => v == null ? null : Money.fromJson(v as Map<String, dynamic>),
        ),
      );
      return val;
    });

Map<String, dynamic> _$SlotItemsInnerToJson(SlotItemsInner instance) =>
    <String, dynamic>{
      'serviceId': instance.serviceId,
      'staffId': instance.staffId,
      'staffName': ?instance.staffName,
      'startUtc': instance.startUtc.toIso8601String(),
      'endUtc': instance.endUtc.toIso8601String(),
      'price': ?instance.price?.toJson(),
    };
