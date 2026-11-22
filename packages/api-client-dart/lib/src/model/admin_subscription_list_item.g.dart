// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_subscription_list_item.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminSubscriptionListItemCWProxy {
  AdminSubscriptionListItem brandId(String brandId);

  AdminSubscriptionListItem brandName(String brandName);

  AdminSubscriptionListItem planName(String planName);

  AdminSubscriptionListItem status(SubscriptionStatus status);

  AdminSubscriptionListItem pastDueDays(int? pastDueDays);

  AdminSubscriptionListItem trialDaysLeft(int? trialDaysLeft);

  AdminSubscriptionListItem creditBalance(int creditBalance);

  AdminSubscriptionListItem creditsLow(bool creditsLow);

  AdminSubscriptionListItem nextStep(
    AdminSubscriptionListItemNextStep nextStep,
  );

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminSubscriptionListItem(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminSubscriptionListItem(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminSubscriptionListItem call({
    String brandId,
    String brandName,
    String planName,
    SubscriptionStatus status,
    int? pastDueDays,
    int? trialDaysLeft,
    int creditBalance,
    bool creditsLow,
    AdminSubscriptionListItemNextStep nextStep,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminSubscriptionListItem.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminSubscriptionListItem.copyWith.fieldName(...)`
class _$AdminSubscriptionListItemCWProxyImpl
    implements _$AdminSubscriptionListItemCWProxy {
  const _$AdminSubscriptionListItemCWProxyImpl(this._value);

  final AdminSubscriptionListItem _value;

  @override
  AdminSubscriptionListItem brandId(String brandId) => this(brandId: brandId);

  @override
  AdminSubscriptionListItem brandName(String brandName) =>
      this(brandName: brandName);

  @override
  AdminSubscriptionListItem planName(String planName) =>
      this(planName: planName);

  @override
  AdminSubscriptionListItem status(SubscriptionStatus status) =>
      this(status: status);

  @override
  AdminSubscriptionListItem pastDueDays(int? pastDueDays) =>
      this(pastDueDays: pastDueDays);

  @override
  AdminSubscriptionListItem trialDaysLeft(int? trialDaysLeft) =>
      this(trialDaysLeft: trialDaysLeft);

  @override
  AdminSubscriptionListItem creditBalance(int creditBalance) =>
      this(creditBalance: creditBalance);

  @override
  AdminSubscriptionListItem creditsLow(bool creditsLow) =>
      this(creditsLow: creditsLow);

  @override
  AdminSubscriptionListItem nextStep(
    AdminSubscriptionListItemNextStep nextStep,
  ) => this(nextStep: nextStep);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminSubscriptionListItem(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminSubscriptionListItem(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminSubscriptionListItem call({
    Object? brandId = const $CopyWithPlaceholder(),
    Object? brandName = const $CopyWithPlaceholder(),
    Object? planName = const $CopyWithPlaceholder(),
    Object? status = const $CopyWithPlaceholder(),
    Object? pastDueDays = const $CopyWithPlaceholder(),
    Object? trialDaysLeft = const $CopyWithPlaceholder(),
    Object? creditBalance = const $CopyWithPlaceholder(),
    Object? creditsLow = const $CopyWithPlaceholder(),
    Object? nextStep = const $CopyWithPlaceholder(),
  }) {
    return AdminSubscriptionListItem(
      brandId: brandId == const $CopyWithPlaceholder()
          ? _value.brandId
          // ignore: cast_nullable_to_non_nullable
          : brandId as String,
      brandName: brandName == const $CopyWithPlaceholder()
          ? _value.brandName
          // ignore: cast_nullable_to_non_nullable
          : brandName as String,
      planName: planName == const $CopyWithPlaceholder()
          ? _value.planName
          // ignore: cast_nullable_to_non_nullable
          : planName as String,
      status: status == const $CopyWithPlaceholder()
          ? _value.status
          // ignore: cast_nullable_to_non_nullable
          : status as SubscriptionStatus,
      pastDueDays: pastDueDays == const $CopyWithPlaceholder()
          ? _value.pastDueDays
          // ignore: cast_nullable_to_non_nullable
          : pastDueDays as int?,
      trialDaysLeft: trialDaysLeft == const $CopyWithPlaceholder()
          ? _value.trialDaysLeft
          // ignore: cast_nullable_to_non_nullable
          : trialDaysLeft as int?,
      creditBalance: creditBalance == const $CopyWithPlaceholder()
          ? _value.creditBalance
          // ignore: cast_nullable_to_non_nullable
          : creditBalance as int,
      creditsLow: creditsLow == const $CopyWithPlaceholder()
          ? _value.creditsLow
          // ignore: cast_nullable_to_non_nullable
          : creditsLow as bool,
      nextStep: nextStep == const $CopyWithPlaceholder()
          ? _value.nextStep
          // ignore: cast_nullable_to_non_nullable
          : nextStep as AdminSubscriptionListItemNextStep,
    );
  }
}

extension $AdminSubscriptionListItemCopyWith on AdminSubscriptionListItem {
  /// Returns a callable class that can be used as follows: `instanceOfAdminSubscriptionListItem.copyWith(...)` or like so:`instanceOfAdminSubscriptionListItem.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminSubscriptionListItemCWProxy get copyWith =>
      _$AdminSubscriptionListItemCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminSubscriptionListItem _$AdminSubscriptionListItemFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('AdminSubscriptionListItem', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const [
      'brandId',
      'brandName',
      'planName',
      'status',
      'pastDueDays',
      'trialDaysLeft',
      'creditBalance',
      'creditsLow',
      'nextStep',
    ],
  );
  final val = AdminSubscriptionListItem(
    brandId: $checkedConvert('brandId', (v) => v as String),
    brandName: $checkedConvert('brandName', (v) => v as String),
    planName: $checkedConvert('planName', (v) => v as String),
    status: $checkedConvert(
      'status',
      (v) => $enumDecode(
        _$SubscriptionStatusEnumMap,
        v,
        unknownValue: SubscriptionStatus.unknownDefaultOpenApi,
      ),
    ),
    pastDueDays: $checkedConvert('pastDueDays', (v) => (v as num?)?.toInt()),
    trialDaysLeft: $checkedConvert(
      'trialDaysLeft',
      (v) => (v as num?)?.toInt(),
    ),
    creditBalance: $checkedConvert('creditBalance', (v) => (v as num).toInt()),
    creditsLow: $checkedConvert('creditsLow', (v) => v as bool),
    nextStep: $checkedConvert(
      'nextStep',
      (v) =>
          AdminSubscriptionListItemNextStep.fromJson(v as Map<String, dynamic>),
    ),
  );
  return val;
});

Map<String, dynamic> _$AdminSubscriptionListItemToJson(
  AdminSubscriptionListItem instance,
) => <String, dynamic>{
  'brandId': instance.brandId,
  'brandName': instance.brandName,
  'planName': instance.planName,
  'status': _$SubscriptionStatusEnumMap[instance.status]!,
  'pastDueDays': instance.pastDueDays,
  'trialDaysLeft': instance.trialDaysLeft,
  'creditBalance': instance.creditBalance,
  'creditsLow': instance.creditsLow,
  'nextStep': instance.nextStep.toJson(),
};

const _$SubscriptionStatusEnumMap = {
  SubscriptionStatus.trialing: 'Trialing',
  SubscriptionStatus.active: 'Active',
  SubscriptionStatus.pastDue: 'PastDue',
  SubscriptionStatus.readOnly: 'ReadOnly',
  SubscriptionStatus.cancelled: 'Cancelled',
  SubscriptionStatus.unknownDefaultOpenApi: 'unknown_default_open_api',
};
