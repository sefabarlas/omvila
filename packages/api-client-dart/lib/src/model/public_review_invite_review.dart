//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'public_review_invite_review.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PublicReviewInviteReview {
  /// Returns a new [PublicReviewInviteReview] instance.
  PublicReviewInviteReview({

    required  this.rating,

     this.comment,

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
    
    name: r'comment',
    required: false,
    includeIfNull: false,
  )


  final String? comment;



  @JsonKey(
    
    name: r'createdUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime createdUtc;





    @override
    bool operator ==(Object other) => identical(this, other) || other is PublicReviewInviteReview &&
      other.rating == rating &&
      other.comment == comment &&
      other.createdUtc == createdUtc;

    @override
    int get hashCode =>
        rating.hashCode +
        (comment == null ? 0 : comment.hashCode) +
        createdUtc.hashCode;

  factory PublicReviewInviteReview.fromJson(Map<String, dynamic> json) => _$PublicReviewInviteReviewFromJson(json);

  Map<String, dynamic> toJson() => _$PublicReviewInviteReviewToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

