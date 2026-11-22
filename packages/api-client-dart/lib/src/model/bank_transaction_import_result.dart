//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/bank_transaction_import_result_errors_inner.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'bank_transaction_import_result.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class BankTransactionImportResult {
  /// Returns a new [BankTransactionImportResult] instance.
  BankTransactionImportResult({

    required  this.dryRun,

    required  this.fileName,

    required  this.totalRows,

    required  this.imported,

    required  this.duplicates,

    required  this.skippedOutgoing,

    required  this.invalidRows,

    required  this.errors,
  });

      /// Önizleme mi
  @JsonKey(
    
    name: r'dryRun',
    required: true,
    includeIfNull: false,
  )


  final bool dryRun;



      /// Yüklenen dosya adı
  @JsonKey(
    
    name: r'fileName',
    required: true,
    includeIfNull: false,
  )


  final String fileName;



      /// Başlık dışı satır
          // minimum: 0
  @JsonKey(
    
    name: r'totalRows',
    required: true,
    includeIfNull: false,
  )


  final int totalRows;



      /// Eklenen (önizlemede eklenecek) hareket
          // minimum: 0
  @JsonKey(
    
    name: r'imported',
    required: true,
    includeIfNull: false,
  )


  final int imported;



      /// Referansı zaten kayıtlı olduğu için atlanan
          // minimum: 0
  @JsonKey(
    
    name: r'duplicates',
    required: true,
    includeIfNull: false,
  )


  final int duplicates;



      /// Giden (eksi) tutarlı olduğu için atlanan
          // minimum: 0
  @JsonKey(
    
    name: r'skippedOutgoing',
    required: true,
    includeIfNull: false,
  )


  final int skippedOutgoing;



      /// Hatalı satır
          // minimum: 0
  @JsonKey(
    
    name: r'invalidRows',
    required: true,
    includeIfNull: false,
  )


  final int invalidRows;



      /// Hatalı satırlar (en fazla 100)
  @JsonKey(
    
    name: r'errors',
    required: true,
    includeIfNull: false,
  )


  final List<BankTransactionImportResultErrorsInner> errors;





    @override
    bool operator ==(Object other) => identical(this, other) || other is BankTransactionImportResult &&
      other.dryRun == dryRun &&
      other.fileName == fileName &&
      other.totalRows == totalRows &&
      other.imported == imported &&
      other.duplicates == duplicates &&
      other.skippedOutgoing == skippedOutgoing &&
      other.invalidRows == invalidRows &&
      other.errors == errors;

    @override
    int get hashCode =>
        dryRun.hashCode +
        fileName.hashCode +
        totalRows.hashCode +
        imported.hashCode +
        duplicates.hashCode +
        skippedOutgoing.hashCode +
        invalidRows.hashCode +
        errors.hashCode;

  factory BankTransactionImportResult.fromJson(Map<String, dynamic> json) => _$BankTransactionImportResultFromJson(json);

  Map<String, dynamic> toJson() => _$BankTransactionImportResultToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

