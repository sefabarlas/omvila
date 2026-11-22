//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/admin_approval_queue_item_payment_notice.dart';
import 'package:omvila_api/src/model/admin_approval_queue_item_sms_sender.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_approval_queue_item.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminApprovalQueueItem {
  /// Returns a new [AdminApprovalQueueItem] instance.
  AdminApprovalQueueItem({

    required  this.type,

    required  this.id,

    required  this.brandId,

    required  this.brandName,

    required  this.submittedUtc,

    required  this.paymentNotice,

    required  this.smsSender,
  });

      /// Satır türü
  @JsonKey(
    
    name: r'type',
    required: true,
    includeIfNull: false,
  unknownEnumValue: AdminApprovalQueueItemTypeEnum.unknownDefaultOpenApi,
  )


  final AdminApprovalQueueItemTypeEnum type;



      /// PaymentNotice veya BrandSmsSender kimliği
  @JsonKey(
    
    name: r'id',
    required: true,
    includeIfNull: false,
  )


  final String id;



      /// Marka
  @JsonKey(
    
    name: r'brandId',
    required: true,
    includeIfNull: false,
  )


  final String brandId;



      /// Marka adı
  @JsonKey(
    
    name: r'brandName',
    required: true,
    includeIfNull: false,
  )


  final String brandName;



      /// Bildirim veya başvuru anı (\"2 gündür bekliyor\")
  @JsonKey(
    
    name: r'submittedUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime submittedUtc;



  @JsonKey(
    
    name: r'paymentNotice',
    required: true,
    includeIfNull: true,
  )


  final AdminApprovalQueueItemPaymentNotice? paymentNotice;



  @JsonKey(
    
    name: r'smsSender',
    required: true,
    includeIfNull: true,
  )


  final AdminApprovalQueueItemSmsSender? smsSender;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminApprovalQueueItem &&
      other.type == type &&
      other.id == id &&
      other.brandId == brandId &&
      other.brandName == brandName &&
      other.submittedUtc == submittedUtc &&
      other.paymentNotice == paymentNotice &&
      other.smsSender == smsSender;

    @override
    int get hashCode =>
        type.hashCode +
        id.hashCode +
        brandId.hashCode +
        brandName.hashCode +
        submittedUtc.hashCode +
        (paymentNotice == null ? 0 : paymentNotice.hashCode) +
        (smsSender == null ? 0 : smsSender.hashCode);

  factory AdminApprovalQueueItem.fromJson(Map<String, dynamic> json) => _$AdminApprovalQueueItemFromJson(json);

  Map<String, dynamic> toJson() => _$AdminApprovalQueueItemToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

/// Satır türü
enum AdminApprovalQueueItemTypeEnum {
@JsonValue(r'PaymentNotice')
paymentNotice(r'PaymentNotice'),
@JsonValue(r'SmsSender')
smsSender(r'SmsSender'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const AdminApprovalQueueItemTypeEnum(this.value);

final String value;

@override
String toString() => value;
}


