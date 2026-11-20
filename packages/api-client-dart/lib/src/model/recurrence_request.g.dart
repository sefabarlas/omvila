// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'recurrence_request.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$RecurrenceRequestCWProxy {
  RecurrenceRequest frequency(RecurrenceRequestFrequencyEnum frequency);

  RecurrenceRequest intervalWeeks(
    RecurrenceRequestIntervalWeeksEnum intervalWeeks,
  );

  RecurrenceRequest endsOn(DateTime? endsOn);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `RecurrenceRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// RecurrenceRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  RecurrenceRequest call({
    RecurrenceRequestFrequencyEnum frequency,
    RecurrenceRequestIntervalWeeksEnum intervalWeeks,
    DateTime? endsOn,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfRecurrenceRequest.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfRecurrenceRequest.copyWith.fieldName(...)`
class _$RecurrenceRequestCWProxyImpl implements _$RecurrenceRequestCWProxy {
  const _$RecurrenceRequestCWProxyImpl(this._value);

  final RecurrenceRequest _value;

  @override
  RecurrenceRequest frequency(RecurrenceRequestFrequencyEnum frequency) =>
      this(frequency: frequency);

  @override
  RecurrenceRequest intervalWeeks(
    RecurrenceRequestIntervalWeeksEnum intervalWeeks,
  ) => this(intervalWeeks: intervalWeeks);

  @override
  RecurrenceRequest endsOn(DateTime? endsOn) => this(endsOn: endsOn);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `RecurrenceRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// RecurrenceRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  RecurrenceRequest call({
    Object? frequency = const $CopyWithPlaceholder(),
    Object? intervalWeeks = const $CopyWithPlaceholder(),
    Object? endsOn = const $CopyWithPlaceholder(),
  }) {
    return RecurrenceRequest(
      frequency: frequency == const $CopyWithPlaceholder()
          ? _value.frequency
          // ignore: cast_nullable_to_non_nullable
          : frequency as RecurrenceRequestFrequencyEnum,
      intervalWeeks: intervalWeeks == const $CopyWithPlaceholder()
          ? _value.intervalWeeks
          // ignore: cast_nullable_to_non_nullable
          : intervalWeeks as RecurrenceRequestIntervalWeeksEnum,
      endsOn: endsOn == const $CopyWithPlaceholder()
          ? _value.endsOn
          // ignore: cast_nullable_to_non_nullable
          : endsOn as DateTime?,
    );
  }
}

extension $RecurrenceRequestCopyWith on RecurrenceRequest {
  /// Returns a callable class that can be used as follows: `instanceOfRecurrenceRequest.copyWith(...)` or like so:`instanceOfRecurrenceRequest.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$RecurrenceRequestCWProxy get copyWith =>
      _$RecurrenceRequestCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

RecurrenceRequest _$RecurrenceRequestFromJson(Map<String, dynamic> json) =>
    $checkedCreate('RecurrenceRequest', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['frequency', 'intervalWeeks']);
      final val = RecurrenceRequest(
        frequency: $checkedConvert(
          'frequency',
          (v) => $enumDecode(
            _$RecurrenceRequestFrequencyEnumEnumMap,
            v,
            unknownValue: RecurrenceRequestFrequencyEnum.unknownDefaultOpenApi,
          ),
        ),
        intervalWeeks: $checkedConvert(
          'intervalWeeks',
          (v) => $enumDecode(
            _$RecurrenceRequestIntervalWeeksEnumEnumMap,
            v,
            unknownValue:
                RecurrenceRequestIntervalWeeksEnum.unknownDefaultOpenApi,
          ),
        ),
        endsOn: $checkedConvert(
          'endsOn',
          (v) => v == null ? null : DateTime.parse(v as String),
        ),
      );
      return val;
    });

Map<String, dynamic> _$RecurrenceRequestToJson(RecurrenceRequest instance) =>
    <String, dynamic>{
      'frequency': _$RecurrenceRequestFrequencyEnumEnumMap[instance.frequency]!,
      'intervalWeeks':
          _$RecurrenceRequestIntervalWeeksEnumEnumMap[instance.intervalWeeks]!,
      'endsOn': ?instance.endsOn?.toIso8601String(),
    };

const _$RecurrenceRequestFrequencyEnumEnumMap = {
  RecurrenceRequestFrequencyEnum.weekly: 'Weekly',
  RecurrenceRequestFrequencyEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};

const _$RecurrenceRequestIntervalWeeksEnumEnumMap = {
  RecurrenceRequestIntervalWeeksEnum.number1: 1,
  RecurrenceRequestIntervalWeeksEnum.number2: 2,
  RecurrenceRequestIntervalWeeksEnum.number4: 4,
  RecurrenceRequestIntervalWeeksEnum.unknownDefaultOpenApi: 11184809,
};
