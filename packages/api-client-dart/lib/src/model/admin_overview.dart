//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/admin_overview_funnel.dart';
import 'package:omvila_api/src/model/admin_attention_brand.dart';
import 'package:omvila_api/src/model/admin_overview_active_brands.dart';
import 'package:omvila_api/src/model/admin_overview_past_due.dart';
import 'package:omvila_api/src/model/admin_overview_pending_approvals.dart';
import 'package:omvila_api/src/model/admin_overview_trialing.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_overview.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminOverview {
  /// Returns a new [AdminOverview] instance.
  AdminOverview({

    required  this.generatedUtc,

    required  this.activeBrands,

    required  this.pendingApprovals,

    required  this.trialing,

    required  this.pastDue,

    required  this.funnel,

    required  this.attentionBrands,
  });

      /// Yanıtın üretildiği an
  @JsonKey(
    
    name: r'generatedUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime generatedUtc;



  @JsonKey(
    
    name: r'activeBrands',
    required: true,
    includeIfNull: false,
  )


  final AdminOverviewActiveBrands activeBrands;



  @JsonKey(
    
    name: r'pendingApprovals',
    required: true,
    includeIfNull: false,
  )


  final AdminOverviewPendingApprovals pendingApprovals;



  @JsonKey(
    
    name: r'trialing',
    required: true,
    includeIfNull: false,
  )


  final AdminOverviewTrialing trialing;



  @JsonKey(
    
    name: r'pastDue',
    required: true,
    includeIfNull: false,
  )


  final AdminOverviewPastDue pastDue;



  @JsonKey(
    
    name: r'funnel',
    required: true,
    includeIfNull: false,
  )


  final AdminOverviewFunnel funnel;



      /// Aciliyete göre sıralı, en fazla 10
  @JsonKey(
    
    name: r'attentionBrands',
    required: true,
    includeIfNull: false,
  )


  final List<AdminAttentionBrand> attentionBrands;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminOverview &&
      other.generatedUtc == generatedUtc &&
      other.activeBrands == activeBrands &&
      other.pendingApprovals == pendingApprovals &&
      other.trialing == trialing &&
      other.pastDue == pastDue &&
      other.funnel == funnel &&
      other.attentionBrands == attentionBrands;

    @override
    int get hashCode =>
        generatedUtc.hashCode +
        activeBrands.hashCode +
        pendingApprovals.hashCode +
        trialing.hashCode +
        pastDue.hashCode +
        funnel.hashCode +
        attentionBrands.hashCode;

  factory AdminOverview.fromJson(Map<String, dynamic> json) => _$AdminOverviewFromJson(json);

  Map<String, dynamic> toJson() => _$AdminOverviewToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

