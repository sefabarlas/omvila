//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'subscription_detail_branches.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class SubscriptionDetailBranches {
  /// Returns a new [SubscriptionDetailBranches] instance.
  SubscriptionDetailBranches({

    required  this.count,

    required  this.included,

    required  this.extra,
  });

      /// Etkin şube sayısı
          // minimum: 0
  @JsonKey(
    
    name: r'count',
    required: true,
    includeIfNull: false,
  )


  final int count;



      /// Planın içerdiği şube sayısı; null = sınırsız
          // minimum: 0
  @JsonKey(
    
    name: r'included',
    required: true,
    includeIfNull: true,
  )


  final int? included;



      /// Ücretli ek şube sayısı
          // minimum: 0
  @JsonKey(
    
    name: r'extra',
    required: true,
    includeIfNull: false,
  )


  final int extra;





    @override
    bool operator ==(Object other) => identical(this, other) || other is SubscriptionDetailBranches &&
      other.count == count &&
      other.included == included &&
      other.extra == extra;

    @override
    int get hashCode =>
        count.hashCode +
        (included == null ? 0 : included.hashCode) +
        extra.hashCode;

  factory SubscriptionDetailBranches.fromJson(Map<String, dynamic> json) => _$SubscriptionDetailBranchesFromJson(json);

  Map<String, dynamic> toJson() => _$SubscriptionDetailBranchesToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

