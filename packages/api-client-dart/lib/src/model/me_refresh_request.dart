//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'me_refresh_request.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class MeRefreshRequest {
  /// Returns a new [MeRefreshRequest] instance.
  MeRefreshRequest({

    required  this.refreshToken,
  });

      /// Saklanan yenileme jetonu
  @JsonKey(
    
    name: r'refreshToken',
    required: true,
    includeIfNull: false,
  )


  final String refreshToken;





    @override
    bool operator ==(Object other) => identical(this, other) || other is MeRefreshRequest &&
      other.refreshToken == refreshToken;

    @override
    int get hashCode =>
        refreshToken.hashCode;

  factory MeRefreshRequest.fromJson(Map<String, dynamic> json) => _$MeRefreshRequestFromJson(json);

  Map<String, dynamic> toJson() => _$MeRefreshRequestToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

