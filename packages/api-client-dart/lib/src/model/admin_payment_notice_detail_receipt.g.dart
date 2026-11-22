// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_payment_notice_detail_receipt.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminPaymentNoticeDetailReceiptCWProxy {
  AdminPaymentNoticeDetailReceipt fileName(String fileName);

  AdminPaymentNoticeDetailReceipt contentType(
    AdminPaymentNoticeDetailReceiptContentTypeEnum contentType,
  );

  AdminPaymentNoticeDetailReceipt sizeBytes(int sizeBytes);

  AdminPaymentNoticeDetailReceipt uploadedUtc(DateTime uploadedUtc);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminPaymentNoticeDetailReceipt(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminPaymentNoticeDetailReceipt(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminPaymentNoticeDetailReceipt call({
    String fileName,
    AdminPaymentNoticeDetailReceiptContentTypeEnum contentType,
    int sizeBytes,
    DateTime uploadedUtc,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminPaymentNoticeDetailReceipt.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminPaymentNoticeDetailReceipt.copyWith.fieldName(...)`
class _$AdminPaymentNoticeDetailReceiptCWProxyImpl
    implements _$AdminPaymentNoticeDetailReceiptCWProxy {
  const _$AdminPaymentNoticeDetailReceiptCWProxyImpl(this._value);

  final AdminPaymentNoticeDetailReceipt _value;

  @override
  AdminPaymentNoticeDetailReceipt fileName(String fileName) =>
      this(fileName: fileName);

  @override
  AdminPaymentNoticeDetailReceipt contentType(
    AdminPaymentNoticeDetailReceiptContentTypeEnum contentType,
  ) => this(contentType: contentType);

  @override
  AdminPaymentNoticeDetailReceipt sizeBytes(int sizeBytes) =>
      this(sizeBytes: sizeBytes);

  @override
  AdminPaymentNoticeDetailReceipt uploadedUtc(DateTime uploadedUtc) =>
      this(uploadedUtc: uploadedUtc);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminPaymentNoticeDetailReceipt(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminPaymentNoticeDetailReceipt(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminPaymentNoticeDetailReceipt call({
    Object? fileName = const $CopyWithPlaceholder(),
    Object? contentType = const $CopyWithPlaceholder(),
    Object? sizeBytes = const $CopyWithPlaceholder(),
    Object? uploadedUtc = const $CopyWithPlaceholder(),
  }) {
    return AdminPaymentNoticeDetailReceipt(
      fileName: fileName == const $CopyWithPlaceholder()
          ? _value.fileName
          // ignore: cast_nullable_to_non_nullable
          : fileName as String,
      contentType: contentType == const $CopyWithPlaceholder()
          ? _value.contentType
          // ignore: cast_nullable_to_non_nullable
          : contentType as AdminPaymentNoticeDetailReceiptContentTypeEnum,
      sizeBytes: sizeBytes == const $CopyWithPlaceholder()
          ? _value.sizeBytes
          // ignore: cast_nullable_to_non_nullable
          : sizeBytes as int,
      uploadedUtc: uploadedUtc == const $CopyWithPlaceholder()
          ? _value.uploadedUtc
          // ignore: cast_nullable_to_non_nullable
          : uploadedUtc as DateTime,
    );
  }
}

extension $AdminPaymentNoticeDetailReceiptCopyWith
    on AdminPaymentNoticeDetailReceipt {
  /// Returns a callable class that can be used as follows: `instanceOfAdminPaymentNoticeDetailReceipt.copyWith(...)` or like so:`instanceOfAdminPaymentNoticeDetailReceipt.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminPaymentNoticeDetailReceiptCWProxy get copyWith =>
      _$AdminPaymentNoticeDetailReceiptCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminPaymentNoticeDetailReceipt _$AdminPaymentNoticeDetailReceiptFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('AdminPaymentNoticeDetailReceipt', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const ['fileName', 'contentType', 'sizeBytes', 'uploadedUtc'],
  );
  final val = AdminPaymentNoticeDetailReceipt(
    fileName: $checkedConvert('fileName', (v) => v as String),
    contentType: $checkedConvert(
      'contentType',
      (v) => $enumDecode(
        _$AdminPaymentNoticeDetailReceiptContentTypeEnumEnumMap,
        v,
        unknownValue: AdminPaymentNoticeDetailReceiptContentTypeEnum
            .unknownDefaultOpenApi,
      ),
    ),
    sizeBytes: $checkedConvert('sizeBytes', (v) => (v as num).toInt()),
    uploadedUtc: $checkedConvert(
      'uploadedUtc',
      (v) => DateTime.parse(v as String),
    ),
  );
  return val;
});

Map<String, dynamic> _$AdminPaymentNoticeDetailReceiptToJson(
  AdminPaymentNoticeDetailReceipt instance,
) => <String, dynamic>{
  'fileName': instance.fileName,
  'contentType':
      _$AdminPaymentNoticeDetailReceiptContentTypeEnumEnumMap[instance
          .contentType]!,
  'sizeBytes': instance.sizeBytes,
  'uploadedUtc': instance.uploadedUtc.toIso8601String(),
};

const _$AdminPaymentNoticeDetailReceiptContentTypeEnumEnumMap = {
  AdminPaymentNoticeDetailReceiptContentTypeEnum.applicationSlashPdf:
      'application/pdf',
  AdminPaymentNoticeDetailReceiptContentTypeEnum.imageSlashJpeg: 'image/jpeg',
  AdminPaymentNoticeDetailReceiptContentTypeEnum.imageSlashPng: 'image/png',
  AdminPaymentNoticeDetailReceiptContentTypeEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};
