//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'sms_sender_request.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class SmsSenderRequest {
  /// Returns a new [SmsSenderRequest] instance.
  SmsSenderRequest({

    required  this.name,
  });

      /// 3–11 karakter, büyük Latin harfi ve rakam, en az bir harf (operatör başlık kuralı)
  @JsonKey(
    
    name: r'name',
    required: true,
    includeIfNull: false,
  )


  final String name;





    @override
    bool operator ==(Object other) => identical(this, other) || other is SmsSenderRequest &&
      other.name == name;

    @override
    int get hashCode =>
        name.hashCode;

  factory SmsSenderRequest.fromJson(Map<String, dynamic> json) => _$SmsSenderRequestFromJson(json);

  Map<String, dynamic> toJson() => _$SmsSenderRequestToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

