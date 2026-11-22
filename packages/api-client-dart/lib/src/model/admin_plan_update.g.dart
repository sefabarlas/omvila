// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_plan_update.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminPlanUpdateCWProxy {
  AdminPlanUpdate code(String code);

  AdminPlanUpdate name(String name);

  AdminPlanUpdate description(String? description);

  AdminPlanUpdate monthlyPrice(Money? monthlyPrice);

  AdminPlanUpdate yearlyPrice(Money? yearlyPrice);

  AdminPlanUpdate includedBranchCount(int? includedBranchCount);

  AdminPlanUpdate extraBranchPrice(Money? extraBranchPrice);

  AdminPlanUpdate staffLimit(int? staffLimit);

  AdminPlanUpdate includedSmsCredits(int? includedSmsCredits);

  AdminPlanUpdate features(PlanFeatures features);

  AdminPlanUpdate isPublic(bool isPublic);

  AdminPlanUpdate isActive(bool isActive);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminPlanUpdate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminPlanUpdate(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminPlanUpdate call({
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
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminPlanUpdate.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminPlanUpdate.copyWith.fieldName(...)`
class _$AdminPlanUpdateCWProxyImpl implements _$AdminPlanUpdateCWProxy {
  const _$AdminPlanUpdateCWProxyImpl(this._value);

  final AdminPlanUpdate _value;

  @override
  AdminPlanUpdate code(String code) => this(code: code);

  @override
  AdminPlanUpdate name(String name) => this(name: name);

  @override
  AdminPlanUpdate description(String? description) =>
      this(description: description);

  @override
  AdminPlanUpdate monthlyPrice(Money? monthlyPrice) =>
      this(monthlyPrice: monthlyPrice);

  @override
  AdminPlanUpdate yearlyPrice(Money? yearlyPrice) =>
      this(yearlyPrice: yearlyPrice);

  @override
  AdminPlanUpdate includedBranchCount(int? includedBranchCount) =>
      this(includedBranchCount: includedBranchCount);

  @override
  AdminPlanUpdate extraBranchPrice(Money? extraBranchPrice) =>
      this(extraBranchPrice: extraBranchPrice);

  @override
  AdminPlanUpdate staffLimit(int? staffLimit) => this(staffLimit: staffLimit);

  @override
  AdminPlanUpdate includedSmsCredits(int? includedSmsCredits) =>
      this(includedSmsCredits: includedSmsCredits);

  @override
  AdminPlanUpdate features(PlanFeatures features) => this(features: features);

  @override
  AdminPlanUpdate isPublic(bool isPublic) => this(isPublic: isPublic);

  @override
  AdminPlanUpdate isActive(bool isActive) => this(isActive: isActive);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminPlanUpdate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminPlanUpdate(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminPlanUpdate call({
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
  }) {
    return AdminPlanUpdate(
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
    );
  }
}

extension $AdminPlanUpdateCopyWith on AdminPlanUpdate {
  /// Returns a callable class that can be used as follows: `instanceOfAdminPlanUpdate.copyWith(...)` or like so:`instanceOfAdminPlanUpdate.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminPlanUpdateCWProxy get copyWith => _$AdminPlanUpdateCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminPlanUpdate _$AdminPlanUpdateFromJson(Map<String, dynamic> json) =>
    $checkedCreate('AdminPlanUpdate', json, ($checkedConvert) {
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
        ],
      );
      final val = AdminPlanUpdate(
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
      );
      return val;
    });

Map<String, dynamic> _$AdminPlanUpdateToJson(AdminPlanUpdate instance) =>
    <String, dynamic>{
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
    };
