// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_overview_trialing.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminOverviewTrialingCWProxy {
  AdminOverviewTrialing count(int count);

  AdminOverviewTrialing endingThisWeek(int endingThisWeek);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminOverviewTrialing(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminOverviewTrialing(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminOverviewTrialing call({int count, int endingThisWeek});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminOverviewTrialing.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminOverviewTrialing.copyWith.fieldName(...)`
class _$AdminOverviewTrialingCWProxyImpl
    implements _$AdminOverviewTrialingCWProxy {
  const _$AdminOverviewTrialingCWProxyImpl(this._value);

  final AdminOverviewTrialing _value;

  @override
  AdminOverviewTrialing count(int count) => this(count: count);

  @override
  AdminOverviewTrialing endingThisWeek(int endingThisWeek) =>
      this(endingThisWeek: endingThisWeek);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminOverviewTrialing(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminOverviewTrialing(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminOverviewTrialing call({
    Object? count = const $CopyWithPlaceholder(),
    Object? endingThisWeek = const $CopyWithPlaceholder(),
  }) {
    return AdminOverviewTrialing(
      count: count == const $CopyWithPlaceholder()
          ? _value.count
          // ignore: cast_nullable_to_non_nullable
          : count as int,
      endingThisWeek: endingThisWeek == const $CopyWithPlaceholder()
          ? _value.endingThisWeek
          // ignore: cast_nullable_to_non_nullable
          : endingThisWeek as int,
    );
  }
}

extension $AdminOverviewTrialingCopyWith on AdminOverviewTrialing {
  /// Returns a callable class that can be used as follows: `instanceOfAdminOverviewTrialing.copyWith(...)` or like so:`instanceOfAdminOverviewTrialing.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminOverviewTrialingCWProxy get copyWith =>
      _$AdminOverviewTrialingCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminOverviewTrialing _$AdminOverviewTrialingFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('AdminOverviewTrialing', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['count', 'endingThisWeek']);
  final val = AdminOverviewTrialing(
    count: $checkedConvert('count', (v) => (v as num).toInt()),
    endingThisWeek: $checkedConvert(
      'endingThisWeek',
      (v) => (v as num).toInt(),
    ),
  );
  return val;
});

Map<String, dynamic> _$AdminOverviewTrialingToJson(
  AdminOverviewTrialing instance,
) => <String, dynamic>{
  'count': instance.count,
  'endingThisWeek': instance.endingThisWeek,
};
