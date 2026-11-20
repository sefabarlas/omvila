//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/customer_appointment_row.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'manage_list_customer_appointments200_response.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ManageListCustomerAppointments200Response {
  /// Returns a new [ManageListCustomerAppointments200Response] instance.
  ManageListCustomerAppointments200Response({

    required  this.items,

    required  this.nextCursor,
  });

  @JsonKey(
    
    name: r'items',
    required: true,
    includeIfNull: false,
  )


  final List<CustomerAppointmentRow> items;



  @JsonKey(
    
    name: r'nextCursor',
    required: true,
    includeIfNull: true,
  )


  final String? nextCursor;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ManageListCustomerAppointments200Response &&
      other.items == items &&
      other.nextCursor == nextCursor;

    @override
    int get hashCode =>
        items.hashCode +
        (nextCursor == null ? 0 : nextCursor.hashCode);

  factory ManageListCustomerAppointments200Response.fromJson(Map<String, dynamic> json) => _$ManageListCustomerAppointments200ResponseFromJson(json);

  Map<String, dynamic> toJson() => _$ManageListCustomerAppointments200ResponseToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

