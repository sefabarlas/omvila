//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/me_branch_ref.dart';
import 'package:omvila_api/src/model/me_brand_package_badge.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'me_brand.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class MeBrand {
  /// Returns a new [MeBrand] instance.
  MeBrand({

    required  this.id,

    required  this.name,

    required  this.slug,

    required  this.logoUrl,

    required  this.coverColor,

    required  this.bookingEnabled,

    required  this.hasBooked,

    required  this.savedUtc,

    required  this.lastVisitOn,

    required  this.branches,

    required  this.activePackages,
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
    
    name: r'logoUrl',
    required: true,
    includeIfNull: true,
  )


  final String? logoUrl;



      /// Rozet zemini (rezervasyon sayfası kapak rengi)
  @JsonKey(
    
    name: r'coverColor',
    required: true,
    includeIfNull: false,
  unknownEnumValue: MeBrandCoverColorEnum.unknownDefaultOpenApi,
  )


  final MeBrandCoverColorEnum coverColor;



      /// false ise \"Online randevu kapalı · Salonu arayın\"
  @JsonKey(
    
    name: r'bookingEnabled',
    required: true,
    includeIfNull: false,
  )


  final bool bookingEnabled;



      /// Bu markada müşteri kaydı (randevu) var
  @JsonKey(
    
    name: r'hasBooked',
    required: true,
    includeIfNull: false,
  )


  final bool hasBooked;



      /// QR veya bağlantıyla eklendiği an (CustomerSavedBrand); yalnız randevuyla gelen markada null
  @JsonKey(
    
    name: r'savedUtc',
    required: true,
    includeIfNull: true,
  )


  final DateTime? savedUtc;



      /// \"son ziyaret 12 Kas\" (son tamamlanan randevu, şubenin yerel günü)
  @JsonKey(
    
    name: r'lastVisitOn',
    required: true,
    includeIfNull: true,
  )
  @DateOnlyConverter()


  final DateTime? lastVisitOn;



      /// Şubeler (\"Kadıköy\", \"Online ve Şişli\")
  @JsonKey(
    
    name: r'branches',
    required: true,
    includeIfNull: false,
  )


  final List<MeBranchRef> branches;



  @JsonKey(
    
    name: r'activePackages',
    required: true,
    includeIfNull: false,
  )


  final List<MeBrandPackageBadge> activePackages;





    @override
    bool operator ==(Object other) => identical(this, other) || other is MeBrand &&
      other.id == id &&
      other.name == name &&
      other.slug == slug &&
      other.logoUrl == logoUrl &&
      other.coverColor == coverColor &&
      other.bookingEnabled == bookingEnabled &&
      other.hasBooked == hasBooked &&
      other.savedUtc == savedUtc &&
      other.lastVisitOn == lastVisitOn &&
      other.branches == branches &&
      other.activePackages == activePackages;

    @override
    int get hashCode =>
        id.hashCode +
        name.hashCode +
        slug.hashCode +
        (logoUrl == null ? 0 : logoUrl.hashCode) +
        coverColor.hashCode +
        bookingEnabled.hashCode +
        hasBooked.hashCode +
        (savedUtc == null ? 0 : savedUtc.hashCode) +
        (lastVisitOn == null ? 0 : lastVisitOn.hashCode) +
        branches.hashCode +
        activePackages.hashCode;

  factory MeBrand.fromJson(Map<String, dynamic> json) => _$MeBrandFromJson(json);

  Map<String, dynamic> toJson() => _$MeBrandToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

/// Rozet zemini (rezervasyon sayfası kapak rengi)
enum MeBrandCoverColorEnum {
@JsonValue(r'petrol')
petrol(r'petrol'),
@JsonValue(r'mercan')
mercan(r'mercan'),
@JsonValue(r'kum')
kum(r'kum'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const MeBrandCoverColorEnum(this.value);

final String value;

@override
String toString() => value;
}


