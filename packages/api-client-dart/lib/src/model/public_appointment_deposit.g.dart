// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'public_appointment_deposit.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PublicAppointmentDepositCWProxy {
  PublicAppointmentDeposit required_(Money required_);

  PublicAppointmentDeposit status(PublicAppointmentDepositStatusEnum status);

  PublicAppointmentDeposit transferCode(String? transferCode);

  PublicAppointmentDeposit customerReportedUtc(DateTime? customerReportedUtc);

  PublicAppointmentDeposit receivedUtc(DateTime? receivedUtc);

  PublicAppointmentDeposit instructions(DepositInstructions? instructions);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicAppointmentDeposit(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicAppointmentDeposit(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicAppointmentDeposit call({
    Money required_,
    PublicAppointmentDepositStatusEnum status,
    String? transferCode,
    DateTime? customerReportedUtc,
    DateTime? receivedUtc,
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
  PublicAppointmentDeposit status(PublicAppointmentDepositStatusEnum status) =>
      this(status: status);

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
          : status as PublicAppointmentDepositStatusEnum,
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
        _$PublicAppointmentDepositStatusEnumEnumMap,
        v,
        unknownValue: PublicAppointmentDepositStatusEnum.unknownDefaultOpenApi,
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
  'status': _$PublicAppointmentDepositStatusEnumEnumMap[instance.status]!,
  'transferCode': ?instance.transferCode,
  'customerReportedUtc': ?instance.customerReportedUtc?.toIso8601String(),
  'receivedUtc': ?instance.receivedUtc?.toIso8601String(),
  'instructions': ?instance.instructions?.toJson(),
};

const _$PublicAppointmentDepositStatusEnumEnumMap = {
  PublicAppointmentDepositStatusEnum.awaiting: 'Awaiting',
  PublicAppointmentDepositStatusEnum.customerReported: 'CustomerReported',
  PublicAppointmentDepositStatusEnum.received: 'Received',
  PublicAppointmentDepositStatusEnum.refunded: 'Refunded',
  PublicAppointmentDepositStatusEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};
