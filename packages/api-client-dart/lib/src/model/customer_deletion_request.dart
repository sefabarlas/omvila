//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'customer_deletion_request.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class CustomerDeletionRequest {
  /// Returns a new [CustomerDeletionRequest] instance.
  CustomerDeletionRequest({

    required  this.confirmFullName,
  });

      /// \"Onaylamak için müşterinin adını yazın\"; ad ile birebir (büyük/küçük harf ve baştaki/sondaki boşluk yok sayılarak) eşleşmeli
  @JsonKey(
    
    name: r'confirmFullName',
    required: true,
    includeIfNull: false,
  )


  final String confirmFullName;





    @override
    bool operator ==(Object other) => identical(this, other) || other is CustomerDeletionRequest &&
      other.confirmFullName == confirmFullName;

    @override
    int get hashCode =>
        confirmFullName.hashCode;

  factory CustomerDeletionRequest.fromJson(Map<String, dynamic> json) => _$CustomerDeletionRequestFromJson(json);

  Map<String, dynamic> toJson() => _$CustomerDeletionRequestToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

