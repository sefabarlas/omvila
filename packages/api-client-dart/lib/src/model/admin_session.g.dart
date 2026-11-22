// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_session.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminSessionCWProxy {
  AdminSession user(AdminSessionUser user);

  AdminSession role(PlatformRole role);

  AdminSession permissions(List<AdminPermission> permissions);

  AdminSession pendingApprovalCount(int pendingApprovalCount);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminSession(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminSession(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminSession call({
    AdminSessionUser user,
    PlatformRole role,
    List<AdminPermission> permissions,
    int pendingApprovalCount,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminSession.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminSession.copyWith.fieldName(...)`
class _$AdminSessionCWProxyImpl implements _$AdminSessionCWProxy {
  const _$AdminSessionCWProxyImpl(this._value);

  final AdminSession _value;

  @override
  AdminSession user(AdminSessionUser user) => this(user: user);

  @override
  AdminSession role(PlatformRole role) => this(role: role);

  @override
  AdminSession permissions(List<AdminPermission> permissions) =>
      this(permissions: permissions);

  @override
  AdminSession pendingApprovalCount(int pendingApprovalCount) =>
      this(pendingApprovalCount: pendingApprovalCount);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminSession(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminSession(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminSession call({
    Object? user = const $CopyWithPlaceholder(),
    Object? role = const $CopyWithPlaceholder(),
    Object? permissions = const $CopyWithPlaceholder(),
    Object? pendingApprovalCount = const $CopyWithPlaceholder(),
  }) {
    return AdminSession(
      user: user == const $CopyWithPlaceholder()
          ? _value.user
          // ignore: cast_nullable_to_non_nullable
          : user as AdminSessionUser,
      role: role == const $CopyWithPlaceholder()
          ? _value.role
          // ignore: cast_nullable_to_non_nullable
          : role as PlatformRole,
      permissions: permissions == const $CopyWithPlaceholder()
          ? _value.permissions
          // ignore: cast_nullable_to_non_nullable
          : permissions as List<AdminPermission>,
      pendingApprovalCount: pendingApprovalCount == const $CopyWithPlaceholder()
          ? _value.pendingApprovalCount
          // ignore: cast_nullable_to_non_nullable
          : pendingApprovalCount as int,
    );
  }
}

extension $AdminSessionCopyWith on AdminSession {
  /// Returns a callable class that can be used as follows: `instanceOfAdminSession.copyWith(...)` or like so:`instanceOfAdminSession.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminSessionCWProxy get copyWith => _$AdminSessionCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminSession _$AdminSessionFromJson(Map<String, dynamic> json) =>
    $checkedCreate('AdminSession', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'user',
          'role',
          'permissions',
          'pendingApprovalCount',
        ],
      );
      final val = AdminSession(
        user: $checkedConvert(
          'user',
          (v) => AdminSessionUser.fromJson(v as Map<String, dynamic>),
        ),
        role: $checkedConvert(
          'role',
          (v) => $enumDecode(
            _$PlatformRoleEnumMap,
            v,
            unknownValue: PlatformRole.unknownDefaultOpenApi,
          ),
        ),
        permissions: $checkedConvert(
          'permissions',
          (v) => (v as List<dynamic>)
              .map(
                (e) => $enumDecode(
                  _$AdminPermissionEnumMap,
                  e,
                  unknownValue: AdminPermission.unknownDefaultOpenApi,
                ),
              )
              .toList(),
        ),
        pendingApprovalCount: $checkedConvert(
          'pendingApprovalCount',
          (v) => (v as num).toInt(),
        ),
      );
      return val;
    });

Map<String, dynamic> _$AdminSessionToJson(AdminSession instance) =>
    <String, dynamic>{
      'user': instance.user.toJson(),
      'role': _$PlatformRoleEnumMap[instance.role]!,
      'permissions': instance.permissions
          .map((e) => _$AdminPermissionEnumMap[e]!)
          .toList(),
      'pendingApprovalCount': instance.pendingApprovalCount,
    };

const _$PlatformRoleEnumMap = {
  PlatformRole.platformAdmin: 'PlatformAdmin',
  PlatformRole.platformSupport: 'PlatformSupport',
  PlatformRole.platformFinance: 'PlatformFinance',
  PlatformRole.unknownDefaultOpenApi: 'unknown_default_open_api',
};

const _$AdminPermissionEnumMap = {
  AdminPermission.platformPeriodBrandsPeriodView: 'platform.brands.view',
  AdminPermission.platformPeriodBrandsPeriodManage: 'platform.brands.manage',
  AdminPermission.platformPeriodPaymentsPeriodMatch: 'platform.payments.match',
  AdminPermission.platformPeriodSmsSendersPeriodDecide:
      'platform.sms-senders.decide',
  AdminPermission.platformPeriodCreditsPeriodGrant: 'platform.credits.grant',
  AdminPermission.platformPeriodPlansPeriodView: 'platform.plans.view',
  AdminPermission.platformPeriodPlansPeriodManage: 'platform.plans.manage',
  AdminPermission.platformPeriodBillingPeriodManage: 'platform.billing.manage',
  AdminPermission.platformPeriodAuditPeriodView: 'platform.audit.view',
  AdminPermission.platformPeriodAuditPeriodExport: 'platform.audit.export',
  AdminPermission.unknownDefaultOpenApi: 'unknown_default_open_api',
};
