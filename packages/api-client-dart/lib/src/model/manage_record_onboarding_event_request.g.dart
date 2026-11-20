// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'manage_record_onboarding_event_request.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ManageRecordOnboardingEventRequestCWProxy {
  ManageRecordOnboardingEventRequest event(
    ManageRecordOnboardingEventRequestEventEnum event,
  );

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageRecordOnboardingEventRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageRecordOnboardingEventRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageRecordOnboardingEventRequest call({
    ManageRecordOnboardingEventRequestEventEnum event,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfManageRecordOnboardingEventRequest.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfManageRecordOnboardingEventRequest.copyWith.fieldName(...)`
class _$ManageRecordOnboardingEventRequestCWProxyImpl
    implements _$ManageRecordOnboardingEventRequestCWProxy {
  const _$ManageRecordOnboardingEventRequestCWProxyImpl(this._value);

  final ManageRecordOnboardingEventRequest _value;

  @override
  ManageRecordOnboardingEventRequest event(
    ManageRecordOnboardingEventRequestEventEnum event,
  ) => this(event: event);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageRecordOnboardingEventRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageRecordOnboardingEventRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageRecordOnboardingEventRequest call({
    Object? event = const $CopyWithPlaceholder(),
  }) {
    return ManageRecordOnboardingEventRequest(
      event: event == const $CopyWithPlaceholder()
          ? _value.event
          // ignore: cast_nullable_to_non_nullable
          : event as ManageRecordOnboardingEventRequestEventEnum,
    );
  }
}

extension $ManageRecordOnboardingEventRequestCopyWith
    on ManageRecordOnboardingEventRequest {
  /// Returns a callable class that can be used as follows: `instanceOfManageRecordOnboardingEventRequest.copyWith(...)` or like so:`instanceOfManageRecordOnboardingEventRequest.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ManageRecordOnboardingEventRequestCWProxy get copyWith =>
      _$ManageRecordOnboardingEventRequestCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ManageRecordOnboardingEventRequest _$ManageRecordOnboardingEventRequestFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('ManageRecordOnboardingEventRequest', json, (
  $checkedConvert,
) {
  $checkKeys(json, requiredKeys: const ['event']);
  final val = ManageRecordOnboardingEventRequest(
    event: $checkedConvert(
      'event',
      (v) => $enumDecode(
        _$ManageRecordOnboardingEventRequestEventEnumEnumMap,
        v,
        unknownValue:
            ManageRecordOnboardingEventRequestEventEnum.unknownDefaultOpenApi,
      ),
    ),
  );
  return val;
});

Map<String, dynamic> _$ManageRecordOnboardingEventRequestToJson(
  ManageRecordOnboardingEventRequest instance,
) => <String, dynamic>{
  'event':
      _$ManageRecordOnboardingEventRequestEventEnumEnumMap[instance.event]!,
};

const _$ManageRecordOnboardingEventRequestEventEnumEnumMap = {
  ManageRecordOnboardingEventRequestEventEnum.bookingLinkCopied:
      'BookingLinkCopied',
  ManageRecordOnboardingEventRequestEventEnum.qrDownloaded: 'QrDownloaded',
  ManageRecordOnboardingEventRequestEventEnum.storyImageDownloaded:
      'StoryImageDownloaded',
  ManageRecordOnboardingEventRequestEventEnum.soloConfirmed: 'SoloConfirmed',
  ManageRecordOnboardingEventRequestEventEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};
