//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'manage_overview_walk_ins.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ManageOverviewWalkIns {
  /// Returns a new [ManageOverviewWalkIns] instance.
  ManageOverviewWalkIns({

    required  this.waitingCount,

    required  this.estimatedWaitMinutesForNew,
  });

      /// Sırada bekleyen
          // minimum: 0
  @JsonKey(
    
    name: r'waitingCount',
    required: true,
    includeIfNull: false,
  )


  final int waitingCount;



      /// \"~35 dk bekleme\"
          // minimum: 0
  @JsonKey(
    
    name: r'estimatedWaitMinutesForNew',
    required: true,
    includeIfNull: true,
  )


  final int? estimatedWaitMinutesForNew;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ManageOverviewWalkIns &&
      other.waitingCount == waitingCount &&
      other.estimatedWaitMinutesForNew == estimatedWaitMinutesForNew;

    @override
    int get hashCode =>
        waitingCount.hashCode +
        (estimatedWaitMinutesForNew == null ? 0 : estimatedWaitMinutesForNew.hashCode);

  factory ManageOverviewWalkIns.fromJson(Map<String, dynamic> json) => _$ManageOverviewWalkInsFromJson(json);

  Map<String, dynamic> toJson() => _$ManageOverviewWalkInsToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

