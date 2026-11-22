//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'google_calendar_connection.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class GoogleCalendarConnection {
  /// Returns a new [GoogleCalendarConnection] instance.
  GoogleCalendarConnection({

    required  this.status,

     this.accountEmailMasked,

     this.connectedUtc,

     this.lastSyncedUtc,
  });

      /// NotConnected = bağlantı yok; TokenExpired = \"Google Takvim bağlantısı koptu\"
  @JsonKey(
    
    name: r'status',
    required: true,
    includeIfNull: false,
  unknownEnumValue: GoogleCalendarConnectionStatusEnum.unknownDefaultOpenApi,
  )


  final GoogleCalendarConnectionStatusEnum status;



      /// Bağlı Google hesabı (maskeli)
  @JsonKey(
    
    name: r'accountEmailMasked',
    required: false,
    includeIfNull: false,
  )


  final String? accountEmailMasked;



      /// Bağlandığı an
  @JsonKey(
    
    name: r'connectedUtc',
    required: false,
    includeIfNull: false,
  )


  final DateTime? connectedUtc;



      /// Son başarılı aktarım
  @JsonKey(
    
    name: r'lastSyncedUtc',
    required: false,
    includeIfNull: false,
  )


  final DateTime? lastSyncedUtc;





    @override
    bool operator ==(Object other) => identical(this, other) || other is GoogleCalendarConnection &&
      other.status == status &&
      other.accountEmailMasked == accountEmailMasked &&
      other.connectedUtc == connectedUtc &&
      other.lastSyncedUtc == lastSyncedUtc;

    @override
    int get hashCode =>
        status.hashCode +
        (accountEmailMasked == null ? 0 : accountEmailMasked.hashCode) +
        (connectedUtc == null ? 0 : connectedUtc.hashCode) +
        (lastSyncedUtc == null ? 0 : lastSyncedUtc.hashCode);

  factory GoogleCalendarConnection.fromJson(Map<String, dynamic> json) => _$GoogleCalendarConnectionFromJson(json);

  Map<String, dynamic> toJson() => _$GoogleCalendarConnectionToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

/// NotConnected = bağlantı yok; TokenExpired = \"Google Takvim bağlantısı koptu\"
enum GoogleCalendarConnectionStatusEnum {
@JsonValue(r'NotConnected')
notConnected(r'NotConnected'),
@JsonValue(r'Active')
active(r'Active'),
@JsonValue(r'TokenExpired')
tokenExpired(r'TokenExpired'),
@JsonValue(r'Revoked')
revoked(r'Revoked'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const GoogleCalendarConnectionStatusEnum(this.value);

final String value;

@override
String toString() => value;
}


