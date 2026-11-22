// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'google_calendar_connect_url.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$GoogleCalendarConnectUrlCWProxy {
  GoogleCalendarConnectUrl url(String url);

  GoogleCalendarConnectUrl state(String state);

  GoogleCalendarConnectUrl expiresUtc(DateTime expiresUtc);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `GoogleCalendarConnectUrl(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// GoogleCalendarConnectUrl(...).copyWith(id: 12, name: "My name")
  /// ````
  GoogleCalendarConnectUrl call({
    String url,
    String state,
    DateTime expiresUtc,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfGoogleCalendarConnectUrl.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfGoogleCalendarConnectUrl.copyWith.fieldName(...)`
class _$GoogleCalendarConnectUrlCWProxyImpl
    implements _$GoogleCalendarConnectUrlCWProxy {
  const _$GoogleCalendarConnectUrlCWProxyImpl(this._value);

  final GoogleCalendarConnectUrl _value;

  @override
  GoogleCalendarConnectUrl url(String url) => this(url: url);

  @override
  GoogleCalendarConnectUrl state(String state) => this(state: state);

  @override
  GoogleCalendarConnectUrl expiresUtc(DateTime expiresUtc) =>
      this(expiresUtc: expiresUtc);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `GoogleCalendarConnectUrl(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// GoogleCalendarConnectUrl(...).copyWith(id: 12, name: "My name")
  /// ````
  GoogleCalendarConnectUrl call({
    Object? url = const $CopyWithPlaceholder(),
    Object? state = const $CopyWithPlaceholder(),
    Object? expiresUtc = const $CopyWithPlaceholder(),
  }) {
    return GoogleCalendarConnectUrl(
      url: url == const $CopyWithPlaceholder()
          ? _value.url
          // ignore: cast_nullable_to_non_nullable
          : url as String,
      state: state == const $CopyWithPlaceholder()
          ? _value.state
          // ignore: cast_nullable_to_non_nullable
          : state as String,
      expiresUtc: expiresUtc == const $CopyWithPlaceholder()
          ? _value.expiresUtc
          // ignore: cast_nullable_to_non_nullable
          : expiresUtc as DateTime,
    );
  }
}

extension $GoogleCalendarConnectUrlCopyWith on GoogleCalendarConnectUrl {
  /// Returns a callable class that can be used as follows: `instanceOfGoogleCalendarConnectUrl.copyWith(...)` or like so:`instanceOfGoogleCalendarConnectUrl.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$GoogleCalendarConnectUrlCWProxy get copyWith =>
      _$GoogleCalendarConnectUrlCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

GoogleCalendarConnectUrl _$GoogleCalendarConnectUrlFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('GoogleCalendarConnectUrl', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['url', 'state', 'expiresUtc']);
  final val = GoogleCalendarConnectUrl(
    url: $checkedConvert('url', (v) => v as String),
    state: $checkedConvert('state', (v) => v as String),
    expiresUtc: $checkedConvert(
      'expiresUtc',
      (v) => DateTime.parse(v as String),
    ),
  );
  return val;
});

Map<String, dynamic> _$GoogleCalendarConnectUrlToJson(
  GoogleCalendarConnectUrl instance,
) => <String, dynamic>{
  'url': instance.url,
  'state': instance.state,
  'expiresUtc': instance.expiresUtc.toIso8601String(),
};
