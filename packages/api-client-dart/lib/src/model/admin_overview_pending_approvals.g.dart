// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_overview_pending_approvals.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminOverviewPendingApprovalsCWProxy {
  AdminOverviewPendingApprovals total(int total);

  AdminOverviewPendingApprovals paymentNotices(int paymentNotices);

  AdminOverviewPendingApprovals smsSenders(int smsSenders);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminOverviewPendingApprovals(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminOverviewPendingApprovals(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminOverviewPendingApprovals call({
    int total,
    int paymentNotices,
    int smsSenders,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminOverviewPendingApprovals.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminOverviewPendingApprovals.copyWith.fieldName(...)`
class _$AdminOverviewPendingApprovalsCWProxyImpl
    implements _$AdminOverviewPendingApprovalsCWProxy {
  const _$AdminOverviewPendingApprovalsCWProxyImpl(this._value);

  final AdminOverviewPendingApprovals _value;

  @override
  AdminOverviewPendingApprovals total(int total) => this(total: total);

  @override
  AdminOverviewPendingApprovals paymentNotices(int paymentNotices) =>
      this(paymentNotices: paymentNotices);

  @override
  AdminOverviewPendingApprovals smsSenders(int smsSenders) =>
      this(smsSenders: smsSenders);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminOverviewPendingApprovals(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminOverviewPendingApprovals(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminOverviewPendingApprovals call({
    Object? total = const $CopyWithPlaceholder(),
    Object? paymentNotices = const $CopyWithPlaceholder(),
    Object? smsSenders = const $CopyWithPlaceholder(),
  }) {
    return AdminOverviewPendingApprovals(
      total: total == const $CopyWithPlaceholder()
          ? _value.total
          // ignore: cast_nullable_to_non_nullable
          : total as int,
      paymentNotices: paymentNotices == const $CopyWithPlaceholder()
          ? _value.paymentNotices
          // ignore: cast_nullable_to_non_nullable
          : paymentNotices as int,
      smsSenders: smsSenders == const $CopyWithPlaceholder()
          ? _value.smsSenders
          // ignore: cast_nullable_to_non_nullable
          : smsSenders as int,
    );
  }
}

extension $AdminOverviewPendingApprovalsCopyWith
    on AdminOverviewPendingApprovals {
  /// Returns a callable class that can be used as follows: `instanceOfAdminOverviewPendingApprovals.copyWith(...)` or like so:`instanceOfAdminOverviewPendingApprovals.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminOverviewPendingApprovalsCWProxy get copyWith =>
      _$AdminOverviewPendingApprovalsCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminOverviewPendingApprovals _$AdminOverviewPendingApprovalsFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('AdminOverviewPendingApprovals', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const ['total', 'paymentNotices', 'smsSenders'],
  );
  final val = AdminOverviewPendingApprovals(
    total: $checkedConvert('total', (v) => (v as num).toInt()),
    paymentNotices: $checkedConvert(
      'paymentNotices',
      (v) => (v as num).toInt(),
    ),
    smsSenders: $checkedConvert('smsSenders', (v) => (v as num).toInt()),
  );
  return val;
});

Map<String, dynamic> _$AdminOverviewPendingApprovalsToJson(
  AdminOverviewPendingApprovals instance,
) => <String, dynamic>{
  'total': instance.total,
  'paymentNotices': instance.paymentNotices,
  'smsSenders': instance.smsSenders,
};
