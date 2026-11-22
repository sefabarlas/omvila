//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/device_platform.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'registered_device.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class RegisteredDevice {
  /// Returns a new [RegisteredDevice] instance.
  RegisteredDevice({

    required  this.deviceId,

    required  this.platform,

    required  this.lastSeenUtc,
  });

      /// Cihaz kaydı kimliği
  @JsonKey(
    
    name: r'deviceId',
    required: true,
    includeIfNull: false,
  )


  final String deviceId;



  @JsonKey(
    
    name: r'platform',
    required: true,
    includeIfNull: false,
  unknownEnumValue: DevicePlatform.unknownDefaultOpenApi,
  )


  final DevicePlatform platform;



      /// Son kayıt/yenileme anı
  @JsonKey(
    
    name: r'lastSeenUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime lastSeenUtc;





    @override
    bool operator ==(Object other) => identical(this, other) || other is RegisteredDevice &&
      other.deviceId == deviceId &&
      other.platform == platform &&
      other.lastSeenUtc == lastSeenUtc;

    @override
    int get hashCode =>
        deviceId.hashCode +
        platform.hashCode +
        lastSeenUtc.hashCode;

  factory RegisteredDevice.fromJson(Map<String, dynamic> json) => _$RegisteredDeviceFromJson(json);

  Map<String, dynamic> toJson() => _$RegisteredDeviceToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

