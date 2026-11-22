//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'me_appointment_review.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class MeAppointmentReview {
  /// Returns a new [MeAppointmentReview] instance.
  MeAppointmentReview({

    required  this.rating,

    required  this.createdUtc,
  });

          // minimum: 1
          // maximum: 5
  @JsonKey(
    
    name: r'rating',
    required: true,
    includeIfNull: false,
  )


  final int rating;



  @JsonKey(
    
    name: r'createdUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime createdUtc;





    @override
    bool operator ==(Object other) => identical(this, other) || other is MeAppointmentReview &&
      other.rating == rating &&
      other.createdUtc == createdUtc;

    @override
    int get hashCode =>
        rating.hashCode +
        createdUtc.hashCode;

  factory MeAppointmentReview.fromJson(Map<String, dynamic> json) => _$MeAppointmentReviewFromJson(json);

  Map<String, dynamic> toJson() => _$MeAppointmentReviewToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

