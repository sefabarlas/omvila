//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'public_review_create.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PublicReviewCreate {
  /// Returns a new [PublicReviewCreate] instance.
  PublicReviewCreate({

    required  this.rating,

     this.comment,
  });

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
    includeIfNull: true,
  )


  final String? comment;





    @override
    bool operator ==(Object other) => identical(this, other) || other is PublicReviewCreate &&
      other.rating == rating &&
      other.comment == comment;

    @override
    int get hashCode =>
        rating.hashCode +
        (comment == null ? 0 : comment.hashCode);

  factory PublicReviewCreate.fromJson(Map<String, dynamic> json) => _$PublicReviewCreateFromJson(json);

  Map<String, dynamic> toJson() => _$PublicReviewCreateToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

