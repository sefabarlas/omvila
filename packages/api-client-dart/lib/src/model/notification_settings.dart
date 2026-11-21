//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/notification_template_setting.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'notification_settings.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class NotificationSettings {
  /// Returns a new [NotificationSettings] instance.
  NotificationSettings({

    required  this.reminderOffsetsMinutes,

    required  this.reviewInviteEnabled,

    required  this.templates,

    required  this.version,
  });

      /// \"Randevu hatırlatma zamanları\" (dakika, randevudan önce); boş dizi = hatırlatma yok. 1440 = \"1 gün önce\", 120 = \"2 saat önce\"
  @JsonKey(
    
    name: r'reminderOffsetsMinutes',
    required: true,
    includeIfNull: false,
  )


  final Set<int> reminderOffsetsMinutes;



      /// \"Değerlendirme daveti\" (randevu tamamlandıktan sonra)
  @JsonKey(
    
    name: r'reviewInviteEnabled',
    required: true,
    includeIfNull: false,
  )


  final bool reviewInviteEnabled;



      /// Salonun ezebildiği SMS şablonları
  @JsonKey(
    
    name: r'templates',
    required: true,
    includeIfNull: false,
  )


  final List<NotificationTemplateSetting> templates;



      /// Kaynak sürümü (ETag ile aynı değer)
  @JsonKey(
    
    name: r'version',
    required: true,
    includeIfNull: false,
  )


  final String version;





    @override
    bool operator ==(Object other) => identical(this, other) || other is NotificationSettings &&
      other.reminderOffsetsMinutes == reminderOffsetsMinutes &&
      other.reviewInviteEnabled == reviewInviteEnabled &&
      other.templates == templates &&
      other.version == version;

    @override
    int get hashCode =>
        reminderOffsetsMinutes.hashCode +
        reviewInviteEnabled.hashCode +
        templates.hashCode +
        version.hashCode;

  factory NotificationSettings.fromJson(Map<String, dynamic> json) => _$NotificationSettingsFromJson(json);

  Map<String, dynamic> toJson() => _$NotificationSettingsToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

