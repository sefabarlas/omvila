//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'me_save_brand_request.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class MeSaveBrandRequest {
  /// Returns a new [MeSaveBrandRequest] instance.
  MeSaveBrandRequest({

    required  this.brandSlug,

    required  this.source_,
  });

  @JsonKey(
    
    name: r'brandSlug',
    required: true,
    includeIfNull: false,
  )


  final String brandSlug;



      /// CustomerSavedBrand.Source (Booking yalnız sunucu yazar)
  @JsonKey(
    
    name: r'source',
    required: true,
    includeIfNull: false,
  unknownEnumValue: MeSaveBrandRequestSource_Enum.unknownDefaultOpenApi,
  )


  final MeSaveBrandRequestSource_Enum source_;





    @override
    bool operator ==(Object other) => identical(this, other) || other is MeSaveBrandRequest &&
      other.brandSlug == brandSlug &&
      other.source_ == source_;

    @override
    int get hashCode =>
        brandSlug.hashCode +
        source_.hashCode;

  factory MeSaveBrandRequest.fromJson(Map<String, dynamic> json) => _$MeSaveBrandRequestFromJson(json);

  Map<String, dynamic> toJson() => _$MeSaveBrandRequestToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

/// CustomerSavedBrand.Source (Booking yalnız sunucu yazar)
enum MeSaveBrandRequestSource_Enum {
@JsonValue(r'Qr')
qr(r'Qr'),
@JsonValue(r'Link')
link(r'Link'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const MeSaveBrandRequestSource_Enum(this.value);

final String value;

@override
String toString() => value;
}


