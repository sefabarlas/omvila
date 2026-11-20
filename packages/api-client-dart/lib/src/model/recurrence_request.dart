//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'recurrence_request.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class RecurrenceRequest {
  /// Returns a new [RecurrenceRequest] instance.
  RecurrenceRequest({

    required  this.frequency,

    required  this.intervalWeeks,

     this.endsOn,
  });

  @JsonKey(
    
    name: r'frequency',
    required: true,
    includeIfNull: false,
  unknownEnumValue: RecurrenceRequestFrequencyEnum.unknownDefaultOpenApi,
  )


  final RecurrenceRequestFrequencyEnum frequency;



  @JsonKey(
    
    name: r'intervalWeeks',
    required: true,
    includeIfNull: false,
  unknownEnumValue: RecurrenceRequestIntervalWeeksEnum.unknownDefaultOpenApi,
  )


  final RecurrenceRequestIntervalWeeksEnum intervalWeeks;



  @JsonKey(
    
    name: r'endsOn',
    required: false,
    includeIfNull: false,
  )


  final DateTime? endsOn;





    @override
    bool operator ==(Object other) => identical(this, other) || other is RecurrenceRequest &&
      other.frequency == frequency &&
      other.intervalWeeks == intervalWeeks &&
      other.endsOn == endsOn;

    @override
    int get hashCode =>
        frequency.hashCode +
        intervalWeeks.hashCode +
        (endsOn == null ? 0 : endsOn.hashCode);

  factory RecurrenceRequest.fromJson(Map<String, dynamic> json) => _$RecurrenceRequestFromJson(json);

  Map<String, dynamic> toJson() => _$RecurrenceRequestToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

enum RecurrenceRequestFrequencyEnum {
@JsonValue(r'Weekly')
weekly(r'Weekly'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const RecurrenceRequestFrequencyEnum(this.value);

final String value;

@override
String toString() => value;
}


enum RecurrenceRequestIntervalWeeksEnum {
@JsonValue(1)
number1('1'),
@JsonValue(2)
number2('2'),
@JsonValue(4)
number4('4'),
@JsonValue(11184809)
unknownDefaultOpenApi('11184809');

const RecurrenceRequestIntervalWeeksEnum(this.value);

final String value;

@override
String toString() => value;
}


