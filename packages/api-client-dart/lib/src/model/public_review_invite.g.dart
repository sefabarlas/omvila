// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'public_review_invite.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PublicReviewInviteCWProxy {
  PublicReviewInvite brand(PublicReviewInviteBrand brand);

  PublicReviewInvite branchName(String branchName);

  PublicReviewInvite appointmentStartUtc(DateTime appointmentStartUtc);

  PublicReviewInvite timeZone(String timeZone);

  PublicReviewInvite serviceNames(List<String> serviceNames);

  PublicReviewInvite staff(PublicReviewInviteStaff? staff);

  PublicReviewInvite customerDisplayName(String customerDisplayName);

  PublicReviewInvite alreadyReviewed(bool alreadyReviewed);

  PublicReviewInvite review(PublicReviewInviteReview? review);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicReviewInvite(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicReviewInvite(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicReviewInvite call({
    PublicReviewInviteBrand brand,
    String branchName,
    DateTime appointmentStartUtc,
    String timeZone,
    List<String> serviceNames,
    PublicReviewInviteStaff? staff,
    String customerDisplayName,
    bool alreadyReviewed,
    PublicReviewInviteReview? review,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPublicReviewInvite.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPublicReviewInvite.copyWith.fieldName(...)`
class _$PublicReviewInviteCWProxyImpl implements _$PublicReviewInviteCWProxy {
  const _$PublicReviewInviteCWProxyImpl(this._value);

  final PublicReviewInvite _value;

  @override
  PublicReviewInvite brand(PublicReviewInviteBrand brand) => this(brand: brand);

  @override
  PublicReviewInvite branchName(String branchName) =>
      this(branchName: branchName);

  @override
  PublicReviewInvite appointmentStartUtc(DateTime appointmentStartUtc) =>
      this(appointmentStartUtc: appointmentStartUtc);

  @override
  PublicReviewInvite timeZone(String timeZone) => this(timeZone: timeZone);

  @override
  PublicReviewInvite serviceNames(List<String> serviceNames) =>
      this(serviceNames: serviceNames);

  @override
  PublicReviewInvite staff(PublicReviewInviteStaff? staff) =>
      this(staff: staff);

  @override
  PublicReviewInvite customerDisplayName(String customerDisplayName) =>
      this(customerDisplayName: customerDisplayName);

  @override
  PublicReviewInvite alreadyReviewed(bool alreadyReviewed) =>
      this(alreadyReviewed: alreadyReviewed);

  @override
  PublicReviewInvite review(PublicReviewInviteReview? review) =>
      this(review: review);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicReviewInvite(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicReviewInvite(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicReviewInvite call({
    Object? brand = const $CopyWithPlaceholder(),
    Object? branchName = const $CopyWithPlaceholder(),
    Object? appointmentStartUtc = const $CopyWithPlaceholder(),
    Object? timeZone = const $CopyWithPlaceholder(),
    Object? serviceNames = const $CopyWithPlaceholder(),
    Object? staff = const $CopyWithPlaceholder(),
    Object? customerDisplayName = const $CopyWithPlaceholder(),
    Object? alreadyReviewed = const $CopyWithPlaceholder(),
    Object? review = const $CopyWithPlaceholder(),
  }) {
    return PublicReviewInvite(
      brand: brand == const $CopyWithPlaceholder()
          ? _value.brand
          // ignore: cast_nullable_to_non_nullable
          : brand as PublicReviewInviteBrand,
      branchName: branchName == const $CopyWithPlaceholder()
          ? _value.branchName
          // ignore: cast_nullable_to_non_nullable
          : branchName as String,
      appointmentStartUtc: appointmentStartUtc == const $CopyWithPlaceholder()
          ? _value.appointmentStartUtc
          // ignore: cast_nullable_to_non_nullable
          : appointmentStartUtc as DateTime,
      timeZone: timeZone == const $CopyWithPlaceholder()
          ? _value.timeZone
          // ignore: cast_nullable_to_non_nullable
          : timeZone as String,
      serviceNames: serviceNames == const $CopyWithPlaceholder()
          ? _value.serviceNames
          // ignore: cast_nullable_to_non_nullable
          : serviceNames as List<String>,
      staff: staff == const $CopyWithPlaceholder()
          ? _value.staff
          // ignore: cast_nullable_to_non_nullable
          : staff as PublicReviewInviteStaff?,
      customerDisplayName: customerDisplayName == const $CopyWithPlaceholder()
          ? _value.customerDisplayName
          // ignore: cast_nullable_to_non_nullable
          : customerDisplayName as String,
      alreadyReviewed: alreadyReviewed == const $CopyWithPlaceholder()
          ? _value.alreadyReviewed
          // ignore: cast_nullable_to_non_nullable
          : alreadyReviewed as bool,
      review: review == const $CopyWithPlaceholder()
          ? _value.review
          // ignore: cast_nullable_to_non_nullable
          : review as PublicReviewInviteReview?,
    );
  }
}

extension $PublicReviewInviteCopyWith on PublicReviewInvite {
  /// Returns a callable class that can be used as follows: `instanceOfPublicReviewInvite.copyWith(...)` or like so:`instanceOfPublicReviewInvite.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PublicReviewInviteCWProxy get copyWith =>
      _$PublicReviewInviteCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PublicReviewInvite _$PublicReviewInviteFromJson(Map<String, dynamic> json) =>
    $checkedCreate('PublicReviewInvite', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'brand',
          'branchName',
          'appointmentStartUtc',
          'timeZone',
          'serviceNames',
          'customerDisplayName',
          'alreadyReviewed',
        ],
      );
      final val = PublicReviewInvite(
        brand: $checkedConvert(
          'brand',
          (v) => PublicReviewInviteBrand.fromJson(v as Map<String, dynamic>),
        ),
        branchName: $checkedConvert('branchName', (v) => v as String),
        appointmentStartUtc: $checkedConvert(
          'appointmentStartUtc',
          (v) => DateTime.parse(v as String),
        ),
        timeZone: $checkedConvert('timeZone', (v) => v as String),
        serviceNames: $checkedConvert(
          'serviceNames',
          (v) => (v as List<dynamic>).map((e) => e as String).toList(),
        ),
        staff: $checkedConvert(
          'staff',
          (v) => v == null
              ? null
              : PublicReviewInviteStaff.fromJson(v as Map<String, dynamic>),
        ),
        customerDisplayName: $checkedConvert(
          'customerDisplayName',
          (v) => v as String,
        ),
        alreadyReviewed: $checkedConvert('alreadyReviewed', (v) => v as bool),
        review: $checkedConvert(
          'review',
          (v) => v == null
              ? null
              : PublicReviewInviteReview.fromJson(v as Map<String, dynamic>),
        ),
      );
      return val;
    });

Map<String, dynamic> _$PublicReviewInviteToJson(PublicReviewInvite instance) =>
    <String, dynamic>{
      'brand': instance.brand.toJson(),
      'branchName': instance.branchName,
      'appointmentStartUtc': instance.appointmentStartUtc.toIso8601String(),
      'timeZone': instance.timeZone,
      'serviceNames': instance.serviceNames,
      'staff': ?instance.staff?.toJson(),
      'customerDisplayName': instance.customerDisplayName,
      'alreadyReviewed': instance.alreadyReviewed,
      'review': ?instance.review?.toJson(),
    };
