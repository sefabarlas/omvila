//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/notification_settings_update_templates_inner.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'notification_settings_update.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class NotificationSettingsUpdate {
  /// Returns a new [NotificationSettingsUpdate] instance.
  NotificationSettingsUpdate({

    required  this.reminderOffsetsMinutes,

    required  this.reviewInviteEnabled,

     this.templates,
  });

      /// Hatırlatma zamanları (dakika)
  @JsonKey(
    
    name: r'reminderOffsetsMinutes',
    required: true,
    includeIfNull: false,
  )


  final Set<int> reminderOffsetsMinutes;



      /// Değerlendirme daveti açık mı
  @JsonKey(
    
    name: r'reviewInviteEnabled',
    required: true,
    includeIfNull: false,
  )


  final bool reviewInviteEnabled;



      /// Değiştirilen şablonlar; gönderilmeyen şablon olduğu gibi kalır
  @JsonKey(
    
    name: r'templates',
    required: false,
    includeIfNull: false,
  )


  final List<NotificationSettingsUpdateTemplatesInner>? templates;





    @override
    bool operator ==(Object other) => identical(this, other) || other is NotificationSettingsUpdate &&
      other.reminderOffsetsMinutes == reminderOffsetsMinutes &&
      other.reviewInviteEnabled == reviewInviteEnabled &&
      other.templates == templates;

    @override
    int get hashCode =>
        reminderOffsetsMinutes.hashCode +
        reviewInviteEnabled.hashCode +
        templates.hashCode;

  factory NotificationSettingsUpdate.fromJson(Map<String, dynamic> json) => _$NotificationSettingsUpdateFromJson(json);

  Map<String, dynamic> toJson() => _$NotificationSettingsUpdateToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

