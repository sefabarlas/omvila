//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/manage_appointment_create_new_customer.dart';
import 'package:omvila_api/src/model/waitlist_preferences.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'manage_waitlist_create.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ManageWaitlistCreate {
  /// Returns a new [ManageWaitlistCreate] instance.
  ManageWaitlistCreate({

    required  this.branchId,

     this.brandCustomerId,

     this.newCustomer,

    required  this.preferences,

     this.note,
  });

  @JsonKey(
    
    name: r'branchId',
    required: true,
    includeIfNull: false,
  )


  final String branchId;



      /// Var olan müşteri; newCustomer ile XOR
  @JsonKey(
    
    name: r'brandCustomerId',
    required: false,
    includeIfNull: false,
  )


  final String? brandCustomerId;



  @JsonKey(
    
    name: r'newCustomer',
    required: false,
    includeIfNull: false,
  )


  final ManageAppointmentCreateNewCustomer? newCustomer;



  @JsonKey(
    
    name: r'preferences',
    required: true,
    includeIfNull: false,
  )


  final WaitlistPreferences preferences;



  @JsonKey(
    
    name: r'note',
    required: false,
    includeIfNull: false,
  )


  final String? note;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ManageWaitlistCreate &&
      other.branchId == branchId &&
      other.brandCustomerId == brandCustomerId &&
      other.newCustomer == newCustomer &&
      other.preferences == preferences &&
      other.note == note;

    @override
    int get hashCode =>
        branchId.hashCode +
        brandCustomerId.hashCode +
        newCustomer.hashCode +
        preferences.hashCode +
        (note == null ? 0 : note.hashCode);

  factory ManageWaitlistCreate.fromJson(Map<String, dynamic> json) => _$ManageWaitlistCreateFromJson(json);

  Map<String, dynamic> toJson() => _$ManageWaitlistCreateToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

