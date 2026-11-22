//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'customer_import_commit_request.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class CustomerImportCommitRequest {
  /// Returns a new [CustomerImportCommitRequest] instance.
  CustomerImportCommitRequest({

     this.sendInfoSms = false,
  });

      /// \"İçe aktarılanlara randevu bilgilendirme SMS'i gönderilebilir\" (kampanya izni içe aktarılmaz)
  @JsonKey(
    defaultValue: false,
    name: r'sendInfoSms',
    required: false,
    includeIfNull: false,
  )


  final bool? sendInfoSms;





    @override
    bool operator ==(Object other) => identical(this, other) || other is CustomerImportCommitRequest &&
      other.sendInfoSms == sendInfoSms;

    @override
    int get hashCode =>
        sendInfoSms.hashCode;

  factory CustomerImportCommitRequest.fromJson(Map<String, dynamic> json) => _$CustomerImportCommitRequestFromJson(json);

  Map<String, dynamic> toJson() => _$CustomerImportCommitRequestToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

