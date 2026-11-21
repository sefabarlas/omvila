// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'today_appointment.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$TodayAppointmentCWProxy {
  TodayAppointment appointmentId(String appointmentId);

  TodayAppointment startUtc(DateTime startUtc);

  TodayAppointment endUtc(DateTime endUtc);

  TodayAppointment status(AppointmentStatus status);

  TodayAppointment displayStatus(DisplayStatus displayStatus);

  TodayAppointment customerName(String customerName);

  TodayAppointment isNewCustomer(bool isNewCustomer);

  TodayAppointment serviceSummary(String serviceSummary);

  TodayAppointment staffNames(List<String> staffNames);

  TodayAppointment holdExpiresUtc(DateTime? holdExpiresUtc);

  TodayAppointment depositStatus(DepositStatus? depositStatus);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `TodayAppointment(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// TodayAppointment(...).copyWith(id: 12, name: "My name")
  /// ````
  TodayAppointment call({
    String appointmentId,
    DateTime startUtc,
    DateTime endUtc,
    AppointmentStatus status,
    DisplayStatus displayStatus,
    String customerName,
    bool isNewCustomer,
    String serviceSummary,
    List<String> staffNames,
    DateTime? holdExpiresUtc,
    DepositStatus? depositStatus,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfTodayAppointment.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfTodayAppointment.copyWith.fieldName(...)`
class _$TodayAppointmentCWProxyImpl implements _$TodayAppointmentCWProxy {
  const _$TodayAppointmentCWProxyImpl(this._value);

  final TodayAppointment _value;

  @override
  TodayAppointment appointmentId(String appointmentId) =>
      this(appointmentId: appointmentId);

  @override
  TodayAppointment startUtc(DateTime startUtc) => this(startUtc: startUtc);

  @override
  TodayAppointment endUtc(DateTime endUtc) => this(endUtc: endUtc);

  @override
  TodayAppointment status(AppointmentStatus status) => this(status: status);

  @override
  TodayAppointment displayStatus(DisplayStatus displayStatus) =>
      this(displayStatus: displayStatus);

  @override
  TodayAppointment customerName(String customerName) =>
      this(customerName: customerName);

  @override
  TodayAppointment isNewCustomer(bool isNewCustomer) =>
      this(isNewCustomer: isNewCustomer);

  @override
  TodayAppointment serviceSummary(String serviceSummary) =>
      this(serviceSummary: serviceSummary);

  @override
  TodayAppointment staffNames(List<String> staffNames) =>
      this(staffNames: staffNames);

  @override
  TodayAppointment holdExpiresUtc(DateTime? holdExpiresUtc) =>
      this(holdExpiresUtc: holdExpiresUtc);

