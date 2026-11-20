//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'manage_reschedule_request_items_inner.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ManageRescheduleRequestItemsInner {
  /// Returns a new [ManageRescheduleRequestItemsInner] instance.
  ManageRescheduleRequestItemsInner({

    required  this.itemId,

    required  this.staffId,
  });

  @JsonKey(
    
    name: r'itemId',
    required: true,
    includeIfNull: false,
  )


  final String itemId;



  @JsonKey(
    
    name: r'staffId',
    required: true,
    includeIfNull: false,
  )


  final String staffId;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ManageRescheduleRequestItemsInner &&
      other.itemId == itemId &&
      other.staffId == staffId;

    @override
    int get hashCode =>
        itemId.hashCode +
        staffId.hashCode;

  factory ManageRescheduleRequestItemsInner.fromJson(Map<String, dynamic> json) => _$ManageRescheduleRequestItemsInnerFromJson(json);

  Map<String, dynamic> toJson() => _$ManageRescheduleRequestItemsInnerToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

