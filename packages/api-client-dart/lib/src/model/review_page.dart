//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/review_view.dart';
import 'package:omvila_api/src/model/review_overview.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'review_page.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ReviewPage {
  /// Returns a new [ReviewPage] instance.
  ReviewPage({

    required  this.items,

    required  this.nextCursor,

    required  this.overview,
  });

  @JsonKey(
    
    name: r'items',
    required: true,
    includeIfNull: false,
  )


  final List<ReviewView> items;



  @JsonKey(
    
    name: r'nextCursor',
    required: true,
    includeIfNull: true,
  )


  final String? nextCursor;



  @JsonKey(
    
    name: r'overview',
    required: true,
    includeIfNull: false,
  )


  final ReviewOverview overview;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ReviewPage &&
      other.items == items &&
      other.nextCursor == nextCursor &&
      other.overview == overview;

    @override
    int get hashCode =>
        items.hashCode +
        (nextCursor == null ? 0 : nextCursor.hashCode) +
        overview.hashCode;

  factory ReviewPage.fromJson(Map<String, dynamic> json) => _$ReviewPageFromJson(json);

  Map<String, dynamic> toJson() => _$ReviewPageToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

