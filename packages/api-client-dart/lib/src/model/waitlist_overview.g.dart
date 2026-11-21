// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'waitlist_overview.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$WaitlistOverviewCWProxy {
  WaitlistOverview waitingCount(int waitingCount);

  WaitlistOverview oldestJoinedUtc(DateTime? oldestJoinedUtc);

  WaitlistOverview convertedThisMonth(int convertedThisMonth);

  WaitlistOverview offerHoldMinutes(int offerHoldMinutes);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `WaitlistOverview(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// WaitlistOverview(...).copyWith(id: 12, name: "My name")
  /// ````
  WaitlistOverview call({
    int waitingCount,
    DateTime? oldestJoinedUtc,
    int convertedThisMonth,
    int offerHoldMinutes,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfWaitlistOverview.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfWaitlistOverview.copyWith.fieldName(...)`
class _$WaitlistOverviewCWProxyImpl implements _$WaitlistOverviewCWProxy {
  const _$WaitlistOverviewCWProxyImpl(this._value);

  final WaitlistOverview _value;

  @override
  WaitlistOverview waitingCount(int waitingCount) =>
      this(waitingCount: waitingCount);

  @override
  WaitlistOverview oldestJoinedUtc(DateTime? oldestJoinedUtc) =>
      this(oldestJoinedUtc: oldestJoinedUtc);

  @override
  WaitlistOverview convertedThisMonth(int convertedThisMonth) =>
      this(convertedThisMonth: convertedThisMonth);

  @override
  WaitlistOverview offerHoldMinutes(int offerHoldMinutes) =>
      this(offerHoldMinutes: offerHoldMinutes);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `WaitlistOverview(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// WaitlistOverview(...).copyWith(id: 12, name: "My name")
  /// ````
  WaitlistOverview call({
    Object? waitingCount = const $CopyWithPlaceholder(),
    Object? oldestJoinedUtc = const $CopyWithPlaceholder(),
    Object? convertedThisMonth = const $CopyWithPlaceholder(),
    Object? offerHoldMinutes = const $CopyWithPlaceholder(),
  }) {
    return WaitlistOverview(
      waitingCount: waitingCount == const $CopyWithPlaceholder()
          ? _value.waitingCount
          // ignore: cast_nullable_to_non_nullable
          : waitingCount as int,
      oldestJoinedUtc: oldestJoinedUtc == const $CopyWithPlaceholder()
          ? _value.oldestJoinedUtc
          // ignore: cast_nullable_to_non_nullable
          : oldestJoinedUtc as DateTime?,
      convertedThisMonth: convertedThisMonth == const $CopyWithPlaceholder()
          ? _value.convertedThisMonth
          // ignore: cast_nullable_to_non_nullable
          : convertedThisMonth as int,
      offerHoldMinutes: offerHoldMinutes == const $CopyWithPlaceholder()
          ? _value.offerHoldMinutes
          // ignore: cast_nullable_to_non_nullable
          : offerHoldMinutes as int,
    );
  }
}

extension $WaitlistOverviewCopyWith on WaitlistOverview {
  /// Returns a callable class that can be used as follows: `instanceOfWaitlistOverview.copyWith(...)` or like so:`instanceOfWaitlistOverview.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$WaitlistOverviewCWProxy get copyWith => _$WaitlistOverviewCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

WaitlistOverview _$WaitlistOverviewFromJson(Map<String, dynamic> json) =>
    $checkedCreate('WaitlistOverview', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'waitingCount',
          'convertedThisMonth',
          'offerHoldMinutes',
        ],
      );
      final val = WaitlistOverview(
        waitingCount: $checkedConvert(
          'waitingCount',
          (v) => (v as num).toInt(),
        ),
        oldestJoinedUtc: $checkedConvert(
          'oldestJoinedUtc',
          (v) => v == null ? null : DateTime.parse(v as String),
        ),
        convertedThisMonth: $checkedConvert(
          'convertedThisMonth',
          (v) => (v as num).toInt(),
        ),
        offerHoldMinutes: $checkedConvert(
          'offerHoldMinutes',
          (v) => (v as num).toInt(),
        ),
      );
      return val;
    });

Map<String, dynamic> _$WaitlistOverviewToJson(WaitlistOverview instance) =>
    <String, dynamic>{
      'waitingCount': instance.waitingCount,
      'oldestJoinedUtc': ?instance.oldestJoinedUtc?.toIso8601String(),
      'convertedThisMonth': instance.convertedThisMonth,
      'offerHoldMinutes': instance.offerHoldMinutes,
    };
