//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'data_export_request.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class DataExportRequest {
  /// Returns a new [DataExportRequest] instance.
  DataExportRequest({

     this.email,
  });

      /// Bağlantının gideceği adres; verilmezse kullanıcının kayıtlı e-postası
  @JsonKey(
    
    name: r'email',
    required: false,
    includeIfNull: false,
  )


  final String? email;





    @override
    bool operator ==(Object other) => identical(this, other) || other is DataExportRequest &&
      other.email == email;

    @override
    int get hashCode =>
        email.hashCode;

  factory DataExportRequest.fromJson(Map<String, dynamic> json) => _$DataExportRequestFromJson(json);

  Map<String, dynamic> toJson() => _$DataExportRequestToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

