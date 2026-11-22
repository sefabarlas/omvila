// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_payment_notice_detail.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminPaymentNoticeDetailCWProxy {
  AdminPaymentNoticeDetail id(String id);

  AdminPaymentNoticeDetail brandId(String brandId);

  AdminPaymentNoticeDetail brandName(String brandName);

  AdminPaymentNoticeDetail purpose(PaymentNoticePurpose purpose);

  AdminPaymentNoticeDetail planName(String? planName);

  AdminPaymentNoticeDetail periodStart(DateTime? periodStart);

  AdminPaymentNoticeDetail periodEnd(DateTime? periodEnd);

  AdminPaymentNoticeDetail credits(int? credits);

  AdminPaymentNoticeDetail expectedAmount(Money? expectedAmount);

  AdminPaymentNoticeDetail transferCode(String transferCode);

  AdminPaymentNoticeDetail senderName(String? senderName);

  AdminPaymentNoticeDetail notifiedUtc(DateTime notifiedUtc);

  AdminPaymentNoticeDetail channel(AdminPaymentNoticeDetailChannelEnum channel);

  AdminPaymentNoticeDetail status(PaymentNoticeStatus status);

  AdminPaymentNoticeDetail receipt(AdminPaymentNoticeDetailReceipt? receipt);

  AdminPaymentNoticeDetail resolution(
    AdminPaymentNoticeDetailResolution? resolution,
  );

  AdminPaymentNoticeDetail candidateWindow(
    AdminPaymentNoticeDetailCandidateWindow candidateWindow,
  );

  AdminPaymentNoticeDetail candidates(
    List<BankTransactionCandidate> candidates,
  );

  AdminPaymentNoticeDetail version(String version);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminPaymentNoticeDetail(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminPaymentNoticeDetail(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminPaymentNoticeDetail call({
    String id,
    String brandId,
    String brandName,
    PaymentNoticePurpose purpose,
    String? planName,
    DateTime? periodStart,
    DateTime? periodEnd,
    int? credits,
    Money? expectedAmount,
    String transferCode,
    String? senderName,
    DateTime notifiedUtc,
    AdminPaymentNoticeDetailChannelEnum channel,
    PaymentNoticeStatus status,
    AdminPaymentNoticeDetailReceipt? receipt,
    AdminPaymentNoticeDetailResolution? resolution,
    AdminPaymentNoticeDetailCandidateWindow candidateWindow,
    List<BankTransactionCandidate> candidates,
    String version,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminPaymentNoticeDetail.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminPaymentNoticeDetail.copyWith.fieldName(...)`
class _$AdminPaymentNoticeDetailCWProxyImpl
    implements _$AdminPaymentNoticeDetailCWProxy {
  const _$AdminPaymentNoticeDetailCWProxyImpl(this._value);

  final AdminPaymentNoticeDetail _value;

  @override
  AdminPaymentNoticeDetail id(String id) => this(id: id);

  @override
  AdminPaymentNoticeDetail brandId(String brandId) => this(brandId: brandId);

  @override
  AdminPaymentNoticeDetail brandName(String brandName) =>
      this(brandName: brandName);

  @override
  AdminPaymentNoticeDetail purpose(PaymentNoticePurpose purpose) =>
      this(purpose: purpose);

  @override
  AdminPaymentNoticeDetail planName(String? planName) =>
      this(planName: planName);

  @override
  AdminPaymentNoticeDetail periodStart(DateTime? periodStart) =>
      this(periodStart: periodStart);

  @override
  AdminPaymentNoticeDetail periodEnd(DateTime? periodEnd) =>
      this(periodEnd: periodEnd);

  @override
  AdminPaymentNoticeDetail credits(int? credits) => this(credits: credits);

  @override
  AdminPaymentNoticeDetail expectedAmount(Money? expectedAmount) =>
      this(expectedAmount: expectedAmount);

  @override
  AdminPaymentNoticeDetail transferCode(String transferCode) =>
      this(transferCode: transferCode);

  @override
  AdminPaymentNoticeDetail senderName(String? senderName) =>
      this(senderName: senderName);

  @override
  AdminPaymentNoticeDetail notifiedUtc(DateTime notifiedUtc) =>
      this(notifiedUtc: notifiedUtc);

  @override
  AdminPaymentNoticeDetail channel(
    AdminPaymentNoticeDetailChannelEnum channel,
  ) => this(channel: channel);

  @override
  AdminPaymentNoticeDetail status(PaymentNoticeStatus status) =>
      this(status: status);

  @override
  AdminPaymentNoticeDetail receipt(AdminPaymentNoticeDetailReceipt? receipt) =>
      this(receipt: receipt);

  @override
  AdminPaymentNoticeDetail resolution(
    AdminPaymentNoticeDetailResolution? resolution,
  ) => this(resolution: resolution);

  @override
  AdminPaymentNoticeDetail candidateWindow(
    AdminPaymentNoticeDetailCandidateWindow candidateWindow,
  ) => this(candidateWindow: candidateWindow);

  @override
  AdminPaymentNoticeDetail candidates(
    List<BankTransactionCandidate> candidates,
  ) => this(candidates: candidates);

  @override
  AdminPaymentNoticeDetail version(String version) => this(version: version);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminPaymentNoticeDetail(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminPaymentNoticeDetail(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminPaymentNoticeDetail call({
    Object? id = const $CopyWithPlaceholder(),
    Object? brandId = const $CopyWithPlaceholder(),
    Object? brandName = const $CopyWithPlaceholder(),
    Object? purpose = const $CopyWithPlaceholder(),
    Object? planName = const $CopyWithPlaceholder(),
    Object? periodStart = const $CopyWithPlaceholder(),
    Object? periodEnd = const $CopyWithPlaceholder(),
    Object? credits = const $CopyWithPlaceholder(),
    Object? expectedAmount = const $CopyWithPlaceholder(),
    Object? transferCode = const $CopyWithPlaceholder(),
    Object? senderName = const $CopyWithPlaceholder(),
    Object? notifiedUtc = const $CopyWithPlaceholder(),
    Object? channel = const $CopyWithPlaceholder(),
    Object? status = const $CopyWithPlaceholder(),
    Object? receipt = const $CopyWithPlaceholder(),
    Object? resolution = const $CopyWithPlaceholder(),
    Object? candidateWindow = const $CopyWithPlaceholder(),
    Object? candidates = const $CopyWithPlaceholder(),
    Object? version = const $CopyWithPlaceholder(),
  }) {
    return AdminPaymentNoticeDetail(
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as String,
      brandId: brandId == const $CopyWithPlaceholder()
          ? _value.brandId
          // ignore: cast_nullable_to_non_nullable
          : brandId as String,
      brandName: brandName == const $CopyWithPlaceholder()
          ? _value.brandName
          // ignore: cast_nullable_to_non_nullable
          : brandName as String,
      purpose: purpose == const $CopyWithPlaceholder()
          ? _value.purpose
          // ignore: cast_nullable_to_non_nullable
          : purpose as PaymentNoticePurpose,
      planName: planName == const $CopyWithPlaceholder()
          ? _value.planName
          // ignore: cast_nullable_to_non_nullable
          : planName as String?,
      periodStart: periodStart == const $CopyWithPlaceholder()
          ? _value.periodStart
          // ignore: cast_nullable_to_non_nullable
          : periodStart as DateTime?,
      periodEnd: periodEnd == const $CopyWithPlaceholder()
          ? _value.periodEnd
          // ignore: cast_nullable_to_non_nullable
          : periodEnd as DateTime?,
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
      notifiedUtc: notifiedUtc == const $CopyWithPlaceholder()
          ? _value.notifiedUtc
          // ignore: cast_nullable_to_non_nullable
          : notifiedUtc as DateTime,
      channel: channel == const $CopyWithPlaceholder()
          ? _value.channel
          // ignore: cast_nullable_to_non_nullable
          : channel as AdminPaymentNoticeDetailChannelEnum,
      status: status == const $CopyWithPlaceholder()
          ? _value.status
          // ignore: cast_nullable_to_non_nullable
          : status as PaymentNoticeStatus,
      receipt: receipt == const $CopyWithPlaceholder()
          ? _value.receipt
          // ignore: cast_nullable_to_non_nullable
          : receipt as AdminPaymentNoticeDetailReceipt?,
      resolution: resolution == const $CopyWithPlaceholder()
          ? _value.resolution
          // ignore: cast_nullable_to_non_nullable
          : resolution as AdminPaymentNoticeDetailResolution?,
      candidateWindow: candidateWindow == const $CopyWithPlaceholder()
          ? _value.candidateWindow
          // ignore: cast_nullable_to_non_nullable
          : candidateWindow as AdminPaymentNoticeDetailCandidateWindow,
      candidates: candidates == const $CopyWithPlaceholder()
          ? _value.candidates
          // ignore: cast_nullable_to_non_nullable
          : candidates as List<BankTransactionCandidate>,
      version: version == const $CopyWithPlaceholder()
          ? _value.version
          // ignore: cast_nullable_to_non_nullable
          : version as String,
    );
  }
}

extension $AdminPaymentNoticeDetailCopyWith on AdminPaymentNoticeDetail {
  /// Returns a callable class that can be used as follows: `instanceOfAdminPaymentNoticeDetail.copyWith(...)` or like so:`instanceOfAdminPaymentNoticeDetail.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminPaymentNoticeDetailCWProxy get copyWith =>
      _$AdminPaymentNoticeDetailCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminPaymentNoticeDetail _$AdminPaymentNoticeDetailFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('AdminPaymentNoticeDetail', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const [
      'id',
      'brandId',
      'brandName',
      'purpose',
      'planName',
      'periodStart',
      'periodEnd',
      'credits',
      'expectedAmount',
      'transferCode',
      'senderName',
      'notifiedUtc',
      'channel',
      'status',
      'receipt',
      'resolution',
      'candidateWindow',
      'candidates',
      'version',
    ],
  );
  final val = AdminPaymentNoticeDetail(
    id: $checkedConvert('id', (v) => v as String),
    brandId: $checkedConvert('brandId', (v) => v as String),
    brandName: $checkedConvert('brandName', (v) => v as String),
    purpose: $checkedConvert(
      'purpose',
      (v) => $enumDecode(
        _$PaymentNoticePurposeEnumMap,
        v,
        unknownValue: PaymentNoticePurpose.unknownDefaultOpenApi,
      ),
    ),
    planName: $checkedConvert('planName', (v) => v as String?),
    periodStart: $checkedConvert(
      'periodStart',
      (v) => _$JsonConverterFromJson<String, DateTime>(
        v,
        const DateOnlyConverter().fromJson,
      ),
    ),
    periodEnd: $checkedConvert(
      'periodEnd',
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
    notifiedUtc: $checkedConvert(
      'notifiedUtc',
      (v) => DateTime.parse(v as String),
    ),
    channel: $checkedConvert(
      'channel',
      (v) => $enumDecode(
        _$AdminPaymentNoticeDetailChannelEnumEnumMap,
        v,
        unknownValue: AdminPaymentNoticeDetailChannelEnum.unknownDefaultOpenApi,
      ),
    ),
    status: $checkedConvert(
      'status',
      (v) => $enumDecode(
        _$PaymentNoticeStatusEnumMap,
        v,
        unknownValue: PaymentNoticeStatus.unknownDefaultOpenApi,
      ),
    ),
    receipt: $checkedConvert(
      'receipt',
      (v) => v == null
          ? null
          : AdminPaymentNoticeDetailReceipt.fromJson(v as Map<String, dynamic>),
    ),
    resolution: $checkedConvert(
      'resolution',
      (v) => v == null
          ? null
          : AdminPaymentNoticeDetailResolution.fromJson(
              v as Map<String, dynamic>,
            ),
    ),
    candidateWindow: $checkedConvert(
      'candidateWindow',
      (v) => AdminPaymentNoticeDetailCandidateWindow.fromJson(
        v as Map<String, dynamic>,
      ),
    ),
    candidates: $checkedConvert(
      'candidates',
      (v) => (v as List<dynamic>)
          .map(
            (e) => BankTransactionCandidate.fromJson(e as Map<String, dynamic>),
          )
          .toList(),
    ),
    version: $checkedConvert('version', (v) => v as String),
  );
  return val;
});

Map<String, dynamic> _$AdminPaymentNoticeDetailToJson(
  AdminPaymentNoticeDetail instance,
) => <String, dynamic>{
  'id': instance.id,
  'brandId': instance.brandId,
  'brandName': instance.brandName,
  'purpose': _$PaymentNoticePurposeEnumMap[instance.purpose]!,
  'planName': instance.planName,
  'periodStart': _$JsonConverterToJson<String, DateTime>(
    instance.periodStart,
    const DateOnlyConverter().toJson,
  ),
  'periodEnd': _$JsonConverterToJson<String, DateTime>(
    instance.periodEnd,
    const DateOnlyConverter().toJson,
  ),
  'credits': instance.credits,
  'expectedAmount': instance.expectedAmount?.toJson(),
  'transferCode': instance.transferCode,
  'senderName': instance.senderName,
  'notifiedUtc': instance.notifiedUtc.toIso8601String(),
  'channel': _$AdminPaymentNoticeDetailChannelEnumEnumMap[instance.channel]!,
  'status': _$PaymentNoticeStatusEnumMap[instance.status]!,
  'receipt': instance.receipt?.toJson(),
  'resolution': instance.resolution?.toJson(),
  'candidateWindow': instance.candidateWindow.toJson(),
  'candidates': instance.candidates.map((e) => e.toJson()).toList(),
  'version': instance.version,
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

const _$AdminPaymentNoticeDetailChannelEnumEnumMap = {
  AdminPaymentNoticeDetailChannelEnum.panel: 'Panel',
  AdminPaymentNoticeDetailChannelEnum.app: 'App',
  AdminPaymentNoticeDetailChannelEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};

const _$PaymentNoticeStatusEnumMap = {
  PaymentNoticeStatus.pending: 'Pending',
  PaymentNoticeStatus.matched: 'Matched',
  PaymentNoticeStatus.notFound: 'NotFound',
  PaymentNoticeStatus.rejected: 'Rejected',
  PaymentNoticeStatus.unknownDefaultOpenApi: 'unknown_default_open_api',
};

Json? _$JsonConverterToJson<Json, Value>(
  Value? value,
  Json? Function(Value value) toJson,
) => value == null ? null : toJson(value);
