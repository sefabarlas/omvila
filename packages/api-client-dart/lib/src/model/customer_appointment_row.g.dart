// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'customer_appointment_row.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$CustomerAppointmentRowCWProxy {
  CustomerAppointmentRow appointmentId(String appointmentId);

  CustomerAppointmentRow branchId(String branchId);

  CustomerAppointmentRow startUtc(DateTime startUtc);

  CustomerAppointmentRow status(AppointmentStatus status);

  CustomerAppointmentRow displayStatus(DisplayStatus displayStatus);

  CustomerAppointmentRow serviceNames(List<String> serviceNames);

  CustomerAppointmentRow staffNames(List<String> staffNames);

  CustomerAppointmentRow total(Money total);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CustomerAppointmentRow(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CustomerAppointmentRow(...).copyWith(id: 12, name: "My name")
  /// ````
  CustomerAppointmentRow call({
    String appointmentId,
    String branchId,
    DateTime startUtc,
    AppointmentStatus status,
    DisplayStatus displayStatus,
    List<String> serviceNames,
    List<String> staffNames,
    Money total,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfCustomerAppointmentRow.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfCustomerAppointmentRow.copyWith.fieldName(...)`
class _$CustomerAppointmentRowCWProxyImpl
    implements _$CustomerAppointmentRowCWProxy {
  const _$CustomerAppointmentRowCWProxyImpl(this._value);

  final CustomerAppointmentRow _value;

  @override
  CustomerAppointmentRow appointmentId(String appointmentId) =>
      this(appointmentId: appointmentId);

  @override
  CustomerAppointmentRow branchId(String branchId) => this(branchId: branchId);

  @override
  CustomerAppointmentRow startUtc(DateTime startUtc) =>
      this(startUtc: startUtc);

  @override
  CustomerAppointmentRow status(AppointmentStatus status) =>
      this(status: status);

  @override
  CustomerAppointmentRow displayStatus(DisplayStatus displayStatus) =>
      this(displayStatus: displayStatus);

  @override
  CustomerAppointmentRow serviceNames(List<String> serviceNames) =>
      this(serviceNames: serviceNames);

  @override
  CustomerAppointmentRow staffNames(List<String> staffNames) =>
      this(staffNames: staffNames);

  @override
  CustomerAppointmentRow total(Money total) => this(total: total);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CustomerAppointmentRow(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CustomerAppointmentRow(...).copyWith(id: 12, name: "My name")
  /// ````
  CustomerAppointmentRow call({
    Object? appointmentId = const $CopyWithPlaceholder(),
    Object? branchId = const $CopyWithPlaceholder(),
    Object? startUtc = const $CopyWithPlaceholder(),
    Object? status = const $CopyWithPlaceholder(),
    Object? displayStatus = const $CopyWithPlaceholder(),
    Object? serviceNames = const $CopyWithPlaceholder(),
    Object? staffNames = const $CopyWithPlaceholder(),
    Object? total = const $CopyWithPlaceholder(),
  }) {
    return CustomerAppointmentRow(
      appointmentId: appointmentId == const $CopyWithPlaceholder()
          ? _value.appointmentId
          // ignore: cast_nullable_to_non_nullable
          : appointmentId as String,
      branchId: branchId == const $CopyWithPlaceholder()
          ? _value.branchId
          // ignore: cast_nullable_to_non_nullable
          : branchId as String,
      startUtc: startUtc == const $CopyWithPlaceholder()
          ? _value.startUtc
          // ignore: cast_nullable_to_non_nullable
          : startUtc as DateTime,
      status: status == const $CopyWithPlaceholder()
          ? _value.status
          // ignore: cast_nullable_to_non_nullable
          : status as AppointmentStatus,
      displayStatus: displayStatus == const $CopyWithPlaceholder()
          ? _value.displayStatus
          // ignore: cast_nullable_to_non_nullable
          : displayStatus as DisplayStatus,
      serviceNames: serviceNames == const $CopyWithPlaceholder()
          ? _value.serviceNames
          // ignore: cast_nullable_to_non_nullable
          : serviceNames as List<String>,
      staffNames: staffNames == const $CopyWithPlaceholder()
          ? _value.staffNames
          // ignore: cast_nullable_to_non_nullable
          : staffNames as List<String>,
      total: total == const $CopyWithPlaceholder()
          ? _value.total
          // ignore: cast_nullable_to_non_nullable
          : total as Money,
    );
  }
}

extension $CustomerAppointmentRowCopyWith on CustomerAppointmentRow {
  /// Returns a callable class that can be used as follows: `instanceOfCustomerAppointmentRow.copyWith(...)` or like so:`instanceOfCustomerAppointmentRow.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$CustomerAppointmentRowCWProxy get copyWith =>
      _$CustomerAppointmentRowCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CustomerAppointmentRow _$CustomerAppointmentRowFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('CustomerAppointmentRow', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const [
      'appointmentId',
      'branchId',
      'startUtc',
      'status',
      'displayStatus',
      'serviceNames',
      'staffNames',
      'total',
    ],
  );
  final val = CustomerAppointmentRow(
    appointmentId: $checkedConvert('appointmentId', (v) => v as String),
    branchId: $checkedConvert('branchId', (v) => v as String),
    startUtc: $checkedConvert('startUtc', (v) => DateTime.parse(v as String)),
    status: $checkedConvert(
      'status',
      (v) => $enumDecode(
        _$AppointmentStatusEnumMap,
        v,
        unknownValue: AppointmentStatus.unknownDefaultOpenApi,
      ),
    ),
    displayStatus: $checkedConvert(
      'displayStatus',
      (v) => $enumDecode(
        _$DisplayStatusEnumMap,
        v,
        unknownValue: DisplayStatus.unknownDefaultOpenApi,
      ),
    ),
    serviceNames: $checkedConvert(
      'serviceNames',
      (v) => (v as List<dynamic>).map((e) => e as String).toList(),
    ),
    staffNames: $checkedConvert(
      'staffNames',
      (v) => (v as List<dynamic>).map((e) => e as String).toList(),
    ),
    total: $checkedConvert(
      'total',
      (v) => Money.fromJson(v as Map<String, dynamic>),
    ),
  );
  return val;
});

Map<String, dynamic> _$CustomerAppointmentRowToJson(
  CustomerAppointmentRow instance,
) => <String, dynamic>{
  'appointmentId': instance.appointmentId,
  'branchId': instance.branchId,
  'startUtc': instance.startUtc.toIso8601String(),
  'status': _$AppointmentStatusEnumMap[instance.status]!,
  'displayStatus': _$DisplayStatusEnumMap[instance.displayStatus]!,
  'serviceNames': instance.serviceNames,
  'staffNames': instance.staffNames,
  'total': instance.total.toJson(),
};

const _$AppointmentStatusEnumMap = {
  AppointmentStatus.draft: 'Draft',
  AppointmentStatus.pendingApproval: 'PendingApproval',
  AppointmentStatus.confirmed: 'Confirmed',
  AppointmentStatus.checkedIn: 'CheckedIn',
  AppointmentStatus.completed: 'Completed',
  AppointmentStatus.cancelled: 'Cancelled',
  AppointmentStatus.noShow: 'NoShow',
  AppointmentStatus.unknownDefaultOpenApi: 'unknown_default_open_api',
};

const _$DisplayStatusEnumMap = {
  DisplayStatus.pendingApproval: 'PendingApproval',
  DisplayStatus.depositPending: 'DepositPending',
  DisplayStatus.confirmed: 'Confirmed',
  DisplayStatus.checkedIn: 'CheckedIn',
  DisplayStatus.completed: 'Completed',
  DisplayStatus.cancelled: 'Cancelled',
  DisplayStatus.noShow: 'NoShow',
  DisplayStatus.unknownDefaultOpenApi: 'unknown_default_open_api',
};
