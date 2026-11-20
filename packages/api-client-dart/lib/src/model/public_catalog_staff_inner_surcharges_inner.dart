//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/money.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'public_catalog_staff_inner_surcharges_inner.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PublicCatalogStaffInnerSurchargesInner {
  /// Returns a new [PublicCatalogStaffInnerSurchargesInner] instance.
  PublicCatalogStaffInnerSurchargesInner({

    required  this.serviceId,

    required  this.amount,
  });

  @JsonKey(
    
    name: r'serviceId',
    required: true,
    includeIfNull: false,
  )


  final String serviceId;



  @JsonKey(
    
    name: r'amount',
    required: true,
    includeIfNull: false,
  )


  final Money amount;





    @override
    bool operator ==(Object other) => identical(this, other) || other is PublicCatalogStaffInnerSurchargesInner &&
      other.serviceId == serviceId &&
      other.amount == amount;

    @override
    int get hashCode =>
        serviceId.hashCode +
        amount.hashCode;

  factory PublicCatalogStaffInnerSurchargesInner.fromJson(Map<String, dynamic> json) => _$PublicCatalogStaffInnerSurchargesInnerFromJson(json);

  Map<String, dynamic> toJson() => _$PublicCatalogStaffInnerSurchargesInnerToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

