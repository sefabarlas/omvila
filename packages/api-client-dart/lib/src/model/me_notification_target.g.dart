// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'me_notification_target.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$MeNotificationTargetCWProxy {
  MeNotificationTarget type(MeNotificationTargetTypeEnum type);

  MeNotificationTarget appointmentId(String? appointmentId);

  MeNotificationTarget accessCode(String? accessCode);

  MeNotificationTarget brandSlug(String? brandSlug);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeNotificationTarget(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeNotificationTarget(...).copyWith(id: 12, name: "My name")
  /// ````
  MeNotificationTarget call({
    MeNotificationTargetTypeEnum type,
    String? appointmentId,
    String? accessCode,
    String? brandSlug,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfMeNotificationTarget.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfMeNotificationTarget.copyWith.fieldName(...)`
class _$MeNotificationTargetCWProxyImpl
    implements _$MeNotificationTargetCWProxy {
  const _$MeNotificationTargetCWProxyImpl(this._value);

  final MeNotificationTarget _value;

  @override
  MeNotificationTarget type(MeNotificationTargetTypeEnum type) =>
      this(type: type);

  @override
  MeNotificationTarget appointmentId(String? appointmentId) =>
      this(appointmentId: appointmentId);

  @override
  MeNotificationTarget accessCode(String? accessCode) =>
      this(accessCode: accessCode);

  @override
  MeNotificationTarget brandSlug(String? brandSlug) =>
      this(brandSlug: brandSlug);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeNotificationTarget(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeNotificationTarget(...).copyWith(id: 12, name: "My name")
  /// ````
  MeNotificationTarget call({
    Object? type = const $CopyWithPlaceholder(),
    Object? appointmentId = const $CopyWithPlaceholder(),
    Object? accessCode = const $CopyWithPlaceholder(),
    Object? brandSlug = const $CopyWithPlaceholder(),
  }) {
    return MeNotificationTarget(
      type: type == const $CopyWithPlaceholder()
          ? _value.type
          // ignore: cast_nullable_to_non_nullable
          : type as MeNotificationTargetTypeEnum,
      appointmentId: appointmentId == const $CopyWithPlaceholder()
          ? _value.appointmentId
          // ignore: cast_nullable_to_non_nullable
          : appointmentId as String?,
      accessCode: accessCode == const $CopyWithPlaceholder()
          ? _value.accessCode
          // ignore: cast_nullable_to_non_nullable
          : accessCode as String?,
      brandSlug: brandSlug == const $CopyWithPlaceholder()
          ? _value.brandSlug
          // ignore: cast_nullable_to_non_nullable
          : brandSlug as String?,
    );
  }
}

extension $MeNotificationTargetCopyWith on MeNotificationTarget {
  /// Returns a callable class that can be used as follows: `instanceOfMeNotificationTarget.copyWith(...)` or like so:`instanceOfMeNotificationTarget.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$MeNotificationTargetCWProxy get copyWith =>
      _$MeNotificationTargetCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MeNotificationTarget _$MeNotificationTargetFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('MeNotificationTarget', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['type']);
  final val = MeNotificationTarget(
    type: $checkedConvert(
      'type',
      (v) => $enumDecode(
        _$MeNotificationTargetTypeEnumEnumMap,
        v,
        unknownValue: MeNotificationTargetTypeEnum.unknownDefaultOpenApi,
      ),
    ),
    appointmentId: $checkedConvert('appointmentId', (v) => v as String?),
    accessCode: $checkedConvert('accessCode', (v) => v as String?),
    brandSlug: $checkedConvert('brandSlug', (v) => v as String?),
  );
  return val;
});

Map<String, dynamic> _$MeNotificationTargetToJson(
  MeNotificationTarget instance,
) => <String, dynamic>{
  'type': _$MeNotificationTargetTypeEnumEnumMap[instance.type]!,
  'appointmentId': ?instance.appointmentId,
  'accessCode': ?instance.accessCode,
  'brandSlug': ?instance.brandSlug,
};

const _$MeNotificationTargetTypeEnumEnumMap = {
  MeNotificationTargetTypeEnum.appointment: 'Appointment',
  MeNotificationTargetTypeEnum.waitlistOffer: 'WaitlistOffer',
  MeNotificationTargetTypeEnum.review: 'Review',
  MeNotificationTargetTypeEnum.booking: 'Booking',
  MeNotificationTargetTypeEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};
