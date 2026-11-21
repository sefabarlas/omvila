//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'me_auth_result.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class MeAuthResult {
  /// Returns a new [MeAuthResult] instance.
  MeAuthResult({

    required  this.accessToken,

    required  this.expiresInSeconds,

    required  this.refreshToken,

    required  this.isNewAccount,
  });

      /// Erişim jetonu
  @JsonKey(
    
    name: r'accessToken',
    required: true,
    includeIfNull: false,
  )


  final String accessToken;



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



      /// Uygulamaya ilk giriş (HasAppAccount bu istekle açıldı); BildirimIzni ve MainBos akışı için
  @JsonKey(
    
    name: r'isNewAccount',
    required: true,
    includeIfNull: false,
  )


  final bool isNewAccount;





    @override
    bool operator ==(Object other) => identical(this, other) || other is MeAuthResult &&
      other.accessToken == accessToken &&
      other.expiresInSeconds == expiresInSeconds &&
      other.refreshToken == refreshToken &&
      other.isNewAccount == isNewAccount;

    @override
    int get hashCode =>
        accessToken.hashCode +
        expiresInSeconds.hashCode +
        refreshToken.hashCode +
        isNewAccount.hashCode;

  factory MeAuthResult.fromJson(Map<String, dynamic> json) => _$MeAuthResultFromJson(json);

  Map<String, dynamic> toJson() => _$MeAuthResultToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

