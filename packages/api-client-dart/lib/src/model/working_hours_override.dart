//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/time_interval.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'working_hours_override.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class WorkingHoursOverride {
  /// Returns a new [WorkingHoursOverride] instance.
  WorkingHoursOverride({

    required  this.id,

    required  this.date,

    required  this.kind,

    required  this.isClosed,

     this.intervals,

     this.note,
  });

  @JsonKey(
    
    name: r'id',
    required: true,
    includeIfNull: false,
  )


  final String id;



  @JsonKey(
    
    name: r'date',
    required: true,
    includeIfNull: false,
  )


  final DateTime date;



  @JsonKey(
    
    name: r'kind',
    required: true,
    includeIfNull: false,
  unknownEnumValue: WorkingHoursOverrideKindEnum.unknownDefaultOpenApi,
  )


  final WorkingHoursOverrideKindEnum kind;



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
    bool operator ==(Object other) => identical(this, other) || other is WorkingHoursOverride &&
      other.id == id &&
      other.date == date &&
      other.kind == kind &&
      other.isClosed == isClosed &&
      other.intervals == intervals &&
      other.note == note;

    @override
    int get hashCode =>
        id.hashCode +
        date.hashCode +
        kind.hashCode +
        isClosed.hashCode +
        intervals.hashCode +
        (note == null ? 0 : note.hashCode);

  factory WorkingHoursOverride.fromJson(Map<String, dynamic> json) => _$WorkingHoursOverrideFromJson(json);

  Map<String, dynamic> toJson() => _$WorkingHoursOverrideToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

enum WorkingHoursOverrideKindEnum {
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

const WorkingHoursOverrideKindEnum(this.value);

final String value;

@override
String toString() => value;
}


