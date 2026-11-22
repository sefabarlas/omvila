//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'me_review.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class MeReview {
  /// Returns a new [MeReview] instance.
  MeReview({

    required  this.id,

    required  this.appointmentId,

    required  this.rating,

    required  this.comment,

    required  this.createdUtc,
  });

  @JsonKey(
    
    name: r'id',
    required: true,
    includeIfNull: false,
  )


  final String id;



  @JsonKey(
    
    name: r'appointmentId',
    required: true,
    includeIfNull: false,
  )


  final String appointmentId;



          // minimum: 1
          // maximum: 5
  @JsonKey(
    
    name: r'rating',
    required: true,
    includeIfNull: false,
  )


  final int rating;



  @JsonKey(
    
    name: r'comment',
    required: true,
    includeIfNull: true,
  )


  final String? comment;



  @JsonKey(
    
    name: r'createdUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime createdUtc;





    @override
    bool operator ==(Object other) => identical(this, other) || other is MeReview &&
      other.id == id &&
      other.appointmentId == appointmentId &&
      other.rating == rating &&
      other.comment == comment &&
      other.createdUtc == createdUtc;

    @override
    int get hashCode =>
        id.hashCode +
        appointmentId.hashCode +
        rating.hashCode +
        (comment == null ? 0 : comment.hashCode) +
        createdUtc.hashCode;

  factory MeReview.fromJson(Map<String, dynamic> json) => _$MeReviewFromJson(json);

  Map<String, dynamic> toJson() => _$MeReviewToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

