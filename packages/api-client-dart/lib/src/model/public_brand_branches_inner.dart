//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'public_brand_branches_inner.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PublicBrandBranchesInner {
  /// Returns a new [PublicBrandBranchesInner] instance.
  PublicBrandBranchesInner({

    required  this.id,

    required  this.name,

    required  this.addressLine,

    required  this.district,

    required  this.city,

     this.phone,

     this.latitude,

     this.longitude,

    required  this.timeZone,

    required  this.openNow,

     this.todayClosesAt,

     this.nextOpeningUtc,
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
    
    name: r'city',
    required: true,
    includeIfNull: false,
  )


  final String city;



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



      /// IANA saat dilimi
  @JsonKey(
    
    name: r'timeZone',
    required: true,
    includeIfNull: false,
  )


  final String timeZone;



  @JsonKey(
    
    name: r'openNow',
    required: true,
    includeIfNull: false,
  )


  final bool openNow;



      /// Yerel saat HH:mm (şubenin saat dilimine göre)
  @JsonKey(
    
    name: r'todayClosesAt',
    required: false,
    includeIfNull: false,
  )


  final String? todayClosesAt;



      /// Bugün kapalıysa (\"Bugün kapalı · Yarın 10:00'da açılır\")
  @JsonKey(
    
    name: r'nextOpeningUtc',
    required: false,
    includeIfNull: false,
  )


  final DateTime? nextOpeningUtc;





    @override
    bool operator ==(Object other) => identical(this, other) || other is PublicBrandBranchesInner &&
      other.id == id &&
      other.name == name &&
      other.addressLine == addressLine &&
      other.district == district &&
      other.city == city &&
      other.phone == phone &&
      other.latitude == latitude &&
      other.longitude == longitude &&
      other.timeZone == timeZone &&
      other.openNow == openNow &&
      other.todayClosesAt == todayClosesAt &&
      other.nextOpeningUtc == nextOpeningUtc;

    @override
    int get hashCode =>
        id.hashCode +
        name.hashCode +
        addressLine.hashCode +
        district.hashCode +
        city.hashCode +
        (phone == null ? 0 : phone.hashCode) +
        (latitude == null ? 0 : latitude.hashCode) +
        (longitude == null ? 0 : longitude.hashCode) +
        timeZone.hashCode +
        openNow.hashCode +
        (todayClosesAt == null ? 0 : todayClosesAt.hashCode) +
        (nextOpeningUtc == null ? 0 : nextOpeningUtc.hashCode);

  factory PublicBrandBranchesInner.fromJson(Map<String, dynamic> json) => _$PublicBrandBranchesInnerFromJson(json);

  Map<String, dynamic> toJson() => _$PublicBrandBranchesInnerToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

