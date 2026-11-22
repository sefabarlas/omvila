//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/appointment_detail.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'appointment_created.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AppointmentCreated {
  /// Returns a new [AppointmentCreated] instance.
  AppointmentCreated({

    required  this.appointment,

    required  this.skippedOccurrences,
  });

  @JsonKey(
    
    name: r'appointment',
    required: true,
    includeIfNull: false,
  )


  final AppointmentDetail appointment;



      /// Tekrarlayan randevuda çakıştığı için oluşturulamayan tarihler
  @JsonKey(
    
    name: r'skippedOccurrences',
    required: true,
    includeIfNull: false,
  )
  @DateOnlyConverter()


  final List<DateTime> skippedOccurrences;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AppointmentCreated &&
      other.appointment == appointment &&
      other.skippedOccurrences == skippedOccurrences;

    @override
    int get hashCode =>
        appointment.hashCode +
        skippedOccurrences.hashCode;

  factory AppointmentCreated.fromJson(Map<String, dynamic> json) => _$AppointmentCreatedFromJson(json);

  Map<String, dynamic> toJson() => _$AppointmentCreatedToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

