// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_overview_past_due.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminOverviewPastDueCWProxy {
  AdminOverviewPastDue count(int count);

  AdminOverviewPastDue readOnlySoon(int readOnlySoon);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminOverviewPastDue(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminOverviewPastDue(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminOverviewPastDue call({int count, int readOnlySoon});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminOverviewPastDue.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminOverviewPastDue.copyWith.fieldName(...)`
class _$AdminOverviewPastDueCWProxyImpl
    implements _$AdminOverviewPastDueCWProxy {
  const _$AdminOverviewPastDueCWProxyImpl(this._value);

  final AdminOverviewPastDue _value;

  @override
  AdminOverviewPastDue count(int count) => this(count: count);

  @override
  AdminOverviewPastDue readOnlySoon(int readOnlySoon) =>
      this(readOnlySoon: readOnlySoon);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminOverviewPastDue(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminOverviewPastDue(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminOverviewPastDue call({
    Object? count = const $CopyWithPlaceholder(),
    Object? readOnlySoon = const $CopyWithPlaceholder(),
  }) {
    return AdminOverviewPastDue(
      count: count == const $CopyWithPlaceholder()
          ? _value.count
          // ignore: cast_nullable_to_non_nullable
          : count as int,
      readOnlySoon: readOnlySoon == const $CopyWithPlaceholder()
          ? _value.readOnlySoon
          // ignore: cast_nullable_to_non_nullable
          : readOnlySoon as int,
    );
  }
}

extension $AdminOverviewPastDueCopyWith on AdminOverviewPastDue {
  /// Returns a callable class that can be used as follows: `instanceOfAdminOverviewPastDue.copyWith(...)` or like so:`instanceOfAdminOverviewPastDue.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminOverviewPastDueCWProxy get copyWith =>
      _$AdminOverviewPastDueCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminOverviewPastDue _$AdminOverviewPastDueFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('AdminOverviewPastDue', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['count', 'readOnlySoon']);
  final val = AdminOverviewPastDue(
    count: $checkedConvert('count', (v) => (v as num).toInt()),
    readOnlySoon: $checkedConvert('readOnlySoon', (v) => (v as num).toInt()),
  );
  return val;
});

Map<String, dynamic> _$AdminOverviewPastDueToJson(
  AdminOverviewPastDue instance,
) => <String, dynamic>{
  'count': instance.count,
  'readOnlySoon': instance.readOnlySoon,
};
