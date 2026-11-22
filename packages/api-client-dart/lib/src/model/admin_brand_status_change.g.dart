// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_brand_status_change.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminBrandStatusChangeCWProxy {
  AdminBrandStatusChange status(AdminBrandStatusChangeStatusEnum status);

  AdminBrandStatusChange reason(String reason);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminBrandStatusChange(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminBrandStatusChange(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminBrandStatusChange call({
    AdminBrandStatusChangeStatusEnum status,
    String reason,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminBrandStatusChange.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminBrandStatusChange.copyWith.fieldName(...)`
class _$AdminBrandStatusChangeCWProxyImpl
    implements _$AdminBrandStatusChangeCWProxy {
  const _$AdminBrandStatusChangeCWProxyImpl(this._value);

  final AdminBrandStatusChange _value;

  @override
  AdminBrandStatusChange status(AdminBrandStatusChangeStatusEnum status) =>
      this(status: status);

  @override
  AdminBrandStatusChange reason(String reason) => this(reason: reason);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminBrandStatusChange(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminBrandStatusChange(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminBrandStatusChange call({
    Object? status = const $CopyWithPlaceholder(),
    Object? reason = const $CopyWithPlaceholder(),
  }) {
    return AdminBrandStatusChange(
      status: status == const $CopyWithPlaceholder()
          ? _value.status
          // ignore: cast_nullable_to_non_nullable
          : status as AdminBrandStatusChangeStatusEnum,
      reason: reason == const $CopyWithPlaceholder()
          ? _value.reason
          // ignore: cast_nullable_to_non_nullable
          : reason as String,
    );
  }
}

extension $AdminBrandStatusChangeCopyWith on AdminBrandStatusChange {
  /// Returns a callable class that can be used as follows: `instanceOfAdminBrandStatusChange.copyWith(...)` or like so:`instanceOfAdminBrandStatusChange.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminBrandStatusChangeCWProxy get copyWith =>
      _$AdminBrandStatusChangeCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminBrandStatusChange _$AdminBrandStatusChangeFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('AdminBrandStatusChange', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['status', 'reason']);
  final val = AdminBrandStatusChange(
    status: $checkedConvert(
      'status',
      (v) => $enumDecode(
        _$AdminBrandStatusChangeStatusEnumEnumMap,
        v,
        unknownValue: AdminBrandStatusChangeStatusEnum.unknownDefaultOpenApi,
      ),
    ),
    reason: $checkedConvert('reason', (v) => v as String),
  );
  return val;
});

Map<String, dynamic> _$AdminBrandStatusChangeToJson(
  AdminBrandStatusChange instance,
) => <String, dynamic>{
  'status': _$AdminBrandStatusChangeStatusEnumEnumMap[instance.status]!,
  'reason': instance.reason,
};

const _$AdminBrandStatusChangeStatusEnumEnumMap = {
  AdminBrandStatusChangeStatusEnum.active: 'Active',
  AdminBrandStatusChangeStatusEnum.suspended: 'Suspended',
  AdminBrandStatusChangeStatusEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};
