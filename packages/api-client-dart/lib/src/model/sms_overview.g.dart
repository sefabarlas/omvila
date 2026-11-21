// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'sms_overview.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$SmsOverviewCWProxy {
  SmsOverview creditBalance(int creditBalance);

  SmsOverview estimatedDaysLeft(int? estimatedDaysLeft);

  SmsOverview lowCreditThreshold(int lowCreditThreshold);

  SmsOverview isLow(bool isLow);

  SmsOverview sentThisMonth(SmsOverviewSentThisMonth sentThisMonth);

  SmsOverview sender(SmsSender sender);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `SmsOverview(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// SmsOverview(...).copyWith(id: 12, name: "My name")
  /// ````
  SmsOverview call({
    int creditBalance,
    int? estimatedDaysLeft,
    int lowCreditThreshold,
    bool isLow,
    SmsOverviewSentThisMonth sentThisMonth,
    SmsSender sender,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfSmsOverview.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfSmsOverview.copyWith.fieldName(...)`
class _$SmsOverviewCWProxyImpl implements _$SmsOverviewCWProxy {
  const _$SmsOverviewCWProxyImpl(this._value);

  final SmsOverview _value;

  @override
  SmsOverview creditBalance(int creditBalance) =>
      this(creditBalance: creditBalance);

  @override
  SmsOverview estimatedDaysLeft(int? estimatedDaysLeft) =>
      this(estimatedDaysLeft: estimatedDaysLeft);

  @override
  SmsOverview lowCreditThreshold(int lowCreditThreshold) =>
      this(lowCreditThreshold: lowCreditThreshold);

  @override
  SmsOverview isLow(bool isLow) => this(isLow: isLow);

  @override
  SmsOverview sentThisMonth(SmsOverviewSentThisMonth sentThisMonth) =>
      this(sentThisMonth: sentThisMonth);

  @override
  SmsOverview sender(SmsSender sender) => this(sender: sender);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `SmsOverview(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// SmsOverview(...).copyWith(id: 12, name: "My name")
  /// ````
  SmsOverview call({
    Object? creditBalance = const $CopyWithPlaceholder(),
    Object? estimatedDaysLeft = const $CopyWithPlaceholder(),
    Object? lowCreditThreshold = const $CopyWithPlaceholder(),
    Object? isLow = const $CopyWithPlaceholder(),
    Object? sentThisMonth = const $CopyWithPlaceholder(),
    Object? sender = const $CopyWithPlaceholder(),
  }) {
    return SmsOverview(
      creditBalance: creditBalance == const $CopyWithPlaceholder()
          ? _value.creditBalance
          // ignore: cast_nullable_to_non_nullable
          : creditBalance as int,
      estimatedDaysLeft: estimatedDaysLeft == const $CopyWithPlaceholder()
          ? _value.estimatedDaysLeft
          // ignore: cast_nullable_to_non_nullable
          : estimatedDaysLeft as int?,
      lowCreditThreshold: lowCreditThreshold == const $CopyWithPlaceholder()
          ? _value.lowCreditThreshold
          // ignore: cast_nullable_to_non_nullable
          : lowCreditThreshold as int,
      isLow: isLow == const $CopyWithPlaceholder()
          ? _value.isLow
          // ignore: cast_nullable_to_non_nullable
          : isLow as bool,
      sentThisMonth: sentThisMonth == const $CopyWithPlaceholder()
          ? _value.sentThisMonth
          // ignore: cast_nullable_to_non_nullable
          : sentThisMonth as SmsOverviewSentThisMonth,
      sender: sender == const $CopyWithPlaceholder()
          ? _value.sender
          // ignore: cast_nullable_to_non_nullable
          : sender as SmsSender,
    );
  }
}

extension $SmsOverviewCopyWith on SmsOverview {
  /// Returns a callable class that can be used as follows: `instanceOfSmsOverview.copyWith(...)` or like so:`instanceOfSmsOverview.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$SmsOverviewCWProxy get copyWith => _$SmsOverviewCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

SmsOverview _$SmsOverviewFromJson(Map<String, dynamic> json) =>
    $checkedCreate('SmsOverview', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'creditBalance',
          'estimatedDaysLeft',
          'lowCreditThreshold',
          'isLow',
          'sentThisMonth',
          'sender',
        ],
      );
      final val = SmsOverview(
        creditBalance: $checkedConvert(
          'creditBalance',
          (v) => (v as num).toInt(),
        ),
        estimatedDaysLeft: $checkedConvert(
          'estimatedDaysLeft',
          (v) => (v as num?)?.toInt(),
        ),
        lowCreditThreshold: $checkedConvert(
          'lowCreditThreshold',
          (v) => (v as num).toInt(),
        ),
        isLow: $checkedConvert('isLow', (v) => v as bool),
        sentThisMonth: $checkedConvert(
          'sentThisMonth',
          (v) => SmsOverviewSentThisMonth.fromJson(v as Map<String, dynamic>),
        ),
        sender: $checkedConvert(
          'sender',
          (v) => SmsSender.fromJson(v as Map<String, dynamic>),
        ),
      );
      return val;
    });

Map<String, dynamic> _$SmsOverviewToJson(SmsOverview instance) =>
    <String, dynamic>{
      'creditBalance': instance.creditBalance,
      'estimatedDaysLeft': instance.estimatedDaysLeft,
      'lowCreditThreshold': instance.lowCreditThreshold,
      'isLow': instance.isLow,
      'sentThisMonth': instance.sentThisMonth.toJson(),
      'sender': instance.sender.toJson(),
    };
