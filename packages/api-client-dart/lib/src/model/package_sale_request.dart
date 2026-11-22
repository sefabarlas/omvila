//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/collected_payment_method.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'package_sale_request.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PackageSaleRequest {
  /// Returns a new [PackageSaleRequest] instance.
  PackageSaleRequest({

    required  this.packageId,

    required  this.branchId,

    required  this.soldByStaffId,

    required  this.paymentMethod,
  });

  @JsonKey(
    
    name: r'packageId',
    required: true,
    includeIfNull: false,
  )


  final String packageId;



      /// Satışın yapıldığı şube (kasaya bu şubede yazılır)
  @JsonKey(
    
    name: r'branchId',
    required: true,
    includeIfNull: false,
  )


  final String branchId;



      /// \"Satan personel\"; satış primi ona yazılır
  @JsonKey(
    
    name: r'soldByStaffId',
    required: true,
    includeIfNull: false,
  )


  final String soldByStaffId;



  @JsonKey(
    
    name: r'paymentMethod',
    required: true,
    includeIfNull: false,
  unknownEnumValue: CollectedPaymentMethod.unknownDefaultOpenApi,
  )


  final CollectedPaymentMethod paymentMethod;





    @override
    bool operator ==(Object other) => identical(this, other) || other is PackageSaleRequest &&
      other.packageId == packageId &&
      other.branchId == branchId &&
      other.soldByStaffId == soldByStaffId &&
      other.paymentMethod == paymentMethod;

    @override
    int get hashCode =>
        packageId.hashCode +
        branchId.hashCode +
        soldByStaffId.hashCode +
        paymentMethod.hashCode;

  factory PackageSaleRequest.fromJson(Map<String, dynamic> json) => _$PackageSaleRequestFromJson(json);

  Map<String, dynamic> toJson() => _$PackageSaleRequestToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

