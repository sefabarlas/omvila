//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/me_language.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'me_profile.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class MeProfile {
  /// Returns a new [MeProfile] instance.
  MeProfile({

    required  this.id,

    required  this.fullName,

    required  this.reviewerDisplayName,

    required  this.phone,

    required  this.email,

    required  this.language,

    required  this.version,
  });

      /// Customer kimliği
  @JsonKey(
    
    name: r'id',
    required: true,
    includeIfNull: false,
  )


  final String id;



      /// Ad soyad; henüz verilmediyse null
  @JsonKey(
    
    name: r'fullName',
    required: true,
    includeIfNull: true,
  )


  final String? fullName;



      /// Değerlendirmede görünen ad (\"Merve Y.\"); ad yoksa null
  @JsonKey(
    
    name: r'reviewerDisplayName',
    required: true,
    includeIfNull: true,
  )


  final String? reviewerDisplayName;



      /// E.164 telefon
  @JsonKey(
    
    name: r'phone',
    required: true,
    includeIfNull: false,
  )


  final String phone;



      /// \"Makbuz ve hatırlatma için\" (isteğe bağlı)
  @JsonKey(
    
    name: r'email',
    required: true,
    includeIfNull: true,
  )


  final String? email;



  @JsonKey(
    
    name: r'language',
    required: true,
    includeIfNull: false,
  unknownEnumValue: MeLanguage.unknownDefaultOpenApi,
  )


  final MeLanguage language;



      /// Kaynak sürümü (ETag ile aynı değer)
  @JsonKey(
    
    name: r'version',
    required: true,
    includeIfNull: false,
  )


  final String version;





    @override
    bool operator ==(Object other) => identical(this, other) || other is MeProfile &&
      other.id == id &&
      other.fullName == fullName &&
      other.reviewerDisplayName == reviewerDisplayName &&
      other.phone == phone &&
      other.email == email &&
      other.language == language &&
      other.version == version;

    @override
    int get hashCode =>
        id.hashCode +
        (fullName == null ? 0 : fullName.hashCode) +
        (reviewerDisplayName == null ? 0 : reviewerDisplayName.hashCode) +
        phone.hashCode +
        (email == null ? 0 : email.hashCode) +
        language.hashCode +
        version.hashCode;

  factory MeProfile.fromJson(Map<String, dynamic> json) => _$MeProfileFromJson(json);

  Map<String, dynamic> toJson() => _$MeProfileToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

