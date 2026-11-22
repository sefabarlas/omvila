//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/waitlist_preferences.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'public_waitlist_join.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PublicWaitlistJoin {
  /// Returns a new [PublicWaitlistJoin] instance.
  PublicWaitlistJoin({

    required  this.preferences,

    required  this.fullName,

    required  this.privacyNoticeAcknowledged,

    required  this.privacyNoticeVersion,
  });

  @JsonKey(
    
    name: r'preferences',
    required: true,
    includeIfNull: false,
  )


  final WaitlistPreferences preferences;



  @JsonKey(
    
    name: r'fullName',
    required: true,
    includeIfNull: false,
  )


  final String fullName;



      /// Aydınlatma metni okundu
  @JsonKey(
    
    name: r'privacyNoticeAcknowledged',
    required: true,
    includeIfNull: false,
  )


  final bool privacyNoticeAcknowledged;



  @JsonKey(
    
    name: r'privacyNoticeVersion',
    required: true,
    includeIfNull: false,
  )


  final String privacyNoticeVersion;





    @override
    bool operator ==(Object other) => identical(this, other) || other is PublicWaitlistJoin &&
      other.preferences == preferences &&
      other.fullName == fullName &&
      other.privacyNoticeAcknowledged == privacyNoticeAcknowledged &&
      other.privacyNoticeVersion == privacyNoticeVersion;

    @override
    int get hashCode =>
        preferences.hashCode +
        fullName.hashCode +
        privacyNoticeAcknowledged.hashCode +
        privacyNoticeVersion.hashCode;

  factory PublicWaitlistJoin.fromJson(Map<String, dynamic> json) => _$PublicWaitlistJoinFromJson(json);

  Map<String, dynamic> toJson() => _$PublicWaitlistJoinToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

