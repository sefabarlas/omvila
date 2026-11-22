// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'subscription_detail_staff.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$SubscriptionDetailStaffCWProxy {
  SubscriptionDetailStaff count(int count);

  SubscriptionDetailStaff limit(int? limit);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `SubscriptionDetailStaff(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// SubscriptionDetailStaff(...).copyWith(id: 12, name: "My name")
  /// ````
  SubscriptionDetailStaff call({int count, int? limit});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfSubscriptionDetailStaff.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfSubscriptionDetailStaff.copyWith.fieldName(...)`
class _$SubscriptionDetailStaffCWProxyImpl
    implements _$SubscriptionDetailStaffCWProxy {
  const _$SubscriptionDetailStaffCWProxyImpl(this._value);

  final SubscriptionDetailStaff _value;

  @override
  SubscriptionDetailStaff count(int count) => this(count: count);

  @override
  SubscriptionDetailStaff limit(int? limit) => this(limit: limit);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `SubscriptionDetailStaff(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// SubscriptionDetailStaff(...).copyWith(id: 12, name: "My name")
  /// ````
  SubscriptionDetailStaff call({
    Object? count = const $CopyWithPlaceholder(),
    Object? limit = const $CopyWithPlaceholder(),
  }) {
    return SubscriptionDetailStaff(
      count: count == const $CopyWithPlaceholder()
          ? _value.count
          // ignore: cast_nullable_to_non_nullable
          : count as int,
      limit: limit == const $CopyWithPlaceholder()
          ? _value.limit
          // ignore: cast_nullable_to_non_nullable
          : limit as int?,
    );
  }
}

extension $SubscriptionDetailStaffCopyWith on SubscriptionDetailStaff {
  /// Returns a callable class that can be used as follows: `instanceOfSubscriptionDetailStaff.copyWith(...)` or like so:`instanceOfSubscriptionDetailStaff.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$SubscriptionDetailStaffCWProxy get copyWith =>
      _$SubscriptionDetailStaffCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

SubscriptionDetailStaff _$SubscriptionDetailStaffFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('SubscriptionDetailStaff', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['count', 'limit']);
  final val = SubscriptionDetailStaff(
    count: $checkedConvert('count', (v) => (v as num).toInt()),
    limit: $checkedConvert('limit', (v) => (v as num?)?.toInt()),
  );
  return val;
});

Map<String, dynamic> _$SubscriptionDetailStaffToJson(
  SubscriptionDetailStaff instance,
) => <String, dynamic>{'count': instance.count, 'limit': instance.limit};
