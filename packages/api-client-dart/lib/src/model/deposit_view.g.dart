// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'deposit_view.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$DepositViewCWProxy {
  DepositView required_(Money required_);

  DepositView status(DepositStatus status);

  DepositView transferCode(String? transferCode);

  DepositView customerReportedUtc(DateTime? customerReportedUtc);

  DepositView receivedUtc(DateTime? receivedUtc);

  DepositView remindedUtc(DateTime? remindedUtc);

  DepositView refundedUtc(DateTime? refundedUtc);

  DepositView refundedAmount(Money? refundedAmount);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `DepositView(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// DepositView(...).copyWith(id: 12, name: "My name")
  /// ````
  DepositView call({
    Money required_,
    DepositStatus status,
    String? transferCode,
    DateTime? customerReportedUtc,
    DateTime? receivedUtc,
    DateTime? remindedUtc,
    DateTime? refundedUtc,
    Money? refundedAmount,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfDepositView.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfDepositView.copyWith.fieldName(...)`
class _$DepositViewCWProxyImpl implements _$DepositViewCWProxy {
  const _$DepositViewCWProxyImpl(this._value);

  final DepositView _value;

  @override
  DepositView required_(Money required_) => this(required_: required_);

  @override
  DepositView status(DepositStatus status) => this(status: status);

  @override
  DepositView transferCode(String? transferCode) =>
      this(transferCode: transferCode);

  @override
  DepositView customerReportedUtc(DateTime? customerReportedUtc) =>
      this(customerReportedUtc: customerReportedUtc);

  @override
  DepositView receivedUtc(DateTime? receivedUtc) =>
      this(receivedUtc: receivedUtc);

  @override
  DepositView remindedUtc(DateTime? remindedUtc) =>
      this(remindedUtc: remindedUtc);

  @override
  DepositView refundedUtc(DateTime? refundedUtc) =>
      this(refundedUtc: refundedUtc);

  @override
  DepositView refundedAmount(Money? refundedAmount) =>
      this(refundedAmount: refundedAmount);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `DepositView(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// DepositView(...).copyWith(id: 12, name: "My name")
  /// ````
  DepositView call({
    Object? required_ = const $CopyWithPlaceholder(),
    Object? status = const $CopyWithPlaceholder(),
    Object? transferCode = const $CopyWithPlaceholder(),
    Object? customerReportedUtc = const $CopyWithPlaceholder(),
    Object? receivedUtc = const $CopyWithPlaceholder(),
    Object? remindedUtc = const $CopyWithPlaceholder(),
    Object? refundedUtc = const $CopyWithPlaceholder(),
    Object? refundedAmount = const $CopyWithPlaceholder(),
  }) {
    return DepositView(
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
    );
  }
}

extension $DepositViewCopyWith on DepositView {
  /// Returns a callable class that can be used as follows: `instanceOfDepositView.copyWith(...)` or like so:`instanceOfDepositView.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$DepositViewCWProxy get copyWith => _$DepositViewCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

DepositView _$DepositViewFromJson(Map<String, dynamic> json) =>
    $checkedCreate('DepositView', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['required', 'status']);
      final val = DepositView(
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
      );
      return val;
    }, fieldKeyMap: const {'required_': 'required'});

Map<String, dynamic> _$DepositViewToJson(DepositView instance) =>
    <String, dynamic>{
      'required': instance.required_.toJson(),
      'status': _$DepositStatusEnumMap[instance.status]!,
      'transferCode': ?instance.transferCode,
      'customerReportedUtc': ?instance.customerReportedUtc?.toIso8601String(),
      'receivedUtc': ?instance.receivedUtc?.toIso8601String(),
      'remindedUtc': ?instance.remindedUtc?.toIso8601String(),
      'refundedUtc': ?instance.refundedUtc?.toIso8601String(),
      'refundedAmount': ?instance.refundedAmount?.toJson(),
    };

const _$DepositStatusEnumMap = {
  DepositStatus.awaiting: 'Awaiting',
  DepositStatus.customerReported: 'CustomerReported',
  DepositStatus.received: 'Received',
  DepositStatus.refunded: 'Refunded',
  DepositStatus.waived: 'Waived',
  DepositStatus.unknownDefaultOpenApi: 'unknown_default_open_api',
};
