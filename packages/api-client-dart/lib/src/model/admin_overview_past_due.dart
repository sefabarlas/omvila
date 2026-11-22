//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_overview_past_due.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminOverviewPastDue {
  /// Returns a new [AdminOverviewPastDue] instance.
  AdminOverviewPastDue({

    required  this.count,

    required  this.readOnlySoon,
  });

      /// Ödemesi geciken marka
          // minimum: 0
  @JsonKey(
    
    name: r'count',
    required: true,
    includeIfNull: false,
  )


  final int count;



      /// Salt okunur günü 2 gün içinde olan
          // minimum: 0
  @JsonKey(
    
    name: r'readOnlySoon',
    required: true,
    includeIfNull: false,
  )


  final int readOnlySoon;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminOverviewPastDue &&
      other.count == count &&
      other.readOnlySoon == readOnlySoon;

    @override
    int get hashCode =>
        count.hashCode +
        readOnlySoon.hashCode;

  factory AdminOverviewPastDue.fromJson(Map<String, dynamic> json) => _$AdminOverviewPastDueFromJson(json);

  Map<String, dynamic> toJson() => _$AdminOverviewPastDueToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

