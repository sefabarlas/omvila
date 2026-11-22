//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'notification_target.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class NotificationTarget {
  /// Returns a new [NotificationTarget] instance.
  NotificationTarget({

    required  this.type,

     this.id,

     this.holdExpiresUtc,
  });

      /// Appointment = randevu yan paneli; Calendar = takvim (id randevu); Today = bugün; Deposit = \"Kapora geldi mi?\"; Reviews = değerlendirmeler; WalkInQueue = kapıda sıra; SmsCredits = Ayarlar → Bildirimler ve SMS; Subscription = Ayarlar → Abonelik
  @JsonKey(
    
    name: r'type',
    required: true,
    includeIfNull: false,
  unknownEnumValue: NotificationTargetTypeEnum.unknownDefaultOpenApi,
  )


  final NotificationTargetTypeEnum type;



      /// Hedef kayıt (randevu, değerlendirme…); ekran düzeyindeki hedeflerde null
  @JsonKey(
    
    name: r'id',
    required: false,
    includeIfNull: false,
  )


  final String? id;



      /// Yalnız staff.approval_needed; panelde canlı kalan süre (\"1 sa 35 dk kaldı\") bundan sayılır. Talep yanıtlandıysa null
  @JsonKey(
    
    name: r'holdExpiresUtc',
    required: false,
    includeIfNull: false,
  )


  final DateTime? holdExpiresUtc;





    @override
    bool operator ==(Object other) => identical(this, other) || other is NotificationTarget &&
      other.type == type &&
      other.id == id &&
      other.holdExpiresUtc == holdExpiresUtc;

    @override
    int get hashCode =>
        type.hashCode +
        (id == null ? 0 : id.hashCode) +
        (holdExpiresUtc == null ? 0 : holdExpiresUtc.hashCode);

  factory NotificationTarget.fromJson(Map<String, dynamic> json) => _$NotificationTargetFromJson(json);

  Map<String, dynamic> toJson() => _$NotificationTargetToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

/// Appointment = randevu yan paneli; Calendar = takvim (id randevu); Today = bugün; Deposit = \"Kapora geldi mi?\"; Reviews = değerlendirmeler; WalkInQueue = kapıda sıra; SmsCredits = Ayarlar → Bildirimler ve SMS; Subscription = Ayarlar → Abonelik
enum NotificationTargetTypeEnum {
@JsonValue(r'Appointment')
appointment(r'Appointment'),
@JsonValue(r'Calendar')
calendar(r'Calendar'),
@JsonValue(r'Today')
today(r'Today'),
@JsonValue(r'Deposit')
deposit(r'Deposit'),
@JsonValue(r'Reviews')
reviews(r'Reviews'),
@JsonValue(r'WalkInQueue')
walkInQueue(r'WalkInQueue'),
@JsonValue(r'SmsCredits')
smsCredits(r'SmsCredits'),
@JsonValue(r'Subscription')
subscription(r'Subscription'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const NotificationTargetTypeEnum(this.value);

final String value;

@override
String toString() => value;
}


