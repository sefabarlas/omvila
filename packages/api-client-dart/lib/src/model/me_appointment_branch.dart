//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'me_appointment_branch.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class MeAppointmentBranch {
  /// Returns a new [MeAppointmentBranch] instance.
  MeAppointmentBranch({

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



      /// \"Ara\"
  @JsonKey(
    
    name: r'phone',
    required: false,
    includeIfNull: false,
  )


  final String? phone;



      /// \"Yol tarifi\"
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
    bool operator ==(Object other) => identical(this, other) || other is MeAppointmentBranch &&
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

  factory MeAppointmentBranch.fromJson(Map<String, dynamic> json) => _$MeAppointmentBranchFromJson(json);

  Map<String, dynamic> toJson() => _$MeAppointmentBranchToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

