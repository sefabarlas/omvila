//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/sms_sender_status.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_sms_sender_decision.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminSmsSenderDecision {
  /// Returns a new [AdminSmsSenderDecision] instance.
  AdminSmsSenderDecision({

    required  this.senderId,

    required  this.brandId,

    required  this.name,

    required  this.status,

    required  this.decidedUtc,

    required  this.rejectionReason,
  });

      /// Başvuru
  @JsonKey(
    
    name: r'senderId',
    required: true,
    includeIfNull: false,
  )


  final String senderId;



      /// Marka
  @JsonKey(
    
    name: r'brandId',
    required: true,
    includeIfNull: false,
  )


  final String brandId;



      /// Başvurulan ad
  @JsonKey(
    
    name: r'name',
    required: true,
    includeIfNull: false,
  )


  final String name;



  @JsonKey(
    
    name: r'status',
    required: true,
    includeIfNull: false,
  unknownEnumValue: SmsSenderStatus.unknownDefaultOpenApi,
  )


  final SmsSenderStatus status;



      /// Karar anı
  @JsonKey(
    
    name: r'decidedUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime decidedUtc;



      /// Ret nedeni
  @JsonKey(
    
    name: r'rejectionReason',
    required: true,
    includeIfNull: true,
  )


  final String? rejectionReason;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminSmsSenderDecision &&
      other.senderId == senderId &&
      other.brandId == brandId &&
      other.name == name &&
      other.status == status &&
      other.decidedUtc == decidedUtc &&
      other.rejectionReason == rejectionReason;

    @override
    int get hashCode =>
        senderId.hashCode +
        brandId.hashCode +
        name.hashCode +
        status.hashCode +
        decidedUtc.hashCode +
        (rejectionReason == null ? 0 : rejectionReason.hashCode);

  factory AdminSmsSenderDecision.fromJson(Map<String, dynamic> json) => _$AdminSmsSenderDecisionFromJson(json);

  Map<String, dynamic> toJson() => _$AdminSmsSenderDecisionToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

