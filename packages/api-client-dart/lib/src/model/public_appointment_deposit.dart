//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/money.dart';
import 'package:omvila_api/src/model/deposit_instructions.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'public_appointment_deposit.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PublicAppointmentDeposit {
  /// Returns a new [PublicAppointmentDeposit] instance.
  PublicAppointmentDeposit({

    required  this.required_,

    required  this.status,

     this.transferCode,

     this.customerReportedUtc,

     this.receivedUtc,

     this.instructions,
  });

  @JsonKey(
    
    name: r'required',
    required: true,
    includeIfNull: false,
  )


  final Money required_;



  @JsonKey(
    
    name: r'status',
    required: true,
    includeIfNull: false,
  unknownEnumValue: PublicAppointmentDepositStatusEnum.unknownDefaultOpenApi,
  )


  final PublicAppointmentDepositStatusEnum status;



  @JsonKey(
    
    name: r'transferCode',
    required: false,
    includeIfNull: false,
  )


  final String? transferCode;



  @JsonKey(
    
    name: r'customerReportedUtc',
    required: false,
    includeIfNull: false,
  )


  final DateTime? customerReportedUtc;



  @JsonKey(
    
    name: r'receivedUtc',
    required: false,
    includeIfNull: false,
  )


  final DateTime? receivedUtc;



  @JsonKey(
    
    name: r'instructions',
    required: false,
    includeIfNull: false,
  )


  final DepositInstructions? instructions;





    @override
    bool operator ==(Object other) => identical(this, other) || other is PublicAppointmentDeposit &&
      other.required_ == required_ &&
      other.status == status &&
      other.transferCode == transferCode &&
      other.customerReportedUtc == customerReportedUtc &&
      other.receivedUtc == receivedUtc &&
      other.instructions == instructions;

    @override
    int get hashCode =>
        required_.hashCode +
        status.hashCode +
        transferCode.hashCode +
        (customerReportedUtc == null ? 0 : customerReportedUtc.hashCode) +
        (receivedUtc == null ? 0 : receivedUtc.hashCode) +
        instructions.hashCode;

  factory PublicAppointmentDeposit.fromJson(Map<String, dynamic> json) => _$PublicAppointmentDepositFromJson(json);

  Map<String, dynamic> toJson() => _$PublicAppointmentDepositToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

enum PublicAppointmentDepositStatusEnum {
@JsonValue(r'Awaiting')
awaiting(r'Awaiting'),
@JsonValue(r'CustomerReported')
customerReported(r'CustomerReported'),
@JsonValue(r'Received')
received(r'Received'),
@JsonValue(r'Refunded')
refunded(r'Refunded'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const PublicAppointmentDepositStatusEnum(this.value);

final String value;

@override
String toString() => value;
}


