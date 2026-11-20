//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/appointment_item_request.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'slot_search_request.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class SlotSearchRequest {
  /// Returns a new [SlotSearchRequest] instance.
  SlotSearchRequest({

    required  this.items,

     this.sameStaffForAll = true,

    required  this.date,
  });

  @JsonKey(
    
    name: r'items',
    required: true,
    includeIfNull: false,
  )


  final List<AppointmentItemRequest> items;



  @JsonKey(
    defaultValue: true,
    name: r'sameStaffForAll',
    required: false,
    includeIfNull: false,
  )


  final bool? sameStaffForAll;



  @JsonKey(
    
    name: r'date',
    required: true,
    includeIfNull: false,
  )


  final DateTime date;





    @override
    bool operator ==(Object other) => identical(this, other) || other is SlotSearchRequest &&
      other.items == items &&
      other.sameStaffForAll == sameStaffForAll &&
      other.date == date;

    @override
    int get hashCode =>
        items.hashCode +
        sameStaffForAll.hashCode +
        date.hashCode;

  factory SlotSearchRequest.fromJson(Map<String, dynamic> json) => _$SlotSearchRequestFromJson(json);

  Map<String, dynamic> toJson() => _$SlotSearchRequestToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

