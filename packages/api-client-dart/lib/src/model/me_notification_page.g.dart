// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'me_notification_page.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$MeNotificationPageCWProxy {
  MeNotificationPage items(List<MeNotification> items);

  MeNotificationPage nextCursor(String? nextCursor);

  MeNotificationPage unreadCount(int unreadCount);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeNotificationPage(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeNotificationPage(...).copyWith(id: 12, name: "My name")
  /// ````
  MeNotificationPage call({
    List<MeNotification> items,
    String? nextCursor,
    int unreadCount,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfMeNotificationPage.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfMeNotificationPage.copyWith.fieldName(...)`
class _$MeNotificationPageCWProxyImpl implements _$MeNotificationPageCWProxy {
  const _$MeNotificationPageCWProxyImpl(this._value);

  final MeNotificationPage _value;

  @override
  MeNotificationPage items(List<MeNotification> items) => this(items: items);

  @override
  MeNotificationPage nextCursor(String? nextCursor) =>
      this(nextCursor: nextCursor);

  @override
  MeNotificationPage unreadCount(int unreadCount) =>
      this(unreadCount: unreadCount);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeNotificationPage(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeNotificationPage(...).copyWith(id: 12, name: "My name")
  /// ````
  MeNotificationPage call({
    Object? items = const $CopyWithPlaceholder(),
    Object? nextCursor = const $CopyWithPlaceholder(),
    Object? unreadCount = const $CopyWithPlaceholder(),
  }) {
    return MeNotificationPage(
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<MeNotification>,
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

extension $MeNotificationPageCopyWith on MeNotificationPage {
  /// Returns a callable class that can be used as follows: `instanceOfMeNotificationPage.copyWith(...)` or like so:`instanceOfMeNotificationPage.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$MeNotificationPageCWProxy get copyWith =>
      _$MeNotificationPageCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MeNotificationPage _$MeNotificationPageFromJson(Map<String, dynamic> json) =>
    $checkedCreate('MeNotificationPage', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const ['items', 'nextCursor', 'unreadCount'],
      );
      final val = MeNotificationPage(
        items: $checkedConvert(
          'items',
          (v) => (v as List<dynamic>)
              .map((e) => MeNotification.fromJson(e as Map<String, dynamic>))
              .toList(),
        ),
        nextCursor: $checkedConvert('nextCursor', (v) => v as String?),
        unreadCount: $checkedConvert('unreadCount', (v) => (v as num).toInt()),
      );
      return val;
    });

Map<String, dynamic> _$MeNotificationPageToJson(MeNotificationPage instance) =>
    <String, dynamic>{
      'items': instance.items.map((e) => e.toJson()).toList(),
      'nextCursor': instance.nextCursor,
      'unreadCount': instance.unreadCount,
    };
