// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'sms_overview_sent_this_month.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$SmsOverviewSentThisMonthCWProxy {
  SmsOverviewSentThisMonth month(DateTime month);

  SmsOverviewSentThisMonth smsCount(int smsCount);

  SmsOverviewSentThisMonth creditsUsed(int creditsUsed);

  SmsOverviewSentThisMonth notificationCount(int notificationCount);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `SmsOverviewSentThisMonth(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// SmsOverviewSentThisMonth(...).copyWith(id: 12, name: "My name")
  /// ````
  SmsOverviewSentThisMonth call({
    DateTime month,
    int smsCount,
    int creditsUsed,
    int notificationCount,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfSmsOverviewSentThisMonth.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfSmsOverviewSentThisMonth.copyWith.fieldName(...)`
class _$SmsOverviewSentThisMonthCWProxyImpl
    implements _$SmsOverviewSentThisMonthCWProxy {
  const _$SmsOverviewSentThisMonthCWProxyImpl(this._value);

  final SmsOverviewSentThisMonth _value;

  @override
  SmsOverviewSentThisMonth month(DateTime month) => this(month: month);

  @override
  SmsOverviewSentThisMonth smsCount(int smsCount) => this(smsCount: smsCount);

  @override
  SmsOverviewSentThisMonth creditsUsed(int creditsUsed) =>
      this(creditsUsed: creditsUsed);

  @override
  SmsOverviewSentThisMonth notificationCount(int notificationCount) =>
      this(notificationCount: notificationCount);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `SmsOverviewSentThisMonth(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// SmsOverviewSentThisMonth(...).copyWith(id: 12, name: "My name")
  /// ````
  SmsOverviewSentThisMonth call({
    Object? month = const $CopyWithPlaceholder(),
    Object? smsCount = const $CopyWithPlaceholder(),
    Object? creditsUsed = const $CopyWithPlaceholder(),
    Object? notificationCount = const $CopyWithPlaceholder(),
  }) {
    return SmsOverviewSentThisMonth(
      month: month == const $CopyWithPlaceholder()
          ? _value.month
          // ignore: cast_nullable_to_non_nullable
          : month as DateTime,
      smsCount: smsCount == const $CopyWithPlaceholder()
          ? _value.smsCount
          // ignore: cast_nullable_to_non_nullable
          : smsCount as int,
      creditsUsed: creditsUsed == const $CopyWithPlaceholder()
          ? _value.creditsUsed
          // ignore: cast_nullable_to_non_nullable
          : creditsUsed as int,
      notificationCount: notificationCount == const $CopyWithPlaceholder()
          ? _value.notificationCount
          // ignore: cast_nullable_to_non_nullable
          : notificationCount as int,
    );
  }
}

extension $SmsOverviewSentThisMonthCopyWith on SmsOverviewSentThisMonth {
  /// Returns a callable class that can be used as follows: `instanceOfSmsOverviewSentThisMonth.copyWith(...)` or like so:`instanceOfSmsOverviewSentThisMonth.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$SmsOverviewSentThisMonthCWProxy get copyWith =>
      _$SmsOverviewSentThisMonthCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

SmsOverviewSentThisMonth _$SmsOverviewSentThisMonthFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('SmsOverviewSentThisMonth', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const [
      'month',
      'smsCount',
      'creditsUsed',
      'notificationCount',
    ],
  );
  final val = SmsOverviewSentThisMonth(
    month: $checkedConvert(
      'month',
      (v) => const DateOnlyConverter().fromJson(v as String),
    ),
    smsCount: $checkedConvert('smsCount', (v) => (v as num).toInt()),
    creditsUsed: $checkedConvert('creditsUsed', (v) => (v as num).toInt()),
    notificationCount: $checkedConvert(
      'notificationCount',
      (v) => (v as num).toInt(),
    ),
  );
  return val;
});

Map<String, dynamic> _$SmsOverviewSentThisMonthToJson(
  SmsOverviewSentThisMonth instance,
) => <String, dynamic>{
  'month': const DateOnlyConverter().toJson(instance.month),
  'smsCount': instance.smsCount,
  'creditsUsed': instance.creditsUsed,
  'notificationCount': instance.notificationCount,
};
