// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'role_matrix_rows_inner.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$RoleMatrixRowsInnerCWProxy {
  RoleMatrixRowsInner group(RoleMatrixRowsInnerGroupEnum group);

  RoleMatrixRowsInner permissions(List<String> permissions);

  RoleMatrixRowsInner owner(RoleAccessLevel owner);

  RoleMatrixRowsInner branchManager(RoleAccessLevel branchManager);

  RoleMatrixRowsInner staff(RoleAccessLevel staff);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `RoleMatrixRowsInner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// RoleMatrixRowsInner(...).copyWith(id: 12, name: "My name")
  /// ````
  RoleMatrixRowsInner call({
    RoleMatrixRowsInnerGroupEnum group,
    List<String> permissions,
    RoleAccessLevel owner,
    RoleAccessLevel branchManager,
    RoleAccessLevel staff,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfRoleMatrixRowsInner.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfRoleMatrixRowsInner.copyWith.fieldName(...)`
class _$RoleMatrixRowsInnerCWProxyImpl implements _$RoleMatrixRowsInnerCWProxy {
  const _$RoleMatrixRowsInnerCWProxyImpl(this._value);

  final RoleMatrixRowsInner _value;

  @override
  RoleMatrixRowsInner group(RoleMatrixRowsInnerGroupEnum group) =>
      this(group: group);

  @override
  RoleMatrixRowsInner permissions(List<String> permissions) =>
      this(permissions: permissions);

  @override
  RoleMatrixRowsInner owner(RoleAccessLevel owner) => this(owner: owner);

  @override
  RoleMatrixRowsInner branchManager(RoleAccessLevel branchManager) =>
      this(branchManager: branchManager);

  @override
  RoleMatrixRowsInner staff(RoleAccessLevel staff) => this(staff: staff);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `RoleMatrixRowsInner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// RoleMatrixRowsInner(...).copyWith(id: 12, name: "My name")
  /// ````
  RoleMatrixRowsInner call({
    Object? group = const $CopyWithPlaceholder(),
    Object? permissions = const $CopyWithPlaceholder(),
    Object? owner = const $CopyWithPlaceholder(),
    Object? branchManager = const $CopyWithPlaceholder(),
    Object? staff = const $CopyWithPlaceholder(),
  }) {
    return RoleMatrixRowsInner(
      group: group == const $CopyWithPlaceholder()
          ? _value.group
          // ignore: cast_nullable_to_non_nullable
          : group as RoleMatrixRowsInnerGroupEnum,
      permissions: permissions == const $CopyWithPlaceholder()
          ? _value.permissions
          // ignore: cast_nullable_to_non_nullable
          : permissions as List<String>,
      owner: owner == const $CopyWithPlaceholder()
          ? _value.owner
          // ignore: cast_nullable_to_non_nullable
          : owner as RoleAccessLevel,
      branchManager: branchManager == const $CopyWithPlaceholder()
          ? _value.branchManager
          // ignore: cast_nullable_to_non_nullable
          : branchManager as RoleAccessLevel,
      staff: staff == const $CopyWithPlaceholder()
          ? _value.staff
          // ignore: cast_nullable_to_non_nullable
          : staff as RoleAccessLevel,
    );
  }
}

extension $RoleMatrixRowsInnerCopyWith on RoleMatrixRowsInner {
  /// Returns a callable class that can be used as follows: `instanceOfRoleMatrixRowsInner.copyWith(...)` or like so:`instanceOfRoleMatrixRowsInner.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$RoleMatrixRowsInnerCWProxy get copyWith =>
      _$RoleMatrixRowsInnerCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

RoleMatrixRowsInner _$RoleMatrixRowsInnerFromJson(Map<String, dynamic> json) =>
    $checkedCreate('RoleMatrixRowsInner', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'group',
          'permissions',
          'owner',
          'branchManager',
          'staff',
        ],
      );
      final val = RoleMatrixRowsInner(
        group: $checkedConvert(
          'group',
          (v) => $enumDecode(
            _$RoleMatrixRowsInnerGroupEnumEnumMap,
            v,
            unknownValue: RoleMatrixRowsInnerGroupEnum.unknownDefaultOpenApi,
          ),
        ),
        permissions: $checkedConvert(
          'permissions',
          (v) => (v as List<dynamic>).map((e) => e as String).toList(),
        ),
        owner: $checkedConvert(
          'owner',
          (v) => $enumDecode(
            _$RoleAccessLevelEnumMap,
            v,
            unknownValue: RoleAccessLevel.unknownDefaultOpenApi,
          ),
        ),
        branchManager: $checkedConvert(
          'branchManager',
          (v) => $enumDecode(
            _$RoleAccessLevelEnumMap,
            v,
            unknownValue: RoleAccessLevel.unknownDefaultOpenApi,
          ),
        ),
        staff: $checkedConvert(
          'staff',
          (v) => $enumDecode(
            _$RoleAccessLevelEnumMap,
            v,
            unknownValue: RoleAccessLevel.unknownDefaultOpenApi,
          ),
        ),
      );
      return val;
    });

Map<String, dynamic> _$RoleMatrixRowsInnerToJson(
  RoleMatrixRowsInner instance,
) => <String, dynamic>{
  'group': _$RoleMatrixRowsInnerGroupEnumEnumMap[instance.group]!,
  'permissions': instance.permissions,
  'owner': _$RoleAccessLevelEnumMap[instance.owner]!,
  'branchManager': _$RoleAccessLevelEnumMap[instance.branchManager]!,
  'staff': _$RoleAccessLevelEnumMap[instance.staff]!,
};

const _$RoleMatrixRowsInnerGroupEnumEnumMap = {
  RoleMatrixRowsInnerGroupEnum.calendarAndAppointments:
      'CalendarAndAppointments',
  RoleMatrixRowsInnerGroupEnum.customerPhone: 'CustomerPhone',
  RoleMatrixRowsInnerGroupEnum.customerCardAndNotes: 'CustomerCardAndNotes',
  RoleMatrixRowsInnerGroupEnum.cashAndPayments: 'CashAndPayments',
  RoleMatrixRowsInnerGroupEnum.reportsAndCommission: 'ReportsAndCommission',
  RoleMatrixRowsInnerGroupEnum.servicesAndPrices: 'ServicesAndPrices',
  RoleMatrixRowsInnerGroupEnum.settings: 'Settings',
  RoleMatrixRowsInnerGroupEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};

const _$RoleAccessLevelEnumMap = {
  RoleAccessLevel.all: 'All',
  RoleAccessLevel.branch: 'Branch',
  RoleAccessLevel.own: 'Own',
  RoleAccessLevel.none: 'None',
  RoleAccessLevel.unknownDefaultOpenApi: 'unknown_default_open_api',
};
