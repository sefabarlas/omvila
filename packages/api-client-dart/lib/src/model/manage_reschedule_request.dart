//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/manage_reschedule_request_items_inner.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'manage_reschedule_request.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ManageRescheduleRequest {
  /// Returns a new [ManageRescheduleRequest] instance.
  ManageRescheduleRequest({

    required  this.startUtc,

     this.items,

     this.scope,

     this.notifyCustomer = true,

     this.allowOutsideWorkingHours = false,
  });

  @JsonKey(
    
    name: r'startUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime startUtc;



      /// Personel değişikliği; verilmezse mevcut personel korunur
  @JsonKey(
    
    name: r'items',
    required: false,
    includeIfNull: false,
  )


  final List<ManageRescheduleRequestItemsInner>? items;



  @JsonKey(
    
    name: r'scope',
    required: false,
    includeIfNull: false,
  unknownEnumValue: ManageRescheduleRequestScopeEnum.unknownDefaultOpenApi,
  )


  final ManageRescheduleRequestScopeEnum? scope;



  @JsonKey(
    defaultValue: true,
    name: r'notifyCustomer',
    required: false,
    includeIfNull: false,
  )


  final bool? notifyCustomer;



  @JsonKey(
    defaultValue: false,
    name: r'allowOutsideWorkingHours',
    required: false,
    includeIfNull: false,
  )


  final bool? allowOutsideWorkingHours;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ManageRescheduleRequest &&
      other.startUtc == startUtc &&
      other.items == items &&
      other.scope == scope &&
      other.notifyCustomer == notifyCustomer &&
      other.allowOutsideWorkingHours == allowOutsideWorkingHours;

    @override
    int get hashCode =>
        startUtc.hashCode +
        items.hashCode +
        scope.hashCode +
        notifyCustomer.hashCode +
        allowOutsideWorkingHours.hashCode;

  factory ManageRescheduleRequest.fromJson(Map<String, dynamic> json) => _$ManageRescheduleRequestFromJson(json);

  Map<String, dynamic> toJson() => _$ManageRescheduleRequestToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

enum ManageRescheduleRequestScopeEnum {
@JsonValue(r'ThisOnly')
thisOnly(r'ThisOnly'),
@JsonValue(r'ThisAndFollowing')
thisAndFollowing(r'ThisAndFollowing'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const ManageRescheduleRequestScopeEnum(this.value);

final String value;

@override
String toString() => value;
}


