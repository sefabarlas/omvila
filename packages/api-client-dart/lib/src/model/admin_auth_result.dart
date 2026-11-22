//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_auth_result.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminAuthResult {
  /// Returns a new [AdminAuthResult] instance.
  AdminAuthResult({

    required  this.accessToken,

    required  this.expiresInSeconds,

    required  this.refreshToken,
  });

      /// Erişim jetonu
  @JsonKey(
    
    name: r'accessToken',
    required: true,
    includeIfNull: false,
  )


  final String accessToken;



      /// Erişim jetonunun ömrü (sn)
  @JsonKey(
    
    name: r'expiresInSeconds',
    required: true,
    includeIfNull: false,
  )


  final int expiresInSeconds;



      /// Yenileme jetonu (her kullanımda döndürülür)
  @JsonKey(
    
    name: r'refreshToken',
    required: true,
    includeIfNull: false,
  )


  final String refreshToken;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminAuthResult &&
      other.accessToken == accessToken &&
      other.expiresInSeconds == expiresInSeconds &&
      other.refreshToken == refreshToken;

    @override
    int get hashCode =>
        accessToken.hashCode +
        expiresInSeconds.hashCode +
        refreshToken.hashCode;

  factory AdminAuthResult.fromJson(Map<String, dynamic> json) => _$AdminAuthResultFromJson(json);

  Map<String, dynamic> toJson() => _$AdminAuthResultToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

