// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'time_interval.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$TimeIntervalCWProxy {
  TimeInterval start(String start);

  TimeInterval end(String end);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `TimeInterval(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// TimeInterval(...).copyWith(id: 12, name: "My name")
  /// ````
  TimeInterval call({String start, String end});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfTimeInterval.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfTimeInterval.copyWith.fieldName(...)`
class _$TimeIntervalCWProxyImpl implements _$TimeIntervalCWProxy {
  const _$TimeIntervalCWProxyImpl(this._value);

  final TimeInterval _value;

  @override
  TimeInterval start(String start) => this(start: start);

  @override
  TimeInterval end(String end) => this(end: end);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `TimeInterval(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// TimeInterval(...).copyWith(id: 12, name: "My name")
  /// ````
  TimeInterval call({
    Object? start = const $CopyWithPlaceholder(),
    Object? end = const $CopyWithPlaceholder(),
  }) {
    return TimeInterval(
      start: start == const $CopyWithPlaceholder()
          ? _value.start
          // ignore: cast_nullable_to_non_nullable
          : start as String,
      end: end == const $CopyWithPlaceholder()
          ? _value.end
          // ignore: cast_nullable_to_non_nullable
          : end as String,
    );
  }
}

extension $TimeIntervalCopyWith on TimeInterval {
  /// Returns a callable class that can be used as follows: `instanceOfTimeInterval.copyWith(...)` or like so:`instanceOfTimeInterval.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$TimeIntervalCWProxy get copyWith => _$TimeIntervalCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

TimeInterval _$TimeIntervalFromJson(Map<String, dynamic> json) =>
    $checkedCreate('TimeInterval', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['start', 'end']);
      final val = TimeInterval(
        start: $checkedConvert('start', (v) => v as String),
        end: $checkedConvert('end', (v) => v as String),
      );
      return val;
    });

Map<String, dynamic> _$TimeIntervalToJson(TimeInterval instance) =>
    <String, dynamic>{'start': instance.start, 'end': instance.end};
