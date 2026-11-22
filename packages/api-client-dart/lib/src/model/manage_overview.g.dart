// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'manage_overview.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ManageOverviewCWProxy {
  ManageOverview branchId(String branchId);

  ManageOverview branchName(String branchName);

  ManageOverview timeZone(String timeZone);

  ManageOverview date(DateTime date);

  ManageOverview generatedUtc(DateTime generatedUtc);

  ManageOverview todayRevenue(Money todayRevenue);

  ManageOverview todayAppointmentCount(int todayAppointmentCount);

  ManageOverview pendingApproval(
    ManageOverviewPendingApproval? pendingApproval,
  );

  ManageOverview walkIns(ManageOverviewWalkIns? walkIns);

  ManageOverview weeklyOccupancy(ManageOverviewWeeklyOccupancy weeklyOccupancy);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageOverview(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageOverview(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageOverview call({
    String branchId,
    String branchName,
    String timeZone,
    DateTime date,
    DateTime generatedUtc,
    Money todayRevenue,
    int todayAppointmentCount,
    ManageOverviewPendingApproval? pendingApproval,
    ManageOverviewWalkIns? walkIns,
    ManageOverviewWeeklyOccupancy weeklyOccupancy,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfManageOverview.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfManageOverview.copyWith.fieldName(...)`
class _$ManageOverviewCWProxyImpl implements _$ManageOverviewCWProxy {
  const _$ManageOverviewCWProxyImpl(this._value);

  final ManageOverview _value;

  @override
  ManageOverview branchId(String branchId) => this(branchId: branchId);

  @override
  ManageOverview branchName(String branchName) => this(branchName: branchName);

  @override
  ManageOverview timeZone(String timeZone) => this(timeZone: timeZone);

  @override
  ManageOverview date(DateTime date) => this(date: date);

  @override
  ManageOverview generatedUtc(DateTime generatedUtc) =>
      this(generatedUtc: generatedUtc);

  @override
  ManageOverview todayRevenue(Money todayRevenue) =>
      this(todayRevenue: todayRevenue);

  @override
  ManageOverview todayAppointmentCount(int todayAppointmentCount) =>
      this(todayAppointmentCount: todayAppointmentCount);

  @override
  ManageOverview pendingApproval(
    ManageOverviewPendingApproval? pendingApproval,
  ) => this(pendingApproval: pendingApproval);

  @override
  ManageOverview walkIns(ManageOverviewWalkIns? walkIns) =>
      this(walkIns: walkIns);

  @override
  ManageOverview weeklyOccupancy(
    ManageOverviewWeeklyOccupancy weeklyOccupancy,
  ) => this(weeklyOccupancy: weeklyOccupancy);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageOverview(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageOverview(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageOverview call({
    Object? branchId = const $CopyWithPlaceholder(),
    Object? branchName = const $CopyWithPlaceholder(),
    Object? timeZone = const $CopyWithPlaceholder(),
    Object? date = const $CopyWithPlaceholder(),
    Object? generatedUtc = const $CopyWithPlaceholder(),
    Object? todayRevenue = const $CopyWithPlaceholder(),
    Object? todayAppointmentCount = const $CopyWithPlaceholder(),
    Object? pendingApproval = const $CopyWithPlaceholder(),
    Object? walkIns = const $CopyWithPlaceholder(),
    Object? weeklyOccupancy = const $CopyWithPlaceholder(),
  }) {
    return ManageOverview(
      branchId: branchId == const $CopyWithPlaceholder()
          ? _value.branchId
          // ignore: cast_nullable_to_non_nullable
          : branchId as String,
      branchName: branchName == const $CopyWithPlaceholder()
          ? _value.branchName
          // ignore: cast_nullable_to_non_nullable
          : branchName as String,
      timeZone: timeZone == const $CopyWithPlaceholder()
          ? _value.timeZone
          // ignore: cast_nullable_to_non_nullable
          : timeZone as String,
      date: date == const $CopyWithPlaceholder()
          ? _value.date
          // ignore: cast_nullable_to_non_nullable
          : date as DateTime,
      generatedUtc: generatedUtc == const $CopyWithPlaceholder()
          ? _value.generatedUtc
          // ignore: cast_nullable_to_non_nullable
          : generatedUtc as DateTime,
      todayRevenue: todayRevenue == const $CopyWithPlaceholder()
          ? _value.todayRevenue
          // ignore: cast_nullable_to_non_nullable
          : todayRevenue as Money,
      todayAppointmentCount:
          todayAppointmentCount == const $CopyWithPlaceholder()
          ? _value.todayAppointmentCount
          // ignore: cast_nullable_to_non_nullable
          : todayAppointmentCount as int,
      pendingApproval: pendingApproval == const $CopyWithPlaceholder()
          ? _value.pendingApproval
          // ignore: cast_nullable_to_non_nullable
          : pendingApproval as ManageOverviewPendingApproval?,
      walkIns: walkIns == const $CopyWithPlaceholder()
          ? _value.walkIns
          // ignore: cast_nullable_to_non_nullable
          : walkIns as ManageOverviewWalkIns?,
      weeklyOccupancy: weeklyOccupancy == const $CopyWithPlaceholder()
          ? _value.weeklyOccupancy
          // ignore: cast_nullable_to_non_nullable
          : weeklyOccupancy as ManageOverviewWeeklyOccupancy,
    );
  }
}

extension $ManageOverviewCopyWith on ManageOverview {
  /// Returns a callable class that can be used as follows: `instanceOfManageOverview.copyWith(...)` or like so:`instanceOfManageOverview.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ManageOverviewCWProxy get copyWith => _$ManageOverviewCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ManageOverview _$ManageOverviewFromJson(Map<String, dynamic> json) =>
    $checkedCreate('ManageOverview', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'branchId',
          'branchName',
          'timeZone',
          'date',
          'generatedUtc',
          'todayRevenue',
          'todayAppointmentCount',
          'pendingApproval',
          'walkIns',
          'weeklyOccupancy',
        ],
      );
      final val = ManageOverview(
        branchId: $checkedConvert('branchId', (v) => v as String),
        branchName: $checkedConvert('branchName', (v) => v as String),
        timeZone: $checkedConvert('timeZone', (v) => v as String),
        date: $checkedConvert(
          'date',
          (v) => const DateOnlyConverter().fromJson(v as String),
        ),
        generatedUtc: $checkedConvert(
          'generatedUtc',
          (v) => DateTime.parse(v as String),
        ),
        todayRevenue: $checkedConvert(
          'todayRevenue',
          (v) => Money.fromJson(v as Map<String, dynamic>),
        ),
        todayAppointmentCount: $checkedConvert(
          'todayAppointmentCount',
          (v) => (v as num).toInt(),
        ),
        pendingApproval: $checkedConvert(
          'pendingApproval',
          (v) => v == null
              ? null
              : ManageOverviewPendingApproval.fromJson(
                  v as Map<String, dynamic>,
                ),
        ),
        walkIns: $checkedConvert(
          'walkIns',
          (v) => v == null
              ? null
              : ManageOverviewWalkIns.fromJson(v as Map<String, dynamic>),
        ),
        weeklyOccupancy: $checkedConvert(
          'weeklyOccupancy',
          (v) =>
              ManageOverviewWeeklyOccupancy.fromJson(v as Map<String, dynamic>),
        ),
      );
      return val;
    });

Map<String, dynamic> _$ManageOverviewToJson(ManageOverview instance) =>
    <String, dynamic>{
      'branchId': instance.branchId,
      'branchName': instance.branchName,
      'timeZone': instance.timeZone,
      'date': const DateOnlyConverter().toJson(instance.date),
      'generatedUtc': instance.generatedUtc.toIso8601String(),
      'todayRevenue': instance.todayRevenue.toJson(),
      'todayAppointmentCount': instance.todayAppointmentCount,
      'pendingApproval': instance.pendingApproval?.toJson(),
      'walkIns': instance.walkIns?.toJson(),
      'weeklyOccupancy': instance.weeklyOccupancy.toJson(),
    };
