//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'bank_account.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class BankAccount {
  /// Returns a new [BankAccount] instance.
  BankAccount({

    required  this.configured,

    required  this.isActive,

     this.accountHolderName,

     this.bankName,

     this.ibanMasked,

     this.ibanLast4,

    required  this.transferCodePrefix,

    required  this.transferWindowMinutes,

     this.updatedUtc,

    required  this.version,
  });

      /// Hesap tanımlı mı (false = \"Kapora alınamaz · IBAN ekle\")
  @JsonKey(
    
    name: r'configured',
    required: true,
    includeIfNull: false,
  )


  final bool configured;



      /// Hesap kaporada kullanılıyor mu; false iken kapora istenmez (BrandSettings.depositAvailable ile aynı)
  @JsonKey(
    
    name: r'isActive',
    required: true,
    includeIfNull: false,
  )


  final bool isActive;



      /// Alıcı adı (müşterinin bankasında görünecek ad)
  @JsonKey(
    
    name: r'accountHolderName',
    required: false,
    includeIfNull: false,
  )


  final String? accountHolderName;



      /// Banka
  @JsonKey(
    
    name: r'bankName',
    required: false,
    includeIfNull: false,
  )


  final String? bankName;



      /// Maskeli IBAN: ilk 4 karakter + \" •••• \" + son 6 hane \"dddd dd\" gruplamasıyla (Ayarlar özetindeki \"TR12 •••• 6789 01\"). İstemci dar alanda (AyarlarOdeme \"Müşteri ne görür?\") \"••••\" yerine \"…\" gösterebilir. 
  @JsonKey(
    
    name: r'ibanMasked',
    required: false,
    includeIfNull: false,
  )


  final String? ibanMasked;



      /// IBAN'ın son 4 hanesi (listelerde)
  @JsonKey(
    
    name: r'ibanLast4',
    required: false,
    includeIfNull: false,
  )


  final String? ibanLast4;



      /// Açıklama kodu öneki (\"OMV-7K2X\"); varsayılan OMV
  @JsonKey(
    
    name: r'transferCodePrefix',
    required: true,
    includeIfNull: false,
  )


  final String transferCodePrefix;



      /// Havale süresi; dolarsa randevu otomatik iptal olur (A5). Varsayılan 120
  @JsonKey(
    
    name: r'transferWindowMinutes',
    required: true,
    includeIfNull: false,
  unknownEnumValue: BankAccountTransferWindowMinutesEnum.unknownDefaultOpenApi,
  )


  final BankAccountTransferWindowMinutesEnum transferWindowMinutes;



      /// Son kayıt anı
  @JsonKey(
    
    name: r'updatedUtc',
    required: false,
    includeIfNull: false,
  )


  final DateTime? updatedUtc;



      /// Kaynak sürümü (ETag ile aynı değer)
  @JsonKey(
    
    name: r'version',
    required: true,
    includeIfNull: false,
  )


  final String version;





    @override
    bool operator ==(Object other) => identical(this, other) || other is BankAccount &&
      other.configured == configured &&
      other.isActive == isActive &&
      other.accountHolderName == accountHolderName &&
      other.bankName == bankName &&
      other.ibanMasked == ibanMasked &&
      other.ibanLast4 == ibanLast4 &&
      other.transferCodePrefix == transferCodePrefix &&
      other.transferWindowMinutes == transferWindowMinutes &&
      other.updatedUtc == updatedUtc &&
      other.version == version;

    @override
    int get hashCode =>
        configured.hashCode +
        isActive.hashCode +
        accountHolderName.hashCode +
        bankName.hashCode +
        ibanMasked.hashCode +
        ibanLast4.hashCode +
        transferCodePrefix.hashCode +
        transferWindowMinutes.hashCode +
        updatedUtc.hashCode +
        version.hashCode;

  factory BankAccount.fromJson(Map<String, dynamic> json) => _$BankAccountFromJson(json);

  Map<String, dynamic> toJson() => _$BankAccountToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

/// Havale süresi; dolarsa randevu otomatik iptal olur (A5). Varsayılan 120
enum BankAccountTransferWindowMinutesEnum {
@JsonValue(60)
number60('60'),
@JsonValue(120)
number120('120'),
@JsonValue(240)
number240('240'),
@JsonValue(11184809)
unknownDefaultOpenApi('11184809');

const BankAccountTransferWindowMinutesEnum(this.value);

final String value;

@override
String toString() => value;
}


