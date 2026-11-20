// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'staff_detail.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$StaffDetailCWProxy {
  StaffDetail id(String id);

  StaffDetail fullName(String fullName);

  StaffDetail displayName(String? displayName);

  StaffDetail title(String? title);

  StaffDetail photoUrl(String? photoUrl);

  StaffDetail calendarColor(String calendarColor);

  StaffDetail isBookableOnline(bool isBookableOnline);

  StaffDetail isActive(bool isActive);

  StaffDetail branchIds(List<String> branchIds);

  StaffDetail role(StaffDetailRoleEnum? role);

  StaffDetail membershipStatus(
    StaffDetailMembershipStatusEnum? membershipStatus,
  );

  StaffDetail phoneMasked(String? phoneMasked);

  StaffDetail bio(String? bio);

  StaffDetail serviceCommissionPercent(num serviceCommissionPercent);

  StaffDetail saleCommissionPercent(num saleCommissionPercent);

  StaffDetail services(List<StaffServiceAssignment> services);

  StaffDetail workingHours(List<StaffDetailAllOfWorkingHours> workingHours);

  StaffDetail googleCalendarStatus(
    StaffDetailGoogleCalendarStatusEnum? googleCalendarStatus,
  );

  StaffDetail version(String version);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `StaffDetail(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// StaffDetail(...).copyWith(id: 12, name: "My name")
  /// ````
  StaffDetail call({
    String id,
    String fullName,
    String? displayName,
    String? title,
    String? photoUrl,
    String calendarColor,
    bool isBookableOnline,
    bool isActive,
    List<String> branchIds,
    StaffDetailRoleEnum? role,
    StaffDetailMembershipStatusEnum? membershipStatus,
    String? phoneMasked,
    String? bio,
    num serviceCommissionPercent,
    num saleCommissionPercent,
    List<StaffServiceAssignment> services,
    List<StaffDetailAllOfWorkingHours> workingHours,
    StaffDetailGoogleCalendarStatusEnum? googleCalendarStatus,
    String version,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfStaffDetail.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfStaffDetail.copyWith.fieldName(...)`
class _$StaffDetailCWProxyImpl implements _$StaffDetailCWProxy {
  const _$StaffDetailCWProxyImpl(this._value);

  final StaffDetail _value;

  @override
  StaffDetail id(String id) => this(id: id);

  @override
  StaffDetail fullName(String fullName) => this(fullName: fullName);

  @override
  StaffDetail displayName(String? displayName) =>
      this(displayName: displayName);

  @override
  StaffDetail title(String? title) => this(title: title);

  @override
  StaffDetail photoUrl(String? photoUrl) => this(photoUrl: photoUrl);

  @override
  StaffDetail calendarColor(String calendarColor) =>
      this(calendarColor: calendarColor);

  @override
  StaffDetail isBookableOnline(bool isBookableOnline) =>
      this(isBookableOnline: isBookableOnline);

  @override
  StaffDetail isActive(bool isActive) => this(isActive: isActive);

  @override
  StaffDetail branchIds(List<String> branchIds) => this(branchIds: branchIds);

  @override
  StaffDetail role(StaffDetailRoleEnum? role) => this(role: role);

  @override
  StaffDetail membershipStatus(
    StaffDetailMembershipStatusEnum? membershipStatus,
  ) => this(membershipStatus: membershipStatus);

  @override
  StaffDetail phoneMasked(String? phoneMasked) =>
      this(phoneMasked: phoneMasked);

  @override
  StaffDetail bio(String? bio) => this(bio: bio);

  @override
  StaffDetail serviceCommissionPercent(num serviceCommissionPercent) =>
      this(serviceCommissionPercent: serviceCommissionPercent);

  @override
  StaffDetail saleCommissionPercent(num saleCommissionPercent) =>
      this(saleCommissionPercent: saleCommissionPercent);

  @override
  StaffDetail services(List<StaffServiceAssignment> services) =>
      this(services: services);

  @override
  StaffDetail workingHours(List<StaffDetailAllOfWorkingHours> workingHours) =>
      this(workingHours: workingHours);

  @override
  StaffDetail googleCalendarStatus(
    StaffDetailGoogleCalendarStatusEnum? googleCalendarStatus,
  ) => this(googleCalendarStatus: googleCalendarStatus);

