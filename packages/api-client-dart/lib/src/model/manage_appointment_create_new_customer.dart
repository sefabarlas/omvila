//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'manage_appointment_create_new_customer.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ManageAppointmentCreateNewCustomer {
  /// Returns a new [ManageAppointmentCreateNewCustomer] instance.
  ManageAppointmentCreateNewCustomer({

    required  this.fullName,

    required  this.phone,
  });

  @JsonKey(
    
    name: r'fullName',
    required: true,
    includeIfNull: false,
  )


  final String fullName;



      /// Serbest biçimli telefon girişi; sunucu E.164'e çevirir (\"0532 418 77 05\", \"5324187705\", \"+90 532 418 7705\")
  @JsonKey(
    
    name: r'phone',
    required: true,
    includeIfNull: false,
  )


  final String phone;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ManageAppointmentCreateNewCustomer &&
      other.fullName == fullName &&
      other.phone == phone;

    @override
    int get hashCode =>
        fullName.hashCode +
        phone.hashCode;

  factory ManageAppointmentCreateNewCustomer.fromJson(Map<String, dynamic> json) => _$ManageAppointmentCreateNewCustomerFromJson(json);

  Map<String, dynamic> toJson() => _$ManageAppointmentCreateNewCustomerToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

