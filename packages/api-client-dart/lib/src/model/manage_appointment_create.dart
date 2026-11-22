//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/manage_appointment_create_new_customer.dart';
import 'package:omvila_api/src/model/recurrence_request.dart';
import 'package:omvila_api/src/model/manage_appointment_create_items_inner.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'manage_appointment_create.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ManageAppointmentCreate {
  /// Returns a new [ManageAppointmentCreate] instance.
  ManageAppointmentCreate({

    required  this.branchId,

     this.brandCustomerId,

     this.newCustomer,

    required  this.channel,

    required  this.startUtc,

    required  this.items,

     this.staffNote,

     this.recurrence,

     this.notifyCustomer = true,

     this.allowOutsideWorkingHours = false,
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



      /// \"Nasıl geldi?\"
  @JsonKey(
    
    name: r'channel',
    required: true,
    includeIfNull: false,
  unknownEnumValue: ManageAppointmentCreateChannelEnum.unknownDefaultOpenApi,
  )


  final ManageAppointmentCreateChannelEnum channel;



      /// İlk kalemin başlangıcı; diğerleri zincirlenir
  @JsonKey(
    
    name: r'startUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime startUtc;



  @JsonKey(
    
    name: r'items',
    required: true,
    includeIfNull: false,
  )


  final List<ManageAppointmentCreateItemsInner> items;



  @JsonKey(
    
    name: r'staffNote',
    required: false,
    includeIfNull: true,
  )


  final String? staffNote;



  @JsonKey(
    
    name: r'recurrence',
    required: false,
    includeIfNull: false,
  )


  final RecurrenceRequest? recurrence;



      /// \"Müşteriye SMS gönder\"
  @JsonKey(
    defaultValue: true,
    name: r'notifyCustomer',
    required: false,
    includeIfNull: false,
  )


  final bool? notifyCustomer;



  @JsonKey(
    defaultValue: false,
    name: r'allowOutsideWorkingHours',
    required: false,
    includeIfNull: false,
  )


  final bool? allowOutsideWorkingHours;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ManageAppointmentCreate &&
      other.branchId == branchId &&
      other.brandCustomerId == brandCustomerId &&
      other.newCustomer == newCustomer &&
      other.channel == channel &&
      other.startUtc == startUtc &&
      other.items == items &&
      other.staffNote == staffNote &&
      other.recurrence == recurrence &&
      other.notifyCustomer == notifyCustomer &&
      other.allowOutsideWorkingHours == allowOutsideWorkingHours;

    @override
    int get hashCode =>
        branchId.hashCode +
        brandCustomerId.hashCode +
        newCustomer.hashCode +
        channel.hashCode +
        startUtc.hashCode +
        items.hashCode +
        (staffNote == null ? 0 : staffNote.hashCode) +
        recurrence.hashCode +
        notifyCustomer.hashCode +
        allowOutsideWorkingHours.hashCode;

  factory ManageAppointmentCreate.fromJson(Map<String, dynamic> json) => _$ManageAppointmentCreateFromJson(json);

  Map<String, dynamic> toJson() => _$ManageAppointmentCreateToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

/// \"Nasıl geldi?\"
enum ManageAppointmentCreateChannelEnum {
@JsonValue(r'Panel')
panel(r'Panel'),
@JsonValue(r'Phone')
phone(r'Phone'),
@JsonValue(r'WalkIn')
walkIn(r'WalkIn'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const ManageAppointmentCreateChannelEnum(this.value);

final String value;

@override
String toString() => value;
}


