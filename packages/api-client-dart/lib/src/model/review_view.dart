//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/customer_ref.dart';
import 'package:omvila_api/src/model/review_view_reply.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'review_view.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ReviewView {
  /// Returns a new [ReviewView] instance.
  ReviewView({

    required  this.id,

    required  this.appointmentId,

    required  this.branchId,

    required  this.rating,

     this.comment,

    required  this.customer,

    required  this.serviceNames,

     this.staffId,

     this.staffName,

    required  this.createdUtc,

    required  this.isPublished,

    required  this.reply,
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



  @JsonKey(
    
    name: r'branchId',
    required: true,
    includeIfNull: false,
  )


  final String branchId;



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
    
    name: r'customer',
    required: true,
    includeIfNull: false,
  )


  final CustomerRef customer;



  @JsonKey(
    
    name: r'serviceNames',
    required: true,
    includeIfNull: false,
  )


  final List<String> serviceNames;



  @JsonKey(
    
    name: r'staffId',
    required: false,
    includeIfNull: false,
  )


  final String? staffId;



  @JsonKey(
    
    name: r'staffName',
    required: false,
    includeIfNull: false,
  )


  final String? staffName;



  @JsonKey(
    
    name: r'createdUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime createdUtc;



      /// Rezervasyon sayfasında gösterilebilir
  @JsonKey(
    
    name: r'isPublished',
    required: true,
    includeIfNull: false,
  )


  final bool isPublished;



  @JsonKey(
    
    name: r'reply',
    required: true,
    includeIfNull: true,
  )


  final ReviewViewReply? reply;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ReviewView &&
      other.id == id &&
      other.appointmentId == appointmentId &&
      other.branchId == branchId &&
      other.rating == rating &&
      other.comment == comment &&
      other.customer == customer &&
      other.serviceNames == serviceNames &&
      other.staffId == staffId &&
      other.staffName == staffName &&
      other.createdUtc == createdUtc &&
      other.isPublished == isPublished &&
      other.reply == reply;

    @override
    int get hashCode =>
        id.hashCode +
        appointmentId.hashCode +
        branchId.hashCode +
        rating.hashCode +
        (comment == null ? 0 : comment.hashCode) +
        customer.hashCode +
        serviceNames.hashCode +
        (staffId == null ? 0 : staffId.hashCode) +
        (staffName == null ? 0 : staffName.hashCode) +
        createdUtc.hashCode +
        isPublished.hashCode +
        (reply == null ? 0 : reply.hashCode);

  factory ReviewView.fromJson(Map<String, dynamic> json) => _$ReviewViewFromJson(json);

  Map<String, dynamic> toJson() => _$ReviewViewToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

