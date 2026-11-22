// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_brand_detail_subscription.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminBrandDetailSubscriptionCWProxy {
  AdminBrandDetailSubscription status(SubscriptionStatus status);

  AdminBrandDetailSubscription planCode(String planCode);

  AdminBrandDetailSubscription planName(String planName);

  AdminBrandDetailSubscription billingCycle(BillingCycle billingCycle);

  AdminBrandDetailSubscription paymentMethod(
    SubscriptionPaymentMethod? paymentMethod,
  );

  AdminBrandDetailSubscription trialEndsOn(DateTime? trialEndsOn);

  AdminBrandDetailSubscription trialDaysLeft(int? trialDaysLeft);

  AdminBrandDetailSubscription pastDueSince(DateTime? pastDueSince);

  AdminBrandDetailSubscription readOnlyOn(DateTime? readOnlyOn);

  AdminBrandDetailSubscription transferCode(String transferCode);

  AdminBrandDetailSubscription pendingNoticeId(String? pendingNoticeId);

  AdminBrandDetailSubscription pendingNoticeNotifiedUtc(
    DateTime? pendingNoticeNotifiedUtc,
  );

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminBrandDetailSubscription(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminBrandDetailSubscription(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminBrandDetailSubscription call({
    SubscriptionStatus status,
    String planCode,
    String planName,
    BillingCycle billingCycle,
    SubscriptionPaymentMethod? paymentMethod,
    DateTime? trialEndsOn,
    int? trialDaysLeft,
    DateTime? pastDueSince,
    DateTime? readOnlyOn,
    String transferCode,
    String? pendingNoticeId,
    DateTime? pendingNoticeNotifiedUtc,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminBrandDetailSubscription.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminBrandDetailSubscription.copyWith.fieldName(...)`
class _$AdminBrandDetailSubscriptionCWProxyImpl
    implements _$AdminBrandDetailSubscriptionCWProxy {
  const _$AdminBrandDetailSubscriptionCWProxyImpl(this._value);

  final AdminBrandDetailSubscription _value;

  @override
  AdminBrandDetailSubscription status(SubscriptionStatus status) =>
      this(status: status);

  @override
  AdminBrandDetailSubscription planCode(String planCode) =>
      this(planCode: planCode);

  @override
  AdminBrandDetailSubscription planName(String planName) =>
      this(planName: planName);

  @override
  AdminBrandDetailSubscription billingCycle(BillingCycle billingCycle) =>
      this(billingCycle: billingCycle);

  @override
  AdminBrandDetailSubscription paymentMethod(
    SubscriptionPaymentMethod? paymentMethod,
  ) => this(paymentMethod: paymentMethod);

  @override
  AdminBrandDetailSubscription trialEndsOn(DateTime? trialEndsOn) =>
      this(trialEndsOn: trialEndsOn);

  @override
  AdminBrandDetailSubscription trialDaysLeft(int? trialDaysLeft) =>
      this(trialDaysLeft: trialDaysLeft);

  @override
  AdminBrandDetailSubscription pastDueSince(DateTime? pastDueSince) =>
      this(pastDueSince: pastDueSince);

  @override
  AdminBrandDetailSubscription readOnlyOn(DateTime? readOnlyOn) =>
      this(readOnlyOn: readOnlyOn);

  @override
  AdminBrandDetailSubscription transferCode(String transferCode) =>
      this(transferCode: transferCode);

  @override
  AdminBrandDetailSubscription pendingNoticeId(String? pendingNoticeId) =>
      this(pendingNoticeId: pendingNoticeId);

  @override
  AdminBrandDetailSubscription pendingNoticeNotifiedUtc(
    DateTime? pendingNoticeNotifiedUtc,
  ) => this(pendingNoticeNotifiedUtc: pendingNoticeNotifiedUtc);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminBrandDetailSubscription(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminBrandDetailSubscription(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminBrandDetailSubscription call({
    Object? status = const $CopyWithPlaceholder(),
    Object? planCode = const $CopyWithPlaceholder(),
    Object? planName = const $CopyWithPlaceholder(),
    Object? billingCycle = const $CopyWithPlaceholder(),
    Object? paymentMethod = const $CopyWithPlaceholder(),
    Object? trialEndsOn = const $CopyWithPlaceholder(),
    Object? trialDaysLeft = const $CopyWithPlaceholder(),
    Object? pastDueSince = const $CopyWithPlaceholder(),
    Object? readOnlyOn = const $CopyWithPlaceholder(),
    Object? transferCode = const $CopyWithPlaceholder(),
    Object? pendingNoticeId = const $CopyWithPlaceholder(),
    Object? pendingNoticeNotifiedUtc = const $CopyWithPlaceholder(),
  }) {
    return AdminBrandDetailSubscription(
      status: status == const $CopyWithPlaceholder()
          ? _value.status
          // ignore: cast_nullable_to_non_nullable
          : status as SubscriptionStatus,
      planCode: planCode == const $CopyWithPlaceholder()
          ? _value.planCode
          // ignore: cast_nullable_to_non_nullable
          : planCode as String,
      planName: planName == const $CopyWithPlaceholder()
          ? _value.planName
          // ignore: cast_nullable_to_non_nullable
          : planName as String,
      billingCycle: billingCycle == const $CopyWithPlaceholder()
          ? _value.billingCycle
          // ignore: cast_nullable_to_non_nullable
          : billingCycle as BillingCycle,
      paymentMethod: paymentMethod == const $CopyWithPlaceholder()
          ? _value.paymentMethod
          // ignore: cast_nullable_to_non_nullable
          : paymentMethod as SubscriptionPaymentMethod?,
      trialEndsOn: trialEndsOn == const $CopyWithPlaceholder()
          ? _value.trialEndsOn
          // ignore: cast_nullable_to_non_nullable
          : trialEndsOn as DateTime?,
      trialDaysLeft: trialDaysLeft == const $CopyWithPlaceholder()
          ? _value.trialDaysLeft
          // ignore: cast_nullable_to_non_nullable
          : trialDaysLeft as int?,
      pastDueSince: pastDueSince == const $CopyWithPlaceholder()
          ? _value.pastDueSince
          // ignore: cast_nullable_to_non_nullable
          : pastDueSince as DateTime?,
      readOnlyOn: readOnlyOn == const $CopyWithPlaceholder()
          ? _value.readOnlyOn
          // ignore: cast_nullable_to_non_nullable
          : readOnlyOn as DateTime?,
      transferCode: transferCode == const $CopyWithPlaceholder()
          ? _value.transferCode
          // ignore: cast_nullable_to_non_nullable
          : transferCode as String,
      pendingNoticeId: pendingNoticeId == const $CopyWithPlaceholder()
          ? _value.pendingNoticeId
          // ignore: cast_nullable_to_non_nullable
          : pendingNoticeId as String?,
      pendingNoticeNotifiedUtc:
          pendingNoticeNotifiedUtc == const $CopyWithPlaceholder()
          ? _value.pendingNoticeNotifiedUtc
          // ignore: cast_nullable_to_non_nullable
          : pendingNoticeNotifiedUtc as DateTime?,
    );
  }
}

extension $AdminBrandDetailSubscriptionCopyWith
    on AdminBrandDetailSubscription {
  /// Returns a callable class that can be used as follows: `instanceOfAdminBrandDetailSubscription.copyWith(...)` or like so:`instanceOfAdminBrandDetailSubscription.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminBrandDetailSubscriptionCWProxy get copyWith =>
      _$AdminBrandDetailSubscriptionCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminBrandDetailSubscription _$AdminBrandDetailSubscriptionFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('AdminBrandDetailSubscription', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const [
      'status',
      'planCode',
      'planName',
      'billingCycle',
      'paymentMethod',
      'trialEndsOn',
      'trialDaysLeft',
      'pastDueSince',
      'readOnlyOn',
      'transferCode',
      'pendingNoticeId',
      'pendingNoticeNotifiedUtc',
    ],
  );
  final val = AdminBrandDetailSubscription(
    status: $checkedConvert(
      'status',
      (v) => $enumDecode(
        _$SubscriptionStatusEnumMap,
        v,
        unknownValue: SubscriptionStatus.unknownDefaultOpenApi,
      ),
    ),
    planCode: $checkedConvert('planCode', (v) => v as String),
    planName: $checkedConvert('planName', (v) => v as String),
    billingCycle: $checkedConvert(
      'billingCycle',
      (v) => $enumDecode(
        _$BillingCycleEnumMap,
        v,
        unknownValue: BillingCycle.unknownDefaultOpenApi,
      ),
    ),
    paymentMethod: $checkedConvert(
      'paymentMethod',
      (v) => $enumDecodeNullable(
        _$SubscriptionPaymentMethodEnumMap,
        v,
        unknownValue: SubscriptionPaymentMethod.unknownDefaultOpenApi,
      ),
    ),
    trialEndsOn: $checkedConvert(
      'trialEndsOn',
      (v) => _$JsonConverterFromJson<String, DateTime>(
        v,
        const DateOnlyConverter().fromJson,
      ),
    ),
    trialDaysLeft: $checkedConvert(
      'trialDaysLeft',
      (v) => (v as num?)?.toInt(),
    ),
    pastDueSince: $checkedConvert(
      'pastDueSince',
      (v) => _$JsonConverterFromJson<String, DateTime>(
        v,
        const DateOnlyConverter().fromJson,
      ),
    ),
    readOnlyOn: $checkedConvert(
      'readOnlyOn',
      (v) => _$JsonConverterFromJson<String, DateTime>(
        v,
        const DateOnlyConverter().fromJson,
      ),
    ),
    transferCode: $checkedConvert('transferCode', (v) => v as String),
    pendingNoticeId: $checkedConvert('pendingNoticeId', (v) => v as String?),
    pendingNoticeNotifiedUtc: $checkedConvert(
      'pendingNoticeNotifiedUtc',
      (v) => v == null ? null : DateTime.parse(v as String),
    ),
  );
  return val;
});

Map<String, dynamic> _$AdminBrandDetailSubscriptionToJson(
  AdminBrandDetailSubscription instance,
) => <String, dynamic>{
  'status': _$SubscriptionStatusEnumMap[instance.status]!,
  'planCode': instance.planCode,
  'planName': instance.planName,
  'billingCycle': _$BillingCycleEnumMap[instance.billingCycle]!,
  'paymentMethod': _$SubscriptionPaymentMethodEnumMap[instance.paymentMethod],
  'trialEndsOn': _$JsonConverterToJson<String, DateTime>(
    instance.trialEndsOn,
    const DateOnlyConverter().toJson,
  ),
  'trialDaysLeft': instance.trialDaysLeft,
  'pastDueSince': _$JsonConverterToJson<String, DateTime>(
    instance.pastDueSince,
    const DateOnlyConverter().toJson,
  ),
  'readOnlyOn': _$JsonConverterToJson<String, DateTime>(
    instance.readOnlyOn,
    const DateOnlyConverter().toJson,
  ),
  'transferCode': instance.transferCode,
  'pendingNoticeId': instance.pendingNoticeId,
  'pendingNoticeNotifiedUtc': instance.pendingNoticeNotifiedUtc
      ?.toIso8601String(),
};

const _$SubscriptionStatusEnumMap = {
  SubscriptionStatus.trialing: 'Trialing',
  SubscriptionStatus.active: 'Active',
  SubscriptionStatus.pastDue: 'PastDue',
  SubscriptionStatus.readOnly: 'ReadOnly',
  SubscriptionStatus.cancelled: 'Cancelled',
  SubscriptionStatus.unknownDefaultOpenApi: 'unknown_default_open_api',
};

const _$BillingCycleEnumMap = {
  BillingCycle.monthly: 'Monthly',
  BillingCycle.yearly: 'Yearly',
  BillingCycle.unknownDefaultOpenApi: 'unknown_default_open_api',
};

const _$SubscriptionPaymentMethodEnumMap = {
  SubscriptionPaymentMethod.card: 'Card',
  SubscriptionPaymentMethod.bankTransfer: 'BankTransfer',
  SubscriptionPaymentMethod.unknownDefaultOpenApi: 'unknown_default_open_api',
};

Value? _$JsonConverterFromJson<Json, Value>(
  Object? json,
  Value? Function(Json json) fromJson,
) => json == null ? null : fromJson(json as Json);

Json? _$JsonConverterToJson<Json, Value>(
  Value? value,
  Json? Function(Value value) toJson,
) => value == null ? null : toJson(value);
