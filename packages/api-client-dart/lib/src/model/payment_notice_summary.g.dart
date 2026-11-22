// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'payment_notice_summary.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PaymentNoticeSummaryCWProxy {
  PaymentNoticeSummary id(String id);

  PaymentNoticeSummary purpose(PaymentNoticePurpose purpose);

  PaymentNoticeSummary transferCode(String transferCode);

  PaymentNoticeSummary expectedAmount(Money? expectedAmount);

  PaymentNoticeSummary credits(int? credits);

  PaymentNoticeSummary senderName(String? senderName);

  PaymentNoticeSummary hasReceipt(bool hasReceipt);

  PaymentNoticeSummary status(PaymentNoticeStatus status);

  PaymentNoticeSummary notifiedUtc(DateTime notifiedUtc);

  PaymentNoticeSummary resolvedUtc(DateTime? resolvedUtc);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PaymentNoticeSummary(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PaymentNoticeSummary(...).copyWith(id: 12, name: "My name")
  /// ````
  PaymentNoticeSummary call({
    String id,
    PaymentNoticePurpose purpose,
    String transferCode,
    Money? expectedAmount,
    int? credits,
    String? senderName,
    bool hasReceipt,
    PaymentNoticeStatus status,
    DateTime notifiedUtc,
    DateTime? resolvedUtc,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPaymentNoticeSummary.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPaymentNoticeSummary.copyWith.fieldName(...)`
class _$PaymentNoticeSummaryCWProxyImpl
    implements _$PaymentNoticeSummaryCWProxy {
  const _$PaymentNoticeSummaryCWProxyImpl(this._value);

  final PaymentNoticeSummary _value;

  @override
  PaymentNoticeSummary id(String id) => this(id: id);

  @override
  PaymentNoticeSummary purpose(PaymentNoticePurpose purpose) =>
      this(purpose: purpose);

  @override
  PaymentNoticeSummary transferCode(String transferCode) =>
      this(transferCode: transferCode);

  @override
  PaymentNoticeSummary expectedAmount(Money? expectedAmount) =>
      this(expectedAmount: expectedAmount);

  @override
  PaymentNoticeSummary credits(int? credits) => this(credits: credits);

  @override
  PaymentNoticeSummary senderName(String? senderName) =>
      this(senderName: senderName);

  @override
  PaymentNoticeSummary hasReceipt(bool hasReceipt) =>
      this(hasReceipt: hasReceipt);

  @override
  PaymentNoticeSummary status(PaymentNoticeStatus status) =>
      this(status: status);

  @override
  PaymentNoticeSummary notifiedUtc(DateTime notifiedUtc) =>
      this(notifiedUtc: notifiedUtc);

  @override
  PaymentNoticeSummary resolvedUtc(DateTime? resolvedUtc) =>
      this(resolvedUtc: resolvedUtc);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PaymentNoticeSummary(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PaymentNoticeSummary(...).copyWith(id: 12, name: "My name")
  /// ````
  PaymentNoticeSummary call({
    Object? id = const $CopyWithPlaceholder(),
    Object? purpose = const $CopyWithPlaceholder(),
    Object? transferCode = const $CopyWithPlaceholder(),
    Object? expectedAmount = const $CopyWithPlaceholder(),
    Object? credits = const $CopyWithPlaceholder(),
    Object? senderName = const $CopyWithPlaceholder(),
    Object? hasReceipt = const $CopyWithPlaceholder(),
    Object? status = const $CopyWithPlaceholder(),
    Object? notifiedUtc = const $CopyWithPlaceholder(),
    Object? resolvedUtc = const $CopyWithPlaceholder(),
  }) {
    return PaymentNoticeSummary(
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as String,
      purpose: purpose == const $CopyWithPlaceholder()
          ? _value.purpose
          // ignore: cast_nullable_to_non_nullable
          : purpose as PaymentNoticePurpose,
      transferCode: transferCode == const $CopyWithPlaceholder()
          ? _value.transferCode
          // ignore: cast_nullable_to_non_nullable
          : transferCode as String,
      expectedAmount: expectedAmount == const $CopyWithPlaceholder()
          ? _value.expectedAmount
          // ignore: cast_nullable_to_non_nullable
          : expectedAmount as Money?,
      credits: credits == const $CopyWithPlaceholder()
          ? _value.credits
          // ignore: cast_nullable_to_non_nullable
          : credits as int?,
      senderName: senderName == const $CopyWithPlaceholder()
          ? _value.senderName
          // ignore: cast_nullable_to_non_nullable
          : senderName as String?,
      hasReceipt: hasReceipt == const $CopyWithPlaceholder()
          ? _value.hasReceipt
          // ignore: cast_nullable_to_non_nullable
          : hasReceipt as bool,
      status: status == const $CopyWithPlaceholder()
          ? _value.status
          // ignore: cast_nullable_to_non_nullable
          : status as PaymentNoticeStatus,
      notifiedUtc: notifiedUtc == const $CopyWithPlaceholder()
          ? _value.notifiedUtc
          // ignore: cast_nullable_to_non_nullable
          : notifiedUtc as DateTime,
      resolvedUtc: resolvedUtc == const $CopyWithPlaceholder()
          ? _value.resolvedUtc
          // ignore: cast_nullable_to_non_nullable
          : resolvedUtc as DateTime?,
    );
  }
}

extension $PaymentNoticeSummaryCopyWith on PaymentNoticeSummary {
  /// Returns a callable class that can be used as follows: `instanceOfPaymentNoticeSummary.copyWith(...)` or like so:`instanceOfPaymentNoticeSummary.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PaymentNoticeSummaryCWProxy get copyWith =>
      _$PaymentNoticeSummaryCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PaymentNoticeSummary _$PaymentNoticeSummaryFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('PaymentNoticeSummary', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const [
      'id',
      'purpose',
      'transferCode',
      'expectedAmount',
      'credits',
      'senderName',
      'hasReceipt',
      'status',
      'notifiedUtc',
    ],
  );
  final val = PaymentNoticeSummary(
    id: $checkedConvert('id', (v) => v as String),
    purpose: $checkedConvert(
      'purpose',
      (v) => $enumDecode(
        _$PaymentNoticePurposeEnumMap,
        v,
        unknownValue: PaymentNoticePurpose.unknownDefaultOpenApi,
      ),
    ),
    transferCode: $checkedConvert('transferCode', (v) => v as String),
    expectedAmount: $checkedConvert(
      'expectedAmount',
      (v) => v == null ? null : Money.fromJson(v as Map<String, dynamic>),
    ),
    credits: $checkedConvert('credits', (v) => (v as num?)?.toInt()),
    senderName: $checkedConvert('senderName', (v) => v as String?),
    hasReceipt: $checkedConvert('hasReceipt', (v) => v as bool),
    status: $checkedConvert(
      'status',
      (v) => $enumDecode(
        _$PaymentNoticeStatusEnumMap,
        v,
        unknownValue: PaymentNoticeStatus.unknownDefaultOpenApi,
      ),
    ),
    notifiedUtc: $checkedConvert(
      'notifiedUtc',
      (v) => DateTime.parse(v as String),
    ),
    resolvedUtc: $checkedConvert(
      'resolvedUtc',
      (v) => v == null ? null : DateTime.parse(v as String),
    ),
  );
  return val;
});

Map<String, dynamic> _$PaymentNoticeSummaryToJson(
  PaymentNoticeSummary instance,
) => <String, dynamic>{
  'id': instance.id,
  'purpose': _$PaymentNoticePurposeEnumMap[instance.purpose]!,
  'transferCode': instance.transferCode,
  'expectedAmount': instance.expectedAmount?.toJson(),
  'credits': instance.credits,
  'senderName': instance.senderName,
  'hasReceipt': instance.hasReceipt,
  'status': _$PaymentNoticeStatusEnumMap[instance.status]!,
  'notifiedUtc': instance.notifiedUtc.toIso8601String(),
  'resolvedUtc': ?instance.resolvedUtc?.toIso8601String(),
};

const _$PaymentNoticePurposeEnumMap = {
  PaymentNoticePurpose.subscription: 'Subscription',
  PaymentNoticePurpose.credits: 'Credits',
  PaymentNoticePurpose.unknownDefaultOpenApi: 'unknown_default_open_api',
};

const _$PaymentNoticeStatusEnumMap = {
  PaymentNoticeStatus.pending: 'Pending',
  PaymentNoticeStatus.matched: 'Matched',
  PaymentNoticeStatus.notFound: 'NotFound',
  PaymentNoticeStatus.rejected: 'Rejected',
  PaymentNoticeStatus.unknownDefaultOpenApi: 'unknown_default_open_api',
};
