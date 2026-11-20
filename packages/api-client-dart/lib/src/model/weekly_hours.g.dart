// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'weekly_hours.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$WeeklyHoursCWProxy {
  WeeklyHours days(List<WeeklyHoursDaysInner> days);

  WeeklyHours version(String? version);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `WeeklyHours(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// WeeklyHours(...).copyWith(id: 12, name: "My name")
  /// ````
  WeeklyHours call({List<WeeklyHoursDaysInner> days, String? version});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfWeeklyHours.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfWeeklyHours.copyWith.fieldName(...)`
class _$WeeklyHoursCWProxyImpl implements _$WeeklyHoursCWProxy {
  const _$WeeklyHoursCWProxyImpl(this._value);

  final WeeklyHours _value;

  @override
  WeeklyHours days(List<WeeklyHoursDaysInner> days) => this(days: days);

  @override
  WeeklyHours version(String? version) => this(version: version);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `WeeklyHours(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// WeeklyHours(...).copyWith(id: 12, name: "My name")
  /// ````
  WeeklyHours call({
    Object? days = const $CopyWithPlaceholder(),
    Object? version = const $CopyWithPlaceholder(),
  }) {
    return WeeklyHours(
      days: days == const $CopyWithPlaceholder()
          ? _value.days
          // ignore: cast_nullable_to_non_nullable
          : days as List<WeeklyHoursDaysInner>,
      version: version == const $CopyWithPlaceholder()
          ? _value.version
          // ignore: cast_nullable_to_non_nullable
          : version as String?,
    );
  }
}

extension $WeeklyHoursCopyWith on WeeklyHours {
  /// Returns a callable class that can be used as follows: `instanceOfWeeklyHours.copyWith(...)` or like so:`instanceOfWeeklyHours.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$WeeklyHoursCWProxy get copyWith => _$WeeklyHoursCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

WeeklyHours _$WeeklyHoursFromJson(Map<String, dynamic> json) =>
    $checkedCreate('WeeklyHours', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['days']);
      final val = WeeklyHours(
        days: $checkedConvert(
          'days',
          (v) => (v as List<dynamic>)
              .map(
                (e) => WeeklyHoursDaysInner.fromJson(e as Map<String, dynamic>),
              )
              .toList(),
        ),
        version: $checkedConvert('version', (v) => v as String?),
      );
      return val;
    });

Map<String, dynamic> _$WeeklyHoursToJson(WeeklyHours instance) =>
    <String, dynamic>{
      'days': instance.days.map((e) => e.toJson()).toList(),
      'version': ?instance.version,
    };