  @override
  TodayAppointment depositStatus(DepositStatus? depositStatus) =>
      this(depositStatus: depositStatus);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `TodayAppointment(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// TodayAppointment(...).copyWith(id: 12, name: "My name")
  /// ````
  TodayAppointment call({
    Object? appointmentId = const $CopyWithPlaceholder(),
    Object? startUtc = const $CopyWithPlaceholder(),
    Object? endUtc = const $CopyWithPlaceholder(),
    Object? status = const $CopyWithPlaceholder(),
    Object? displayStatus = const $CopyWithPlaceholder(),
    Object? customerName = const $CopyWithPlaceholder(),
    Object? isNewCustomer = const $CopyWithPlaceholder(),
    Object? serviceSummary = const $CopyWithPlaceholder(),
    Object? staffNames = const $CopyWithPlaceholder(),
    Object? holdExpiresUtc = const $CopyWithPlaceholder(),
    Object? depositStatus = const $CopyWithPlaceholder(),
  }) {
    return TodayAppointment(
      appointmentId: appointmentId == const $CopyWithPlaceholder()
          ? _value.appointmentId
          // ignore: cast_nullable_to_non_nullable
          : appointmentId as String,
      startUtc: startUtc == const $CopyWithPlaceholder()
          ? _value.startUtc
          // ignore: cast_nullable_to_non_nullable
          : startUtc as DateTime,
      endUtc: endUtc == const $CopyWithPlaceholder()
          ? _value.endUtc
          // ignore: cast_nullable_to_non_nullable
          : endUtc as DateTime,
      status: status == const $CopyWithPlaceholder()
          ? _value.status
          // ignore: cast_nullable_to_non_nullable
          : status as AppointmentStatus,
      displayStatus: displayStatus == const $CopyWithPlaceholder()
          ? _value.displayStatus
          // ignore: cast_nullable_to_non_nullable
          : displayStatus as DisplayStatus,
      customerName: customerName == const $CopyWithPlaceholder()
          ? _value.customerName
          // ignore: cast_nullable_to_non_nullable
          : customerName as String,
      isNewCustomer: isNewCustomer == const $CopyWithPlaceholder()
          ? _value.isNewCustomer
          // ignore: cast_nullable_to_non_nullable
          : isNewCustomer as bool,
      serviceSummary: serviceSummary == const $CopyWithPlaceholder()
          ? _value.serviceSummary
          // ignore: cast_nullable_to_non_nullable
          : serviceSummary as String,
      staffNames: staffNames == const $CopyWithPlaceholder()
          ? _value.staffNames
          // ignore: cast_nullable_to_non_nullable
          : staffNames as List<String>,
      holdExpiresUtc: holdExpiresUtc == const $CopyWithPlaceholder()
          ? _value.holdExpiresUtc
          // ignore: cast_nullable_to_non_nullable
          : holdExpiresUtc as DateTime?,
      depositStatus: depositStatus == const $CopyWithPlaceholder()
          ? _value.depositStatus
          // ignore: cast_nullable_to_non_nullable
          : depositStatus as DepositStatus?,
    );
  }
}

extension $TodayAppointmentCopyWith on TodayAppointment {
  /// Returns a callable class that can be used as follows: `instanceOfTodayAppointment.copyWith(...)` or like so:`instanceOfTodayAppointment.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$TodayAppointmentCWProxy get copyWith => _$TodayAppointmentCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

TodayAppointment _$TodayAppointmentFromJson(Map<String, dynamic> json) =>
    $checkedCreate('TodayAppointment', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'appointmentId',
          'startUtc',
          'endUtc',
          'status',
          'displayStatus',
          'customerName',
          'isNewCustomer',
          'serviceSummary',
          'staffNames',
        ],
      );
      final val = TodayAppointment(
        appointmentId: $checkedConvert('appointmentId', (v) => v as String),
        startUtc: $checkedConvert(
          'startUtc',
          (v) => DateTime.parse(v as String),
        ),
        endUtc: $checkedConvert('endUtc', (v) => DateTime.parse(v as String)),
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
        customerName: $checkedConvert('customerName', (v) => v as String),
        isNewCustomer: $checkedConvert('isNewCustomer', (v) => v as bool),
        serviceSummary: $checkedConvert('serviceSummary', (v) => v as String),
        staffNames: $checkedConvert(
          'staffNames',
          (v) => (v as List<dynamic>).map((e) => e as String).toList(),
        ),
        holdExpiresUtc: $checkedConvert(
          'holdExpiresUtc',
          (v) => v == null ? null : DateTime.parse(v as String),
        ),
        depositStatus: $checkedConvert(
          'depositStatus',
          (v) => $enumDecodeNullable(
            _$DepositStatusEnumMap,
            v,
            unknownValue: DepositStatus.unknownDefaultOpenApi,
          ),
        ),
      );
      return val;
    });

Map<String, dynamic> _$TodayAppointmentToJson(TodayAppointment instance) =>
    <String, dynamic>{
      'appointmentId': instance.appointmentId,
      'startUtc': instance.startUtc.toIso8601String(),
      'endUtc': instance.endUtc.toIso8601String(),
      'status': _$AppointmentStatusEnumMap[instance.status]!,
      'displayStatus': _$DisplayStatusEnumMap[instance.displayStatus]!,
      'customerName': instance.customerName,
      'isNewCustomer': instance.isNewCustomer,
      'serviceSummary': instance.serviceSummary,
      'staffNames': instance.staffNames,
      'holdExpiresUtc': ?instance.holdExpiresUtc?.toIso8601String(),
      'depositStatus': ?_$DepositStatusEnumMap[instance.depositStatus],
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

const _$DepositStatusEnumMap = {
  DepositStatus.awaiting: 'Awaiting',
  DepositStatus.customerReported: 'CustomerReported',
  DepositStatus.received: 'Received',
  DepositStatus.refunded: 'Refunded',
  DepositStatus.waived: 'Waived',
  DepositStatus.unknownDefaultOpenApi: 'unknown_default_open_api',
};
