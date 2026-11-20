//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'public_brand_rating.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PublicBrandRating {
  /// Returns a new [PublicBrandRating] instance.
  PublicBrandRating({

    required  this.average,

    required  this.count,
  });

  @JsonKey(
    
    name: r'average',
    required: true,
    includeIfNull: false,
  )


  final num average;



  @JsonKey(
    
    name: r'count',
    required: true,
    includeIfNull: false,
  )


  final int count;





    @override
    bool operator ==(Object other) => identical(this, other) || other is PublicBrandRating &&
      other.average == average &&
      other.count == count;

    @override
    int get hashCode =>
        average.hashCode +
        count.hashCode;

  factory PublicBrandRating.fromJson(Map<String, dynamic> json) => _$PublicBrandRatingFromJson(json);

  Map<String, dynamic> toJson() => _$PublicBrandRatingToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

