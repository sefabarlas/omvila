// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'me_appointment_review.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$MeAppointmentReviewCWProxy {
  MeAppointmentReview rating(int rating);

  MeAppointmentReview createdUtc(DateTime createdUtc);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeAppointmentReview(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeAppointmentReview(...).copyWith(id: 12, name: "My name")
  /// ````
  MeAppointmentReview call({int rating, DateTime createdUtc});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfMeAppointmentReview.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfMeAppointmentReview.copyWith.fieldName(...)`
class _$MeAppointmentReviewCWProxyImpl implements _$MeAppointmentReviewCWProxy {
  const _$MeAppointmentReviewCWProxyImpl(this._value);

  final MeAppointmentReview _value;

  @override
  MeAppointmentReview rating(int rating) => this(rating: rating);

  @override
  MeAppointmentReview createdUtc(DateTime createdUtc) =>
      this(createdUtc: createdUtc);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeAppointmentReview(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeAppointmentReview(...).copyWith(id: 12, name: "My name")
  /// ````
  MeAppointmentReview call({
    Object? rating = const $CopyWithPlaceholder(),
    Object? createdUtc = const $CopyWithPlaceholder(),
  }) {
    return MeAppointmentReview(
      rating: rating == const $CopyWithPlaceholder()
          ? _value.rating
          // ignore: cast_nullable_to_non_nullable
          : rating as int,
      createdUtc: createdUtc == const $CopyWithPlaceholder()
          ? _value.createdUtc
          // ignore: cast_nullable_to_non_nullable
          : createdUtc as DateTime,
    );
  }
}

extension $MeAppointmentReviewCopyWith on MeAppointmentReview {
  /// Returns a callable class that can be used as follows: `instanceOfMeAppointmentReview.copyWith(...)` or like so:`instanceOfMeAppointmentReview.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$MeAppointmentReviewCWProxy get copyWith =>
      _$MeAppointmentReviewCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MeAppointmentReview _$MeAppointmentReviewFromJson(Map<String, dynamic> json) =>
    $checkedCreate('MeAppointmentReview', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['rating', 'createdUtc']);
      final val = MeAppointmentReview(
        rating: $checkedConvert('rating', (v) => (v as num).toInt()),
        createdUtc: $checkedConvert(
          'createdUtc',
          (v) => DateTime.parse(v as String),
        ),
      );
      return val;
    });

Map<String, dynamic> _$MeAppointmentReviewToJson(
  MeAppointmentReview instance,
) => <String, dynamic>{
  'rating': instance.rating,
  'createdUtc': instance.createdUtc.toIso8601String(),
};
