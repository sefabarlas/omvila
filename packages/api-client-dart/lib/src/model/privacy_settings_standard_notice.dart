//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'privacy_settings_standard_notice.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PrivacySettingsStandardNotice {
  /// Returns a new [PrivacySettingsStandardNotice] instance.
  PrivacySettingsStandardNotice({

    required  this.version,

    required  this.updatedOn,

    required  this.text,
  });

      /// PrivacyNoticeVersion
  @JsonKey(
    
    name: r'version',
    required: true,
    includeIfNull: false,
  )


  final String version;



      /// Son güncelleme günü
  @JsonKey(
    
    name: r'updatedOn',
    required: true,
    includeIfNull: false,
  )
  @DateOnlyConverter()


  final DateTime updatedOn;



      /// Markanın adı ve iletişim bilgisiyle doldurulmuş standart metin (düz metin)
  @JsonKey(
    
    name: r'text',
    required: true,
    includeIfNull: false,
  )


  final String text;





    @override
    bool operator ==(Object other) => identical(this, other) || other is PrivacySettingsStandardNotice &&
      other.version == version &&
      other.updatedOn == updatedOn &&
      other.text == text;

    @override
    int get hashCode =>
        version.hashCode +
        updatedOn.hashCode +
        text.hashCode;

  factory PrivacySettingsStandardNotice.fromJson(Map<String, dynamic> json) => _$PrivacySettingsStandardNoticeFromJson(json);

  Map<String, dynamic> toJson() => _$PrivacySettingsStandardNoticeToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

