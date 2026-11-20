//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'public_reschedule_appointment_request.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PublicRescheduleAppointmentRequest {
  /// Returns a new [PublicRescheduleAppointmentRequest] instance.
  PublicRescheduleAppointmentRequest({

    required  this.startUtc,
  });

  @JsonKey(
    
    name: r'startUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime startUtc;





    @override
    bool operator ==(Object other) => identical(this, other) || other is PublicRescheduleAppointmentRequest &&
      other.startUtc == startUtc;

    @override
    int get hashCode =>
        startUtc.hashCode;

  factory PublicRescheduleAppointmentRequest.fromJson(Map<String, dynamic> json) => _$PublicRescheduleAppointmentRequestFromJson(json);

  Map<String, dynamic> toJson() => _$PublicRescheduleAppointmentRequestToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

