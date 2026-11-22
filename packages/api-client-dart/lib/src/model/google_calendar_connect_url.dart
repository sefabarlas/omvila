//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'google_calendar_connect_url.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class GoogleCalendarConnectUrl {
  /// Returns a new [GoogleCalendarConnectUrl] instance.
  GoogleCalendarConnectUrl({

    required  this.url,

    required  this.state,

    required  this.expiresUtc,
  });

      /// Kullanıcının yönlendirileceği adres
  @JsonKey(
    
    name: r'url',
    required: true,
    includeIfNull: false,
  )


  final String url;



      /// Tek kullanımlık state değeri
  @JsonKey(
    
    name: r'state',
    required: true,
    includeIfNull: false,
  )


  final String state;



      /// state'in geçerlilik sonu
  @JsonKey(
    
    name: r'expiresUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime expiresUtc;





    @override
    bool operator ==(Object other) => identical(this, other) || other is GoogleCalendarConnectUrl &&
      other.url == url &&
      other.state == state &&
      other.expiresUtc == expiresUtc;

    @override
    int get hashCode =>
        url.hashCode +
        state.hashCode +
        expiresUtc.hashCode;

  factory GoogleCalendarConnectUrl.fromJson(Map<String, dynamic> json) => _$GoogleCalendarConnectUrlFromJson(json);

  Map<String, dynamic> toJson() => _$GoogleCalendarConnectUrlToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

