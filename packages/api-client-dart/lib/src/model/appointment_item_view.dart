//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/money.dart';
import 'package:omvila_api/src/model/segment_view.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'appointment_item_view.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AppointmentItemView {
  /// Returns a new [AppointmentItemView] instance.
  AppointmentItemView({

    required  this.id,

    required  this.serviceId,

    required  this.serviceName,

     this.staffId,

     this.staffName,

    required  this.startUtc,

    required  this.endUtc,

    required  this.price,

     this.staffSurcharge,

     this.discount,

     this.paidFromPackage,

    required  this.status,

    required  this.segments,
  });

  @JsonKey(
    
    name: r'id',
    required: true,
    includeIfNull: false,
  )


  final String id;



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



  @JsonKey(
    
    name: r'staffSurcharge',
    required: false,
    includeIfNull: false,
  )


  final Money? staffSurcharge;



  @JsonKey(
    
    name: r'discount',
    required: false,
    includeIfNull: false,
  )


  final Money? discount;



  @JsonKey(
    
    name: r'paidFromPackage',
    required: false,
    includeIfNull: false,
  )


  final bool? paidFromPackage;



  @JsonKey(
    
    name: r'status',
    required: true,
    includeIfNull: false,
  unknownEnumValue: AppointmentItemViewStatusEnum.unknownDefaultOpenApi,
  )


  final AppointmentItemViewStatusEnum status;



  @JsonKey(
    
    name: r'segments',
    required: true,
    includeIfNull: false,
  )


  final List<SegmentView> segments;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AppointmentItemView &&
      other.id == id &&
      other.serviceId == serviceId &&
      other.serviceName == serviceName &&
      other.staffId == staffId &&
      other.staffName == staffName &&
      other.startUtc == startUtc &&
      other.endUtc == endUtc &&
      other.price == price &&
      other.staffSurcharge == staffSurcharge &&
      other.discount == discount &&
      other.paidFromPackage == paidFromPackage &&
      other.status == status &&
      other.segments == segments;

    @override
    int get hashCode =>
        id.hashCode +
        serviceId.hashCode +
        serviceName.hashCode +
        (staffId == null ? 0 : staffId.hashCode) +
        (staffName == null ? 0 : staffName.hashCode) +
        startUtc.hashCode +
        endUtc.hashCode +
        price.hashCode +
        staffSurcharge.hashCode +
        discount.hashCode +
        paidFromPackage.hashCode +
        status.hashCode +
        segments.hashCode;

  factory AppointmentItemView.fromJson(Map<String, dynamic> json) => _$AppointmentItemViewFromJson(json);

  Map<String, dynamic> toJson() => _$AppointmentItemViewToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

enum AppointmentItemViewStatusEnum {
@JsonValue(r'Active')
active(r'Active'),
@JsonValue(r'Completed')
completed(r'Completed'),
@JsonValue(r'Cancelled')
cancelled(r'Cancelled'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const AppointmentItemViewStatusEnum(this.value);

final String value;

@override
String toString() => value;
}


