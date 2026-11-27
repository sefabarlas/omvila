//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/industry_code.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'signup_request.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class SignupRequest {
  /// Returns a new [SignupRequest] instance.
  SignupRequest({

    required  this.signupToken,

    required  this.businessName,

    required  this.industryCode,

     this.ownerFullName,

    required  this.acceptedTerms,

    required  this.privacyNoticeVersion,
  });

  @JsonKey(
    
    name: r'signupToken',
    required: true,
    includeIfNull: false,
  )


  final String signupToken;



  @JsonKey(
    
    name: r'businessName',
    required: true,
    includeIfNull: false,
  )


  final String businessName;



  @JsonKey(
    
    name: r'industryCode',
    required: true,
    includeIfNull: false,
  unknownEnumValue: IndustryCode.unknownDefaultOpenApi,
  )


  final IndustryCode industryCode;



      /// Kayıt ekranındaki \"Adınız soyadınız\": işletme sahibinin görünen adı (User.fullName ve sahip personel kaydı). Sunucu zorunlu tutar (T-130): eksik, boş veya kurala uymayan değer 400 validation_failed (errors.ownerFullName) döner. Şemada zorunlu işaretlenmez; sözleşme geriye uyumlu kalır. Tek değerdir, ad ve soyad ayrılmaz. 2-120 karakter; Latin harfleri (Türkçe harfler dahil), kesme işareti, tire ve boşluk; harfle başlar, rakam kabul edilmez. Sunucu baştaki ve sondaki boşluğu kırpar, aradaki çoklu boşluğu teke indirir; sınırlar bu hâle uygulanır. 
  @JsonKey(
    
    name: r'ownerFullName',
    required: false,
    includeIfNull: false,
  )


  final String? ownerFullName;



      /// Kullanım koşulları ve aydınlatma metni okundu
  @JsonKey(
    
    name: r'acceptedTerms',
    required: true,
    includeIfNull: false,
  )


  final bool acceptedTerms;



  @JsonKey(
    
    name: r'privacyNoticeVersion',
    required: true,
    includeIfNull: false,
  )


  final String privacyNoticeVersion;





    @override
    bool operator ==(Object other) => identical(this, other) || other is SignupRequest &&
      other.signupToken == signupToken &&
      other.businessName == businessName &&
      other.industryCode == industryCode &&
      other.ownerFullName == ownerFullName &&
      other.acceptedTerms == acceptedTerms &&
      other.privacyNoticeVersion == privacyNoticeVersion;

    @override
    int get hashCode =>
        signupToken.hashCode +
        businessName.hashCode +
        industryCode.hashCode +
        ownerFullName.hashCode +
        acceptedTerms.hashCode +
        privacyNoticeVersion.hashCode;

  factory SignupRequest.fromJson(Map<String, dynamic> json) => _$SignupRequestFromJson(json);

  Map<String, dynamic> toJson() => _$SignupRequestToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

