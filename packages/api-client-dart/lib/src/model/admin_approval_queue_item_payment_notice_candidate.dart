//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/match_verdict.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_approval_queue_item_payment_notice_candidate.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminApprovalQueueItemPaymentNoticeCandidate {
  /// Returns a new [AdminApprovalQueueItemPaymentNoticeCandidate] instance.
  AdminApprovalQueueItemPaymentNoticeCandidate({

    required  this.bankTransactionId,

    required  this.verdict,

    required  this.codeFound,

    required  this.amountMatches,
  });

      /// Aday banka hareketi (\"Eşleştir\" bununla çağrılır)
  @JsonKey(
    
    name: r'bankTransactionId',
    required: true,
    includeIfNull: false,
  )


  final String bankTransactionId;



  @JsonKey(
    
    name: r'verdict',
    required: true,
    includeIfNull: false,
  unknownEnumValue: MatchVerdict.unknownDefaultOpenApi,
  )


  final MatchVerdict verdict;



      /// Açıklamada kod var
  @JsonKey(
    
    name: r'codeFound',
    required: true,
    includeIfNull: false,
  )


  final bool codeFound;



      /// Tutar beklenene eşit
  @JsonKey(
    
    name: r'amountMatches',
    required: true,
    includeIfNull: false,
  )


  final bool amountMatches;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminApprovalQueueItemPaymentNoticeCandidate &&
      other.bankTransactionId == bankTransactionId &&
      other.verdict == verdict &&
      other.codeFound == codeFound &&
      other.amountMatches == amountMatches;

    @override
    int get hashCode =>
        bankTransactionId.hashCode +
        verdict.hashCode +
        codeFound.hashCode +
        amountMatches.hashCode;

  factory AdminApprovalQueueItemPaymentNoticeCandidate.fromJson(Map<String, dynamic> json) => _$AdminApprovalQueueItemPaymentNoticeCandidateFromJson(json);

  Map<String, dynamic> toJson() => _$AdminApprovalQueueItemPaymentNoticeCandidateToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

