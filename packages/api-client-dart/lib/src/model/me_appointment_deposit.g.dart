// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'me_appointment_deposit.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$MeAppointmentDepositCWProxy {
  MeAppointmentDeposit required_(Money required_);

  MeAppointmentDeposit status(DepositStatus status);

  MeAppointmentDeposit transferCode(String? transferCode);

  MeAppointmentDeposit customerReportedUtc(DateTime? customerReportedUtc);

  MeAppointmentDeposit receivedUtc(DateTime? receivedUtc);

  MeAppointmentDeposit remindedUtc(DateTime? remindedUtc);

  MeAppointmentDeposit refundedUtc(DateTime? refundedUtc);

  MeAppointmentDeposit refundedAmount(Money? refundedAmount);

  MeAppointmentDeposit instructions(DepositInstructions? instructions);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeAppointmentDeposit(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeAppointmentDeposit(...).copyWith(id: 12, name: "My name")
  /// ````
  MeAppointmentDeposit call({
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

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfMeAppointmentDeposit.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfMeAppointmentDeposit.copyWith.fieldName(...)`
class _$MeAppointmentDepositCWProxyImpl
    implements _$MeAppointmentDepositCWProxy {
  const _$MeAppointmentDepositCWProxyImpl(this._value);

  final MeAppointmentDeposit _value;

  @override
  MeAppointmentDeposit required_(Money required_) => this(required_: required_);

  @override
  MeAppointmentDeposit status(DepositStatus status) => this(status: status);

  @override
  MeAppointmentDeposit transferCode(String? transferCode) =>
      this(transferCode: transferCode);

  @override
  MeAppointmentDeposit customerReportedUtc(DateTime? customerReportedUtc) =>
      this(customerReportedUtc: customerReportedUtc);

  @override
  MeAppointmentDeposit receivedUtc(DateTime? receivedUtc) =>
      this(receivedUtc: receivedUtc);

  @override
  MeAppointmentDeposit remindedUtc(DateTime? remindedUtc) =>
      this(remindedUtc: remindedUtc);

  @override
  MeAppointmentDeposit refundedUtc(DateTime? refundedUtc) =>
      this(refundedUtc: refundedUtc);

  @override
  MeAppointmentDeposit refundedAmount(Money? refundedAmount) =>
      this(refundedAmount: refundedAmount);

  @override
  MeAppointmentDeposit instructions(DepositInstructions? instructions) =>
      this(instructions: instructions);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeAppointmentDeposit(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeAppointmentDeposit(...).copyWith(id: 12, name: "My name")
  /// ````
  MeAppointmentDeposit call({
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
    return MeAppointmentDeposit(
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

extension $MeAppointmentDepositCopyWith on MeAppointmentDeposit {
  /// Returns a callable class that can be used as follows: `instanceOfMeAppointmentDeposit.copyWith(...)` or like so:`instanceOfMeAppointmentDeposit.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$MeAppointmentDepositCWProxy get copyWith =>
      _$MeAppointmentDepositCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MeAppointmentDeposit _$MeAppointmentDepositFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('MeAppointmentDeposit', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['required', 'status']);
  final val = MeAppointmentDeposit(
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

Map<String, dynamic> _$MeAppointmentDepositToJson(
  MeAppointmentDeposit instance,
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
