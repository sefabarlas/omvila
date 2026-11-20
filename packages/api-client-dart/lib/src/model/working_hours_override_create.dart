//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/time_interval.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'working_hours_override_create.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class WorkingHoursOverrideCreate {
  /// Returns a new [WorkingHoursOverrideCreate] instance.
  WorkingHoursOverrideCreate({

    required  this.fromDate,

     this.toDate,

    required  this.kind,

    required  this.isClosed,

     this.intervals,

     this.note,
  });

  @JsonKey(
    
    name: r'fromDate',
    required: true,
    includeIfNull: false,
  )


  final DateTime fromDate;



      /// Çok günlük kapanış için (Ramazan Bayramı · 3 gün)
  @JsonKey(
    
    name: r'toDate',
    required: false,
    includeIfNull: false,
  )


  final DateTime? toDate;



  @JsonKey(
    
    name: r'kind',
    required: true,
    includeIfNull: false,
  unknownEnumValue: WorkingHoursOverrideCreateKindEnum.unknownDefaultOpenApi,
  )


  final WorkingHoursOverrideCreateKindEnum kind;



  @JsonKey(
    
    name: r'isClosed',
    required: true,
    includeIfNull: false,
  )


  final bool isClosed;



  @JsonKey(
    
    name: r'intervals',
    required: false,
    includeIfNull: false,
  )


  final List<TimeInterval>? intervals;



  @JsonKey(
    
    name: r'note',
    required: false,
    includeIfNull: false,
  )


  final String? note;





    @override
    bool operator ==(Object other) => identical(this, other) || other is WorkingHoursOverrideCreate &&
      other.fromDate == fromDate &&
      other.toDate == toDate &&
      other.kind == kind &&
      other.isClosed == isClosed &&
      other.intervals == intervals &&
      other.note == note;

    @override
    int get hashCode =>
        fromDate.hashCode +
        toDate.hashCode +
        kind.hashCode +
        isClosed.hashCode +
        intervals.hashCode +
        (note == null ? 0 : note.hashCode);

  factory WorkingHoursOverrideCreate.fromJson(Map<String, dynamic> json) => _$WorkingHoursOverrideCreateFromJson(json);

  Map<String, dynamic> toJson() => _$WorkingHoursOverrideCreateToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

enum WorkingHoursOverrideCreateKindEnum {
@JsonValue(r'Holiday')
holiday(r'Holiday'),
@JsonValue(r'Closure')
closure(r'Closure'),
@JsonValue(r'ExtraOpening')
extraOpening(r'ExtraOpening'),
@JsonValue(r'ShortDay')
shortDay(r'ShortDay'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const WorkingHoursOverrideCreateKindEnum(this.value);

final String value;

@override
String toString() => value;
}


