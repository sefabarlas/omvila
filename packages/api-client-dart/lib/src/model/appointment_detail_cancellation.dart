//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/cancellation_reason.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'appointment_detail_cancellation.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AppointmentDetailCancellation {
  /// Returns a new [AppointmentDetailCancellation] instance.
  AppointmentDetailCancellation({

    required  this.cancelledUtc,

    required  this.cancelledBy,

    required  this.reason,

     this.note,
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
  unknownEnumValue: AppointmentDetailCancellationCancelledByEnum.unknownDefaultOpenApi,
  )


  final AppointmentDetailCancellationCancelledByEnum cancelledBy;



  @JsonKey(
    
    name: r'reason',
    required: true,
    includeIfNull: false,
  unknownEnumValue: CancellationReason.unknownDefaultOpenApi,
  )


  final CancellationReason reason;



  @JsonKey(
    
    name: r'note',
    required: false,
    includeIfNull: false,
  )


  final String? note;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AppointmentDetailCancellation &&
      other.cancelledUtc == cancelledUtc &&
      other.cancelledBy == cancelledBy &&
      other.reason == reason &&
      other.note == note;

    @override
    int get hashCode =>
        cancelledUtc.hashCode +
        cancelledBy.hashCode +
        reason.hashCode +
        (note == null ? 0 : note.hashCode);

  factory AppointmentDetailCancellation.fromJson(Map<String, dynamic> json) => _$AppointmentDetailCancellationFromJson(json);

  Map<String, dynamic> toJson() => _$AppointmentDetailCancellationToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

enum AppointmentDetailCancellationCancelledByEnum {
@JsonValue(r'Customer')
customer(r'Customer'),
@JsonValue(r'Salon')
salon(r'Salon'),
@JsonValue(r'System')
system(r'System'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const AppointmentDetailCancellationCancelledByEnum(this.value);

final String value;

@override
String toString() => value;
}


