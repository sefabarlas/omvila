//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'manage_overview_pending_approval.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ManageOverviewPendingApproval {
  /// Returns a new [ManageOverviewPendingApproval] instance.
  ManageOverviewPendingApproval({

    required  this.count,

    required  this.earliestHoldExpiresUtc,
  });

      /// Bekleyen talep sayısı
          // minimum: 0
  @JsonKey(
    
    name: r'count',
    required: true,
    includeIfNull: false,
  )


  final int count;



      /// \"ilki 1 sa 35 dk içinde iptal olur\"
  @JsonKey(
    
    name: r'earliestHoldExpiresUtc',
    required: true,
    includeIfNull: true,
  )


  final DateTime? earliestHoldExpiresUtc;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ManageOverviewPendingApproval &&
      other.count == count &&
      other.earliestHoldExpiresUtc == earliestHoldExpiresUtc;

    @override
    int get hashCode =>
        count.hashCode +
        (earliestHoldExpiresUtc == null ? 0 : earliestHoldExpiresUtc.hashCode);

  factory ManageOverviewPendingApproval.fromJson(Map<String, dynamic> json) => _$ManageOverviewPendingApprovalFromJson(json);

  Map<String, dynamic> toJson() => _$ManageOverviewPendingApprovalToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

