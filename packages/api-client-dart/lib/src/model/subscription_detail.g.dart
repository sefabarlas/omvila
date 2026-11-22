// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'subscription_detail.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$SubscriptionDetailCWProxy {
  SubscriptionDetail status(SubscriptionStatus status);

  SubscriptionDetail plan(PlanRef plan);

  SubscriptionDetail billingCycle(BillingCycle billingCycle);

  SubscriptionDetail paymentMethod(SubscriptionPaymentMethod? paymentMethod);

  SubscriptionDetail trialEndsOn(DateTime? trialEndsOn);

  SubscriptionDetail currentPeriodStart(DateTime currentPeriodStart);

  SubscriptionDetail currentPeriodEnd(DateTime currentPeriodEnd);

  SubscriptionDetail pastDueSince(DateTime? pastDueSince);

  SubscriptionDetail readOnlyOn(DateTime? readOnlyOn);

  SubscriptionDetail readOnlySince(DateTime? readOnlySince);

  SubscriptionDetail branches(SubscriptionDetailBranches branches);

  SubscriptionDetail staff(SubscriptionDetailStaff staff);

  SubscriptionDetail amountDue(Money? amountDue);

  SubscriptionDetail transferCode(String transferCode);

  SubscriptionDetail omvilaBankAccount(OmvilaBankAccount? omvilaBankAccount);

  SubscriptionDetail pendingNotice(PaymentNoticeSummary? pendingNotice);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `SubscriptionDetail(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// SubscriptionDetail(...).copyWith(id: 12, name: "My name")
  /// ````
  SubscriptionDetail call({
    SubscriptionStatus status,
    PlanRef plan,
    BillingCycle billingCycle,
    SubscriptionPaymentMethod? paymentMethod,
    DateTime? trialEndsOn,
    DateTime currentPeriodStart,
    DateTime currentPeriodEnd,
    DateTime? pastDueSince,
    DateTime? readOnlyOn,
    DateTime? readOnlySince,
    SubscriptionDetailBranches branches,
    SubscriptionDetailStaff staff,
    Money? amountDue,
    String transferCode,
    OmvilaBankAccount? omvilaBankAccount,
    PaymentNoticeSummary? pendingNotice,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfSubscriptionDetail.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfSubscriptionDetail.copyWith.fieldName(...)`
class _$SubscriptionDetailCWProxyImpl implements _$SubscriptionDetailCWProxy {
  const _$SubscriptionDetailCWProxyImpl(this._value);

  final SubscriptionDetail _value;

  @override
  SubscriptionDetail status(SubscriptionStatus status) => this(status: status);

  @override
  SubscriptionDetail plan(PlanRef plan) => this(plan: plan);

  @override
  SubscriptionDetail billingCycle(BillingCycle billingCycle) =>
      this(billingCycle: billingCycle);

  @override
  SubscriptionDetail paymentMethod(SubscriptionPaymentMethod? paymentMethod) =>
      this(paymentMethod: paymentMethod);

  @override
  SubscriptionDetail trialEndsOn(DateTime? trialEndsOn) =>
      this(trialEndsOn: trialEndsOn);

  @override
  SubscriptionDetail currentPeriodStart(DateTime currentPeriodStart) =>
      this(currentPeriodStart: currentPeriodStart);

  @override
  SubscriptionDetail currentPeriodEnd(DateTime currentPeriodEnd) =>
      this(currentPeriodEnd: currentPeriodEnd);

  @override
  SubscriptionDetail pastDueSince(DateTime? pastDueSince) =>
      this(pastDueSince: pastDueSince);

  @override
  SubscriptionDetail readOnlyOn(DateTime? readOnlyOn) =>
      this(readOnlyOn: readOnlyOn);

  @override
  SubscriptionDetail readOnlySince(DateTime? readOnlySince) =>
      this(readOnlySince: readOnlySince);

  @override
  SubscriptionDetail branches(SubscriptionDetailBranches branches) =>
      this(branches: branches);

  @override
  SubscriptionDetail staff(SubscriptionDetailStaff staff) => this(staff: staff);

  @override
  SubscriptionDetail amountDue(Money? amountDue) => this(amountDue: amountDue);

  @override
  SubscriptionDetail transferCode(String transferCode) =>
      this(transferCode: transferCode);

