//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_payment_notice_detail_receipt.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminPaymentNoticeDetailReceipt {
  /// Returns a new [AdminPaymentNoticeDetailReceipt] instance.
  AdminPaymentNoticeDetailReceipt({

    required  this.fileName,

    required  this.contentType,

    required  this.sizeBytes,

    required  this.uploadedUtc,
  });

      /// Dosya adı
  @JsonKey(
    
    name: r'fileName',
    required: true,
    includeIfNull: false,
  )


  final String fileName;



      /// Dosya türü
  @JsonKey(
    
    name: r'contentType',
    required: true,
    includeIfNull: false,
  unknownEnumValue: AdminPaymentNoticeDetailReceiptContentTypeEnum.unknownDefaultOpenApi,
  )


  final AdminPaymentNoticeDetailReceiptContentTypeEnum contentType;



      /// Boyut (bayt)
          // minimum: 0
          // maximum: 5242880
  @JsonKey(
    
    name: r'sizeBytes',
    required: true,
    includeIfNull: false,
  )


  final int sizeBytes;



      /// Yüklendiği an
  @JsonKey(
    
    name: r'uploadedUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime uploadedUtc;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminPaymentNoticeDetailReceipt &&
      other.fileName == fileName &&
      other.contentType == contentType &&
      other.sizeBytes == sizeBytes &&
      other.uploadedUtc == uploadedUtc;

    @override
    int get hashCode =>
        fileName.hashCode +
        contentType.hashCode +
        sizeBytes.hashCode +
        uploadedUtc.hashCode;

  factory AdminPaymentNoticeDetailReceipt.fromJson(Map<String, dynamic> json) => _$AdminPaymentNoticeDetailReceiptFromJson(json);

  Map<String, dynamic> toJson() => _$AdminPaymentNoticeDetailReceiptToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

/// Dosya türü
enum AdminPaymentNoticeDetailReceiptContentTypeEnum {
@JsonValue(r'application/pdf')
applicationSlashPdf(r'application/pdf'),
@JsonValue(r'image/jpeg')
imageSlashJpeg(r'image/jpeg'),
@JsonValue(r'image/png')
imageSlashPng(r'image/png'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const AdminPaymentNoticeDetailReceiptContentTypeEnum(this.value);

final String value;

@override
String toString() => value;
}


