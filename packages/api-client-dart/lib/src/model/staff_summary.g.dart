// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'staff_summary.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$StaffSummaryCWProxy {
  StaffSummary id(String id);

  StaffSummary fullName(String fullName);

  StaffSummary displayName(String? displayName);

  StaffSummary title(String? title);

  StaffSummary photoUrl(String? photoUrl);

  StaffSummary calendarColor(String calendarColor);

  StaffSummary isBookableOnline(bool isBookableOnline);

  StaffSummary isActive(bool isActive);

  StaffSummary branchIds(List<String> branchIds);

  StaffSummary role(StaffSummaryRoleEnum? role);

  StaffSummary membershipStatus(
    StaffSummaryMembershipStatusEnum? membershipStatus,
  );

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `StaffSummary(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// StaffSummary(...).copyWith(id: 12, name: "My name")
  /// ````
  StaffSummary call({
    String id,
    String fullName,
    String? displayName,
    String? title,
    String? photoUrl,
    String calendarColor,
    bool isBookableOnline,
    bool isActive,
    List<String> branchIds,
    StaffSummaryRoleEnum? role,
    StaffSummaryMembershipStatusEnum? membershipStatus,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfStaffSummary.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfStaffSummary.copyWith.fieldName(...)`
class _$StaffSummaryCWProxyImpl implements _$StaffSummaryCWProxy {
  const _$StaffSummaryCWProxyImpl(this._value);

  final StaffSummary _value;

  @override
  StaffSummary id(String id) => this(id: id);

  @override
  StaffSummary fullName(String fullName) => this(fullName: fullName);

  @override
  StaffSummary displayName(String? displayName) =>
      this(displayName: displayName);

  @override
  StaffSummary title(String? title) => this(title: title);

  @override
  StaffSummary photoUrl(String? photoUrl) => this(photoUrl: photoUrl);

  @override
  StaffSummary calendarColor(String calendarColor) =>
      this(calendarColor: calendarColor);

  @override
  StaffSummary isBookableOnline(bool isBookableOnline) =>
      this(isBookableOnline: isBookableOnline);

  @override
  StaffSummary isActive(bool isActive) => this(isActive: isActive);

  @override
  StaffSummary branchIds(List<String> branchIds) => this(branchIds: branchIds);

  @override
  StaffSummary role(StaffSummaryRoleEnum? role) => this(role: role);

  @override
  StaffSummary membershipStatus(
    StaffSummaryMembershipStatusEnum? membershipStatus,
  ) => this(membershipStatus: membershipStatus);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `StaffSummary(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// StaffSummary(...).copyWith(id: 12, name: "My name")
  /// ````
  StaffSummary call({
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
  }) {
    return StaffSummary(
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
          : role as StaffSummaryRoleEnum?,
      membershipStatus: membershipStatus == const $CopyWithPlaceholder()
          ? _value.membershipStatus
          // ignore: cast_nullable_to_non_nullable
          : membershipStatus as StaffSummaryMembershipStatusEnum?,
    );
  }
}

extension $StaffSummaryCopyWith on StaffSummary {
  /// Returns a callable class that can be used as follows: `instanceOfStaffSummary.copyWith(...)` or like so:`instanceOfStaffSummary.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$StaffSummaryCWProxy get copyWith => _$StaffSummaryCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

StaffSummary _$StaffSummaryFromJson(Map<String, dynamic> json) =>
    $checkedCreate('StaffSummary', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'id',
          'fullName',
          'calendarColor',
          'isBookableOnline',
          'isActive',
          'branchIds',
        ],
      );
      final val = StaffSummary(
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
            _$StaffSummaryRoleEnumEnumMap,
            v,
            unknownValue: StaffSummaryRoleEnum.unknownDefaultOpenApi,
          ),
        ),
        membershipStatus: $checkedConvert(
          'membershipStatus',
          (v) => $enumDecodeNullable(
            _$StaffSummaryMembershipStatusEnumEnumMap,
            v,
            unknownValue:
                StaffSummaryMembershipStatusEnum.unknownDefaultOpenApi,
          ),
        ),
      );
      return val;
    });

Map<String, dynamic> _$StaffSummaryToJson(StaffSummary instance) =>
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
      'role': ?_$StaffSummaryRoleEnumEnumMap[instance.role],
      'membershipStatus':
          ?_$StaffSummaryMembershipStatusEnumEnumMap[instance.membershipStatus],
    };

const _$StaffSummaryRoleEnumEnumMap = {
  StaffSummaryRoleEnum.owner: 'Owner',
  StaffSummaryRoleEnum.branchManager: 'BranchManager',
  StaffSummaryRoleEnum.staff: 'Staff',
  StaffSummaryRoleEnum.unknownDefaultOpenApi: 'unknown_default_open_api',
};

const _$StaffSummaryMembershipStatusEnumEnumMap = {
  StaffSummaryMembershipStatusEnum.invited: 'Invited',
  StaffSummaryMembershipStatusEnum.active: 'Active',
  StaffSummaryMembershipStatusEnum.disabled: 'Disabled',
  StaffSummaryMembershipStatusEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};
