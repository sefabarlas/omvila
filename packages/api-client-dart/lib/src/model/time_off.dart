//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'time_off.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class TimeOff {
  /// Returns a new [TimeOff] instance.
  TimeOff({

    required  this.id,

    required  this.staffId,

    required  this.type,

    required  this.startUtc,

    required  this.endUtc,

    required  this.isAllDay,

     this.note,
  });

  @JsonKey(
    
    name: r'id',
    required: true,
    includeIfNull: false,
  )


  final String id;



  @JsonKey(
    
    name: r'staffId',
    required: true,
    includeIfNull: false,
  )


  final String staffId;



  @JsonKey(
    
    name: r'type',
    required: true,
    includeIfNull: false,
  unknownEnumValue: TimeOffTypeEnum.unknownDefaultOpenApi,
  )


  final TimeOffTypeEnum type;



  @JsonKey(
    
    name: r'startUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime startUtc;



  @JsonKey(
    
    name: r'endUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime endUtc;



  @JsonKey(
    
    name: r'isAllDay',
    required: true,
    includeIfNull: false,
  )


  final bool isAllDay;



  @JsonKey(
    
    name: r'note',
    required: false,
    includeIfNull: false,
  )


  final String? note;





    @override
    bool operator ==(Object other) => identical(this, other) || other is TimeOff &&
      other.id == id &&
      other.staffId == staffId &&
      other.type == type &&
      other.startUtc == startUtc &&
      other.endUtc == endUtc &&
      other.isAllDay == isAllDay &&
      other.note == note;

    @override
    int get hashCode =>
        id.hashCode +
        staffId.hashCode +
        type.hashCode +
        startUtc.hashCode +
        endUtc.hashCode +
        isAllDay.hashCode +
        (note == null ? 0 : note.hashCode);

  factory TimeOff.fromJson(Map<String, dynamic> json) => _$TimeOffFromJson(json);

  Map<String, dynamic> toJson() => _$TimeOffToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

enum TimeOffTypeEnum {
@JsonValue(r'Leave')
leave(r'Leave'),
@JsonValue(r'Break')
break_(r'Break'),
@JsonValue(r'Training')
training(r'Training'),
@JsonValue(r'Sick')
sick(r'Sick'),
@JsonValue(r'Other')
other(r'Other'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const TimeOffTypeEnum(this.value);

final String value;

@override
String toString() => value;
}


