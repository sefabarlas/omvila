// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'today_commission.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$TodayCommissionCWProxy {
  TodayCommission month(String month);

  TodayCommission totalMinor(int totalMinor);

  TodayCommission currency(TodayCommissionCurrencyEnum currency);

  TodayCommission daily(List<CommissionDayAmount> daily);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `TodayCommission(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// TodayCommission(...).copyWith(id: 12, name: "My name")
  /// ````
  TodayCommission call({
    String month,
    int totalMinor,
    TodayCommissionCurrencyEnum currency,
    List<CommissionDayAmount> daily,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfTodayCommission.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfTodayCommission.copyWith.fieldName(...)`
class _$TodayCommissionCWProxyImpl implements _$TodayCommissionCWProxy {
  const _$TodayCommissionCWProxyImpl(this._value);

  final TodayCommission _value;

  @override
  TodayCommission month(String month) => this(month: month);

  @override
  TodayCommission totalMinor(int totalMinor) => this(totalMinor: totalMinor);

  @override
  TodayCommission currency(TodayCommissionCurrencyEnum currency) =>
      this(currency: currency);

  @override
  TodayCommission daily(List<CommissionDayAmount> daily) => this(daily: daily);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `TodayCommission(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// TodayCommission(...).copyWith(id: 12, name: "My name")
  /// ````
  TodayCommission call({
    Object? month = const $CopyWithPlaceholder(),
    Object? totalMinor = const $CopyWithPlaceholder(),
    Object? currency = const $CopyWithPlaceholder(),
    Object? daily = const $CopyWithPlaceholder(),
  }) {
    return TodayCommission(
      month: month == const $CopyWithPlaceholder()
          ? _value.month
          // ignore: cast_nullable_to_non_nullable
          : month as String,
      totalMinor: totalMinor == const $CopyWithPlaceholder()
          ? _value.totalMinor
          // ignore: cast_nullable_to_non_nullable
          : totalMinor as int,
      currency: currency == const $CopyWithPlaceholder()
          ? _value.currency
          // ignore: cast_nullable_to_non_nullable
          : currency as TodayCommissionCurrencyEnum,
      daily: daily == const $CopyWithPlaceholder()
          ? _value.daily
          // ignore: cast_nullable_to_non_nullable
          : daily as List<CommissionDayAmount>,
    );
  }
}

extension $TodayCommissionCopyWith on TodayCommission {
  /// Returns a callable class that can be used as follows: `instanceOfTodayCommission.copyWith(...)` or like so:`instanceOfTodayCommission.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$TodayCommissionCWProxy get copyWith => _$TodayCommissionCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

TodayCommission _$TodayCommissionFromJson(Map<String, dynamic> json) =>
    $checkedCreate('TodayCommission', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const ['month', 'totalMinor', 'currency', 'daily'],
      );
      final val = TodayCommission(
        month: $checkedConvert('month', (v) => v as String),
        totalMinor: $checkedConvert('totalMinor', (v) => (v as num).toInt()),
        currency: $checkedConvert(
          'currency',
          (v) => $enumDecode(
            _$TodayCommissionCurrencyEnumEnumMap,
            v,
            unknownValue: TodayCommissionCurrencyEnum.unknownDefaultOpenApi,
          ),
        ),
        daily: $checkedConvert(
          'daily',
          (v) => (v as List<dynamic>)
              .map(
                (e) => CommissionDayAmount.fromJson(e as Map<String, dynamic>),
              )
              .toList(),
        ),
      );
      return val;
    });

Map<String, dynamic> _$TodayCommissionToJson(TodayCommission instance) =>
    <String, dynamic>{
      'month': instance.month,
      'totalMinor': instance.totalMinor,
      'currency': _$TodayCommissionCurrencyEnumEnumMap[instance.currency]!,
      'daily': instance.daily.map((e) => e.toJson()).toList(),
    };

const _$TodayCommissionCurrencyEnumEnumMap = {
  TodayCommissionCurrencyEnum.TRY: 'TRY',
  TodayCommissionCurrencyEnum.unknownDefaultOpenApi: 'unknown_default_open_api',
};
