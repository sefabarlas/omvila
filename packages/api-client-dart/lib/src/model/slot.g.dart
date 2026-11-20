// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'slot.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$SlotCWProxy {
  Slot startUtc(DateTime startUtc);

  Slot localTime(String localTime);

  Slot endLocalTime(String? endLocalTime);

  Slot period(SlotPeriodEnum period);

  Slot items(List<SlotItemsInner> items);

  Slot total(Money total);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `Slot(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// Slot(...).copyWith(id: 12, name: "My name")
  /// ````
  Slot call({
    DateTime startUtc,
    String localTime,
    String? endLocalTime,
    SlotPeriodEnum period,
    List<SlotItemsInner> items,
    Money total,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfSlot.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfSlot.copyWith.fieldName(...)`
class _$SlotCWProxyImpl implements _$SlotCWProxy {
  const _$SlotCWProxyImpl(this._value);

  final Slot _value;

  @override
  Slot startUtc(DateTime startUtc) => this(startUtc: startUtc);

  @override
  Slot localTime(String localTime) => this(localTime: localTime);

  @override
  Slot endLocalTime(String? endLocalTime) => this(endLocalTime: endLocalTime);

  @override
  Slot period(SlotPeriodEnum period) => this(period: period);

  @override
  Slot items(List<SlotItemsInner> items) => this(items: items);

  @override
  Slot total(Money total) => this(total: total);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `Slot(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// Slot(...).copyWith(id: 12, name: "My name")
  /// ````
  Slot call({
    Object? startUtc = const $CopyWithPlaceholder(),
    Object? localTime = const $CopyWithPlaceholder(),
    Object? endLocalTime = const $CopyWithPlaceholder(),
    Object? period = const $CopyWithPlaceholder(),
    Object? items = const $CopyWithPlaceholder(),
    Object? total = const $CopyWithPlaceholder(),
  }) {
    return Slot(
      startUtc: startUtc == const $CopyWithPlaceholder()
          ? _value.startUtc
          // ignore: cast_nullable_to_non_nullable
          : startUtc as DateTime,
      localTime: localTime == const $CopyWithPlaceholder()
          ? _value.localTime
          // ignore: cast_nullable_to_non_nullable
          : localTime as String,
      endLocalTime: endLocalTime == const $CopyWithPlaceholder()
          ? _value.endLocalTime
          // ignore: cast_nullable_to_non_nullable
          : endLocalTime as String?,
      period: period == const $CopyWithPlaceholder()
          ? _value.period
          // ignore: cast_nullable_to_non_nullable
          : period as SlotPeriodEnum,
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<SlotItemsInner>,
      total: total == const $CopyWithPlaceholder()
          ? _value.total
          // ignore: cast_nullable_to_non_nullable
          : total as Money,
    );
  }
}

extension $SlotCopyWith on Slot {
  /// Returns a callable class that can be used as follows: `instanceOfSlot.copyWith(...)` or like so:`instanceOfSlot.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$SlotCWProxy get copyWith => _$SlotCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

Slot _$SlotFromJson(Map<String, dynamic> json) => $checkedCreate('Slot', json, (
  $checkedConvert,
) {
  $checkKeys(
    json,
    requiredKeys: const ['startUtc', 'localTime', 'period', 'items', 'total'],
  );
  final val = Slot(
    startUtc: $checkedConvert('startUtc', (v) => DateTime.parse(v as String)),
    localTime: $checkedConvert('localTime', (v) => v as String),
    endLocalTime: $checkedConvert('endLocalTime', (v) => v as String?),
    period: $checkedConvert(
      'period',
      (v) => $enumDecode(
        _$SlotPeriodEnumEnumMap,
        v,
        unknownValue: SlotPeriodEnum.unknownDefaultOpenApi,
      ),
    ),
    items: $checkedConvert(
      'items',
      (v) => (v as List<dynamic>)
          .map((e) => SlotItemsInner.fromJson(e as Map<String, dynamic>))
          .toList(),
    ),
    total: $checkedConvert(
      'total',
      (v) => Money.fromJson(v as Map<String, dynamic>),
    ),
  );
  return val;
});

Map<String, dynamic> _$SlotToJson(Slot instance) => <String, dynamic>{
  'startUtc': instance.startUtc.toIso8601String(),
  'localTime': instance.localTime,
  'endLocalTime': ?instance.endLocalTime,
  'period': _$SlotPeriodEnumEnumMap[instance.period]!,
  'items': instance.items.map((e) => e.toJson()).toList(),
  'total': instance.total.toJson(),
};

const _$SlotPeriodEnumEnumMap = {
  SlotPeriodEnum.morning: 'Morning',
  SlotPeriodEnum.afternoon: 'Afternoon',
  SlotPeriodEnum.evening: 'Evening',
  SlotPeriodEnum.unknownDefaultOpenApi: 'unknown_default_open_api',
};
