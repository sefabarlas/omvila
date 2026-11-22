// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'manage_user.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ManageUserCWProxy {
  ManageUser userId(String userId);

  ManageUser fullName(String fullName);

  ManageUser phoneMasked(String phoneMasked);

  ManageUser role(MembershipRole role);

  ManageUser branchIds(List<String> branchIds);

  ManageUser status(ManageUserStatusEnum status);

  ManageUser staffId(String? staffId);

  ManageUser isCurrentUser(bool isCurrentUser);

  ManageUser version(String version);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageUser(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageUser(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageUser call({
    String userId,
    String fullName,
    String phoneMasked,
    MembershipRole role,
    List<String> branchIds,
    ManageUserStatusEnum status,
    String? staffId,
    bool isCurrentUser,
    String version,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfManageUser.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfManageUser.copyWith.fieldName(...)`
class _$ManageUserCWProxyImpl implements _$ManageUserCWProxy {
  const _$ManageUserCWProxyImpl(this._value);

  final ManageUser _value;

  @override
  ManageUser userId(String userId) => this(userId: userId);

  @override
  ManageUser fullName(String fullName) => this(fullName: fullName);

  @override
  ManageUser phoneMasked(String phoneMasked) => this(phoneMasked: phoneMasked);

  @override
  ManageUser role(MembershipRole role) => this(role: role);

  @override
  ManageUser branchIds(List<String> branchIds) => this(branchIds: branchIds);

  @override
  ManageUser status(ManageUserStatusEnum status) => this(status: status);

  @override
  ManageUser staffId(String? staffId) => this(staffId: staffId);

  @override
  ManageUser isCurrentUser(bool isCurrentUser) =>
      this(isCurrentUser: isCurrentUser);

  @override
  ManageUser version(String version) => this(version: version);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageUser(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageUser(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageUser call({
    Object? userId = const $CopyWithPlaceholder(),
    Object? fullName = const $CopyWithPlaceholder(),
    Object? phoneMasked = const $CopyWithPlaceholder(),
    Object? role = const $CopyWithPlaceholder(),
    Object? branchIds = const $CopyWithPlaceholder(),
    Object? status = const $CopyWithPlaceholder(),
    Object? staffId = const $CopyWithPlaceholder(),
    Object? isCurrentUser = const $CopyWithPlaceholder(),
    Object? version = const $CopyWithPlaceholder(),
  }) {
    return ManageUser(
      userId: userId == const $CopyWithPlaceholder()
          ? _value.userId
          // ignore: cast_nullable_to_non_nullable
          : userId as String,
      fullName: fullName == const $CopyWithPlaceholder()
          ? _value.fullName
          // ignore: cast_nullable_to_non_nullable
          : fullName as String,
      phoneMasked: phoneMasked == const $CopyWithPlaceholder()
          ? _value.phoneMasked
          // ignore: cast_nullable_to_non_nullable
          : phoneMasked as String,
      role: role == const $CopyWithPlaceholder()
          ? _value.role
          // ignore: cast_nullable_to_non_nullable
          : role as MembershipRole,
      branchIds: branchIds == const $CopyWithPlaceholder()
          ? _value.branchIds
          // ignore: cast_nullable_to_non_nullable
          : branchIds as List<String>,
      status: status == const $CopyWithPlaceholder()
          ? _value.status
          // ignore: cast_nullable_to_non_nullable
          : status as ManageUserStatusEnum,
      staffId: staffId == const $CopyWithPlaceholder()
          ? _value.staffId
          // ignore: cast_nullable_to_non_nullable
          : staffId as String?,
      isCurrentUser: isCurrentUser == const $CopyWithPlaceholder()
          ? _value.isCurrentUser
          // ignore: cast_nullable_to_non_nullable
          : isCurrentUser as bool,
      version: version == const $CopyWithPlaceholder()
          ? _value.version
          // ignore: cast_nullable_to_non_nullable
          : version as String,
    );
  }
}

extension $ManageUserCopyWith on ManageUser {
  /// Returns a callable class that can be used as follows: `instanceOfManageUser.copyWith(...)` or like so:`instanceOfManageUser.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ManageUserCWProxy get copyWith => _$ManageUserCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ManageUser _$ManageUserFromJson(Map<String, dynamic> json) =>
    $checkedCreate('ManageUser', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'userId',
          'fullName',
          'phoneMasked',
          'role',
          'branchIds',
          'status',
          'staffId',
          'isCurrentUser',
          'version',
        ],
      );
      final val = ManageUser(
        userId: $checkedConvert('userId', (v) => v as String),
        fullName: $checkedConvert('fullName', (v) => v as String),
        phoneMasked: $checkedConvert('phoneMasked', (v) => v as String),
        role: $checkedConvert(
          'role',
          (v) => $enumDecode(
            _$MembershipRoleEnumMap,
            v,
            unknownValue: MembershipRole.unknownDefaultOpenApi,
          ),
        ),
        branchIds: $checkedConvert(
          'branchIds',
          (v) => (v as List<dynamic>).map((e) => e as String).toList(),
        ),
        status: $checkedConvert(
          'status',
          (v) => $enumDecode(
            _$ManageUserStatusEnumEnumMap,
            v,
            unknownValue: ManageUserStatusEnum.unknownDefaultOpenApi,
          ),
        ),
        staffId: $checkedConvert('staffId', (v) => v as String?),
        isCurrentUser: $checkedConvert('isCurrentUser', (v) => v as bool),
        version: $checkedConvert('version', (v) => v as String),
      );
      return val;
    });

Map<String, dynamic> _$ManageUserToJson(ManageUser instance) =>
    <String, dynamic>{
      'userId': instance.userId,
      'fullName': instance.fullName,
      'phoneMasked': instance.phoneMasked,
      'role': _$MembershipRoleEnumMap[instance.role]!,
      'branchIds': instance.branchIds,
      'status': _$ManageUserStatusEnumEnumMap[instance.status]!,
      'staffId': instance.staffId,
      'isCurrentUser': instance.isCurrentUser,
      'version': instance.version,
    };

const _$MembershipRoleEnumMap = {
  MembershipRole.owner: 'Owner',
  MembershipRole.branchManager: 'BranchManager',
  MembershipRole.staff: 'Staff',
  MembershipRole.unknownDefaultOpenApi: 'unknown_default_open_api',
};

const _$ManageUserStatusEnumEnumMap = {
  ManageUserStatusEnum.invited: 'Invited',
  ManageUserStatusEnum.active: 'Active',
  ManageUserStatusEnum.disabled: 'Disabled',
  ManageUserStatusEnum.unknownDefaultOpenApi: 'unknown_default_open_api',
};
