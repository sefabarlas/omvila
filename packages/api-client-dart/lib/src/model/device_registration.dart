//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/device_platform.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'device_registration.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class DeviceRegistration {
  /// Returns a new [DeviceRegistration] instance.
  DeviceRegistration({

    required  this.platform,

    required  this.pushToken,

    required  this.appVersion,
  });

  @JsonKey(
    
    name: r'platform',
    required: true,
    includeIfNull: false,
  unknownEnumValue: DevicePlatform.unknownDefaultOpenApi,
  )


  final DevicePlatform platform;



      /// FCM kayıt jetonu
  @JsonKey(
    
    name: r'pushToken',
    required: true,
    includeIfNull: false,
  )


  final String pushToken;



      /// Uygulama sürümü
  @JsonKey(
    
    name: r'appVersion',
    required: true,
    includeIfNull: false,
  )


  final String appVersion;





    @override
    bool operator ==(Object other) => identical(this, other) || other is DeviceRegistration &&
      other.platform == platform &&
      other.pushToken == pushToken &&
      other.appVersion == appVersion;

    @override
    int get hashCode =>
        platform.hashCode +
        pushToken.hashCode +
        appVersion.hashCode;

  factory DeviceRegistration.fromJson(Map<String, dynamic> json) => _$DeviceRegistrationFromJson(json);

  Map<String, dynamic> toJson() => _$DeviceRegistrationToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

