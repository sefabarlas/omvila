// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'report_overview_no_show.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ReportOverviewNoShowCWProxy {
  ReportOverviewNoShow count(int count);

  ReportOverviewNoShow percent(num? percent);

  ReportOverviewNoShow lostRevenue(Money lostRevenue);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ReportOverviewNoShow(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ReportOverviewNoShow(...).copyWith(id: 12, name: "My name")
  /// ````
  ReportOverviewNoShow call({int count, num? percent, Money lostRevenue});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfReportOverviewNoShow.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfReportOverviewNoShow.copyWith.fieldName(...)`
class _$ReportOverviewNoShowCWProxyImpl
    implements _$ReportOverviewNoShowCWProxy {
  const _$ReportOverviewNoShowCWProxyImpl(this._value);

  final ReportOverviewNoShow _value;

  @override
  ReportOverviewNoShow count(int count) => this(count: count);

  @override
  ReportOverviewNoShow percent(num? percent) => this(percent: percent);

  @override
  ReportOverviewNoShow lostRevenue(Money lostRevenue) =>
      this(lostRevenue: lostRevenue);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ReportOverviewNoShow(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ReportOverviewNoShow(...).copyWith(id: 12, name: "My name")
  /// ````
  ReportOverviewNoShow call({
    Object? count = const $CopyWithPlaceholder(),
    Object? percent = const $CopyWithPlaceholder(),
    Object? lostRevenue = const $CopyWithPlaceholder(),
  }) {
    return ReportOverviewNoShow(
      count: count == const $CopyWithPlaceholder()
          ? _value.count
          // ignore: cast_nullable_to_non_nullable
          : count as int,
      percent: percent == const $CopyWithPlaceholder()
          ? _value.percent
          // ignore: cast_nullable_to_non_nullable
          : percent as num?,
      lostRevenue: lostRevenue == const $CopyWithPlaceholder()
          ? _value.lostRevenue
          // ignore: cast_nullable_to_non_nullable
          : lostRevenue as Money,
    );
  }
}

extension $ReportOverviewNoShowCopyWith on ReportOverviewNoShow {
  /// Returns a callable class that can be used as follows: `instanceOfReportOverviewNoShow.copyWith(...)` or like so:`instanceOfReportOverviewNoShow.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ReportOverviewNoShowCWProxy get copyWith =>
      _$ReportOverviewNoShowCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ReportOverviewNoShow _$ReportOverviewNoShowFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('ReportOverviewNoShow', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['count', 'percent', 'lostRevenue']);
  final val = ReportOverviewNoShow(
    count: $checkedConvert('count', (v) => (v as num).toInt()),
    percent: $checkedConvert('percent', (v) => v as num?),
    lostRevenue: $checkedConvert(
      'lostRevenue',
      (v) => Money.fromJson(v as Map<String, dynamic>),
    ),
  );
  return val;
});

Map<String, dynamic> _$ReportOverviewNoShowToJson(
  ReportOverviewNoShow instance,
) => <String, dynamic>{
  'count': instance.count,
  'percent': instance.percent,
  'lostRevenue': instance.lostRevenue.toJson(),
};
