//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/public_review_invite_brand.dart';
import 'package:omvila_api/src/model/public_review_invite_review.dart';
import 'package:omvila_api/src/model/public_review_invite_staff.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'public_review_invite.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PublicReviewInvite {
  /// Returns a new [PublicReviewInvite] instance.
  PublicReviewInvite({

    required  this.brand,

    required  this.branchName,

    required  this.appointmentStartUtc,

    required  this.timeZone,

    required  this.serviceNames,

     this.staff,

    required  this.customerDisplayName,

    required  this.alreadyReviewed,

     this.review,
  });

  @JsonKey(
    
    name: r'brand',
    required: true,
    includeIfNull: false,
  )


  final PublicReviewInviteBrand brand;



  @JsonKey(
    
    name: r'branchName',
    required: true,
    includeIfNull: false,
  )


  final String branchName;



  @JsonKey(
    
    name: r'appointmentStartUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime appointmentStartUtc;



      /// IANA saat dilimi
  @JsonKey(
    
    name: r'timeZone',
    required: true,
    includeIfNull: false,
  )


  final String timeZone;



  @JsonKey(
    
    name: r'serviceNames',
    required: true,
    includeIfNull: false,
  )


  final List<String> serviceNames;



  @JsonKey(
    
    name: r'staff',
    required: false,
    includeIfNull: false,
  )


  final PublicReviewInviteStaff? staff;



      /// Yorumda görünecek ad (\"Merve Y.\"): ad + soyadın baş harfi
  @JsonKey(
    
    name: r'customerDisplayName',
    required: true,
    includeIfNull: false,
  )


  final String customerDisplayName;



      /// true ise form yerine gönderilmiş değerlendirme gösterilir
  @JsonKey(
    
    name: r'alreadyReviewed',
    required: true,
    includeIfNull: false,
  )


  final bool alreadyReviewed;



  @JsonKey(
    
    name: r'review',
    required: false,
    includeIfNull: false,
  )


  final PublicReviewInviteReview? review;





    @override
    bool operator ==(Object other) => identical(this, other) || other is PublicReviewInvite &&
      other.brand == brand &&
      other.branchName == branchName &&
      other.appointmentStartUtc == appointmentStartUtc &&
      other.timeZone == timeZone &&
      other.serviceNames == serviceNames &&
      other.staff == staff &&
      other.customerDisplayName == customerDisplayName &&
      other.alreadyReviewed == alreadyReviewed &&
      other.review == review;

    @override
    int get hashCode =>
        brand.hashCode +
        branchName.hashCode +
        appointmentStartUtc.hashCode +
        timeZone.hashCode +
        serviceNames.hashCode +
        (staff == null ? 0 : staff.hashCode) +
        customerDisplayName.hashCode +
        alreadyReviewed.hashCode +
        (review == null ? 0 : review.hashCode);

  factory PublicReviewInvite.fromJson(Map<String, dynamic> json) => _$PublicReviewInviteFromJson(json);

  Map<String, dynamic> toJson() => _$PublicReviewInviteToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

