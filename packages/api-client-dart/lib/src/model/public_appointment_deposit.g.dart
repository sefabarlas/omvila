// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'public_appointment_deposit.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PublicAppointmentDepositCWProxy {
  PublicAppointmentDeposit required_(Money required_);

  PublicAppointmentDeposit status(DepositStatus status);

  PublicAppointmentDeposit transferCode(String? transferCode);

  PublicAppointmentDeposit customerReportedUtc(DateTime? customerReportedUtc);

  PublicAppointmentDeposit receivedUtc(DateTime? receivedUtc);

  PublicAppointmentDeposit remindedUtc(DateTime? remindedUtc);

  PublicAppointmentDeposit refundedUtc(DateTime? refundedUtc);

  PublicAppointmentDeposit refundedAmount(Money? refundedAmount);

  PublicAppointmentDeposit instructions(DepositInstructions? instructions);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicAppointmentDeposit(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicAppointmentDeposit(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicAppointmentDeposit call({
    Money required_,
    DepositStatus status,
    String? transferCode,
    DateTime? customerReportedUtc,
    DateTime? receivedUtc,
    DateTime? remindedUtc,
    DateTime? refundedUtc,
    Money? refundedAmount,
    DepositInstructions? instructions,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPublicAppointmentDeposit.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPublicAppointmentDeposit.copyWith.fieldName(...)`
class _$PublicAppointmentDepositCWProxyImpl
    implements _$PublicAppointmentDepositCWProxy {
  const _$PublicAppointmentDepositCWProxyImpl(this._value);

  final PublicAppointmentDeposit _value;

  @override
  PublicAppointmentDeposit required_(Money required_) =>
      this(required_: required_);

  @override
  PublicAppointmentDeposit status(DepositStatus status) => this(status: status);

  @override
  PublicAppointmentDeposit transferCode(String? transferCode) =>
      this(transferCode: transferCode);

  @override
  PublicAppointmentDeposit customerReportedUtc(DateTime? customerReportedUtc) =>
      this(customerReportedUtc: customerReportedUtc);

  @override
  PublicAppointmentDeposit receivedUtc(DateTime? receivedUtc) =>
      this(receivedUtc: receivedUtc);

  @override
  PublicAppointmentDeposit remindedUtc(DateTime? remindedUtc) =>
      this(remindedUtc: remindedUtc);

  @override
  PublicAppointmentDeposit refundedUtc(DateTime? refundedUtc) =>
      this(refundedUtc: refundedUtc);

  @override
  PublicAppointmentDeposit refundedAmount(Money? refundedAmount) =>
      this(refundedAmount: refundedAmount);

  @override
  PublicAppointmentDeposit instructions(DepositInstructions? instructions) =>
      this(instructions: instructions);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicAppointmentDeposit(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicAppointmentDeposit(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicAppointmentDeposit call({
    Object? required_ = const $CopyWithPlaceholder(),
    Object? status = const $CopyWithPlaceholder(),
    Object? transferCode = const $CopyWithPlaceholder(),
    Object? customerReportedUtc = const $CopyWithPlaceholder(),
    Object? receivedUtc = const $CopyWithPlaceholder(),
    Object? remindedUtc = const $CopyWithPlaceholder(),
    Object? refundedUtc = const $CopyWithPlaceholder(),
    Object? refundedAmount = const $CopyWithPlaceholder(),
    Object? instructions = const $CopyWithPlaceholder(),
  }) {
    return PublicAppointmentDeposit(
      required_: required_ == const $CopyWithPlaceholder()
          ? _value.required_
          // ignore: cast_nullable_to_non_nullable
          : required_ as Money,
      status: status == const $CopyWithPlaceholder()
          ? _value.status
          // ignore: cast_nullable_to_non_nullable
          : status as DepositStatus,
      transferCode: transferCode == const $CopyWithPlaceholder()
          ? _value.transferCode
          // ignore: cast_nullable_to_non_nullable
          : transferCode as String?,
      customerReportedUtc: customerReportedUtc == const $CopyWithPlaceholder()
          ? _value.customerReportedUtc
          // ignore: cast_nullable_to_non_nullable
          : customerReportedUtc as DateTime?,
      receivedUtc: receivedUtc == const $CopyWithPlaceholder()
          ? _value.receivedUtc
          // ignore: cast_nullable_to_non_nullable
          : receivedUtc as DateTime?,
      remindedUtc: remindedUtc == const $CopyWithPlaceholder()
          ? _value.remindedUtc
          // ignore: cast_nullable_to_non_nullable
          : remindedUtc as DateTime?,
      refundedUtc: refundedUtc == const $CopyWithPlaceholder()
          ? _value.refundedUtc
          // ignore: cast_nullable_to_non_nullable
          : refundedUtc as DateTime?,
      refundedAmount: refundedAmount == const $CopyWithPlaceholder()
          ? _value.refundedAmount
          // ignore: cast_nullable_to_non_nullable
          : refundedAmount as Money?,
      instructions: instructions == const $CopyWithPlaceholder()
          ? _value.instructions
          // ignore: cast_nullable_to_non_nullable
          : instructions as DepositInstructions?,
    );
  }
}

extension $PublicAppointmentDepositCopyWith on PublicAppointmentDeposit {
  /// Returns a callable class that can be used as follows: `instanceOfPublicAppointmentDeposit.copyWith(...)` or like so:`instanceOfPublicAppointmentDeposit.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PublicAppointmentDepositCWProxy get copyWith =>
      _$PublicAppointmentDepositCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PublicAppointmentDeposit _$PublicAppointmentDepositFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('PublicAppointmentDeposit', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['required', 'status']);
  final val = PublicAppointmentDeposit(
    required_: $checkedConvert(
      'required',
      (v) => Money.fromJson(v as Map<String, dynamic>),
    ),
    status: $checkedConvert(
      'status',
      (v) => $enumDecode(
        _$DepositStatusEnumMap,
        v,
        unknownValue: DepositStatus.unknownDefaultOpenApi,
      ),
    ),
    transferCode: $checkedConvert('transferCode', (v) => v as String?),
    customerReportedUtc: $checkedConvert(
      'customerReportedUtc',
      (v) => v == null ? null : DateTime.parse(v as String),
    ),
    receivedUtc: $checkedConvert(
      'receivedUtc',
      (v) => v == null ? null : DateTime.parse(v as String),
    ),
    remindedUtc: $checkedConvert(
      'remindedUtc',
      (v) => v == null ? null : DateTime.parse(v as String),
    ),
    refundedUtc: $checkedConvert(
      'refundedUtc',
      (v) => v == null ? null : DateTime.parse(v as String),
    ),
    refundedAmount: $checkedConvert(
      'refundedAmount',
      (v) => v == null ? null : Money.fromJson(v as Map<String, dynamic>),
    ),
    instructions: $checkedConvert(
      'instructions',
      (v) => v == null
          ? null
          : DepositInstructions.fromJson(v as Map<String, dynamic>),
    ),
  );
  return val;
}, fieldKeyMap: const {'required_': 'required'});

Map<String, dynamic> _$PublicAppointmentDepositToJson(
  PublicAppointmentDeposit instance,
) => <String, dynamic>{
  'required': instance.required_.toJson(),
  'status': _$DepositStatusEnumMap[instance.status]!,
  'transferCode': ?instance.transferCode,
  'customerReportedUtc': ?instance.customerReportedUtc?.toIso8601String(),
  'receivedUtc': ?instance.receivedUtc?.toIso8601String(),
  'remindedUtc': ?instance.remindedUtc?.toIso8601String(),
  'refundedUtc': ?instance.refundedUtc?.toIso8601String(),
  'refundedAmount': ?instance.refundedAmount?.toJson(),
  'instructions': ?instance.instructions?.toJson(),
};

const _$DepositStatusEnumMap = {
  DepositStatus.awaiting: 'Awaiting',
  DepositStatus.customerReported: 'CustomerReported',
  DepositStatus.received: 'Received',
  DepositStatus.refunded: 'Refunded',
  DepositStatus.waived: 'Waived',
  DepositStatus.unknownDefaultOpenApi: 'unknown_default_open_api',
};
