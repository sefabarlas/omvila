// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'public_review_invite_staff.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PublicReviewInviteStaffCWProxy {
  PublicReviewInviteStaff name(String name);

  PublicReviewInviteStaff photoUrl(String? photoUrl);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicReviewInviteStaff(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicReviewInviteStaff(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicReviewInviteStaff call({String name, String? photoUrl});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPublicReviewInviteStaff.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPublicReviewInviteStaff.copyWith.fieldName(...)`
class _$PublicReviewInviteStaffCWProxyImpl
    implements _$PublicReviewInviteStaffCWProxy {
  const _$PublicReviewInviteStaffCWProxyImpl(this._value);

  final PublicReviewInviteStaff _value;

  @override
  PublicReviewInviteStaff name(String name) => this(name: name);

  @override
  PublicReviewInviteStaff photoUrl(String? photoUrl) =>
      this(photoUrl: photoUrl);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicReviewInviteStaff(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicReviewInviteStaff(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicReviewInviteStaff call({
    Object? name = const $CopyWithPlaceholder(),
    Object? photoUrl = const $CopyWithPlaceholder(),
  }) {
    return PublicReviewInviteStaff(
      name: name == const $CopyWithPlaceholder()
          ? _value.name
          // ignore: cast_nullable_to_non_nullable
          : name as String,
      photoUrl: photoUrl == const $CopyWithPlaceholder()
          ? _value.photoUrl
          // ignore: cast_nullable_to_non_nullable
          : photoUrl as String?,
    );
  }
}

extension $PublicReviewInviteStaffCopyWith on PublicReviewInviteStaff {
  /// Returns a callable class that can be used as follows: `instanceOfPublicReviewInviteStaff.copyWith(...)` or like so:`instanceOfPublicReviewInviteStaff.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PublicReviewInviteStaffCWProxy get copyWith =>
      _$PublicReviewInviteStaffCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PublicReviewInviteStaff _$PublicReviewInviteStaffFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('PublicReviewInviteStaff', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['name']);
  final val = PublicReviewInviteStaff(
    name: $checkedConvert('name', (v) => v as String),
    photoUrl: $checkedConvert('photoUrl', (v) => v as String?),
  );
  return val;
});

Map<String, dynamic> _$PublicReviewInviteStaffToJson(
  PublicReviewInviteStaff instance,
) => <String, dynamic>{'name': instance.name, 'photoUrl': ?instance.photoUrl};
