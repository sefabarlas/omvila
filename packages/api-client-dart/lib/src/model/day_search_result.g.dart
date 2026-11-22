// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'day_search_result.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$DaySearchResultCWProxy {
  DaySearchResult timeZone(String timeZone);

  DaySearchResult maxDate(DateTime? maxDate);

  DaySearchResult days(List<DaySearchResultDaysInner> days);

  DaySearchResult nextAvailableUtc(DateTime? nextAvailableUtc);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `DaySearchResult(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// DaySearchResult(...).copyWith(id: 12, name: "My name")
  /// ````
  DaySearchResult call({
    String timeZone,
    DateTime? maxDate,
    List<DaySearchResultDaysInner> days,
    DateTime? nextAvailableUtc,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfDaySearchResult.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfDaySearchResult.copyWith.fieldName(...)`
class _$DaySearchResultCWProxyImpl implements _$DaySearchResultCWProxy {
  const _$DaySearchResultCWProxyImpl(this._value);

  final DaySearchResult _value;

  @override
  DaySearchResult timeZone(String timeZone) => this(timeZone: timeZone);

  @override
  DaySearchResult maxDate(DateTime? maxDate) => this(maxDate: maxDate);

  @override
  DaySearchResult days(List<DaySearchResultDaysInner> days) => this(days: days);

  @override
  DaySearchResult nextAvailableUtc(DateTime? nextAvailableUtc) =>
      this(nextAvailableUtc: nextAvailableUtc);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `DaySearchResult(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// DaySearchResult(...).copyWith(id: 12, name: "My name")
  /// ````
  DaySearchResult call({
    Object? timeZone = const $CopyWithPlaceholder(),
    Object? maxDate = const $CopyWithPlaceholder(),
    Object? days = const $CopyWithPlaceholder(),
    Object? nextAvailableUtc = const $CopyWithPlaceholder(),
  }) {
    return DaySearchResult(
      timeZone: timeZone == const $CopyWithPlaceholder()
          ? _value.timeZone
          // ignore: cast_nullable_to_non_nullable
          : timeZone as String,
      maxDate: maxDate == const $CopyWithPlaceholder()
          ? _value.maxDate
          // ignore: cast_nullable_to_non_nullable
          : maxDate as DateTime?,
      days: days == const $CopyWithPlaceholder()
          ? _value.days
          // ignore: cast_nullable_to_non_nullable
          : days as List<DaySearchResultDaysInner>,
      nextAvailableUtc: nextAvailableUtc == const $CopyWithPlaceholder()
          ? _value.nextAvailableUtc
          // ignore: cast_nullable_to_non_nullable
          : nextAvailableUtc as DateTime?,
    );
  }
}

extension $DaySearchResultCopyWith on DaySearchResult {
  /// Returns a callable class that can be used as follows: `instanceOfDaySearchResult.copyWith(...)` or like so:`instanceOfDaySearchResult.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$DaySearchResultCWProxy get copyWith => _$DaySearchResultCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

DaySearchResult _$DaySearchResultFromJson(Map<String, dynamic> json) =>
    $checkedCreate('DaySearchResult', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['timeZone', 'days']);
      final val = DaySearchResult(
        timeZone: $checkedConvert('timeZone', (v) => v as String),
        maxDate: $checkedConvert(
          'maxDate',
          (v) => _$JsonConverterFromJson<String, DateTime>(
            v,
            const DateOnlyConverter().fromJson,
          ),
        ),
        days: $checkedConvert(
          'days',
          (v) => (v as List<dynamic>)
              .map(
                (e) => DaySearchResultDaysInner.fromJson(
                  e as Map<String, dynamic>,
                ),
              )
              .toList(),
        ),
        nextAvailableUtc: $checkedConvert(
          'nextAvailableUtc',
          (v) => v == null ? null : DateTime.parse(v as String),
        ),
      );
      return val;
    });

Map<String, dynamic> _$DaySearchResultToJson(DaySearchResult instance) =>
    <String, dynamic>{
      'timeZone': instance.timeZone,
      'maxDate': ?_$JsonConverterToJson<String, DateTime>(
        instance.maxDate,
        const DateOnlyConverter().toJson,
      ),
      'days': instance.days.map((e) => e.toJson()).toList(),
      'nextAvailableUtc': ?instance.nextAvailableUtc?.toIso8601String(),
    };

Value? _$JsonConverterFromJson<Json, Value>(
  Object? json,
  Value? Function(Json json) fromJson,
) => json == null ? null : fromJson(json as Json);

Json? _$JsonConverterToJson<Json, Value>(
  Value? value,
  Json? Function(Value value) toJson,
) => value == null ? null : toJson(value);
