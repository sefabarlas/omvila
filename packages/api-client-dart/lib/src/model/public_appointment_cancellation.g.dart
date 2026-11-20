// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'public_appointment_cancellation.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PublicAppointmentCancellationCWProxy {
  PublicAppointmentCancellation cancelledUtc(DateTime cancelledUtc);

  PublicAppointmentCancellation cancelledBy(
    PublicAppointmentCancellationCancelledByEnum cancelledBy,
  );

  PublicAppointmentCancellation reason(CancellationReason reason);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicAppointmentCancellation(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicAppointmentCancellation(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicAppointmentCancellation call({
    DateTime cancelledUtc,
    PublicAppointmentCancellationCancelledByEnum cancelledBy,
    CancellationReason reason,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPublicAppointmentCancellation.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPublicAppointmentCancellation.copyWith.fieldName(...)`
class _$PublicAppointmentCancellationCWProxyImpl
    implements _$PublicAppointmentCancellationCWProxy {
  const _$PublicAppointmentCancellationCWProxyImpl(this._value);

  final PublicAppointmentCancellation _value;

  @override
  PublicAppointmentCancellation cancelledUtc(DateTime cancelledUtc) =>
      this(cancelledUtc: cancelledUtc);

  @override
  PublicAppointmentCancellation cancelledBy(
    PublicAppointmentCancellationCancelledByEnum cancelledBy,
  ) => this(cancelledBy: cancelledBy);

  @override
  PublicAppointmentCancellation reason(CancellationReason reason) =>
      this(reason: reason);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicAppointmentCancellation(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicAppointmentCancellation(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicAppointmentCancellation call({
    Object? cancelledUtc = const $CopyWithPlaceholder(),
    Object? cancelledBy = const $CopyWithPlaceholder(),
    Object? reason = const $CopyWithPlaceholder(),
  }) {
    return PublicAppointmentCancellation(
      cancelledUtc: cancelledUtc == const $CopyWithPlaceholder()
          ? _value.cancelledUtc
          // ignore: cast_nullable_to_non_nullable
          : cancelledUtc as DateTime,
      cancelledBy: cancelledBy == const $CopyWithPlaceholder()
          ? _value.cancelledBy
          // ignore: cast_nullable_to_non_nullable
          : cancelledBy as PublicAppointmentCancellationCancelledByEnum,
      reason: reason == const $CopyWithPlaceholder()
          ? _value.reason
          // ignore: cast_nullable_to_non_nullable
          : reason as CancellationReason,
    );
  }
}

extension $PublicAppointmentCancellationCopyWith
    on PublicAppointmentCancellation {
  /// Returns a callable class that can be used as follows: `instanceOfPublicAppointmentCancellation.copyWith(...)` or like so:`instanceOfPublicAppointmentCancellation.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PublicAppointmentCancellationCWProxy get copyWith =>
      _$PublicAppointmentCancellationCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PublicAppointmentCancellation _$PublicAppointmentCancellationFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('PublicAppointmentCancellation', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const ['cancelledUtc', 'cancelledBy', 'reason'],
  );
  final val = PublicAppointmentCancellation(
    cancelledUtc: $checkedConvert(
      'cancelledUtc',
      (v) => DateTime.parse(v as String),
    ),
    cancelledBy: $checkedConvert(
      'cancelledBy',
      (v) => $enumDecode(
        _$PublicAppointmentCancellationCancelledByEnumEnumMap,
        v,
        unknownValue:
            PublicAppointmentCancellationCancelledByEnum.unknownDefaultOpenApi,
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
  );
  return val;
});

Map<String, dynamic> _$PublicAppointmentCancellationToJson(
  PublicAppointmentCancellation instance,
) => <String, dynamic>{
  'cancelledUtc': instance.cancelledUtc.toIso8601String(),
  'cancelledBy':
      _$PublicAppointmentCancellationCancelledByEnumEnumMap[instance
          .cancelledBy]!,
  'reason': _$CancellationReasonEnumMap[instance.reason]!,
};

const _$PublicAppointmentCancellationCancelledByEnumEnumMap = {
  PublicAppointmentCancellationCancelledByEnum.customer: 'Customer',
  PublicAppointmentCancellationCancelledByEnum.salon: 'Salon',
  PublicAppointmentCancellationCancelledByEnum.system: 'System',
  PublicAppointmentCancellationCancelledByEnum.unknownDefaultOpenApi:
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
