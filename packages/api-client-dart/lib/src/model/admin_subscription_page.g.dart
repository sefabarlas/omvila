// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_subscription_page.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminSubscriptionPageCWProxy {
  AdminSubscriptionPage counts(AdminSubscriptionPageCounts counts);

  AdminSubscriptionPage items(List<AdminSubscriptionListItem> items);

  AdminSubscriptionPage nextCursor(String? nextCursor);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminSubscriptionPage(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminSubscriptionPage(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminSubscriptionPage call({
    AdminSubscriptionPageCounts counts,
    List<AdminSubscriptionListItem> items,
    String? nextCursor,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminSubscriptionPage.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminSubscriptionPage.copyWith.fieldName(...)`
class _$AdminSubscriptionPageCWProxyImpl
    implements _$AdminSubscriptionPageCWProxy {
  const _$AdminSubscriptionPageCWProxyImpl(this._value);

  final AdminSubscriptionPage _value;

  @override
  AdminSubscriptionPage counts(AdminSubscriptionPageCounts counts) =>
      this(counts: counts);

  @override
  AdminSubscriptionPage items(List<AdminSubscriptionListItem> items) =>
      this(items: items);

  @override
  AdminSubscriptionPage nextCursor(String? nextCursor) =>
      this(nextCursor: nextCursor);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminSubscriptionPage(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminSubscriptionPage(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminSubscriptionPage call({
    Object? counts = const $CopyWithPlaceholder(),
    Object? items = const $CopyWithPlaceholder(),
    Object? nextCursor = const $CopyWithPlaceholder(),
  }) {
    return AdminSubscriptionPage(
      counts: counts == const $CopyWithPlaceholder()
          ? _value.counts
          // ignore: cast_nullable_to_non_nullable
          : counts as AdminSubscriptionPageCounts,
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<AdminSubscriptionListItem>,
      nextCursor: nextCursor == const $CopyWithPlaceholder()
          ? _value.nextCursor
          // ignore: cast_nullable_to_non_nullable
          : nextCursor as String?,
    );
  }
}

extension $AdminSubscriptionPageCopyWith on AdminSubscriptionPage {
  /// Returns a callable class that can be used as follows: `instanceOfAdminSubscriptionPage.copyWith(...)` or like so:`instanceOfAdminSubscriptionPage.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminSubscriptionPageCWProxy get copyWith =>
      _$AdminSubscriptionPageCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminSubscriptionPage _$AdminSubscriptionPageFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('AdminSubscriptionPage', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['counts', 'items', 'nextCursor']);
  final val = AdminSubscriptionPage(
    counts: $checkedConvert(
      'counts',
      (v) => AdminSubscriptionPageCounts.fromJson(v as Map<String, dynamic>),
    ),
    items: $checkedConvert(
      'items',
      (v) => (v as List<dynamic>)
          .map(
            (e) =>
                AdminSubscriptionListItem.fromJson(e as Map<String, dynamic>),
          )
          .toList(),
    ),
    nextCursor: $checkedConvert('nextCursor', (v) => v as String?),
  );
  return val;
});

Map<String, dynamic> _$AdminSubscriptionPageToJson(
  AdminSubscriptionPage instance,
) => <String, dynamic>{
  'counts': instance.counts.toJson(),
  'items': instance.items.map((e) => e.toJson()).toList(),
  'nextCursor': instance.nextCursor,
};
