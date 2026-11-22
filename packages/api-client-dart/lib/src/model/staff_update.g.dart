// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'staff_update.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$StaffUpdateCWProxy {
  StaffUpdate fullName(String? fullName);

  StaffUpdate displayName(String? displayName);

  StaffUpdate title(String? title);

  StaffUpdate bio(String? bio);

  StaffUpdate role(StaffUpdateRoleEnum? role);

  StaffUpdate calendarColor(String? calendarColor);

  StaffUpdate branchIds(List<String>? branchIds);

  StaffUpdate serviceCommissionPercent(num? serviceCommissionPercent);

  StaffUpdate saleCommissionPercent(num? saleCommissionPercent);

  StaffUpdate isBookableOnline(bool? isBookableOnline);

  StaffUpdate isActive(bool? isActive);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `StaffUpdate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// StaffUpdate(...).copyWith(id: 12, name: "My name")
  /// ````
  StaffUpdate call({
    String? fullName,
    String? displayName,
    String? title,
    String? bio,
    StaffUpdateRoleEnum? role,
    String? calendarColor,
    List<String>? branchIds,
    num? serviceCommissionPercent,
    num? saleCommissionPercent,
    bool? isBookableOnline,
    bool? isActive,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfStaffUpdate.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfStaffUpdate.copyWith.fieldName(...)`
class _$StaffUpdateCWProxyImpl implements _$StaffUpdateCWProxy {
  const _$StaffUpdateCWProxyImpl(this._value);

  final StaffUpdate _value;

  @override
  StaffUpdate fullName(String? fullName) => this(fullName: fullName);

  @override
  StaffUpdate displayName(String? displayName) =>
      this(displayName: displayName);

  @override
  StaffUpdate title(String? title) => this(title: title);

  @override
  StaffUpdate bio(String? bio) => this(bio: bio);

  @override
  StaffUpdate role(StaffUpdateRoleEnum? role) => this(role: role);

  @override
  StaffUpdate calendarColor(String? calendarColor) =>
      this(calendarColor: calendarColor);

  @override
  StaffUpdate branchIds(List<String>? branchIds) => this(branchIds: branchIds);

  @override
  StaffUpdate serviceCommissionPercent(num? serviceCommissionPercent) =>
      this(serviceCommissionPercent: serviceCommissionPercent);

  @override
  StaffUpdate saleCommissionPercent(num? saleCommissionPercent) =>
      this(saleCommissionPercent: saleCommissionPercent);

  @override
  StaffUpdate isBookableOnline(bool? isBookableOnline) =>
      this(isBookableOnline: isBookableOnline);

