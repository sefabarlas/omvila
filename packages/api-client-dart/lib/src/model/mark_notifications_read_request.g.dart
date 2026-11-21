// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'mark_notifications_read_request.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$MarkNotificationsReadRequestCWProxy {
  MarkNotificationsReadRequest notificationIds(Set<String>? notificationIds);

  MarkNotificationsReadRequest all(bool? all);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MarkNotificationsReadRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MarkNotificationsReadRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  MarkNotificationsReadRequest call({Set<String>? notificationIds, bool? all});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfMarkNotificationsReadRequest.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfMarkNotificationsReadRequest.copyWith.fieldName(...)`
class _$MarkNotificationsReadRequestCWProxyImpl
    implements _$MarkNotificationsReadRequestCWProxy {
  const _$MarkNotificationsReadRequestCWProxyImpl(this._value);

  final MarkNotificationsReadRequest _value;

  @override
  MarkNotificationsReadRequest notificationIds(Set<String>? notificationIds) =>
      this(notificationIds: notificationIds);

  @override
  MarkNotificationsReadRequest all(bool? all) => this(all: all);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MarkNotificationsReadRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MarkNotificationsReadRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  MarkNotificationsReadRequest call({
    Object? notificationIds = const $CopyWithPlaceholder(),
    Object? all = const $CopyWithPlaceholder(),
  }) {
    return MarkNotificationsReadRequest(
      notificationIds: notificationIds == const $CopyWithPlaceholder()
          ? _value.notificationIds
          // ignore: cast_nullable_to_non_nullable
          : notificationIds as Set<String>?,
      all: all == const $CopyWithPlaceholder()
          ? _value.all
          // ignore: cast_nullable_to_non_nullable
          : all as bool?,
    );
  }
}

extension $MarkNotificationsReadRequestCopyWith
    on MarkNotificationsReadRequest {
  /// Returns a callable class that can be used as follows: `instanceOfMarkNotificationsReadRequest.copyWith(...)` or like so:`instanceOfMarkNotificationsReadRequest.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$MarkNotificationsReadRequestCWProxy get copyWith =>
      _$MarkNotificationsReadRequestCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MarkNotificationsReadRequest _$MarkNotificationsReadRequestFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('MarkNotificationsReadRequest', json, ($checkedConvert) {
  final val = MarkNotificationsReadRequest(
    notificationIds: $checkedConvert(
      'notificationIds',
      (v) => (v as List<dynamic>?)?.map((e) => e as String).toSet(),
    ),
    all: $checkedConvert('all', (v) => v as bool?),
  );
  return val;
});

Map<String, dynamic> _$MarkNotificationsReadRequestToJson(
  MarkNotificationsReadRequest instance,
) => <String, dynamic>{
  'notificationIds': ?instance.notificationIds?.toList(),
  'all': ?instance.all,
};
