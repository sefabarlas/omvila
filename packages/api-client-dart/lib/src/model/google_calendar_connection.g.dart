// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'google_calendar_connection.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$GoogleCalendarConnectionCWProxy {
  GoogleCalendarConnection status(GoogleCalendarConnectionStatusEnum status);

  GoogleCalendarConnection accountEmailMasked(String? accountEmailMasked);

  GoogleCalendarConnection connectedUtc(DateTime? connectedUtc);

  GoogleCalendarConnection lastSyncedUtc(DateTime? lastSyncedUtc);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `GoogleCalendarConnection(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// GoogleCalendarConnection(...).copyWith(id: 12, name: "My name")
  /// ````
  GoogleCalendarConnection call({
    GoogleCalendarConnectionStatusEnum status,
    String? accountEmailMasked,
    DateTime? connectedUtc,
    DateTime? lastSyncedUtc,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfGoogleCalendarConnection.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfGoogleCalendarConnection.copyWith.fieldName(...)`
class _$GoogleCalendarConnectionCWProxyImpl
    implements _$GoogleCalendarConnectionCWProxy {
  const _$GoogleCalendarConnectionCWProxyImpl(this._value);

  final GoogleCalendarConnection _value;

  @override
  GoogleCalendarConnection status(GoogleCalendarConnectionStatusEnum status) =>
      this(status: status);

  @override
  GoogleCalendarConnection accountEmailMasked(String? accountEmailMasked) =>
      this(accountEmailMasked: accountEmailMasked);

  @override
  GoogleCalendarConnection connectedUtc(DateTime? connectedUtc) =>
      this(connectedUtc: connectedUtc);

  @override
  GoogleCalendarConnection lastSyncedUtc(DateTime? lastSyncedUtc) =>
      this(lastSyncedUtc: lastSyncedUtc);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `GoogleCalendarConnection(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// GoogleCalendarConnection(...).copyWith(id: 12, name: "My name")
  /// ````
  GoogleCalendarConnection call({
    Object? status = const $CopyWithPlaceholder(),
    Object? accountEmailMasked = const $CopyWithPlaceholder(),
    Object? connectedUtc = const $CopyWithPlaceholder(),
    Object? lastSyncedUtc = const $CopyWithPlaceholder(),
  }) {
    return GoogleCalendarConnection(
      status: status == const $CopyWithPlaceholder()
          ? _value.status
          // ignore: cast_nullable_to_non_nullable
          : status as GoogleCalendarConnectionStatusEnum,
      accountEmailMasked: accountEmailMasked == const $CopyWithPlaceholder()
          ? _value.accountEmailMasked
          // ignore: cast_nullable_to_non_nullable
          : accountEmailMasked as String?,
      connectedUtc: connectedUtc == const $CopyWithPlaceholder()
          ? _value.connectedUtc
          // ignore: cast_nullable_to_non_nullable
          : connectedUtc as DateTime?,
      lastSyncedUtc: lastSyncedUtc == const $CopyWithPlaceholder()
          ? _value.lastSyncedUtc
          // ignore: cast_nullable_to_non_nullable
          : lastSyncedUtc as DateTime?,
    );
  }
}

extension $GoogleCalendarConnectionCopyWith on GoogleCalendarConnection {
  /// Returns a callable class that can be used as follows: `instanceOfGoogleCalendarConnection.copyWith(...)` or like so:`instanceOfGoogleCalendarConnection.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$GoogleCalendarConnectionCWProxy get copyWith =>
      _$GoogleCalendarConnectionCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

GoogleCalendarConnection _$GoogleCalendarConnectionFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('GoogleCalendarConnection', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['status']);
  final val = GoogleCalendarConnection(
    status: $checkedConvert(
      'status',
      (v) => $enumDecode(
        _$GoogleCalendarConnectionStatusEnumEnumMap,
        v,
        unknownValue: GoogleCalendarConnectionStatusEnum.unknownDefaultOpenApi,
      ),
    ),
    accountEmailMasked: $checkedConvert(
      'accountEmailMasked',
      (v) => v as String?,
    ),
    connectedUtc: $checkedConvert(
      'connectedUtc',
      (v) => v == null ? null : DateTime.parse(v as String),
    ),
    lastSyncedUtc: $checkedConvert(
      'lastSyncedUtc',
      (v) => v == null ? null : DateTime.parse(v as String),
    ),
  );
  return val;
});

Map<String, dynamic> _$GoogleCalendarConnectionToJson(
  GoogleCalendarConnection instance,
) => <String, dynamic>{
  'status': _$GoogleCalendarConnectionStatusEnumEnumMap[instance.status]!,
  'accountEmailMasked': ?instance.accountEmailMasked,
  'connectedUtc': ?instance.connectedUtc?.toIso8601String(),
  'lastSyncedUtc': ?instance.lastSyncedUtc?.toIso8601String(),
};

const _$GoogleCalendarConnectionStatusEnumEnumMap = {
  GoogleCalendarConnectionStatusEnum.notConnected: 'NotConnected',
  GoogleCalendarConnectionStatusEnum.active: 'Active',
  GoogleCalendarConnectionStatusEnum.tokenExpired: 'TokenExpired',
  GoogleCalendarConnectionStatusEnum.revoked: 'Revoked',
  GoogleCalendarConnectionStatusEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};
