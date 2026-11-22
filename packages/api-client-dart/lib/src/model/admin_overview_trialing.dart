//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_overview_trialing.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminOverviewTrialing {
  /// Returns a new [AdminOverviewTrialing] instance.
  AdminOverviewTrialing({

    required  this.count,

    required  this.endingThisWeek,
  });

      /// Denemedeki marka
          // minimum: 0
  @JsonKey(
    
    name: r'count',
    required: true,
    includeIfNull: false,
  )


  final int count;



      /// Denemesi 7 gün içinde biten
          // minimum: 0
  @JsonKey(
    
    name: r'endingThisWeek',
    required: true,
    includeIfNull: false,
  )


  final int endingThisWeek;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminOverviewTrialing &&
      other.count == count &&
      other.endingThisWeek == endingThisWeek;

    @override
    int get hashCode =>
        count.hashCode +
        endingThisWeek.hashCode;

  factory AdminOverviewTrialing.fromJson(Map<String, dynamic> json) => _$AdminOverviewTrialingFromJson(json);

  Map<String, dynamic> toJson() => _$AdminOverviewTrialingToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

