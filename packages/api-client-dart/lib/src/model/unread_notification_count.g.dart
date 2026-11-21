// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'unread_notification_count.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$UnreadNotificationCountCWProxy {
  UnreadNotificationCount unreadCount(int unreadCount);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `UnreadNotificationCount(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// UnreadNotificationCount(...).copyWith(id: 12, name: "My name")
  /// ````
  UnreadNotificationCount call({int unreadCount});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfUnreadNotificationCount.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfUnreadNotificationCount.copyWith.fieldName(...)`
class _$UnreadNotificationCountCWProxyImpl
    implements _$UnreadNotificationCountCWProxy {
  const _$UnreadNotificationCountCWProxyImpl(this._value);

  final UnreadNotificationCount _value;

  @override
  UnreadNotificationCount unreadCount(int unreadCount) =>
      this(unreadCount: unreadCount);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `UnreadNotificationCount(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// UnreadNotificationCount(...).copyWith(id: 12, name: "My name")
  /// ````
  UnreadNotificationCount call({
    Object? unreadCount = const $CopyWithPlaceholder(),
  }) {
    return UnreadNotificationCount(
      unreadCount: unreadCount == const $CopyWithPlaceholder()
          ? _value.unreadCount
          // ignore: cast_nullable_to_non_nullable
          : unreadCount as int,
    );
  }
}

extension $UnreadNotificationCountCopyWith on UnreadNotificationCount {
  /// Returns a callable class that can be used as follows: `instanceOfUnreadNotificationCount.copyWith(...)` or like so:`instanceOfUnreadNotificationCount.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$UnreadNotificationCountCWProxy get copyWith =>
      _$UnreadNotificationCountCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

UnreadNotificationCount _$UnreadNotificationCountFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('UnreadNotificationCount', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['unreadCount']);
  final val = UnreadNotificationCount(
    unreadCount: $checkedConvert('unreadCount', (v) => (v as num).toInt()),
  );
  return val;
});

Map<String, dynamic> _$UnreadNotificationCountToJson(
  UnreadNotificationCount instance,
) => <String, dynamic>{'unreadCount': instance.unreadCount};
