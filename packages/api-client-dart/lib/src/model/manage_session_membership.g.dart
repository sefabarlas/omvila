// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'manage_session_membership.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ManageSessionMembershipCWProxy {
  ManageSessionMembership role(ManageSessionMembershipRoleEnum role);

  ManageSessionMembership branchIds(List<String> branchIds);

  ManageSessionMembership staffId(String? staffId);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageSessionMembership(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageSessionMembership(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageSessionMembership call({
    ManageSessionMembershipRoleEnum role,
    List<String> branchIds,
    String? staffId,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfManageSessionMembership.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfManageSessionMembership.copyWith.fieldName(...)`
class _$ManageSessionMembershipCWProxyImpl
    implements _$ManageSessionMembershipCWProxy {
  const _$ManageSessionMembershipCWProxyImpl(this._value);

  final ManageSessionMembership _value;

  @override
  ManageSessionMembership role(ManageSessionMembershipRoleEnum role) =>
      this(role: role);

  @override
  ManageSessionMembership branchIds(List<String> branchIds) =>
      this(branchIds: branchIds);

  @override
  ManageSessionMembership staffId(String? staffId) => this(staffId: staffId);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageSessionMembership(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageSessionMembership(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageSessionMembership call({
    Object? role = const $CopyWithPlaceholder(),
    Object? branchIds = const $CopyWithPlaceholder(),
    Object? staffId = const $CopyWithPlaceholder(),
  }) {
    return ManageSessionMembership(
      role: role == const $CopyWithPlaceholder()
          ? _value.role
          // ignore: cast_nullable_to_non_nullable
          : role as ManageSessionMembershipRoleEnum,
      branchIds: branchIds == const $CopyWithPlaceholder()
          ? _value.branchIds
          // ignore: cast_nullable_to_non_nullable
          : branchIds as List<String>,
      staffId: staffId == const $CopyWithPlaceholder()
          ? _value.staffId
          // ignore: cast_nullable_to_non_nullable
          : staffId as String?,
    );
  }
}

extension $ManageSessionMembershipCopyWith on ManageSessionMembership {
  /// Returns a callable class that can be used as follows: `instanceOfManageSessionMembership.copyWith(...)` or like so:`instanceOfManageSessionMembership.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ManageSessionMembershipCWProxy get copyWith =>
      _$ManageSessionMembershipCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ManageSessionMembership _$ManageSessionMembershipFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('ManageSessionMembership', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['role', 'branchIds']);
  final val = ManageSessionMembership(
    role: $checkedConvert(
      'role',
      (v) => $enumDecode(
        _$ManageSessionMembershipRoleEnumEnumMap,
        v,
        unknownValue: ManageSessionMembershipRoleEnum.unknownDefaultOpenApi,
      ),
    ),
    branchIds: $checkedConvert(
      'branchIds',
      (v) => (v as List<dynamic>).map((e) => e as String).toList(),
    ),
    staffId: $checkedConvert('staffId', (v) => v as String?),
  );
  return val;
});

Map<String, dynamic> _$ManageSessionMembershipToJson(
  ManageSessionMembership instance,
) => <String, dynamic>{
  'role': _$ManageSessionMembershipRoleEnumEnumMap[instance.role]!,
  'branchIds': instance.branchIds,
  'staffId': ?instance.staffId,
};

const _$ManageSessionMembershipRoleEnumEnumMap = {
  ManageSessionMembershipRoleEnum.owner: 'Owner',
  ManageSessionMembershipRoleEnum.branchManager: 'BranchManager',
  ManageSessionMembershipRoleEnum.staff: 'Staff',
  ManageSessionMembershipRoleEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};
