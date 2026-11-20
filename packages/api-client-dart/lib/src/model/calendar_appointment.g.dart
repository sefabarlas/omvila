// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'calendar_appointment.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$CalendarAppointmentCWProxy {
  CalendarAppointment appointmentId(String appointmentId);

  CalendarAppointment itemId(String itemId);

  CalendarAppointment staffId(String? staffId);

  CalendarAppointment startUtc(DateTime startUtc);

  CalendarAppointment endUtc(DateTime endUtc);

  CalendarAppointment status(AppointmentStatus status);

  CalendarAppointment displayStatus(DisplayStatus displayStatus);

  CalendarAppointment customerName(String customerName);

  CalendarAppointment serviceName(String serviceName);

  CalendarAppointment isNewCustomer(bool? isNewCustomer);

  CalendarAppointment isRecurring(bool isRecurring);

  CalendarAppointment holdExpiresUtc(DateTime? holdExpiresUtc);

  CalendarAppointment segments(List<SegmentView> segments);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CalendarAppointment(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CalendarAppointment(...).copyWith(id: 12, name: "My name")
  /// ````
  CalendarAppointment call({
    String appointmentId,
    String itemId,
    String? staffId,
    DateTime startUtc,
    DateTime endUtc,
    AppointmentStatus status,
    DisplayStatus displayStatus,
    String customerName,
    String serviceName,
    bool? isNewCustomer,
    bool isRecurring,
    DateTime? holdExpiresUtc,
    List<SegmentView> segments,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfCalendarAppointment.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfCalendarAppointment.copyWith.fieldName(...)`
class _$CalendarAppointmentCWProxyImpl implements _$CalendarAppointmentCWProxy {
  const _$CalendarAppointmentCWProxyImpl(this._value);

  final CalendarAppointment _value;

  @override
  CalendarAppointment appointmentId(String appointmentId) =>
      this(appointmentId: appointmentId);

  @override
  CalendarAppointment itemId(String itemId) => this(itemId: itemId);

  @override
  CalendarAppointment staffId(String? staffId) => this(staffId: staffId);

  @override
  CalendarAppointment startUtc(DateTime startUtc) => this(startUtc: startUtc);

  @override
  CalendarAppointment endUtc(DateTime endUtc) => this(endUtc: endUtc);

  @override
  CalendarAppointment status(AppointmentStatus status) => this(status: status);

  @override
  CalendarAppointment displayStatus(DisplayStatus displayStatus) =>
      this(displayStatus: displayStatus);

  @override
  CalendarAppointment customerName(String customerName) =>
      this(customerName: customerName);

  @override
  CalendarAppointment serviceName(String serviceName) =>
      this(serviceName: serviceName);

  @override
  CalendarAppointment isNewCustomer(bool? isNewCustomer) =>
      this(isNewCustomer: isNewCustomer);

  @override
  CalendarAppointment isRecurring(bool isRecurring) =>
      this(isRecurring: isRecurring);

  @override
  CalendarAppointment holdExpiresUtc(DateTime? holdExpiresUtc) =>
      this(holdExpiresUtc: holdExpiresUtc);

  @override
  CalendarAppointment segments(List<SegmentView> segments) =>
      this(segments: segments);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CalendarAppointment(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CalendarAppointment(...).copyWith(id: 12, name: "My name")
  /// ````
  CalendarAppointment call({
    Object? appointmentId = const $CopyWithPlaceholder(),
    Object? itemId = const $CopyWithPlaceholder(),
    Object? staffId = const $CopyWithPlaceholder(),
    Object? startUtc = const $CopyWithPlaceholder(),
    Object? endUtc = const $CopyWithPlaceholder(),
    Object? status = const $CopyWithPlaceholder(),
    Object? displayStatus = const $CopyWithPlaceholder(),
    Object? customerName = const $CopyWithPlaceholder(),
    Object? serviceName = const $CopyWithPlaceholder(),
    Object? isNewCustomer = const $CopyWithPlaceholder(),
    Object? isRecurring = const $CopyWithPlaceholder(),
    Object? holdExpiresUtc = const $CopyWithPlaceholder(),
    Object? segments = const $CopyWithPlaceholder(),
  }) {
    return CalendarAppointment(
      appointmentId: appointmentId == const $CopyWithPlaceholder()
          ? _value.appointmentId
          // ignore: cast_nullable_to_non_nullable
          : appointmentId as String,
      itemId: itemId == const $CopyWithPlaceholder()
          ? _value.itemId
          // ignore: cast_nullable_to_non_nullable
          : itemId as String,
      staffId: staffId == const $CopyWithPlaceholder()
          ? _value.staffId
          // ignore: cast_nullable_to_non_nullable
          : staffId as String?,
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
      serviceName: serviceName == const $CopyWithPlaceholder()
          ? _value.serviceName
          // ignore: cast_nullable_to_non_nullable
          : serviceName as String,
      isNewCustomer: isNewCustomer == const $CopyWithPlaceholder()
          ? _value.isNewCustomer
          // ignore: cast_nullable_to_non_nullable
          : isNewCustomer as bool?,
      isRecurring: isRecurring == const $CopyWithPlaceholder()
          ? _value.isRecurring
          // ignore: cast_nullable_to_non_nullable
          : isRecurring as bool,
      holdExpiresUtc: holdExpiresUtc == const $CopyWithPlaceholder()
          ? _value.holdExpiresUtc
          // ignore: cast_nullable_to_non_nullable
          : holdExpiresUtc as DateTime?,
      segments: segments == const $CopyWithPlaceholder()
          ? _value.segments
          // ignore: cast_nullable_to_non_nullable
          : segments as List<SegmentView>,
    );
  }
}

extension $CalendarAppointmentCopyWith on CalendarAppointment {
  /// Returns a callable class that can be used as follows: `instanceOfCalendarAppointment.copyWith(...)` or like so:`instanceOfCalendarAppointment.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$CalendarAppointmentCWProxy get copyWith =>
      _$CalendarAppointmentCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CalendarAppointment _$CalendarAppointmentFromJson(Map<String, dynamic> json) =>
    $checkedCreate('CalendarAppointment', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'appointmentId',
          'itemId',
          'staffId',
          'startUtc',
          'endUtc',
          'status',
          'displayStatus',
          'customerName',
          'serviceName',
          'isRecurring',
          'segments',
        ],
      );
      final val = CalendarAppointment(
        appointmentId: $checkedConvert('appointmentId', (v) => v as String),
        itemId: $checkedConvert('itemId', (v) => v as String),
        staffId: $checkedConvert('staffId', (v) => v as String?),
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
        serviceName: $checkedConvert('serviceName', (v) => v as String),
        isNewCustomer: $checkedConvert('isNewCustomer', (v) => v as bool?),
        isRecurring: $checkedConvert('isRecurring', (v) => v as bool),
        holdExpiresUtc: $checkedConvert(
          'holdExpiresUtc',
          (v) => v == null ? null : DateTime.parse(v as String),
        ),
        segments: $checkedConvert(
          'segments',
          (v) => (v as List<dynamic>)
              .map((e) => SegmentView.fromJson(e as Map<String, dynamic>))
              .toList(),
        ),
      );
      return val;
    });

Map<String, dynamic> _$CalendarAppointmentToJson(
  CalendarAppointment instance,
) => <String, dynamic>{
  'appointmentId': instance.appointmentId,
  'itemId': instance.itemId,
  'staffId': instance.staffId,
  'startUtc': instance.startUtc.toIso8601String(),
  'endUtc': instance.endUtc.toIso8601String(),
  'status': _$AppointmentStatusEnumMap[instance.status]!,
  'displayStatus': _$DisplayStatusEnumMap[instance.displayStatus]!,
  'customerName': instance.customerName,
  'serviceName': instance.serviceName,
  'isNewCustomer': ?instance.isNewCustomer,
  'isRecurring': instance.isRecurring,
  'holdExpiresUtc': ?instance.holdExpiresUtc?.toIso8601String(),
  'segments': instance.segments.map((e) => e.toJson()).toList(),
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
