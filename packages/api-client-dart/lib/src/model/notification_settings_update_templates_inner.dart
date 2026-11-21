//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/customer_notification_event.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'notification_settings_update_templates_inner.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class NotificationSettingsUpdateTemplatesInner {
  /// Returns a new [NotificationSettingsUpdateTemplatesInner] instance.
  NotificationSettingsUpdateTemplatesInner({

    required  this.eventCode,

     this.variant,

    required  this.customBody,
  });

  @JsonKey(
    
    name: r'eventCode',
    required: true,
    includeIfNull: false,
  unknownEnumValue: CustomerNotificationEvent.unknownDefaultOpenApi,
  )


  final CustomerNotificationEvent eventCode;



      /// Şablon varyantı; null = temel metin
  @JsonKey(
    
    name: r'variant',
    required: false,
    includeIfNull: false,
  )


  final String? variant;



      /// Salon metni; null = sistem şablonuna dön
  @JsonKey(
    
    name: r'customBody',
    required: true,
    includeIfNull: true,
  )


  final String? customBody;





    @override
    bool operator ==(Object other) => identical(this, other) || other is NotificationSettingsUpdateTemplatesInner &&
      other.eventCode == eventCode &&
      other.variant == variant &&
      other.customBody == customBody;

    @override
    int get hashCode =>
        eventCode.hashCode +
        (variant == null ? 0 : variant.hashCode) +
        (customBody == null ? 0 : customBody.hashCode);

  factory NotificationSettingsUpdateTemplatesInner.fromJson(Map<String, dynamic> json) => _$NotificationSettingsUpdateTemplatesInnerFromJson(json);

  Map<String, dynamic> toJson() => _$NotificationSettingsUpdateTemplatesInnerToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

