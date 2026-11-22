//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'me_review_create.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class MeReviewCreate {
  /// Returns a new [MeReviewCreate] instance.
  MeReviewCreate({

    required  this.appointmentId,

    required  this.rating,

     this.comment,
  });

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



      /// \"Yorum (isteğe bağlı)\"
  @JsonKey(
    
    name: r'comment',
    required: false,
    includeIfNull: false,
  )


  final String? comment;





    @override
    bool operator ==(Object other) => identical(this, other) || other is MeReviewCreate &&
      other.appointmentId == appointmentId &&
      other.rating == rating &&
      other.comment == comment;

    @override
    int get hashCode =>
        appointmentId.hashCode +
        rating.hashCode +
        (comment == null ? 0 : comment.hashCode);

  factory MeReviewCreate.fromJson(Map<String, dynamic> json) => _$MeReviewCreateFromJson(json);

  Map<String, dynamic> toJson() => _$MeReviewCreateToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

