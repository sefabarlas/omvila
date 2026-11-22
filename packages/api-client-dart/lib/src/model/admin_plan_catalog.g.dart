// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_plan_catalog.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminPlanCatalogCWProxy {
  AdminPlanCatalog plans(List<AdminPlan> plans);

  AdminPlanCatalog rules(AdminPlanCatalogRules rules);

  AdminPlanCatalog version(String version);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminPlanCatalog(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminPlanCatalog(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminPlanCatalog call({
    List<AdminPlan> plans,
    AdminPlanCatalogRules rules,
    String version,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminPlanCatalog.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminPlanCatalog.copyWith.fieldName(...)`
class _$AdminPlanCatalogCWProxyImpl implements _$AdminPlanCatalogCWProxy {
  const _$AdminPlanCatalogCWProxyImpl(this._value);

  final AdminPlanCatalog _value;

  @override
  AdminPlanCatalog plans(List<AdminPlan> plans) => this(plans: plans);

  @override
  AdminPlanCatalog rules(AdminPlanCatalogRules rules) => this(rules: rules);

  @override
  AdminPlanCatalog version(String version) => this(version: version);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminPlanCatalog(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminPlanCatalog(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminPlanCatalog call({
    Object? plans = const $CopyWithPlaceholder(),
    Object? rules = const $CopyWithPlaceholder(),
    Object? version = const $CopyWithPlaceholder(),
  }) {
    return AdminPlanCatalog(
      plans: plans == const $CopyWithPlaceholder()
          ? _value.plans
          // ignore: cast_nullable_to_non_nullable
          : plans as List<AdminPlan>,
      rules: rules == const $CopyWithPlaceholder()
          ? _value.rules
          // ignore: cast_nullable_to_non_nullable
          : rules as AdminPlanCatalogRules,
      version: version == const $CopyWithPlaceholder()
          ? _value.version
          // ignore: cast_nullable_to_non_nullable
          : version as String,
    );
  }
}

extension $AdminPlanCatalogCopyWith on AdminPlanCatalog {
  /// Returns a callable class that can be used as follows: `instanceOfAdminPlanCatalog.copyWith(...)` or like so:`instanceOfAdminPlanCatalog.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminPlanCatalogCWProxy get copyWith => _$AdminPlanCatalogCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminPlanCatalog _$AdminPlanCatalogFromJson(Map<String, dynamic> json) =>
    $checkedCreate('AdminPlanCatalog', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['plans', 'rules', 'version']);
      final val = AdminPlanCatalog(
        plans: $checkedConvert(
          'plans',
          (v) => (v as List<dynamic>)
              .map((e) => AdminPlan.fromJson(e as Map<String, dynamic>))
              .toList(),
        ),
        rules: $checkedConvert(
          'rules',
          (v) => AdminPlanCatalogRules.fromJson(v as Map<String, dynamic>),
        ),
        version: $checkedConvert('version', (v) => v as String),
      );
      return val;
    });

Map<String, dynamic> _$AdminPlanCatalogToJson(AdminPlanCatalog instance) =>
    <String, dynamic>{
      'plans': instance.plans.map((e) => e.toJson()).toList(),
      'rules': instance.rules.toJson(),
      'version': instance.version,
    };
