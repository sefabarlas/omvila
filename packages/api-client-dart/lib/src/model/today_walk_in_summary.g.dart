// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'today_walk_in_summary.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$TodayWalkInSummaryCWProxy {
  TodayWalkInSummary waitingCount(int waitingCount);

  TodayWalkInSummary estimatedWaitMinutesForNew(
    int? estimatedWaitMinutesForNew,
  );

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `TodayWalkInSummary(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// TodayWalkInSummary(...).copyWith(id: 12, name: "My name")
  /// ````
  TodayWalkInSummary call({int waitingCount, int? estimatedWaitMinutesForNew});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfTodayWalkInSummary.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfTodayWalkInSummary.copyWith.fieldName(...)`
class _$TodayWalkInSummaryCWProxyImpl implements _$TodayWalkInSummaryCWProxy {
  const _$TodayWalkInSummaryCWProxyImpl(this._value);

  final TodayWalkInSummary _value;

  @override
  TodayWalkInSummary waitingCount(int waitingCount) =>
      this(waitingCount: waitingCount);

  @override
  TodayWalkInSummary estimatedWaitMinutesForNew(
    int? estimatedWaitMinutesForNew,
  ) => this(estimatedWaitMinutesForNew: estimatedWaitMinutesForNew);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `TodayWalkInSummary(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// TodayWalkInSummary(...).copyWith(id: 12, name: "My name")
  /// ````
  TodayWalkInSummary call({
    Object? waitingCount = const $CopyWithPlaceholder(),
    Object? estimatedWaitMinutesForNew = const $CopyWithPlaceholder(),
  }) {
    return TodayWalkInSummary(
      waitingCount: waitingCount == const $CopyWithPlaceholder()
          ? _value.waitingCount
          // ignore: cast_nullable_to_non_nullable
          : waitingCount as int,
      estimatedWaitMinutesForNew:
          estimatedWaitMinutesForNew == const $CopyWithPlaceholder()
          ? _value.estimatedWaitMinutesForNew
          // ignore: cast_nullable_to_non_nullable
          : estimatedWaitMinutesForNew as int?,
    );
  }
}

extension $TodayWalkInSummaryCopyWith on TodayWalkInSummary {
  /// Returns a callable class that can be used as follows: `instanceOfTodayWalkInSummary.copyWith(...)` or like so:`instanceOfTodayWalkInSummary.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$TodayWalkInSummaryCWProxy get copyWith =>
      _$TodayWalkInSummaryCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

TodayWalkInSummary _$TodayWalkInSummaryFromJson(Map<String, dynamic> json) =>
    $checkedCreate('TodayWalkInSummary', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['waitingCount']);
      final val = TodayWalkInSummary(
        waitingCount: $checkedConvert(
          'waitingCount',
          (v) => (v as num).toInt(),
        ),
        estimatedWaitMinutesForNew: $checkedConvert(
          'estimatedWaitMinutesForNew',
          (v) => (v as num?)?.toInt(),
        ),
      );
      return val;
    });

Map<String, dynamic> _$TodayWalkInSummaryToJson(TodayWalkInSummary instance) =>
    <String, dynamic>{
      'waitingCount': instance.waitingCount,
      'estimatedWaitMinutesForNew': ?instance.estimatedWaitMinutesForNew,
    };
