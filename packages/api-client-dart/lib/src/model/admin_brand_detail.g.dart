// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_brand_detail.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminBrandDetailCWProxy {
  AdminBrandDetail id(String id);

  AdminBrandDetail name(String name);

  AdminBrandDetail slug(String slug);

  AdminBrandDetail industryCode(IndustryCode industryCode);

  AdminBrandDetail city(String? city);

  AdminBrandDetail brandStatus(AdminBrandDetailBrandStatusEnum brandStatus);

  AdminBrandDetail createdUtc(DateTime createdUtc);

  AdminBrandDetail bookingUrl(String bookingUrl);

  AdminBrandDetail owner(AdminBrandDetailOwner owner);

  AdminBrandDetail branches(List<BranchRef> branches);

  AdminBrandDetail activeStaffCount(int activeStaffCount);

  AdminBrandDetail customerCount(int customerCount);

  AdminBrandDetail googleCalendarConnectedStaffCount(
    int googleCalendarConnectedStaffCount,
  );

  AdminBrandDetail smsSender(SmsSender smsSender);

  AdminBrandDetail subscription(AdminBrandDetailSubscription subscription);

  AdminBrandDetail credits(AdminBrandDetailCredits credits);

  AdminBrandDetail appointmentsThisMonth(
    AdminBrandDetailAppointmentsThisMonth appointmentsThisMonth,
  );

  AdminBrandDetail recentCreditTransactions(
    List<AdminCreditTransaction> recentCreditTransactions,
  );

  AdminBrandDetail version(String version);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminBrandDetail(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminBrandDetail(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminBrandDetail call({
    String id,
    String name,
    String slug,
    IndustryCode industryCode,
    String? city,
    AdminBrandDetailBrandStatusEnum brandStatus,
    DateTime createdUtc,
    String bookingUrl,
    AdminBrandDetailOwner owner,
    List<BranchRef> branches,
    int activeStaffCount,
    int customerCount,
    int googleCalendarConnectedStaffCount,
    SmsSender smsSender,
    AdminBrandDetailSubscription subscription,
    AdminBrandDetailCredits credits,
    AdminBrandDetailAppointmentsThisMonth appointmentsThisMonth,
    List<AdminCreditTransaction> recentCreditTransactions,
    String version,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminBrandDetail.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminBrandDetail.copyWith.fieldName(...)`
class _$AdminBrandDetailCWProxyImpl implements _$AdminBrandDetailCWProxy {
  const _$AdminBrandDetailCWProxyImpl(this._value);

  final AdminBrandDetail _value;

  @override
  AdminBrandDetail id(String id) => this(id: id);

  @override
  AdminBrandDetail name(String name) => this(name: name);

  @override
  AdminBrandDetail slug(String slug) => this(slug: slug);

  @override
  AdminBrandDetail industryCode(IndustryCode industryCode) =>
      this(industryCode: industryCode);

  @override
  AdminBrandDetail city(String? city) => this(city: city);

  @override
  AdminBrandDetail brandStatus(AdminBrandDetailBrandStatusEnum brandStatus) =>
      this(brandStatus: brandStatus);

  @override
  AdminBrandDetail createdUtc(DateTime createdUtc) =>
      this(createdUtc: createdUtc);

  @override
  AdminBrandDetail bookingUrl(String bookingUrl) =>
      this(bookingUrl: bookingUrl);

  @override
  AdminBrandDetail owner(AdminBrandDetailOwner owner) => this(owner: owner);

  @override
  AdminBrandDetail branches(List<BranchRef> branches) =>
      this(branches: branches);

  @override
  AdminBrandDetail activeStaffCount(int activeStaffCount) =>
      this(activeStaffCount: activeStaffCount);

  @override
  AdminBrandDetail customerCount(int customerCount) =>
      this(customerCount: customerCount);

  @override
  AdminBrandDetail googleCalendarConnectedStaffCount(
    int googleCalendarConnectedStaffCount,
  ) => this(
    googleCalendarConnectedStaffCount: googleCalendarConnectedStaffCount,
  );

  @override
  AdminBrandDetail smsSender(SmsSender smsSender) => this(smsSender: smsSender);

  @override
  AdminBrandDetail subscription(AdminBrandDetailSubscription subscription) =>
      this(subscription: subscription);

  @override
  AdminBrandDetail credits(AdminBrandDetailCredits credits) =>
      this(credits: credits);

  @override
  AdminBrandDetail appointmentsThisMonth(
    AdminBrandDetailAppointmentsThisMonth appointmentsThisMonth,
  ) => this(appointmentsThisMonth: appointmentsThisMonth);

  @override
  AdminBrandDetail recentCreditTransactions(
    List<AdminCreditTransaction> recentCreditTransactions,
  ) => this(recentCreditTransactions: recentCreditTransactions);

  @override
  AdminBrandDetail version(String version) => this(version: version);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminBrandDetail(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminBrandDetail(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminBrandDetail call({
    Object? id = const $CopyWithPlaceholder(),
    Object? name = const $CopyWithPlaceholder(),
    Object? slug = const $CopyWithPlaceholder(),
    Object? industryCode = const $CopyWithPlaceholder(),
    Object? city = const $CopyWithPlaceholder(),
    Object? brandStatus = const $CopyWithPlaceholder(),
    Object? createdUtc = const $CopyWithPlaceholder(),
    Object? bookingUrl = const $CopyWithPlaceholder(),
    Object? owner = const $CopyWithPlaceholder(),
    Object? branches = const $CopyWithPlaceholder(),
    Object? activeStaffCount = const $CopyWithPlaceholder(),
    Object? customerCount = const $CopyWithPlaceholder(),
    Object? googleCalendarConnectedStaffCount = const $CopyWithPlaceholder(),
    Object? smsSender = const $CopyWithPlaceholder(),
    Object? subscription = const $CopyWithPlaceholder(),
    Object? credits = const $CopyWithPlaceholder(),
    Object? appointmentsThisMonth = const $CopyWithPlaceholder(),
    Object? recentCreditTransactions = const $CopyWithPlaceholder(),
    Object? version = const $CopyWithPlaceholder(),
  }) {
    return AdminBrandDetail(
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as String,
      name: name == const $CopyWithPlaceholder()
          ? _value.name
          // ignore: cast_nullable_to_non_nullable
          : name as String,
      slug: slug == const $CopyWithPlaceholder()
          ? _value.slug
          // ignore: cast_nullable_to_non_nullable
          : slug as String,
      industryCode: industryCode == const $CopyWithPlaceholder()
          ? _value.industryCode
          // ignore: cast_nullable_to_non_nullable
          : industryCode as IndustryCode,
      city: city == const $CopyWithPlaceholder()
          ? _value.city
          // ignore: cast_nullable_to_non_nullable
          : city as String?,
      brandStatus: brandStatus == const $CopyWithPlaceholder()
          ? _value.brandStatus
          // ignore: cast_nullable_to_non_nullable
          : brandStatus as AdminBrandDetailBrandStatusEnum,
      createdUtc: createdUtc == const $CopyWithPlaceholder()
          ? _value.createdUtc
          // ignore: cast_nullable_to_non_nullable
          : createdUtc as DateTime,
      bookingUrl: bookingUrl == const $CopyWithPlaceholder()
          ? _value.bookingUrl
          // ignore: cast_nullable_to_non_nullable
          : bookingUrl as String,
      owner: owner == const $CopyWithPlaceholder()
          ? _value.owner
          // ignore: cast_nullable_to_non_nullable
          : owner as AdminBrandDetailOwner,
      branches: branches == const $CopyWithPlaceholder()
          ? _value.branches
          // ignore: cast_nullable_to_non_nullable
          : branches as List<BranchRef>,
      activeStaffCount: activeStaffCount == const $CopyWithPlaceholder()
          ? _value.activeStaffCount
          // ignore: cast_nullable_to_non_nullable
          : activeStaffCount as int,
      customerCount: customerCount == const $CopyWithPlaceholder()
          ? _value.customerCount
          // ignore: cast_nullable_to_non_nullable
          : customerCount as int,
      googleCalendarConnectedStaffCount:
          googleCalendarConnectedStaffCount == const $CopyWithPlaceholder()
          ? _value.googleCalendarConnectedStaffCount
          // ignore: cast_nullable_to_non_nullable
          : googleCalendarConnectedStaffCount as int,
      smsSender: smsSender == const $CopyWithPlaceholder()
          ? _value.smsSender
          // ignore: cast_nullable_to_non_nullable
          : smsSender as SmsSender,
      subscription: subscription == const $CopyWithPlaceholder()
          ? _value.subscription
          // ignore: cast_nullable_to_non_nullable
          : subscription as AdminBrandDetailSubscription,
      credits: credits == const $CopyWithPlaceholder()
          ? _value.credits
          // ignore: cast_nullable_to_non_nullable
          : credits as AdminBrandDetailCredits,
      appointmentsThisMonth:
          appointmentsThisMonth == const $CopyWithPlaceholder()
          ? _value.appointmentsThisMonth
          // ignore: cast_nullable_to_non_nullable
          : appointmentsThisMonth as AdminBrandDetailAppointmentsThisMonth,
      recentCreditTransactions:
          recentCreditTransactions == const $CopyWithPlaceholder()
          ? _value.recentCreditTransactions
          // ignore: cast_nullable_to_non_nullable
          : recentCreditTransactions as List<AdminCreditTransaction>,
      version: version == const $CopyWithPlaceholder()
          ? _value.version
          // ignore: cast_nullable_to_non_nullable
          : version as String,
    );
  }
}

extension $AdminBrandDetailCopyWith on AdminBrandDetail {
  /// Returns a callable class that can be used as follows: `instanceOfAdminBrandDetail.copyWith(...)` or like so:`instanceOfAdminBrandDetail.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminBrandDetailCWProxy get copyWith => _$AdminBrandDetailCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminBrandDetail _$AdminBrandDetailFromJson(Map<String, dynamic> json) =>
    $checkedCreate('AdminBrandDetail', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'id',
          'name',
          'slug',
          'industryCode',
          'city',
          'brandStatus',
          'createdUtc',
          'bookingUrl',
          'owner',
          'branches',
          'activeStaffCount',
          'customerCount',
          'googleCalendarConnectedStaffCount',
          'smsSender',
          'subscription',
          'credits',
          'appointmentsThisMonth',
          'recentCreditTransactions',
          'version',
        ],
      );
      final val = AdminBrandDetail(
        id: $checkedConvert('id', (v) => v as String),
        name: $checkedConvert('name', (v) => v as String),
        slug: $checkedConvert('slug', (v) => v as String),
        industryCode: $checkedConvert(
          'industryCode',
          (v) => $enumDecode(
            _$IndustryCodeEnumMap,
            v,
            unknownValue: IndustryCode.unknownDefaultOpenApi,
          ),
        ),
        city: $checkedConvert('city', (v) => v as String?),
        brandStatus: $checkedConvert(
          'brandStatus',
          (v) => $enumDecode(
            _$AdminBrandDetailBrandStatusEnumEnumMap,
            v,
            unknownValue: AdminBrandDetailBrandStatusEnum.unknownDefaultOpenApi,
          ),
        ),
        createdUtc: $checkedConvert(
          'createdUtc',
          (v) => DateTime.parse(v as String),
        ),
        bookingUrl: $checkedConvert('bookingUrl', (v) => v as String),
        owner: $checkedConvert(
          'owner',
          (v) => AdminBrandDetailOwner.fromJson(v as Map<String, dynamic>),
        ),
        branches: $checkedConvert(
          'branches',
          (v) => (v as List<dynamic>)
              .map((e) => BranchRef.fromJson(e as Map<String, dynamic>))
              .toList(),
        ),
        activeStaffCount: $checkedConvert(
          'activeStaffCount',
          (v) => (v as num).toInt(),
        ),
        customerCount: $checkedConvert(
          'customerCount',
          (v) => (v as num).toInt(),
        ),
        googleCalendarConnectedStaffCount: $checkedConvert(
          'googleCalendarConnectedStaffCount',
          (v) => (v as num).toInt(),
        ),
        smsSender: $checkedConvert(
          'smsSender',
          (v) => SmsSender.fromJson(v as Map<String, dynamic>),
        ),
        subscription: $checkedConvert(
          'subscription',
          (v) =>
              AdminBrandDetailSubscription.fromJson(v as Map<String, dynamic>),
        ),
        credits: $checkedConvert(
          'credits',
          (v) => AdminBrandDetailCredits.fromJson(v as Map<String, dynamic>),
        ),
        appointmentsThisMonth: $checkedConvert(
          'appointmentsThisMonth',
          (v) => AdminBrandDetailAppointmentsThisMonth.fromJson(
            v as Map<String, dynamic>,
          ),
        ),
        recentCreditTransactions: $checkedConvert(
          'recentCreditTransactions',
          (v) => (v as List<dynamic>)
              .map(
                (e) =>
                    AdminCreditTransaction.fromJson(e as Map<String, dynamic>),
              )
              .toList(),
        ),
        version: $checkedConvert('version', (v) => v as String),
      );
      return val;
    });

Map<String, dynamic> _$AdminBrandDetailToJson(AdminBrandDetail instance) =>
    <String, dynamic>{
      'id': instance.id,
      'name': instance.name,
      'slug': instance.slug,
      'industryCode': _$IndustryCodeEnumMap[instance.industryCode]!,
      'city': instance.city,
      'brandStatus':
          _$AdminBrandDetailBrandStatusEnumEnumMap[instance.brandStatus]!,
      'createdUtc': instance.createdUtc.toIso8601String(),
      'bookingUrl': instance.bookingUrl,
      'owner': instance.owner.toJson(),
      'branches': instance.branches.map((e) => e.toJson()).toList(),
      'activeStaffCount': instance.activeStaffCount,
      'customerCount': instance.customerCount,
      'googleCalendarConnectedStaffCount':
          instance.googleCalendarConnectedStaffCount,
      'smsSender': instance.smsSender.toJson(),
      'subscription': instance.subscription.toJson(),
      'credits': instance.credits.toJson(),
      'appointmentsThisMonth': instance.appointmentsThisMonth.toJson(),
      'recentCreditTransactions': instance.recentCreditTransactions
          .map((e) => e.toJson())
          .toList(),
      'version': instance.version,
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

const _$AdminBrandDetailBrandStatusEnumEnumMap = {
  AdminBrandDetailBrandStatusEnum.onboarding: 'Onboarding',
  AdminBrandDetailBrandStatusEnum.active: 'Active',
  AdminBrandDetailBrandStatusEnum.suspended: 'Suspended',
  AdminBrandDetailBrandStatusEnum.archived: 'Archived',
  AdminBrandDetailBrandStatusEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};
