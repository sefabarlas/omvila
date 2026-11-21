// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'public_waitlist_join.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PublicWaitlistJoinCWProxy {
  PublicWaitlistJoin preferences(WaitlistPreferences preferences);

  PublicWaitlistJoin fullName(String fullName);

  PublicWaitlistJoin privacyNoticeAcknowledged(bool privacyNoticeAcknowledged);

  PublicWaitlistJoin privacyNoticeVersion(String privacyNoticeVersion);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicWaitlistJoin(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicWaitlistJoin(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicWaitlistJoin call({
    WaitlistPreferences preferences,
    String fullName,
    bool privacyNoticeAcknowledged,
    String privacyNoticeVersion,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPublicWaitlistJoin.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPublicWaitlistJoin.copyWith.fieldName(...)`
class _$PublicWaitlistJoinCWProxyImpl implements _$PublicWaitlistJoinCWProxy {
  const _$PublicWaitlistJoinCWProxyImpl(this._value);

  final PublicWaitlistJoin _value;

  @override
  PublicWaitlistJoin preferences(WaitlistPreferences preferences) =>
      this(preferences: preferences);

  @override
  PublicWaitlistJoin fullName(String fullName) => this(fullName: fullName);

  @override
  PublicWaitlistJoin privacyNoticeAcknowledged(
    bool privacyNoticeAcknowledged,
  ) => this(privacyNoticeAcknowledged: privacyNoticeAcknowledged);

  @override
  PublicWaitlistJoin privacyNoticeVersion(String privacyNoticeVersion) =>
      this(privacyNoticeVersion: privacyNoticeVersion);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicWaitlistJoin(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicWaitlistJoin(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicWaitlistJoin call({
    Object? preferences = const $CopyWithPlaceholder(),
    Object? fullName = const $CopyWithPlaceholder(),
    Object? privacyNoticeAcknowledged = const $CopyWithPlaceholder(),
    Object? privacyNoticeVersion = const $CopyWithPlaceholder(),
  }) {
    return PublicWaitlistJoin(
      preferences: preferences == const $CopyWithPlaceholder()
          ? _value.preferences
          // ignore: cast_nullable_to_non_nullable
          : preferences as WaitlistPreferences,
      fullName: fullName == const $CopyWithPlaceholder()
          ? _value.fullName
          // ignore: cast_nullable_to_non_nullable
          : fullName as String,
      privacyNoticeAcknowledged:
          privacyNoticeAcknowledged == const $CopyWithPlaceholder()
          ? _value.privacyNoticeAcknowledged
          // ignore: cast_nullable_to_non_nullable
          : privacyNoticeAcknowledged as bool,
      privacyNoticeVersion: privacyNoticeVersion == const $CopyWithPlaceholder()
          ? _value.privacyNoticeVersion
          // ignore: cast_nullable_to_non_nullable
          : privacyNoticeVersion as String,
    );
  }
}

extension $PublicWaitlistJoinCopyWith on PublicWaitlistJoin {
  /// Returns a callable class that can be used as follows: `instanceOfPublicWaitlistJoin.copyWith(...)` or like so:`instanceOfPublicWaitlistJoin.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PublicWaitlistJoinCWProxy get copyWith =>
      _$PublicWaitlistJoinCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PublicWaitlistJoin _$PublicWaitlistJoinFromJson(Map<String, dynamic> json) =>
    $checkedCreate('PublicWaitlistJoin', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'preferences',
          'fullName',
          'privacyNoticeAcknowledged',
          'privacyNoticeVersion',
        ],
      );
      final val = PublicWaitlistJoin(
        preferences: $checkedConvert(
          'preferences',
          (v) => WaitlistPreferences.fromJson(v as Map<String, dynamic>),
        ),
        fullName: $checkedConvert('fullName', (v) => v as String),
        privacyNoticeAcknowledged: $checkedConvert(
          'privacyNoticeAcknowledged',
          (v) => v as bool,
        ),
        privacyNoticeVersion: $checkedConvert(
          'privacyNoticeVersion',
          (v) => v as String,
        ),
      );
      return val;
    });

Map<String, dynamic> _$PublicWaitlistJoinToJson(PublicWaitlistJoin instance) =>
    <String, dynamic>{
      'preferences': instance.preferences.toJson(),
      'fullName': instance.fullName,
      'privacyNoticeAcknowledged': instance.privacyNoticeAcknowledged,
      'privacyNoticeVersion': instance.privacyNoticeVersion,
    };
