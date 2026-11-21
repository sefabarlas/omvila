// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'notification_target.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$NotificationTargetCWProxy {
  NotificationTarget type(NotificationTargetTypeEnum type);

  NotificationTarget id(String? id);

  NotificationTarget holdExpiresUtc(DateTime? holdExpiresUtc);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `NotificationTarget(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// NotificationTarget(...).copyWith(id: 12, name: "My name")
  /// ````
  NotificationTarget call({
    NotificationTargetTypeEnum type,
    String? id,
    DateTime? holdExpiresUtc,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfNotificationTarget.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfNotificationTarget.copyWith.fieldName(...)`
class _$NotificationTargetCWProxyImpl implements _$NotificationTargetCWProxy {
  const _$NotificationTargetCWProxyImpl(this._value);

  final NotificationTarget _value;

  @override
  NotificationTarget type(NotificationTargetTypeEnum type) => this(type: type);

  @override
  NotificationTarget id(String? id) => this(id: id);

  @override
  NotificationTarget holdExpiresUtc(DateTime? holdExpiresUtc) =>
      this(holdExpiresUtc: holdExpiresUtc);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `NotificationTarget(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// NotificationTarget(...).copyWith(id: 12, name: "My name")
  /// ````
  NotificationTarget call({
    Object? type = const $CopyWithPlaceholder(),
    Object? id = const $CopyWithPlaceholder(),
    Object? holdExpiresUtc = const $CopyWithPlaceholder(),
  }) {
    return NotificationTarget(
      type: type == const $CopyWithPlaceholder()
          ? _value.type
          // ignore: cast_nullable_to_non_nullable
          : type as NotificationTargetTypeEnum,
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as String?,
      holdExpiresUtc: holdExpiresUtc == const $CopyWithPlaceholder()
          ? _value.holdExpiresUtc
          // ignore: cast_nullable_to_non_nullable
          : holdExpiresUtc as DateTime?,
    );
  }
}

extension $NotificationTargetCopyWith on NotificationTarget {
  /// Returns a callable class that can be used as follows: `instanceOfNotificationTarget.copyWith(...)` or like so:`instanceOfNotificationTarget.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$NotificationTargetCWProxy get copyWith =>
      _$NotificationTargetCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

NotificationTarget _$NotificationTargetFromJson(Map<String, dynamic> json) =>
    $checkedCreate('NotificationTarget', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['type']);
      final val = NotificationTarget(
        type: $checkedConvert(
          'type',
          (v) => $enumDecode(
            _$NotificationTargetTypeEnumEnumMap,
            v,
            unknownValue: NotificationTargetTypeEnum.unknownDefaultOpenApi,
          ),
        ),
        id: $checkedConvert('id', (v) => v as String?),
        holdExpiresUtc: $checkedConvert(
          'holdExpiresUtc',
          (v) => v == null ? null : DateTime.parse(v as String),
        ),
      );
      return val;
    });

Map<String, dynamic> _$NotificationTargetToJson(NotificationTarget instance) =>
    <String, dynamic>{
      'type': _$NotificationTargetTypeEnumEnumMap[instance.type]!,
      'id': ?instance.id,
      'holdExpiresUtc': ?instance.holdExpiresUtc?.toIso8601String(),
    };

const _$NotificationTargetTypeEnumEnumMap = {
  NotificationTargetTypeEnum.appointment: 'Appointment',
  NotificationTargetTypeEnum.calendar: 'Calendar',
  NotificationTargetTypeEnum.today: 'Today',
  NotificationTargetTypeEnum.deposit: 'Deposit',
  NotificationTargetTypeEnum.reviews: 'Reviews',
  NotificationTargetTypeEnum.walkInQueue: 'WalkInQueue',
  NotificationTargetTypeEnum.smsCredits: 'SmsCredits',
  NotificationTargetTypeEnum.subscription: 'Subscription',
  NotificationTargetTypeEnum.unknownDefaultOpenApi: 'unknown_default_open_api',
};
