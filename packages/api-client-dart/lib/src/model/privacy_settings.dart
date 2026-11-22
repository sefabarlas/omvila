//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/privacy_settings_standard_notice.dart';
import 'package:omvila_api/src/model/privacy_notice_mode.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'privacy_settings.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PrivacySettings {
  /// Returns a new [PrivacySettings] instance.
  PrivacySettings({

    required  this.noticeMode,

    required  this.standardNotice,

    required  this.customNoticeText,

    required  this.customNoticeUpdatedUtc,

    required  this.transactionRetentionYears,

    required  this.allowedRetentionYears,

    required  this.version,
  });

  @JsonKey(
    
    name: r'noticeMode',
    required: true,
    includeIfNull: false,
  unknownEnumValue: PrivacyNoticeMode.unknownDefaultOpenApi,
  )


  final PrivacyNoticeMode noticeMode;



  @JsonKey(
    
    name: r'standardNotice',
    required: true,
    includeIfNull: false,
  )


  final PrivacySettingsStandardNotice standardNotice;



      /// Salonun kendi metni; yoksa null
  @JsonKey(
    
    name: r'customNoticeText',
    required: true,
    includeIfNull: true,
  )


  final String? customNoticeText;



      /// Salon metninin son değiştiği an
  @JsonKey(
    
    name: r'customNoticeUpdatedUtc',
    required: true,
    includeIfNull: true,
  )


  final DateTime? customNoticeUpdatedUtc;



      /// \"İşlem kayıtlarını saklama süresi\" (TransactionRetentionYears)
  @JsonKey(
    
    name: r'transactionRetentionYears',
    required: true,
    includeIfNull: false,
  )


  final int transactionRetentionYears;



      /// Seçilebilir süreler (yıl)
  @JsonKey(
    
    name: r'allowedRetentionYears',
    required: true,
    includeIfNull: false,
  )


  final List<int> allowedRetentionYears;



      /// Kaynak sürümü (ETag ile aynı değer)
  @JsonKey(
    
    name: r'version',
    required: true,
    includeIfNull: false,
  )


  final String version;





    @override
    bool operator ==(Object other) => identical(this, other) || other is PrivacySettings &&
      other.noticeMode == noticeMode &&
      other.standardNotice == standardNotice &&
      other.customNoticeText == customNoticeText &&
      other.customNoticeUpdatedUtc == customNoticeUpdatedUtc &&
      other.transactionRetentionYears == transactionRetentionYears &&
      other.allowedRetentionYears == allowedRetentionYears &&
      other.version == version;

    @override
    int get hashCode =>
        noticeMode.hashCode +
        standardNotice.hashCode +
        (customNoticeText == null ? 0 : customNoticeText.hashCode) +
        (customNoticeUpdatedUtc == null ? 0 : customNoticeUpdatedUtc.hashCode) +
        transactionRetentionYears.hashCode +
        allowedRetentionYears.hashCode +
        version.hashCode;

  factory PrivacySettings.fromJson(Map<String, dynamic> json) => _$PrivacySettingsFromJson(json);

  Map<String, dynamic> toJson() => _$PrivacySettingsToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

