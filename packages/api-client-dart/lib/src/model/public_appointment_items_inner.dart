//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/money.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'public_appointment_items_inner.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PublicAppointmentItemsInner {
  /// Returns a new [PublicAppointmentItemsInner] instance.
  PublicAppointmentItemsInner({

    required  this.serviceName,

     this.staffName,

    required  this.startUtc,

    required  this.endUtc,

    required  this.price,

    required  this.paidFromPackage,
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
    
    name: r'startUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime startUtc;



  @JsonKey(
    
    name: r'endUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime endUtc;



  @JsonKey(
    
    name: r'price',
    required: true,
    includeIfNull: false,
  )


  final Money price;



  @JsonKey(
    
    name: r'paidFromPackage',
    required: true,
    includeIfNull: false,
  )


  final bool paidFromPackage;





    @override
    bool operator ==(Object other) => identical(this, other) || other is PublicAppointmentItemsInner &&
      other.serviceName == serviceName &&
      other.staffName == staffName &&
      other.startUtc == startUtc &&
      other.endUtc == endUtc &&
      other.price == price &&
      other.paidFromPackage == paidFromPackage;

    @override
    int get hashCode =>
        serviceName.hashCode +
        (staffName == null ? 0 : staffName.hashCode) +
        startUtc.hashCode +
        endUtc.hashCode +
        price.hashCode +
        paidFromPackage.hashCode;

  factory PublicAppointmentItemsInner.fromJson(Map<String, dynamic> json) => _$PublicAppointmentItemsInnerFromJson(json);

  Map<String, dynamic> toJson() => _$PublicAppointmentItemsInnerToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

