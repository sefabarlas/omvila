//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/time_off.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'manage_list_time_off200_response.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ManageListTimeOff200Response {
  /// Returns a new [ManageListTimeOff200Response] instance.
  ManageListTimeOff200Response({

    required  this.items,
  });

  @JsonKey(
    
    name: r'items',
    required: true,
    includeIfNull: false,
  )


  final List<TimeOff> items;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ManageListTimeOff200Response &&
      other.items == items;

    @override
    int get hashCode =>
        items.hashCode;

  factory ManageListTimeOff200Response.fromJson(Map<String, dynamic> json) => _$ManageListTimeOff200ResponseFromJson(json);

  Map<String, dynamic> toJson() => _$ManageListTimeOff200ResponseToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

