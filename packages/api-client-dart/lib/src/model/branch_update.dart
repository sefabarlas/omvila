//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'branch_update.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class BranchUpdate {
  /// Returns a new [BranchUpdate] instance.
  BranchUpdate({

     this.name,

     this.addressLine,

     this.district,

     this.city,

     this.postalCode,

     this.latitude,

     this.longitude,

     this.phone,

     this.email,

     this.slotIntervalMinutes,

     this.isActive,
  });

  @JsonKey(
    
    name: r'name',
    required: false,
    includeIfNull: false,
  )


  final String? name;



  @JsonKey(
    
    name: r'addressLine',
    required: false,
    includeIfNull: false,
  )


  final String? addressLine;



  @JsonKey(
    
    name: r'district',
    required: false,
    includeIfNull: false,
  )


  final String? district;



  @JsonKey(
    
    name: r'city',
    required: false,
    includeIfNull: false,
  )


  final String? city;



  @JsonKey(
    
    name: r'postalCode',
    required: false,
    includeIfNull: true,
  )


  final String? postalCode;



  @JsonKey(
    
    name: r'latitude',
    required: false,
    includeIfNull: true,
  )


  final num? latitude;



  @JsonKey(
    
    name: r'longitude',
    required: false,
    includeIfNull: true,
  )


  final num? longitude;



  @JsonKey(
    
    name: r'phone',
    required: false,
    includeIfNull: true,
  )


  final String? phone;



  @JsonKey(
    
    name: r'email',
    required: false,
    includeIfNull: true,
  )


  final String? email;



  @JsonKey(
    
    name: r'slotIntervalMinutes',
    required: false,
    includeIfNull: true,
  unknownEnumValue: BranchUpdateSlotIntervalMinutesEnum.unknownDefaultOpenApi,
  )


  final BranchUpdateSlotIntervalMinutesEnum? slotIntervalMinutes;



  @JsonKey(
    
    name: r'isActive',
    required: false,
    includeIfNull: false,
  )


  final bool? isActive;





    @override
    bool operator ==(Object other) => identical(this, other) || other is BranchUpdate &&
      other.name == name &&
      other.addressLine == addressLine &&
      other.district == district &&
      other.city == city &&
      other.postalCode == postalCode &&
      other.latitude == latitude &&
      other.longitude == longitude &&
      other.phone == phone &&
      other.email == email &&
      other.slotIntervalMinutes == slotIntervalMinutes &&
      other.isActive == isActive;

    @override
    int get hashCode =>
        name.hashCode +
        addressLine.hashCode +
        district.hashCode +
        city.hashCode +
        (postalCode == null ? 0 : postalCode.hashCode) +
        (latitude == null ? 0 : latitude.hashCode) +
        (longitude == null ? 0 : longitude.hashCode) +
        (phone == null ? 0 : phone.hashCode) +
        (email == null ? 0 : email.hashCode) +
        (slotIntervalMinutes == null ? 0 : slotIntervalMinutes.hashCode) +
        isActive.hashCode;

  factory BranchUpdate.fromJson(Map<String, dynamic> json) => _$BranchUpdateFromJson(json);

  Map<String, dynamic> toJson() => _$BranchUpdateToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

enum BranchUpdateSlotIntervalMinutesEnum {
@JsonValue(5)
number5('5'),
@JsonValue(10)
number10('10'),
@JsonValue(15)
number15('15'),
@JsonValue(11184809)
unknownDefaultOpenApi('11184809');

const BranchUpdateSlotIntervalMinutesEnum(this.value);

final String value;

@override
String toString() => value;
}


