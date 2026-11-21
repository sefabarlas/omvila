//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/pending_approval_item.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'pending_approval_list.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PendingApprovalList {
  /// Returns a new [PendingApprovalList] instance.
  PendingApprovalList({

    required  this.generatedUtc,

    required  this.totalCount,

    required  this.earliestHoldExpiresUtc,

    required  this.items,
  });

      /// Sunucunun \"şimdi\"si; kalan süre bu ana göre sayılır
  @JsonKey(
    
    name: r'generatedUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime generatedUtc;



      /// Onay bekleyen talep sayısı (\"Onay bekliyor 2\")
          // minimum: 0
  @JsonKey(
    
    name: r'totalCount',
    required: true,
    includeIfNull: false,
  )


  final int totalCount;



      /// En yakın otomatik iptal anı (\"ilki 1 sa 35 dk içinde iptal olur\"); liste boşsa null
  @JsonKey(
    
    name: r'earliestHoldExpiresUtc',
    required: true,
    includeIfNull: true,
  )


  final DateTime? earliestHoldExpiresUtc;



      /// holdExpiresUtc'ye göre artan sıralı
  @JsonKey(
    
    name: r'items',
    required: true,
    includeIfNull: false,
  )


  final List<PendingApprovalItem> items;





    @override
    bool operator ==(Object other) => identical(this, other) || other is PendingApprovalList &&
      other.generatedUtc == generatedUtc &&
      other.totalCount == totalCount &&
      other.earliestHoldExpiresUtc == earliestHoldExpiresUtc &&
      other.items == items;

    @override
    int get hashCode =>
        generatedUtc.hashCode +
        totalCount.hashCode +
        (earliestHoldExpiresUtc == null ? 0 : earliestHoldExpiresUtc.hashCode) +
        items.hashCode;

  factory PendingApprovalList.fromJson(Map<String, dynamic> json) => _$PendingApprovalListFromJson(json);

  Map<String, dynamic> toJson() => _$PendingApprovalListToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

