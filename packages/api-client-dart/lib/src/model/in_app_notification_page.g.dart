// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'in_app_notification_page.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$InAppNotificationPageCWProxy {
  InAppNotificationPage items(List<InAppNotification> items);

  InAppNotificationPage nextCursor(String? nextCursor);

  InAppNotificationPage unreadCount(int unreadCount);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `InAppNotificationPage(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// InAppNotificationPage(...).copyWith(id: 12, name: "My name")
  /// ````
  InAppNotificationPage call({
    List<InAppNotification> items,
    String? nextCursor,
    int unreadCount,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfInAppNotificationPage.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfInAppNotificationPage.copyWith.fieldName(...)`
class _$InAppNotificationPageCWProxyImpl
    implements _$InAppNotificationPageCWProxy {
  const _$InAppNotificationPageCWProxyImpl(this._value);

  final InAppNotificationPage _value;

  @override
  InAppNotificationPage items(List<InAppNotification> items) =>
      this(items: items);

  @override
  InAppNotificationPage nextCursor(String? nextCursor) =>
      this(nextCursor: nextCursor);

  @override
  InAppNotificationPage unreadCount(int unreadCount) =>
      this(unreadCount: unreadCount);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `InAppNotificationPage(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// InAppNotificationPage(...).copyWith(id: 12, name: "My name")
  /// ````
  InAppNotificationPage call({
    Object? items = const $CopyWithPlaceholder(),
    Object? nextCursor = const $CopyWithPlaceholder(),
    Object? unreadCount = const $CopyWithPlaceholder(),
  }) {
    return InAppNotificationPage(
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<InAppNotification>,
      nextCursor: nextCursor == const $CopyWithPlaceholder()
          ? _value.nextCursor
          // ignore: cast_nullable_to_non_nullable
          : nextCursor as String?,
      unreadCount: unreadCount == const $CopyWithPlaceholder()
          ? _value.unreadCount
          // ignore: cast_nullable_to_non_nullable
          : unreadCount as int,
    );
  }
}

extension $InAppNotificationPageCopyWith on InAppNotificationPage {
  /// Returns a callable class that can be used as follows: `instanceOfInAppNotificationPage.copyWith(...)` or like so:`instanceOfInAppNotificationPage.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$InAppNotificationPageCWProxy get copyWith =>
      _$InAppNotificationPageCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

InAppNotificationPage _$InAppNotificationPageFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('InAppNotificationPage', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['items', 'nextCursor', 'unreadCount']);
  final val = InAppNotificationPage(
    items: $checkedConvert(
      'items',
      (v) => (v as List<dynamic>)
          .map((e) => InAppNotification.fromJson(e as Map<String, dynamic>))
          .toList(),
    ),
    nextCursor: $checkedConvert('nextCursor', (v) => v as String?),
    unreadCount: $checkedConvert('unreadCount', (v) => (v as num).toInt()),
  );
  return val;
});

Map<String, dynamic> _$InAppNotificationPageToJson(
  InAppNotificationPage instance,
) => <String, dynamic>{
  'items': instance.items.map((e) => e.toJson()).toList(),
  'nextCursor': instance.nextCursor,
  'unreadCount': instance.unreadCount,
};
