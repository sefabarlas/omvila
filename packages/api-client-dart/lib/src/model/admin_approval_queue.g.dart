// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_approval_queue.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminApprovalQueueCWProxy {
  AdminApprovalQueue paymentNoticeCount(int paymentNoticeCount);

  AdminApprovalQueue smsSenderCount(int smsSenderCount);

  AdminApprovalQueue oldestSubmittedUtc(DateTime? oldestSubmittedUtc);

  AdminApprovalQueue items(List<AdminApprovalQueueItem> items);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminApprovalQueue(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminApprovalQueue(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminApprovalQueue call({
    int paymentNoticeCount,
    int smsSenderCount,
    DateTime? oldestSubmittedUtc,
    List<AdminApprovalQueueItem> items,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminApprovalQueue.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminApprovalQueue.copyWith.fieldName(...)`
class _$AdminApprovalQueueCWProxyImpl implements _$AdminApprovalQueueCWProxy {
  const _$AdminApprovalQueueCWProxyImpl(this._value);

  final AdminApprovalQueue _value;

  @override
  AdminApprovalQueue paymentNoticeCount(int paymentNoticeCount) =>
      this(paymentNoticeCount: paymentNoticeCount);

  @override
  AdminApprovalQueue smsSenderCount(int smsSenderCount) =>
      this(smsSenderCount: smsSenderCount);

  @override
  AdminApprovalQueue oldestSubmittedUtc(DateTime? oldestSubmittedUtc) =>
      this(oldestSubmittedUtc: oldestSubmittedUtc);

  @override
  AdminApprovalQueue items(List<AdminApprovalQueueItem> items) =>
      this(items: items);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminApprovalQueue(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminApprovalQueue(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminApprovalQueue call({
    Object? paymentNoticeCount = const $CopyWithPlaceholder(),
    Object? smsSenderCount = const $CopyWithPlaceholder(),
    Object? oldestSubmittedUtc = const $CopyWithPlaceholder(),
    Object? items = const $CopyWithPlaceholder(),
  }) {
    return AdminApprovalQueue(
      paymentNoticeCount: paymentNoticeCount == const $CopyWithPlaceholder()
          ? _value.paymentNoticeCount
          // ignore: cast_nullable_to_non_nullable
          : paymentNoticeCount as int,
      smsSenderCount: smsSenderCount == const $CopyWithPlaceholder()
          ? _value.smsSenderCount
          // ignore: cast_nullable_to_non_nullable
          : smsSenderCount as int,
      oldestSubmittedUtc: oldestSubmittedUtc == const $CopyWithPlaceholder()
          ? _value.oldestSubmittedUtc
          // ignore: cast_nullable_to_non_nullable
          : oldestSubmittedUtc as DateTime?,
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<AdminApprovalQueueItem>,
    );
  }
}

extension $AdminApprovalQueueCopyWith on AdminApprovalQueue {
  /// Returns a callable class that can be used as follows: `instanceOfAdminApprovalQueue.copyWith(...)` or like so:`instanceOfAdminApprovalQueue.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminApprovalQueueCWProxy get copyWith =>
      _$AdminApprovalQueueCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminApprovalQueue _$AdminApprovalQueueFromJson(Map<String, dynamic> json) =>
    $checkedCreate('AdminApprovalQueue', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'paymentNoticeCount',
          'smsSenderCount',
          'oldestSubmittedUtc',
          'items',
        ],
      );
      final val = AdminApprovalQueue(
        paymentNoticeCount: $checkedConvert(
          'paymentNoticeCount',
          (v) => (v as num).toInt(),
        ),
        smsSenderCount: $checkedConvert(
          'smsSenderCount',
          (v) => (v as num).toInt(),
        ),
        oldestSubmittedUtc: $checkedConvert(
          'oldestSubmittedUtc',
          (v) => v == null ? null : DateTime.parse(v as String),
        ),
        items: $checkedConvert(
          'items',
          (v) => (v as List<dynamic>)
              .map(
                (e) =>
                    AdminApprovalQueueItem.fromJson(e as Map<String, dynamic>),
              )
              .toList(),
        ),
      );
      return val;
    });

Map<String, dynamic> _$AdminApprovalQueueToJson(AdminApprovalQueue instance) =>
    <String, dynamic>{
      'paymentNoticeCount': instance.paymentNoticeCount,
      'smsSenderCount': instance.smsSenderCount,
      'oldestSubmittedUtc': instance.oldestSubmittedUtc?.toIso8601String(),
      'items': instance.items.map((e) => e.toJson()).toList(),
    };
