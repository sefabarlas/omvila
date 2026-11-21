//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'review_overview_top_staff.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ReviewOverviewTopStaff {
  /// Returns a new [ReviewOverviewTopStaff] instance.
  ReviewOverviewTopStaff({

    required  this.staffId,

    required  this.fullName,

    required  this.averageRating,

    required  this.count,
  });

  @JsonKey(
    
    name: r'staffId',
    required: true,
    includeIfNull: false,
  )


  final String staffId;



  @JsonKey(
    
    name: r'fullName',
    required: true,
    includeIfNull: false,
  )


  final String fullName;



  @JsonKey(
    
    name: r'averageRating',
    required: true,
    includeIfNull: false,
  )


  final num averageRating;



  @JsonKey(
    
    name: r'count',
    required: true,
    includeIfNull: false,
  )


  final int count;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ReviewOverviewTopStaff &&
      other.staffId == staffId &&
      other.fullName == fullName &&
      other.averageRating == averageRating &&
      other.count == count;

    @override
    int get hashCode =>
        staffId.hashCode +
        fullName.hashCode +
        averageRating.hashCode +
        count.hashCode;

  factory ReviewOverviewTopStaff.fromJson(Map<String, dynamic> json) => _$ReviewOverviewTopStaffFromJson(json);

  Map<String, dynamic> toJson() => _$ReviewOverviewTopStaffToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

