//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'google_calendar_callback_request.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class GoogleCalendarCallbackRequest {
  /// Returns a new [GoogleCalendarCallbackRequest] instance.
  GoogleCalendarCallbackRequest({

    required  this.code,

    required  this.state,
  });

      /// Sağlayıcının yetki kodu
  @JsonKey(
    
    name: r'code',
    required: true,
    includeIfNull: false,
  )


  final String code;



      /// connect-url'deki state
  @JsonKey(
    
    name: r'state',
    required: true,
    includeIfNull: false,
  )


  final String state;





    @override
    bool operator ==(Object other) => identical(this, other) || other is GoogleCalendarCallbackRequest &&
      other.code == code &&
      other.state == state;

    @override
    int get hashCode =>
        code.hashCode +
        state.hashCode;

  factory GoogleCalendarCallbackRequest.fromJson(Map<String, dynamic> json) => _$GoogleCalendarCallbackRequestFromJson(json);

  Map<String, dynamic> toJson() => _$GoogleCalendarCallbackRequestToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

