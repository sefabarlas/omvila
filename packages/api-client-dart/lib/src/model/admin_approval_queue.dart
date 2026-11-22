//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/admin_approval_queue_item.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_approval_queue.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminApprovalQueue {
  /// Returns a new [AdminApprovalQueue] instance.
  AdminApprovalQueue({

    required  this.paymentNoticeCount,

    required  this.smsSenderCount,

    required  this.oldestSubmittedUtc,

    required  this.items,
  });

      /// \"Havale bildirimi 5\"
          // minimum: 0
  @JsonKey(
    
    name: r'paymentNoticeCount',
    required: true,
    includeIfNull: false,
  )


  final int paymentNoticeCount;



      /// \"SMS başlığı 2\"
          // minimum: 0
  @JsonKey(
    
    name: r'smsSenderCount',
    required: true,
    includeIfNull: false,
  )


  final int smsSenderCount;



      /// \"en eskisi 2 gün\"; kuyruk boşsa null
  @JsonKey(
    
    name: r'oldestSubmittedUtc',
    required: true,
    includeIfNull: true,
  )


  final DateTime? oldestSubmittedUtc;



      /// En eskiden yeniye
  @JsonKey(
    
    name: r'items',
    required: true,
    includeIfNull: false,
  )


  final List<AdminApprovalQueueItem> items;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminApprovalQueue &&
      other.paymentNoticeCount == paymentNoticeCount &&
      other.smsSenderCount == smsSenderCount &&
      other.oldestSubmittedUtc == oldestSubmittedUtc &&
      other.items == items;

    @override
    int get hashCode =>
        paymentNoticeCount.hashCode +
        smsSenderCount.hashCode +
        (oldestSubmittedUtc == null ? 0 : oldestSubmittedUtc.hashCode) +
        items.hashCode;

  factory AdminApprovalQueue.fromJson(Map<String, dynamic> json) => _$AdminApprovalQueueFromJson(json);

  Map<String, dynamic> toJson() => _$AdminApprovalQueueToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

