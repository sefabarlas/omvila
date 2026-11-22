// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_plan_change.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminPlanChangeCWProxy {
  AdminPlanChange planCode(String planCode);

  AdminPlanChange billingCycle(BillingCycle? billingCycle);

  AdminPlanChange reason(String reason);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminPlanChange(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminPlanChange(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminPlanChange call({
    String planCode,
    BillingCycle? billingCycle,
    String reason,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminPlanChange.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminPlanChange.copyWith.fieldName(...)`
class _$AdminPlanChangeCWProxyImpl implements _$AdminPlanChangeCWProxy {
  const _$AdminPlanChangeCWProxyImpl(this._value);

  final AdminPlanChange _value;

  @override
  AdminPlanChange planCode(String planCode) => this(planCode: planCode);

  @override
  AdminPlanChange billingCycle(BillingCycle? billingCycle) =>
      this(billingCycle: billingCycle);

  @override
  AdminPlanChange reason(String reason) => this(reason: reason);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminPlanChange(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminPlanChange(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminPlanChange call({
    Object? planCode = const $CopyWithPlaceholder(),
    Object? billingCycle = const $CopyWithPlaceholder(),
    Object? reason = const $CopyWithPlaceholder(),
  }) {
    return AdminPlanChange(
      planCode: planCode == const $CopyWithPlaceholder()
          ? _value.planCode
          // ignore: cast_nullable_to_non_nullable
          : planCode as String,
      billingCycle: billingCycle == const $CopyWithPlaceholder()
          ? _value.billingCycle
          // ignore: cast_nullable_to_non_nullable
          : billingCycle as BillingCycle?,
      reason: reason == const $CopyWithPlaceholder()
          ? _value.reason
          // ignore: cast_nullable_to_non_nullable
          : reason as String,
    );
  }
}

extension $AdminPlanChangeCopyWith on AdminPlanChange {
  /// Returns a callable class that can be used as follows: `instanceOfAdminPlanChange.copyWith(...)` or like so:`instanceOfAdminPlanChange.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminPlanChangeCWProxy get copyWith => _$AdminPlanChangeCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminPlanChange _$AdminPlanChangeFromJson(Map<String, dynamic> json) =>
    $checkedCreate('AdminPlanChange', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['planCode', 'reason']);
      final val = AdminPlanChange(
        planCode: $checkedConvert('planCode', (v) => v as String),
        billingCycle: $checkedConvert(
          'billingCycle',
          (v) => $enumDecodeNullable(
            _$BillingCycleEnumMap,
            v,
            unknownValue: BillingCycle.unknownDefaultOpenApi,
          ),
        ),
        reason: $checkedConvert('reason', (v) => v as String),
      );
      return val;
    });

Map<String, dynamic> _$AdminPlanChangeToJson(AdminPlanChange instance) =>
    <String, dynamic>{
      'planCode': instance.planCode,
      'billingCycle': ?_$BillingCycleEnumMap[instance.billingCycle],
      'reason': instance.reason,
    };

const _$BillingCycleEnumMap = {
  BillingCycle.monthly: 'Monthly',
  BillingCycle.yearly: 'Yearly',
  BillingCycle.unknownDefaultOpenApi: 'unknown_default_open_api',
};
