//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/money.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'public_waitlist_offer_items_inner.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PublicWaitlistOfferItemsInner {
  /// Returns a new [PublicWaitlistOfferItemsInner] instance.
  PublicWaitlistOfferItemsInner({

    required  this.serviceName,

     this.staffName,

    required  this.durationMinutes,

    required  this.price,
  });

  @JsonKey(
    
    name: r'serviceName',
    required: true,
    includeIfNull: false,
  )


  final String serviceName;



  @JsonKey(
    
    name: r'staffName',
    required: false,
    includeIfNull: false,
  )


  final String? staffName;



  @JsonKey(
    
    name: r'durationMinutes',
    required: true,
    includeIfNull: false,
  )


  final int durationMinutes;



  @JsonKey(
    
    name: r'price',
    required: true,
    includeIfNull: false,
  )


  final Money price;





    @override
    bool operator ==(Object other) => identical(this, other) || other is PublicWaitlistOfferItemsInner &&
      other.serviceName == serviceName &&
      other.staffName == staffName &&
      other.durationMinutes == durationMinutes &&
      other.price == price;

    @override
    int get hashCode =>
        serviceName.hashCode +
        (staffName == null ? 0 : staffName.hashCode) +
        durationMinutes.hashCode +
        price.hashCode;

  factory PublicWaitlistOfferItemsInner.fromJson(Map<String, dynamic> json) => _$PublicWaitlistOfferItemsInnerFromJson(json);

  Map<String, dynamic> toJson() => _$PublicWaitlistOfferItemsInnerToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

