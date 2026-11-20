// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'appointment_detail_cancellation.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AppointmentDetailCancellationCWProxy {
  AppointmentDetailCancellation cancelledUtc(DateTime cancelledUtc);

  AppointmentDetailCancellation cancelledBy(
    AppointmentDetailCancellationCancelledByEnum cancelledBy,
  );

  AppointmentDetailCancellation reason(CancellationReason reason);

  AppointmentDetailCancellation note(String? note);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AppointmentDetailCancellation(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AppointmentDetailCancellation(...).copyWith(id: 12, name: "My name")
  /// ````
  AppointmentDetailCancellation call({
    DateTime cancelledUtc,
    AppointmentDetailCancellationCancelledByEnum cancelledBy,
    CancellationReason reason,
    String? note,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAppointmentDetailCancellation.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAppointmentDetailCancellation.copyWith.fieldName(...)`
class _$AppointmentDetailCancellationCWProxyImpl
    implements _$AppointmentDetailCancellationCWProxy {
  const _$AppointmentDetailCancellationCWProxyImpl(this._value);

  final AppointmentDetailCancellation _value;

  @override
  AppointmentDetailCancellation cancelledUtc(DateTime cancelledUtc) =>
      this(cancelledUtc: cancelledUtc);

  @override
  AppointmentDetailCancellation cancelledBy(
    AppointmentDetailCancellationCancelledByEnum cancelledBy,
  ) => this(cancelledBy: cancelledBy);

  @override
  AppointmentDetailCancellation reason(CancellationReason reason) =>
      this(reason: reason);

  @override
  AppointmentDetailCancellation note(String? note) => this(note: note);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AppointmentDetailCancellation(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AppointmentDetailCancellation(...).copyWith(id: 12, name: "My name")
  /// ````
  AppointmentDetailCancellation call({
    Object? cancelledUtc = const $CopyWithPlaceholder(),
    Object? cancelledBy = const $CopyWithPlaceholder(),
    Object? reason = const $CopyWithPlaceholder(),
    Object? note = const $CopyWithPlaceholder(),
  }) {
    return AppointmentDetailCancellation(
      cancelledUtc: cancelledUtc == const $CopyWithPlaceholder()
          ? _value.cancelledUtc
          // ignore: cast_nullable_to_non_nullable
          : cancelledUtc as DateTime,
      cancelledBy: cancelledBy == const $CopyWithPlaceholder()
          ? _value.cancelledBy
          // ignore: cast_nullable_to_non_nullable
          : cancelledBy as AppointmentDetailCancellationCancelledByEnum,
      reason: reason == const $CopyWithPlaceholder()
          ? _value.reason
          // ignore: cast_nullable_to_non_nullable
          : reason as CancellationReason,
      note: note == const $CopyWithPlaceholder()
          ? _value.note
          // ignore: cast_nullable_to_non_nullable
          : note as String?,
    );
  }
}

extension $AppointmentDetailCancellationCopyWith
    on AppointmentDetailCancellation {
  /// Returns a callable class that can be used as follows: `instanceOfAppointmentDetailCancellation.copyWith(...)` or like so:`instanceOfAppointmentDetailCancellation.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AppointmentDetailCancellationCWProxy get copyWith =>
      _$AppointmentDetailCancellationCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AppointmentDetailCancellation _$AppointmentDetailCancellationFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('AppointmentDetailCancellation', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const ['cancelledUtc', 'cancelledBy', 'reason'],
  );
  final val = AppointmentDetailCancellation(
    cancelledUtc: $checkedConvert(
      'cancelledUtc',
      (v) => DateTime.parse(v as String),
    ),
    cancelledBy: $checkedConvert(
      'cancelledBy',
      (v) => $enumDecode(
        _$AppointmentDetailCancellationCancelledByEnumEnumMap,
        v,
        unknownValue:
            AppointmentDetailCancellationCancelledByEnum.unknownDefaultOpenApi,
      ),
    ),
    reason: $checkedConvert(
      'reason',
      (v) => $enumDecode(
        _$CancellationReasonEnumMap,
        v,
        unknownValue: CancellationReason.unknownDefaultOpenApi,
      ),
    ),
    note: $checkedConvert('note', (v) => v as String?),
  );
  return val;
});

Map<String, dynamic> _$AppointmentDetailCancellationToJson(
  AppointmentDetailCancellation instance,
) => <String, dynamic>{
  'cancelledUtc': instance.cancelledUtc.toIso8601String(),
  'cancelledBy':
      _$AppointmentDetailCancellationCancelledByEnumEnumMap[instance
          .cancelledBy]!,
  'reason': _$CancellationReasonEnumMap[instance.reason]!,
  'note': ?instance.note,
};

const _$AppointmentDetailCancellationCancelledByEnumEnumMap = {
  AppointmentDetailCancellationCancelledByEnum.customer: 'Customer',
  AppointmentDetailCancellationCancelledByEnum.salon: 'Salon',
  AppointmentDetailCancellationCancelledByEnum.system: 'System',
  AppointmentDetailCancellationCancelledByEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};

const _$CancellationReasonEnumMap = {
  CancellationReason.planChanged: 'PlanChanged',
  CancellationReason.sick: 'Sick',
  CancellationReason.wantsDifferentTime: 'WantsDifferentTime',
  CancellationReason.other: 'Other',
  CancellationReason.customerRequested: 'CustomerRequested',
  CancellationReason.staffUnavailable: 'StaffUnavailable',
  CancellationReason.declinedBySalon: 'DeclinedBySalon',
  CancellationReason.approvalExpired: 'ApprovalExpired',
  CancellationReason.depositNotReceived: 'DepositNotReceived',
  CancellationReason.accountDeleted: 'AccountDeleted',
  CancellationReason.brandReadOnly: 'BrandReadOnly',
  CancellationReason.unknownDefaultOpenApi: 'unknown_default_open_api',
};
