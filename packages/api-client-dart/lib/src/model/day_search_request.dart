//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/appointment_item_request.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'day_search_request.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class DaySearchRequest {
  /// Returns a new [DaySearchRequest] instance.
  DaySearchRequest({

    required  this.items,

     this.sameStaffForAll = true,

    required  this.from,

    required  this.to,
  });

  @JsonKey(
    
    name: r'items',
    required: true,
    includeIfNull: false,
  )


  final List<AppointmentItemRequest> items;



      /// \"İki hizmet de aynı kişiden\"
  @JsonKey(
    defaultValue: true,
    name: r'sameStaffForAll',
    required: false,
    includeIfNull: false,
  )


  final bool? sameStaffForAll;



  @JsonKey(
    
    name: r'from',
    required: true,
    includeIfNull: false,
  )


  final DateTime from;



      /// En fazla 31 gün
  @JsonKey(
    
    name: r'to',
    required: true,
    includeIfNull: false,
  )


  final DateTime to;





    @override
    bool operator ==(Object other) => identical(this, other) || other is DaySearchRequest &&
      other.items == items &&
      other.sameStaffForAll == sameStaffForAll &&
      other.from == from &&
      other.to == to;

    @override
    int get hashCode =>
        items.hashCode +
        sameStaffForAll.hashCode +
        from.hashCode +
        to.hashCode;

  factory DaySearchRequest.fromJson(Map<String, dynamic> json) => _$DaySearchRequestFromJson(json);

  Map<String, dynamic> toJson() => _$DaySearchRequestToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

