//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_payment_notice_detail_candidate_window.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminPaymentNoticeDetailCandidateWindow {
  /// Returns a new [AdminPaymentNoticeDetailCandidateWindow] instance.
  AdminPaymentNoticeDetailCandidateWindow({

    required  this.from,

    required  this.to,
  });

      /// İlk gün
  @JsonKey(
    
    name: r'from',
    required: true,
    includeIfNull: false,
  )
  @DateOnlyConverter()


  final DateTime from;



      /// Son gün
  @JsonKey(
    
    name: r'to',
    required: true,
    includeIfNull: false,
  )
  @DateOnlyConverter()


  final DateTime to;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminPaymentNoticeDetailCandidateWindow &&
      other.from == from &&
      other.to == to;

    @override
    int get hashCode =>
        from.hashCode +
        to.hashCode;

  factory AdminPaymentNoticeDetailCandidateWindow.fromJson(Map<String, dynamic> json) => _$AdminPaymentNoticeDetailCandidateWindowFromJson(json);

  Map<String, dynamic> toJson() => _$AdminPaymentNoticeDetailCandidateWindowToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

