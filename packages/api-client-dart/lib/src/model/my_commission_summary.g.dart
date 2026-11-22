// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'my_commission_summary.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$MyCommissionSummaryCWProxy {
  MyCommissionSummary staffId(String staffId);

  MyCommissionSummary staffName(String staffName);

  MyCommissionSummary timeZone(String timeZone);

  MyCommissionSummary month(String month);

  MyCommissionSummary from(DateTime from);

  MyCommissionSummary to(DateTime to);

  MyCommissionSummary totalMinor(int totalMinor);

  MyCommissionSummary serviceRevenueMinor(int serviceRevenueMinor);

  MyCommissionSummary currency(MyCommissionSummaryCurrencyEnum currency);

  MyCommissionSummary appointmentCount(int appointmentCount);

  MyCommissionSummary noShowPercent(num? noShowPercent);

  MyCommissionSummary averageRating(num? averageRating);

  MyCommissionSummary daily(List<CommissionDayAmount> daily);

  MyCommissionSummary thisWeek(List<CommissionWeekDay> thisWeek);

  MyCommissionSummary entries(List<MyCommissionEntry> entries);

  MyCommissionSummary nextAccrual(MyCommissionSummaryNextAccrual? nextAccrual);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MyCommissionSummary(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MyCommissionSummary(...).copyWith(id: 12, name: "My name")
  /// ````
  MyCommissionSummary call({
    String staffId,
    String staffName,
    String timeZone,
    String month,
    DateTime from,
    DateTime to,
    int totalMinor,
    int serviceRevenueMinor,
    MyCommissionSummaryCurrencyEnum currency,
    int appointmentCount,
    num? noShowPercent,
    num? averageRating,
    List<CommissionDayAmount> daily,
    List<CommissionWeekDay> thisWeek,
    List<MyCommissionEntry> entries,
    MyCommissionSummaryNextAccrual? nextAccrual,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfMyCommissionSummary.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfMyCommissionSummary.copyWith.fieldName(...)`
class _$MyCommissionSummaryCWProxyImpl implements _$MyCommissionSummaryCWProxy {
  const _$MyCommissionSummaryCWProxyImpl(this._value);

  final MyCommissionSummary _value;

  @override
  MyCommissionSummary staffId(String staffId) => this(staffId: staffId);

  @override
  MyCommissionSummary staffName(String staffName) => this(staffName: staffName);

  @override
  MyCommissionSummary timeZone(String timeZone) => this(timeZone: timeZone);

  @override
  MyCommissionSummary month(String month) => this(month: month);

  @override
  MyCommissionSummary from(DateTime from) => this(from: from);

  @override
  MyCommissionSummary to(DateTime to) => this(to: to);

  @override
  MyCommissionSummary totalMinor(int totalMinor) =>
      this(totalMinor: totalMinor);

  @override
  MyCommissionSummary serviceRevenueMinor(int serviceRevenueMinor) =>
      this(serviceRevenueMinor: serviceRevenueMinor);

  @override
  MyCommissionSummary currency(MyCommissionSummaryCurrencyEnum currency) =>
      this(currency: currency);

  @override
  MyCommissionSummary appointmentCount(int appointmentCount) =>
      this(appointmentCount: appointmentCount);

  @override
  MyCommissionSummary noShowPercent(num? noShowPercent) =>
      this(noShowPercent: noShowPercent);

  @override
  MyCommissionSummary averageRating(num? averageRating) =>
      this(averageRating: averageRating);

  @override
  MyCommissionSummary daily(List<CommissionDayAmount> daily) =>
      this(daily: daily);

  @override
  MyCommissionSummary thisWeek(List<CommissionWeekDay> thisWeek) =>
      this(thisWeek: thisWeek);

  @override
  MyCommissionSummary entries(List<MyCommissionEntry> entries) =>
      this(entries: entries);

  @override
  MyCommissionSummary nextAccrual(
    MyCommissionSummaryNextAccrual? nextAccrual,
  ) => this(nextAccrual: nextAccrual);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MyCommissionSummary(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MyCommissionSummary(...).copyWith(id: 12, name: "My name")
  /// ````
  MyCommissionSummary call({
    Object? staffId = const $CopyWithPlaceholder(),
    Object? staffName = const $CopyWithPlaceholder(),
    Object? timeZone = const $CopyWithPlaceholder(),
    Object? month = const $CopyWithPlaceholder(),
    Object? from = const $CopyWithPlaceholder(),
    Object? to = const $CopyWithPlaceholder(),
    Object? totalMinor = const $CopyWithPlaceholder(),
    Object? serviceRevenueMinor = const $CopyWithPlaceholder(),
    Object? currency = const $CopyWithPlaceholder(),
    Object? appointmentCount = const $CopyWithPlaceholder(),
    Object? noShowPercent = const $CopyWithPlaceholder(),
    Object? averageRating = const $CopyWithPlaceholder(),
    Object? daily = const $CopyWithPlaceholder(),
    Object? thisWeek = const $CopyWithPlaceholder(),
    Object? entries = const $CopyWithPlaceholder(),
    Object? nextAccrual = const $CopyWithPlaceholder(),
  }) {
    return MyCommissionSummary(
      staffId: staffId == const $CopyWithPlaceholder()
          ? _value.staffId
          // ignore: cast_nullable_to_non_nullable
          : staffId as String,
      staffName: staffName == const $CopyWithPlaceholder()
          ? _value.staffName
          // ignore: cast_nullable_to_non_nullable
          : staffName as String,
      timeZone: timeZone == const $CopyWithPlaceholder()
          ? _value.timeZone
          // ignore: cast_nullable_to_non_nullable
          : timeZone as String,
      month: month == const $CopyWithPlaceholder()
          ? _value.month
          // ignore: cast_nullable_to_non_nullable
          : month as String,
      from: from == const $CopyWithPlaceholder()
          ? _value.from
          // ignore: cast_nullable_to_non_nullable
          : from as DateTime,
      to: to == const $CopyWithPlaceholder()
          ? _value.to
          // ignore: cast_nullable_to_non_nullable
          : to as DateTime,
      totalMinor: totalMinor == const $CopyWithPlaceholder()
          ? _value.totalMinor
          // ignore: cast_nullable_to_non_nullable
          : totalMinor as int,
      serviceRevenueMinor: serviceRevenueMinor == const $CopyWithPlaceholder()
          ? _value.serviceRevenueMinor
          // ignore: cast_nullable_to_non_nullable
          : serviceRevenueMinor as int,
      currency: currency == const $CopyWithPlaceholder()
          ? _value.currency
          // ignore: cast_nullable_to_non_nullable
          : currency as MyCommissionSummaryCurrencyEnum,
      appointmentCount: appointmentCount == const $CopyWithPlaceholder()
          ? _value.appointmentCount
          // ignore: cast_nullable_to_non_nullable
          : appointmentCount as int,
      noShowPercent: noShowPercent == const $CopyWithPlaceholder()
          ? _value.noShowPercent
          // ignore: cast_nullable_to_non_nullable
          : noShowPercent as num?,
      averageRating: averageRating == const $CopyWithPlaceholder()
          ? _value.averageRating
          // ignore: cast_nullable_to_non_nullable
          : averageRating as num?,
      daily: daily == const $CopyWithPlaceholder()
          ? _value.daily
          // ignore: cast_nullable_to_non_nullable
          : daily as List<CommissionDayAmount>,
      thisWeek: thisWeek == const $CopyWithPlaceholder()
          ? _value.thisWeek
          // ignore: cast_nullable_to_non_nullable
          : thisWeek as List<CommissionWeekDay>,
      entries: entries == const $CopyWithPlaceholder()
          ? _value.entries
          // ignore: cast_nullable_to_non_nullable
          : entries as List<MyCommissionEntry>,
      nextAccrual: nextAccrual == const $CopyWithPlaceholder()
          ? _value.nextAccrual
          // ignore: cast_nullable_to_non_nullable
          : nextAccrual as MyCommissionSummaryNextAccrual?,
    );
  }
}

extension $MyCommissionSummaryCopyWith on MyCommissionSummary {
  /// Returns a callable class that can be used as follows: `instanceOfMyCommissionSummary.copyWith(...)` or like so:`instanceOfMyCommissionSummary.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$MyCommissionSummaryCWProxy get copyWith =>
      _$MyCommissionSummaryCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MyCommissionSummary _$MyCommissionSummaryFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('MyCommissionSummary', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const [
      'staffId',
      'staffName',
      'timeZone',
      'month',
      'from',
      'to',
      'totalMinor',
      'serviceRevenueMinor',
      'currency',
      'appointmentCount',
      'noShowPercent',
      'averageRating',
      'daily',
      'thisWeek',
      'entries',
      'nextAccrual',
    ],
  );
  final val = MyCommissionSummary(
    staffId: $checkedConvert('staffId', (v) => v as String),
    staffName: $checkedConvert('staffName', (v) => v as String),
    timeZone: $checkedConvert('timeZone', (v) => v as String),
    month: $checkedConvert('month', (v) => v as String),
    from: $checkedConvert(
      'from',
      (v) => const DateOnlyConverter().fromJson(v as String),
    ),
    to: $checkedConvert(
      'to',
      (v) => const DateOnlyConverter().fromJson(v as String),
    ),
    totalMinor: $checkedConvert('totalMinor', (v) => (v as num).toInt()),
    serviceRevenueMinor: $checkedConvert(
      'serviceRevenueMinor',
      (v) => (v as num).toInt(),
    ),
    currency: $checkedConvert(
      'currency',
      (v) => $enumDecode(
        _$MyCommissionSummaryCurrencyEnumEnumMap,
        v,
        unknownValue: MyCommissionSummaryCurrencyEnum.unknownDefaultOpenApi,
      ),
    ),
    appointmentCount: $checkedConvert(
      'appointmentCount',
      (v) => (v as num).toInt(),
    ),
    noShowPercent: $checkedConvert('noShowPercent', (v) => v as num?),
    averageRating: $checkedConvert('averageRating', (v) => v as num?),
    daily: $checkedConvert(
      'daily',
      (v) => (v as List<dynamic>)
          .map((e) => CommissionDayAmount.fromJson(e as Map<String, dynamic>))
          .toList(),
    ),
    thisWeek: $checkedConvert(
      'thisWeek',
      (v) => (v as List<dynamic>)
          .map((e) => CommissionWeekDay.fromJson(e as Map<String, dynamic>))
          .toList(),
    ),
    entries: $checkedConvert(
      'entries',
      (v) => (v as List<dynamic>)
          .map((e) => MyCommissionEntry.fromJson(e as Map<String, dynamic>))
          .toList(),
    ),
    nextAccrual: $checkedConvert(
      'nextAccrual',
      (v) => v == null
          ? null
          : MyCommissionSummaryNextAccrual.fromJson(v as Map<String, dynamic>),
    ),
  );
  return val;
});

Map<String, dynamic> _$MyCommissionSummaryToJson(
  MyCommissionSummary instance,
) => <String, dynamic>{
  'staffId': instance.staffId,
  'staffName': instance.staffName,
  'timeZone': instance.timeZone,
  'month': instance.month,
  'from': const DateOnlyConverter().toJson(instance.from),
  'to': const DateOnlyConverter().toJson(instance.to),
  'totalMinor': instance.totalMinor,
  'serviceRevenueMinor': instance.serviceRevenueMinor,
  'currency': _$MyCommissionSummaryCurrencyEnumEnumMap[instance.currency]!,
  'appointmentCount': instance.appointmentCount,
  'noShowPercent': instance.noShowPercent,
  'averageRating': instance.averageRating,
  'daily': instance.daily.map((e) => e.toJson()).toList(),
  'thisWeek': instance.thisWeek.map((e) => e.toJson()).toList(),
  'entries': instance.entries.map((e) => e.toJson()).toList(),
  'nextAccrual': instance.nextAccrual?.toJson(),
};

const _$MyCommissionSummaryCurrencyEnumEnumMap = {
  MyCommissionSummaryCurrencyEnum.TRY: 'TRY',
  MyCommissionSummaryCurrencyEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};
