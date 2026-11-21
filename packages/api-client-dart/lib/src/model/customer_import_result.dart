//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'customer_import_result.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class CustomerImportResult {
  /// Returns a new [CustomerImportResult] instance.
  CustomerImportResult({

    required  this.importId,

    required  this.status,

    required  this.created,

    required  this.merged,

    required  this.skipped,
  });

  @JsonKey(
    
    name: r'importId',
    required: true,
    includeIfNull: false,
  )


  final String importId;



  @JsonKey(
    
    name: r'status',
    required: true,
    includeIfNull: false,
  unknownEnumValue: CustomerImportResultStatusEnum.unknownDefaultOpenApi,
  )


  final CustomerImportResultStatusEnum status;



  @JsonKey(
    
    name: r'created',
    required: true,
    includeIfNull: false,
  )


  final int created;



  @JsonKey(
    
    name: r'merged',
    required: true,
    includeIfNull: false,
  )


  final int merged;



  @JsonKey(
    
    name: r'skipped',
    required: true,
    includeIfNull: false,
  )


  final int skipped;





    @override
    bool operator ==(Object other) => identical(this, other) || other is CustomerImportResult &&
      other.importId == importId &&
      other.status == status &&
      other.created == created &&
      other.merged == merged &&
      other.skipped == skipped;

    @override
    int get hashCode =>
        importId.hashCode +
        status.hashCode +
        created.hashCode +
        merged.hashCode +
        skipped.hashCode;

  factory CustomerImportResult.fromJson(Map<String, dynamic> json) => _$CustomerImportResultFromJson(json);

  Map<String, dynamic> toJson() => _$CustomerImportResultToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

enum CustomerImportResultStatusEnum {
@JsonValue(r'Committed')
committed(r'Committed'),
@JsonValue(r'Failed')
failed(r'Failed'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const CustomerImportResultStatusEnum(this.value);

final String value;

@override
String toString() => value;
}


