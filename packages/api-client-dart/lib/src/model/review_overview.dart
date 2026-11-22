//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/review_overview_top_staff.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'review_overview.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ReviewOverview {
  /// Returns a new [ReviewOverview] instance.
  ReviewOverview({

    required  this.averageRating,

    required  this.ratingCount,

    required  this.totalCount,

    required  this.awaitingReplyCount,

    required  this.thisMonthCount,

     this.thisMonthInviteResponsePercent,

     this.topStaff,
  });

      /// \"Ortalama · son 90 gün\"; değerlendirme yoksa null
  @JsonKey(
    
    name: r'averageRating',
    required: true,
    includeIfNull: true,
  )


  final num? averageRating;



      /// Son 90 gündeki değerlendirme sayısı
  @JsonKey(
    
    name: r'ratingCount',
    required: true,
    includeIfNull: false,
  )


  final int ratingCount;



      /// Başlıktaki \"214 değerlendirme\"
  @JsonKey(
    
    name: r'totalCount',
    required: true,
    includeIfNull: false,
  )


  final int totalCount;



      /// \"Yanıt bekleyen\"
  @JsonKey(
    
    name: r'awaitingReplyCount',
    required: true,
    includeIfNull: false,
  )


  final int awaitingReplyCount;



      /// \"Bu ay\"
  @JsonKey(
    
    name: r'thisMonthCount',
    required: true,
    includeIfNull: false,
  )


  final int thisMonthCount;



      /// \"davet edilenlerin %38'i\"; bu ay davet yoksa null
  @JsonKey(
    
    name: r'thisMonthInviteResponsePercent',
    required: false,
    includeIfNull: false,
  )


  final num? thisMonthInviteResponsePercent;



  @JsonKey(
    
    name: r'topStaff',
    required: false,
    includeIfNull: false,
  )


  final ReviewOverviewTopStaff? topStaff;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ReviewOverview &&
      other.averageRating == averageRating &&
      other.ratingCount == ratingCount &&
      other.totalCount == totalCount &&
      other.awaitingReplyCount == awaitingReplyCount &&
      other.thisMonthCount == thisMonthCount &&
      other.thisMonthInviteResponsePercent == thisMonthInviteResponsePercent &&
      other.topStaff == topStaff;

    @override
    int get hashCode =>
        (averageRating == null ? 0 : averageRating.hashCode) +
        ratingCount.hashCode +
        totalCount.hashCode +
        awaitingReplyCount.hashCode +
        thisMonthCount.hashCode +
        (thisMonthInviteResponsePercent == null ? 0 : thisMonthInviteResponsePercent.hashCode) +
        (topStaff == null ? 0 : topStaff.hashCode);

  factory ReviewOverview.fromJson(Map<String, dynamic> json) => _$ReviewOverviewFromJson(json);

  Map<String, dynamic> toJson() => _$ReviewOverviewToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

