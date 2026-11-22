// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'report_overview_online.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ReportOverviewOnlineCWProxy {
  ReportOverviewOnline appointmentCount(int appointmentCount);

  ReportOverviewOnline percent(num? percent);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ReportOverviewOnline(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ReportOverviewOnline(...).copyWith(id: 12, name: "My name")
  /// ````
  ReportOverviewOnline call({int appointmentCount, num? percent});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfReportOverviewOnline.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfReportOverviewOnline.copyWith.fieldName(...)`
class _$ReportOverviewOnlineCWProxyImpl
    implements _$ReportOverviewOnlineCWProxy {
  const _$ReportOverviewOnlineCWProxyImpl(this._value);

  final ReportOverviewOnline _value;

  @override
  ReportOverviewOnline appointmentCount(int appointmentCount) =>
      this(appointmentCount: appointmentCount);

  @override
  ReportOverviewOnline percent(num? percent) => this(percent: percent);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ReportOverviewOnline(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ReportOverviewOnline(...).copyWith(id: 12, name: "My name")
  /// ````
  ReportOverviewOnline call({
    Object? appointmentCount = const $CopyWithPlaceholder(),
    Object? percent = const $CopyWithPlaceholder(),
  }) {
    return ReportOverviewOnline(
      appointmentCount: appointmentCount == const $CopyWithPlaceholder()
          ? _value.appointmentCount
          // ignore: cast_nullable_to_non_nullable
          : appointmentCount as int,
      percent: percent == const $CopyWithPlaceholder()
          ? _value.percent
          // ignore: cast_nullable_to_non_nullable
          : percent as num?,
    );
  }
}

extension $ReportOverviewOnlineCopyWith on ReportOverviewOnline {
  /// Returns a callable class that can be used as follows: `instanceOfReportOverviewOnline.copyWith(...)` or like so:`instanceOfReportOverviewOnline.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ReportOverviewOnlineCWProxy get copyWith =>
      _$ReportOverviewOnlineCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ReportOverviewOnline _$ReportOverviewOnlineFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('ReportOverviewOnline', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['appointmentCount', 'percent']);
  final val = ReportOverviewOnline(
    appointmentCount: $checkedConvert(
      'appointmentCount',
      (v) => (v as num).toInt(),
    ),
    percent: $checkedConvert('percent', (v) => v as num?),
  );
  return val;
});

Map<String, dynamic> _$ReportOverviewOnlineToJson(
  ReportOverviewOnline instance,
) => <String, dynamic>{
  'appointmentCount': instance.appointmentCount,
  'percent': instance.percent,
};
