//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'appointment_detail_customer.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AppointmentDetailCustomer {
  /// Returns a new [AppointmentDetailCustomer] instance.
  AppointmentDetailCustomer({

    required  this.brandCustomerId,

    required  this.fullName,

     this.phone,

     this.phoneMasked,

    required  this.visitCount,

     this.lastVisitLabel,

     this.lastVisitOn,

     this.isNew,
  });

  @JsonKey(
    
    name: r'brandCustomerId',
    required: true,
    includeIfNull: false,
  )


  final String brandCustomerId;



  @JsonKey(
    
    name: r'fullName',
    required: true,
    includeIfNull: false,
  )


  final String fullName;



      /// E.164 telefon
  @JsonKey(
    
    name: r'phone',
    required: false,
    includeIfNull: false,
  )


  final String? phone;



  @JsonKey(
    
    name: r'phoneMasked',
    required: false,
    includeIfNull: false,
  )


  final String? phoneMasked;



  @JsonKey(
    
    name: r'visitCount',
    required: true,
    includeIfNull: false,
  )


  final int visitCount;



      /// Sunucu biçimlemez; istemci lastVisitOn'dan üretir
  @JsonKey(
    
    name: r'lastVisitLabel',
    required: false,
    includeIfNull: false,
  )


  final String? lastVisitLabel;



  @JsonKey(
    
    name: r'lastVisitOn',
    required: false,
    includeIfNull: false,
  )


  final DateTime? lastVisitOn;



  @JsonKey(
    
    name: r'isNew',
    required: false,
    includeIfNull: false,
  )


  final bool? isNew;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AppointmentDetailCustomer &&
      other.brandCustomerId == brandCustomerId &&
      other.fullName == fullName &&
      other.phone == phone &&
      other.phoneMasked == phoneMasked &&
      other.visitCount == visitCount &&
      other.lastVisitLabel == lastVisitLabel &&
      other.lastVisitOn == lastVisitOn &&
      other.isNew == isNew;

    @override
    int get hashCode =>
        brandCustomerId.hashCode +
        fullName.hashCode +
        phone.hashCode +
        phoneMasked.hashCode +
        visitCount.hashCode +
        (lastVisitLabel == null ? 0 : lastVisitLabel.hashCode) +
        (lastVisitOn == null ? 0 : lastVisitOn.hashCode) +
        isNew.hashCode;

  factory AppointmentDetailCustomer.fromJson(Map<String, dynamic> json) => _$AppointmentDetailCustomerFromJson(json);

  Map<String, dynamic> toJson() => _$AppointmentDetailCustomerToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

