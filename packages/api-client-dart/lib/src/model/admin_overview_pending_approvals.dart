//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_overview_pending_approvals.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminOverviewPendingApprovals {
  /// Returns a new [AdminOverviewPendingApprovals] instance.
  AdminOverviewPendingApprovals({

    required  this.total,

    required  this.paymentNotices,

    required  this.smsSenders,
  });

      /// Toplam bekleyen
          // minimum: 0
  @JsonKey(
    
    name: r'total',
    required: true,
    includeIfNull: false,
  )


  final int total;



      /// Bekleyen havale bildirimi
          // minimum: 0
  @JsonKey(
    
    name: r'paymentNotices',
    required: true,
    includeIfNull: false,
  )


  final int paymentNotices;



      /// Bekleyen SMS başlığı
          // minimum: 0
  @JsonKey(
    
    name: r'smsSenders',
    required: true,
    includeIfNull: false,
  )


  final int smsSenders;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminOverviewPendingApprovals &&
      other.total == total &&
      other.paymentNotices == paymentNotices &&
      other.smsSenders == smsSenders;

    @override
    int get hashCode =>
        total.hashCode +
        paymentNotices.hashCode +
        smsSenders.hashCode;

  factory AdminOverviewPendingApprovals.fromJson(Map<String, dynamic> json) => _$AdminOverviewPendingApprovalsFromJson(json);

  Map<String, dynamic> toJson() => _$AdminOverviewPendingApprovalsToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

