// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'manage_overview_pending_approval.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ManageOverviewPendingApprovalCWProxy {
  ManageOverviewPendingApproval count(int count);

  ManageOverviewPendingApproval earliestHoldExpiresUtc(
    DateTime? earliestHoldExpiresUtc,
  );

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageOverviewPendingApproval(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageOverviewPendingApproval(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageOverviewPendingApproval call({
    int count,
    DateTime? earliestHoldExpiresUtc,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfManageOverviewPendingApproval.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfManageOverviewPendingApproval.copyWith.fieldName(...)`
class _$ManageOverviewPendingApprovalCWProxyImpl
    implements _$ManageOverviewPendingApprovalCWProxy {
  const _$ManageOverviewPendingApprovalCWProxyImpl(this._value);

  final ManageOverviewPendingApproval _value;

  @override
  ManageOverviewPendingApproval count(int count) => this(count: count);

  @override
  ManageOverviewPendingApproval earliestHoldExpiresUtc(
    DateTime? earliestHoldExpiresUtc,
  ) => this(earliestHoldExpiresUtc: earliestHoldExpiresUtc);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageOverviewPendingApproval(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageOverviewPendingApproval(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageOverviewPendingApproval call({
    Object? count = const $CopyWithPlaceholder(),
    Object? earliestHoldExpiresUtc = const $CopyWithPlaceholder(),
  }) {
    return ManageOverviewPendingApproval(
      count: count == const $CopyWithPlaceholder()
          ? _value.count
          // ignore: cast_nullable_to_non_nullable
          : count as int,
      earliestHoldExpiresUtc:
          earliestHoldExpiresUtc == const $CopyWithPlaceholder()
          ? _value.earliestHoldExpiresUtc
          // ignore: cast_nullable_to_non_nullable
          : earliestHoldExpiresUtc as DateTime?,
    );
  }
}

extension $ManageOverviewPendingApprovalCopyWith
    on ManageOverviewPendingApproval {
  /// Returns a callable class that can be used as follows: `instanceOfManageOverviewPendingApproval.copyWith(...)` or like so:`instanceOfManageOverviewPendingApproval.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ManageOverviewPendingApprovalCWProxy get copyWith =>
      _$ManageOverviewPendingApprovalCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ManageOverviewPendingApproval _$ManageOverviewPendingApprovalFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('ManageOverviewPendingApproval', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['count', 'earliestHoldExpiresUtc']);
  final val = ManageOverviewPendingApproval(
    count: $checkedConvert('count', (v) => (v as num).toInt()),
    earliestHoldExpiresUtc: $checkedConvert(
      'earliestHoldExpiresUtc',
      (v) => v == null ? null : DateTime.parse(v as String),
    ),
  );
  return val;
});

Map<String, dynamic> _$ManageOverviewPendingApprovalToJson(
  ManageOverviewPendingApproval instance,
) => <String, dynamic>{
  'count': instance.count,
  'earliestHoldExpiresUtc': instance.earliestHoldExpiresUtc?.toIso8601String(),
};
