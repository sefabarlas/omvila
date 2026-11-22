// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_plan_catalog_rules.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminPlanCatalogRulesCWProxy {
  AdminPlanCatalogRules trialDays(int trialDays);

  AdminPlanCatalogRules pastDueGraceDays(int pastDueGraceDays);

  AdminPlanCatalogRules trialCredits(int? trialCredits);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminPlanCatalogRules(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminPlanCatalogRules(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminPlanCatalogRules call({
    int trialDays,
    int pastDueGraceDays,
    int? trialCredits,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminPlanCatalogRules.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminPlanCatalogRules.copyWith.fieldName(...)`
class _$AdminPlanCatalogRulesCWProxyImpl
    implements _$AdminPlanCatalogRulesCWProxy {
  const _$AdminPlanCatalogRulesCWProxyImpl(this._value);

  final AdminPlanCatalogRules _value;

  @override
  AdminPlanCatalogRules trialDays(int trialDays) => this(trialDays: trialDays);

  @override
  AdminPlanCatalogRules pastDueGraceDays(int pastDueGraceDays) =>
      this(pastDueGraceDays: pastDueGraceDays);

  @override
  AdminPlanCatalogRules trialCredits(int? trialCredits) =>
      this(trialCredits: trialCredits);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminPlanCatalogRules(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminPlanCatalogRules(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminPlanCatalogRules call({
    Object? trialDays = const $CopyWithPlaceholder(),
    Object? pastDueGraceDays = const $CopyWithPlaceholder(),
    Object? trialCredits = const $CopyWithPlaceholder(),
  }) {
    return AdminPlanCatalogRules(
      trialDays: trialDays == const $CopyWithPlaceholder()
          ? _value.trialDays
          // ignore: cast_nullable_to_non_nullable
          : trialDays as int,
      pastDueGraceDays: pastDueGraceDays == const $CopyWithPlaceholder()
          ? _value.pastDueGraceDays
          // ignore: cast_nullable_to_non_nullable
          : pastDueGraceDays as int,
      trialCredits: trialCredits == const $CopyWithPlaceholder()
          ? _value.trialCredits
          // ignore: cast_nullable_to_non_nullable
          : trialCredits as int?,
    );
  }
}

extension $AdminPlanCatalogRulesCopyWith on AdminPlanCatalogRules {
  /// Returns a callable class that can be used as follows: `instanceOfAdminPlanCatalogRules.copyWith(...)` or like so:`instanceOfAdminPlanCatalogRules.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminPlanCatalogRulesCWProxy get copyWith =>
      _$AdminPlanCatalogRulesCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminPlanCatalogRules _$AdminPlanCatalogRulesFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('AdminPlanCatalogRules', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const ['trialDays', 'pastDueGraceDays', 'trialCredits'],
  );
  final val = AdminPlanCatalogRules(
    trialDays: $checkedConvert('trialDays', (v) => (v as num).toInt()),
    pastDueGraceDays: $checkedConvert(
      'pastDueGraceDays',
      (v) => (v as num).toInt(),
    ),
    trialCredits: $checkedConvert('trialCredits', (v) => (v as num?)?.toInt()),
  );
  return val;
});

Map<String, dynamic> _$AdminPlanCatalogRulesToJson(
  AdminPlanCatalogRules instance,
) => <String, dynamic>{
  'trialDays': instance.trialDays,
  'pastDueGraceDays': instance.pastDueGraceDays,
  'trialCredits': instance.trialCredits,
};
