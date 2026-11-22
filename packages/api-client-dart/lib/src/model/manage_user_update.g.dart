// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'manage_user_update.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ManageUserUpdateCWProxy {
  ManageUserUpdate role(MembershipRole role);

  ManageUserUpdate branchIds(List<String> branchIds);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageUserUpdate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageUserUpdate(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageUserUpdate call({MembershipRole role, List<String> branchIds});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfManageUserUpdate.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfManageUserUpdate.copyWith.fieldName(...)`
class _$ManageUserUpdateCWProxyImpl implements _$ManageUserUpdateCWProxy {
  const _$ManageUserUpdateCWProxyImpl(this._value);

  final ManageUserUpdate _value;

  @override
  ManageUserUpdate role(MembershipRole role) => this(role: role);

  @override
  ManageUserUpdate branchIds(List<String> branchIds) =>
      this(branchIds: branchIds);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageUserUpdate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageUserUpdate(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageUserUpdate call({
    Object? role = const $CopyWithPlaceholder(),
    Object? branchIds = const $CopyWithPlaceholder(),
  }) {
    return ManageUserUpdate(
      role: role == const $CopyWithPlaceholder()
          ? _value.role
          // ignore: cast_nullable_to_non_nullable
          : role as MembershipRole,
      branchIds: branchIds == const $CopyWithPlaceholder()
          ? _value.branchIds
          // ignore: cast_nullable_to_non_nullable
          : branchIds as List<String>,
    );
  }
}

extension $ManageUserUpdateCopyWith on ManageUserUpdate {
  /// Returns a callable class that can be used as follows: `instanceOfManageUserUpdate.copyWith(...)` or like so:`instanceOfManageUserUpdate.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ManageUserUpdateCWProxy get copyWith => _$ManageUserUpdateCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ManageUserUpdate _$ManageUserUpdateFromJson(Map<String, dynamic> json) =>
    $checkedCreate('ManageUserUpdate', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['role', 'branchIds']);
      final val = ManageUserUpdate(
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
      );
      return val;
    });

Map<String, dynamic> _$ManageUserUpdateToJson(ManageUserUpdate instance) =>
    <String, dynamic>{
      'role': _$MembershipRoleEnumMap[instance.role]!,
      'branchIds': instance.branchIds,
    };

const _$MembershipRoleEnumMap = {
  MembershipRole.owner: 'Owner',
  MembershipRole.branchManager: 'BranchManager',
  MembershipRole.staff: 'Staff',
  MembershipRole.unknownDefaultOpenApi: 'unknown_default_open_api',
};
