//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'public_appointment_branch.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PublicAppointmentBranch {
  /// Returns a new [PublicAppointmentBranch] instance.
  PublicAppointmentBranch({

    required  this.id,

    required  this.name,

    required  this.addressLine,

    required  this.district,

     this.phone,

     this.latitude,

     this.longitude,
  });

  @JsonKey(
    
    name: r'id',
    required: true,
    includeIfNull: false,
  )


  final String id;



  @JsonKey(
    
    name: r'name',
    required: true,
    includeIfNull: false,
  )


  final String name;



  @JsonKey(
    
    name: r'addressLine',
    required: true,
    includeIfNull: false,
  )


  final String addressLine;



  @JsonKey(
    
    name: r'district',
    required: true,
    includeIfNull: false,
  )


  final String district;



  @JsonKey(
    
    name: r'phone',
    required: false,
    includeIfNull: false,
  )


  final String? phone;



  @JsonKey(
    
    name: r'latitude',
    required: false,
    includeIfNull: false,
  )


  final num? latitude;



  @JsonKey(
    
    name: r'longitude',
    required: false,
    includeIfNull: false,
  )


  final num? longitude;





    @override
    bool operator ==(Object other) => identical(this, other) || other is PublicAppointmentBranch &&
      other.id == id &&
      other.name == name &&
      other.addressLine == addressLine &&
      other.district == district &&
      other.phone == phone &&
      other.latitude == latitude &&
      other.longitude == longitude;

    @override
    int get hashCode =>
        id.hashCode +
        name.hashCode +
        addressLine.hashCode +
        district.hashCode +
        (phone == null ? 0 : phone.hashCode) +
        (latitude == null ? 0 : latitude.hashCode) +
        (longitude == null ? 0 : longitude.hashCode);

  factory PublicAppointmentBranch.fromJson(Map<String, dynamic> json) => _$PublicAppointmentBranchFromJson(json);

  Map<String, dynamic> toJson() => _$PublicAppointmentBranchToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

