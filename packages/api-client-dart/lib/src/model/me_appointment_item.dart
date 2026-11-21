//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/money.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'me_appointment_item.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class MeAppointmentItem {
  /// Returns a new [MeAppointmentItem] instance.
  MeAppointmentItem({

    required  this.serviceId,

    required  this.serviceName,

     this.staffId,

     this.staffName,

    required  this.startUtc,

    required  this.endUtc,

    required  this.price,

    required  this.paidFromPackage,
  });

  @JsonKey(
    
    name: r'serviceId',
    required: true,
    includeIfNull: false,
  )


  final String serviceId;



  @JsonKey(
    
    name: r'serviceName',
    required: true,
    includeIfNull: false,
  )


  final String serviceName;



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



      /// Paketten düşüldü
  @JsonKey(
    
    name: r'paidFromPackage',
    required: true,
    includeIfNull: false,
  )


  final bool paidFromPackage;





    @override
    bool operator ==(Object other) => identical(this, other) || other is MeAppointmentItem &&
      other.serviceId == serviceId &&
      other.serviceName == serviceName &&
      other.staffId == staffId &&
      other.staffName == staffName &&
      other.startUtc == startUtc &&
      other.endUtc == endUtc &&
      other.price == price &&
      other.paidFromPackage == paidFromPackage;

    @override
    int get hashCode =>
        serviceId.hashCode +
        serviceName.hashCode +
        (staffId == null ? 0 : staffId.hashCode) +
        (staffName == null ? 0 : staffName.hashCode) +
        startUtc.hashCode +
        endUtc.hashCode +
        price.hashCode +
        paidFromPackage.hashCode;

  factory MeAppointmentItem.fromJson(Map<String, dynamic> json) => _$MeAppointmentItemFromJson(json);

  Map<String, dynamic> toJson() => _$MeAppointmentItemToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

