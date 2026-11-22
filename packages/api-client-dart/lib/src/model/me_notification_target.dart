//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'me_notification_target.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class MeNotificationTarget {
  /// Returns a new [MeNotificationTarget] instance.
  MeNotificationTarget({

    required  this.type,

     this.appointmentId,

     this.accessCode,

     this.brandSlug,
  });

      /// Appointment = RandevuDetay (appointmentId); WaitlistOffer = BeklemeTeklif (accessCode ile public waitlist-offer uçları); Review = Degerlendir (appointmentId); Booking = rezervasyon akışı (brandSlug; \"Başka bir saat seçin\")
  @JsonKey(
    
    name: r'type',
    required: true,
    includeIfNull: false,
  unknownEnumValue: MeNotificationTargetTypeEnum.unknownDefaultOpenApi,
  )


  final MeNotificationTargetTypeEnum type;



  @JsonKey(
    
    name: r'appointmentId',
    required: false,
    includeIfNull: false,
  )


  final String? appointmentId;



      /// Bekleme teklifi kodu (/b/{code})
  @JsonKey(
    
    name: r'accessCode',
    required: false,
    includeIfNull: false,
  )


  final String? accessCode;



  @JsonKey(
    
    name: r'brandSlug',
    required: false,
    includeIfNull: false,
  )


  final String? brandSlug;





    @override
    bool operator ==(Object other) => identical(this, other) || other is MeNotificationTarget &&
      other.type == type &&
      other.appointmentId == appointmentId &&
      other.accessCode == accessCode &&
      other.brandSlug == brandSlug;

    @override
    int get hashCode =>
        type.hashCode +
        (appointmentId == null ? 0 : appointmentId.hashCode) +
        (accessCode == null ? 0 : accessCode.hashCode) +
        (brandSlug == null ? 0 : brandSlug.hashCode);

  factory MeNotificationTarget.fromJson(Map<String, dynamic> json) => _$MeNotificationTargetFromJson(json);

  Map<String, dynamic> toJson() => _$MeNotificationTargetToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

/// Appointment = RandevuDetay (appointmentId); WaitlistOffer = BeklemeTeklif (accessCode ile public waitlist-offer uçları); Review = Degerlendir (appointmentId); Booking = rezervasyon akışı (brandSlug; \"Başka bir saat seçin\")
enum MeNotificationTargetTypeEnum {
@JsonValue(r'Appointment')
appointment(r'Appointment'),
@JsonValue(r'WaitlistOffer')
waitlistOffer(r'WaitlistOffer'),
@JsonValue(r'Review')
review(r'Review'),
@JsonValue(r'Booking')
booking(r'Booking'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const MeNotificationTargetTypeEnum(this.value);

final String value;

@override
String toString() => value;
}


