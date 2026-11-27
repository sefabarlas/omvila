//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'data_export_request_result.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class DataExportRequestResult {
  /// Returns a new [DataExportRequestResult] instance.
  DataExportRequestResult({

    required  this.exportId,

    required  this.status,

    required  this.requestedUtc,

    required  this.emailMasked,

     this.linkExpiresUtc,
  });

      /// İstek kimliği
  @JsonKey(
    
    name: r'exportId',
    required: true,
    includeIfNull: false,
  )


  final String exportId;



      /// İsteğin durumu
  @JsonKey(
    
    name: r'status',
    required: true,
    includeIfNull: false,
  unknownEnumValue: DataExportRequestResultStatusEnum.unknownDefaultOpenApi,
  )


  final DataExportRequestResultStatusEnum status;



      /// İstek anı
  @JsonKey(
    
    name: r'requestedUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime requestedUtc;



      /// Bağlantının gideceği maskeli adres
  @JsonKey(
    
    name: r'emailMasked',
    required: true,
    includeIfNull: false,
  )


  final String emailMasked;



      /// İndirme bağlantısının geçerlilik sonu; arşiv hazırlanıp bağlantı e-postayla gidince (status = Sent) dolar, öncesinde alan dönmez
  @JsonKey(
    
    name: r'linkExpiresUtc',
    required: false,
    includeIfNull: false,
  )


  final DateTime? linkExpiresUtc;





    @override
    bool operator ==(Object other) => identical(this, other) || other is DataExportRequestResult &&
      other.exportId == exportId &&
      other.status == status &&
      other.requestedUtc == requestedUtc &&
      other.emailMasked == emailMasked &&
      other.linkExpiresUtc == linkExpiresUtc;

    @override
    int get hashCode =>
        exportId.hashCode +
        status.hashCode +
        requestedUtc.hashCode +
        emailMasked.hashCode +
        linkExpiresUtc.hashCode;

  factory DataExportRequestResult.fromJson(Map<String, dynamic> json) => _$DataExportRequestResultFromJson(json);

  Map<String, dynamic> toJson() => _$DataExportRequestResultToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

/// İsteğin durumu
enum DataExportRequestResultStatusEnum {
@JsonValue(r'Queued')
queued(r'Queued'),
@JsonValue(r'Processing')
processing(r'Processing'),
@JsonValue(r'Sent')
sent(r'Sent'),
@JsonValue(r'Failed')
failed(r'Failed'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const DataExportRequestResultStatusEnum(this.value);

final String value;

@override
String toString() => value;
}


