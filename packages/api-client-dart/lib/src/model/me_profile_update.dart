//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/me_language.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'me_profile_update.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class MeProfileUpdate {
  /// Returns a new [MeProfileUpdate] instance.
  MeProfileUpdate({

     this.fullName,

     this.email,

     this.language,
  });

      /// Ad soyad
  @JsonKey(
    
    name: r'fullName',
    required: false,
    includeIfNull: false,
  )


  final String? fullName;



      /// null = e-postayı sil
  @JsonKey(
    
    name: r'email',
    required: false,
    includeIfNull: false,
  )


  final String? email;



  @JsonKey(
    
    name: r'language',
    required: false,
    includeIfNull: false,
  unknownEnumValue: MeLanguage.unknownDefaultOpenApi,
  )


  final MeLanguage? language;





    @override
    bool operator ==(Object other) => identical(this, other) || other is MeProfileUpdate &&
      other.fullName == fullName &&
      other.email == email &&
      other.language == language;

    @override
    int get hashCode =>
        fullName.hashCode +
        (email == null ? 0 : email.hashCode) +
        language.hashCode;

  factory MeProfileUpdate.fromJson(Map<String, dynamic> json) => _$MeProfileUpdateFromJson(json);

  Map<String, dynamic> toJson() => _$MeProfileUpdateToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

