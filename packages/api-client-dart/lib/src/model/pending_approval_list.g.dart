// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'pending_approval_list.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PendingApprovalListCWProxy {
  PendingApprovalList generatedUtc(DateTime generatedUtc);

  PendingApprovalList totalCount(int totalCount);

  PendingApprovalList earliestHoldExpiresUtc(DateTime? earliestHoldExpiresUtc);

  PendingApprovalList items(List<PendingApprovalItem> items);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PendingApprovalList(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PendingApprovalList(...).copyWith(id: 12, name: "My name")
  /// ````
  PendingApprovalList call({
    DateTime generatedUtc,
    int totalCount,
    DateTime? earliestHoldExpiresUtc,
    List<PendingApprovalItem> items,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPendingApprovalList.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPendingApprovalList.copyWith.fieldName(...)`
class _$PendingApprovalListCWProxyImpl implements _$PendingApprovalListCWProxy {
  const _$PendingApprovalListCWProxyImpl(this._value);

  final PendingApprovalList _value;

  @override
  PendingApprovalList generatedUtc(DateTime generatedUtc) =>
      this(generatedUtc: generatedUtc);

  @override
  PendingApprovalList totalCount(int totalCount) =>
      this(totalCount: totalCount);

  @override
  PendingApprovalList earliestHoldExpiresUtc(
    DateTime? earliestHoldExpiresUtc,
  ) => this(earliestHoldExpiresUtc: earliestHoldExpiresUtc);

  @override
  PendingApprovalList items(List<PendingApprovalItem> items) =>
      this(items: items);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PendingApprovalList(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PendingApprovalList(...).copyWith(id: 12, name: "My name")
  /// ````
  PendingApprovalList call({
    Object? generatedUtc = const $CopyWithPlaceholder(),
    Object? totalCount = const $CopyWithPlaceholder(),
    Object? earliestHoldExpiresUtc = const $CopyWithPlaceholder(),
    Object? items = const $CopyWithPlaceholder(),
  }) {
    return PendingApprovalList(
      generatedUtc: generatedUtc == const $CopyWithPlaceholder()
          ? _value.generatedUtc
          // ignore: cast_nullable_to_non_nullable
          : generatedUtc as DateTime,
      totalCount: totalCount == const $CopyWithPlaceholder()
          ? _value.totalCount
          // ignore: cast_nullable_to_non_nullable
          : totalCount as int,
      earliestHoldExpiresUtc:
          earliestHoldExpiresUtc == const $CopyWithPlaceholder()
          ? _value.earliestHoldExpiresUtc
          // ignore: cast_nullable_to_non_nullable
          : earliestHoldExpiresUtc as DateTime?,
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<PendingApprovalItem>,
    );
  }
}

extension $PendingApprovalListCopyWith on PendingApprovalList {
  /// Returns a callable class that can be used as follows: `instanceOfPendingApprovalList.copyWith(...)` or like so:`instanceOfPendingApprovalList.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PendingApprovalListCWProxy get copyWith =>
      _$PendingApprovalListCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PendingApprovalList _$PendingApprovalListFromJson(Map<String, dynamic> json) =>
    $checkedCreate('PendingApprovalList', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'generatedUtc',
          'totalCount',
          'earliestHoldExpiresUtc',
          'items',
        ],
      );
      final val = PendingApprovalList(
        generatedUtc: $checkedConvert(
          'generatedUtc',
          (v) => DateTime.parse(v as String),
        ),
        totalCount: $checkedConvert('totalCount', (v) => (v as num).toInt()),
        earliestHoldExpiresUtc: $checkedConvert(
          'earliestHoldExpiresUtc',
          (v) => v == null ? null : DateTime.parse(v as String),
        ),
        items: $checkedConvert(
          'items',
          (v) => (v as List<dynamic>)
              .map(
                (e) => PendingApprovalItem.fromJson(e as Map<String, dynamic>),
              )
              .toList(),
        ),
      );
      return val;
    });

Map<String, dynamic> _$PendingApprovalListToJson(
  PendingApprovalList instance,
) => <String, dynamic>{
  'generatedUtc': instance.generatedUtc.toIso8601String(),
  'totalCount': instance.totalCount,
  'earliestHoldExpiresUtc': instance.earliestHoldExpiresUtc?.toIso8601String(),
  'items': instance.items.map((e) => e.toJson()).toList(),
};
