// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'me_review.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$MeReviewCWProxy {
  MeReview id(String id);

  MeReview appointmentId(String appointmentId);

  MeReview rating(int rating);

  MeReview comment(String? comment);

  MeReview createdUtc(DateTime createdUtc);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeReview(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeReview(...).copyWith(id: 12, name: "My name")
  /// ````
  MeReview call({
    String id,
    String appointmentId,
    int rating,
    String? comment,
    DateTime createdUtc,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfMeReview.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfMeReview.copyWith.fieldName(...)`
class _$MeReviewCWProxyImpl implements _$MeReviewCWProxy {
  const _$MeReviewCWProxyImpl(this._value);

  final MeReview _value;

  @override
  MeReview id(String id) => this(id: id);

  @override
  MeReview appointmentId(String appointmentId) =>
      this(appointmentId: appointmentId);

  @override
  MeReview rating(int rating) => this(rating: rating);

  @override
  MeReview comment(String? comment) => this(comment: comment);

  @override
  MeReview createdUtc(DateTime createdUtc) => this(createdUtc: createdUtc);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeReview(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeReview(...).copyWith(id: 12, name: "My name")
  /// ````
  MeReview call({
    Object? id = const $CopyWithPlaceholder(),
    Object? appointmentId = const $CopyWithPlaceholder(),
    Object? rating = const $CopyWithPlaceholder(),
    Object? comment = const $CopyWithPlaceholder(),
    Object? createdUtc = const $CopyWithPlaceholder(),
  }) {
    return MeReview(
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as String,
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
      createdUtc: createdUtc == const $CopyWithPlaceholder()
          ? _value.createdUtc
          // ignore: cast_nullable_to_non_nullable
          : createdUtc as DateTime,
    );
  }
}

extension $MeReviewCopyWith on MeReview {
  /// Returns a callable class that can be used as follows: `instanceOfMeReview.copyWith(...)` or like so:`instanceOfMeReview.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$MeReviewCWProxy get copyWith => _$MeReviewCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MeReview _$MeReviewFromJson(Map<String, dynamic> json) =>
    $checkedCreate('MeReview', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'id',
          'appointmentId',
          'rating',
          'comment',
          'createdUtc',
        ],
      );
      final val = MeReview(
        id: $checkedConvert('id', (v) => v as String),
        appointmentId: $checkedConvert('appointmentId', (v) => v as String),
        rating: $checkedConvert('rating', (v) => (v as num).toInt()),
        comment: $checkedConvert('comment', (v) => v as String?),
        createdUtc: $checkedConvert(
          'createdUtc',
          (v) => DateTime.parse(v as String),
        ),
      );
      return val;
    });

Map<String, dynamic> _$MeReviewToJson(MeReview instance) => <String, dynamic>{
  'id': instance.id,
  'appointmentId': instance.appointmentId,
  'rating': instance.rating,
  'comment': instance.comment,
  'createdUtc': instance.createdUtc.toIso8601String(),
};
