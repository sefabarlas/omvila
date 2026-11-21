// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'manage_today.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ManageTodayCWProxy {
  ManageToday branchId(String branchId);

  ManageToday branchName(String branchName);

  ManageToday timeZone(String timeZone);

  ManageToday date(DateTime date);

  ManageToday generatedUtc(DateTime generatedUtc);

  ManageToday next(TodayAppointment? next);

  ManageToday pendingApprovalCount(int? pendingApprovalCount);

  ManageToday walkIns(TodayWalkInSummary? walkIns);

  ManageToday commission(TodayCommission? commission);

  ManageToday completedCount(int completedCount);

  ManageToday totalCount(int totalCount);

  ManageToday items(List<TodayAppointment> items);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageToday(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageToday(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageToday call({
    String branchId,
    String branchName,
    String timeZone,
    DateTime date,
    DateTime generatedUtc,
    TodayAppointment? next,
    int? pendingApprovalCount,
    TodayWalkInSummary? walkIns,
    TodayCommission? commission,
    int completedCount,
    int totalCount,
    List<TodayAppointment> items,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfManageToday.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfManageToday.copyWith.fieldName(...)`
class _$ManageTodayCWProxyImpl implements _$ManageTodayCWProxy {
  const _$ManageTodayCWProxyImpl(this._value);

  final ManageToday _value;

  @override
  ManageToday branchId(String branchId) => this(branchId: branchId);

  @override
  ManageToday branchName(String branchName) => this(branchName: branchName);

  @override
  ManageToday timeZone(String timeZone) => this(timeZone: timeZone);

  @override
  ManageToday date(DateTime date) => this(date: date);

  @override
  ManageToday generatedUtc(DateTime generatedUtc) =>
      this(generatedUtc: generatedUtc);

  @override
  ManageToday next(TodayAppointment? next) => this(next: next);

  @override
  ManageToday pendingApprovalCount(int? pendingApprovalCount) =>
      this(pendingApprovalCount: pendingApprovalCount);

  @override
  ManageToday walkIns(TodayWalkInSummary? walkIns) => this(walkIns: walkIns);

  @override
  ManageToday commission(TodayCommission? commission) =>
      this(commission: commission);

  @override
  ManageToday completedCount(int completedCount) =>
      this(completedCount: completedCount);

  @override
  ManageToday totalCount(int totalCount) => this(totalCount: totalCount);

  @override
  ManageToday items(List<TodayAppointment> items) => this(items: items);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageToday(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageToday(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageToday call({
    Object? branchId = const $CopyWithPlaceholder(),
    Object? branchName = const $CopyWithPlaceholder(),
    Object? timeZone = const $CopyWithPlaceholder(),
    Object? date = const $CopyWithPlaceholder(),
    Object? generatedUtc = const $CopyWithPlaceholder(),
    Object? next = const $CopyWithPlaceholder(),
    Object? pendingApprovalCount = const $CopyWithPlaceholder(),
    Object? walkIns = const $CopyWithPlaceholder(),
    Object? commission = const $CopyWithPlaceholder(),
    Object? completedCount = const $CopyWithPlaceholder(),
    Object? totalCount = const $CopyWithPlaceholder(),
    Object? items = const $CopyWithPlaceholder(),
  }) {
    return ManageToday(
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
      next: next == const $CopyWithPlaceholder()
          ? _value.next
          // ignore: cast_nullable_to_non_nullable
          : next as TodayAppointment?,
      pendingApprovalCount: pendingApprovalCount == const $CopyWithPlaceholder()
          ? _value.pendingApprovalCount
          // ignore: cast_nullable_to_non_nullable
          : pendingApprovalCount as int?,
      walkIns: walkIns == const $CopyWithPlaceholder()
          ? _value.walkIns
          // ignore: cast_nullable_to_non_nullable
          : walkIns as TodayWalkInSummary?,
      commission: commission == const $CopyWithPlaceholder()
          ? _value.commission
          // ignore: cast_nullable_to_non_nullable
          : commission as TodayCommission?,
      completedCount: completedCount == const $CopyWithPlaceholder()
          ? _value.completedCount
          // ignore: cast_nullable_to_non_nullable
          : completedCount as int,
      totalCount: totalCount == const $CopyWithPlaceholder()
          ? _value.totalCount
          // ignore: cast_nullable_to_non_nullable
          : totalCount as int,
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<TodayAppointment>,
    );
  }
}

extension $ManageTodayCopyWith on ManageToday {
  /// Returns a callable class that can be used as follows: `instanceOfManageToday.copyWith(...)` or like so:`instanceOfManageToday.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ManageTodayCWProxy get copyWith => _$ManageTodayCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ManageToday _$ManageTodayFromJson(Map<String, dynamic> json) =>
    $checkedCreate('ManageToday', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'branchId',
          'branchName',
          'timeZone',
          'date',
          'generatedUtc',
          'next',
          'pendingApprovalCount',
          'walkIns',
          'commission',
          'completedCount',
          'totalCount',
          'items',
        ],
      );
      final val = ManageToday(
        branchId: $checkedConvert('branchId', (v) => v as String),
        branchName: $checkedConvert('branchName', (v) => v as String),
        timeZone: $checkedConvert('timeZone', (v) => v as String),
        date: $checkedConvert('date', (v) => DateTime.parse(v as String)),
        generatedUtc: $checkedConvert(
          'generatedUtc',
          (v) => DateTime.parse(v as String),
        ),
        next: $checkedConvert(
          'next',
          (v) => v == null
              ? null
              : TodayAppointment.fromJson(v as Map<String, dynamic>),
        ),
        pendingApprovalCount: $checkedConvert(
          'pendingApprovalCount',
          (v) => (v as num?)?.toInt(),
        ),
        walkIns: $checkedConvert(
          'walkIns',
          (v) => v == null
              ? null
              : TodayWalkInSummary.fromJson(v as Map<String, dynamic>),
        ),
        commission: $checkedConvert(
          'commission',
          (v) => v == null
              ? null
              : TodayCommission.fromJson(v as Map<String, dynamic>),
        ),
        completedCount: $checkedConvert(
          'completedCount',
          (v) => (v as num).toInt(),
        ),
        totalCount: $checkedConvert('totalCount', (v) => (v as num).toInt()),
        items: $checkedConvert(
          'items',
          (v) => (v as List<dynamic>)
              .map((e) => TodayAppointment.fromJson(e as Map<String, dynamic>))
              .toList(),
        ),
      );
      return val;
    });

Map<String, dynamic> _$ManageTodayToJson(ManageToday instance) =>
    <String, dynamic>{
      'branchId': instance.branchId,
      'branchName': instance.branchName,
      'timeZone': instance.timeZone,
      'date': instance.date.toIso8601String(),
      'generatedUtc': instance.generatedUtc.toIso8601String(),
      'next': instance.next?.toJson(),
      'pendingApprovalCount': instance.pendingApprovalCount,
      'walkIns': instance.walkIns?.toJson(),
      'commission': instance.commission?.toJson(),
      'completedCount': instance.completedCount,
      'totalCount': instance.totalCount,
      'items': instance.items.map((e) => e.toJson()).toList(),
    };
