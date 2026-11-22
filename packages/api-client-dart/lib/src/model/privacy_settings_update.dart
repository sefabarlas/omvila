//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/privacy_notice_mode.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'privacy_settings_update.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PrivacySettingsUpdate {
  /// Returns a new [PrivacySettingsUpdate] instance.
  PrivacySettingsUpdate({

    required  this.noticeMode,

     this.customNoticeText,

    required  this.transactionRetentionYears,
  });

  @JsonKey(
    
    name: r'noticeMode',
    required: true,
    includeIfNull: false,
  unknownEnumValue: PrivacyNoticeMode.unknownDefaultOpenApi,
  )


  final PrivacyNoticeMode noticeMode;



      /// noticeMode = Custom ise zorunlu; gönderilmezse değişmez, null ise silinir
  @JsonKey(
    
    name: r'customNoticeText',
    required: false,
    includeIfNull: true,
  )


  final String? customNoticeText;



      /// 3, 5 veya 10
  @JsonKey(
    
    name: r'transactionRetentionYears',
    required: true,
    includeIfNull: false,
  )


  final int transactionRetentionYears;





    @override
    bool operator ==(Object other) => identical(this, other) || other is PrivacySettingsUpdate &&
      other.noticeMode == noticeMode &&
      other.customNoticeText == customNoticeText &&
      other.transactionRetentionYears == transactionRetentionYears;

    @override
    int get hashCode =>
        noticeMode.hashCode +
        (customNoticeText == null ? 0 : customNoticeText.hashCode) +
        transactionRetentionYears.hashCode;

  factory PrivacySettingsUpdate.fromJson(Map<String, dynamic> json) => _$PrivacySettingsUpdateFromJson(json);

  Map<String, dynamic> toJson() => _$PrivacySettingsUpdateToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

