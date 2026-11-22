//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'me_reschedule_request.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class MeRescheduleRequest {
  /// Returns a new [MeRescheduleRequest] instance.
  MeRescheduleRequest({

    required  this.startUtc,
  });

  @JsonKey(
    
    name: r'startUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime startUtc;





    @override
    bool operator ==(Object other) => identical(this, other) || other is MeRescheduleRequest &&
      other.startUtc == startUtc;

    @override
    int get hashCode =>
        startUtc.hashCode;

  factory MeRescheduleRequest.fromJson(Map<String, dynamic> json) => _$MeRescheduleRequestFromJson(json);

  Map<String, dynamic> toJson() => _$MeRescheduleRequestToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

