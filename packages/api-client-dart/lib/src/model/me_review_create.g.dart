// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'me_review_create.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$MeReviewCreateCWProxy {
  MeReviewCreate appointmentId(String appointmentId);

  MeReviewCreate rating(int rating);

  MeReviewCreate comment(String? comment);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeReviewCreate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeReviewCreate(...).copyWith(id: 12, name: "My name")
  /// ````
  MeReviewCreate call({String appointmentId, int rating, String? comment});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfMeReviewCreate.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfMeReviewCreate.copyWith.fieldName(...)`
class _$MeReviewCreateCWProxyImpl implements _$MeReviewCreateCWProxy {
  const _$MeReviewCreateCWProxyImpl(this._value);

  final MeReviewCreate _value;

  @override
  MeReviewCreate appointmentId(String appointmentId) =>
      this(appointmentId: appointmentId);

  @override
  MeReviewCreate rating(int rating) => this(rating: rating);

  @override
  MeReviewCreate comment(String? comment) => this(comment: comment);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeReviewCreate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeReviewCreate(...).copyWith(id: 12, name: "My name")
  /// ````
  MeReviewCreate call({
    Object? appointmentId = const $CopyWithPlaceholder(),
    Object? rating = const $CopyWithPlaceholder(),
    Object? comment = const $CopyWithPlaceholder(),
  }) {
    return MeReviewCreate(
      appointmentId: appointmentId == const $CopyWithPlaceholder()
          ? _value.appointmentId
          // ignore: cast_nullable_to_non_nullable
          : appointmentId as String,
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

extension $MeReviewCreateCopyWith on MeReviewCreate {
  /// Returns a callable class that can be used as follows: `instanceOfMeReviewCreate.copyWith(...)` or like so:`instanceOfMeReviewCreate.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$MeReviewCreateCWProxy get copyWith => _$MeReviewCreateCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MeReviewCreate _$MeReviewCreateFromJson(Map<String, dynamic> json) =>
    $checkedCreate('MeReviewCreate', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['appointmentId', 'rating']);
      final val = MeReviewCreate(
        appointmentId: $checkedConvert('appointmentId', (v) => v as String),
        rating: $checkedConvert('rating', (v) => (v as num).toInt()),
        comment: $checkedConvert('comment', (v) => v as String?),
      );
      return val;
    });

Map<String, dynamic> _$MeReviewCreateToJson(MeReviewCreate instance) =>
    <String, dynamic>{
      'appointmentId': instance.appointmentId,
      'rating': instance.rating,
      'comment': instance.comment,
    };
