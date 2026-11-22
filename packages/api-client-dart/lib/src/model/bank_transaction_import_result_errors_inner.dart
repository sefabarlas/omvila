//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'bank_transaction_import_result_errors_inner.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class BankTransactionImportResultErrorsInner {
  /// Returns a new [BankTransactionImportResultErrorsInner] instance.
  BankTransactionImportResultErrorsInner({

    required  this.rowNumber,

    required  this.column,

    required  this.code,
  });

      /// Satır numarası (başlık 1)
          // minimum: 1
  @JsonKey(
    
    name: r'rowNumber',
    required: true,
    includeIfNull: false,
  )


  final int rowNumber;



      /// Sütun başlığı
  @JsonKey(
    
    name: r'column',
    required: true,
    includeIfNull: false,
  )


  final String column;



      /// Hata kodu (required, invalid_format, out_of_range)
  @JsonKey(
    
    name: r'code',
    required: true,
    includeIfNull: false,
  )


  final String code;





    @override
    bool operator ==(Object other) => identical(this, other) || other is BankTransactionImportResultErrorsInner &&
      other.rowNumber == rowNumber &&
      other.column == column &&
      other.code == code;

    @override
    int get hashCode =>
        rowNumber.hashCode +
        column.hashCode +
        code.hashCode;

  factory BankTransactionImportResultErrorsInner.fromJson(Map<String, dynamic> json) => _$BankTransactionImportResultErrorsInnerFromJson(json);

  Map<String, dynamic> toJson() => _$BankTransactionImportResultErrorsInnerToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

