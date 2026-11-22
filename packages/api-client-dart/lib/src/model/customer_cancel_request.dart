//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'customer_cancel_request.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class CustomerCancelRequest {
  /// Returns a new [CustomerCancelRequest] instance.
  CustomerCancelRequest({

    required  this.reason,

     this.note,
  });

  @JsonKey(
    
    name: r'reason',
    required: true,
    includeIfNull: false,
  unknownEnumValue: CustomerCancelRequestReasonEnum.unknownDefaultOpenApi,
  )


  final CustomerCancelRequestReasonEnum reason;



      /// reason = Other ise zorunlu
  @JsonKey(
    
    name: r'note',
    required: false,
    includeIfNull: false,
  )


  final String? note;





    @override
    bool operator ==(Object other) => identical(this, other) || other is CustomerCancelRequest &&
      other.reason == reason &&
      other.note == note;

    @override
    int get hashCode =>
        reason.hashCode +
        (note == null ? 0 : note.hashCode);

  factory CustomerCancelRequest.fromJson(Map<String, dynamic> json) => _$CustomerCancelRequestFromJson(json);

  Map<String, dynamic> toJson() => _$CustomerCancelRequestToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

enum CustomerCancelRequestReasonEnum {
@JsonValue(r'PlanChanged')
planChanged(r'PlanChanged'),
@JsonValue(r'Sick')
sick(r'Sick'),
@JsonValue(r'WantsDifferentTime')
wantsDifferentTime(r'WantsDifferentTime'),
@JsonValue(r'Other')
other(r'Other'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const CustomerCancelRequestReasonEnum(this.value);

final String value;

@override
String toString() => value;
}


