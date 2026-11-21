//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'my_notification_preferences.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class MyNotificationPreferences {
  /// Returns a new [MyNotificationPreferences] instance.
  MyNotificationPreferences({

    required  this.newOnlineAppointment,

    required  this.cancellationAndReschedule,

    required  this.customerCheckedIn,

    required  this.walkInQueue,

    required  this.newReview,

    required  this.dailySummary,

     this.dailySummaryLocalTime,

    required  this.quietHoursEnabled,

     this.quietHoursStartLocal,

     this.quietHoursEndLocal,

     this.version,
  });

      /// \"Yeni online randevu · Onay bekleyenler dahil\"
  @JsonKey(
    
    name: r'newOnlineAppointment',
    required: true,
    includeIfNull: false,
  )


  final bool newOnlineAppointment;



      /// \"İptal ve saat değişikliği · Sizin randevularınızda\"
  @JsonKey(
    
    name: r'cancellationAndReschedule',
    required: true,
    includeIfNull: false,
  )


  final bool cancellationAndReschedule;



      /// \"Müşteri geldi · Resepsiyon check-in yapınca\"
  @JsonKey(
    
    name: r'customerCheckedIn',
    required: true,
    includeIfNull: false,
  )


  final bool customerCheckedIn;



      /// \"Kapıda sıra · Sıra size gelince\"
  @JsonKey(
    
    name: r'walkInQueue',
    required: true,
    includeIfNull: false,
  )


  final bool walkInQueue;



      /// \"Yeni değerlendirme · Sizi puanlayanlar\"
  @JsonKey(
    
    name: r'newReview',
    required: true,
    includeIfNull: false,
  )


  final bool newReview;



      /// \"Gün sonu özeti\"
  @JsonKey(
    
    name: r'dailySummary',
    required: true,
    includeIfNull: false,
  )


  final bool dailySummary;



      /// Gün sonu özetinin gönderildiği yerel saat (\"Her akşam 20:00\"); platform ayarı
  @JsonKey(
    
    name: r'dailySummaryLocalTime',
    required: false,
    includeIfNull: false,
  )


  final String? dailySummaryLocalTime;



      /// \"Sessiz saatler\"
  @JsonKey(
    
    name: r'quietHoursEnabled',
    required: true,
    includeIfNull: false,
  )


  final bool quietHoursEnabled;



      /// Sessiz saat başlangıcı (QuietHoursLocal, şube yerel saati)
  @JsonKey(
    
    name: r'quietHoursStartLocal',
    required: false,
    includeIfNull: false,
  )


  final String? quietHoursStartLocal;



      /// Sessiz saat bitişi
  @JsonKey(
    
    name: r'quietHoursEndLocal',
    required: false,
    includeIfNull: false,
  )


  final String? quietHoursEndLocal;



      /// Kaynak sürümü (ETag ile aynı)
  @JsonKey(
    
    name: r'version',
    required: false,
    includeIfNull: false,
  )


  final String? version;





    @override
    bool operator ==(Object other) => identical(this, other) || other is MyNotificationPreferences &&
      other.newOnlineAppointment == newOnlineAppointment &&
      other.cancellationAndReschedule == cancellationAndReschedule &&
      other.customerCheckedIn == customerCheckedIn &&
      other.walkInQueue == walkInQueue &&
      other.newReview == newReview &&
      other.dailySummary == dailySummary &&
      other.dailySummaryLocalTime == dailySummaryLocalTime &&
      other.quietHoursEnabled == quietHoursEnabled &&
      other.quietHoursStartLocal == quietHoursStartLocal &&
      other.quietHoursEndLocal == quietHoursEndLocal &&
      other.version == version;

    @override
    int get hashCode =>
        newOnlineAppointment.hashCode +
        cancellationAndReschedule.hashCode +
        customerCheckedIn.hashCode +
        walkInQueue.hashCode +
        newReview.hashCode +
        dailySummary.hashCode +
        dailySummaryLocalTime.hashCode +
        quietHoursEnabled.hashCode +
        quietHoursStartLocal.hashCode +
        quietHoursEndLocal.hashCode +
        version.hashCode;

  factory MyNotificationPreferences.fromJson(Map<String, dynamic> json) => _$MyNotificationPreferencesFromJson(json);

  Map<String, dynamic> toJson() => _$MyNotificationPreferencesToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