  @override
  SubscriptionDetail omvilaBankAccount(OmvilaBankAccount? omvilaBankAccount) =>
      this(omvilaBankAccount: omvilaBankAccount);

  @override
  SubscriptionDetail pendingNotice(PaymentNoticeSummary? pendingNotice) =>
      this(pendingNotice: pendingNotice);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `SubscriptionDetail(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// SubscriptionDetail(...).copyWith(id: 12, name: "My name")
  /// ````
  SubscriptionDetail call({
    Object? status = const $CopyWithPlaceholder(),
    Object? plan = const $CopyWithPlaceholder(),
    Object? billingCycle = const $CopyWithPlaceholder(),
    Object? paymentMethod = const $CopyWithPlaceholder(),
    Object? trialEndsOn = const $CopyWithPlaceholder(),
    Object? currentPeriodStart = const $CopyWithPlaceholder(),
    Object? currentPeriodEnd = const $CopyWithPlaceholder(),
    Object? pastDueSince = const $CopyWithPlaceholder(),
    Object? readOnlyOn = const $CopyWithPlaceholder(),
    Object? readOnlySince = const $CopyWithPlaceholder(),
    Object? branches = const $CopyWithPlaceholder(),
    Object? staff = const $CopyWithPlaceholder(),
    Object? amountDue = const $CopyWithPlaceholder(),
    Object? transferCode = const $CopyWithPlaceholder(),
    Object? omvilaBankAccount = const $CopyWithPlaceholder(),
    Object? pendingNotice = const $CopyWithPlaceholder(),
  }) {
    return SubscriptionDetail(
      status: status == const $CopyWithPlaceholder()
          ? _value.status
          // ignore: cast_nullable_to_non_nullable
          : status as SubscriptionStatus,
      plan: plan == const $CopyWithPlaceholder()
          ? _value.plan
          // ignore: cast_nullable_to_non_nullable
          : plan as PlanRef,
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
      currentPeriodStart: currentPeriodStart == const $CopyWithPlaceholder()
          ? _value.currentPeriodStart
          // ignore: cast_nullable_to_non_nullable
          : currentPeriodStart as DateTime,
      currentPeriodEnd: currentPeriodEnd == const $CopyWithPlaceholder()
          ? _value.currentPeriodEnd
          // ignore: cast_nullable_to_non_nullable
          : currentPeriodEnd as DateTime,
      pastDueSince: pastDueSince == const $CopyWithPlaceholder()
          ? _value.pastDueSince
          // ignore: cast_nullable_to_non_nullable
          : pastDueSince as DateTime?,
      readOnlyOn: readOnlyOn == const $CopyWithPlaceholder()
          ? _value.readOnlyOn
          // ignore: cast_nullable_to_non_nullable
          : readOnlyOn as DateTime?,
      readOnlySince: readOnlySince == const $CopyWithPlaceholder()
          ? _value.readOnlySince
          // ignore: cast_nullable_to_non_nullable
          : readOnlySince as DateTime?,
      branches: branches == const $CopyWithPlaceholder()
          ? _value.branches
          // ignore: cast_nullable_to_non_nullable
          : branches as SubscriptionDetailBranches,
      staff: staff == const $CopyWithPlaceholder()
          ? _value.staff
          // ignore: cast_nullable_to_non_nullable
          : staff as SubscriptionDetailStaff,
      amountDue: amountDue == const $CopyWithPlaceholder()
          ? _value.amountDue
          // ignore: cast_nullable_to_non_nullable
          : amountDue as Money?,
      transferCode: transferCode == const $CopyWithPlaceholder()
          ? _value.transferCode
          // ignore: cast_nullable_to_non_nullable
          : transferCode as String,
      omvilaBankAccount: omvilaBankAccount == const $CopyWithPlaceholder()
          ? _value.omvilaBankAccount
          // ignore: cast_nullable_to_non_nullable
          : omvilaBankAccount as OmvilaBankAccount?,
      pendingNotice: pendingNotice == const $CopyWithPlaceholder()
          ? _value.pendingNotice
          // ignore: cast_nullable_to_non_nullable
          : pendingNotice as PaymentNoticeSummary?,
    );
  }
}

extension $SubscriptionDetailCopyWith on SubscriptionDetail {
  /// Returns a callable class that can be used as follows: `instanceOfSubscriptionDetail.copyWith(...)` or like so:`instanceOfSubscriptionDetail.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$SubscriptionDetailCWProxy get copyWith =>
      _$SubscriptionDetailCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

SubscriptionDetail _$SubscriptionDetailFromJson(Map<String, dynamic> json) =>
    $checkedCreate('SubscriptionDetail', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'status',
          'plan',
          'billingCycle',
          'paymentMethod',
          'trialEndsOn',
          'currentPeriodStart',
          'currentPeriodEnd',
          'pastDueSince',
          'readOnlyOn',
          'readOnlySince',
          'branches',
          'staff',
          'amountDue',
          'transferCode',
          'omvilaBankAccount',
          'pendingNotice',
        ],
      );
      final val = SubscriptionDetail(
        status: $checkedConvert(
          'status',
          (v) => $enumDecode(
            _$SubscriptionStatusEnumMap,
            v,
            unknownValue: SubscriptionStatus.unknownDefaultOpenApi,
          ),
        ),
        plan: $checkedConvert(
          'plan',
          (v) => PlanRef.fromJson(v as Map<String, dynamic>),
        ),
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
        currentPeriodStart: $checkedConvert(
          'currentPeriodStart',
          (v) => const DateOnlyConverter().fromJson(v as String),
        ),
        currentPeriodEnd: $checkedConvert(
          'currentPeriodEnd',
          (v) => const DateOnlyConverter().fromJson(v as String),
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
        readOnlySince: $checkedConvert(
          'readOnlySince',
          (v) => _$JsonConverterFromJson<String, DateTime>(
            v,
            const DateOnlyConverter().fromJson,
          ),
        ),
        branches: $checkedConvert(
          'branches',
          (v) => SubscriptionDetailBranches.fromJson(v as Map<String, dynamic>),
        ),
        staff: $checkedConvert(
          'staff',
          (v) => SubscriptionDetailStaff.fromJson(v as Map<String, dynamic>),
        ),
        amountDue: $checkedConvert(
          'amountDue',
          (v) => v == null ? null : Money.fromJson(v as Map<String, dynamic>),
        ),
        transferCode: $checkedConvert('transferCode', (v) => v as String),
        omvilaBankAccount: $checkedConvert(
          'omvilaBankAccount',
          (v) => v == null
              ? null
              : OmvilaBankAccount.fromJson(v as Map<String, dynamic>),
        ),
        pendingNotice: $checkedConvert(
          'pendingNotice',
          (v) => v == null
              ? null
              : PaymentNoticeSummary.fromJson(v as Map<String, dynamic>),
        ),
      );
      return val;
    });

