// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_approval_queue_item_payment_notice.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminApprovalQueueItemPaymentNoticeCWProxy {
  AdminApprovalQueueItemPaymentNotice purpose(PaymentNoticePurpose purpose);

  AdminApprovalQueueItemPaymentNotice periodStart(DateTime? periodStart);

  AdminApprovalQueueItemPaymentNotice credits(int? credits);

  AdminApprovalQueueItemPaymentNotice expectedAmount(Money? expectedAmount);

  AdminApprovalQueueItemPaymentNotice transferCode(String transferCode);

  AdminApprovalQueueItemPaymentNotice senderName(String? senderName);

  AdminApprovalQueueItemPaymentNotice hasReceipt(bool hasReceipt);

  AdminApprovalQueueItemPaymentNotice version(String version);

  AdminApprovalQueueItemPaymentNotice candidate(
    AdminApprovalQueueItemPaymentNoticeCandidate? candidate,
  );

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminApprovalQueueItemPaymentNotice(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminApprovalQueueItemPaymentNotice(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminApprovalQueueItemPaymentNotice call({
    PaymentNoticePurpose purpose,
    DateTime? periodStart,
    int? credits,
    Money? expectedAmount,
    String transferCode,
    String? senderName,
    bool hasReceipt,
    String version,
    AdminApprovalQueueItemPaymentNoticeCandidate? candidate,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminApprovalQueueItemPaymentNotice.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminApprovalQueueItemPaymentNotice.copyWith.fieldName(...)`
class _$AdminApprovalQueueItemPaymentNoticeCWProxyImpl
    implements _$AdminApprovalQueueItemPaymentNoticeCWProxy {
  const _$AdminApprovalQueueItemPaymentNoticeCWProxyImpl(this._value);

  final AdminApprovalQueueItemPaymentNotice _value;

  @override
  AdminApprovalQueueItemPaymentNotice purpose(PaymentNoticePurpose purpose) =>
      this(purpose: purpose);

  @override
  AdminApprovalQueueItemPaymentNotice periodStart(DateTime? periodStart) =>
      this(periodStart: periodStart);

  @override
  AdminApprovalQueueItemPaymentNotice credits(int? credits) =>
      this(credits: credits);

  @override
  AdminApprovalQueueItemPaymentNotice expectedAmount(Money? expectedAmount) =>
      this(expectedAmount: expectedAmount);

  @override
  AdminApprovalQueueItemPaymentNotice transferCode(String transferCode) =>
      this(transferCode: transferCode);

  @override
  AdminApprovalQueueItemPaymentNotice senderName(String? senderName) =>
      this(senderName: senderName);

  @override
  AdminApprovalQueueItemPaymentNotice hasReceipt(bool hasReceipt) =>
      this(hasReceipt: hasReceipt);

  @override
  AdminApprovalQueueItemPaymentNotice version(String version) =>
      this(version: version);

  @override
  AdminApprovalQueueItemPaymentNotice candidate(
    AdminApprovalQueueItemPaymentNoticeCandidate? candidate,
  ) => this(candidate: candidate);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminApprovalQueueItemPaymentNotice(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminApprovalQueueItemPaymentNotice(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminApprovalQueueItemPaymentNotice call({
    Object? purpose = const $CopyWithPlaceholder(),
    Object? periodStart = const $CopyWithPlaceholder(),
    Object? credits = const $CopyWithPlaceholder(),
    Object? expectedAmount = const $CopyWithPlaceholder(),
    Object? transferCode = const $CopyWithPlaceholder(),
    Object? senderName = const $CopyWithPlaceholder(),
    Object? hasReceipt = const $CopyWithPlaceholder(),
    Object? version = const $CopyWithPlaceholder(),
    Object? candidate = const $CopyWithPlaceholder(),
  }) {
    return AdminApprovalQueueItemPaymentNotice(
      purpose: purpose == const $CopyWithPlaceholder()
          ? _value.purpose
          // ignore: cast_nullable_to_non_nullable
          : purpose as PaymentNoticePurpose,
      periodStart: periodStart == const $CopyWithPlaceholder()
          ? _value.periodStart
          // ignore: cast_nullable_to_non_nullable
          : periodStart as DateTime?,
      credits: credits == const $CopyWithPlaceholder()
          ? _value.credits
          // ignore: cast_nullable_to_non_nullable
          : credits as int?,
      expectedAmount: expectedAmount == const $CopyWithPlaceholder()
          ? _value.expectedAmount
          // ignore: cast_nullable_to_non_nullable
          : expectedAmount as Money?,
      transferCode: transferCode == const $CopyWithPlaceholder()
          ? _value.transferCode
          // ignore: cast_nullable_to_non_nullable
          : transferCode as String,
      senderName: senderName == const $CopyWithPlaceholder()
          ? _value.senderName
          // ignore: cast_nullable_to_non_nullable
          : senderName as String?,
      hasReceipt: hasReceipt == const $CopyWithPlaceholder()
          ? _value.hasReceipt
          // ignore: cast_nullable_to_non_nullable
          : hasReceipt as bool,
      version: version == const $CopyWithPlaceholder()
          ? _value.version
          // ignore: cast_nullable_to_non_nullable
          : version as String,
      candidate: candidate == const $CopyWithPlaceholder()
          ? _value.candidate
          // ignore: cast_nullable_to_non_nullable
          : candidate as AdminApprovalQueueItemPaymentNoticeCandidate?,
    );
  }
}

extension $AdminApprovalQueueItemPaymentNoticeCopyWith
    on AdminApprovalQueueItemPaymentNotice {
  /// Returns a callable class that can be used as follows: `instanceOfAdminApprovalQueueItemPaymentNotice.copyWith(...)` or like so:`instanceOfAdminApprovalQueueItemPaymentNotice.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminApprovalQueueItemPaymentNoticeCWProxy get copyWith =>
      _$AdminApprovalQueueItemPaymentNoticeCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminApprovalQueueItemPaymentNotice
_$AdminApprovalQueueItemPaymentNoticeFromJson(Map<String, dynamic> json) =>
    $checkedCreate('AdminApprovalQueueItemPaymentNotice', json, (
      $checkedConvert,
    ) {
      $checkKeys(
        json,
        requiredKeys: const [
          'purpose',
          'periodStart',
          'credits',
          'expectedAmount',
          'transferCode',
          'senderName',
          'hasReceipt',
          'version',
          'candidate',
        ],
      );
      final val = AdminApprovalQueueItemPaymentNotice(
        purpose: $checkedConvert(
          'purpose',
          (v) => $enumDecode(
            _$PaymentNoticePurposeEnumMap,
            v,
            unknownValue: PaymentNoticePurpose.unknownDefaultOpenApi,
          ),
        ),
        periodStart: $checkedConvert(
          'periodStart',
          (v) => _$JsonConverterFromJson<String, DateTime>(
            v,
            const DateOnlyConverter().fromJson,
          ),
        ),
        credits: $checkedConvert('credits', (v) => (v as num?)?.toInt()),
        expectedAmount: $checkedConvert(
          'expectedAmount',
          (v) => v == null ? null : Money.fromJson(v as Map<String, dynamic>),
        ),
        transferCode: $checkedConvert('transferCode', (v) => v as String),
        senderName: $checkedConvert('senderName', (v) => v as String?),
        hasReceipt: $checkedConvert('hasReceipt', (v) => v as bool),
        version: $checkedConvert('version', (v) => v as String),
        candidate: $checkedConvert(
          'candidate',
          (v) => v == null
              ? null
              : AdminApprovalQueueItemPaymentNoticeCandidate.fromJson(
                  v as Map<String, dynamic>,
                ),
        ),
      );
      return val;
    });

Map<String, dynamic> _$AdminApprovalQueueItemPaymentNoticeToJson(
  AdminApprovalQueueItemPaymentNotice instance,
) => <String, dynamic>{
  'purpose': _$PaymentNoticePurposeEnumMap[instance.purpose]!,
  'periodStart': _$JsonConverterToJson<String, DateTime>(
    instance.periodStart,
    const DateOnlyConverter().toJson,
  ),
  'credits': instance.credits,
  'expectedAmount': instance.expectedAmount?.toJson(),
  'transferCode': instance.transferCode,
  'senderName': instance.senderName,
  'hasReceipt': instance.hasReceipt,
  'version': instance.version,
  'candidate': instance.candidate?.toJson(),
};

const _$PaymentNoticePurposeEnumMap = {
  PaymentNoticePurpose.subscription: 'Subscription',
  PaymentNoticePurpose.credits: 'Credits',
  PaymentNoticePurpose.unknownDefaultOpenApi: 'unknown_default_open_api',
};

Value? _$JsonConverterFromJson<Json, Value>(
  Object? json,
  Value? Function(Json json) fromJson,
) => json == null ? null : fromJson(json as Json);

Json? _$JsonConverterToJson<Json, Value>(
  Value? value,
  Json? Function(Value value) toJson,
) => value == null ? null : toJson(value);
