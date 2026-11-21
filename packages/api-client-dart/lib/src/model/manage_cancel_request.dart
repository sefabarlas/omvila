//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'manage_cancel_request.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ManageCancelRequest {
  /// Returns a new [ManageCancelRequest] instance.
  ManageCancelRequest({

    required  this.reason,

     this.note,

     this.scope,

     this.notifyCustomer = true,

     this.offerToWaitlist = true,
  });

  @JsonKey(
    
    name: r'reason',
    required: true,
    includeIfNull: false,
  unknownEnumValue: ManageCancelRequestReasonEnum.unknownDefaultOpenApi,
  )


  final ManageCancelRequestReasonEnum reason;



  @JsonKey(
    
    name: r'note',
    required: false,
    includeIfNull: false,
  )


  final String? note;



  @JsonKey(
    
    name: r'scope',
    required: false,
    includeIfNull: false,
  unknownEnumValue: ManageCancelRequestScopeEnum.unknownDefaultOpenApi,
  )


  final ManageCancelRequestScopeEnum? scope;



      /// \"Müşteriye bildir\"
  @JsonKey(
    defaultValue: true,
    name: r'notifyCustomer',
    required: false,
    includeIfNull: false,
  )


  final bool? notifyCustomer;



      /// Boşalan saat bekleme listesindeki sıradaki uygun kişiye teklif edilsin (waitlist.offer, 15 dk tutma); false ise teklif motoru bu saat için çalışmaz
  @JsonKey(
    defaultValue: true,
    name: r'offerToWaitlist',
    required: false,
    includeIfNull: false,
  )


  final bool? offerToWaitlist;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ManageCancelRequest &&
      other.reason == reason &&
      other.note == note &&
      other.scope == scope &&
      other.notifyCustomer == notifyCustomer &&
      other.offerToWaitlist == offerToWaitlist;

    @override
    int get hashCode =>
        reason.hashCode +
        (note == null ? 0 : note.hashCode) +
        scope.hashCode +
        notifyCustomer.hashCode +
        offerToWaitlist.hashCode;

  factory ManageCancelRequest.fromJson(Map<String, dynamic> json) => _$ManageCancelRequestFromJson(json);

  Map<String, dynamic> toJson() => _$ManageCancelRequestToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

enum ManageCancelRequestReasonEnum {
@JsonValue(r'CustomerRequested')
customerRequested(r'CustomerRequested'),
@JsonValue(r'StaffUnavailable')
staffUnavailable(r'StaffUnavailable'),
@JsonValue(r'Other')
other(r'Other'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const ManageCancelRequestReasonEnum(this.value);

final String value;

@override
String toString() => value;
}


enum ManageCancelRequestScopeEnum {
@JsonValue(r'ThisOnly')
thisOnly(r'ThisOnly'),
@JsonValue(r'ThisAndFollowing')
thisAndFollowing(r'ThisAndFollowing'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const ManageCancelRequestScopeEnum(this.value);

final String value;

@override
String toString() => value;
}


