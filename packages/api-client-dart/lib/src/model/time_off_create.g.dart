// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'time_off_create.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$TimeOffCreateCWProxy {
  TimeOffCreate type(TimeOffCreateTypeEnum type);

  TimeOffCreate isAllDay(bool isAllDay);

  TimeOffCreate fromDate(DateTime? fromDate);

  TimeOffCreate toDate(DateTime? toDate);

  TimeOffCreate startUtc(DateTime? startUtc);

  TimeOffCreate endUtc(DateTime? endUtc);

  TimeOffCreate note(String? note);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `TimeOffCreate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// TimeOffCreate(...).copyWith(id: 12, name: "My name")
  /// ````
  TimeOffCreate call({
    TimeOffCreateTypeEnum type,
    bool isAllDay,
    DateTime? fromDate,
    DateTime? toDate,
    DateTime? startUtc,
    DateTime? endUtc,
    String? note,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfTimeOffCreate.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfTimeOffCreate.copyWith.fieldName(...)`
class _$TimeOffCreateCWProxyImpl implements _$TimeOffCreateCWProxy {
  const _$TimeOffCreateCWProxyImpl(this._value);

  final TimeOffCreate _value;

  @override
  TimeOffCreate type(TimeOffCreateTypeEnum type) => this(type: type);

  @override
  TimeOffCreate isAllDay(bool isAllDay) => this(isAllDay: isAllDay);

  @override
  TimeOffCreate fromDate(DateTime? fromDate) => this(fromDate: fromDate);

  @override
  TimeOffCreate toDate(DateTime? toDate) => this(toDate: toDate);

  @override
  TimeOffCreate startUtc(DateTime? startUtc) => this(startUtc: startUtc);

  @override
  TimeOffCreate endUtc(DateTime? endUtc) => this(endUtc: endUtc);

  @override
  TimeOffCreate note(String? note) => this(note: note);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `TimeOffCreate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// TimeOffCreate(...).copyWith(id: 12, name: "My name")
  /// ````
  TimeOffCreate call({
    Object? type = const $CopyWithPlaceholder(),
    Object? isAllDay = const $CopyWithPlaceholder(),
    Object? fromDate = const $CopyWithPlaceholder(),
    Object? toDate = const $CopyWithPlaceholder(),
    Object? startUtc = const $CopyWithPlaceholder(),
    Object? endUtc = const $CopyWithPlaceholder(),
    Object? note = const $CopyWithPlaceholder(),
  }) {
    return TimeOffCreate(
      type: type == const $CopyWithPlaceholder()
          ? _value.type
          // ignore: cast_nullable_to_non_nullable
          : type as TimeOffCreateTypeEnum,
      isAllDay: isAllDay == const $CopyWithPlaceholder()
          ? _value.isAllDay
          // ignore: cast_nullable_to_non_nullable
          : isAllDay as bool,
      fromDate: fromDate == const $CopyWithPlaceholder()
          ? _value.fromDate
          // ignore: cast_nullable_to_non_nullable
          : fromDate as DateTime?,
      toDate: toDate == const $CopyWithPlaceholder()
          ? _value.toDate
          // ignore: cast_nullable_to_non_nullable
          : toDate as DateTime?,
      startUtc: startUtc == const $CopyWithPlaceholder()
          ? _value.startUtc
          // ignore: cast_nullable_to_non_nullable
          : startUtc as DateTime?,
      endUtc: endUtc == const $CopyWithPlaceholder()
          ? _value.endUtc
          // ignore: cast_nullable_to_non_nullable
          : endUtc as DateTime?,
      note: note == const $CopyWithPlaceholder()
          ? _value.note
          // ignore: cast_nullable_to_non_nullable
          : note as String?,
    );
  }
}

extension $TimeOffCreateCopyWith on TimeOffCreate {
  /// Returns a callable class that can be used as follows: `instanceOfTimeOffCreate.copyWith(...)` or like so:`instanceOfTimeOffCreate.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$TimeOffCreateCWProxy get copyWith => _$TimeOffCreateCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

TimeOffCreate _$TimeOffCreateFromJson(Map<String, dynamic> json) =>
    $checkedCreate('TimeOffCreate', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['type', 'isAllDay']);
      final val = TimeOffCreate(
        type: $checkedConvert(
          'type',
          (v) => $enumDecode(
            _$TimeOffCreateTypeEnumEnumMap,
            v,
            unknownValue: TimeOffCreateTypeEnum.unknownDefaultOpenApi,
          ),
        ),
        isAllDay: $checkedConvert('isAllDay', (v) => v as bool),
        fromDate: $checkedConvert(
          'fromDate',
          (v) => _$JsonConverterFromJson<String, DateTime>(
            v,
            const DateOnlyConverter().fromJson,
          ),
        ),
        toDate: $checkedConvert(
          'toDate',
          (v) => _$JsonConverterFromJson<String, DateTime>(
            v,
            const DateOnlyConverter().fromJson,
          ),
        ),
        startUtc: $checkedConvert(
          'startUtc',
          (v) => v == null ? null : DateTime.parse(v as String),
        ),
        endUtc: $checkedConvert(
          'endUtc',
          (v) => v == null ? null : DateTime.parse(v as String),
        ),
        note: $checkedConvert('note', (v) => v as String?),
      );
      return val;
    });

Map<String, dynamic> _$TimeOffCreateToJson(TimeOffCreate instance) =>
    <String, dynamic>{
      'type': _$TimeOffCreateTypeEnumEnumMap[instance.type]!,
      'isAllDay': instance.isAllDay,
      'fromDate': ?_$JsonConverterToJson<String, DateTime>(
        instance.fromDate,
        const DateOnlyConverter().toJson,
      ),
      'toDate': ?_$JsonConverterToJson<String, DateTime>(
        instance.toDate,
        const DateOnlyConverter().toJson,
      ),
      'startUtc': ?instance.startUtc?.toIso8601String(),
      'endUtc': ?instance.endUtc?.toIso8601String(),
      'note': ?instance.note,
    };

const _$TimeOffCreateTypeEnumEnumMap = {
  TimeOffCreateTypeEnum.leave: 'Leave',
  TimeOffCreateTypeEnum.break_: 'Break',
  TimeOffCreateTypeEnum.training: 'Training',
  TimeOffCreateTypeEnum.sick: 'Sick',
  TimeOffCreateTypeEnum.other: 'Other',
  TimeOffCreateTypeEnum.unknownDefaultOpenApi: 'unknown_default_open_api',
};

Value? _$JsonConverterFromJson<Json, Value>(
  Object? json,
  Value? Function(Json json) fromJson,
) => json == null ? null : fromJson(json as Json);

Json? _$JsonConverterToJson<Json, Value>(
  Value? value,
  Json? Function(Value value) toJson,
) => value == null ? null : toJson(value);