  @override
  StaffUpdate isActive(bool? isActive) => this(isActive: isActive);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `StaffUpdate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// StaffUpdate(...).copyWith(id: 12, name: "My name")
  /// ````
  StaffUpdate call({
    Object? fullName = const $CopyWithPlaceholder(),
    Object? displayName = const $CopyWithPlaceholder(),
    Object? title = const $CopyWithPlaceholder(),
    Object? bio = const $CopyWithPlaceholder(),
    Object? role = const $CopyWithPlaceholder(),
    Object? calendarColor = const $CopyWithPlaceholder(),
    Object? branchIds = const $CopyWithPlaceholder(),
    Object? serviceCommissionPercent = const $CopyWithPlaceholder(),
    Object? saleCommissionPercent = const $CopyWithPlaceholder(),
    Object? isBookableOnline = const $CopyWithPlaceholder(),
    Object? isActive = const $CopyWithPlaceholder(),
  }) {
    return StaffUpdate(
      fullName: fullName == const $CopyWithPlaceholder()
          ? _value.fullName
          // ignore: cast_nullable_to_non_nullable
          : fullName as String?,
      displayName: displayName == const $CopyWithPlaceholder()
          ? _value.displayName
          // ignore: cast_nullable_to_non_nullable
          : displayName as String?,
      title: title == const $CopyWithPlaceholder()
          ? _value.title
          // ignore: cast_nullable_to_non_nullable
          : title as String?,
      bio: bio == const $CopyWithPlaceholder()
          ? _value.bio
          // ignore: cast_nullable_to_non_nullable
          : bio as String?,
      role: role == const $CopyWithPlaceholder()
          ? _value.role
          // ignore: cast_nullable_to_non_nullable
          : role as StaffUpdateRoleEnum?,
      calendarColor: calendarColor == const $CopyWithPlaceholder()
          ? _value.calendarColor
          // ignore: cast_nullable_to_non_nullable
          : calendarColor as String?,
      branchIds: branchIds == const $CopyWithPlaceholder()
          ? _value.branchIds
          // ignore: cast_nullable_to_non_nullable
          : branchIds as List<String>?,
      serviceCommissionPercent:
          serviceCommissionPercent == const $CopyWithPlaceholder()
          ? _value.serviceCommissionPercent
          // ignore: cast_nullable_to_non_nullable
          : serviceCommissionPercent as num?,
      saleCommissionPercent:
          saleCommissionPercent == const $CopyWithPlaceholder()
          ? _value.saleCommissionPercent
          // ignore: cast_nullable_to_non_nullable
          : saleCommissionPercent as num?,
      isBookableOnline: isBookableOnline == const $CopyWithPlaceholder()
          ? _value.isBookableOnline
          // ignore: cast_nullable_to_non_nullable
          : isBookableOnline as bool?,
      isActive: isActive == const $CopyWithPlaceholder()
          ? _value.isActive
          // ignore: cast_nullable_to_non_nullable
          : isActive as bool?,
    );
  }
}

extension $StaffUpdateCopyWith on StaffUpdate {
  /// Returns a callable class that can be used as follows: `instanceOfStaffUpdate.copyWith(...)` or like so:`instanceOfStaffUpdate.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$StaffUpdateCWProxy get copyWith => _$StaffUpdateCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

StaffUpdate _$StaffUpdateFromJson(Map<String, dynamic> json) => $checkedCreate(
  'StaffUpdate',
  json,
  ($checkedConvert) {
    final val = StaffUpdate(
      fullName: $checkedConvert('fullName', (v) => v as String?),
      displayName: $checkedConvert('displayName', (v) => v as String?),
      title: $checkedConvert('title', (v) => v as String?),
      bio: $checkedConvert('bio', (v) => v as String?),
      role: $checkedConvert(
        'role',
        (v) => $enumDecodeNullable(
          _$StaffUpdateRoleEnumEnumMap,
          v,
          unknownValue: StaffUpdateRoleEnum.unknownDefaultOpenApi,
        ),
      ),
      calendarColor: $checkedConvert('calendarColor', (v) => v as String?),
      branchIds: $checkedConvert(
        'branchIds',
        (v) => (v as List<dynamic>?)?.map((e) => e as String).toList(),
      ),
      serviceCommissionPercent: $checkedConvert(
        'serviceCommissionPercent',
        (v) => v as num?,
      ),
      saleCommissionPercent: $checkedConvert(
        'saleCommissionPercent',
        (v) => v as num?,
      ),
      isBookableOnline: $checkedConvert('isBookableOnline', (v) => v as bool?),
      isActive: $checkedConvert('isActive', (v) => v as bool?),
    );
    return val;
  },
);

Map<String, dynamic> _$StaffUpdateToJson(StaffUpdate instance) =>
    <String, dynamic>{
      'fullName': ?instance.fullName,
      'displayName': instance.displayName,
      'title': instance.title,
      'bio': instance.bio,
      'role': ?_$StaffUpdateRoleEnumEnumMap[instance.role],
      'calendarColor': ?instance.calendarColor,
      'branchIds': ?instance.branchIds,
      'serviceCommissionPercent': ?instance.serviceCommissionPercent,
      'saleCommissionPercent': ?instance.saleCommissionPercent,
      'isBookableOnline': ?instance.isBookableOnline,
      'isActive': ?instance.isActive,
    };

const _$StaffUpdateRoleEnumEnumMap = {
  StaffUpdateRoleEnum.branchManager: 'BranchManager',
  StaffUpdateRoleEnum.staff: 'Staff',
  StaffUpdateRoleEnum.unknownDefaultOpenApi: 'unknown_default_open_api',
};