  @override
  StaffDetail version(String version) => this(version: version);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `StaffDetail(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// StaffDetail(...).copyWith(id: 12, name: "My name")
  /// ````
  StaffDetail call({
    Object? id = const $CopyWithPlaceholder(),
    Object? fullName = const $CopyWithPlaceholder(),
    Object? displayName = const $CopyWithPlaceholder(),
    Object? title = const $CopyWithPlaceholder(),
    Object? photoUrl = const $CopyWithPlaceholder(),
    Object? calendarColor = const $CopyWithPlaceholder(),
    Object? isBookableOnline = const $CopyWithPlaceholder(),
    Object? isActive = const $CopyWithPlaceholder(),
    Object? branchIds = const $CopyWithPlaceholder(),
    Object? role = const $CopyWithPlaceholder(),
    Object? membershipStatus = const $CopyWithPlaceholder(),
    Object? phoneMasked = const $CopyWithPlaceholder(),
    Object? bio = const $CopyWithPlaceholder(),
    Object? serviceCommissionPercent = const $CopyWithPlaceholder(),
    Object? saleCommissionPercent = const $CopyWithPlaceholder(),
    Object? services = const $CopyWithPlaceholder(),
    Object? workingHours = const $CopyWithPlaceholder(),
    Object? googleCalendarStatus = const $CopyWithPlaceholder(),
    Object? version = const $CopyWithPlaceholder(),
  }) {
    return StaffDetail(
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as String,
      fullName: fullName == const $CopyWithPlaceholder()
          ? _value.fullName
          // ignore: cast_nullable_to_non_nullable
          : fullName as String,
      displayName: displayName == const $CopyWithPlaceholder()
          ? _value.displayName
          // ignore: cast_nullable_to_non_nullable
          : displayName as String?,
      title: title == const $CopyWithPlaceholder()
          ? _value.title
          // ignore: cast_nullable_to_non_nullable
          : title as String?,
      photoUrl: photoUrl == const $CopyWithPlaceholder()
          ? _value.photoUrl
          // ignore: cast_nullable_to_non_nullable
          : photoUrl as String?,
      calendarColor: calendarColor == const $CopyWithPlaceholder()
          ? _value.calendarColor
          // ignore: cast_nullable_to_non_nullable
          : calendarColor as String,
      isBookableOnline: isBookableOnline == const $CopyWithPlaceholder()
          ? _value.isBookableOnline
          // ignore: cast_nullable_to_non_nullable
          : isBookableOnline as bool,
      isActive: isActive == const $CopyWithPlaceholder()
          ? _value.isActive
          // ignore: cast_nullable_to_non_nullable
          : isActive as bool,
      branchIds: branchIds == const $CopyWithPlaceholder()
          ? _value.branchIds
          // ignore: cast_nullable_to_non_nullable
          : branchIds as List<String>,
      role: role == const $CopyWithPlaceholder()
          ? _value.role
          // ignore: cast_nullable_to_non_nullable
          : role as StaffDetailRoleEnum?,
      membershipStatus: membershipStatus == const $CopyWithPlaceholder()
          ? _value.membershipStatus
          // ignore: cast_nullable_to_non_nullable
          : membershipStatus as StaffDetailMembershipStatusEnum?,
      phoneMasked: phoneMasked == const $CopyWithPlaceholder()
          ? _value.phoneMasked
          // ignore: cast_nullable_to_non_nullable
          : phoneMasked as String?,
      bio: bio == const $CopyWithPlaceholder()
          ? _value.bio
          // ignore: cast_nullable_to_non_nullable
          : bio as String?,
      serviceCommissionPercent:
          serviceCommissionPercent == const $CopyWithPlaceholder()
          ? _value.serviceCommissionPercent
          // ignore: cast_nullable_to_non_nullable
          : serviceCommissionPercent as num,
      saleCommissionPercent:
          saleCommissionPercent == const $CopyWithPlaceholder()
          ? _value.saleCommissionPercent
          // ignore: cast_nullable_to_non_nullable
          : saleCommissionPercent as num,
      services: services == const $CopyWithPlaceholder()
          ? _value.services
          // ignore: cast_nullable_to_non_nullable
          : services as List<StaffServiceAssignment>,
      workingHours: workingHours == const $CopyWithPlaceholder()
          ? _value.workingHours
          // ignore: cast_nullable_to_non_nullable
          : workingHours as List<StaffDetailAllOfWorkingHours>,
      googleCalendarStatus: googleCalendarStatus == const $CopyWithPlaceholder()
          ? _value.googleCalendarStatus
          // ignore: cast_nullable_to_non_nullable
          : googleCalendarStatus as StaffDetailGoogleCalendarStatusEnum?,
      version: version == const $CopyWithPlaceholder()
          ? _value.version
          // ignore: cast_nullable_to_non_nullable
          : version as String,
    );
  }
}

extension $StaffDetailCopyWith on StaffDetail {
  /// Returns a callable class that can be used as follows: `instanceOfStaffDetail.copyWith(...)` or like so:`instanceOfStaffDetail.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$StaffDetailCWProxy get copyWith => _$StaffDetailCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

StaffDetail _$StaffDetailFromJson(Map<String, dynamic> json) =>
    $checkedCreate('StaffDetail', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'id',
          'fullName',
          'calendarColor',
          'isBookableOnline',
          'isActive',
          'branchIds',
          'serviceCommissionPercent',
          'saleCommissionPercent',
          'services',
          'workingHours',
          'version',
        ],
      );
      final val = StaffDetail(
        id: $checkedConvert('id', (v) => v as String),
        fullName: $checkedConvert('fullName', (v) => v as String),
        displayName: $checkedConvert('displayName', (v) => v as String?),
        title: $checkedConvert('title', (v) => v as String?),
        photoUrl: $checkedConvert('photoUrl', (v) => v as String?),
        calendarColor: $checkedConvert('calendarColor', (v) => v as String),
        isBookableOnline: $checkedConvert('isBookableOnline', (v) => v as bool),
        isActive: $checkedConvert('isActive', (v) => v as bool),
        branchIds: $checkedConvert(
          'branchIds',
          (v) => (v as List<dynamic>).map((e) => e as String).toList(),
        ),
        role: $checkedConvert(
          'role',
          (v) => $enumDecodeNullable(
            _$StaffDetailRoleEnumEnumMap,
            v,
            unknownValue: StaffDetailRoleEnum.unknownDefaultOpenApi,
          ),
        ),
        membershipStatus: $checkedConvert(
          'membershipStatus',
          (v) => $enumDecodeNullable(
            _$StaffDetailMembershipStatusEnumEnumMap,
            v,
            unknownValue: StaffDetailMembershipStatusEnum.unknownDefaultOpenApi,
          ),
        ),
        phoneMasked: $checkedConvert('phoneMasked', (v) => v as String?),
        bio: $checkedConvert('bio', (v) => v as String?),
        serviceCommissionPercent: $checkedConvert(
          'serviceCommissionPercent',
          (v) => v as num,
        ),
        saleCommissionPercent: $checkedConvert(
          'saleCommissionPercent',
          (v) => v as num,
        ),
        services: $checkedConvert(
          'services',
          (v) => (v as List<dynamic>)
              .map(
                (e) =>
                    StaffServiceAssignment.fromJson(e as Map<String, dynamic>),
              )
              .toList(),
        ),
        workingHours: $checkedConvert(
          'workingHours',
          (v) => (v as List<dynamic>)
              .map(
                (e) => StaffDetailAllOfWorkingHours.fromJson(
                  e as Map<String, dynamic>,
                ),
              )
              .toList(),
        ),
        googleCalendarStatus: $checkedConvert(
          'googleCalendarStatus',
          (v) => $enumDecodeNullable(
            _$StaffDetailGoogleCalendarStatusEnumEnumMap,
            v,
            unknownValue:
                StaffDetailGoogleCalendarStatusEnum.unknownDefaultOpenApi,
          ),
        ),
        version: $checkedConvert('version', (v) => v as String),
      );
      return val;
    });

