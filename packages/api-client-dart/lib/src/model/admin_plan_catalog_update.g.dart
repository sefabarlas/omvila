// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_plan_catalog_update.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminPlanCatalogUpdateCWProxy {
  AdminPlanCatalogUpdate plans(List<AdminPlanUpdate> plans);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminPlanCatalogUpdate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminPlanCatalogUpdate(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminPlanCatalogUpdate call({List<AdminPlanUpdate> plans});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminPlanCatalogUpdate.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminPlanCatalogUpdate.copyWith.fieldName(...)`
class _$AdminPlanCatalogUpdateCWProxyImpl
    implements _$AdminPlanCatalogUpdateCWProxy {
  const _$AdminPlanCatalogUpdateCWProxyImpl(this._value);

  final AdminPlanCatalogUpdate _value;

  @override
  AdminPlanCatalogUpdate plans(List<AdminPlanUpdate> plans) =>
      this(plans: plans);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminPlanCatalogUpdate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminPlanCatalogUpdate(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminPlanCatalogUpdate call({Object? plans = const $CopyWithPlaceholder()}) {
    return AdminPlanCatalogUpdate(
      plans: plans == const $CopyWithPlaceholder()
          ? _value.plans
          // ignore: cast_nullable_to_non_nullable
          : plans as List<AdminPlanUpdate>,
    );
  }
}

extension $AdminPlanCatalogUpdateCopyWith on AdminPlanCatalogUpdate {
  /// Returns a callable class that can be used as follows: `instanceOfAdminPlanCatalogUpdate.copyWith(...)` or like so:`instanceOfAdminPlanCatalogUpdate.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminPlanCatalogUpdateCWProxy get copyWith =>
      _$AdminPlanCatalogUpdateCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminPlanCatalogUpdate _$AdminPlanCatalogUpdateFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('AdminPlanCatalogUpdate', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['plans']);
  final val = AdminPlanCatalogUpdate(
    plans: $checkedConvert(
      'plans',
      (v) => (v as List<dynamic>)
          .map((e) => AdminPlanUpdate.fromJson(e as Map<String, dynamic>))
          .toList(),
    ),
  );
  return val;
});

Map<String, dynamic> _$AdminPlanCatalogUpdateToJson(
  AdminPlanCatalogUpdate instance,
) => <String, dynamic>{'plans': instance.plans.map((e) => e.toJson()).toList()};
