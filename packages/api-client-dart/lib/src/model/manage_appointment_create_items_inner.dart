//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'manage_appointment_create_items_inner.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ManageAppointmentCreateItemsInner {
  /// Returns a new [ManageAppointmentCreateItemsInner] instance.
  ManageAppointmentCreateItemsInner({

    required  this.serviceId,

    required  this.staffId,
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
    required: true,
    includeIfNull: true,
  )


  final String? staffId;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ManageAppointmentCreateItemsInner &&
      other.serviceId == serviceId &&
      other.staffId == staffId;

    @override
    int get hashCode =>
        serviceId.hashCode +
        (staffId == null ? 0 : staffId.hashCode);

  factory ManageAppointmentCreateItemsInner.fromJson(Map<String, dynamic> json) => _$ManageAppointmentCreateItemsInnerFromJson(json);

  Map<String, dynamic> toJson() => _$ManageAppointmentCreateItemsInnerToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

