// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'working_hours_override.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$WorkingHoursOverrideCWProxy {
  WorkingHoursOverride id(String id);

  WorkingHoursOverride date(DateTime date);

  WorkingHoursOverride kind(WorkingHoursOverrideKindEnum kind);

  WorkingHoursOverride isClosed(bool isClosed);

  WorkingHoursOverride intervals(List<TimeInterval>? intervals);

  WorkingHoursOverride note(String? note);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `WorkingHoursOverride(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// WorkingHoursOverride(...).copyWith(id: 12, name: "My name")
  /// ````
  WorkingHoursOverride call({
    String id,
    DateTime date,
    WorkingHoursOverrideKindEnum kind,
    bool isClosed,
    List<TimeInterval>? intervals,
    String? note,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfWorkingHoursOverride.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfWorkingHoursOverride.copyWith.fieldName(...)`
class _$WorkingHoursOverrideCWProxyImpl
    implements _$WorkingHoursOverrideCWProxy {
  const _$WorkingHoursOverrideCWProxyImpl(this._value);

  final WorkingHoursOverride _value;

  @override
  WorkingHoursOverride id(String id) => this(id: id);

  @override
  WorkingHoursOverride date(DateTime date) => this(date: date);

  @override
  WorkingHoursOverride kind(WorkingHoursOverrideKindEnum kind) =>
      this(kind: kind);

  @override
  WorkingHoursOverride isClosed(bool isClosed) => this(isClosed: isClosed);

  @override
  WorkingHoursOverride intervals(List<TimeInterval>? intervals) =>
      this(intervals: intervals);

  @override
  WorkingHoursOverride note(String? note) => this(note: note);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `WorkingHoursOverride(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// WorkingHoursOverride(...).copyWith(id: 12, name: "My name")
  /// ````
  WorkingHoursOverride call({
    Object? id = const $CopyWithPlaceholder(),
    Object? date = const $CopyWithPlaceholder(),
    Object? kind = const $CopyWithPlaceholder(),
    Object? isClosed = const $CopyWithPlaceholder(),
    Object? intervals = const $CopyWithPlaceholder(),
    Object? note = const $CopyWithPlaceholder(),
  }) {
    return WorkingHoursOverride(
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as String,
      date: date == const $CopyWithPlaceholder()
          ? _value.date
          // ignore: cast_nullable_to_non_nullable
          : date as DateTime,
      kind: kind == const $CopyWithPlaceholder()
          ? _value.kind
          // ignore: cast_nullable_to_non_nullable
          : kind as WorkingHoursOverrideKindEnum,
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

extension $WorkingHoursOverrideCopyWith on WorkingHoursOverride {
  /// Returns a callable class that can be used as follows: `instanceOfWorkingHoursOverride.copyWith(...)` or like so:`instanceOfWorkingHoursOverride.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$WorkingHoursOverrideCWProxy get copyWith =>
      _$WorkingHoursOverrideCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

WorkingHoursOverride _$WorkingHoursOverrideFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('WorkingHoursOverride', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['id', 'date', 'kind', 'isClosed']);
  final val = WorkingHoursOverride(
    id: $checkedConvert('id', (v) => v as String),
    date: $checkedConvert(
      'date',
      (v) => const DateOnlyConverter().fromJson(v as String),
    ),
    kind: $checkedConvert(
      'kind',
      (v) => $enumDecode(
        _$WorkingHoursOverrideKindEnumEnumMap,
        v,
        unknownValue: WorkingHoursOverrideKindEnum.unknownDefaultOpenApi,
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

Map<String, dynamic> _$WorkingHoursOverrideToJson(
  WorkingHoursOverride instance,
) => <String, dynamic>{
  'id': instance.id,
  'date': const DateOnlyConverter().toJson(instance.date),
  'kind': _$WorkingHoursOverrideKindEnumEnumMap[instance.kind]!,
  'isClosed': instance.isClosed,
  'intervals': ?instance.intervals?.map((e) => e.toJson()).toList(),
  'note': ?instance.note,
};

const _$WorkingHoursOverrideKindEnumEnumMap = {
  WorkingHoursOverrideKindEnum.holiday: 'Holiday',
  WorkingHoursOverrideKindEnum.closure: 'Closure',
  WorkingHoursOverrideKindEnum.extraOpening: 'ExtraOpening',
  WorkingHoursOverrideKindEnum.shortDay: 'ShortDay',
  WorkingHoursOverrideKindEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};