Map<String, dynamic> _$SubscriptionDetailToJson(
  SubscriptionDetail instance,
) => <String, dynamic>{
  'status': _$SubscriptionStatusEnumMap[instance.status]!,
  'plan': instance.plan.toJson(),
  'billingCycle': _$BillingCycleEnumMap[instance.billingCycle]!,
  'paymentMethod': _$SubscriptionPaymentMethodEnumMap[instance.paymentMethod],
  'trialEndsOn': _$JsonConverterToJson<String, DateTime>(
    instance.trialEndsOn,
    const DateOnlyConverter().toJson,
  ),
  'currentPeriodStart': const DateOnlyConverter().toJson(
    instance.currentPeriodStart,
  ),
  'currentPeriodEnd': const DateOnlyConverter().toJson(
    instance.currentPeriodEnd,
  ),
  'pastDueSince': _$JsonConverterToJson<String, DateTime>(
    instance.pastDueSince,
    const DateOnlyConverter().toJson,
  ),
  'readOnlyOn': _$JsonConverterToJson<String, DateTime>(
    instance.readOnlyOn,
    const DateOnlyConverter().toJson,
  ),
  'readOnlySince': _$JsonConverterToJson<String, DateTime>(
    instance.readOnlySince,
    const DateOnlyConverter().toJson,
  ),
  'branches': instance.branches.toJson(),
  'staff': instance.staff.toJson(),
  'amountDue': instance.amountDue?.toJson(),
  'transferCode': instance.transferCode,
  'omvilaBankAccount': instance.omvilaBankAccount?.toJson(),
  'pendingNotice': instance.pendingNotice?.toJson(),
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
