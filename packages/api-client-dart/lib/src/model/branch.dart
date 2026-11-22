//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'branch.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class Branch {
  /// Returns a new [Branch] instance.
  Branch({

    required  this.id,

    required  this.name,

    required  this.slug,

    required  this.addressLine,

    required  this.district,

    required  this.city,

     this.postalCode,

     this.latitude,

     this.longitude,

     this.phone,

     this.email,

    required  this.timeZone,

     this.slotIntervalMinutes,

    required  this.isActive,

    required  this.version,
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
    
    name: r'slug',
    required: true,
    includeIfNull: false,
  )


  final String slug;



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
    
    name: r'city',
    required: true,
    includeIfNull: false,
  )


  final String city;



  @JsonKey(
    
    name: r'postalCode',
    required: false,
    includeIfNull: false,
  )


  final String? postalCode;



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



  @JsonKey(
    
    name: r'phone',
    required: false,
    includeIfNull: false,
  )


  final String? phone;



  @JsonKey(
    
    name: r'email',
    required: false,
    includeIfNull: false,
  )


  final String? email;



      /// IANA saat dilimi
  @JsonKey(
    
    name: r'timeZone',
    required: true,
    includeIfNull: false,
  )


  final String timeZone;



  @JsonKey(
    
    name: r'slotIntervalMinutes',
    required: false,
    includeIfNull: false,
  unknownEnumValue: BranchSlotIntervalMinutesEnum.unknownDefaultOpenApi,
  )


  final BranchSlotIntervalMinutesEnum? slotIntervalMinutes;



  @JsonKey(
    
    name: r'isActive',
    required: true,
    includeIfNull: false,
  )


  final bool isActive;



      /// Kaynak sürümü (ETag ile aynı değer)
  @JsonKey(
    
    name: r'version',
    required: true,
    includeIfNull: false,
  )


  final String version;





    @override
    bool operator ==(Object other) => identical(this, other) || other is Branch &&
      other.id == id &&
      other.name == name &&
      other.slug == slug &&
      other.addressLine == addressLine &&
      other.district == district &&
      other.city == city &&
      other.postalCode == postalCode &&
      other.latitude == latitude &&
      other.longitude == longitude &&
      other.phone == phone &&
      other.email == email &&
      other.timeZone == timeZone &&
      other.slotIntervalMinutes == slotIntervalMinutes &&
      other.isActive == isActive &&
      other.version == version;

    @override
    int get hashCode =>
        id.hashCode +
        name.hashCode +
        slug.hashCode +
        addressLine.hashCode +
        district.hashCode +
        city.hashCode +
        (postalCode == null ? 0 : postalCode.hashCode) +
        (latitude == null ? 0 : latitude.hashCode) +
        (longitude == null ? 0 : longitude.hashCode) +
        (phone == null ? 0 : phone.hashCode) +
        (email == null ? 0 : email.hashCode) +
        timeZone.hashCode +
        (slotIntervalMinutes == null ? 0 : slotIntervalMinutes.hashCode) +
        isActive.hashCode +
        version.hashCode;

  factory Branch.fromJson(Map<String, dynamic> json) => _$BranchFromJson(json);

  Map<String, dynamic> toJson() => _$BranchToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

enum BranchSlotIntervalMinutesEnum {
@JsonValue(5)
number5('5'),
@JsonValue(10)
number10('10'),
@JsonValue(15)
number15('15'),
@JsonValue(11184809)
unknownDefaultOpenApi('11184809');

const BranchSlotIntervalMinutesEnum(this.value);

final String value;

@override
String toString() => value;
}


