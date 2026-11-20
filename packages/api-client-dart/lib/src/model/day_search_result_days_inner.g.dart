// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'day_search_result_days_inner.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$DaySearchResultDaysInnerCWProxy {
  DaySearchResultDaysInner date(DateTime date);

  DaySearchResultDaysInner hasAvailability(bool hasAvailability);

  DaySearchResultDaysInner isClosed(bool? isClosed);

  DaySearchResultDaysInner firstSlotUtc(DateTime? firstSlotUtc);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `DaySearchResultDaysInner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// DaySearchResultDaysInner(...).copyWith(id: 12, name: "My name")
  /// ````
  DaySearchResultDaysInner call({
    DateTime date,
    bool hasAvailability,
    bool? isClosed,
    DateTime? firstSlotUtc,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfDaySearchResultDaysInner.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfDaySearchResultDaysInner.copyWith.fieldName(...)`
class _$DaySearchResultDaysInnerCWProxyImpl
    implements _$DaySearchResultDaysInnerCWProxy {
  const _$DaySearchResultDaysInnerCWProxyImpl(this._value);

  final DaySearchResultDaysInner _value;

  @override
  DaySearchResultDaysInner date(DateTime date) => this(date: date);

  @override
  DaySearchResultDaysInner hasAvailability(bool hasAvailability) =>
      this(hasAvailability: hasAvailability);

  @override
  DaySearchResultDaysInner isClosed(bool? isClosed) => this(isClosed: isClosed);

  @override
  DaySearchResultDaysInner firstSlotUtc(DateTime? firstSlotUtc) =>
      this(firstSlotUtc: firstSlotUtc);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `DaySearchResultDaysInner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// DaySearchResultDaysInner(...).copyWith(id: 12, name: "My name")
  /// ````
  DaySearchResultDaysInner call({
    Object? date = const $CopyWithPlaceholder(),
    Object? hasAvailability = const $CopyWithPlaceholder(),
    Object? isClosed = const $CopyWithPlaceholder(),
    Object? firstSlotUtc = const $CopyWithPlaceholder(),
  }) {
    return DaySearchResultDaysInner(
      date: date == const $CopyWithPlaceholder()
          ? _value.date
          // ignore: cast_nullable_to_non_nullable
          : date as DateTime,
      hasAvailability: hasAvailability == const $CopyWithPlaceholder()
          ? _value.hasAvailability
          // ignore: cast_nullable_to_non_nullable
          : hasAvailability as bool,
      isClosed: isClosed == const $CopyWithPlaceholder()
          ? _value.isClosed
          // ignore: cast_nullable_to_non_nullable
          : isClosed as bool?,
      firstSlotUtc: firstSlotUtc == const $CopyWithPlaceholder()
          ? _value.firstSlotUtc
          // ignore: cast_nullable_to_non_nullable
          : firstSlotUtc as DateTime?,
    );
  }
}

extension $DaySearchResultDaysInnerCopyWith on DaySearchResultDaysInner {
  /// Returns a callable class that can be used as follows: `instanceOfDaySearchResultDaysInner.copyWith(...)` or like so:`instanceOfDaySearchResultDaysInner.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$DaySearchResultDaysInnerCWProxy get copyWith =>
      _$DaySearchResultDaysInnerCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

DaySearchResultDaysInner _$DaySearchResultDaysInnerFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('DaySearchResultDaysInner', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['date', 'hasAvailability']);
  final val = DaySearchResultDaysInner(
    date: $checkedConvert('date', (v) => DateTime.parse(v as String)),
    hasAvailability: $checkedConvert('hasAvailability', (v) => v as bool),
    isClosed: $checkedConvert('isClosed', (v) => v as bool?),
    firstSlotUtc: $checkedConvert(
      'firstSlotUtc',
      (v) => v == null ? null : DateTime.parse(v as String),
    ),
  );
  return val;
});

Map<String, dynamic> _$DaySearchResultDaysInnerToJson(
  DaySearchResultDaysInner instance,
) => <String, dynamic>{
  'date': instance.date.toIso8601String(),
  'hasAvailability': instance.hasAvailability,
  'isClosed': ?instance.isClosed,
  'firstSlotUtc': ?instance.firstSlotUtc?.toIso8601String(),
};
