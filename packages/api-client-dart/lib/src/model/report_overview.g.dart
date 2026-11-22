// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'report_overview.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ReportOverviewCWProxy {
  ReportOverview branchId(String? branchId);

  ReportOverview timeZone(String timeZone);

  ReportOverview from(DateTime from);

  ReportOverview to(DateTime to);

  ReportOverview earnedRevenue(Money earnedRevenue);

  ReportOverview completedAppointmentCount(int completedAppointmentCount);

  ReportOverview earnedRevenueChangePercent(num? earnedRevenueChangePercent);

  ReportOverview collections(Money collections);

  ReportOverview noShow(ReportOverviewNoShow noShow);

  ReportOverview online(ReportOverviewOnline online);

  ReportOverview dailyCollections(
    List<ReportOverviewDailyCollectionsInner> dailyCollections,
  );

  ReportOverview currency(ReportOverviewCurrencyEnum currency);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ReportOverview(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ReportOverview(...).copyWith(id: 12, name: "My name")
  /// ````
  ReportOverview call({
    String? branchId,
    String timeZone,
    DateTime from,
    DateTime to,
    Money earnedRevenue,
    int completedAppointmentCount,
    num? earnedRevenueChangePercent,
    Money collections,
    ReportOverviewNoShow noShow,
    ReportOverviewOnline online,
    List<ReportOverviewDailyCollectionsInner> dailyCollections,
    ReportOverviewCurrencyEnum currency,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfReportOverview.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfReportOverview.copyWith.fieldName(...)`
class _$ReportOverviewCWProxyImpl implements _$ReportOverviewCWProxy {
  const _$ReportOverviewCWProxyImpl(this._value);

  final ReportOverview _value;

  @override
  ReportOverview branchId(String? branchId) => this(branchId: branchId);

  @override
  ReportOverview timeZone(String timeZone) => this(timeZone: timeZone);

  @override
  ReportOverview from(DateTime from) => this(from: from);

  @override
  ReportOverview to(DateTime to) => this(to: to);

  @override
  ReportOverview earnedRevenue(Money earnedRevenue) =>
      this(earnedRevenue: earnedRevenue);

  @override
  ReportOverview completedAppointmentCount(int completedAppointmentCount) =>
      this(completedAppointmentCount: completedAppointmentCount);

  @override
  ReportOverview earnedRevenueChangePercent(num? earnedRevenueChangePercent) =>
      this(earnedRevenueChangePercent: earnedRevenueChangePercent);

  @override
  ReportOverview collections(Money collections) =>
      this(collections: collections);

  @override
  ReportOverview noShow(ReportOverviewNoShow noShow) => this(noShow: noShow);

  @override
  ReportOverview online(ReportOverviewOnline online) => this(online: online);

  @override
  ReportOverview dailyCollections(
    List<ReportOverviewDailyCollectionsInner> dailyCollections,
  ) => this(dailyCollections: dailyCollections);

  @override
  ReportOverview currency(ReportOverviewCurrencyEnum currency) =>
      this(currency: currency);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ReportOverview(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ReportOverview(...).copyWith(id: 12, name: "My name")
  /// ````
  ReportOverview call({
    Object? branchId = const $CopyWithPlaceholder(),
    Object? timeZone = const $CopyWithPlaceholder(),
    Object? from = const $CopyWithPlaceholder(),
    Object? to = const $CopyWithPlaceholder(),
    Object? earnedRevenue = const $CopyWithPlaceholder(),
    Object? completedAppointmentCount = const $CopyWithPlaceholder(),
    Object? earnedRevenueChangePercent = const $CopyWithPlaceholder(),
    Object? collections = const $CopyWithPlaceholder(),
    Object? noShow = const $CopyWithPlaceholder(),
    Object? online = const $CopyWithPlaceholder(),
    Object? dailyCollections = const $CopyWithPlaceholder(),
    Object? currency = const $CopyWithPlaceholder(),
  }) {
    return ReportOverview(
      branchId: branchId == const $CopyWithPlaceholder()
          ? _value.branchId
          // ignore: cast_nullable_to_non_nullable
          : branchId as String?,
      timeZone: timeZone == const $CopyWithPlaceholder()
          ? _value.timeZone
          // ignore: cast_nullable_to_non_nullable
          : timeZone as String,
      from: from == const $CopyWithPlaceholder()
          ? _value.from
          // ignore: cast_nullable_to_non_nullable
          : from as DateTime,
      to: to == const $CopyWithPlaceholder()
          ? _value.to
          // ignore: cast_nullable_to_non_nullable
          : to as DateTime,
      earnedRevenue: earnedRevenue == const $CopyWithPlaceholder()
          ? _value.earnedRevenue
          // ignore: cast_nullable_to_non_nullable
          : earnedRevenue as Money,
      completedAppointmentCount:
          completedAppointmentCount == const $CopyWithPlaceholder()
          ? _value.completedAppointmentCount
          // ignore: cast_nullable_to_non_nullable
          : completedAppointmentCount as int,
      earnedRevenueChangePercent:
          earnedRevenueChangePercent == const $CopyWithPlaceholder()
          ? _value.earnedRevenueChangePercent
          // ignore: cast_nullable_to_non_nullable
          : earnedRevenueChangePercent as num?,
      collections: collections == const $CopyWithPlaceholder()
          ? _value.collections
          // ignore: cast_nullable_to_non_nullable
          : collections as Money,
      noShow: noShow == const $CopyWithPlaceholder()
          ? _value.noShow
          // ignore: cast_nullable_to_non_nullable
          : noShow as ReportOverviewNoShow,
      online: online == const $CopyWithPlaceholder()
          ? _value.online
          // ignore: cast_nullable_to_non_nullable
          : online as ReportOverviewOnline,
      dailyCollections: dailyCollections == const $CopyWithPlaceholder()
          ? _value.dailyCollections
          // ignore: cast_nullable_to_non_nullable
          : dailyCollections as List<ReportOverviewDailyCollectionsInner>,
      currency: currency == const $CopyWithPlaceholder()
          ? _value.currency
          // ignore: cast_nullable_to_non_nullable
          : currency as ReportOverviewCurrencyEnum,
    );
  }
}

extension $ReportOverviewCopyWith on ReportOverview {
  /// Returns a callable class that can be used as follows: `instanceOfReportOverview.copyWith(...)` or like so:`instanceOfReportOverview.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ReportOverviewCWProxy get copyWith => _$ReportOverviewCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ReportOverview _$ReportOverviewFromJson(Map<String, dynamic> json) =>
    $checkedCreate('ReportOverview', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'branchId',
          'timeZone',
          'from',
          'to',
          'earnedRevenue',
          'completedAppointmentCount',
          'earnedRevenueChangePercent',
          'collections',
          'noShow',
          'online',
          'dailyCollections',
          'currency',
        ],
      );
      final val = ReportOverview(
        branchId: $checkedConvert('branchId', (v) => v as String?),
        timeZone: $checkedConvert('timeZone', (v) => v as String),
        from: $checkedConvert(
          'from',
          (v) => const DateOnlyConverter().fromJson(v as String),
        ),
        to: $checkedConvert(
          'to',
          (v) => const DateOnlyConverter().fromJson(v as String),
        ),
        earnedRevenue: $checkedConvert(
          'earnedRevenue',
          (v) => Money.fromJson(v as Map<String, dynamic>),
        ),
        completedAppointmentCount: $checkedConvert(
          'completedAppointmentCount',
          (v) => (v as num).toInt(),
        ),
        earnedRevenueChangePercent: $checkedConvert(
          'earnedRevenueChangePercent',
          (v) => v as num?,
        ),
        collections: $checkedConvert(
          'collections',
          (v) => Money.fromJson(v as Map<String, dynamic>),
        ),
        noShow: $checkedConvert(
          'noShow',
          (v) => ReportOverviewNoShow.fromJson(v as Map<String, dynamic>),
        ),
        online: $checkedConvert(
          'online',
          (v) => ReportOverviewOnline.fromJson(v as Map<String, dynamic>),
        ),
        dailyCollections: $checkedConvert(
          'dailyCollections',
          (v) => (v as List<dynamic>)
              .map(
                (e) => ReportOverviewDailyCollectionsInner.fromJson(
                  e as Map<String, dynamic>,
                ),
              )
              .toList(),
        ),
        currency: $checkedConvert(
          'currency',
          (v) => $enumDecode(
            _$ReportOverviewCurrencyEnumEnumMap,
            v,
            unknownValue: ReportOverviewCurrencyEnum.unknownDefaultOpenApi,
          ),
        ),
      );
      return val;
    });

Map<String, dynamic> _$ReportOverviewToJson(
  ReportOverview instance,
) => <String, dynamic>{
  'branchId': instance.branchId,
  'timeZone': instance.timeZone,
  'from': const DateOnlyConverter().toJson(instance.from),
  'to': const DateOnlyConverter().toJson(instance.to),
  'earnedRevenue': instance.earnedRevenue.toJson(),
  'completedAppointmentCount': instance.completedAppointmentCount,
  'earnedRevenueChangePercent': instance.earnedRevenueChangePercent,
  'collections': instance.collections.toJson(),
  'noShow': instance.noShow.toJson(),
  'online': instance.online.toJson(),
  'dailyCollections': instance.dailyCollections.map((e) => e.toJson()).toList(),
  'currency': _$ReportOverviewCurrencyEnumEnumMap[instance.currency]!,
};

const _$ReportOverviewCurrencyEnumEnumMap = {
  ReportOverviewCurrencyEnum.TRY: 'TRY',
  ReportOverviewCurrencyEnum.unknownDefaultOpenApi: 'unknown_default_open_api',
};
