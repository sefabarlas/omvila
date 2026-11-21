// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'walk_in_queue.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$WalkInQueueCWProxy {
  WalkInQueue branchId(String branchId);

  WalkInQueue date(DateTime date);

  WalkInQueue timeZone(String timeZone);

  WalkInQueue generatedUtc(DateTime generatedUtc);

  WalkInQueue items(List<WalkInTicketView> items);

  WalkInQueue waitingCount(int waitingCount);

  WalkInQueue oldestJoinedUtc(DateTime? oldestJoinedUtc);

  WalkInQueue estimatedWaitMinutesForNew(int? estimatedWaitMinutesForNew);

  WalkInQueue servedCount(int servedCount);

  WalkInQueue nextQueueNumber(int nextQueueNumber);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `WalkInQueue(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// WalkInQueue(...).copyWith(id: 12, name: "My name")
  /// ````
  WalkInQueue call({
    String branchId,
    DateTime date,
    String timeZone,
    DateTime generatedUtc,
    List<WalkInTicketView> items,
    int waitingCount,
    DateTime? oldestJoinedUtc,
    int? estimatedWaitMinutesForNew,
    int servedCount,
    int nextQueueNumber,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfWalkInQueue.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfWalkInQueue.copyWith.fieldName(...)`
class _$WalkInQueueCWProxyImpl implements _$WalkInQueueCWProxy {
  const _$WalkInQueueCWProxyImpl(this._value);

  final WalkInQueue _value;

  @override
  WalkInQueue branchId(String branchId) => this(branchId: branchId);

  @override
  WalkInQueue date(DateTime date) => this(date: date);

  @override
  WalkInQueue timeZone(String timeZone) => this(timeZone: timeZone);

  @override
  WalkInQueue generatedUtc(DateTime generatedUtc) =>
      this(generatedUtc: generatedUtc);

  @override
  WalkInQueue items(List<WalkInTicketView> items) => this(items: items);

  @override
  WalkInQueue waitingCount(int waitingCount) =>
      this(waitingCount: waitingCount);

  @override
  WalkInQueue oldestJoinedUtc(DateTime? oldestJoinedUtc) =>
      this(oldestJoinedUtc: oldestJoinedUtc);

  @override
  WalkInQueue estimatedWaitMinutesForNew(int? estimatedWaitMinutesForNew) =>
      this(estimatedWaitMinutesForNew: estimatedWaitMinutesForNew);

  @override
  WalkInQueue servedCount(int servedCount) => this(servedCount: servedCount);

