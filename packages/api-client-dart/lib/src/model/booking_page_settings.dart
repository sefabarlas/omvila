//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'booking_page_settings.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class BookingPageSettings {
  /// Returns a new [BookingPageSettings] instance.
  BookingPageSettings({

    required  this.slug,

    required  this.url,

    required  this.coverColor,

    required  this.showReviews,

    required  this.showPrices,

     this.preAppointmentNote,

    required  this.version,
  });

  @JsonKey(
    
    name: r'slug',
    required: true,
    includeIfNull: false,
  )


  final String slug;



  @JsonKey(
    
    name: r'url',
    required: true,
    includeIfNull: false,
  )


  final String url;



  @JsonKey(
    
    name: r'coverColor',
    required: true,
    includeIfNull: false,
  unknownEnumValue: BookingPageSettingsCoverColorEnum.unknownDefaultOpenApi,
  )


  final BookingPageSettingsCoverColorEnum coverColor;



  @JsonKey(
    
    name: r'showReviews',
    required: true,
    includeIfNull: false,
  )


  final bool showReviews;



  @JsonKey(
    
    name: r'showPrices',
    required: true,
    includeIfNull: false,
  )


  final bool showPrices;



  @JsonKey(
    
    name: r'preAppointmentNote',
    required: false,
    includeIfNull: false,
  )


  final String? preAppointmentNote;



      /// Kaynak sürümü (ETag ile aynı değer)
  @JsonKey(
    
    name: r'version',
    required: true,
    includeIfNull: false,
  )


  final String version;





    @override
    bool operator ==(Object other) => identical(this, other) || other is BookingPageSettings &&
      other.slug == slug &&
      other.url == url &&
      other.coverColor == coverColor &&
      other.showReviews == showReviews &&
      other.showPrices == showPrices &&
      other.preAppointmentNote == preAppointmentNote &&
      other.version == version;

    @override
    int get hashCode =>
        slug.hashCode +
        url.hashCode +
        coverColor.hashCode +
        showReviews.hashCode +
        showPrices.hashCode +
        (preAppointmentNote == null ? 0 : preAppointmentNote.hashCode) +
        version.hashCode;

  factory BookingPageSettings.fromJson(Map<String, dynamic> json) => _$BookingPageSettingsFromJson(json);

  Map<String, dynamic> toJson() => _$BookingPageSettingsToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

enum BookingPageSettingsCoverColorEnum {
@JsonValue(r'petrol')
petrol(r'petrol'),
@JsonValue(r'mercan')
mercan(r'mercan'),
@JsonValue(r'kum')
kum(r'kum'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const BookingPageSettingsCoverColorEnum(this.value);

final String value;

@override
String toString() => value;
}


