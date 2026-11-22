// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'subscription_detail_branches.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$SubscriptionDetailBranchesCWProxy {
  SubscriptionDetailBranches count(int count);

  SubscriptionDetailBranches included(int? included);

  SubscriptionDetailBranches extra(int extra);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `SubscriptionDetailBranches(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// SubscriptionDetailBranches(...).copyWith(id: 12, name: "My name")
  /// ````
  SubscriptionDetailBranches call({int count, int? included, int extra});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfSubscriptionDetailBranches.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfSubscriptionDetailBranches.copyWith.fieldName(...)`
class _$SubscriptionDetailBranchesCWProxyImpl
    implements _$SubscriptionDetailBranchesCWProxy {
  const _$SubscriptionDetailBranchesCWProxyImpl(this._value);

  final SubscriptionDetailBranches _value;

  @override
  SubscriptionDetailBranches count(int count) => this(count: count);

  @override
  SubscriptionDetailBranches included(int? included) =>
      this(included: included);

  @override
  SubscriptionDetailBranches extra(int extra) => this(extra: extra);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `SubscriptionDetailBranches(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// SubscriptionDetailBranches(...).copyWith(id: 12, name: "My name")
  /// ````
  SubscriptionDetailBranches call({
    Object? count = const $CopyWithPlaceholder(),
    Object? included = const $CopyWithPlaceholder(),
    Object? extra = const $CopyWithPlaceholder(),
  }) {
    return SubscriptionDetailBranches(
      count: count == const $CopyWithPlaceholder()
          ? _value.count
          // ignore: cast_nullable_to_non_nullable
          : count as int,
      included: included == const $CopyWithPlaceholder()
          ? _value.included
          // ignore: cast_nullable_to_non_nullable
          : included as int?,
      extra: extra == const $CopyWithPlaceholder()
          ? _value.extra
          // ignore: cast_nullable_to_non_nullable
          : extra as int,
    );
  }
}

extension $SubscriptionDetailBranchesCopyWith on SubscriptionDetailBranches {
  /// Returns a callable class that can be used as follows: `instanceOfSubscriptionDetailBranches.copyWith(...)` or like so:`instanceOfSubscriptionDetailBranches.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$SubscriptionDetailBranchesCWProxy get copyWith =>
      _$SubscriptionDetailBranchesCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

SubscriptionDetailBranches _$SubscriptionDetailBranchesFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('SubscriptionDetailBranches', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['count', 'included', 'extra']);
  final val = SubscriptionDetailBranches(
    count: $checkedConvert('count', (v) => (v as num).toInt()),
    included: $checkedConvert('included', (v) => (v as num?)?.toInt()),
    extra: $checkedConvert('extra', (v) => (v as num).toInt()),
  );
  return val;
});

Map<String, dynamic> _$SubscriptionDetailBranchesToJson(
  SubscriptionDetailBranches instance,
) => <String, dynamic>{
  'count': instance.count,
  'included': instance.included,
  'extra': instance.extra,
};
