// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'public_review_create.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PublicReviewCreateCWProxy {
  PublicReviewCreate rating(int rating);

  PublicReviewCreate comment(String? comment);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicReviewCreate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicReviewCreate(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicReviewCreate call({int rating, String? comment});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPublicReviewCreate.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPublicReviewCreate.copyWith.fieldName(...)`
class _$PublicReviewCreateCWProxyImpl implements _$PublicReviewCreateCWProxy {
  const _$PublicReviewCreateCWProxyImpl(this._value);

  final PublicReviewCreate _value;

  @override
  PublicReviewCreate rating(int rating) => this(rating: rating);

  @override
  PublicReviewCreate comment(String? comment) => this(comment: comment);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicReviewCreate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicReviewCreate(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicReviewCreate call({
    Object? rating = const $CopyWithPlaceholder(),
    Object? comment = const $CopyWithPlaceholder(),
  }) {
    return PublicReviewCreate(
      rating: rating == const $CopyWithPlaceholder()
          ? _value.rating
          // ignore: cast_nullable_to_non_nullable
          : rating as int,
      comment: comment == const $CopyWithPlaceholder()
          ? _value.comment
          // ignore: cast_nullable_to_non_nullable
          : comment as String?,
    );
  }
}

extension $PublicReviewCreateCopyWith on PublicReviewCreate {
  /// Returns a callable class that can be used as follows: `instanceOfPublicReviewCreate.copyWith(...)` or like so:`instanceOfPublicReviewCreate.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PublicReviewCreateCWProxy get copyWith =>
      _$PublicReviewCreateCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PublicReviewCreate _$PublicReviewCreateFromJson(Map<String, dynamic> json) =>
    $checkedCreate('PublicReviewCreate', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['rating']);
      final val = PublicReviewCreate(
        rating: $checkedConvert('rating', (v) => (v as num).toInt()),
        comment: $checkedConvert('comment', (v) => v as String?),
      );
      return val;
    });

Map<String, dynamic> _$PublicReviewCreateToJson(PublicReviewCreate instance) =>
    <String, dynamic>{'rating': instance.rating, 'comment': instance.comment};
