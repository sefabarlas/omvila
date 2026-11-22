// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'report_overview_daily_collections_inner.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ReportOverviewDailyCollectionsInnerCWProxy {
  ReportOverviewDailyCollectionsInner date(DateTime date);

  ReportOverviewDailyCollectionsInner amountMinor(int amountMinor);

  ReportOverviewDailyCollectionsInner isClosed(bool isClosed);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ReportOverviewDailyCollectionsInner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ReportOverviewDailyCollectionsInner(...).copyWith(id: 12, name: "My name")
  /// ````
  ReportOverviewDailyCollectionsInner call({
    DateTime date,
    int amountMinor,
    bool isClosed,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfReportOverviewDailyCollectionsInner.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfReportOverviewDailyCollectionsInner.copyWith.fieldName(...)`
class _$ReportOverviewDailyCollectionsInnerCWProxyImpl
    implements _$ReportOverviewDailyCollectionsInnerCWProxy {
  const _$ReportOverviewDailyCollectionsInnerCWProxyImpl(this._value);

  final ReportOverviewDailyCollectionsInner _value;

  @override
  ReportOverviewDailyCollectionsInner date(DateTime date) => this(date: date);

  @override
  ReportOverviewDailyCollectionsInner amountMinor(int amountMinor) =>
      this(amountMinor: amountMinor);

  @override
  ReportOverviewDailyCollectionsInner isClosed(bool isClosed) =>
      this(isClosed: isClosed);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ReportOverviewDailyCollectionsInner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ReportOverviewDailyCollectionsInner(...).copyWith(id: 12, name: "My name")
  /// ````
  ReportOverviewDailyCollectionsInner call({
    Object? date = const $CopyWithPlaceholder(),
    Object? amountMinor = const $CopyWithPlaceholder(),
    Object? isClosed = const $CopyWithPlaceholder(),
  }) {
    return ReportOverviewDailyCollectionsInner(
      date: date == const $CopyWithPlaceholder()
          ? _value.date
          // ignore: cast_nullable_to_non_nullable
          : date as DateTime,
      amountMinor: amountMinor == const $CopyWithPlaceholder()
          ? _value.amountMinor
          // ignore: cast_nullable_to_non_nullable
          : amountMinor as int,
      isClosed: isClosed == const $CopyWithPlaceholder()
          ? _value.isClosed
          // ignore: cast_nullable_to_non_nullable
          : isClosed as bool,
    );
  }
}

extension $ReportOverviewDailyCollectionsInnerCopyWith
    on ReportOverviewDailyCollectionsInner {
  /// Returns a callable class that can be used as follows: `instanceOfReportOverviewDailyCollectionsInner.copyWith(...)` or like so:`instanceOfReportOverviewDailyCollectionsInner.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ReportOverviewDailyCollectionsInnerCWProxy get copyWith =>
      _$ReportOverviewDailyCollectionsInnerCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ReportOverviewDailyCollectionsInner
_$ReportOverviewDailyCollectionsInnerFromJson(Map<String, dynamic> json) =>
    $checkedCreate('ReportOverviewDailyCollectionsInner', json, (
      $checkedConvert,
    ) {
      $checkKeys(json, requiredKeys: const ['date', 'amountMinor', 'isClosed']);
      final val = ReportOverviewDailyCollectionsInner(
        date: $checkedConvert(
          'date',
          (v) => const DateOnlyConverter().fromJson(v as String),
        ),
        amountMinor: $checkedConvert('amountMinor', (v) => (v as num).toInt()),
        isClosed: $checkedConvert('isClosed', (v) => v as bool),
      );
      return val;
    });

Map<String, dynamic> _$ReportOverviewDailyCollectionsInnerToJson(
  ReportOverviewDailyCollectionsInner instance,
) => <String, dynamic>{
  'date': const DateOnlyConverter().toJson(instance.date),
  'amountMinor': instance.amountMinor,
  'isClosed': instance.isClosed,
};
