//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'time_off_create.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class TimeOffCreate {
  /// Returns a new [TimeOffCreate] instance.
  TimeOffCreate({

    required  this.type,

    required  this.isAllDay,

     this.fromDate,

     this.toDate,

     this.startUtc,

     this.endUtc,

     this.note,
  });

  @JsonKey(
    
    name: r'type',
    required: true,
    includeIfNull: false,
  unknownEnumValue: TimeOffCreateTypeEnum.unknownDefaultOpenApi,
  )


  final TimeOffCreateTypeEnum type;



  @JsonKey(
    
    name: r'isAllDay',
    required: true,
    includeIfNull: false,
  )


  final bool isAllDay;



      /// isAllDay = true ise
  @JsonKey(
    
    name: r'fromDate',
    required: false,
    includeIfNull: false,
  )
  @DateOnlyConverter()


  final DateTime? fromDate;



      /// isAllDay = true ise (dahil)
  @JsonKey(
    
    name: r'toDate',
    required: false,
    includeIfNull: false,
  )
  @DateOnlyConverter()


  final DateTime? toDate;



      /// isAllDay = false ise
  @JsonKey(
    
    name: r'startUtc',
    required: false,
    includeIfNull: false,
  )


  final DateTime? startUtc;



      /// isAllDay = false ise
  @JsonKey(
    
    name: r'endUtc',
    required: false,
    includeIfNull: false,
  )


  final DateTime? endUtc;



  @JsonKey(
    
    name: r'note',
    required: false,
    includeIfNull: false,
  )


  final String? note;





    @override
    bool operator ==(Object other) => identical(this, other) || other is TimeOffCreate &&
      other.type == type &&
      other.isAllDay == isAllDay &&
      other.fromDate == fromDate &&
      other.toDate == toDate &&
      other.startUtc == startUtc &&
      other.endUtc == endUtc &&
      other.note == note;

    @override
    int get hashCode =>
        type.hashCode +
        isAllDay.hashCode +
        fromDate.hashCode +
        toDate.hashCode +
        startUtc.hashCode +
        endUtc.hashCode +
        (note == null ? 0 : note.hashCode);

  factory TimeOffCreate.fromJson(Map<String, dynamic> json) => _$TimeOffCreateFromJson(json);

  Map<String, dynamic> toJson() => _$TimeOffCreateToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

enum TimeOffCreateTypeEnum {
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

const TimeOffCreateTypeEnum(this.value);

final String value;

@override
String toString() => value;
}


