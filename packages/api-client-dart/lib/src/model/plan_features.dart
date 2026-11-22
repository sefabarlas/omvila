//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'plan_features.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PlanFeatures {
  /// Returns a new [PlanFeatures] instance.
  PlanFeatures({

    required  this.walkInQueue,

    required  this.waitlist,

    required  this.packages,

    required  this.crossBranchReports,

    required  this.prioritySupport,
  });

      /// Kapıda sıra
  @JsonKey(
    
    name: r'walkInQueue',
    required: true,
    includeIfNull: false,
  )


  final bool walkInQueue;



      /// Bekleme listesi
  @JsonKey(
    
    name: r'waitlist',
    required: true,
    includeIfNull: false,
  )


  final bool waitlist;



      /// Paketler ve prim raporu
  @JsonKey(
    
    name: r'packages',
    required: true,
    includeIfNull: false,
  )


  final bool packages;



      /// Şubeler arası rapor
  @JsonKey(
    
    name: r'crossBranchReports',
    required: true,
    includeIfNull: false,
  )


  final bool crossBranchReports;



      /// Öncelikli destek
  @JsonKey(
    
    name: r'prioritySupport',
    required: true,
    includeIfNull: false,
  )


  final bool prioritySupport;





    @override
    bool operator ==(Object other) => identical(this, other) || other is PlanFeatures &&
      other.walkInQueue == walkInQueue &&
      other.waitlist == waitlist &&
      other.packages == packages &&
      other.crossBranchReports == crossBranchReports &&
      other.prioritySupport == prioritySupport;

    @override
    int get hashCode =>
        walkInQueue.hashCode +
        waitlist.hashCode +
        packages.hashCode +
        crossBranchReports.hashCode +
        prioritySupport.hashCode;

  factory PlanFeatures.fromJson(Map<String, dynamic> json) => _$PlanFeaturesFromJson(json);

  Map<String, dynamic> toJson() => _$PlanFeaturesToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

