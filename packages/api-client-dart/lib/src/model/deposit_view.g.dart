// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'deposit_view.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$DepositViewCWProxy {
  DepositView required_(Money required_);

  DepositView status(DepositViewStatusEnum status);

  DepositView transferCode(String? transferCode);

  DepositView customerReportedUtc(DateTime? customerReportedUtc);

  DepositView receivedUtc(DateTime? receivedUtc);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `DepositView(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// DepositView(...).copyWith(id: 12, name: "My name")
  /// ````
  DepositView call({
    Money required_,
    DepositViewStatusEnum status,
    String? transferCode,
    DateTime? customerReportedUtc,
    DateTime? receivedUtc,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfDepositView.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfDepositView.copyWith.fieldName(...)`
class _$DepositViewCWProxyImpl implements _$DepositViewCWProxy {
  const _$DepositViewCWProxyImpl(this._value);

  final DepositView _value;

  @override
  DepositView required_(Money required_) => this(required_: required_);

  @override
  DepositView status(DepositViewStatusEnum status) => this(status: status);

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
  }) {
    return DepositView(
      required_: required_ == const $CopyWithPlaceholder()
          ? _value.required_
          // ignore: cast_nullable_to_non_nullable
          : required_ as Money,
      status: status == const $CopyWithPlaceholder()
          ? _value.status
          // ignore: cast_nullable_to_non_nullable
          : status as DepositViewStatusEnum,
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
            _$DepositViewStatusEnumEnumMap,
            v,
            unknownValue: DepositViewStatusEnum.unknownDefaultOpenApi,
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
      );
      return val;
    }, fieldKeyMap: const {'required_': 'required'});

Map<String, dynamic> _$DepositViewToJson(DepositView instance) =>
    <String, dynamic>{
      'required': instance.required_.toJson(),
      'status': _$DepositViewStatusEnumEnumMap[instance.status]!,
      'transferCode': ?instance.transferCode,
      'customerReportedUtc': ?instance.customerReportedUtc?.toIso8601String(),
      'receivedUtc': ?instance.receivedUtc?.toIso8601String(),
    };

const _$DepositViewStatusEnumEnumMap = {
  DepositViewStatusEnum.awaiting: 'Awaiting',
  DepositViewStatusEnum.customerReported: 'CustomerReported',
  DepositViewStatusEnum.received: 'Received',
  DepositViewStatusEnum.refunded: 'Refunded',
  DepositViewStatusEnum.unknownDefaultOpenApi: 'unknown_default_open_api',
};
