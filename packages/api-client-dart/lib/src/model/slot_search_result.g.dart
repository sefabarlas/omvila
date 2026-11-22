// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'slot_search_result.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$SlotSearchResultCWProxy {
  SlotSearchResult timeZone(String timeZone);

  SlotSearchResult date(DateTime date);

  SlotSearchResult slots(List<Slot> slots);

  SlotSearchResult unavailableTimes(List<String> unavailableTimes);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `SlotSearchResult(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// SlotSearchResult(...).copyWith(id: 12, name: "My name")
  /// ````
  SlotSearchResult call({
    String timeZone,
    DateTime date,
    List<Slot> slots,
    List<String> unavailableTimes,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfSlotSearchResult.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfSlotSearchResult.copyWith.fieldName(...)`
class _$SlotSearchResultCWProxyImpl implements _$SlotSearchResultCWProxy {
  const _$SlotSearchResultCWProxyImpl(this._value);

  final SlotSearchResult _value;

  @override
  SlotSearchResult timeZone(String timeZone) => this(timeZone: timeZone);

  @override
  SlotSearchResult date(DateTime date) => this(date: date);

  @override
  SlotSearchResult slots(List<Slot> slots) => this(slots: slots);

  @override
  SlotSearchResult unavailableTimes(List<String> unavailableTimes) =>
      this(unavailableTimes: unavailableTimes);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `SlotSearchResult(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// SlotSearchResult(...).copyWith(id: 12, name: "My name")
  /// ````
  SlotSearchResult call({
    Object? timeZone = const $CopyWithPlaceholder(),
    Object? date = const $CopyWithPlaceholder(),
    Object? slots = const $CopyWithPlaceholder(),
    Object? unavailableTimes = const $CopyWithPlaceholder(),
  }) {
    return SlotSearchResult(
      timeZone: timeZone == const $CopyWithPlaceholder()
          ? _value.timeZone
          // ignore: cast_nullable_to_non_nullable
          : timeZone as String,
      date: date == const $CopyWithPlaceholder()
          ? _value.date
          // ignore: cast_nullable_to_non_nullable
          : date as DateTime,
      slots: slots == const $CopyWithPlaceholder()
          ? _value.slots
          // ignore: cast_nullable_to_non_nullable
          : slots as List<Slot>,
      unavailableTimes: unavailableTimes == const $CopyWithPlaceholder()
          ? _value.unavailableTimes
          // ignore: cast_nullable_to_non_nullable
          : unavailableTimes as List<String>,
    );
  }
}

extension $SlotSearchResultCopyWith on SlotSearchResult {
  /// Returns a callable class that can be used as follows: `instanceOfSlotSearchResult.copyWith(...)` or like so:`instanceOfSlotSearchResult.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$SlotSearchResultCWProxy get copyWith => _$SlotSearchResultCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

SlotSearchResult _$SlotSearchResultFromJson(Map<String, dynamic> json) =>
    $checkedCreate('SlotSearchResult', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const ['timeZone', 'date', 'slots', 'unavailableTimes'],
      );
      final val = SlotSearchResult(
        timeZone: $checkedConvert('timeZone', (v) => v as String),
        date: $checkedConvert(
          'date',
          (v) => const DateOnlyConverter().fromJson(v as String),
        ),
        slots: $checkedConvert(
          'slots',
          (v) => (v as List<dynamic>)
              .map((e) => Slot.fromJson(e as Map<String, dynamic>))
              .toList(),
        ),
        unavailableTimes: $checkedConvert(
          'unavailableTimes',
          (v) => (v as List<dynamic>).map((e) => e as String).toList(),
        ),
      );
      return val;
    });

Map<String, dynamic> _$SlotSearchResultToJson(SlotSearchResult instance) =>
    <String, dynamic>{
      'timeZone': instance.timeZone,
      'date': const DateOnlyConverter().toJson(instance.date),
      'slots': instance.slots.map((e) => e.toJson()).toList(),
      'unavailableTimes': instance.unavailableTimes,
    };
