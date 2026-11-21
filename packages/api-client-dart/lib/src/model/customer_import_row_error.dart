//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'customer_import_row_error.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class CustomerImportRowError {
  /// Returns a new [CustomerImportRowError] instance.
  CustomerImportRowError({

    required  this.rowNumber,

     this.column,

    required  this.code,
  });

      /// CSV satır numarası (başlık 1)
  @JsonKey(
    
    name: r'rowNumber',
    required: true,
    includeIfNull: false,
  )


  final int rowNumber;



  @JsonKey(
    
    name: r'column',
    required: false,
    includeIfNull: false,
  )


  final String? column;



      /// \"telefon okunamadı\" vb.
  @JsonKey(
    
    name: r'code',
    required: true,
    includeIfNull: false,
  unknownEnumValue: CustomerImportRowErrorCodeEnum.unknownDefaultOpenApi,
  )


  final CustomerImportRowErrorCodeEnum code;





    @override
    bool operator ==(Object other) => identical(this, other) || other is CustomerImportRowError &&
      other.rowNumber == rowNumber &&
      other.column == column &&
      other.code == code;

    @override
    int get hashCode =>
        rowNumber.hashCode +
        (column == null ? 0 : column.hashCode) +
        code.hashCode;

  factory CustomerImportRowError.fromJson(Map<String, dynamic> json) => _$CustomerImportRowErrorFromJson(json);

  Map<String, dynamic> toJson() => _$CustomerImportRowErrorToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

/// \"telefon okunamadı\" vb.
enum CustomerImportRowErrorCodeEnum {
@JsonValue(r'invalid_phone')
invalidPhone(r'invalid_phone'),
@JsonValue(r'missing_name')
missingName(r'missing_name'),
@JsonValue(r'duplicate_in_file')
duplicateInFile(r'duplicate_in_file'),
@JsonValue(r'too_long')
tooLong(r'too_long'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const CustomerImportRowErrorCodeEnum(this.value);

final String value;

@override
String toString() => value;
}


