//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/appointment_detail.dart';
import 'package:omvila_api/src/model/walk_in_ticket_view.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'walk_in_started.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class WalkInStarted {
  /// Returns a new [WalkInStarted] instance.
  WalkInStarted({

    required  this.ticket,

    required  this.appointment,
  });

  @JsonKey(
    
    name: r'ticket',
    required: true,
    includeIfNull: false,
  )


  final WalkInTicketView ticket;



  @JsonKey(
    
    name: r'appointment',
    required: true,
    includeIfNull: false,
  )


  final AppointmentDetail appointment;





    @override
    bool operator ==(Object other) => identical(this, other) || other is WalkInStarted &&
      other.ticket == ticket &&
      other.appointment == appointment;

    @override
    int get hashCode =>
        ticket.hashCode +
        appointment.hashCode;

  factory WalkInStarted.fromJson(Map<String, dynamic> json) => _$WalkInStartedFromJson(json);

  Map<String, dynamic> toJson() => _$WalkInStartedToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