  @override
  WalkInQueue nextQueueNumber(int nextQueueNumber) =>
      this(nextQueueNumber: nextQueueNumber);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `WalkInQueue(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// WalkInQueue(...).copyWith(id: 12, name: "My name")
  /// ````
  WalkInQueue call({
    Object? branchId = const $CopyWithPlaceholder(),
    Object? date = const $CopyWithPlaceholder(),
    Object? timeZone = const $CopyWithPlaceholder(),
    Object? generatedUtc = const $CopyWithPlaceholder(),
    Object? items = const $CopyWithPlaceholder(),
    Object? waitingCount = const $CopyWithPlaceholder(),
    Object? oldestJoinedUtc = const $CopyWithPlaceholder(),
    Object? estimatedWaitMinutesForNew = const $CopyWithPlaceholder(),
    Object? servedCount = const $CopyWithPlaceholder(),
    Object? nextQueueNumber = const $CopyWithPlaceholder(),
  }) {
    return WalkInQueue(
      branchId: branchId == const $CopyWithPlaceholder()
          ? _value.branchId
          // ignore: cast_nullable_to_non_nullable
          : branchId as String,
      date: date == const $CopyWithPlaceholder()
          ? _value.date
          // ignore: cast_nullable_to_non_nullable
          : date as DateTime,
      timeZone: timeZone == const $CopyWithPlaceholder()
          ? _value.timeZone
          // ignore: cast_nullable_to_non_nullable
          : timeZone as String,
      generatedUtc: generatedUtc == const $CopyWithPlaceholder()
          ? _value.generatedUtc
          // ignore: cast_nullable_to_non_nullable
          : generatedUtc as DateTime,
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<WalkInTicketView>,
      waitingCount: waitingCount == const $CopyWithPlaceholder()
          ? _value.waitingCount
          // ignore: cast_nullable_to_non_nullable
          : waitingCount as int,
      oldestJoinedUtc: oldestJoinedUtc == const $CopyWithPlaceholder()
          ? _value.oldestJoinedUtc
          // ignore: cast_nullable_to_non_nullable
          : oldestJoinedUtc as DateTime?,
      estimatedWaitMinutesForNew:
          estimatedWaitMinutesForNew == const $CopyWithPlaceholder()
          ? _value.estimatedWaitMinutesForNew
          // ignore: cast_nullable_to_non_nullable
          : estimatedWaitMinutesForNew as int?,
      servedCount: servedCount == const $CopyWithPlaceholder()
          ? _value.servedCount
          // ignore: cast_nullable_to_non_nullable
          : servedCount as int,
      nextQueueNumber: nextQueueNumber == const $CopyWithPlaceholder()
          ? _value.nextQueueNumber
          // ignore: cast_nullable_to_non_nullable
          : nextQueueNumber as int,
    );
  }
}

extension $WalkInQueueCopyWith on WalkInQueue {
  /// Returns a callable class that can be used as follows: `instanceOfWalkInQueue.copyWith(...)` or like so:`instanceOfWalkInQueue.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$WalkInQueueCWProxy get copyWith => _$WalkInQueueCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

WalkInQueue _$WalkInQueueFromJson(Map<String, dynamic> json) => $checkedCreate(
  'WalkInQueue',
  json,
  ($checkedConvert) {
    $checkKeys(
      json,
      requiredKeys: const [
        'branchId',
        'date',
        'timeZone',
        'generatedUtc',
        'items',
        'waitingCount',
        'servedCount',
        'nextQueueNumber',
      ],
    );
    final val = WalkInQueue(
      branchId: $checkedConvert('branchId', (v) => v as String),
      date: $checkedConvert('date', (v) => DateTime.parse(v as String)),
      timeZone: $checkedConvert('timeZone', (v) => v as String),
      generatedUtc: $checkedConvert(
        'generatedUtc',
        (v) => DateTime.parse(v as String),
      ),
      items: $checkedConvert(
        'items',
        (v) => (v as List<dynamic>)
            .map((e) => WalkInTicketView.fromJson(e as Map<String, dynamic>))
            .toList(),
      ),
      waitingCount: $checkedConvert('waitingCount', (v) => (v as num).toInt()),
      oldestJoinedUtc: $checkedConvert(
        'oldestJoinedUtc',
        (v) => v == null ? null : DateTime.parse(v as String),
      ),
      estimatedWaitMinutesForNew: $checkedConvert(
        'estimatedWaitMinutesForNew',
        (v) => (v as num?)?.toInt(),
      ),
      servedCount: $checkedConvert('servedCount', (v) => (v as num).toInt()),
      nextQueueNumber: $checkedConvert(
        'nextQueueNumber',
        (v) => (v as num).toInt(),
      ),
    );
    return val;
  },
);

Map<String, dynamic> _$WalkInQueueToJson(WalkInQueue instance) =>
    <String, dynamic>{
      'branchId': instance.branchId,
      'date': instance.date.toIso8601String(),
      'timeZone': instance.timeZone,
      'generatedUtc': instance.generatedUtc.toIso8601String(),
      'items': instance.items.map((e) => e.toJson()).toList(),
      'waitingCount': instance.waitingCount,
      'oldestJoinedUtc': ?instance.oldestJoinedUtc?.toIso8601String(),
      'estimatedWaitMinutesForNew': ?instance.estimatedWaitMinutesForNew,
      'servedCount': instance.servedCount,
      'nextQueueNumber': instance.nextQueueNumber,
    };
