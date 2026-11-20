// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'time_off.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$TimeOffCWProxy {
  TimeOff id(String id);

  TimeOff staffId(String staffId);

  TimeOff type(TimeOffTypeEnum type);

  TimeOff startUtc(DateTime startUtc);

  TimeOff endUtc(DateTime endUtc);

  TimeOff isAllDay(bool isAllDay);

  TimeOff note(String? note);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `TimeOff(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// TimeOff(...).copyWith(id: 12, name: "My name")
  /// ````
  TimeOff call({
    String id,
    String staffId,
    TimeOffTypeEnum type,
    DateTime startUtc,
    DateTime endUtc,
    bool isAllDay,
    String? note,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfTimeOff.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfTimeOff.copyWith.fieldName(...)`
class _$TimeOffCWProxyImpl implements _$TimeOffCWProxy {
  const _$TimeOffCWProxyImpl(this._value);

  final TimeOff _value;

  @override
  TimeOff id(String id) => this(id: id);

  @override
  TimeOff staffId(String staffId) => this(staffId: staffId);

  @override
  TimeOff type(TimeOffTypeEnum type) => this(type: type);

  @override
  TimeOff startUtc(DateTime startUtc) => this(startUtc: startUtc);

  @override
  TimeOff endUtc(DateTime endUtc) => this(endUtc: endUtc);

  @override
  TimeOff isAllDay(bool isAllDay) => this(isAllDay: isAllDay);

  @override
  TimeOff note(String? note) => this(note: note);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `TimeOff(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// TimeOff(...).copyWith(id: 12, name: "My name")
  /// ````
  TimeOff call({
    Object? id = const $CopyWithPlaceholder(),
    Object? staffId = const $CopyWithPlaceholder(),
    Object? type = const $CopyWithPlaceholder(),
    Object? startUtc = const $CopyWithPlaceholder(),
    Object? endUtc = const $CopyWithPlaceholder(),
    Object? isAllDay = const $CopyWithPlaceholder(),
    Object? note = const $CopyWithPlaceholder(),
  }) {
    return TimeOff(
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as String,
      staffId: staffId == const $CopyWithPlaceholder()
          ? _value.staffId
          // ignore: cast_nullable_to_non_nullable
          : staffId as String,
      type: type == const $CopyWithPlaceholder()
          ? _value.type
          // ignore: cast_nullable_to_non_nullable
          : type as TimeOffTypeEnum,
      startUtc: startUtc == const $CopyWithPlaceholder()
          ? _value.startUtc
          // ignore: cast_nullable_to_non_nullable
          : startUtc as DateTime,
      endUtc: endUtc == const $CopyWithPlaceholder()
          ? _value.endUtc
          // ignore: cast_nullable_to_non_nullable
          : endUtc as DateTime,
      isAllDay: isAllDay == const $CopyWithPlaceholder()
          ? _value.isAllDay
          // ignore: cast_nullable_to_non_nullable
          : isAllDay as bool,
      note: note == const $CopyWithPlaceholder()
          ? _value.note
          // ignore: cast_nullable_to_non_nullable
          : note as String?,
    );
  }
}

extension $TimeOffCopyWith on TimeOff {
  /// Returns a callable class that can be used as follows: `instanceOfTimeOff.copyWith(...)` or like so:`instanceOfTimeOff.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$TimeOffCWProxy get copyWith => _$TimeOffCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

TimeOff _$TimeOffFromJson(Map<String, dynamic> json) => $checkedCreate(
  'TimeOff',
  json,
  ($checkedConvert) {
    $checkKeys(
      json,
      requiredKeys: const [
        'id',
        'staffId',
        'type',
        'startUtc',
        'endUtc',
        'isAllDay',
      ],
    );
    final val = TimeOff(
      id: $checkedConvert('id', (v) => v as String),
      staffId: $checkedConvert('staffId', (v) => v as String),
      type: $checkedConvert(
        'type',
        (v) => $enumDecode(
          _$TimeOffTypeEnumEnumMap,
          v,
          unknownValue: TimeOffTypeEnum.unknownDefaultOpenApi,
        ),
      ),
      startUtc: $checkedConvert('startUtc', (v) => DateTime.parse(v as String)),
      endUtc: $checkedConvert('endUtc', (v) => DateTime.parse(v as String)),
      isAllDay: $checkedConvert('isAllDay', (v) => v as bool),
      note: $checkedConvert('note', (v) => v as String?),
    );
    return val;
  },
);

Map<String, dynamic> _$TimeOffToJson(TimeOff instance) => <String, dynamic>{
  'id': instance.id,
  'staffId': instance.staffId,
  'type': _$TimeOffTypeEnumEnumMap[instance.type]!,
  'startUtc': instance.startUtc.toIso8601String(),
  'endUtc': instance.endUtc.toIso8601String(),
  'isAllDay': instance.isAllDay,
  'note': ?instance.note,
};

const _$TimeOffTypeEnumEnumMap = {
  TimeOffTypeEnum.leave: 'Leave',
  TimeOffTypeEnum.break_: 'Break',
  TimeOffTypeEnum.training: 'Training',
  TimeOffTypeEnum.sick: 'Sick',
  TimeOffTypeEnum.other: 'Other',
  TimeOffTypeEnum.unknownDefaultOpenApi: 'unknown_default_open_api',
};
