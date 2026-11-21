//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/notification_target.dart';
import 'package:omvila_api/src/model/staff_notification_event.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'in_app_notification.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class InAppNotification {
  /// Returns a new [InAppNotification] instance.
  InAppNotification({

    required  this.id,

    required  this.event,

    required  this.title,

    required  this.body,

    required  this.createdUtc,

    required  this.readUtc,

     this.branchId,

    required  this.target,
  });

      /// Bildirim kimliği
  @JsonKey(
    
    name: r'id',
    required: true,
    includeIfNull: false,
  )


  final String id;



  @JsonKey(
    
    name: r'event',
    required: true,
    includeIfNull: false,
  unknownEnumValue: StaffNotificationEvent.unknownDefaultOpenApi,
  )


  final StaffNotificationEvent event;



      /// Başlık (push başlığıyla aynı dil)
  @JsonKey(
    
    name: r'title',
    required: true,
    includeIfNull: false,
  )


  final String title;



      /// Metin; sağlık bilgisi ve not içeriği yazılmaz
  @JsonKey(
    
    name: r'body',
    required: true,
    includeIfNull: false,
  )


  final String body;



      /// Olayın oluştuğu an (satırdaki \"12:02\", \"Dün\")
  @JsonKey(
    
    name: r'createdUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime createdUtc;



      /// Okundu sayıldığı an; null = okunmadı
  @JsonKey(
    
    name: r'readUtc',
    required: true,
    includeIfNull: true,
  )


  final DateTime? readUtc;



      /// Olayın şubesi; marka düzeyindeki olaylarda (kontör, abonelik) null
  @JsonKey(
    
    name: r'branchId',
    required: false,
    includeIfNull: false,
  )


  final String? branchId;



  @JsonKey(
    
    name: r'target',
    required: true,
    includeIfNull: false,
  )


  final NotificationTarget target;





    @override
    bool operator ==(Object other) => identical(this, other) || other is InAppNotification &&
      other.id == id &&
      other.event == event &&
      other.title == title &&
      other.body == body &&
      other.createdUtc == createdUtc &&
      other.readUtc == readUtc &&
      other.branchId == branchId &&
      other.target == target;

    @override
    int get hashCode =>
        id.hashCode +
        event.hashCode +
        title.hashCode +
        body.hashCode +
        createdUtc.hashCode +
        (readUtc == null ? 0 : readUtc.hashCode) +
        (branchId == null ? 0 : branchId.hashCode) +
        target.hashCode;

  factory InAppNotification.fromJson(Map<String, dynamic> json) => _$InAppNotificationFromJson(json);

  Map<String, dynamic> toJson() => _$InAppNotificationToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

