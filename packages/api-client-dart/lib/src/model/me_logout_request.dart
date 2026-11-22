//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'me_logout_request.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class MeLogoutRequest {
  /// Returns a new [MeLogoutRequest] instance.
  MeLogoutRequest({

     this.refreshToken,

     this.pushToken,
  });

      /// Zinciri iptal edilecek yenileme jetonu
  @JsonKey(
    
    name: r'refreshToken',
    required: false,
    includeIfNull: false,
  )


  final String? refreshToken;



      /// Bu telefonun FCM jetonu; cihaz kaydı pasifleşir
  @JsonKey(
    
    name: r'pushToken',
    required: false,
    includeIfNull: false,
  )


  final String? pushToken;





    @override
    bool operator ==(Object other) => identical(this, other) || other is MeLogoutRequest &&
      other.refreshToken == refreshToken &&
      other.pushToken == pushToken;

    @override
    int get hashCode =>
        refreshToken.hashCode +
        pushToken.hashCode;

  factory MeLogoutRequest.fromJson(Map<String, dynamic> json) => _$MeLogoutRequestFromJson(json);

  Map<String, dynamic> toJson() => _$MeLogoutRequestToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

