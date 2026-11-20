// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'working_hours_override_create.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$WorkingHoursOverrideCreateCWProxy {
  WorkingHoursOverrideCreate fromDate(DateTime fromDate);

  WorkingHoursOverrideCreate toDate(DateTime? toDate);

  WorkingHoursOverrideCreate kind(WorkingHoursOverrideCreateKindEnum kind);

  WorkingHoursOverrideCreate isClosed(bool isClosed);

  WorkingHoursOverrideCreate intervals(List<TimeInterval>? intervals);

  WorkingHoursOverrideCreate note(String? note);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `WorkingHoursOverrideCreate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// WorkingHoursOverrideCreate(...).copyWith(id: 12, name: "My name")
  /// ````
  WorkingHoursOverrideCreate call({
    DateTime fromDate,
    DateTime? toDate,
    WorkingHoursOverrideCreateKindEnum kind,
    bool isClosed,
    List<TimeInterval>? intervals,
    String? note,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfWorkingHoursOverrideCreate.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfWorkingHoursOverrideCreate.copyWith.fieldName(...)`
class _$WorkingHoursOverrideCreateCWProxyImpl
    implements _$WorkingHoursOverrideCreateCWProxy {
  const _$WorkingHoursOverrideCreateCWProxyImpl(this._value);

  final WorkingHoursOverrideCreate _value;

  @override
  WorkingHoursOverrideCreate fromDate(DateTime fromDate) =>
      this(fromDate: fromDate);

  @override
  WorkingHoursOverrideCreate toDate(DateTime? toDate) => this(toDate: toDate);

  @override
  WorkingHoursOverrideCreate kind(WorkingHoursOverrideCreateKindEnum kind) =>
      this(kind: kind);

  @override
  WorkingHoursOverrideCreate isClosed(bool isClosed) =>
      this(isClosed: isClosed);

  @override
  WorkingHoursOverrideCreate intervals(List<TimeInterval>? intervals) =>
      this(intervals: intervals);

  @override
  WorkingHoursOverrideCreate note(String? note) => this(note: note);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `WorkingHoursOverrideCreate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// WorkingHoursOverrideCreate(...).copyWith(id: 12, name: "My name")
  /// ````
  WorkingHoursOverrideCreate call({
    Object? fromDate = const $CopyWithPlaceholder(),
    Object? toDate = const $CopyWithPlaceholder(),
    Object? kind = const $CopyWithPlaceholder(),
    Object? isClosed = const $CopyWithPlaceholder(),
    Object? intervals = const $CopyWithPlaceholder(),
    Object? note = const $CopyWithPlaceholder(),
  }) {
    return WorkingHoursOverrideCreate(
      fromDate: fromDate == const $CopyWithPlaceholder()
          ? _value.fromDate
          // ignore: cast_nullable_to_non_nullable
          : fromDate as DateTime,
      toDate: toDate == const $CopyWithPlaceholder()
          ? _value.toDate
          // ignore: cast_nullable_to_non_nullable
          : toDate as DateTime?,
      kind: kind == const $CopyWithPlaceholder()
          ? _value.kind
          // ignore: cast_nullable_to_non_nullable
          : kind as WorkingHoursOverrideCreateKindEnum,
      isClosed: isClosed == const $CopyWithPlaceholder()
          ? _value.isClosed
          // ignore: cast_nullable_to_non_nullable
          : isClosed as bool,
      intervals: intervals == const $CopyWithPlaceholder()
          ? _value.intervals
          // ignore: cast_nullable_to_non_nullable
          : intervals as List<TimeInterval>?,
      note: note == const $CopyWithPlaceholder()
          ? _value.note
          // ignore: cast_nullable_to_non_nullable
          : note as String?,
    );
  }
}

extension $WorkingHoursOverrideCreateCopyWith on WorkingHoursOverrideCreate {
  /// Returns a callable class that can be used as follows: `instanceOfWorkingHoursOverrideCreate.copyWith(...)` or like so:`instanceOfWorkingHoursOverrideCreate.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$WorkingHoursOverrideCreateCWProxy get copyWith =>
      _$WorkingHoursOverrideCreateCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

WorkingHoursOverrideCreate _$WorkingHoursOverrideCreateFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('WorkingHoursOverrideCreate', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['fromDate', 'kind', 'isClosed']);
  final val = WorkingHoursOverrideCreate(
    fromDate: $checkedConvert('fromDate', (v) => DateTime.parse(v as String)),
    toDate: $checkedConvert(
      'toDate',
      (v) => v == null ? null : DateTime.parse(v as String),
    ),
    kind: $checkedConvert(
      'kind',
      (v) => $enumDecode(
        _$WorkingHoursOverrideCreateKindEnumEnumMap,
        v,
        unknownValue: WorkingHoursOverrideCreateKindEnum.unknownDefaultOpenApi,
      ),
    ),
    isClosed: $checkedConvert('isClosed', (v) => v as bool),
    intervals: $checkedConvert(
      'intervals',
      (v) => (v as List<dynamic>?)
          ?.map((e) => TimeInterval.fromJson(e as Map<String, dynamic>))
          .toList(),
    ),
    note: $checkedConvert('note', (v) => v as String?),
  );
  return val;
});

Map<String, dynamic> _$WorkingHoursOverrideCreateToJson(
  WorkingHoursOverrideCreate instance,
) => <String, dynamic>{
  'fromDate': instance.fromDate.toIso8601String(),
  'toDate': ?instance.toDate?.toIso8601String(),
  'kind': _$WorkingHoursOverrideCreateKindEnumEnumMap[instance.kind]!,
  'isClosed': instance.isClosed,
  'intervals': ?instance.intervals?.map((e) => e.toJson()).toList(),
  'note': ?instance.note,
};

const _$WorkingHoursOverrideCreateKindEnumEnumMap = {
  WorkingHoursOverrideCreateKindEnum.holiday: 'Holiday',
  WorkingHoursOverrideCreateKindEnum.closure: 'Closure',
  WorkingHoursOverrideCreateKindEnum.extraOpening: 'ExtraOpening',
  WorkingHoursOverrideCreateKindEnum.shortDay: 'ShortDay',
  WorkingHoursOverrideCreateKindEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};
