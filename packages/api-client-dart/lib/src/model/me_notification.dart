//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/me_brand_ref.dart';
import 'package:omvila_api/src/model/me_notification_target.dart';
import 'package:omvila_api/src/model/me_notification_event.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'me_notification.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class MeNotification {
  /// Returns a new [MeNotification] instance.
  MeNotification({

    required  this.id,

    required  this.event,

    required  this.title,

    required  this.body,

    required  this.createdUtc,

    required  this.readUtc,

    required  this.brand,

    required  this.target,
  });

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
  unknownEnumValue: MeNotificationEvent.unknownDefaultOpenApi,
  )


  final MeNotificationEvent event;



  @JsonKey(
    
    name: r'title',
    required: true,
    includeIfNull: false,
  )


  final String title;



  @JsonKey(
    
    name: r'body',
    required: true,
    includeIfNull: false,
  )


  final String body;



  @JsonKey(
    
    name: r'createdUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime createdUtc;



  @JsonKey(
    
    name: r'readUtc',
    required: true,
    includeIfNull: true,
  )


  final DateTime? readUtc;



  @JsonKey(
    
    name: r'brand',
    required: true,
    includeIfNull: true,
  )


  final MeBrandRef? brand;



  @JsonKey(
    
    name: r'target',
    required: true,
    includeIfNull: false,
  )


  final MeNotificationTarget target;





    @override
    bool operator ==(Object other) => identical(this, other) || other is MeNotification &&
      other.id == id &&
      other.event == event &&
      other.title == title &&
      other.body == body &&
      other.createdUtc == createdUtc &&
      other.readUtc == readUtc &&
      other.brand == brand &&
      other.target == target;

    @override
    int get hashCode =>
        id.hashCode +
        event.hashCode +
        title.hashCode +
        body.hashCode +
        createdUtc.hashCode +
        (readUtc == null ? 0 : readUtc.hashCode) +
        (brand == null ? 0 : brand.hashCode) +
        target.hashCode;

  factory MeNotification.fromJson(Map<String, dynamic> json) => _$MeNotificationFromJson(json);

  Map<String, dynamic> toJson() => _$MeNotificationToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

