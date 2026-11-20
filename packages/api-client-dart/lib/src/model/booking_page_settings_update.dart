//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'booking_page_settings_update.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class BookingPageSettingsUpdate {
  /// Returns a new [BookingPageSettingsUpdate] instance.
  BookingPageSettingsUpdate({

    required  this.slug,

    required  this.coverColor,

    required  this.showReviews,

    required  this.showPrices,

     this.preAppointmentNote,
  });

  @JsonKey(
    
    name: r'slug',
    required: true,
    includeIfNull: false,
  )


  final String slug;



  @JsonKey(
    
    name: r'coverColor',
    required: true,
    includeIfNull: false,
  unknownEnumValue: BookingPageSettingsUpdateCoverColorEnum.unknownDefaultOpenApi,
  )


  final BookingPageSettingsUpdateCoverColorEnum coverColor;



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





    @override
    bool operator ==(Object other) => identical(this, other) || other is BookingPageSettingsUpdate &&
      other.slug == slug &&
      other.coverColor == coverColor &&
      other.showReviews == showReviews &&
      other.showPrices == showPrices &&
      other.preAppointmentNote == preAppointmentNote;

    @override
    int get hashCode =>
        slug.hashCode +
        coverColor.hashCode +
        showReviews.hashCode +
        showPrices.hashCode +
        (preAppointmentNote == null ? 0 : preAppointmentNote.hashCode);

  factory BookingPageSettingsUpdate.fromJson(Map<String, dynamic> json) => _$BookingPageSettingsUpdateFromJson(json);

  Map<String, dynamic> toJson() => _$BookingPageSettingsUpdateToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

enum BookingPageSettingsUpdateCoverColorEnum {
@JsonValue(r'petrol')
petrol(r'petrol'),
@JsonValue(r'mercan')
mercan(r'mercan'),
@JsonValue(r'kum')
kum(r'kum'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const BookingPageSettingsUpdateCoverColorEnum(this.value);

final String value;

@override
String toString() => value;
}


