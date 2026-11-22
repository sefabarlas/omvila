// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_plan.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminPlanCWProxy {
  AdminPlan code(String code);

  AdminPlan name(String name);

  AdminPlan description(String? description);

  AdminPlan monthlyPrice(Money? monthlyPrice);

  AdminPlan yearlyPrice(Money? yearlyPrice);

  AdminPlan includedBranchCount(int? includedBranchCount);

  AdminPlan extraBranchPrice(Money? extraBranchPrice);

  AdminPlan staffLimit(int? staffLimit);

  AdminPlan includedSmsCredits(int? includedSmsCredits);

  AdminPlan features(PlanFeatures features);

  AdminPlan isPublic(bool isPublic);

  AdminPlan isActive(bool isActive);

  AdminPlan brandCount(int brandCount);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminPlan(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminPlan(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminPlan call({
    String code,
    String name,
    String? description,
    Money? monthlyPrice,
    Money? yearlyPrice,
    int? includedBranchCount,
    Money? extraBranchPrice,
    int? staffLimit,
    int? includedSmsCredits,
    PlanFeatures features,
    bool isPublic,
    bool isActive,
    int brandCount,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminPlan.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminPlan.copyWith.fieldName(...)`
class _$AdminPlanCWProxyImpl implements _$AdminPlanCWProxy {
  const _$AdminPlanCWProxyImpl(this._value);

  final AdminPlan _value;

  @override
  AdminPlan code(String code) => this(code: code);

  @override
  AdminPlan name(String name) => this(name: name);

  @override
  AdminPlan description(String? description) => this(description: description);

  @override
  AdminPlan monthlyPrice(Money? monthlyPrice) =>
      this(monthlyPrice: monthlyPrice);

  @override
  AdminPlan yearlyPrice(Money? yearlyPrice) => this(yearlyPrice: yearlyPrice);

  @override
  AdminPlan includedBranchCount(int? includedBranchCount) =>
      this(includedBranchCount: includedBranchCount);

  @override
  AdminPlan extraBranchPrice(Money? extraBranchPrice) =>
      this(extraBranchPrice: extraBranchPrice);

  @override
  AdminPlan staffLimit(int? staffLimit) => this(staffLimit: staffLimit);

  @override
  AdminPlan includedSmsCredits(int? includedSmsCredits) =>
      this(includedSmsCredits: includedSmsCredits);

  @override
  AdminPlan features(PlanFeatures features) => this(features: features);

  @override
  AdminPlan isPublic(bool isPublic) => this(isPublic: isPublic);

  @override
  AdminPlan isActive(bool isActive) => this(isActive: isActive);

  @override
  AdminPlan brandCount(int brandCount) => this(brandCount: brandCount);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminPlan(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminPlan(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminPlan call({
    Object? code = const $CopyWithPlaceholder(),
    Object? name = const $CopyWithPlaceholder(),
    Object? description = const $CopyWithPlaceholder(),
    Object? monthlyPrice = const $CopyWithPlaceholder(),
    Object? yearlyPrice = const $CopyWithPlaceholder(),
    Object? includedBranchCount = const $CopyWithPlaceholder(),
    Object? extraBranchPrice = const $CopyWithPlaceholder(),
    Object? staffLimit = const $CopyWithPlaceholder(),
    Object? includedSmsCredits = const $CopyWithPlaceholder(),
    Object? features = const $CopyWithPlaceholder(),
    Object? isPublic = const $CopyWithPlaceholder(),
    Object? isActive = const $CopyWithPlaceholder(),
    Object? brandCount = const $CopyWithPlaceholder(),
  }) {
    return AdminPlan(
      code: code == const $CopyWithPlaceholder()
          ? _value.code
          // ignore: cast_nullable_to_non_nullable
          : code as String,
      name: name == const $CopyWithPlaceholder()
          ? _value.name
          // ignore: cast_nullable_to_non_nullable
          : name as String,
      description: description == const $CopyWithPlaceholder()
          ? _value.description
          // ignore: cast_nullable_to_non_nullable
          : description as String?,
      monthlyPrice: monthlyPrice == const $CopyWithPlaceholder()
          ? _value.monthlyPrice
          // ignore: cast_nullable_to_non_nullable
          : monthlyPrice as Money?,
      yearlyPrice: yearlyPrice == const $CopyWithPlaceholder()
          ? _value.yearlyPrice
          // ignore: cast_nullable_to_non_nullable
          : yearlyPrice as Money?,
      includedBranchCount: includedBranchCount == const $CopyWithPlaceholder()
          ? _value.includedBranchCount
          // ignore: cast_nullable_to_non_nullable
          : includedBranchCount as int?,
      extraBranchPrice: extraBranchPrice == const $CopyWithPlaceholder()
          ? _value.extraBranchPrice
          // ignore: cast_nullable_to_non_nullable
          : extraBranchPrice as Money?,
      staffLimit: staffLimit == const $CopyWithPlaceholder()
          ? _value.staffLimit
          // ignore: cast_nullable_to_non_nullable
          : staffLimit as int?,
      includedSmsCredits: includedSmsCredits == const $CopyWithPlaceholder()
          ? _value.includedSmsCredits
          // ignore: cast_nullable_to_non_nullable
          : includedSmsCredits as int?,
      features: features == const $CopyWithPlaceholder()
          ? _value.features
          // ignore: cast_nullable_to_non_nullable
          : features as PlanFeatures,
      isPublic: isPublic == const $CopyWithPlaceholder()
          ? _value.isPublic
          // ignore: cast_nullable_to_non_nullable
          : isPublic as bool,
      isActive: isActive == const $CopyWithPlaceholder()
          ? _value.isActive
          // ignore: cast_nullable_to_non_nullable
          : isActive as bool,
      brandCount: brandCount == const $CopyWithPlaceholder()
          ? _value.brandCount
          // ignore: cast_nullable_to_non_nullable
          : brandCount as int,
    );
  }
}

extension $AdminPlanCopyWith on AdminPlan {
  /// Returns a callable class that can be used as follows: `instanceOfAdminPlan.copyWith(...)` or like so:`instanceOfAdminPlan.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminPlanCWProxy get copyWith => _$AdminPlanCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminPlan _$AdminPlanFromJson(Map<String, dynamic> json) =>
    $checkedCreate('AdminPlan', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'code',
          'name',
          'description',
          'monthlyPrice',
          'yearlyPrice',
          'includedBranchCount',
          'extraBranchPrice',
          'staffLimit',
          'includedSmsCredits',
          'features',
          'isPublic',
          'isActive',
          'brandCount',
        ],
      );
      final val = AdminPlan(
        code: $checkedConvert('code', (v) => v as String),
        name: $checkedConvert('name', (v) => v as String),
        description: $checkedConvert('description', (v) => v as String?),
        monthlyPrice: $checkedConvert(
          'monthlyPrice',
          (v) => v == null ? null : Money.fromJson(v as Map<String, dynamic>),
        ),
        yearlyPrice: $checkedConvert(
          'yearlyPrice',
          (v) => v == null ? null : Money.fromJson(v as Map<String, dynamic>),
        ),
        includedBranchCount: $checkedConvert(
          'includedBranchCount',
          (v) => (v as num?)?.toInt(),
        ),
        extraBranchPrice: $checkedConvert(
          'extraBranchPrice',
          (v) => v == null ? null : Money.fromJson(v as Map<String, dynamic>),
        ),
        staffLimit: $checkedConvert('staffLimit', (v) => (v as num?)?.toInt()),
        includedSmsCredits: $checkedConvert(
          'includedSmsCredits',
          (v) => (v as num?)?.toInt(),
        ),
        features: $checkedConvert(
          'features',
          (v) => PlanFeatures.fromJson(v as Map<String, dynamic>),
        ),
        isPublic: $checkedConvert('isPublic', (v) => v as bool),
        isActive: $checkedConvert('isActive', (v) => v as bool),
        brandCount: $checkedConvert('brandCount', (v) => (v as num).toInt()),
      );
      return val;
    });

Map<String, dynamic> _$AdminPlanToJson(AdminPlan instance) => <String, dynamic>{
  'code': instance.code,
  'name': instance.name,
  'description': instance.description,
  'monthlyPrice': instance.monthlyPrice?.toJson(),
  'yearlyPrice': instance.yearlyPrice?.toJson(),
  'includedBranchCount': instance.includedBranchCount,
  'extraBranchPrice': instance.extraBranchPrice?.toJson(),
  'staffLimit': instance.staffLimit,
  'includedSmsCredits': instance.includedSmsCredits,
  'features': instance.features.toJson(),
  'isPublic': instance.isPublic,
  'isActive': instance.isActive,
  'brandCount': instance.brandCount,
};
