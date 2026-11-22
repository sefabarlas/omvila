//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'manage_record_onboarding_event_request.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ManageRecordOnboardingEventRequest {
  /// Returns a new [ManageRecordOnboardingEventRequest] instance.
  ManageRecordOnboardingEventRequest({

    required  this.event,
  });

  @JsonKey(
    
    name: r'event',
    required: true,
    includeIfNull: false,
  unknownEnumValue: ManageRecordOnboardingEventRequestEventEnum.unknownDefaultOpenApi,
  )


  final ManageRecordOnboardingEventRequestEventEnum event;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ManageRecordOnboardingEventRequest &&
      other.event == event;

    @override
    int get hashCode =>
        event.hashCode;

  factory ManageRecordOnboardingEventRequest.fromJson(Map<String, dynamic> json) => _$ManageRecordOnboardingEventRequestFromJson(json);

  Map<String, dynamic> toJson() => _$ManageRecordOnboardingEventRequestToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

enum ManageRecordOnboardingEventRequestEventEnum {
@JsonValue(r'BookingLinkCopied')
bookingLinkCopied(r'BookingLinkCopied'),
@JsonValue(r'QrDownloaded')
qrDownloaded(r'QrDownloaded'),
@JsonValue(r'StoryImageDownloaded')
storyImageDownloaded(r'StoryImageDownloaded'),
@JsonValue(r'SoloConfirmed')
soloConfirmed(r'SoloConfirmed'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const ManageRecordOnboardingEventRequestEventEnum(this.value);

final String value;

@override
String toString() => value;
}