Map<String, dynamic> _$StaffDetailToJson(StaffDetail instance) =>
    <String, dynamic>{
      'id': instance.id,
      'fullName': instance.fullName,
      'displayName': ?instance.displayName,
      'title': ?instance.title,
      'photoUrl': ?instance.photoUrl,
      'calendarColor': instance.calendarColor,
      'isBookableOnline': instance.isBookableOnline,
      'isActive': instance.isActive,
      'branchIds': instance.branchIds,
      'role': ?_$StaffDetailRoleEnumEnumMap[instance.role],
      'membershipStatus':
          ?_$StaffDetailMembershipStatusEnumEnumMap[instance.membershipStatus],
      'phoneMasked': ?instance.phoneMasked,
      'bio': ?instance.bio,
      'serviceCommissionPercent': instance.serviceCommissionPercent,
      'saleCommissionPercent': instance.saleCommissionPercent,
      'services': instance.services.map((e) => e.toJson()).toList(),
      'workingHours': instance.workingHours.map((e) => e.toJson()).toList(),
      'googleCalendarStatus':
          ?_$StaffDetailGoogleCalendarStatusEnumEnumMap[instance
              .googleCalendarStatus],
      'version': instance.version,
    };

const _$StaffDetailRoleEnumEnumMap = {
  StaffDetailRoleEnum.owner: 'Owner',
  StaffDetailRoleEnum.branchManager: 'BranchManager',
  StaffDetailRoleEnum.staff: 'Staff',
  StaffDetailRoleEnum.unknownDefaultOpenApi: 'unknown_default_open_api',
};

const _$StaffDetailMembershipStatusEnumEnumMap = {
  StaffDetailMembershipStatusEnum.invited: 'Invited',
  StaffDetailMembershipStatusEnum.active: 'Active',
  StaffDetailMembershipStatusEnum.disabled: 'Disabled',
  StaffDetailMembershipStatusEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};

const _$StaffDetailGoogleCalendarStatusEnumEnumMap = {
  StaffDetailGoogleCalendarStatusEnum.active: 'Active',
  StaffDetailGoogleCalendarStatusEnum.tokenExpired: 'TokenExpired',
  StaffDetailGoogleCalendarStatusEnum.revoked: 'Revoked',
  StaffDetailGoogleCalendarStatusEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};
