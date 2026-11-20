//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/cancellation_reason.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'public_appointment_cancellation.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PublicAppointmentCancellation {
  /// Returns a new [PublicAppointmentCancellation] instance.
  PublicAppointmentCancellation({

    required  this.cancelledUtc,

    required  this.cancelledBy,

    required  this.reason,
  });

  @JsonKey(
    
    name: r'cancelledUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime cancelledUtc;



  @JsonKey(
    
    name: r'cancelledBy',
    required: true,
    includeIfNull: false,
  unknownEnumValue: PublicAppointmentCancellationCancelledByEnum.unknownDefaultOpenApi,
  )


  final PublicAppointmentCancellationCancelledByEnum cancelledBy;



  @JsonKey(
    
    name: r'reason',
    required: true,
    includeIfNull: false,
  unknownEnumValue: CancellationReason.unknownDefaultOpenApi,
  )


  final CancellationReason reason;





    @override
    bool operator ==(Object other) => identical(this, other) || other is PublicAppointmentCancellation &&
      other.cancelledUtc == cancelledUtc &&
      other.cancelledBy == cancelledBy &&
      other.reason == reason;

    @override
    int get hashCode =>
        cancelledUtc.hashCode +
        cancelledBy.hashCode +
        reason.hashCode;

  factory PublicAppointmentCancellation.fromJson(Map<String, dynamic> json) => _$PublicAppointmentCancellationFromJson(json);

  Map<String, dynamic> toJson() => _$PublicAppointmentCancellationToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

enum PublicAppointmentCancellationCancelledByEnum {
@JsonValue(r'Customer')
customer(r'Customer'),
@JsonValue(r'Salon')
salon(r'Salon'),
@JsonValue(r'System')
system(r'System'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const PublicAppointmentCancellationCancelledByEnum(this.value);

final String value;

@override
String toString() => value;
}


