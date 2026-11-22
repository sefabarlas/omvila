// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_attention_brand.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminAttentionBrandCWProxy {
  AdminAttentionBrand brandId(String brandId);

  AdminAttentionBrand brandName(String brandName);

  AdminAttentionBrand industryCode(IndustryCode industryCode);

  AdminAttentionBrand city(String? city);

  AdminAttentionBrand planName(String planName);

  AdminAttentionBrand reason(AdminAttentionBrandReasonEnum reason);

  AdminAttentionBrand pastDueDays(int? pastDueDays);

  AdminAttentionBrand trialDaysLeft(int? trialDaysLeft);

  AdminAttentionBrand branchCount(int branchCount);

  AdminAttentionBrand appointmentsThisMonth(int appointmentsThisMonth);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminAttentionBrand(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminAttentionBrand(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminAttentionBrand call({
    String brandId,
    String brandName,
    IndustryCode industryCode,
    String? city,
    String planName,
    AdminAttentionBrandReasonEnum reason,
    int? pastDueDays,
    int? trialDaysLeft,
    int branchCount,
    int appointmentsThisMonth,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminAttentionBrand.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminAttentionBrand.copyWith.fieldName(...)`
class _$AdminAttentionBrandCWProxyImpl implements _$AdminAttentionBrandCWProxy {
  const _$AdminAttentionBrandCWProxyImpl(this._value);

  final AdminAttentionBrand _value;

  @override
  AdminAttentionBrand brandId(String brandId) => this(brandId: brandId);

  @override
  AdminAttentionBrand brandName(String brandName) => this(brandName: brandName);

  @override
  AdminAttentionBrand industryCode(IndustryCode industryCode) =>
      this(industryCode: industryCode);

  @override
  AdminAttentionBrand city(String? city) => this(city: city);

  @override
  AdminAttentionBrand planName(String planName) => this(planName: planName);

  @override
  AdminAttentionBrand reason(AdminAttentionBrandReasonEnum reason) =>
      this(reason: reason);

  @override
  AdminAttentionBrand pastDueDays(int? pastDueDays) =>
      this(pastDueDays: pastDueDays);

  @override
  AdminAttentionBrand trialDaysLeft(int? trialDaysLeft) =>
      this(trialDaysLeft: trialDaysLeft);

  @override
  AdminAttentionBrand branchCount(int branchCount) =>
      this(branchCount: branchCount);

  @override
  AdminAttentionBrand appointmentsThisMonth(int appointmentsThisMonth) =>
      this(appointmentsThisMonth: appointmentsThisMonth);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminAttentionBrand(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminAttentionBrand(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminAttentionBrand call({
    Object? brandId = const $CopyWithPlaceholder(),
    Object? brandName = const $CopyWithPlaceholder(),
    Object? industryCode = const $CopyWithPlaceholder(),
    Object? city = const $CopyWithPlaceholder(),
    Object? planName = const $CopyWithPlaceholder(),
    Object? reason = const $CopyWithPlaceholder(),
    Object? pastDueDays = const $CopyWithPlaceholder(),
    Object? trialDaysLeft = const $CopyWithPlaceholder(),
    Object? branchCount = const $CopyWithPlaceholder(),
    Object? appointmentsThisMonth = const $CopyWithPlaceholder(),
  }) {
    return AdminAttentionBrand(
      brandId: brandId == const $CopyWithPlaceholder()
          ? _value.brandId
          // ignore: cast_nullable_to_non_nullable
          : brandId as String,
      brandName: brandName == const $CopyWithPlaceholder()
          ? _value.brandName
          // ignore: cast_nullable_to_non_nullable
          : brandName as String,
      industryCode: industryCode == const $CopyWithPlaceholder()
          ? _value.industryCode
          // ignore: cast_nullable_to_non_nullable
          : industryCode as IndustryCode,
      city: city == const $CopyWithPlaceholder()
          ? _value.city
          // ignore: cast_nullable_to_non_nullable
          : city as String?,
      planName: planName == const $CopyWithPlaceholder()
          ? _value.planName
          // ignore: cast_nullable_to_non_nullable
          : planName as String,
      reason: reason == const $CopyWithPlaceholder()
          ? _value.reason
          // ignore: cast_nullable_to_non_nullable
          : reason as AdminAttentionBrandReasonEnum,
      pastDueDays: pastDueDays == const $CopyWithPlaceholder()
          ? _value.pastDueDays
          // ignore: cast_nullable_to_non_nullable
          : pastDueDays as int?,
      trialDaysLeft: trialDaysLeft == const $CopyWithPlaceholder()
          ? _value.trialDaysLeft
          // ignore: cast_nullable_to_non_nullable
          : trialDaysLeft as int?,
      branchCount: branchCount == const $CopyWithPlaceholder()
          ? _value.branchCount
          // ignore: cast_nullable_to_non_nullable
          : branchCount as int,
      appointmentsThisMonth:
          appointmentsThisMonth == const $CopyWithPlaceholder()
          ? _value.appointmentsThisMonth
          // ignore: cast_nullable_to_non_nullable
          : appointmentsThisMonth as int,
    );
  }
}

extension $AdminAttentionBrandCopyWith on AdminAttentionBrand {
  /// Returns a callable class that can be used as follows: `instanceOfAdminAttentionBrand.copyWith(...)` or like so:`instanceOfAdminAttentionBrand.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminAttentionBrandCWProxy get copyWith =>
      _$AdminAttentionBrandCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminAttentionBrand _$AdminAttentionBrandFromJson(Map<String, dynamic> json) =>
    $checkedCreate('AdminAttentionBrand', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'brandId',
          'brandName',
          'industryCode',
          'city',
          'planName',
          'reason',
          'pastDueDays',
          'trialDaysLeft',
          'branchCount',
          'appointmentsThisMonth',
        ],
      );
      final val = AdminAttentionBrand(
        brandId: $checkedConvert('brandId', (v) => v as String),
        brandName: $checkedConvert('brandName', (v) => v as String),
        industryCode: $checkedConvert(
          'industryCode',
          (v) => $enumDecode(
            _$IndustryCodeEnumMap,
            v,
            unknownValue: IndustryCode.unknownDefaultOpenApi,
          ),
        ),
        city: $checkedConvert('city', (v) => v as String?),
        planName: $checkedConvert('planName', (v) => v as String),
        reason: $checkedConvert(
          'reason',
          (v) => $enumDecode(
            _$AdminAttentionBrandReasonEnumEnumMap,
            v,
            unknownValue: AdminAttentionBrandReasonEnum.unknownDefaultOpenApi,
          ),
        ),
        pastDueDays: $checkedConvert(
          'pastDueDays',
          (v) => (v as num?)?.toInt(),
        ),
        trialDaysLeft: $checkedConvert(
          'trialDaysLeft',
          (v) => (v as num?)?.toInt(),
        ),
        branchCount: $checkedConvert('branchCount', (v) => (v as num).toInt()),
        appointmentsThisMonth: $checkedConvert(
          'appointmentsThisMonth',
          (v) => (v as num).toInt(),
        ),
      );
      return val;
    });

Map<String, dynamic> _$AdminAttentionBrandToJson(
  AdminAttentionBrand instance,
) => <String, dynamic>{
  'brandId': instance.brandId,
  'brandName': instance.brandName,
  'industryCode': _$IndustryCodeEnumMap[instance.industryCode]!,
  'city': instance.city,
  'planName': instance.planName,
  'reason': _$AdminAttentionBrandReasonEnumEnumMap[instance.reason]!,
  'pastDueDays': instance.pastDueDays,
  'trialDaysLeft': instance.trialDaysLeft,
  'branchCount': instance.branchCount,
  'appointmentsThisMonth': instance.appointmentsThisMonth,
};

const _$IndustryCodeEnumMap = {
  IndustryCode.barber: 'barber',
  IndustryCode.hairSalon: 'hair_salon',
  IndustryCode.beauty: 'beauty',
  IndustryCode.spa: 'spa',
  IndustryCode.nail: 'nail',
  IndustryCode.dietitian: 'dietitian',
  IndustryCode.other: 'other',
  IndustryCode.unknownDefaultOpenApi: 'unknown_default_open_api',
};

const _$AdminAttentionBrandReasonEnumEnumMap = {
  AdminAttentionBrandReasonEnum.pastDue: 'PastDue',
  AdminAttentionBrandReasonEnum.creditsDepleted: 'CreditsDepleted',
  AdminAttentionBrandReasonEnum.trialEnding: 'TrialEnding',
  AdminAttentionBrandReasonEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};
