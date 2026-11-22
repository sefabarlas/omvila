// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'commission_report_row.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$CommissionReportRowCWProxy {
  CommissionReportRow staffId(String staffId);

  CommissionReportRow staffName(String staffName);

  CommissionReportRow appointmentCount(int appointmentCount);

  CommissionReportRow serviceRevenueMinor(int serviceRevenueMinor);

  CommissionReportRow serviceCommissionPercent(num? serviceCommissionPercent);

  CommissionReportRow serviceCommissionMinor(int serviceCommissionMinor);

  CommissionReportRow saleCommissionMinor(int saleCommissionMinor);

  CommissionReportRow totalCommissionMinor(int totalCommissionMinor);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CommissionReportRow(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CommissionReportRow(...).copyWith(id: 12, name: "My name")
  /// ````
  CommissionReportRow call({
    String staffId,
    String staffName,
    int appointmentCount,
    int serviceRevenueMinor,
    num? serviceCommissionPercent,
    int serviceCommissionMinor,
    int saleCommissionMinor,
    int totalCommissionMinor,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfCommissionReportRow.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfCommissionReportRow.copyWith.fieldName(...)`
class _$CommissionReportRowCWProxyImpl implements _$CommissionReportRowCWProxy {
  const _$CommissionReportRowCWProxyImpl(this._value);

  final CommissionReportRow _value;

  @override
  CommissionReportRow staffId(String staffId) => this(staffId: staffId);

  @override
  CommissionReportRow staffName(String staffName) => this(staffName: staffName);

  @override
  CommissionReportRow appointmentCount(int appointmentCount) =>
      this(appointmentCount: appointmentCount);

  @override
  CommissionReportRow serviceRevenueMinor(int serviceRevenueMinor) =>
      this(serviceRevenueMinor: serviceRevenueMinor);

  @override
  CommissionReportRow serviceCommissionPercent(num? serviceCommissionPercent) =>
      this(serviceCommissionPercent: serviceCommissionPercent);

  @override
  CommissionReportRow serviceCommissionMinor(int serviceCommissionMinor) =>
      this(serviceCommissionMinor: serviceCommissionMinor);

  @override
  CommissionReportRow saleCommissionMinor(int saleCommissionMinor) =>
      this(saleCommissionMinor: saleCommissionMinor);

  @override
  CommissionReportRow totalCommissionMinor(int totalCommissionMinor) =>
      this(totalCommissionMinor: totalCommissionMinor);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CommissionReportRow(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CommissionReportRow(...).copyWith(id: 12, name: "My name")
  /// ````
  CommissionReportRow call({
    Object? staffId = const $CopyWithPlaceholder(),
    Object? staffName = const $CopyWithPlaceholder(),
    Object? appointmentCount = const $CopyWithPlaceholder(),
    Object? serviceRevenueMinor = const $CopyWithPlaceholder(),
    Object? serviceCommissionPercent = const $CopyWithPlaceholder(),
    Object? serviceCommissionMinor = const $CopyWithPlaceholder(),
    Object? saleCommissionMinor = const $CopyWithPlaceholder(),
    Object? totalCommissionMinor = const $CopyWithPlaceholder(),
  }) {
    return CommissionReportRow(
      staffId: staffId == const $CopyWithPlaceholder()
          ? _value.staffId
          // ignore: cast_nullable_to_non_nullable
          : staffId as String,
      staffName: staffName == const $CopyWithPlaceholder()
          ? _value.staffName
          // ignore: cast_nullable_to_non_nullable
          : staffName as String,
      appointmentCount: appointmentCount == const $CopyWithPlaceholder()
          ? _value.appointmentCount
          // ignore: cast_nullable_to_non_nullable
          : appointmentCount as int,
      serviceRevenueMinor: serviceRevenueMinor == const $CopyWithPlaceholder()
          ? _value.serviceRevenueMinor
          // ignore: cast_nullable_to_non_nullable
          : serviceRevenueMinor as int,
      serviceCommissionPercent:
          serviceCommissionPercent == const $CopyWithPlaceholder()
          ? _value.serviceCommissionPercent
          // ignore: cast_nullable_to_non_nullable
          : serviceCommissionPercent as num?,
      serviceCommissionMinor:
          serviceCommissionMinor == const $CopyWithPlaceholder()
          ? _value.serviceCommissionMinor
          // ignore: cast_nullable_to_non_nullable
          : serviceCommissionMinor as int,
      saleCommissionMinor: saleCommissionMinor == const $CopyWithPlaceholder()
          ? _value.saleCommissionMinor
          // ignore: cast_nullable_to_non_nullable
          : saleCommissionMinor as int,
      totalCommissionMinor: totalCommissionMinor == const $CopyWithPlaceholder()
          ? _value.totalCommissionMinor
          // ignore: cast_nullable_to_non_nullable
          : totalCommissionMinor as int,
    );
  }
}

extension $CommissionReportRowCopyWith on CommissionReportRow {
  /// Returns a callable class that can be used as follows: `instanceOfCommissionReportRow.copyWith(...)` or like so:`instanceOfCommissionReportRow.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$CommissionReportRowCWProxy get copyWith =>
      _$CommissionReportRowCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CommissionReportRow _$CommissionReportRowFromJson(Map<String, dynamic> json) =>
    $checkedCreate('CommissionReportRow', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'staffId',
          'staffName',
          'appointmentCount',
          'serviceRevenueMinor',
          'serviceCommissionPercent',
          'serviceCommissionMinor',
          'saleCommissionMinor',
          'totalCommissionMinor',
        ],
      );
      final val = CommissionReportRow(
        staffId: $checkedConvert('staffId', (v) => v as String),
        staffName: $checkedConvert('staffName', (v) => v as String),
        appointmentCount: $checkedConvert(
          'appointmentCount',
          (v) => (v as num).toInt(),
        ),
        serviceRevenueMinor: $checkedConvert(
          'serviceRevenueMinor',
          (v) => (v as num).toInt(),
        ),
        serviceCommissionPercent: $checkedConvert(
          'serviceCommissionPercent',
          (v) => v as num?,
        ),
        serviceCommissionMinor: $checkedConvert(
          'serviceCommissionMinor',
          (v) => (v as num).toInt(),
        ),
        saleCommissionMinor: $checkedConvert(
          'saleCommissionMinor',
          (v) => (v as num).toInt(),
        ),
        totalCommissionMinor: $checkedConvert(
          'totalCommissionMinor',
          (v) => (v as num).toInt(),
        ),
      );
      return val;
    });

Map<String, dynamic> _$CommissionReportRowToJson(
  CommissionReportRow instance,
) => <String, dynamic>{
  'staffId': instance.staffId,
  'staffName': instance.staffName,
  'appointmentCount': instance.appointmentCount,
  'serviceRevenueMinor': instance.serviceRevenueMinor,
  'serviceCommissionPercent': instance.serviceCommissionPercent,
  'serviceCommissionMinor': instance.serviceCommissionMinor,
  'saleCommissionMinor': instance.saleCommissionMinor,
  'totalCommissionMinor': instance.totalCommissionMinor,
};
