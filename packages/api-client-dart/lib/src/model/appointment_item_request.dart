//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'appointment_item_request.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AppointmentItemRequest {
  /// Returns a new [AppointmentItemRequest] instance.
  AppointmentItemRequest({

    required  this.serviceId,

     this.staffId,
  });

  @JsonKey(
    
    name: r'serviceId',
    required: true,
    includeIfNull: false,
  )


  final String serviceId;



      /// null = fark etmez (yalnız arama ve online oluşturmada; sunucu atar)
  @JsonKey(
    
    name: r'staffId',
    required: false,
    includeIfNull: true,
  )


  final String? staffId;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AppointmentItemRequest &&
      other.serviceId == serviceId &&
      other.staffId == staffId;

    @override
    int get hashCode =>
        serviceId.hashCode +
        (staffId == null ? 0 : staffId.hashCode);

  factory AppointmentItemRequest.fromJson(Map<String, dynamic> json) => _$AppointmentItemRequestFromJson(json);

  Map<String, dynamic> toJson() => _$AppointmentItemRequestToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

