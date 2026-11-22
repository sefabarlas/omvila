// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_subscription_page_counts.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminSubscriptionPageCountsCWProxy {
  AdminSubscriptionPageCounts active(int active);

  AdminSubscriptionPageCounts trialing(int trialing);

  AdminSubscriptionPageCounts trialEndingThisWeek(int trialEndingThisWeek);

  AdminSubscriptionPageCounts pastDue(int pastDue);

  AdminSubscriptionPageCounts readOnly(int readOnly);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminSubscriptionPageCounts(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminSubscriptionPageCounts(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminSubscriptionPageCounts call({
    int active,
    int trialing,
    int trialEndingThisWeek,
    int pastDue,
    int readOnly,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminSubscriptionPageCounts.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminSubscriptionPageCounts.copyWith.fieldName(...)`
class _$AdminSubscriptionPageCountsCWProxyImpl
    implements _$AdminSubscriptionPageCountsCWProxy {
  const _$AdminSubscriptionPageCountsCWProxyImpl(this._value);

  final AdminSubscriptionPageCounts _value;

  @override
  AdminSubscriptionPageCounts active(int active) => this(active: active);

  @override
  AdminSubscriptionPageCounts trialing(int trialing) =>
      this(trialing: trialing);

  @override
  AdminSubscriptionPageCounts trialEndingThisWeek(int trialEndingThisWeek) =>
      this(trialEndingThisWeek: trialEndingThisWeek);

  @override
  AdminSubscriptionPageCounts pastDue(int pastDue) => this(pastDue: pastDue);

  @override
  AdminSubscriptionPageCounts readOnly(int readOnly) =>
      this(readOnly: readOnly);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminSubscriptionPageCounts(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminSubscriptionPageCounts(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminSubscriptionPageCounts call({
    Object? active = const $CopyWithPlaceholder(),
    Object? trialing = const $CopyWithPlaceholder(),
    Object? trialEndingThisWeek = const $CopyWithPlaceholder(),
    Object? pastDue = const $CopyWithPlaceholder(),
    Object? readOnly = const $CopyWithPlaceholder(),
  }) {
    return AdminSubscriptionPageCounts(
      active: active == const $CopyWithPlaceholder()
          ? _value.active
          // ignore: cast_nullable_to_non_nullable
          : active as int,
      trialing: trialing == const $CopyWithPlaceholder()
          ? _value.trialing
          // ignore: cast_nullable_to_non_nullable
          : trialing as int,
      trialEndingThisWeek: trialEndingThisWeek == const $CopyWithPlaceholder()
          ? _value.trialEndingThisWeek
          // ignore: cast_nullable_to_non_nullable
          : trialEndingThisWeek as int,
      pastDue: pastDue == const $CopyWithPlaceholder()
          ? _value.pastDue
          // ignore: cast_nullable_to_non_nullable
          : pastDue as int,
      readOnly: readOnly == const $CopyWithPlaceholder()
          ? _value.readOnly
          // ignore: cast_nullable_to_non_nullable
          : readOnly as int,
    );
  }
}

extension $AdminSubscriptionPageCountsCopyWith on AdminSubscriptionPageCounts {
  /// Returns a callable class that can be used as follows: `instanceOfAdminSubscriptionPageCounts.copyWith(...)` or like so:`instanceOfAdminSubscriptionPageCounts.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminSubscriptionPageCountsCWProxy get copyWith =>
      _$AdminSubscriptionPageCountsCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminSubscriptionPageCounts _$AdminSubscriptionPageCountsFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('AdminSubscriptionPageCounts', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const [
      'active',
      'trialing',
      'trialEndingThisWeek',
      'pastDue',
      'readOnly',
    ],
  );
  final val = AdminSubscriptionPageCounts(
    active: $checkedConvert('active', (v) => (v as num).toInt()),
    trialing: $checkedConvert('trialing', (v) => (v as num).toInt()),
    trialEndingThisWeek: $checkedConvert(
      'trialEndingThisWeek',
      (v) => (v as num).toInt(),
    ),
    pastDue: $checkedConvert('pastDue', (v) => (v as num).toInt()),
    readOnly: $checkedConvert('readOnly', (v) => (v as num).toInt()),
  );
  return val;
});

Map<String, dynamic> _$AdminSubscriptionPageCountsToJson(
  AdminSubscriptionPageCounts instance,
) => <String, dynamic>{
  'active': instance.active,
  'trialing': instance.trialing,
  'trialEndingThisWeek': instance.trialEndingThisWeek,
  'pastDue': instance.pastDue,
  'readOnly': instance.readOnly,
};
