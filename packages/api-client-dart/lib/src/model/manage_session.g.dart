// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'manage_session.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ManageSessionCWProxy {
  ManageSession user(ManageSessionUser user);

  ManageSession brand(ManageSessionBrand brand);

  ManageSession membership(ManageSessionMembership membership);

  ManageSession permissions(List<String> permissions);

  ManageSession branches(List<BranchRef> branches);

  ManageSession subscription(SubscriptionBanner subscription);

  ManageSession smsCredits(ManageSessionSmsCredits? smsCredits);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageSession(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageSession(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageSession call({
    ManageSessionUser user,
    ManageSessionBrand brand,
    ManageSessionMembership membership,
    List<String> permissions,
    List<BranchRef> branches,
    SubscriptionBanner subscription,
    ManageSessionSmsCredits? smsCredits,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfManageSession.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfManageSession.copyWith.fieldName(...)`
class _$ManageSessionCWProxyImpl implements _$ManageSessionCWProxy {
  const _$ManageSessionCWProxyImpl(this._value);

  final ManageSession _value;

  @override
  ManageSession user(ManageSessionUser user) => this(user: user);

  @override
  ManageSession brand(ManageSessionBrand brand) => this(brand: brand);

  @override
  ManageSession membership(ManageSessionMembership membership) =>
      this(membership: membership);

  @override
  ManageSession permissions(List<String> permissions) =>
      this(permissions: permissions);

  @override
  ManageSession branches(List<BranchRef> branches) => this(branches: branches);

  @override
  ManageSession subscription(SubscriptionBanner subscription) =>
      this(subscription: subscription);

  @override
  ManageSession smsCredits(ManageSessionSmsCredits? smsCredits) =>
      this(smsCredits: smsCredits);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageSession(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageSession(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageSession call({
    Object? user = const $CopyWithPlaceholder(),
    Object? brand = const $CopyWithPlaceholder(),
    Object? membership = const $CopyWithPlaceholder(),
    Object? permissions = const $CopyWithPlaceholder(),
    Object? branches = const $CopyWithPlaceholder(),
    Object? subscription = const $CopyWithPlaceholder(),
    Object? smsCredits = const $CopyWithPlaceholder(),
  }) {
    return ManageSession(
      user: user == const $CopyWithPlaceholder()
          ? _value.user
          // ignore: cast_nullable_to_non_nullable
          : user as ManageSessionUser,
      brand: brand == const $CopyWithPlaceholder()
          ? _value.brand
          // ignore: cast_nullable_to_non_nullable
          : brand as ManageSessionBrand,
      membership: membership == const $CopyWithPlaceholder()
          ? _value.membership
          // ignore: cast_nullable_to_non_nullable
          : membership as ManageSessionMembership,
      permissions: permissions == const $CopyWithPlaceholder()
          ? _value.permissions
          // ignore: cast_nullable_to_non_nullable
          : permissions as List<String>,
      branches: branches == const $CopyWithPlaceholder()
          ? _value.branches
          // ignore: cast_nullable_to_non_nullable
          : branches as List<BranchRef>,
      subscription: subscription == const $CopyWithPlaceholder()
          ? _value.subscription
          // ignore: cast_nullable_to_non_nullable
          : subscription as SubscriptionBanner,
      smsCredits: smsCredits == const $CopyWithPlaceholder()
          ? _value.smsCredits
          // ignore: cast_nullable_to_non_nullable
          : smsCredits as ManageSessionSmsCredits?,
    );
  }
}

extension $ManageSessionCopyWith on ManageSession {
  /// Returns a callable class that can be used as follows: `instanceOfManageSession.copyWith(...)` or like so:`instanceOfManageSession.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ManageSessionCWProxy get copyWith => _$ManageSessionCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ManageSession _$ManageSessionFromJson(Map<String, dynamic> json) =>
    $checkedCreate('ManageSession', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'user',
          'brand',
          'membership',
          'permissions',
          'branches',
          'subscription',
        ],
      );
      final val = ManageSession(
        user: $checkedConvert(
          'user',
          (v) => ManageSessionUser.fromJson(v as Map<String, dynamic>),
        ),
        brand: $checkedConvert(
          'brand',
          (v) => ManageSessionBrand.fromJson(v as Map<String, dynamic>),
        ),
        membership: $checkedConvert(
          'membership',
          (v) => ManageSessionMembership.fromJson(v as Map<String, dynamic>),
        ),
        permissions: $checkedConvert(
          'permissions',
          (v) => (v as List<dynamic>).map((e) => e as String).toList(),
        ),
        branches: $checkedConvert(
          'branches',
          (v) => (v as List<dynamic>)
              .map((e) => BranchRef.fromJson(e as Map<String, dynamic>))
              .toList(),
        ),
        subscription: $checkedConvert(
          'subscription',
          (v) => SubscriptionBanner.fromJson(v as Map<String, dynamic>),
        ),
        smsCredits: $checkedConvert(
          'smsCredits',
          (v) => v == null
              ? null
              : ManageSessionSmsCredits.fromJson(v as Map<String, dynamic>),
        ),
      );
      return val;
    });

Map<String, dynamic> _$ManageSessionToJson(ManageSession instance) =>
    <String, dynamic>{
      'user': instance.user.toJson(),
      'brand': instance.brand.toJson(),
      'membership': instance.membership.toJson(),
      'permissions': instance.permissions,
      'branches': instance.branches.map((e) => e.toJson()).toList(),
      'subscription': instance.subscription.toJson(),
      'smsCredits': ?instance.smsCredits?.toJson(),
    };
