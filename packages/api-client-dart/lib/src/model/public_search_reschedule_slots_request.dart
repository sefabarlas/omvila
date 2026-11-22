//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'public_search_reschedule_slots_request.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PublicSearchRescheduleSlotsRequest {
  /// Returns a new [PublicSearchRescheduleSlotsRequest] instance.
  PublicSearchRescheduleSlotsRequest({

    required  this.date,
  });

  @JsonKey(
    
    name: r'date',
    required: true,
    includeIfNull: false,
  )
  @DateOnlyConverter()


  final DateTime date;





    @override
    bool operator ==(Object other) => identical(this, other) || other is PublicSearchRescheduleSlotsRequest &&
      other.date == date;

    @override
    int get hashCode =>
        date.hashCode;

  factory PublicSearchRescheduleSlotsRequest.fromJson(Map<String, dynamic> json) => _$PublicSearchRescheduleSlotsRequestFromJson(json);

  Map<String, dynamic> toJson() => _$PublicSearchRescheduleSlotsRequestToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

