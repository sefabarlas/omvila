// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_brand_page_counts.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminBrandPageCountsCWProxy {
  AdminBrandPageCounts active(int active);

  AdminBrandPageCounts trialing(int trialing);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminBrandPageCounts(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminBrandPageCounts(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminBrandPageCounts call({int active, int trialing});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminBrandPageCounts.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminBrandPageCounts.copyWith.fieldName(...)`
class _$AdminBrandPageCountsCWProxyImpl
    implements _$AdminBrandPageCountsCWProxy {
  const _$AdminBrandPageCountsCWProxyImpl(this._value);

  final AdminBrandPageCounts _value;

  @override
  AdminBrandPageCounts active(int active) => this(active: active);

  @override
  AdminBrandPageCounts trialing(int trialing) => this(trialing: trialing);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminBrandPageCounts(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminBrandPageCounts(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminBrandPageCounts call({
    Object? active = const $CopyWithPlaceholder(),
    Object? trialing = const $CopyWithPlaceholder(),
  }) {
    return AdminBrandPageCounts(
      active: active == const $CopyWithPlaceholder()
          ? _value.active
          // ignore: cast_nullable_to_non_nullable
          : active as int,
      trialing: trialing == const $CopyWithPlaceholder()
          ? _value.trialing
          // ignore: cast_nullable_to_non_nullable
          : trialing as int,
    );
  }
}

extension $AdminBrandPageCountsCopyWith on AdminBrandPageCounts {
  /// Returns a callable class that can be used as follows: `instanceOfAdminBrandPageCounts.copyWith(...)` or like so:`instanceOfAdminBrandPageCounts.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminBrandPageCountsCWProxy get copyWith =>
      _$AdminBrandPageCountsCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminBrandPageCounts _$AdminBrandPageCountsFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('AdminBrandPageCounts', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['active', 'trialing']);
  final val = AdminBrandPageCounts(
    active: $checkedConvert('active', (v) => (v as num).toInt()),
    trialing: $checkedConvert('trialing', (v) => (v as num).toInt()),
  );
  return val;
});

Map<String, dynamic> _$AdminBrandPageCountsToJson(
  AdminBrandPageCounts instance,
) => <String, dynamic>{
  'active': instance.active,
  'trialing': instance.trialing,
};
