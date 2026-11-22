// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'google_calendar_callback_request.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$GoogleCalendarCallbackRequestCWProxy {
  GoogleCalendarCallbackRequest code(String code);

  GoogleCalendarCallbackRequest state(String state);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `GoogleCalendarCallbackRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// GoogleCalendarCallbackRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  GoogleCalendarCallbackRequest call({String code, String state});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfGoogleCalendarCallbackRequest.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfGoogleCalendarCallbackRequest.copyWith.fieldName(...)`
class _$GoogleCalendarCallbackRequestCWProxyImpl
    implements _$GoogleCalendarCallbackRequestCWProxy {
  const _$GoogleCalendarCallbackRequestCWProxyImpl(this._value);

  final GoogleCalendarCallbackRequest _value;

  @override
  GoogleCalendarCallbackRequest code(String code) => this(code: code);

  @override
  GoogleCalendarCallbackRequest state(String state) => this(state: state);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `GoogleCalendarCallbackRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// GoogleCalendarCallbackRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  GoogleCalendarCallbackRequest call({
    Object? code = const $CopyWithPlaceholder(),
    Object? state = const $CopyWithPlaceholder(),
  }) {
    return GoogleCalendarCallbackRequest(
      code: code == const $CopyWithPlaceholder()
          ? _value.code
          // ignore: cast_nullable_to_non_nullable
          : code as String,
      state: state == const $CopyWithPlaceholder()
          ? _value.state
          // ignore: cast_nullable_to_non_nullable
          : state as String,
    );
  }
}

extension $GoogleCalendarCallbackRequestCopyWith
    on GoogleCalendarCallbackRequest {
  /// Returns a callable class that can be used as follows: `instanceOfGoogleCalendarCallbackRequest.copyWith(...)` or like so:`instanceOfGoogleCalendarCallbackRequest.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$GoogleCalendarCallbackRequestCWProxy get copyWith =>
      _$GoogleCalendarCallbackRequestCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

GoogleCalendarCallbackRequest _$GoogleCalendarCallbackRequestFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('GoogleCalendarCallbackRequest', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['code', 'state']);
  final val = GoogleCalendarCallbackRequest(
    code: $checkedConvert('code', (v) => v as String),
    state: $checkedConvert('state', (v) => v as String),
  );
  return val;
});

Map<String, dynamic> _$GoogleCalendarCallbackRequestToJson(
  GoogleCalendarCallbackRequest instance,
) => <String, dynamic>{'code': instance.code, 'state': instance.state};
