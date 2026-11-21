//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/customer_notification_event.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'notification_template_setting.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class NotificationTemplateSetting {
  /// Returns a new [NotificationTemplateSetting] instance.
  NotificationTemplateSetting({

    required  this.eventCode,

    required  this.variant,

    required  this.channel,

    required  this.systemBody,

    required  this.customBody,

    required  this.variables,

    required  this.requiredVariables,

    required  this.maxLength,
  });

  @JsonKey(
    
    name: r'eventCode',
    required: true,
    includeIfNull: false,
  unknownEnumValue: CustomerNotificationEvent.unknownDefaultOpenApi,
  )


  final CustomerNotificationEvent eventCode;



      /// Aynı olayın ikinci metni (ör. aynı gün hatırlatması same_day); null = temel metin (T-53)
  @JsonKey(
    
    name: r'variant',
    required: true,
    includeIfNull: true,
  )


  final String? variant;



      /// Şablon kanalı; v1'de yalnız SMS metni ezilir, push metni sabittir
  @JsonKey(
    
    name: r'channel',
    required: true,
    includeIfNull: false,
  unknownEnumValue: NotificationTemplateSettingChannelEnum.unknownDefaultOpenApi,
  )


  final NotificationTemplateSettingChannelEnum channel;



      /// Sistem şablonu (değişkenler süslü parantezle)
  @JsonKey(
    
    name: r'systemBody',
    required: true,
    includeIfNull: false,
  )


  final String systemBody;



      /// Salon metni; null = sistem şablonu kullanılıyor
  @JsonKey(
    
    name: r'customBody',
    required: true,
    includeIfNull: true,
  )


  final String? customBody;



      /// Metinde kullanılabilen değişkenler ({salon}, {tarih}…); süslü parantezsiz
  @JsonKey(
    
    name: r'variables',
    required: true,
    includeIfNull: false,
  )


  final List<String> variables;



      /// Salon metninde bulunması zorunlu değişkenler (ör. bağlantı)
  @JsonKey(
    
    name: r'requiredVariables',
    required: true,
    includeIfNull: false,
  )


  final List<String> requiredVariables;



      /// En uzun gerçekçi değerlerle izin verilen GSM-7 Türkçe karakter (1 kontör)
  @JsonKey(
    
    name: r'maxLength',
    required: true,
    includeIfNull: false,
  )


  final int maxLength;





    @override
    bool operator ==(Object other) => identical(this, other) || other is NotificationTemplateSetting &&
      other.eventCode == eventCode &&
      other.variant == variant &&
      other.channel == channel &&
      other.systemBody == systemBody &&
      other.customBody == customBody &&
      other.variables == variables &&
      other.requiredVariables == requiredVariables &&
      other.maxLength == maxLength;

    @override
    int get hashCode =>
        eventCode.hashCode +
        (variant == null ? 0 : variant.hashCode) +
        channel.hashCode +
        systemBody.hashCode +
        (customBody == null ? 0 : customBody.hashCode) +
        variables.hashCode +
        requiredVariables.hashCode +
        maxLength.hashCode;

  factory NotificationTemplateSetting.fromJson(Map<String, dynamic> json) => _$NotificationTemplateSettingFromJson(json);

  Map<String, dynamic> toJson() => _$NotificationTemplateSettingToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

/// Şablon kanalı; v1'de yalnız SMS metni ezilir, push metni sabittir
enum NotificationTemplateSettingChannelEnum {
@JsonValue(r'Sms')
sms(r'Sms'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const NotificationTemplateSettingChannelEnum(this.value);

final String value;

@override
String toString() => value;
}


