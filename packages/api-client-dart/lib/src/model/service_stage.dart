//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'service_stage.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ServiceStage {
  /// Returns a new [ServiceStage] instance.
  ServiceStage({

    required  this.name,

    required  this.durationMinutes,

    required  this.staffBusy,
  });

  @JsonKey(
    
    name: r'name',
    required: true,
    includeIfNull: false,
  )


  final String name;



          // minimum: 5
          // maximum: 600
  @JsonKey(
    
    name: r'durationMinutes',
    required: true,
    includeIfNull: false,
  )


  final int durationMinutes;



      /// false = bekleme; personel başka müşteriye müsait
  @JsonKey(
    
    name: r'staffBusy',
    required: true,
    includeIfNull: false,
  )


  final bool staffBusy;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ServiceStage &&
      other.name == name &&
      other.durationMinutes == durationMinutes &&
      other.staffBusy == staffBusy;

    @override
    int get hashCode =>
        name.hashCode +
        durationMinutes.hashCode +
        staffBusy.hashCode;

  factory ServiceStage.fromJson(Map<String, dynamic> json) => _$ServiceStageFromJson(json);

  Map<String, dynamic> toJson() => _$ServiceStageToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

