//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/customer_import_row_error.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'customer_import_preview.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class CustomerImportPreview {
  /// Returns a new [CustomerImportPreview] instance.
  CustomerImportPreview({

    required  this.importId,

    required  this.fileName,

    required  this.totalRows,

    required  this.toCreate,

    required  this.toMerge,

    required  this.invalidRows,

    required  this.mappedColumns,

    required  this.errors,

    required  this.status,

     this.expiresUtc,
  });

  @JsonKey(
    
    name: r'importId',
    required: true,
    includeIfNull: false,
  )


  final String importId;



  @JsonKey(
    
    name: r'fileName',
    required: true,
    includeIfNull: false,
  )


  final String fileName;



  @JsonKey(
    
    name: r'totalRows',
    required: true,
    includeIfNull: false,
  )


  final int totalRows;



      /// \"Eklenecek · yeni müşteri\"
  @JsonKey(
    
    name: r'toCreate',
    required: true,
    includeIfNull: false,
  )


  final int toCreate;



      /// \"Birleşecek · numarası zaten kayıtlı\"; mevcut notlar korunur
  @JsonKey(
    
    name: r'toMerge',
    required: true,
    includeIfNull: false,
  )


  final int toMerge;



      /// \"Hatalı satır\"
  @JsonKey(
    
    name: r'invalidRows',
    required: true,
    includeIfNull: false,
  )


  final int invalidRows;



      /// Eşlenen sütunlar
  @JsonKey(
    
    name: r'mappedColumns',
    required: true,
    includeIfNull: false,
  unknownEnumValue: CustomerImportPreviewMappedColumnsEnum.unknownDefaultOpenApi,
  )


  final List<CustomerImportPreviewMappedColumnsEnum> mappedColumns;



  @JsonKey(
    
    name: r'errors',
    required: true,
    includeIfNull: false,
  )


  final List<CustomerImportRowError> errors;



  @JsonKey(
    
    name: r'status',
    required: true,
    includeIfNull: false,
  unknownEnumValue: CustomerImportPreviewStatusEnum.unknownDefaultOpenApi,
  )


  final CustomerImportPreviewStatusEnum status;



      /// Önizleme bu andan sonra içe aktarılamaz
  @JsonKey(
    
    name: r'expiresUtc',
    required: false,
    includeIfNull: false,
  )


  final DateTime? expiresUtc;





    @override
    bool operator ==(Object other) => identical(this, other) || other is CustomerImportPreview &&
      other.importId == importId &&
      other.fileName == fileName &&
      other.totalRows == totalRows &&
      other.toCreate == toCreate &&
      other.toMerge == toMerge &&
      other.invalidRows == invalidRows &&
      other.mappedColumns == mappedColumns &&
      other.errors == errors &&
      other.status == status &&
      other.expiresUtc == expiresUtc;

    @override
    int get hashCode =>
        importId.hashCode +
        fileName.hashCode +
        totalRows.hashCode +
        toCreate.hashCode +
        toMerge.hashCode +
        invalidRows.hashCode +
        mappedColumns.hashCode +
        errors.hashCode +
        status.hashCode +
        (expiresUtc == null ? 0 : expiresUtc.hashCode);

  factory CustomerImportPreview.fromJson(Map<String, dynamic> json) => _$CustomerImportPreviewFromJson(json);

  Map<String, dynamic> toJson() => _$CustomerImportPreviewToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

enum CustomerImportPreviewMappedColumnsEnum {
@JsonValue(r'FullName')
fullName(r'FullName'),
@JsonValue(r'Phone')
phone(r'Phone'),
@JsonValue(r'Notes')
notes(r'Notes'),
@JsonValue(r'BirthYear')
birthYear(r'BirthYear'),
@JsonValue(r'Email')
email(r'Email'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const CustomerImportPreviewMappedColumnsEnum(this.value);

final String value;

@override
String toString() => value;
}


enum CustomerImportPreviewStatusEnum {
@JsonValue(r'Previewed')
previewed(r'Previewed'),
@JsonValue(r'Committed')
committed(r'Committed'),
@JsonValue(r'Failed')
failed(r'Failed'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const CustomerImportPreviewStatusEnum(this.value);

final String value;

@override
String toString() => value;
}


