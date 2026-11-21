// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'public_review_invite_review.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PublicReviewInviteReviewCWProxy {
  PublicReviewInviteReview rating(int rating);

  PublicReviewInviteReview comment(String? comment);

  PublicReviewInviteReview createdUtc(DateTime createdUtc);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicReviewInviteReview(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicReviewInviteReview(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicReviewInviteReview call({
    int rating,
    String? comment,
    DateTime createdUtc,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPublicReviewInviteReview.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPublicReviewInviteReview.copyWith.fieldName(...)`
class _$PublicReviewInviteReviewCWProxyImpl
    implements _$PublicReviewInviteReviewCWProxy {
  const _$PublicReviewInviteReviewCWProxyImpl(this._value);

  final PublicReviewInviteReview _value;

  @override
  PublicReviewInviteReview rating(int rating) => this(rating: rating);

  @override
  PublicReviewInviteReview comment(String? comment) => this(comment: comment);

  @override
  PublicReviewInviteReview createdUtc(DateTime createdUtc) =>
      this(createdUtc: createdUtc);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicReviewInviteReview(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicReviewInviteReview(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicReviewInviteReview call({
    Object? rating = const $CopyWithPlaceholder(),
    Object? comment = const $CopyWithPlaceholder(),
    Object? createdUtc = const $CopyWithPlaceholder(),
  }) {
    return PublicReviewInviteReview(
      rating: rating == const $CopyWithPlaceholder()
          ? _value.rating
          // ignore: cast_nullable_to_non_nullable
          : rating as int,
      comment: comment == const $CopyWithPlaceholder()
          ? _value.comment
          // ignore: cast_nullable_to_non_nullable
          : comment as String?,
      createdUtc: createdUtc == const $CopyWithPlaceholder()
          ? _value.createdUtc
          // ignore: cast_nullable_to_non_nullable
          : createdUtc as DateTime,
    );
  }
}

extension $PublicReviewInviteReviewCopyWith on PublicReviewInviteReview {
  /// Returns a callable class that can be used as follows: `instanceOfPublicReviewInviteReview.copyWith(...)` or like so:`instanceOfPublicReviewInviteReview.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PublicReviewInviteReviewCWProxy get copyWith =>
      _$PublicReviewInviteReviewCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PublicReviewInviteReview _$PublicReviewInviteReviewFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('PublicReviewInviteReview', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['rating', 'createdUtc']);
  final val = PublicReviewInviteReview(
    rating: $checkedConvert('rating', (v) => (v as num).toInt()),
    comment: $checkedConvert('comment', (v) => v as String?),
    createdUtc: $checkedConvert(
      'createdUtc',
      (v) => DateTime.parse(v as String),
    ),
  );
  return val;
});

Map<String, dynamic> _$PublicReviewInviteReviewToJson(
  PublicReviewInviteReview instance,
) => <String, dynamic>{
  'rating': instance.rating,
  'comment': ?instance.comment,
  'createdUtc': instance.createdUtc.toIso8601String(),
};
